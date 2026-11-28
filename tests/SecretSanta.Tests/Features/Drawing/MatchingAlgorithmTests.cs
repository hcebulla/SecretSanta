using SecretSanta.Web.Features.Drawing;

using Xunit;

namespace SecretSanta.Tests.Features.Drawing;

public sealed class MatchingAlgorithmTests
{
    [Fact]
    public void Draw_TwoParticipants_AssignsEachOther()
    {
        // Arrange
        var alice = CreateParticipant("Alice");
        var bob = CreateParticipant("Bob");

        Participant[] participants = [alice, bob];

        // Act
        var assignments = MatchingAlgorithm.Draw(
            participants, []);

        // Assert
        Assert.Equal(2, assignments.Count);

        Assert.Contains(assignments, assignment =>
            assignment.Giver.Id == alice.Id &&
            assignment.Recipient.Id == bob.Id);

        Assert.Contains(assignments, assignment =>
            assignment.Giver.Id == bob.Id &&
            assignment.Recipient.Id == alice.Id);
    }

    [Fact]
    public void Draw_TwoCouples_ProducesValidAssignments()
    {
        // Arrange
        var alice = CreateParticipant("Alice");
        var bob = CreateParticipant("Bob");
        var carol = CreateParticipant("Carol");
        var david = CreateParticipant("David");

        Participant[] participants =
            [alice, bob, carol, david];

        SpousePair[] spousePairs =
        [
            new(alice.Id, bob.Id),
            new(carol.Id, david.Id)
        ];

        // Act
        var assignments = MatchingAlgorithm.Draw(
            participants, spousePairs);

        // Assert
        Assert.Equal(participants.Length, assignments.Count);

        Assert.All(assignments, assignment =>
            Assert.NotEqual(
                assignment.Giver.Id,
                assignment.Recipient.Id));

        Assert.Equal(
            participants.Length,
            assignments.Select(a => a.Giver.Id).Distinct().Count());

        Assert.Equal(
            participants.Length,
            assignments.Select(a => a.Recipient.Id).Distinct().Count());

        Assert.All(assignments, assignment =>
            Assert.DoesNotContain(spousePairs, pair =>
                (pair.FirstParticipantId == assignment.Giver.Id &&
                 pair.SecondParticipantId == assignment.Recipient.Id) ||
                (pair.SecondParticipantId == assignment.Giver.Id &&
                 pair.FirstParticipantId == assignment.Recipient.Id)));
    }

    [Fact]
    public void Draw_OnlyOneCouple_ThrowsInvalidOperationException()
    {
        // Arrange
        var alice = CreateParticipant("Alice");
        var bob = CreateParticipant("Bob");

        Participant[] participants = [alice, bob];
        SpousePair[] spousePairs = [new(alice.Id, bob.Id)];

        // Act and assert
        Assert.Throws<InvalidOperationException>(() =>
            MatchingAlgorithm.Draw(participants, spousePairs));
    }

    [Fact]
    public void Draw_EmptyParticipants_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() =>
            MatchingAlgorithm.Draw([], []));
    }

    [Fact]
    public void Draw_OneParticipant_ThrowsArgumentException()
    {
        Participant[] participants = [CreateParticipant("Alice")];

        Assert.Throws<ArgumentException>(() =>
            MatchingAlgorithm.Draw(participants, []));
    }

    [Fact]
    public void Draw_DuplicateParticipantIds_ThrowsArgumentException()
    {
        var alice = CreateParticipant("Alice");
        var bob = CreateParticipant("Bob") with { Id = alice.Id };

        Assert.Throws<ArgumentException>(() =>
            MatchingAlgorithm.Draw([alice, bob], []));
    }

    [Fact]
    public void Draw_DuplicateNames_ProducesValidAssignments()
    {
        var first = CreateParticipant("John");
        var second = CreateParticipant("John");

        var assignments = MatchingAlgorithm.Draw(
            [first, second], []);

        AssertValidAssignments([first, second], [], assignments);
    }

    [Fact]
    public void Draw_UnknownSpouse_ThrowsArgumentException()
    {
        var alice = CreateParticipant("Alice");
        var bob = CreateParticipant("Bob");

        SpousePair[] spousePairs = [new(alice.Id, Guid.NewGuid())];

        Assert.Throws<ArgumentException>(() =>
            MatchingAlgorithm.Draw([alice, bob], spousePairs));
    }

    [Fact]
    public void Draw_SelfReferencingSpouse_ThrowsArgumentException()
    {
        var alice = CreateParticipant("Alice");
        var bob = CreateParticipant("Bob");

        SpousePair[] spousePairs = [new(alice.Id, alice.Id)];

        Assert.Throws<ArgumentException>(() =>
            MatchingAlgorithm.Draw([alice, bob], spousePairs));
    }

    [Fact]
    public void Draw_MultipleSpouses_ThrowsArgumentException()
    {
        var alice = CreateParticipant("Alice");
        var bob = CreateParticipant("Bob");
        var carol = CreateParticipant("Carol");

        SpousePair[] spousePairs =
        [
            new(alice.Id, bob.Id),
            new(alice.Id, carol.Id)
        ];

        Assert.Throws<ArgumentException>(() =>
            MatchingAlgorithm.Draw([alice, bob, carol], spousePairs));
    }

    [Fact]
    public void Draw_ThreeParticipantsWithCouple_ThrowsInvalidOperationException()
    {
        var alice = CreateParticipant("Alice");
        var bob = CreateParticipant("Bob");
        var carol = CreateParticipant("Carol");

        SpousePair[] spousePairs = [new(alice.Id, bob.Id)];

        Assert.Throws<InvalidOperationException>(() =>
            MatchingAlgorithm.Draw([alice, bob, carol], spousePairs));
    }

    private static Participant CreateParticipant(string name)
    {
        return new Participant(
            Guid.NewGuid(),
            name,
            $"{name.ToLowerInvariant()}@example.com");
    }

    private static void AssertValidAssignments(
        IReadOnlyList<Participant> participants,
        IReadOnlyList<SpousePair> spousePairs,
        IReadOnlyList<Assignment> assignments)
    {
        Assert.Equal(participants.Count, assignments.Count);

        var expectedIds = participants
            .Select(p => p.Id)
            .Order()
            .ToArray();

        Assert.Equal(
            expectedIds,
            assignments.Select(a => a.Giver.Id).Order().ToArray());

        Assert.Equal(
            expectedIds,
            assignments.Select(a => a.Recipient.Id).Order().ToArray());

        Assert.All(assignments, assignment =>
        {
            Assert.NotEqual(
                assignment.Giver.Id,
                assignment.Recipient.Id);

            Assert.DoesNotContain(spousePairs, pair =>
                (pair.FirstParticipantId == assignment.Giver.Id &&
                 pair.SecondParticipantId == assignment.Recipient.Id) ||
                (pair.SecondParticipantId == assignment.Giver.Id &&
                 pair.FirstParticipantId == assignment.Recipient.Id));
        });
    }
}