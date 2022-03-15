#nullable disable
using CVRCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Nord_X_WebApp.Data;
using Nord_X_WebApp.Data.Entities;

namespace Nord_X_WebApp.Controllers
{
    [AutoValidateAntiforgeryToken]
    [Authorize(Roles = "Administrator")]
    public class CompaniesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompaniesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Companies
        public async Task<IActionResult> Index()
        {
            return View(await _unitOfWork.CompanyRepository.GetAllAsync());
        }

        // GET: Companies/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
                return NotFound();

            var model = await _unitOfWork.CompanyRepository.GetAsync(id);
            if (model == null)
                return NotFound();

            return View(model);
        }

        // GET: Companies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Companies/Create
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Vat,Name,Address,ZipCode,City,Description,Phone,ContactMail,ReportMail")] Company model)
        {
            if (ModelState.IsValid)
            {
                model.Id = Guid.NewGuid();
                await _unitOfWork.CompanyRepository.InsertAsync(model);
                await _unitOfWork.SaveAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Companies/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
                return NotFound();

            var model = await _unitOfWork.CompanyRepository.GetAsync(id);
            if (model == null)
                return NotFound();

            return View(model);
        }

        // POST: Companies/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, [Bind("Vat,Name,Address,ZipCode,City,Description,Id,AddedDate,RowVersion,Phone,ContactMail,ReportMail")] Company model)
        {
            if (id != model.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _unitOfWork.CompanyRepository.Update(model);
                    await _unitOfWork.SaveAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException e)
                {
                    var entry = e.Entries.Single();
                    var newVersion = (Company)entry.Entity;
                    var oldVersion = await entry.GetDatabaseValuesAsync();
                    if (oldVersion is not null)
                    {
                        ModelState.AddModelError(string.Empty, $"Der opstod en konflikt. Ændringer blev fortaget af en anden bruger.");
                        foreach (IProperty property in entry.Metadata.GetProperties())
                        {
                            if (property.PropertyInfo.DeclaringType.Name == "BaseEntity")
                                continue;
                            object newValue = newVersion.GetType().GetProperty(property.Name).GetValue(newVersion, null);
                            object oldValue = oldVersion.GetValue<object>(property.Name);
                            if (newValue != oldValue && (newValue == null || !newValue.Equals(oldValue)))
                                ModelState.AddModelError(property.Name, $"Ny værdi i formularen: {oldValue}");
                        }
                        model.RowVersion = oldVersion.GetValue<byte[]>("RowVersion");
                        ModelState.Remove("RowVersion");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Dette objekt eksistere ikke længere i databasen.");
                    }
                }
            }
            return View(model);
        }

        // GET: Companies/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
                return NotFound();

            var model = await _unitOfWork.CompanyRepository.GetAsync(id);
            if (model == null)
                return NotFound();

            return View(model);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _unitOfWork.CompanyRepository.DeleteAsync(id);
            await _unitOfWork.SaveAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<JsonResult> GetCompanyCvr(int vatNumber)
        {
            CvrApiClient client = new();
            return Json(await client.GetCompanyAsync(vatNumber));
        }
    }
}
