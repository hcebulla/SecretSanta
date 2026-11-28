namespace SecretSanta.Web.Features.Drawing;

public sealed record SpousePair(
    Guid FirstParticipantId,
    Guid SecondParticipantId);