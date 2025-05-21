using Lunamaroapi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Lunamaroapi.Data
{
    public class AppDBContext :IdentityDbContext<ApplicationUser>
    {
        public AppDBContext(DbContextOptions options) : base(options)
        {

        }
        DbSet<ApplicationUser> Users { get; set; }
    }
}
