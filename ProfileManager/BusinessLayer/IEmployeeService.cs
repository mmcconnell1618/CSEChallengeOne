using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProfileManager.Models;

namespace ProfileManager.BusinessLayer
{
    public interface IEmployeeService
    {
        int CountOfEmployees();

        Task<List<Employee>> ListEmployeesAsync();

        Task<Employee> FindByIdAsync(int id);

        Task<bool> CreateAsync(Employee employee);

        Task<bool> EmployeeExistsAsync(int id);

        Task DeleteByIdAsync(int id);

        Task<bool> UpdateAsync(Employee employee);

        Task<FaceValidationResult> ValidatePhotoAsync(Employee employee);

        Task<List<FaceVerificationResult>> VerifyPhotoAsync(Employee employee, string toVerifyImageUrl);
    }
}
