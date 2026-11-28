namespace SecretSanta.Web.Features.Drawing;

public sealed record Participant(
    Guid Id,
    string Name,
    string Email);