using System;
using System.Threading.Tasks;
using Confaque.Service;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace Confaque.Controllers
{
    public class ProposalController : Controller
    {
        private readonly IProposalService _service;

        public ProposalController(IProposalService service)
        {
            this._service = service;
        }

        public async Task<IActionResult> Index(int conferenceId)
        {
            ViewBag.ConferenceId = conferenceId;
            ViewBag.Title = "Proposals for the selected conference";
            return View(await this._service.GetAll(conferenceId).ConfigureAwait(false));
        }

        public async Task<IActionResult> Add(int conferenceId)
        {
            ViewBag.Title = "Add Proposal";
            return await Task.FromResult<IActionResult>(View(new ProposalModel()
            {
                ConferenceId = conferenceId
            })).ConfigureAwait(false);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(ProposalModel proposal)
        {
            if (!ModelState.IsValid)
            {
                throw new InvalidOperationException("Model state is invalid");
            }

            await this._service.Add(proposal).ConfigureAwait(false);
            return RedirectToAction("Index", new { conferenceId = proposal.ConferenceId });
        }

        public async Task<IActionResult> Approve(int proposalId)
        {
            ProposalModel proposalModel = await this._service.Approve(proposalId).ConfigureAwait(false);
            return RedirectToAction("Index", new { conferenceId = proposalModel.ConferenceId });
        }
    }
}