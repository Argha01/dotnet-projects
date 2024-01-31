using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApp.Data.Account;

namespace WebApp.Data
{
    public class AppDbContext:IdentityDbContext<User>
    {

        public AppDbContext(DbContextOptions options) : base(options) { }
        
    }
}
