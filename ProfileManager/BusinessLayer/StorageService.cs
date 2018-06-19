using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ProfileManager.BusinessLayer
{
    public class StorageService
    {
        private readonly IConfiguration Configuration;

        public StorageService(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public string FullPhotoUrl(int id, string fileName)
        {
            string url = Configuration.GetValue<string>("StorageUrl")
                + id.ToString() + "/" + fileName;
            return url;
        }
    }
}
