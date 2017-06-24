using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Citizen.Models;
using Citizen.Models.Items;

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
        public DbSet<FoodItem> FoodItem { get; set; }

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

            builder.Entity<ApplicationUser>()
                .HasOne(f => f.FoodItem)
                .WithOne(a => a.ApplicationUser);

            builder.Entity<Country>()
                .HasAlternateKey(c => c.Name)
                .HasName("AlternateKey_CountryName");

            builder.Entity<TimeEvent>()
                .HasAlternateKey(c => c.Name)
                .HasName("AlternateKey_TimeEventName");

            builder.Entity<FoodItem>();

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
