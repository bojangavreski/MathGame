using MathGame.API.Entities.User;
using MathGame.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace MathGame.Infrastructure.Context.Interface;
public interface IMathGameContext
{
    DbSet<GameSession> GameSessions { get; set; }

    DbSet<User> Users { get; set; }

    DbSet<T> Set<T>() where T : class;

    Task SaveAsync();
}
