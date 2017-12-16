using System.Threading.Tasks;
using Shared.Models;

namespace Confaque.Service
{
    public interface IAttendeeService
    {
        Task<AttendeeModel> GetById(int attendeeId);
        Task<AttendeeModel> Add(AttendeeModel attendee);
        Task<int> GetAttendeesTotal(int conferenceId);
    }
}
