﻿using System;
using System.Threading.Tasks;
using Confaque.Provider;
using Confaque.Service;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace Confaque.Controllers
{
    public class ProposalController : Controller
    {
        private readonly IProposalService _service;
        private readonly IDataProtector _protector;

        public ProposalController(
            IProposalService service, 
            IDataProtectionProvider dataProtectionProvider, 
            IPurposeString purposeStringConstant)
        {
            this._service = service;
            this._protector = dataProtectionProvider.CreateProtector(purposeStringConstant.ConferenceIdQueryString);
        }

        public async Task<IActionResult> Index(string conferenceId)
        {
            ViewBag.ConferenceId = conferenceId;
            int decryptedConferenceId = Convert.ToInt32(this._protector.Unprotect(conferenceId));
            ViewBag.Title = "Proposals for the selected conference";
            return View(await this._service.GetAll(decryptedConferenceId).ConfigureAwait(false));    
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