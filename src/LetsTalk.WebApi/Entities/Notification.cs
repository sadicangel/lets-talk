namespace LetsTalk.WebApi.Entities;

public sealed class Notification
{
    public required string Id { get; set; }
    public required DateTimeOffset Timestamp { get; set; }
    public required string ContentType { get; set; }
    public required byte[] Content { get; set; }
}
