using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Face.Contract;

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
        public int Top { get; set; }
        public int Left { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

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
            Height = rect.Height;
            Width = rect.Width;
            Top = rect.Top;
            Left = rect.Left;
        }
    }

    public class EmployeePhotoViewModel
    {
        public string Url { get; set; }
        public string AltText { get; set; }

        public List<FaceMarker> Faces { get; set; }

        public string ErrorMessage { get; set; }

        public EmployeePhotoViewModel()
        {
            this.ErrorMessage = string.Empty;
            this.Faces = new List<FaceMarker>();
        }

        public void MapFaces(Face[] faceData)
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
            }
        }
    }
}
