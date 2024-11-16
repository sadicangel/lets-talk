namespace LetsTalk.WebApi.Entities;

public sealed class Channel : IEntity
{
    public required Guid Id { get; set; }
    public required string DisplayName { get; set; }
    public required string? IconUrl { get; set; }
    public required Guid AdminId { get; set; }
    public User Admin { get; set; } = default!;
    public List<User> Members { get; set; } = [];
    public List<Message> Messages { get; set; } = [];

    public static Channel Create(string displayName, string? iconUrl, User admin) => new()
    {
        Id = Guid.CreateVersion7(),
        DisplayName = displayName,
        IconUrl = iconUrl,
        AdminId = admin.Id,
        Admin = admin,
    };
}
