using Microsoft.EntityFrameworkCore;
using PostlyApi.Entities;

namespace PostlyApi.Models
{
    public class PostlyContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }  
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Vote> Votes { get; set; }


        public PostlyContext(DbContextOptions<PostlyContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // Unique constraint for username
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // Follow follower relation
            modelBuilder.Entity<User>()
                .HasMany(u => u.Follower)
                .WithMany(u => u.Following)
                .UsingEntity(e => e.ToTable("Follows"));

            // Author 1 -- n posts
            modelBuilder.Entity<User>()
                .HasMany(u => u.Posts)
                .WithOne(p => p.Author)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Vote>()
                .HasKey(v => new { v.UserId, v.PostId });
        }
    }
}
