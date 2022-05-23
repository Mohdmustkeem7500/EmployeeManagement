using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeManagement.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly ILogger logger;

        public HomeController(IEmployeeRepository employeeRepository,
                               IHostingEnvironment hostingEnvironment,
                               ILogger<HomeController> logger)
        {
          
            _employeeRepository = employeeRepository;
            this.hostingEnvironment = hostingEnvironment;
            this.logger = logger;
        }

        [AllowAnonymous]
        public ViewResult Index()
        {

            //string html;
            //// obtain some arbitrary html....
            //using (var client = new WebClient())
            //{
            //    html = client.DownloadString("http://stackoverflow.com/questions/2038104");
            //}
            //// use the html agility pack: http://www.codeplex.com/htmlagilitypack
            //HtmlDocument doc = new HtmlDocument();
            //doc.LoadHtml(html);
            //StringBuilder sb = new StringBuilder();
            //foreach (HtmlTextNode node in doc.DocumentNode.SelectNodes("//text()"))
            //{
            //    sb.AppendLine(node.Text);
            //}
            //string final = sb.ToString();


            //string html;
            //// obtain some arbitrary html....
            //using (var client = new System.Net.WebClient())
            //{
            //    html = client.DownloadString("http://stackoverflow.com/questions/2038104");
            //}
            var model = _employeeRepository.GetAllEmployee();
            return View(model);

        }

        [AllowAnonymous]
        public ViewResult Details(int? id)
        {
            logger.LogTrace("Trace Log");
            logger.LogDebug("Debug Log");
            logger.LogInformation("Information Log");
            logger.LogWarning("Warning");
            logger.LogCritical("Critical Log");

            Employee employee = _employeeRepository.GetEmployee(id.Value);
            if(employee == null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound", id.Value);
            }

            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel()
            {
                Employee = employee,
                PageTitle = "Employee Details"
            };

            return View(homeDetailsViewModel);

            //global exception handle

            //throw new Exception("Error in Details View");

            
        }

        [HttpGet]
        public ViewResult Create()
        {
            return View();
        }

        [HttpGet]
        public ViewResult Edit(int id)
        {
            Employee employee = _employeeRepository.GetEmployee(id);
            EmployeeEditViweModel employeeEditViweModel = new EmployeeEditViweModel
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                ExistingPhotoPath = employee.PhotoPath
            };

            return View(employeeEditViweModel);
        }

        [HttpPost]
        public IActionResult Edit(EmployeeEditViweModel model)
        {
            if (ModelState.IsValid)
            {
                Employee employee = _employeeRepository.GetEmployee(model.Id);
                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Department = model.Department;
                if (model.Photos != null)
                {
                    if (model.ExistingPhotoPath != null)
                    {
                        string FilePath = Path.Combine(hostingEnvironment.WebRootPath, 
                                           "Images", model.ExistingPhotoPath);
                        System.IO.File.Delete(FilePath);
                    }

                   employee.PhotoPath = ProcessUploadedFile(model);
                }
                _employeeRepository.Update(employee);
                return RedirectToAction("Index");
            }

            return View();
        }

        private string ProcessUploadedFile(EmployeeCreateViewModel model)
        {
            string UniqueFileName = null;
            if (model.Photos != null && model.Photos.Count > 0)
            {
                foreach (IFormFile photo in model.Photos)
                {
                    string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");
                    UniqueFileName = Guid.NewGuid().ToString() + "-" + photo.FileName;
                    string Filepath = Path.Combine(uploadsFolder, UniqueFileName);
                    using (var fileStream = new FileStream(Filepath, FileMode.Create))
                    {
                        photo.CopyTo(fileStream);
                    }
                    
                }
            }

            return UniqueFileName;
        }

        [HttpPost]
        public IActionResult Create(EmployeeCreateViewModel model)
        {
            if(ModelState. IsValid)
            {
                string UniqueFileName = ProcessUploadedFile(model);

                Employee newEmployee = new Employee
                {
                    Name = model.Name,
                    Email = model.Email,
                    Department = model.Department,
                    PhotoPath = UniqueFileName
                };

                _employeeRepository.Add(newEmployee);
                return RedirectToAction("Details", new { id = newEmployee.Id});
            }

            return View();
        }
    }
}
