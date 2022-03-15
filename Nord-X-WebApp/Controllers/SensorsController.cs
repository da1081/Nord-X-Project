using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nord_X_WebApp.Data;
using Nord_X_WebApp.Data.Entities;

namespace Nord_X_WebApp.Controllers
{
    [AutoValidateAntiforgeryToken]
    [Authorize(Roles = "Administrator")]
    public class SensorsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public SensorsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Companies
        public async Task<IActionResult> Index()
        {
            return View(await _unitOfWork.SensorRepository.GetAllAsync());
        }

        // GET: Companies/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
                return NotFound();

            var model = await _unitOfWork.SensorRepository.GetAsync(id, new string[] { "MeasurementType" });
            if (model == null)
                return NotFound();

            return View(model);
        }

        // GET: Companies/Create
        public async Task<IActionResult> Create()
        {
            // Create SelectListItems for Company dropdown.
            var companies = await _unitOfWork.CompanyRepository.GetAllAsync();
            ViewBag.Companies = new List<SelectListItem>();
            foreach (var item in companies) { ViewBag.Companies.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() }); }

            // Create SelectListItems for MeasuremnetType dropdown.
            var measurementTypes = await _unitOfWork.MeasurementTypeRepository.GetAllAsync();
            ViewBag.MeasurementTypes = new List<SelectListItem>();
            foreach (var item in measurementTypes) { ViewBag.MeasurementTypes.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() }); }

            return View();
        }

        // POST: Companies/Create
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Name,Description,Endpoint,CompanyId,MeasurementTypeId")] Sensor model)
        {
            if (ModelState.IsValid)
            {
                model.Id = Guid.NewGuid();
                await _unitOfWork.SensorRepository.InsertAsync(model);
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

            var model = await _unitOfWork.SensorRepository.GetAsync(id);
            if (model == null)
                return NotFound();

            // Create SelectListItems for Company dropdown.
            var companies = await _unitOfWork.CompanyRepository.GetAllAsync();
            ViewBag.Companies = new List<SelectListItem>();
            foreach (var item in companies) { ViewBag.Companies.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() }); }

            // Create SelectListItems for MeasuremnetType dropdown.
            var measurementTypes = await _unitOfWork.MeasurementTypeRepository.GetAllAsync();
            ViewBag.MeasurementTypes = new List<SelectListItem>();
            foreach (var item in measurementTypes) { ViewBag.MeasurementTypes.Add(new SelectListItem { Text = item.Name, Value = item.Id.ToString() }); }

            return View(model);
        }

        // POST: Companies/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, [Bind("Name,Description,Endpoint,CompanyId,MeasurementTypeId,Id,AddedDate,RowVersion")] Sensor model)
        {
            if (id != model.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _unitOfWork.SensorRepository.Update(model);
                    await _unitOfWork.SaveAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    bool exists = await _unitOfWork.SensorRepository.ExistsAsync(x => x.Id == model.Id);
                    if (!exists)
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Companies/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
                return NotFound();

            var model = await _unitOfWork.SensorRepository.GetAsync(id);
            if (model == null)
                return NotFound();

            return View(model);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _unitOfWork.SensorRepository.DeleteAsync(id);
            await _unitOfWork.SaveAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
