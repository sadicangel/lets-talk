namespace LetsTalk.Responses;

public sealed record class UserProfileResponse(string UserId, string UserName, string UserAvatar)
{
    public UserProfileResponse(User user) : this(user.Id, user.UserName, user.Avatar) { }
}