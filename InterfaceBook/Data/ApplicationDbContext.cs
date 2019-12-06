using System;
using System.Collections.Generic;
using System.Text;
using InterfaceBook.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InterfaceBook.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<UserPost> UserPosts { get; set; }
        public DbSet<Comment> Comments { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /*protected override void OnModelCreating(ModelBuilder model)
        {
            model.Entity<UserPost>()
                .HasOne(p => p.User)
                .WithMany()
                .OnDelete(DeleteBehavior.Cascade);

            model.Entity<UserPost>()
                .HasMany(p => p.Comments)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
        }*/
    }
}
