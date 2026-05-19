using TrentonDarts.Web.Domain;

namespace TrentonDarts.Web.Services;

public class ScorecardService
{
    private readonly MatchRepository _matchRepo;
    private readonly UpdateMatchStatsService _statsService;

    public ScorecardService(MatchRepository matchRepo, UpdateMatchStatsService statsService)
    {
        _matchRepo = matchRepo;
        _statsService = statsService;
    }

    public async Task SubmitScorecardAsync(int matchId, ScorecardSaveDto data)
    {
        var matchResult = await _matchRepo.SaveMatchResultsDataAsync(matchId, data);
        await _statsService.UpdateAsync(matchResult);
    }
}
