using System;
using System.Threading.Tasks;
using Confaque.Service;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace Confaque.Controllers
{
    public class ConferenceController : Controller
    {
        private readonly IConferenceService _service;

        public ConferenceController(IConferenceService service)
        {
            this._service = service;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Title = "Conference Overview";
            return View(await this._service.GetAll().ConfigureAwait(false));
        }

        public async Task<IActionResult> Add()
        {
            ViewBag.Title = "Add Conference";
            return await Task.FromResult<IActionResult>(View(new ConferenceModel())).ConfigureAwait(false);
        }

        [HttpPost]
        public async Task<IActionResult> Add(ConferenceModel model)
        {
            if (!this.ModelState.IsValid)
            {
                throw new InvalidOperationException("Model State is invalid");
            }

            await this._service.Add(model).ConfigureAwait(false);
            return RedirectToAction("Index");
        }
    }
}