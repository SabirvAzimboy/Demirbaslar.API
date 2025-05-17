using Demirbaslar.API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Demirbaslar.API.Data
{
    public class AppDbContext : IdentityDbContext<AppUsers>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {
        }

        public DbSet<AppUsers> AppUsers { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Assets> Assets { get; set; }
        public DbSet<Repair> Repairs { get; set; }
        public DbSet<RepairPersons> RepairPersons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Важно для Identity

            modelBuilder.Entity<AuditLog>()
                .HasOne(al => al.User)
                .WithMany()
                .HasForeignKey(al => al.UserId);            

            modelBuilder.Entity<Assets>()
                .Property(a => a.Location)
                .HasDefaultValue(0);

            modelBuilder.Entity<Assets>()
                .Property(a => a.Perfect)
                .HasDefaultValue(true);

            modelBuilder.Entity<Assets>()
                .HasIndex(f => f.InventoryNumber)
                .IsUnique();           

            modelBuilder.Entity<Repair>()
                .Property(r => r.Repaired)
                .HasDefaultValue(true);
        }
    }
}

