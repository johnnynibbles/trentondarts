namespace TrentonDarts.Web.Domain.Models;

public class MatchResult
{
    // Public context fields (set via From factory)
    public int SeasonId { get; set; }
    public SeasonPart? SeasonPart { get; set; }
    public string? Division { get; set; }
    public DateTime Date { get; set; }
    public int AwayTeamId { get; set; }
    public string AwayTeamName { get; set; } = string.Empty;
    public int HomeTeamId { get; set; }
    public string HomeTeamName { get; set; } = string.Empty;

    private int _matchId;
    private bool _hasScorecard;
    private int _awayScoreOverride;
    private int _homeScoreOverride;
    private readonly List<GameResult> _gameResults = new();
    private MatchRules? _matchRules;

    private MatchResult() { }

    public static MatchResult From(MatchResultSnapshot snapshot, MatchRules rules)
    {
        var result = new MatchResult();
        result.LoadSnapshot(snapshot);
        result.LoadRules(rules);
        return result;
    }

    public int GetMatchId() => _matchId;
    public bool GetHasScorecard() => _hasScorecard;
    public List<GameResult> GetGames() => _gameResults;

    public void SetHasScorecard(bool value) => _hasScorecard = value;

    public void SetAwayScoreOverride(int score)
    {
        if (_hasScorecard) throw new InvalidOperationException("Cannot override score when scorecard exists");
        _awayScoreOverride = score;
    }

    public void SetHomeScoreOverride(int score)
    {
        if (_hasScorecard) throw new InvalidOperationException("Cannot override score when scorecard exists");
        _homeScoreOverride = score;
    }

    private void LoadRules(MatchRules rules)
    {
        _matchRules = rules;
        if (_gameResults.Count == 0)
        {
            foreach (var gameRule in rules.GameRules)
                _gameResults.Add(new GameResult(gameRule));
        }
    }

    private void LoadSnapshot(MatchResultSnapshot snapshot)
    {
        _matchId = snapshot.MatchId;
        _hasScorecard = snapshot.HasScorecard;
        _awayScoreOverride = snapshot.AwayScoreOverride;
        _homeScoreOverride = snapshot.HomeScoreOverride;

        SeasonId = snapshot.SeasonId;
        SeasonPart = snapshot.SeasonPart;
        Division = snapshot.Division;
        Date = snapshot.Date;
        AwayTeamId = snapshot.AwayTeamId;
        AwayTeamName = snapshot.AwayTeamName;
        HomeTeamId = snapshot.HomeTeamId;
        HomeTeamName = snapshot.HomeTeamName;

        foreach (var gameSnap in snapshot.GameResults)
        {
            var game = new GameResult(gameSnap.GameRules);
            game.LoadSnapshot(gameSnap);
            _gameResults.Add(game);
        }
    }

    public int GetHomeScore()
    {
        if (!_hasScorecard) return _homeScoreOverride;
        return _gameResults.Sum(g => g.GetHomeScore());
    }

    public int GetAwayScore()
    {
        if (!_hasScorecard) return _awayScoreOverride;
        return _gameResults.Sum(g => g.GetAwayScore());
    }

    public int GetHomeMatchPoints()
    {
        if (_matchRules == null || !_matchRules.IsUsingMatchPoints) return 0;
        if (GetHomeScore() > GetAwayScore()) return _matchRules.WinPoints;
        if (GetHomeScore() >= _matchRules.MinPointForHalfPoints) return _matchRules.HalfPoints;
        return 0;
    }

    public int GetAwayMatchPoints()
    {
        if (_matchRules == null || !_matchRules.IsUsingMatchPoints) return 0;
        if (GetAwayScore() > GetHomeScore()) return _matchRules.WinPoints;
        if (GetAwayScore() >= _matchRules.MinPointForHalfPoints) return _matchRules.HalfPoints;
        return 0;
    }

    public GameResult? GetGameById(int id) =>
        _gameResults.FirstOrDefault(g => g.Id == id);

    public MatchResultSnapshot GetSnapshot()
    {
        var snap = new MatchResultSnapshot
        {
            MatchId = _matchId,
            SeasonId = SeasonId,
            SeasonPart = SeasonPart,
            Division = Division,
            Date = Date,
            AwayTeamId = AwayTeamId,
            AwayTeamName = AwayTeamName,
            HomeTeamId = HomeTeamId,
            HomeTeamName = HomeTeamName,
            HasScorecard = _hasScorecard,
            AwayScoreOverride = _awayScoreOverride,
            HomeScoreOverride = _homeScoreOverride,
        };
        foreach (var g in _gameResults)
            snap.GameResults.Add(g.GetSnapshot());
        return snap;
    }
}
