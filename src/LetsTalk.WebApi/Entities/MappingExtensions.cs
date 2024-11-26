using LetsTalk.Domain;

namespace LetsTalk.WebApi.Entities;

public static class MappingExtensions
{
    public static UserDto ToUserDto(this User user) =>
        new(user.Id, user.UserName, user.AvatarUrl);

    public static ChannelDto ToChannelDto(this Channel channel) =>
        new(channel.Id, channel.DisplayName, channel.IconUrl, channel.Admin.Id);
}
