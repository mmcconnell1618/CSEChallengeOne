using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProfileManager.Models;
using Microsoft.EntityFrameworkCore;
using ProfileManager.Datalayer;

namespace ProfileManager.BusinessLayer
{
    public class EmployeeService
    {
        private DatabaseContext _dbcontext;

        public EmployeeService(DatabaseContext dbcontext)
        {
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

        public async Task<bool> Create(Employee employee)
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

        public async Task<bool> EmployeeExists(int id)
        {
            var employee = await FindByIdAsync(id);
            if (employee == null) return false;
            return employee.Id == id;
        }

        public async Task DeleteById(int id)
        {
            var employee = await _dbcontext.Employees.FindAsync(id);
            _dbcontext.Employees.Remove(employee);
            await _dbcontext.SaveChangesAsync();

            //TODO: Consider cleaning up Blob files on delete
        }

        public async Task<bool> Update(Employee employee)
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
                Console.WriteLine(dex.Message);
                return false;
            }
            return true;
        }
    }
}
