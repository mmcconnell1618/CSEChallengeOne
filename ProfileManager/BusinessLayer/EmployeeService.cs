using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProfileManager.Models;
using Microsoft.EntityFrameworkCore;
using ProfileManager.Datalayer;

namespace ProfileManager.BusinessLayer
{
    public class EmployeeService: IEmployeeService
    {
        private DatabaseContext _dbcontext;
        private IFaceService _faceService;
        private IStorageService _storageService;

        public EmployeeService(DatabaseContext dbcontext, IFaceService faceService, IStorageService storageService)
        {
            _faceService = faceService;
            _storageService = storageService;
            _dbcontext = dbcontext;
        }

        public int CountOfEmployees()
        {
            if (_dbcontext == null) return -1;

            return _dbcontext.Employees.Count();
        }

        public async Task<List<Employee>> ListEmployeesAsync()
        {
            var result = await _dbcontext.Employees.ToListAsync();
            return result;
        }

        public async Task<Employee> FindByIdAsync(int id)
        {
            var result = await _dbcontext.Employees.FirstOrDefaultAsync(m => m.Id == id);
            return result;
        }

        public async Task<bool> CreateAsync(Employee employee)
        {
            _dbcontext.Add(employee);
            try
            {
                int x = await _dbcontext.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException updateEx)
            {
                //TODO: Application Insights or trace the exception
                return false;
            }
        }

        public async Task<bool> EmployeeExistsAsync(int id)
        {
            var employee = await FindByIdAsync(id);
            if (employee == null) return false;
            return employee.Id == id;
        }

        public async Task DeleteByIdAsync(int id)
        {
            var employee = await _dbcontext.Employees.FindAsync(id);
            _dbcontext.Employees.Remove(employee);
            await _dbcontext.SaveChangesAsync();

            //TODO: Consider cleaning up Blob files on delete
        }

        public async Task<bool> UpdateAsync(Employee employee)
        {
            try
            {
                _dbcontext.Attach(employee);
                _dbcontext.Entry(employee).State = EntityState.Modified;
                await _dbcontext.SaveChangesAsync();
                //_dbcontext.Update(employee);
                //await _dbcontext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException dex)
            {
                foreach (var entry in dex.Entries)
                {
                    if (entry.Entity is Employee)
                    {
                        var proposedValues = entry.CurrentValues;
                        var databaseValues = entry.GetDatabaseValues();

                        /*foreach (var property in proposedValues.Properties)
                        {
                            var proposedValue = proposedValues[property];
                            var databaseValue = databaseValues[property];

                            proposedValues[property] = proposedValue;                            
                        }*/

                        // Refresh original values to bypass next concurrency check
                        entry.OriginalValues.SetValues(databaseValues);
                        try
                        {
                            await _dbcontext.SaveChangesAsync();
                            return true;
                        }
                        catch
                        {
                            return false;
                        }

                    }
                    else
                    {
                        /*throw new NotSupportedException(
                            "Don't know how to handle concurrency conflicts for "
                            + entry.Metadata.Name);*/
                        return false;
                    }
                }
            }
            return true;
        }

        public async Task<FaceValidationResult> ValidatePhotoAsync(Employee employee)
        {
            var result = new FaceValidationResult();
            result.Faces = await _faceService.FindFaces(_storageService.FullPhotoUrl(employee.Id, employee.PhotoFileName));
            return result;
        }

        public async Task<List<FaceVerificationResult>> VerifyPhotoAsync(Employee employee, string toVerifyImageUrl)
        {
            var knownImageUrl = _storageService.FullPhotoUrl(employee.Id, employee.PhotoFileName);
            return await _faceService.VerifyFacesMatch(knownImageUrl, toVerifyImageUrl);
        }
    }
}
