using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ProfileManager.BusinessLayer
{
    public interface IStorageService
    {

        string FullPhotoUrl(int id, string fileName);

        string NoPhotoUrl();

        Task<WritePhotoResponse> WriteEmployeePhoto(int employeeId, string photoName, IFormFile postedFile);

        Task<bool> WriteFile(string filePath, IFormFile postedFile);

        string FixUpPathing(string input);
    }
}
