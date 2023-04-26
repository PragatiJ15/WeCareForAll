using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WeCareForAll.Models.DTO;

namespace WeCareForAll.Models.Domain
{
    public class DatabaseContext : IdentityDbContext<ApplicationUser>
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options): base(options)
        {

        }
        public DbSet<TokenInfo> TokenInfos { get; set; }
        public DbSet<Drugs> drugs { get; set; }
        public DbSet <Cart> carts { get; set; }
    }
}
