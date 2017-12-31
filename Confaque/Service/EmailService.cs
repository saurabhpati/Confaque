using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Confaque.Service
{
    public class EmailService : IEmailService
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Task.CompletedTask; // implementation pending
        }
    }
}
