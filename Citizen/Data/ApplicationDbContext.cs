using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Citizen.Models;

namespace Citizen.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        [NotMapped]
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        [NotMapped]
        public DbSet<Country> Country { get; set; }
        [NotMapped]
        public DbSet<TimeEvent> TimeEvents { get; set; }
        [NotMapped]
        public DbSet<UserStorage> UserStorage { get; set; }
        [NotMapped]
        public DbSet<MarketplaceOffer> MarketplaceOffers { get; set; }
        [NotMapped]
        public DbSet<Item> Items { get; set; }
        [NotMapped]
        public DbSet<Company> Companies { get; set; }
        [NotMapped]
        public DbSet<Employment> Employments { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>()
                .HasAlternateKey(c => c.Name)
                .HasName("AlternateKey_CitizenName");

            builder.Entity<Country>()
                .HasAlternateKey(c => c.Name)
                .HasName("AlternateKey_CountryName");

            builder.Entity<TimeEvent>()
                .HasAlternateKey(c => c.Name)
                .HasName("AlternateKey_TimeEventName");

            builder.Entity<UserStorage>()
                .HasAlternateKey(c => c.ApplicationUserId)
                .HasName("AlternateKey_StorageCitizenId");

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
