using Models;
using Orleans;

namespace StarSolitaireTournament.Grains
{
	public interface ILeaderboardGrain : IGrainWithIntegerKey
	{
		Task<Task> AddItem(LeaderboardWithUserDetails item);
		Task<Task> UpdateItems(List<LeaderboardWithUserDetails> items);
		Task<List<LeaderboardWithUserDetails>> GetLeaderboardItems();
		Task<bool> IsDataSet();
	}
}
