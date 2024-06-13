using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models.DatabaseModels;
using Newtonsoft.Json;
using Models;


public class LeaderboardWeeklyQueries : ILeaderboardWeeklyQueries
{
	//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	#region insert

	public async void InsertNewUser(int userId)
	{
		using (var connection = new NpgsqlConnection(ConnectionString.CONNECTION_STRING))
		{
			await connection.OpenAsync();

			// insert query
			var sql = $"INSERT INTO leaderboard_weekly(user_id, games_played, stars_won) VALUES (@a, @b, @c, @d);";
			DynamicParameters dp = new DynamicParameters();
			dp.Add("a", userId);
			dp.Add("b", 0);
			dp.Add("c", 0);

			var result = await connection.ExecuteAsync(sql, dp);
			Console.WriteLine($"result : {result}");
		}
	}

	#endregion

	//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	#region select

	//public async Task<List<LeaderboardWithUserDetails>> GetTop10Players()
	//{
	//	using (var connection = new NpgsqlConnection(ConnectionString.CONNECTION_STRING))
	//	{
	//		await connection.OpenAsync();
	//		//SELECT TOP 1 * FROM[TABLENAME] ORDER BY id DESC
	//		var sql = "SELECT username, profile_url, games_played, stars_won FROM users, leaderboard_weekly WHERE id = user_id ORDER BY stars_won DESC, games_played DESC LIMIT 10;";
	//		var users = await connection.QueryAsync<LeaderboardWithUserDetails>(sql);
	//		List<LeaderboardWithUserDetails> usersList = users.ToList();
	//		return usersList;
	//	}
	//}
	/// <summary>
	/// 
	/// </summary>
	/// <param name="min">included</param>
	/// <param name="max">excluced</param>
	/// <returns></returns>
	public async Task<List<LeaderboardWithUserDetails>> GetAllPlayers(int min, int max)
	{
		using (var connection = new NpgsqlConnection(ConnectionString.CONNECTION_STRING))
		{
			await connection.OpenAsync();
			//SELECT TOP 1 * FROM[TABLENAME] ORDER BY id DESC
			var sql = "SELECT username, profile_url, games_played, stars_won FROM users, leaderboard_weekly WHERE leaderboard_weekly.user_id = users.id AND user_id >= @a AND user_id < @b ORDER BY stars_won DESC, games_played;";
			DynamicParameters dp = new DynamicParameters();
			dp.Add("a", min);
			dp.Add("b", max);
			var users = await connection.QueryAsync<LeaderboardWithUserDetails>(sql, dp);
			List<LeaderboardWithUserDetails> usersList = users.ToList();
			return usersList;
		}
	}

	public async Task<List<LeaderboardWithUserDetails>> GetPreviousAllPlayers()
	{
		using (var connection = new NpgsqlConnection(ConnectionString.CONNECTION_STRING))
		{
			await connection.OpenAsync();
			//SELECT TOP 1 * FROM[TABLENAME] ORDER BY id DESC
			var sql = "SELECT username, profile_url, games_played, stars_won FROM users, leaderboard_previous_weekly WHERE leaderboard_previous_weekly.user_id = users.id ORDER BY stars_won DESC, games_played;";
			var users = await connection.QueryAsync<LeaderboardWithUserDetails>(sql);
			List<LeaderboardWithUserDetails> usersList = users.ToList();
			return usersList;
		}
	}

	public async Task<List<LeaderboardForBotUpdate>> GetPlayersForBotUpdate(int min, int max)
	{
		using (var connection = new NpgsqlConnection(ConnectionString.CONNECTION_STRING))
		{
			await connection.OpenAsync();
			//SELECT TOP 1 * FROM[TABLENAME] ORDER BY id DESC
			var sql = "SELECT user_id, is_bot, games_played, stars_won FROM leaderboard_weekly WHERE user_id >= @a AND user_id < @b;";
			DynamicParameters dp = new DynamicParameters();
			dp.Add("a", min);
			dp.Add("b", max);
			var users = await connection.QueryAsync<LeaderboardForBotUpdate>(sql, dp);
			List<LeaderboardForBotUpdate> usersList = users.ToList();
			return usersList;
		}
	}

	public async Task<List<IdAndUserId>> GetUserIdToLeaderboardId()
	{
		using (var connection = new NpgsqlConnection(ConnectionString.CONNECTION_STRING))
		{
			await connection.OpenAsync();
			//SELECT TOP 1 * FROM[TABLENAME] ORDER BY id DESC
			var sql = "select user_id, id from leaderboard_weekly;";
			var ids = await connection.QueryAsync<IdAndUserId>(sql);
			List<IdAndUserId> idsList = ids.ToList();
			return idsList;
		}
	}

	public async Task<int> GetUserLeaderboardId(int userId)
	{
		using (var connection = new NpgsqlConnection(ConnectionString.CONNECTION_STRING))
		{
			await connection.OpenAsync();
			//SELECT TOP 1 * FROM[TABLENAME] ORDER BY id DESC
			var sql = "select id from leaderboard_weekly WHERE user_id = @a;";
			DynamicParameters dp = new DynamicParameters();
			dp.Add("a", userId);
			int id = await connection.ExecuteScalarAsync<int>(sql, dp);
			return id;
		}
	}

	public async Task<bool> CheckIfUserExists(int userId)
	{
		using (var connection = new NpgsqlConnection(ConnectionString.CONNECTION_STRING))
		{
			await connection.OpenAsync();
			//SELECT TOP 1 * FROM[TABLENAME] ORDER BY id DESC
			var sql = "SELECT EXISTS(SELECT stars_won FROM leaderboard_weekly WHERE user_id = @a);";

			DynamicParameters dp = new DynamicParameters();
			dp.Add("a", userId);
			bool isUserExists = await connection.ExecuteScalarAsync<bool>(sql, dp);
			return isUserExists;
		}
	}


	public async Task<float> GetPlayerStarsWon(int userId)
	{
		using (var connection = new NpgsqlConnection(ConnectionString.CONNECTION_STRING))
		{
			await connection.OpenAsync();
			//SELECT TOP 1 * FROM[TABLENAME] ORDER BY id DESC
			var sql = "SELECT stars_won FROM leaderboard_weekly WHERE user_id = @a;";

			DynamicParameters dp = new DynamicParameters();
			dp.Add("a", userId);
			float amount = await connection.ExecuteScalarAsync<float>(sql, dp);
			return amount;
		}
	}

	#endregion

	//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	#region update


	public async Task UpdatePlayerStarsWon(int userId, int starsWon)
	{
		using (var connection = new NpgsqlConnection(ConnectionString.CONNECTION_STRING))
		{
			await connection.OpenAsync();

			//  query
			var sql = $"UPDATE leaderboard_weekly SET games_played = games_played + 1, stars_won = stars_won + @a WHERE user_id = @b;";
			DynamicParameters dp = new DynamicParameters();
			dp.Add("a", starsWon);
			dp.Add("b", userId);
			var result = await connection.ExecuteAsync(sql, dp);
			Console.WriteLine($"result : {result}");
		}
	}

	public async Task UpdatePlayerStarsWonForBot(int userId, int gamesPlayed, int starsWon, DateTime updatedTime)
	{
		using (var connection = new NpgsqlConnection(ConnectionString.CONNECTION_STRING))
		{
			await connection.OpenAsync();

			//  query
			var sql = $"UPDATE leaderboard_weekly SET games_played = games_played + @a, stars_won = stars_won + @b, last_updated = @c WHERE user_id = @d;";
			DynamicParameters dp = new DynamicParameters();
			dp.Add("a", gamesPlayed);
			dp.Add("b", starsWon);
			dp.Add("c", updatedTime);
			dp.Add("d", userId);
			var result = await connection.ExecuteAsync(sql, dp);
			Console.WriteLine($"result : {result}");
		}
	}

	public async void IncrementPlayerGamesPlayed(int userId)
	{
		using (var connection = new NpgsqlConnection(ConnectionString.CONNECTION_STRING))
		{
			await connection.OpenAsync();

			//  query
			var sql = $"UPDATE leaderboard_weekly SET games_played = games_played + 1 WHERE user_id = @a;";
			DynamicParameters dp = new DynamicParameters();
			dp.Add("a", userId);
			var result = await connection.ExecuteAsync(sql, dp);
			Console.WriteLine($"result : {result}");
		}
	}



	#endregion

	//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	#region amount win

	public async Task MoveDataToPreviousTable()
	{
		using (var connection = new NpgsqlConnection(ConnectionString.CONNECTION_STRING))
		{
			await connection.OpenAsync();

			//  query
			var sql = $"DELETE FROM leaderboard_previous_weekly;";
			var result = await connection.ExecuteAsync(sql);
			Console.WriteLine($"DELETE FROM leaderboard_previous_weekly :: result : {result}");

			sql = $"INSERT INTO leaderboard_previous_weekly SELECT * FROM leaderboard_weekly;";
			result = await connection.ExecuteAsync(sql);
			Console.WriteLine($"INSERT INTO leaderboard_previous_weekly SELECT * FROM leaderboard_weekly :: result : {result}");

			sql = $"update leaderboard_weekly set games_played = 0, stars_won = 0;";
			result = await connection.ExecuteAsync(sql);
			Console.WriteLine($"UPDATE leaderboard_weekly :: result : {result}");
		}
	}

	//public async Task<int[]> GetWinnersIdToCreditAmount()
	//{

	//	using (var connection = new NpgsqlConnection(ConnectionString.CONNECTION_STRING))
	//	{
	//		await connection.OpenAsync();
	//		//SELECT TOP 1 * FROM[TABLENAME] ORDER BY id DESC
	//		var sql = "SELECT user_id FROM leaderboard_previous_weekly ORDER BY amount_won DESC, games_won DESC, games_played DESC  LIMIT 10;";
	//		var users = await connection.QueryAsync<int>(sql);
	//		int[] userIds = users.ToArray();
	//		return userIds;
	//	}
	//}


	#endregion

	//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	#region delete

	public async void DeleteTable()
	{
		using (var connection = new NpgsqlConnection(ConnectionString.CONNECTION_STRING))
		{
			await connection.OpenAsync();

			//  query
			var sql = $"DELETE FROM leaderboard_weekly;";
			var result = await connection.ExecuteAsync(sql);
			Console.WriteLine($"result : {result}");
		}
	}

	public async Task DeleteUser(int userId)
	{
		using (var connection = new NpgsqlConnection(ConnectionString.CONNECTION_STRING))
		{
			await connection.OpenAsync();

			// insert query

			var sql = $"DELETE FROM leaderboard_previous_weekly WHERE user_id = @a;DELETE FROM leaderboard_weekly WHERE user_id = @a;";

			DynamicParameters dp = new DynamicParameters();
			dp.Add("a", userId);

			var result = await connection.QueryAsync(sql, dp);

			Console.WriteLine($"result : {result}");
		}
	}

	#endregion

	//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------
}