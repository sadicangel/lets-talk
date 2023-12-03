namespace LetsTalk.Events;
public sealed record class ChannelMessageBroadcast(Channel Channel, UserProfile UserProfile, Message Message);