using Orleans;

namespace MasterServer.ClusterUniqueService
{
	public interface IUniqueServiceGrain : IGrainWithGuidKey
	{
		Task StartReminderToUpdateLeaderboard();
		//Task StartReminderToGetUserIdToLeaderboardId();
		Task StartReminderForLeaderboardWeeklyCreditWinnersBalances();
		Task StartReminderToUpdateBotsStar();
		Task UpdateLeaderboardItems();
		Task LeaderboardWeeklyCreditWinnersBalances();
		Task UpdateBotsStars(int customRange = -1);
		Task StartReminders();

		Task<Task> SaveState();
		Task<Task> ClearState();
	}
}
