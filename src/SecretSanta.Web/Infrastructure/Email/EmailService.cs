using System.Net.Http.Headers;
using System.Net.Http.Json;

using SecretSanta.Web.Features.Drawing;

namespace SecretSanta.Web.Infrastructure.Email;

public sealed class EmailService(
    HttpClient httpClient,
    IConfiguration configuration) : IEmailService
{
    public async Task SendAssignmentAsync(
        Guid drawId,
        Assignment assignment,
        CancellationToken cancellationToken = default)
    {
        var apiKey = configuration["Email:ApiKey"]
            ?? throw new InvalidOperationException(
                "Email API key is not configured.");

        var from = configuration["Email:From"]
            ?? throw new InvalidOperationException(
                "Email sender is not configured.");

        var payload = new
        {
            from,
            to = new[] { assignment.Giver.Email },
            subject = "Your Secret Santa assignment!",
            text = $"""
                Merry Christmas, {assignment.Giver.Name}!

                You are the Secret Santa for:

                {assignment.Recipient.Name}

                Remember to keep your assignment a secret.

                Happy gifting!
                """
        };

        using var request = new HttpRequestMessage(
            HttpMethod.Post, "emails");

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", apiKey);

        request.Headers.Add(
            "Idempotency-Key",
            $"secret-santa/{drawId}/{assignment.Giver.Id}");

        request.Content = JsonContent.Create(payload);

        using var response = await httpClient.SendAsync(
            request, cancellationToken);

        response.EnsureSuccessStatusCode();
    }
}