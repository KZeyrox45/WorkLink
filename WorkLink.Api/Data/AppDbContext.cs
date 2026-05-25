using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WorkLink.Api.Models;

namespace WorkLink.Api.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<UserSkill> UserSkills => Set<UserSkill>();

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
    }
}
