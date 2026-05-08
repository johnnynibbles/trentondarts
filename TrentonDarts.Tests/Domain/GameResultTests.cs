using FluentAssertions;
using TrentonDarts.Web.Domain.Models;

namespace TrentonDarts.Tests.Domain;

public class GameResultTests
{
    private static GamePlayer GetPlayer(int id, string name) =>
        new GamePlayer { Id = id, Name = name };

    [Fact]
    public void AddingAPlayer_IsAvailableOnGame()
    {
        var gameRules = new GameRules { NumberOfPlayers = 1 };
        var gameResult = new GameResult(gameRules);

        gameResult.SetHomePlayer(GetPlayer(1, "First Player"), 0);
        gameResult.SetAwayPlayer(GetPlayer(2, "Second Player"), 0);

        var snapshot = gameResult.GetSnapshot();
        snapshot.HomePlayers.Count(p => p != null).Should().Be(1);
        snapshot.AwayPlayers.Count(p => p != null).Should().Be(1);
    }

    [Fact]
    public void RemovingAHomePlayer_IsNoLongerOnGame()
    {
        var gameRules = new GameRules { NumberOfPlayers = 2 };
        var gameResult = new GameResult(gameRules);

        gameResult.SetHomePlayer(GetPlayer(1, "First Player"), 0);
        gameResult.SetAwayPlayer(GetPlayer(2, "Second Player"), 0);
        gameResult.SetHomePlayer(GetPlayer(3, "Third Player"), 1);

        gameResult.RemoveHomePlayerAtPosition(1);

        gameResult.GetSnapshot().HomePlayers.Count(p => p != null).Should().Be(1);
    }

    [Fact]
    public void RemovingAnAwayPlayer_IsNoLongerOnGame()
    {
        var gameRules = new GameRules { NumberOfPlayers = 2 };
        var gameResult = new GameResult(gameRules);

        gameResult.SetHomePlayer(GetPlayer(1, "First Player"), 0);
        gameResult.SetAwayPlayer(GetPlayer(2, "Second Player"), 0);
        gameResult.SetAwayPlayer(GetPlayer(3, "Third Player"), 1);

        gameResult.RemoveAwayPlayerAtPosition(1);

        gameResult.GetSnapshot().AwayPlayers.Count(p => p != null).Should().Be(1);
    }

    [Fact]
    public void TooManyHomePlayers_DetectedByHasTooManyPlayers()
    {
        var gameRules = new GameRules { NumberOfPlayers = 1 };
        var gameResult = new GameResult(gameRules);

        gameResult.SetHomePlayer(GetPlayer(1, "First Player"), 0);

        gameResult.HasTooManyPlayers(gameResult.HomePlayers).Should().BeTrue();
    }

    [Fact]
    public void TooManyAwayPlayers_DetectedByHasTooManyPlayers()
    {
        var gameRules = new GameRules { NumberOfPlayers = 1 };
        var gameResult = new GameResult(gameRules);

        gameResult.SetAwayPlayer(GetPlayer(1, "First Player"), 0);

        gameResult.HasTooManyPlayers(gameResult.AwayPlayers).Should().BeTrue();
    }

    [Fact]
    public void WhenAddingLegResult_ItIsAvailableOnSnapshot()
    {
        var gameRules = new GameRules { NumberOfPlayers = 1, NumberOfLegs = 1 };
        var gameResult = new GameResult(gameRules);

        gameResult.AddLeg(0, "home");

        gameResult.GetSnapshot().Legs.Count(l => !string.IsNullOrEmpty(l)).Should().Be(1);
    }

    [Fact]
    public void WhenAddingTooManyLegs_ThrowsException()
    {
        var gameRules = new GameRules { NumberOfPlayers = 1, NumberOfLegs = 1 };
        var gameResult = new GameResult(gameRules);

        gameResult.AddLeg(0, "home");

        var act = () => gameResult.AddLeg(1, "away");
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void WhenGameHasPointValueAndNoBestOf_HomeScoreShouldBeOne()
    {
        var gameRules = new GameRules { NumberOfPlayers = 1, NumberOfLegs = 1, GamePointValue = 1 };
        var gameResult = new GameResult(gameRules);

        gameResult.SetHomePlayer(GetPlayer(1, "Player 1"), 0);
        gameResult.SetAwayPlayer(GetPlayer(2, "Player 2"), 0);
        gameResult.AddLeg(0, "home");

        gameResult.GetHomeScore().Should().Be(1);
    }

    [Fact]
    public void WhenGameHasPointValueAndNoBestOf_AwayScoreShouldBeOne()
    {
        var gameRules = new GameRules { NumberOfPlayers = 1, NumberOfLegs = 1, GamePointValue = 1 };
        var gameResult = new GameResult(gameRules);

        gameResult.SetHomePlayer(GetPlayer(1, "Player 1"), 0);
        gameResult.SetAwayPlayer(GetPlayer(2, "Player 2"), 0);
        gameResult.AddLeg(0, "away");

        gameResult.GetAwayScore().Should().Be(1);
    }

    [Fact]
    public void GamePointValueIsUsedForScore()
    {
        var gameRules = new GameRules { NumberOfPlayers = 1, NumberOfLegs = 1, GamePointValue = 3 };
        var gameResult = new GameResult(gameRules);

        gameResult.SetHomePlayer(GetPlayer(1, "Player 1"), 0);
        gameResult.SetAwayPlayer(GetPlayer(2, "Player 2"), 0);
        gameResult.AddLeg(0, "away");

        gameResult.GetAwayScore().Should().Be(3);
    }
}
