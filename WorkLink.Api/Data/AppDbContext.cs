using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WorkLink.Api.Models;

namespace WorkLink.Api.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<UserSkill> UserSkills => Set<UserSkill>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<JobSkill> JobSkills => Set<JobSkill>();
    public DbSet<Proposal> Proposals => Set<Proposal>();
    public DbSet<Review> Reviews => Set<Review>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UserSkill>(entity =>
        {
            entity.HasKey(us => new { us.UserId, us.SkillId });

            entity.HasOne(us => us.User)
                  .WithMany(u => u.UserSkills)
                  .HasForeignKey(us => us.UserId);

            entity.HasOne(us => us.Skill)
                  .WithMany(s => s.UserSkills)
                  .HasForeignKey(us => us.SkillId);
        });

        builder.Entity<JobSkill>(entity =>
        {
            entity.HasKey(js => new { js.JobId, js.SkillId });

            entity.HasOne(js => js.Job)
                  .WithMany(j => j.JobSkills)
                  .HasForeignKey(js => js.JobId);

            entity.HasOne(js => js.Skill)
                  .WithMany()
                  .HasForeignKey(js => js.SkillId);
        });

        builder.Entity<Job>(entity =>
        {
            entity.Property(j => j.BudgetMin).HasPrecision(18, 2);
            entity.Property(j => j.BudgetMax).HasPrecision(18, 2);

            entity.HasOne(j => j.Client)
                  .WithMany(u => u.Jobs)
                  .HasForeignKey(j => j.ClientId)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(j => j.Category)
                  .WithMany()
                  .HasForeignKey(j => j.CategoryId)
                  .OnDelete(DeleteBehavior.NoAction);
        });

        builder.Entity<Category>(entity =>
        {
            entity.HasIndex(c => c.Slug).IsUnique();
        });

        builder.Entity<Proposal>(entity =>
        {
            entity.Property(p => p.BidAmount).HasPrecision(18, 2);

            entity.HasOne(p => p.Job)
                  .WithMany(j => j.Proposals)
                  .HasForeignKey(p => p.JobId)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(p => p.Freelancer)
                  .WithMany(u => u.Proposals)
                  .HasForeignKey(p => p.FreelancerId)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasIndex(p => new { p.JobId, p.FreelancerId }).IsUnique();
        });

        builder.Entity<Review>(entity =>
        {
            entity.Property(r => r.Rating).HasDefaultValue(5);
            entity.Property(r => r.Comment).HasMaxLength(2000);

            entity.HasOne(r => r.Job)
                  .WithMany()
                  .HasForeignKey(r => r.JobId)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(r => r.Reviewer)
                  .WithMany(u => u.ReviewsGiven)
                  .HasForeignKey(r => r.ReviewerId)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(r => r.Reviewee)
                  .WithMany(u => u.ReviewsReceived)
                  .HasForeignKey(r => r.RevieweeId)
                  .OnDelete(DeleteBehavior.NoAction);

            entity.HasIndex(r => new { r.JobId, r.ReviewerId }).IsUnique();
        });
    }
}
