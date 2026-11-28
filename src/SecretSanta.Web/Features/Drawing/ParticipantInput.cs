namespace SecretSanta.Web.Features.Drawing;

public sealed class ParticipantInput
{
    public Guid Id { get; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public Participant ToParticipant()
    {
        return new Participant(
            Id,
            Name.Trim(),
            Email.Trim());
    }
}