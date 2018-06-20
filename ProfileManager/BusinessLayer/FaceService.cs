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


    }
}
