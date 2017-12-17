using System.Threading.Tasks;
using Confaque.Service;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace Confaque.Controllers.API
{
    [EnableCors("AllowConfaque")]
    [Route("api/[controller]")]
    public class AttendeeController : Controller
    {
        private readonly IAttendeeService _service;

        public AttendeeController(IAttendeeService service)
        {
            this._service = service;
        }

        public async Task<AttendeeModel> Get(int attendeeId)
        {
            return await this._service.GetById(attendeeId).ConfigureAwait(false);
        }

        [HttpPost]
        public async Task<IActionResult> Add(int conferenceId, string name)
        {
            AttendeeModel attendee = await this._service.Add(new AttendeeModel() { Name = name, ConferenceId = conferenceId }).ConfigureAwait(false);
            return new CreatedAtActionResult("Get", "Attendee", new { attendeeId = attendee.Id }, attendee);
        }
    }
}