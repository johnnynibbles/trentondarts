using FluentAssertions;
using TrentonDarts.Web.Domain.Models;

namespace TrentonDarts.Tests.Domain;

public class MatchResultTests
{
    // Standard match: 3×301@1pt + 3×cricket@1pt + 3×501@2pt + 1×801@3pt
    //               + 3×doublecricket@2pt + 3×cricket@1pt + 3×301@1pt = 27 total points

    private static GameRules Make801() =>
        new() { NumberOfLegs = 1, NumberOfPlayers = 3, GameType = "801", GamePointValue = 3 };

    private static GameRules Make501() =>
        new() { NumberOfLegs = 1, NumberOfPlayers = 2, GameType = "501", GamePointValue = 2 };

    private static GameRules Make301() =>
        new() { NumberOfLegs = 1, NumberOfPlayers = 1, GameType = "301", GamePointValue = 1 };

    private static GameRules MakeCricket() =>
        new() { NumberOfLegs = 1, NumberOfPlayers = 1, GameType = "cricket", GamePointValue = 1 };

    private static GameRules MakeDoublesCricket() =>
        new() { NumberOfLegs = 1, NumberOfPlayers = 2, GameType = "cricket", GamePointValue = 2 };

    [Fact]
    public void FromWithRules_HasSameNumberOfGamesAsRules()
    {
        var matchRules = new MatchRules();
        matchRules.GameRules.Add(new GameRules());
        var matchResult = MatchResult.From(new MatchResultSnapshot(), matchRules);

        matchResult.GetGames().Count.Should().Be(matchRules.GameRules.Count);
    }

    [Fact]
    public void GetHomeTotalScore_WhenHomeSweeps_Is27()
    {
        var matchResult = CreateStandardMatchWithSweep("home");
        matchResult.GetHomeScore().Should().Be(27);
    }

    [Fact]
    public void GetAwayTotalScore_WhenAwaySweeps_Is27()
    {
        var matchResult = CreateStandardMatchWithSweep("away");
        matchResult.GetAwayScore().Should().Be(27);
    }

    private static MatchResult CreateStandardMatch()
    {
        var matchRules = new MatchRules();

        for (int i = 0; i < 3; i++) matchRules.GameRules.Add(Make301());
        for (int i = 0; i < 3; i++) matchRules.GameRules.Add(MakeCricket());
        for (int i = 0; i < 3; i++) matchRules.GameRules.Add(Make501());
        matchRules.GameRules.Add(Make801());
        for (int i = 0; i < 3; i++) matchRules.GameRules.Add(MakeDoublesCricket());
        for (int i = 0; i < 3; i++) matchRules.GameRules.Add(MakeCricket());
        for (int i = 0; i < 3; i++) matchRules.GameRules.Add(Make301());

        return MatchResult.From(new MatchResultSnapshot(), matchRules);
    }

    private static MatchResult CreateStandardMatchWithSweep(string teamType)
    {
        var matchResult = CreateStandardMatch();
        matchResult.SetHasScorecard(true);
        foreach (var game in matchResult.GetGames())
            game.AddLeg(0, teamType);
        return matchResult;
    }
}
