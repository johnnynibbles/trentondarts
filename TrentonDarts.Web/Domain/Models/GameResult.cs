namespace TrentonDarts.Web.Domain.Models;

public class GameResult
{
    public int Id { get; set; }
    // Nullable entries preserve index-based positions (position 0, 1, 2 for triples)
    public List<GamePlayer?> HomePlayers { get; private set; } = new();
    public List<GamePlayer?> AwayPlayers { get; private set; } = new();
    public List<string> Legs { get; private set; } = new();
    public string? ForfeitedBy { get; set; }
    public GameRules GameRules { get; private set; }
    public List<GameAward> Awards { get; set; } = new();

    public GameResult(GameRules gameRules)
    {
        GameRules = gameRules;
    }

    public void SetHomePlayer(GamePlayer player, int position)
    {
        EnsureCapacity(HomePlayers, position + 1);
        HomePlayers[position] = player;
    }

    public void SetAwayPlayer(GamePlayer player, int position)
    {
        EnsureCapacity(AwayPlayers, position + 1);
        AwayPlayers[position] = player;
    }

    public void RemoveHomePlayerAtPosition(int index)
    {
        if (index < HomePlayers.Count)
            HomePlayers[index] = null;
    }

    public void RemoveAwayPlayerAtPosition(int index)
    {
        if (index < AwayPlayers.Count)
            AwayPlayers[index] = null;
    }

    public void AddLeg(int legNumber, string winner)
    {
        if (Legs.Count >= GameRules.NumberOfLegs)
            throw new InvalidOperationException("Too many legs");
        EnsureCapacity(Legs, legNumber + 1);
        Legs[legNumber] = winner;
    }

    public void SetLegs(List<string> legs) => Legs = legs;

    public int GetHomeScore()
    {
        if (GameRules.GamePointValue <= 0) return 0;
        return DidHomeTeamWin() ? GameRules.GamePointValue : 0;
    }

    public int GetAwayScore()
    {
        if (GameRules.GamePointValue <= 0) return 0;
        return DidAwayTeamWin() ? GameRules.GamePointValue : 0;
    }

    public bool HasTooManyPlayers(List<GamePlayer?> players) =>
        players.Count(p => p != null) >= GameRules.NumberOfPlayers;

    public GameResultSnapshot GetSnapshot()
    {
        return new GameResultSnapshot
        {
            Id = Id,
            HomePlayers = new List<GamePlayer?>(HomePlayers),
            AwayPlayers = new List<GamePlayer?>(AwayPlayers),
            Legs = new List<string>(Legs),
            ForfeitedBy = ForfeitedBy,
            GameRules = GameRules,
            Awards = new List<GameAward>(Awards)
        };
    }

    public void LoadSnapshot(GameResultSnapshot snapshot)
    {
        Id = snapshot.Id;
        HomePlayers = snapshot.HomePlayers;
        AwayPlayers = snapshot.AwayPlayers;
        Legs = snapshot.Legs;
        ForfeitedBy = snapshot.ForfeitedBy;
        Awards = snapshot.Awards;
    }

    private bool DidHomeTeamWin()
    {
        var homeLegs = LegsWonByTeam("home");
        var awayLegs = LegsWonByTeam("away");
        return homeLegs > awayLegs;
    }

    private bool DidAwayTeamWin()
    {
        var homeLegs = LegsWonByTeam("home");
        var awayLegs = LegsWonByTeam("away");
        return awayLegs > homeLegs;
    }

    private int LegsWonByTeam(string teamType) =>
        Legs.Count(l => l == teamType);

    private static void EnsureCapacity<T>(List<T?> list, int minCount) where T : class
    {
        while (list.Count < minCount)
            list.Add(null);
    }

    private static void EnsureCapacity(List<string> list, int minCount)
    {
        while (list.Count < minCount)
            list.Add(string.Empty);
    }
}
