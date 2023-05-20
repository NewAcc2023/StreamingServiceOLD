using Microsoft.EntityFrameworkCore;
using StreamingService.Models;

namespace StreamingService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<LikedDislikedVideoVote> LikedDislikedVideoVotes { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<LikedDislikedCommentVote> LikedDislikedCommentVotes { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LikedDislikedCommentVote>().HasKey(x => new { x.UserId, x.CommentId });
            modelBuilder.Entity<LikedDislikedVideoVote>().HasKey(x => new { x.UserId, x.VideoId });
            modelBuilder.Entity<Subscription>().HasKey(x => new { x.CreatorId, x.FollowerId });
            modelBuilder.Entity<Video>()
                .HasOne(c => c.User)
                .WithMany(p => p.Videos)
                .HasForeignKey(c => c.CreatorId);

            modelBuilder.Entity<Subscription>()
                .HasOne(ba => ba.User1)
                .WithMany(b => b.s1)
                .HasForeignKey(ba => ba.CreatorId);

            modelBuilder.Entity<Subscription>()
                .HasOne(ba => ba.User2)
                .WithMany(a => a.s2)
                .HasForeignKey(ba => ba.FollowerId);
            // ... other model configurations ...
        }

    }
}
