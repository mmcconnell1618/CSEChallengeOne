using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProfileManager.Datalayer;
using ProfileManager.Models;
using ProfileManager.BusinessLayer;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace ProfileManager.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IEmployeeService _employees;
        private readonly IStorageService _storageService;

        public EmployeesController(IEmployeeService employeeService, IStorageService storageService)
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
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Department,RowVersion")] Employee employee, IFormFile photo)
        {
            if (ModelState.IsValid)
            {                        
                bool isSaved = await _employees.CreateAsync(employee);

                if (isSaved)
                {
                    if (photo != null)
                    {
                        // Upload Photo File to Blob Storage Here
                        var photoFileName = Path.GetFileName(photo.FileName);

                        var photoData = await _storageService.WriteEmployeePhoto(employee.Id, photoFileName, photo);
                        if (photoData.Success)
                        {
                            var employeeToUpdate = await _employees.FindByIdAsync(employee.Id);
                            employeeToUpdate.PhotoHeight = photoData.ImageHeight;
                            employeeToUpdate.PhotoWidth = photoData.ImageWidth;
                            employeeToUpdate.PhotoFileName = photoFileName;
                            await _employees.UpdateAsync(employeeToUpdate);
                        }

                        // Check Faces
                        return new RedirectResult("/employees/edit/" + employee.Id + "?highlightFaces=true");                                                   
                    }
                }
                else
                {                    
                    ViewBag.ErrorMessage = "An error occurred while saving. Make sure a photo is selected";
                    return View(employee);
                }
            }
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id, bool highlightFaces = false)
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

            // Populate Photo View Model
            var photoData = new EmployeePhotoViewModel(employee, _storageService);

            if (highlightFaces == true)
            {
                // Check Faces
                var validPhoto = await _employees.ValidatePhotoAsync(employee);
                photoData.MapFacesForValidation(validPhoto.Faces);
                if (!validPhoto.IsValidEmployeePhoto)
                {
                    ViewBag.ErrorMessage = "There was an error with the photo.";
                }                
            }
            ViewBag.PhotoData = photoData;

            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Department,PhotoFileName,RowVersion")] Employee employee, IFormFile photo)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            // Populate Model for Photo
            var photoData = new EmployeePhotoViewModel(employee, _storageService);

            if (ModelState.IsValid)
            {

                if (await _employees.UpdateAsync(employee))
                {
                    if (photo != null)
                    {
                        // Upload Photo File to Blob Storage Here
                        var photoFileName = Path.GetFileName(photo.FileName);

                        var photoUploadData = await _storageService.WriteEmployeePhoto(employee.Id, photoFileName, photo);
                        if (photoUploadData.Success)
                        {
                            var employeeToUpdate = await _employees.FindByIdAsync(employee.Id);
                            employeeToUpdate.PhotoHeight = photoUploadData.ImageHeight;
                            employeeToUpdate.PhotoWidth = photoUploadData.ImageWidth;
                            employeeToUpdate.PhotoFileName = photoFileName;
                            await _employees.UpdateAsync(employeeToUpdate);

                            // Update ViewModel with new photo information
                            photoData = new EmployeePhotoViewModel(employeeToUpdate, _storageService);
                            /*photoData.Url = _storageService.FullPhotoUrl(employee.Id, employee.PhotoFileName);
                            photoData.AltText = employee.FirstName + " " + employee.LastName;
                            photoData.OriginalImageHeight = photoUploadData.ImageHeight;
                            photoData.OriginalImageWidth = photoUploadData.ImageWidth;*/
                        }

                        // Check Faces
                        var validPhoto = await _employees.ValidatePhotoAsync(employee);
                        photoData.MapFacesForValidation(validPhoto.Faces);
                        if (!validPhoto.IsValidEmployeePhoto)
                        {
                            ViewBag.ErrorMessage = "There was an error with the photo.";
                        }
                        else
                        {
                            ViewBag.SuccessMessage = "Changes Saved";
                        }
                        ViewBag.PhotoData = photoData;
                        return View(employee);
                    }                    
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.ErrorMessage = "An error occurred while updating the employee.";                  
                    ViewBag.PhotoData = photoData;                
                    return View(employee);
                }                
            }

            ViewBag.PhotoData = photoData;
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
            await _employees.DeleteByIdAsync(id);            
            return RedirectToAction(nameof(Index));
        }        

        public IActionResult Find(int employeeid)
        {
            return new RedirectResult("/employees/edit/" + employeeid);
        }

        // GET: Employees/Verify/5
        public async Task<IActionResult> Verify(int? id)
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


            // Populate Photo View Model
            var photoData = new EmployeePhotoViewModel(employee, _storageService);
            var validPhoto = await _employees.ValidatePhotoAsync(employee);
            photoData.MapFacesForValidation(validPhoto.Faces);
            if (!validPhoto.IsValidEmployeePhoto)
            {
                ViewBag.ErrorMessage = "There was an error with the photo.";
            }
            ViewBag.KnownPhotoData = photoData;
            ViewBag.ToVerifyPhotoData = new EmployeePhotoViewModel();

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Verify")]
        public async Task<IActionResult> VerifySubmit(int id, IFormFile photo)
        {
            var employee = await _employees.FindByIdAsync(id);

            // Populate Known Photo View Model
            var photoData = new EmployeePhotoViewModel(employee, _storageService);
            var validPhoto = await _employees.ValidatePhotoAsync(employee);
            photoData.MapFacesForValidation(validPhoto.Faces);
            if (!validPhoto.IsValidEmployeePhoto)
            {
                ViewBag.ErrorMessage = "There was an error with the photo.";
            }
            ViewBag.KnownPhotoData = photoData;

            // Upload the Verification Photo to Blob Storage
            var uniqueVerifyPath = "verify/" + System.Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName);
            var photoUploadData = await _storageService.WriteEmployeePhoto(employee.Id, uniqueVerifyPath, photo);
            var verifyFullUrl = _storageService.FullPhotoUrl(employee.Id, uniqueVerifyPath);

            // Run Verification
            var verifyData = await _employees.VerifyPhotoAsync(employee, verifyFullUrl);

            // Populate Photo Data for Verification Matches
            var verifyPhotoData = new EmployeePhotoViewModel(employee, _storageService);
            verifyPhotoData.Url = verifyFullUrl;
            verifyPhotoData.OriginalImageHeight = photoUploadData.ImageHeight;
            verifyPhotoData.OriginalImageWidth = photoUploadData.ImageWidth;            
            verifyPhotoData.MapFacesForVerification(verifyData);
            ViewBag.ToVerifyPhotoData = verifyPhotoData;

            ViewBag.IsMatch = false;
            if (verifyData.Where(y => y.IsMatch == true).Count() > 0)
            {
                ViewBag.IsMatch = true;
            }

            return View(employee);
        }
    }
}
