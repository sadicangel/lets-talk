﻿namespace LetsTalk.ChannelService.WebApi.Entities;

public sealed class ChannelMember
{
    public required string ChannelId { get; init; }
    public Channel Channel { get; init; } = null!;
    public required string UserId { get; init; }
    public required DateTimeOffset MemberSince { get; init; }
    public required DateTimeOffset LastSeenAt { get; set; }
    public ChannelRole Role { get; set; }
    public ChannelMembershipStatus Status { get; set; }
    public string? InvitedByUserId { get; set; }
}

public enum ChannelRole
{
    Member,
    Moderator,
    Admin
}
public enum ChannelMembershipStatus
{
    Active,
    Muted,
    Banned,
    Invited,
    Pending,
}
