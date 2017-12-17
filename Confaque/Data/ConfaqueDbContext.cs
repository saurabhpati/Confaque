using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Confaque.Data
{
    public class ConfaqueDbContext : IdentityDbContext<ConfaqueUser>
    {
        public ConfaqueDbContext(DbContextOptions<ConfaqueDbContext> options) : base(options)
        {
        }
    }
}
