using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using Microsoft.Extensions.Configuration;

namespace ProfileManager.BusinessLayer
{
    public class FaceService: IFaceService
    {
        private readonly IConfiguration Configuration;
        private string Key { get; set; }
        private string Url { get; set; }

        public FaceService(IConfiguration configuration)
        {
            this.Configuration = configuration;
            this.Key = this.Configuration.GetValue<string>("FaceApiKey");
            this.Url = this.Configuration.GetValue<string>("FaceApiUrl");
        }

        public async Task<Face[]> FindFaces(string imageUrl)
        {
            var faceClient = new FaceServiceClient(this.Key, this.Url);            
            return await faceClient.DetectAsync(imageUrl, true, false);      
        }

        public async Task<List<FaceVerificationResult>> VerifyFacesMatch(string knownPersonImageUrl, string toVerifyImageUrl)
        {
            var matchResults = new List<FaceVerificationResult>();


            var faceClient = new FaceServiceClient(this.Key, this.Url);

            var knownFaces = await faceClient.DetectAsync(knownPersonImageUrl, true, false);
            if (knownFaces == null || knownFaces.Length != 1)
            {
                // Not exactly one face in known photo, no match possible
                return matchResults;
            }
            var knownFace = knownFaces[0];


            var potentialMatches = await faceClient.DetectAsync(toVerifyImageUrl, true, false);
            if (potentialMatches == null || potentialMatches.Length < 1)
            {
                // no faces found in verify photo, no match possible
                return matchResults;
            }
            foreach(var toVerifyFace in potentialMatches)
            {
                var verifyResult = new FaceVerificationResult();
                verifyResult.FaceToVerify = toVerifyFace;
                verifyResult.KnownFace = knownFace;

                var verification = await faceClient.VerifyAsync(knownFace.FaceId, toVerifyFace.FaceId);
                if (verification != null)
                {
                    verifyResult.IsMatch = verification.IsIdentical;
                    verifyResult.ConfidenceLevel = verification.Confidence;
                }

                matchResults.Add(verifyResult);
            }

            return matchResults;
        }


    }
}
