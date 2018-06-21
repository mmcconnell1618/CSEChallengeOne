using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.AspNetCore.Http;
using System.Drawing;

namespace ProfileManager.BusinessLayer
{
    public class StorageService: IStorageService
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
            if (id < 1 || fileName.Trim().Length < 1) return NoPhotoUrl();
            
            return $"{BaseUrl}/{ContainerName}/{id.ToString()}/{fileName}"; 
        }

        public string NoPhotoUrl()
        {
            return $"{BaseUrl}/{ContainerName}/NoPhoto.png";
        }

        public async Task<WritePhotoResponse> WriteEmployeePhoto(int employeeId, string photoName, IFormFile postedFile)
        {
            var response = new WritePhotoResponse();

            // Get Image Dimensions
            using (var photoStream = postedFile.OpenReadStream())
            {
                var uploadedImage = Image.FromStream(photoStream);
                response.ImageHeight = uploadedImage.Height;
                response.ImageWidth = uploadedImage.Width;

                // Reset Stream so it can be uploaded to storage
                photoStream.Seek(0, System.IO.SeekOrigin.Begin);
            }                

            // Upload file to Blob Storage
            string filePath = $"{employeeId.ToString()}/{photoName}";
            response.Success = await WriteFile(filePath, postedFile);

            return response;
        }

        public async Task<bool> WriteFile(string filePath, IFormFile postedFile)
        {
            SetupConnection();
            CloudBlockBlob blockBlob = storageContainer.GetBlockBlobReference(FixUpPathing(filePath));        
            await blockBlob.UploadFromStreamAsync(postedFile.OpenReadStream());
            return true;
        }

        public string FixUpPathing(string input)

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
