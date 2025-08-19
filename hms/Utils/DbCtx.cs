using hms.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace hms.Utils
{
    public class DbCtx : IdentityDbContext<User, IdentityRole, string>
    {
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<UName> UNames { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<PassResetOtp> Otps { get; set; }

        public DbCtx() { }
        public DbCtx(DbContextOptions<DbCtx> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            /*modelBuilder.Entity<User>().HasData(
                new User { Type = User.Types.SuperAdmin, PassHash = "abcd", UName = "sudo"});*/

            modelBuilder.Entity<User>().HasData(new User
            {
                Id = "1",
                UserName = "sudo",
                NormalizedUserName = "SUDO",
                Email = "sudo@example.com",
                NormalizedEmail = "SUDO@EXAMPLE.COM",
                EmailConfirmed = true,
                Type = User.Types.SuperAdmin,
                SecurityStamp = "f613a119-e134-428c-8dec-98bf5c82ea2e",
                ConcurrencyStamp = "f613a119-e134-428c-8dec-98bf5c82ea2a",
                PasswordHash = "AQAAAAIAAYagAAAAEOVIKgI78Fd2jA/QBnJM/7uFMuPgl2jrht/8Z7hVY7kJU37/hOW3V6PIP2AihhitBg=="
            });
            modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Id = "64a1c0ab-8783-4cb7-ad7d-254c050815aa",
                Name = "SuperAdmin",
                NormalizedName = "SUPERADMIN"
            });
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                UserId = "1",
                RoleId = "64a1c0ab-8783-4cb7-ad7d-254c050815aa"
            });
        }

        public bool CanConnect()
        {
            return Database.CanConnect();
        }
    }
}
