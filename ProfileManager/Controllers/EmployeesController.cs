﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProfileManager.Datalayer;
using ProfileManager.Models;
using ProfileManager.BusinessLayer;

namespace ProfileManager.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly EmployeeService _employees;
        private readonly StorageService _storageService;

        public EmployeesController(EmployeeService employeeService, StorageService storageService)
        {
            _employees = employeeService;
            _storageService = storageService;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            ViewBag.TotalCount = _employees.CountOfEmployees();                        
            return View(await _employees.ListEmployeesAsync());
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _employees.FindByIdAsync(id.Value);

            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Department,PhotoFileName,RowVersion")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                bool isSaved = await _employees.Create(employee);

                if (!isSaved)
                {
                    ViewBag.ErrorMessage = "An error occurred while saving.";
                    return View(employee);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _employees.FindByIdAsync(id.Value);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Department,PhotoFileName,RowVersion")] Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if(await _employees.Update(employee))
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.ErrorMessage = "An error occurred while updating the employee.";
                    return View(employee);
                }                
            }
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _employees.FindByIdAsync(id.Value);

            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _employees.DeleteById(id);            
            return RedirectToAction(nameof(Index));
        }        
    }
}
