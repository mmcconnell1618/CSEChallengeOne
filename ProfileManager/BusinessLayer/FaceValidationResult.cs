using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;

namespace ProfileManager.BusinessLayer
{
    public class FaceValidationResult
    {
        public FaceValidationResult()
        {
            this.Faces = new Face[0];
        }

        public int CountOfFaces
        {
            get
            {
                return Faces.Length;
            }
        }
        public Face[] Faces { get; set; }
        public bool IsValidEmployeePhoto
        {
            get
            {
                return CountOfFaces == 1;
            }
        }
    }
}
