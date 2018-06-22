using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Face.Contract;

namespace ProfileManager.BusinessLayer
{
    public class FaceVerificationResult
    {
        public bool IsMatch { get; set; }
        public double ConfidenceLevel { get; set; }

        public Face KnownFace { get; set; }
        public Face FaceToVerify { get; set; }

        public FaceVerificationResult()
        {
            IsMatch = false;
            ConfidenceLevel = 0;
        }
    }
}
