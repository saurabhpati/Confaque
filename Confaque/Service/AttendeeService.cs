using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetSecurity_m1.Models;

namespace Confaque.Service
{
    public class AttendeeService : IAttendeeService
    {
        private readonly List<AttendeeModel> _attendees;

        public AttendeeService()
        {
            this._attendees = new List<AttendeeModel>
            {
                new AttendeeModel { Id = 1, ConferenceId = 1, Name = "Anders Heijlsberg" },
                new AttendeeModel { Id = 2, ConferenceId = 1, Name = "Rob Eisenberg" },
                new AttendeeModel { Id = 3, ConferenceId = 2, Name = "John Mashmellow" }
            };
        }

        public async Task<AttendeeModel> Add(AttendeeModel attendee)
        {
            return await Task<AttendeeModel>.Run(() =>
            {
                if (attendee == null || this._attendees == null)
                {
                    return null;
                }

                attendee.Id = this._attendees.Count + 1;
                return attendee;
            }).ConfigureAwait(false);    
        }

        public async Task<int> GetAttendeesTotal(int conferenceId)
        {
            return await Task.FromResult<int>(this._attendees.Count(attendee => conferenceId.Equals(attendee.ConferenceId))).ConfigureAwait(false);
        }

        public async Task<AttendeeModel> GetById(int attendeeId)
        {
            return await Task.Run(() =>
            {
                if (attendeeId <= 0 || this._attendees == null)
                {
                    return null;
                }

                return this._attendees.Find(attendee => attendeeId.Equals(attendee.Id));
            }).ConfigureAwait(false);
        }
    }
}
