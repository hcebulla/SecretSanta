
namespace SecretSanta.Web.Features.Drawing;

public static class MatchingAlgorithm
{
    public static IReadOnlyList<Assignment> Draw(
        IReadOnlyList<Participant> participants,
        IReadOnlyList<SpousePair> spousePairs)
    {
        ValidateInput(participants, spousePairs);

        var exclusions = BuildExclusions(
            participants, spousePairs);

        var eligibleRecipients = participants.ToDictionary(
            giver => giver.Id,
            giver => Shuffle(
                participants
                    .Where(recipient =>
                        !exclusions[giver.Id].Contains(recipient.Id))
                    .Select(recipient => recipient.Id)));

        var recipientToGiver = new Dictionary<Guid, Guid>();

        foreach (var giver in Shuffle(participants))
        {
            var visited = new HashSet<Guid>();

            if (!TryMatch(
                giver.Id,
                eligibleRecipients,
                recipientToGiver,
                visited))
            {
                throw new InvalidOperationException(
                    "No valid Secret Santa assignment exists.");
            }
        }

        var participantsById = participants.ToDictionary(p => p.Id);

        return recipientToGiver
            .Select(match => new Assignment(
                participantsById[match.Value],
                participantsById[match.Key]))
            .ToArray();
    }

    private static bool TryMatch(
        Guid giverId,
        Dictionary<Guid, Guid[]> eligibleRecipients,
        Dictionary<Guid, Guid> recipientToGiver,
        HashSet<Guid> visited)
    {
        foreach (var recipientId in eligibleRecipients[giverId])
        {
            if (!visited.Add(recipientId))
            {
                continue;
            }

            if (!recipientToGiver.TryGetValue(
                    recipientId, out var existingGiverId) ||
                TryMatch(
                    existingGiverId,
                    eligibleRecipients,
                    recipientToGiver,
                    visited))
            {
                recipientToGiver[recipientId] = giverId;
                return true;
            }
        }

        return false;
    }

    private static Dictionary<Guid, HashSet<Guid>> BuildExclusions(
        IReadOnlyList<Participant> participants,
        IReadOnlyList<SpousePair> spousePairs)
    {
        var exclusions = participants.ToDictionary(
            participant => participant.Id,
            participant => new HashSet<Guid> { participant.Id });

        foreach (var pair in spousePairs)
        {
            exclusions[pair.FirstParticipantId]
                .Add(pair.SecondParticipantId);

            exclusions[pair.SecondParticipantId]
                .Add(pair.FirstParticipantId);
        }

        return exclusions;
    }

    private static void ValidateInput(
        IReadOnlyList<Participant> participants,
        IReadOnlyList<SpousePair> spousePairs)
    {
        ArgumentNullException.ThrowIfNull(participants);
        ArgumentNullException.ThrowIfNull(spousePairs);

        if (participants.Count < 2)
        {
            throw new ArgumentException(
                "At least two participants are required.",
                nameof(participants));
        }

        var participantIds = participants
            .Select(participant => participant.Id)
            .ToHashSet();

        if (participantIds.Count != participants.Count)
        {
            throw new ArgumentException(
                "Participant IDs must be unique.",
                nameof(participants));
        }

        var pairedParticipants = new HashSet<Guid>();

        foreach (var pair in spousePairs)
        {
            if (!participantIds.Contains(pair.FirstParticipantId) ||
                !participantIds.Contains(pair.SecondParticipantId))
            {
                throw new ArgumentException(
                    "Spouse pairs must reference existing participants.",
                    nameof(spousePairs));
            }

            if (pair.FirstParticipantId == pair.SecondParticipantId)
            {
                throw new ArgumentException(
                    "Participants cannot be paired with themselves.",
                    nameof(spousePairs));
            }

            if (!pairedParticipants.Add(pair.FirstParticipantId) ||
                !pairedParticipants.Add(pair.SecondParticipantId))
            {
                throw new ArgumentException(
                    "Participants cannot belong to multiple spouse pairs.",
                    nameof(spousePairs));
            }
        }
    }

    private static T[] Shuffle<T>(IEnumerable<T> items)
    {
        var result = items.ToArray();

        Random.Shared.Shuffle(result.AsSpan());

        return result;
    }
}