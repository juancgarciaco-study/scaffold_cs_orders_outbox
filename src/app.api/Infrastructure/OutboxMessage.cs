namespace app.api.Infrastructure;

public class OutboxMessage
{
    public long Id { get; set; }

    public Ulid MessageId { get; set; }
    public string MessageType { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    public string? JsonContent { get; set; }
    public DateTime OccurredOnUtc { get; set; }
    public DateTime? ProcessedOnUtc { get; set; }
    public string? Error { get; set; }
}
