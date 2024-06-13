using Models;
using Models.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface ILeaderboardWeeklyQueries
{
	void InsertNewUser(int userId);
	//Task<List<LeaderboardWithUserDetails>> GetTop10Players();
	Task<List<LeaderboardWithUserDetails>> GetAllPlayers(int min, int max);
	Task<List<LeaderboardWithUserDetails>> GetPreviousAllPlayers();
	Task<List<LeaderboardForBotUpdate>> GetPlayersForBotUpdate(int min, int max);
	Task<List<IdAndUserId>> GetUserIdToLeaderboardId();
	Task<int> GetUserLeaderboardId(int userId);
	Task<bool> CheckIfUserExists(int userId);
	Task<float> GetPlayerStarsWon(int userId);
	Task UpdatePlayerStarsWon(int userId, int starsWon);
	Task UpdatePlayerStarsWonForBot(int userId, int gamesPlayed, int starsWon, DateTime updatedTime);
	void IncrementPlayerGamesPlayed(int userId);
	Task MoveDataToPreviousTable();
	void DeleteTable();
	Task DeleteUser(int userId);
}

