using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Confaque.Provider;
using Microsoft.AspNetCore.DataProtection;
using Shared.Models;

namespace Confaque.Service
{
    /// <summary>
    /// The service implementing the IProposalService, gets all kinds of data regarding proposals.
    /// </summary>
    public class ProposalService : IProposalService
    {
        #region Private Fields

        private readonly List<ProposalModel> _proposals;
        private readonly IDataProtector _protector;

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the<see cref="ProposalService"/> class. 
        /// </summary>
        public ProposalService(IDataProtectionProvider dataProtectionProvider, IPurposeString purposeStringConstant)
        {
            this._protector = dataProtectionProvider.CreateProtector(purposeStringConstant.ConferenceIdQueryString);
            this._proposals = new List<ProposalModel>
            {
                new ProposalModel
                {
                    Id = 1,
                    ConferenceId = 1,
                    Speaker = "Speaker X",
                    Title = "Understanding ASP.NET Core Security"
                },
                new ProposalModel
                {
                    Id = 2,
                    ConferenceId = 2,
                    Speaker = "Speaker Y",
                    Title = "Starting Your Developer Career"
                },
                new ProposalModel
                {
                    Id = 3,
                    ConferenceId = 2,
                    Speaker = "Speaker Z",
                    Title = "ASP.NET Core TagHelpers"
                }
            };
        }

        #endregion

        /// <summary>
        /// Adds a proposal to the existing proposals.
        /// </summary>
        /// <param name="model">The proposal model to be added.</param>
        /// <returns>The task object.</returns>
        public async Task Add(ProposalModel model)
        {
            await Task.Run(() =>
            {
                if (model == null || this._proposals == null)
                {
                    return;
                }

                model.Id = this._proposals.Count + 1;
                this._proposals.Add(model);
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Approves the proposal for the proposal id provided. 
        /// </summary>
        /// <param name="proposalId">The proposal id for which the proposal has to be approved.</param>
        /// <returns>Task of proposal model.</returns>
        public async Task<ProposalModel> Approve(int proposalId)
        {
            return await Task<ProposalModel>.Run(() =>
            {
                if (proposalId <= 0 || this._proposals == null)
                {
                    return null;
                }

                ProposalModel proposalModel = this._proposals.Find(proposal => proposal.Equals(proposal.Id));

                if (proposalModel == null)
                {
                    return null;
                }

                proposalModel.Approved = true;
                return proposalModel;
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets all the proposals for a conference.
        /// </summary>
        /// <param name="conferenceId">The conference id for which the proposals are required.</param>
        /// <returns>The corresponding proposals for the conference id.</returns>
        public async Task<IEnumerable<ProposalModel>> GetAll(int conferenceId)
        {
            return await Task.Run(() =>
            {
                if (conferenceId <= 0)
                {
                    return null;
                }

                return this._proposals.Where(proposal => conferenceId.Equals(proposal.ConferenceId));
            }).ConfigureAwait(false);
        }
    }
}
