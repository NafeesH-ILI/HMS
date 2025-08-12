using hms.Models;
using Microsoft.EntityFrameworkCore;

namespace hms
{
    public class DbCtx : DbContext
    {
        public static string ConnStr = "Host=127.0.0.1;Username=postgres;Password=abcd1234;Database=hms";
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<UName> UNames { get; set; }

        public DbCtx() { }
        public DbCtx(DbContextOptions<DbCtx> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseNpgsql(ConnStr)
                .UseSeeding(async (ctx, _) =>
                {
                    Department department = new() { UName = "crd", Name = "Cardiology" };
                    ctx.Add(department);
                    await ctx.SaveChangesAsync();
                });
        }

        public bool CanConnect()
        {
            return Database.CanConnect();
        }
    }
}
