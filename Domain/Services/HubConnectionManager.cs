namespace LetsTalk.Services;
public sealed class HubConnectionManager
{
    private readonly ReaderWriterLockSlim _lock = new();
    private readonly Dictionary<string, UserProfile> _connectionIdToUser = [];
    private readonly Dictionary<string, List<string>> _userIdToConnections = [];

    public void Add(string connectionId, UserProfile user)
    {
        _lock.EnterWriteLock();
        _connectionIdToUser[connectionId] = user;
        if (!_userIdToConnections.TryGetValue(user.UserId, out var connections))
            _userIdToConnections[user.UserId] = connections = [connectionId];
        else
            connections.Add(connectionId);
        _lock.ExitWriteLock();
    }

    public void Remove(string connectionId)
    {
        _lock.EnterWriteLock();
        if (_connectionIdToUser.Remove(connectionId, out var user))
            if (_userIdToConnections.Remove(user.UserId, out var connections))
                foreach (var connection in connections)
                    _connectionIdToUser.Remove(connection);
        _lock.ExitWriteLock();
    }

    public void Remove(UserProfile user)
    {
        _lock.EnterWriteLock();
        if (_userIdToConnections.Remove(user.UserId, out var connections))
            foreach (var connection in connections)
                _connectionIdToUser.Remove(connection);
        _lock.ExitWriteLock();
    }

    public IReadOnlyCollection<string> GetConnections()
    {
        _lock.EnterReadLock();
        var result = _connectionIdToUser.Keys;
        _lock.ExitReadLock();
        return result;
    }

    public IReadOnlyCollection<UserProfile> GetUsers()
    {
        _lock.EnterReadLock();
        var result = _connectionIdToUser.Values;
        _lock.ExitReadLock();
        return result;
    }

    public UserProfile GetUserByConnectionId(string connectionId)
    {
        _lock.EnterReadLock();
        var result = _connectionIdToUser[connectionId];
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
