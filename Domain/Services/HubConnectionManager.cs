namespace LetsTalk.Services;
public sealed class HubConnectionManager
{
    private readonly ReaderWriterLockSlim _lock = new();
    private readonly Dictionary<string, List<string>> _userIdToConnections = [];

    public void Add(string userId, string connectionId)
    {
        _lock.EnterWriteLock();
        if (!_userIdToConnections.TryGetValue(userId, out var connections))
            _userIdToConnections[userId] = connections = [];
        connections.Add(connectionId);
        _lock.ExitWriteLock();
    }

    public void Remove(string userId)
    {
        _lock.EnterWriteLock();
        _userIdToConnections.Remove(userId);
        _lock.ExitWriteLock();
    }

    public IReadOnlyCollection<string> GetConnections()
    {
        _lock.EnterReadLock();
        var result = _userIdToConnections.Values.SelectMany(v => v).ToList();
        _lock.ExitReadLock();
        return result;
    }

    public IReadOnlyCollection<string> GetUsers()
    {
        _lock.EnterReadLock();
        var result = _userIdToConnections.Keys.ToList();
        _lock.ExitReadLock();
        return result;
    }

    public IReadOnlyList<string> GetConnectionsByUserId(string userId)
    {
        _lock.EnterReadLock();
        var result = _userIdToConnections[userId];
        _lock.ExitReadLock();
        return result;
    }
}
