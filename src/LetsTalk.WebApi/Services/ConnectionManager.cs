using System.Collections.Concurrent;
using LetsTalk.WebApi.Entities;

namespace LetsTalk.WebApi.Services;

internal sealed class ConnectionManager(IServiceScopeFactory serviceScopeFactory)
{
    private readonly ConcurrentDictionary<string, User> _connectedUsers = [];

    public ICollection<User> ConnectedUsers => _connectedUsers.Values;

    public User AddConnection(string connectionId, Guid userId)
    {
        return _connectedUsers.GetOrAdd(connectionId, _ =>
        {
            using var serviceScope = serviceScopeFactory.CreateScope();
            var dbContext = serviceScope.ServiceProvider.GetRequiredService<LetsTalkDbContext>();
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
