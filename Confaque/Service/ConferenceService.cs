using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Confaque.Provider;
using Microsoft.AspNetCore.DataProtection;
using Shared.Models;

namespace Confaque.Service
{
    /// <summary>
    /// The service implementation of IConferenceService, gets all kinds of data regarding conferences.
    /// </summary>
    public class ConferenceService : IConferenceService
    {
        #region Private Fields

        private readonly List<ConferenceModel> _conferences;
        private readonly IDataProtector _protector;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the<see cref="ConferenceService"/> class. 
        /// </summary>
        public ConferenceService(IDataProtectionProvider dataProtectionProvider, IPurposeString purposeString)
        {
            this._protector = dataProtectionProvider.CreateProtector(purposeString.ConferenceIdQueryString);
            this._conferences = new List<ConferenceModel>
            {
                new ConferenceModel { Id = 1, EncryptedId = this._protector.Protect("1"), Name = "NDC", Location = "Oslo", Start = new DateTime(2017, 6, 12), AttendeeTotal = 2132 },
                new ConferenceModel { Id = 2, EncryptedId = this._protector.Protect("2"), Name = "IT/DevConnections", Location = "San Francisco", Start = new DateTime(2017, 10, 18), AttendeeTotal = 3210 }
            };
        }

        #endregion

        #region IConferenceService Implementation

        /// <summary>
        /// Adds a Conference model to the list of existing conferences.
        /// </summary>
        /// <param name="model">The conferences model to be added.</param>
        /// <returns>The task object.</returns>
        public async Task Add(ConferenceModel model)
        {
            await Task.Run(() =>
            {
                if (model == null || this._conferences == null)
                {
                    return;
                }

                model.Id = this._conferences.Count + 1;
                model.EncryptedId = this._protector.Protect(model.Id.ToString());
                this._conferences.Add(model);
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets all existing conferences.
        /// </summary>
        /// <returns>Task of conferences.</returns>
        public async Task<IEnumerable<ConferenceModel>> GetAll()
        {
            return await Task.FromResult<IEnumerable<ConferenceModel>>(this._conferences).ConfigureAwait(false);
        }

        /// <summary>
        /// Gets a conferences by its provided id.
        /// </summary>
        /// <param name="id">The id of the required conferences.</param>
        /// <returns>Task of the required conference model.</returns>
        public async Task<ConferenceModel> GetById(int id)
        {
            return await Task<ConferenceModel>.Run(() =>
            {
                if (id <= 0)
                {
                    return null;
                }

                return this._conferences.Find(conference => id.Equals(conference.Id));
            }).ConfigureAwait(false);
        }

        #endregion
    }
}
