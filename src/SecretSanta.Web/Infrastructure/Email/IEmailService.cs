using SecretSanta.Web.Features.Drawing;

namespace SecretSanta.Web.Infrastructure.Email;

public interface IEmailService
{
    Task SendAssignmentAsync(
        Guid drawId,
        Assignment assignment,
        CancellationToken cancellationToken = default);
}