using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.AspNetCore.Http;

namespace ProfileManager.BusinessLayer
{
    public class StorageService
    {
        // Configuration Settings
        private readonly IConfiguration Configuration;
        private string ContainerName { get; set; }
        private string BaseUrl { get; set; }
        private string ConnectionString { get; set; }

        // Cloud SDK Objects
        private CloudStorageAccount storageAccount = null;
        private CloudBlobClient storageBlobClient = null;
        private CloudBlobContainer storageContainer = null;

        private bool isConnected = false;

        public StorageService(IConfiguration configuration)
        {
            this.Configuration = configuration;
            BaseUrl = Configuration.GetValue<string>("StorageUrl").TrimEnd('/');
            ContainerName = Configuration.GetValue<string>("StorageContainerName");
            ConnectionString = Configuration.GetValue<string>("StorageConnectionString");         
        }

        public void SetupConnection()
        {
            if (isConnected == false)
            {
                // Connect to storage Account
                storageAccount = CloudStorageAccount.Parse(ConnectionString);
                storageBlobClient = storageAccount.CreateCloudBlobClient();
                storageContainer = storageBlobClient.GetContainerReference(ContainerName);
                storageContainer.CreateIfNotExistsAsync();
                storageContainer.SetPermissionsAsync(
                        new BlobContainerPermissions
                        {
                            PublicAccess = BlobContainerPublicAccessType.Blob
                        });
                isConnected = true;
            }
        }

        public string FullPhotoUrl(int id, string fileName)
        {
            return $"{BaseUrl}/{ContainerName}/{id.ToString()}/{fileName}"; 
        }

        public async Task<bool> WriteEmployeePhoto(int employeeId, string photoName, IFormFile postedFile)
        {
            string filePath = $"{employeeId.ToString()}/{photoName}";
            return await WriteFile(filePath, postedFile);
        }

        public async Task<bool> WriteFile(string filePath, IFormFile postedFile)
        {
            SetupConnection();
            CloudBlockBlob blockBlob = storageContainer.GetBlockBlobReference(FixUpPathing(filePath));        
            await blockBlob.UploadFromStreamAsync(postedFile.OpenReadStream());
            return true;
        }

        public static string FixUpPathing(string input)

        {
            string output = input.Trim();

            // Make Slash instead of backslash
            output = output.Replace('\\', '/');

            // Remove Doubled Slashes
            output = output.Replace("//", "/");
            output = output.TrimStart('/');
            output = output.TrimEnd('/');
            return output;
        }
    }
}
