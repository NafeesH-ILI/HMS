using hms.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace hms.Common
{
    public class DbCtx : IdentityDbContext<User, IdentityRole, string>
    {
        public static string ConnStr = "Host=127.0.0.1;Username=postgres;Password=abcd1234;Database=hms";
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<UName> UNames { get; set; }
        public DbSet<Patient> Patients { get; set; }

        public DbCtx() { }
        public DbCtx(DbContextOptions<DbCtx> options) : base(options) { }

        /*protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>().HasData(
                new User { Type = User.Types.SuperAdmin, PassHash = "abcd", UName = "sudo"});
        }*/

        public bool CanConnect()
        {
            return Database.CanConnect();
        }
    }
}
