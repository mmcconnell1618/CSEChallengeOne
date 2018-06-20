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

        Task<bool> Create(Employee employee);

        Task<bool> EmployeeExists(int id);

        Task DeleteById(int id);

        Task<bool> Update(Employee employee);

        Task<FaceValidationResult> ValidatePhoto(Employee employee);
    }
}
