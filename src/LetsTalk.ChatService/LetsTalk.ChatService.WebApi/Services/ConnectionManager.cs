using System.Collections.Concurrent;
using LetsTalk.Shared;

namespace LetsTalk.ChatService.WebApi.Services;

public sealed class ConnectionManager
{
    private readonly ConcurrentDictionary<string, UserIdentity> _userIdentities = [];
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, object?>> _userConnections = [];
    private readonly ConcurrentDictionary<string, string> _connectionUser = [];

    public void AddConnection(UserIdentity user, string connectionId)
    {
        _userIdentities.TryAdd(user.UserId, user);
        _userConnections.AddOrUpdate(user.UserId, CreateUserConnections, UpdateUserConnections, connectionId);
        _connectionUser[connectionId] = user.UserId;

        static ConcurrentDictionary<string, object?> CreateUserConnections(string user, string connectionId) => new() { [connectionId] = null };

        static ConcurrentDictionary<string, object?> UpdateUserConnections(string user, ConcurrentDictionary<string, object?> connections, string connectionId)
        {
            _ = connections.TryAdd(connectionId, null);
            return connections;
        }
    }

    public void RemoveConnection(string connectionId)
    {
        if (_connectionUser.TryRemove(connectionId, out var userId))
        {
            if (_userConnections[userId].TryRemove(connectionId, out _) && _userConnections.Count == 0)
            {
                _ = _userConnections.TryRemove(userId, out _);
                _ = _userIdentities.TryRemove(userId, out _);
            }
        }
    }

    public ICollection<string> GetConnections(string userId) =>
        _userConnections.TryGetValue(userId, out var connections) ? connections.Keys : [];

    public UserIdentity GetOnlineUser(string userId) => _userIdentities[userId];

    public IEnumerable<UserIdentity> GetOnlineUsers() => _userIdentities.Values;
}
