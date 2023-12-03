namespace LetsTalk.Events;

public sealed record class UserJoined(Channel Channel, UserProfile UserProfile);