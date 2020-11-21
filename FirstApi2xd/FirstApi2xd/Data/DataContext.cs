using System;
using System.Collections.Generic;
using System.Text;
using FirstApi2xd.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FirstApi2xd.Data
{
    //Add-Migration AddedPostsInDatabase
    public class DataContext : IdentityDbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Tags> Tags { get; set; }
        public DbSet<PostTag> PostTag { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<PostTag>().Ignore(xx => xx.Post).HasKey(x => new {x.PostId, x.TagName});
        }
    }
}
