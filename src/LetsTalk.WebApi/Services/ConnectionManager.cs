using LetsTalk.Domain;
using LetsTalk.WebApi.Entities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace LetsTalk.WebApi.Services;

internal sealed class ConnectionManager
{
    private readonly Lock _lock;
    private readonly Dictionary<string, UserDto> _usersById;
    private readonly Dictionary<string, ChannelDto> _channelsById;

    private readonly Dictionary<UserDto, List<ChannelDto>> _channelsByUser;
    private readonly Dictionary<ChannelDto, List<UserDto>> _usersByChannel;

    private readonly Dictionary<string, UserDto> _usersByConnection;
    private readonly Dictionary<UserDto, HashSet<string>> _connectionsByUser;

    public ConnectionManager(IServiceScopeFactory serviceScopeFactory)
    {
        using var serviceScope = serviceScopeFactory.CreateScope();
        var dbContext = serviceScope.ServiceProvider.GetRequiredService<LetsTalkDbContext>();

        var users = dbContext.Users.Include(u => u.Channels).ToDictionary(u => u.Id);
        var channels = dbContext.Channels.Include(c => c.Members).ToDictionary(c => c.Id);

        _lock = new();
        _usersById = users.ToDictionary(e => e.Key, e => e.Value.ToUserDto());
        _channelsById = channels.ToDictionary(e => e.Key, e => e.Value.ToChannelDto());

        _channelsByUser = _usersById.ToDictionary(e => e.Value, e => users[e.Key].Channels.Select(c => _channelsById[c.Id]).ToList());
        _usersByChannel = _channelsById.ToDictionary(e => e.Value, e => channels[e.Key].Members.Select(m => _usersById[m.Id]).ToList());

        _usersByConnection = [];
        _connectionsByUser = [];
    }

    public UserDto GetUser(string userId) => _usersById[userId];

    public ChannelDto GetChannel(string channelId) => _channelsById[channelId];

    public IReadOnlyCollection<UserDto> GetConnectedUsers() => _connectionsByUser.Keys;

    public IReadOnlyCollection<string> GetUserConnections(UserDto user) => _connectionsByUser.TryGetValue(user, out var connections) ? connections : [];

    public IReadOnlyCollection<ChannelDto> GetUserChannels(UserDto user) => _channelsByUser.TryGetValue(user, out var channels) ? channels : [];

    public IReadOnlyCollection<UserDto> GetChannelMembers(ChannelDto channel) => _usersByChannel.TryGetValue(channel, out var members) ? members : [];


    public UserDto AddConnection(HubCallerContext context)
    {
        if (_usersByConnection.TryGetValue(context.ConnectionId, out var user))
        {
            return user;
        }

        lock (_lock)
        {
            if (!_usersByConnection.TryGetValue(context.ConnectionId, out user))
            {
                user = _usersById[context.User.GetUserId()];
                user.IsOnline = true;

                _usersByConnection[context.ConnectionId] = user;
                _connectionsByUser[user] = [context.ConnectionId];
            }

            return user;
        }
    }

    public UserDto RemoveConnection(HubCallerContext context)
    {
        lock (_lock)
        {
            _usersByConnection.Remove(context.ConnectionId, out var user);
            ArgumentNullException.ThrowIfNull(user);
            _connectionsByUser[user].Remove(context.ConnectionId);
            if (_connectionsByUser[user].Count == 0)
            {
                user.IsOnline = false;
            }
            return user;
        }
    }
}
