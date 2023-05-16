using Microsoft.EntityFrameworkCore;

namespace PostlyApi.Models
{
    public class PostlyContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }


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

            // Upvotes n -- n User
            modelBuilder.Entity<Post>()
                .HasMany(p => p.UpvotedBy)
                .WithMany(u => u.UpvotedPosts)
                .UsingEntity(e => e.ToTable("Upvotes"));

            // Downvotes n -- n User
            modelBuilder.Entity<Post>()
                .HasMany(p => p.DownvotedBy)
                .WithMany(u => u.DownvotedPosts)
                .UsingEntity(e => e.ToTable("Downvotes"));
        }
    }
}
