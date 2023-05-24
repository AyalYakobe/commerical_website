using Microsoft.EntityFrameworkCore;
using SHP6.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;

namespace SHP6.DAL
{
    public class NewDbContext:DbContext
    {
        public NewDbContext(DbContextOptions<NewDbContext> options)
    : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationUser>().Ignore("PasswordConfirmed"); //No need for this in the DB
        }

        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}
