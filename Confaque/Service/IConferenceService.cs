using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Models;

namespace Confaque.Service
{
    public interface IConferenceService
    {
        Task<IEnumerable<ConferenceModel>> GetAll();
        Task<ConferenceModel> GetById(int id);
        Task Add(ConferenceModel model);
    }
}
