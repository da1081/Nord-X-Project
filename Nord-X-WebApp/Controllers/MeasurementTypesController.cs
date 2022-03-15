using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nord_X_WebApp.Data;
using Nord_X_WebApp.Data.Entities;

namespace Nord_X_WebApp.Controllers
{
    [AutoValidateAntiforgeryToken]
    [Authorize(Roles = "Administrator")]
    public class MeasurementTypesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public MeasurementTypesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Companies
        public async Task<IActionResult> Index()
        {
            return View(await _unitOfWork.MeasurementTypeRepository.GetAllAsync());
        }

        // GET: Companies/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
                return NotFound();

            var model = await _unitOfWork.MeasurementTypeRepository.GetAsync(id);
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
        public async Task<IActionResult> Create([Bind("Name,Translation,Description,Type")] MeasurementType model)
        {
            if (ModelState.IsValid)
            {
                model.Id = Guid.NewGuid();
                await _unitOfWork.MeasurementTypeRepository.InsertAsync(model);
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

            var model = await _unitOfWork.MeasurementTypeRepository.GetAsync(id);
            if (model == null)
                return NotFound();

            return View(model);
        }

        // POST: Companies/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, [Bind("Name,Translation,Description,Type,Id,AddedDate,RowVersion")] MeasurementType model)
        {
            if (id != model.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _unitOfWork.MeasurementTypeRepository.Update(model);
                    await _unitOfWork.SaveAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    bool exists = await _unitOfWork.MeasurementTypeRepository.ExistsAsync(x => x.Id == model.Id);
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

            var model = await _unitOfWork.MeasurementTypeRepository.GetAsync(id);
            if (model == null)
                return NotFound();

            return View(model);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _unitOfWork.MeasurementTypeRepository.DeleteAsync(id);
            await _unitOfWork.SaveAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
