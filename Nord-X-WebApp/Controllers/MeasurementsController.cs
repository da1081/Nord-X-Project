#nullable disable
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nord_X_WebApp.Data;

namespace Nord_X_WebApp.Controllers
{
    [AutoValidateAntiforgeryToken]
    [Authorize(Roles = "Administrator")]
    public class MeasurementsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public MeasurementsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Measurements
        public async Task<IActionResult> Index()
        {
            var res = await _unitOfWork.MeasurementRepository.GetAllAsync();
            return View(res);
        }

        // GET: Measurements/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
                return NotFound();

            var model = await _unitOfWork.MeasurementRepository.GetAsync(id, new string[] { "Sensor" });
            if (model == null)
                return NotFound();

            return View(model);
        }
    }
}
