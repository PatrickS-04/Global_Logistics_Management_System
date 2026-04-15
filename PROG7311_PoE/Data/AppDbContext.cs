using Microsoft.EntityFrameworkCore;
using PROG7311_PoE.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace PROG7311_PoE.Data
{
    public class AppDbContext : DbContext
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
