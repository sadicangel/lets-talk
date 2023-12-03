namespace LetsTalk;

public sealed record class UserProfile(string UserId, string UserName, string? UserAvatar)
{
    public UserProfile(User user) : this(user.Id, user.UserName, user.Avatar)
    {

    }
}
