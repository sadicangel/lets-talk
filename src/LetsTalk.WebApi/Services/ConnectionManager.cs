using System.Collections.Concurrent;
using LetsTalk.WebApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.WebApi.Services;

internal sealed class ConnectionManager(IDbContextFactory<LetsTalkDbContext> dbContextFactory)
{
    private readonly ConcurrentDictionary<string, User> _connectedUsers = [];

    public ICollection<User> ConnectedUsers => _connectedUsers.Values;

    public User AddConnection(string connectionId, Guid userId)
    {
        return _connectedUsers.GetOrAdd(connectionId, _ =>
        {
            using var dbContext = dbContextFactory.CreateDbContext();
            return dbContext.Users.Single(user => user.Id == userId);
        });
    }

    public User RemoveConnection(string connectionId)
    {
        _connectedUsers.TryRemove(connectionId, out var user);
        ArgumentNullException.ThrowIfNull(user);
        return user;
    }
}
