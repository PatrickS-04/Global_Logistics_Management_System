using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TechMove.Database.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace TechMove.Backend_API.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<StandardContract> StandardContracts { get; set; }
        public DbSet<PremiumContract> PremiumContracts { get; set; }
        public DbSet<InternationalContract> InternationalContracts { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }
        public DbSet<ContractFile> ContractFiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Contract>()
                .HasDiscriminator<string>("ContractType")
                .HasValue<StandardContract>("Standard")
                .HasValue<PremiumContract>("Premium")
                .HasValue<InternationalContract>("International");

            modelBuilder.Entity<ServiceRequest>()
                .Property(serviceRequest => serviceRequest.cost)
                .HasColumnType("decimal(18,2)");
        }
    }
}
