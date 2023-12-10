using System.Collections.Concurrent;

namespace LetsTalk.Services;
public sealed class HubConnectionManager
{
    private readonly ConcurrentDictionary<string, ConcurrentBag<string>> _userIdToConnections = [];

    public void Add(string userId, string connectionId)
    {
        var connections = _userIdToConnections.GetOrAdd(userId, _ => []);
        connections.Add(connectionId);
    }

    public void Remove(string userId) => _userIdToConnections.Remove(userId, out _);

    public IEnumerable<string> GetUsers() => _userIdToConnections.Keys;

    public IEnumerable<string> GetConnections() => _userIdToConnections.Values.SelectMany(v => v);

    public IEnumerable<string> GetConnections(string userId) => _userIdToConnections[userId];
}
