using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Face.Contract;
using ProfileManager.BusinessLayer;

namespace ProfileManager.Models
{
    public enum FaceMarkerType
    {
        Generic = 0,
        Error = 1,
        Verified = 2
    }
   
    public class FaceMarker
    {
        public FaceRectangle OverlayPosition { get; set; }
        
        public FaceRectangle ScaledOverlayPosition(float widthScaler, float heightScaler)
        {
            var scaledRectangle = new FaceRectangle();
            scaledRectangle.Top = (int)(OverlayPosition.Top * heightScaler);
            scaledRectangle.Height = (int)(OverlayPosition.Height * heightScaler);
            scaledRectangle.Left = (int)(OverlayPosition.Left * widthScaler);
            scaledRectangle.Width = (int)(OverlayPosition.Width * widthScaler);
            return scaledRectangle;
        }

        public FaceMarkerType OverlayType { get; set; }
        public string Message { get; set; }

        public FaceMarker()
        {
            OverlayType = FaceMarkerType.Generic;
            Message = "";
        }        

        public FaceMarker(FaceRectangle rect)
        {
            OverlayType = FaceMarkerType.Generic;
            Message = "";
            OverlayPosition = rect;
        }
    }

    public class EmployeePhotoViewModel
    {
        public string Url { get; set; }
        public string AltText { get; set; }

        public int OriginalImageWidth { get; set; }
        public int OriginalImageHeight { get; set; }

        public float WidthScaleFactor(float currentWidth)
        {
            return currentWidth / OriginalImageWidth;
        }
        public float HeightScaleFactor(float currentHeight)
        {
            return currentHeight / OriginalImageHeight;
        }

        public List<FaceMarker> Faces { get; set; }

        public string ErrorMessage { get; set; }

        public EmployeePhotoViewModel()
        {
            this.ErrorMessage = string.Empty;
            this.Faces = new List<FaceMarker>();
        }

        public EmployeePhotoViewModel(Employee employee, IStorageService storageService)
        {
            this.ErrorMessage = string.Empty;
            this.Faces = new List<FaceMarker>();
            this.Url = storageService.FullPhotoUrl(employee.Id, employee.PhotoFileName);
            this.AltText = employee.FirstName + " " + employee.LastName;
            this.OriginalImageWidth = employee.PhotoWidth;
            this.OriginalImageHeight = employee.PhotoHeight;
        }

        public void MapFacesForValidation(Face[] faceData)
        {
            if (faceData.Length < 1)
            {
                // no faces
                this.ErrorMessage = "No Faces Detected";
            }
            else if (faceData.Length > 1)
            {
                // too many faces
                this.ErrorMessage = "Many Faces Detected";
                foreach(var face in faceData)
                {
                    var marker = new FaceMarker(face.FaceRectangle)
                    {
                        OverlayType = FaceMarkerType.Error
                    };
                    this.Faces.Add(marker);
                }
            } else
            {
                // Just one face
                var marker = new FaceMarker(faceData[0].FaceRectangle);
                this.Faces.Add(marker);
            }
        }

        public void MapFacesForVerification(List<FaceVerificationResult> verificationData)
        {
            foreach(var verifyResult in verificationData)
            {
                var marker = new FaceMarker(verifyResult.FaceToVerify.FaceRectangle);
                if (verifyResult.IsMatch)
                {                    
                    marker.OverlayType = FaceMarkerType.Verified;
                    marker.Message = this.AltText;
                }
                else
                {
                    marker.OverlayType = FaceMarkerType.Error;
                }
                this.Faces.Add(marker);
            }
        }


    }
}
