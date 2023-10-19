using Core.Entities.Concrete;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Concrete.EntityFramework.Contexts
{
    public class SwapContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"");
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserWallet> UserWallets { get; set; }
        public DbSet<CompanyVault> CompanyVault { get; set; }
        public DbSet<UserAssets> UserAssets { get; set; }
        public DbSet<OpenOrder> OpenOrders { get; set; }
        public DbSet<Pending> Pending { get; set; }
        public DbSet<SuspendUser> SuspendUsers { get; set; }

        
    }
}
