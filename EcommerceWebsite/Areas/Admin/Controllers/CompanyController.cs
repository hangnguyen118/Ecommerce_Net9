using EcommerceWebsite.DataContext.Repository.IRepository;
using EcommerceWebsite.EntityModels;
using EcommerceWebsite.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceWebsite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;           
        }
        public IActionResult Index()
        {
            List<Company> companies = _unitOfWork.Company.GetAll().ToList();
            return View(companies);
        }
        public IActionResult Upsert(int? Id)
        {                     
            if (Id == null || Id == 0)
            {
                return View(new Company());
            }
            else
            {
                Company company = _unitOfWork.Company.Get(u => u.Id == Id);
                return View(company);
            }
        }
        [HttpPost]
        public IActionResult Upsert(Company obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Company.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Company created successfully";
                return RedirectToAction("Index");
            }
            return View();
        }
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Company? companyFromDb = _unitOfWork.Company.Get(u => u.Id == id);
            if (companyFromDb == null)
            {
                return NotFound();
            }            
            return View(companyFromDb);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Company? obj = _unitOfWork.Company.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }          
            _unitOfWork.Company.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Company deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
