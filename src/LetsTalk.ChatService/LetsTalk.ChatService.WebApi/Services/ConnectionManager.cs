using System.Collections.Concurrent;

namespace LetsTalk.ChatService.WebApi.Services;

public sealed class ConnectionManager
{
    private readonly ConcurrentDictionary<string, List<string>> _userConnections = [];
    private readonly ConcurrentDictionary<string, string> _connectionUser = [];

    public void AddConnection(string userId, string connectionId)
    {
        _userConnections.AddOrUpdate(userId, CreateUserConnections, UpdateUserConnections, connectionId);
        _connectionUser[connectionId] = userId;

        static List<string> CreateUserConnections(string userId, string connectionId) => [connectionId];

        static List<string> UpdateUserConnections(string userId, List<string> connections, string connectionId)
        {
            connections.Add(connectionId);
            return connections;
        }
    }

    public void RemoveConnection(string connectionId)
    {
        if (_connectionUser.TryRemove(connectionId, out var userId))
            _ = _userConnections.TryRemove(userId, out _);
    }
}
