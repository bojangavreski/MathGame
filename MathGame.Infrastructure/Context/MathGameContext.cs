using MathGame.API.Entities.User;
using MathGame.Core.Entities;
using MathGame.Core.Entities.UserInSession;
using MathGame.Infrastructure.Configurations;
using MathGame.Infrastructure.Context.Interface;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MathGame.API;
public class MathGameContext : IdentityDbContext<User>, IMathGameContext
{
    public MathGameContext(DbContextOptions<MathGameContext> options) : base(options)
    {
    }

    #region Tables
    public DbSet<MathExpression> MathExpressions { get; set; }

    public DbSet<GameSession> GameSessions { get; set; }

    public DbSet<UserInGameSession> UsersInGameSession { get; set; }

    public DbSet<User> Users { get; set; }
    #endregion

    #region Configuration
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<MathExpression>().ToTable("MathExpression");
        builder.Entity<UserInGameSession>().ToTable("UserInGameSession");
        builder.Entity<User>().ToTable("AspNetUsers");

        builder.Entity<UserInGameSession>().HasOne(x => x.GameSession)
                                           .WithMany(x => x.UsersInGameSession)
                                           .HasForeignKey(x => x.GameSessionFk);

        builder.Entity<MathExpression>().HasOne(x => x.GameSession)
                                        .WithMany(x => x.MathExpressions)
                                        .HasForeignKey(x => x.GameSessionFk);

        builder.ApplyConfiguration(new GameSessionConfiguration());

        base.OnModelCreating(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();
        base.OnConfiguring(optionsBuilder);
    }
    #endregion

    public async Task SaveAsync()
    {
        await SaveChangesAsync();
    }
}
