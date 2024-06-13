using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models.DatabaseModels;
using Newtonsoft.Json;
using Models;

public class UserQueries : IUserQueries
{

	//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	#region insert

	/// <summary>
	/// insert into users table first and then into balances table
	/// </summary>
	/// <param name="firebaseToken"></param>
	/// <param name="username"></param>
	/// <param name="profileUrl"></param>
	/// <returns></returns>
	public async Task<int> InsertUser(string firebaseToken, string googleSignInUserId, string username, string profileUrl, bool isBot, string email)
	{
		using (var connection = new NpgsqlConnection(ConnectionString.CONNECTION_STRING))
		{
			await connection.OpenAsync();

			// insert query

			var sql = $"INSERT INTO users(firebase_token, google_userid, username, profile_url, email_id, registration_datetime) VALUES (@a, @b, @c, @d, @e, @f) RETURNING id;";

			DynamicParameters dp = new DynamicParameters();
			dp.Add("a", firebaseToken);
			dp.Add("b", googleSignInUserId);
			dp.Add("c", username);
			dp.Add("d", profileUrl);
			dp.Add("e", email);
			dp.Add("f", DateTime.UtcNow);

			int id = await connection.ExecuteScalarAsync<int>(sql, dp);

			sql = $"INSERT INTO leaderboard_weekly(user_id, is_bot, games_played, stars_won, last_updated) VALUES (@a, @b, @c, @d, @e);";

			dp = new DynamicParameters();
			dp.Add("a", id);
			dp.Add("b", isBot);
			dp.Add("c", 0);
			dp.Add("d", 0);
			dp.Add("e", DateTime.UtcNow);

			int result = await connection.ExecuteAsync(sql, dp);

			return id;
		}
	}

	#endregion

	//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	#region select

	public async Task GetUser(int userId)
	{

		using (var connection = new NpgsqlConnection(ConnectionString.CONNECTION_STRING))
		{
			await connection.OpenAsync();

			var sql = "SELECT firebase_token, google_userid, username, profile_url, email_id FROM users WHERE id = @a;";

			DynamicParameters dp = new DynamicParameters();
			dp.Add("a", userId);
			var users = await connection.QueryAsync<User>(sql, dp);

			Console.WriteLine("result : ");
			foreach (var item in users)
			{
				Console.WriteLine($"id : {userId}");
				Console.WriteLine($"firebase_token : {item.firebase_token}");
				Console.WriteLine($"google_userid : {item.google_userid}");
				Console.WriteLine($"username : {item.username}");
				Console.WriteLine($"profile_url : {item.profile_url}");
				Console.WriteLine($"email_id : {item.email_id}");
			}
		}
	}

	public async Task<UserDetails> GetUserDetails(int userId)
	{
		using (var connection = new NpgsqlConnection(ConnectionString.CONNECTION_STRING))
		{
			await connection.OpenAsync();

			var sql = "SELECT username, profile_url, games_played, stars_won, last_updated FROM users, leaderboard_weekly WHERE users.id = @a AND leaderboard_weekly.user_id = users.id";

			DynamicParameters dp = new DynamicParameters();
			dp.Add("a", userId);
			var users = await connection.QueryAsync<LeaderboardWithUserDetails>(sql, dp);

			Console.WriteLine("result : ");
			UserDetails userDetails = new UserDetails();
			userDetails.isSuccess = true;
			foreach (var item in users)
			{

				Console.WriteLine($"id : {userId}");
				Console.WriteLine($"username : {item.username}");
				Console.WriteLine($"profile_url : {item.profile_url}");
				Console.WriteLine($"games_played : {item.games_played}");
				Console.WriteLine($"stars_won : {item.stars_won}");
				Console.WriteLine($"last_updated : {item.last_updated}");

				userDetails.username = item.username;
				userDetails.profileUrl = item.profile_url;
				userDetails.gamesPlayed = item.games_played;
				userDetails.starsWon = item.stars_won;
				userDetails.lastUpdated = item.last_updated;
			}
			return userDetails;
		}
	}


	public async Task<bool> IsGoogleUserIdExists(string phoneNumber)
	{
		using (var connection = new NpgsqlConnection(ConnectionString.CONNECTION_STRING))
		{
			await connection.OpenAsync();

			var sql = "SELECT 1 FROM users WHERE google_userid = @a LIMIT 1;";

			DynamicParameters dp = new DynamicParameters();
			dp.Add("a", phoneNumber);
			var value = await connection.QuerySingleOrDefaultAsync<int>(sql, dp);

			Console.WriteLine($"UserQueries :: IsGoogleUserIdExists :: value : {value}");
			//foreach (var item in users)
			//{
			//    Console.WriteLine($"phoneNumber : {phoneNumber}");
			//}

			return value == 1;
		}
	}

	public async Task<string> GetUserName(int userId)
	{
		using (var connection = new NpgsqlConnection(ConnectionString.CONNECTION_STRING))
		{
			await connection.OpenAsync();

			var sql = "SELECT username FROM users WHERE id = @a LIMIT 1;";

			DynamicParameters dp = new DynamicParameters();
			dp.Add("a", userId);
			var username = await connection.QuerySingleOrDefaultAsync<string>(sql, dp);

			return username;
		}
	}

	public async Task<UsernameAndProfileURL> GetUserNameAndProfileUrl(int userId)
	{
		using (var connection = new NpgsqlConnection(ConnectionString.CONNECTION_STRING))
		{
			await connection.OpenAsync();

			var sql = "SELECT username, profile_url FROM users WHERE id = @a";

			DynamicParameters dp = new DynamicParameters();
			dp.Add("a", userId);
			var details = await connection.QuerySingleOrDefaultAsync<UsernameAndProfileURL>(sql, dp);

			return details;
		}
	}

	public async Task<string> GetEmailId(int userId)
	{
		using (var connection = new NpgsqlConnection(ConnectionString.CONNECTION_STRING))
		{
			await connection.OpenAsync();

			var sql = "SELECT email_id FROM users WHERE id = @a;";

			DynamicParameters dp = new DynamicParameters();
			dp.Add("a", userId);
			var emailId = await connection.QuerySingleOrDefaultAsync<string>(sql, dp);

			return emailId;
		}
	}

	public async Task<string> GetUserProfileURL(int userId)
	{
		using (var connection = new NpgsqlConnection(ConnectionString.CONNECTION_STRING))
		{
			await connection.OpenAsync();

			var sql = "SELECT profile_url FROM users WHERE id = @a LIMIT 1;";

			DynamicParameters dp = new DynamicParameters();
			dp.Add("a", userId);
			var username = await connection.QuerySingleOrDefaultAsync<string>(sql, dp);

			return username;
		}
	}


	public async Task<int> GetUserIdByGoogleUserId(string googleUserId)
	{
		using (var connection = new NpgsqlConnection(ConnectionString.CONNECTION_STRING))
		{
			await connection.OpenAsync();

			var sql = "select id from users where google_userid = @a LIMIT 1;";

			DynamicParameters dp = new DynamicParameters();
			dp.Add("a", googleUserId);
			var userId = await connection.QuerySingleOrDefaultAsync<int>(sql, dp);

			Console.WriteLine($"result : {userId}");
			return userId;
		}
	}


	public async Task<CachedUserDetails> GetUserDetailsForCache(int id)
	{
		using (var connection = new NpgsqlConnection(ConnectionString.CONNECTION_STRING))
		{
			await connection.OpenAsync();

			var sql = "select firebase_token, google_userid,  username, profile_url, email_id, leaderboard_weekly.id, games_played, stars_won, last_updated  from users, leaderboard_weekly where users.id = leaderboard_weekly.user_id  and users.id = @a LIMIT 1;";

			DynamicParameters dp = new DynamicParameters();
			dp.Add("a", id);
			var users = await connection.QuerySingleOrDefaultAsync<CachedUserDetails>(sql, dp);

			Console.WriteLine("result : ");
			//foreach (var item in users)
			//{
			//    Console.WriteLine($"isBlocked : {item.is_blocked}");
			//    Console.WriteLine($"firebaseToken : {item.firebase_token}");
			//    return item;
			//}

			//return null;
			return users;

		}
	}

	public async Task<int> GetTotalPlayers()
	{
		using (var connection = new NpgsqlConnection(ConnectionString.CONNECTION_STRING))
		{
			await connection.OpenAsync();
			//SELECT TOP 1 * FROM[TABLENAME] ORDER BY id DESC
			var sql = "select count(id) from users;";
			var usersCount = await connection.ExecuteScalarAsync<int>(sql);
			return usersCount;
		}
	}





	//public async Task<List<AdminPanelUsers>> GetUserDetailsForAdminPanel(int count, int offset, string sortBy)
	//{
	//	using (var connection = new NpgsqlConnection(ConnectionString.CONNECTION_STRING))
	//	{
	//		await connection.OpenAsync();

	//		var sql = "SELECT users.id, is_blocked, username, google_userid , profile_url, email_id,  registration_datetime, last_updated, deposit_balance, win_balance, total_deposited_balance, total_won_balance, total_withdrawal, total_games_played, total_games_won from users, balances where users.id = balances.id order by users.id LIMIT @a OFFSET @b;";

	//		switch (sortBy)
	//		{
	//			case "users.id":
	//				sql = "SELECT users.id, is_blocked, username, google_userid , profile_url, email_id,  registration_datetime, last_updated, deposit_balance, win_balance, total_deposited_balance, total_won_balance, total_withdrawal, total_games_played, total_games_won from users, balances where users.id = balances.id order by users.id LIMIT @a OFFSET @b;";
	//				break;

	//			case "total_games_won":
	//				sql = "SELECT users.id, is_blocked, username, google_userid , profile_url, email_id,  registration_datetime, last_updated, deposit_balance, win_balance, total_deposited_balance, total_won_balance, total_withdrawal, total_games_played, total_games_won from users, balances where users.id = balances.id order by total_games_won desc LIMIT @a OFFSET @b;";
	//				break;

	//			case "total_games_played":
	//				sql = "SELECT users.id, is_blocked, username, google_userid , profile_url, email_id,  registration_datetime, last_updated, deposit_balance, win_balance, total_deposited_balance, total_won_balance, total_withdrawal, total_games_played, total_games_won from users, balances where users.id = balances.id order by total_games_played desc LIMIT @a OFFSET @b;";
	//				break;

	//			case "total_deposited_balance":
	//				sql = "SELECT users.id, is_blocked, username, google_userid , profile_url, email_id,  registration_datetime, last_updated, deposit_balance, win_balance, total_deposited_balance, total_won_balance, total_withdrawal, total_games_played, total_games_won from users, balances where users.id = balances.id order by total_deposited_balance desc LIMIT @a OFFSET @b;";
	//				break;

	//			case "total_won_balance":
	//				sql = "SELECT users.id, is_blocked, username, google_userid , profile_url, email_id,  registration_datetime, last_updated, deposit_balance, win_balance, total_deposited_balance, total_won_balance, total_withdrawal, total_games_played, total_games_won from users, balances where users.id = balances.id order by total_won_balance desc LIMIT @a OFFSET @b;";
	//				break;

	//			case "total_withdrawal":
	//				sql = "SELECT users.id, is_blocked, username, google_userid , profile_url, email_id,  registration_datetime, last_updated, deposit_balance, win_balance, total_deposited_balance, total_won_balance, total_withdrawal, total_games_played, total_games_won from users, balances where users.id = balances.id order by total_withdrawal desc LIMIT @a OFFSET @b;";
	//				break;
	//		}

	//		DynamicParameters dp = new DynamicParameters();
	//		dp.Add("a", count); // count
	//		dp.Add("b", offset); // offset
	//		var users = await connection.QueryAsync<AdminPanelUsers>(sql, dp);
	//		List<AdminPanelUsers> usersList = users.ToList();

	//		//Console.WriteLine("result : " + users.ToString);

	//		//List<AdminPanelUsers> usersList = new List<AdminPanelUsers>();
	//		//foreach (var item in users)
	//		//{
	//		//    Console.WriteLine($"item : {item}");
	//		//    Console.WriteLine($"item.id : {item.id}");
	//		//    usersList.Add(new AdminPanelUsers
	//		//    {
	//		//        id = (int)item.id,
	//		//        is_blocked = item.is_blocked,
	//		//        username = item.username,
	//		//        google_userid = item.google_userid,
	//		//        profile_url = item.profile_url,
	//		//        email_id = item.email_id,
	//		//        registration_datetime = item.registration_datetime == null ? String.Empty : item.registration_datetime.ToString(),
	//		//        last_updated = item.last_updated == null ? String.Empty : item.last_updated.ToString(),
	//		//        deposit_balance = (float)item.deposit_balance,
	//		//        win_balance = (float)item.win_balance,
	//		//        total_deposited_balance = (float)item.total_deposited_balance,
	//		//        total_won_balance = (float)item.total_won_balance,
	//		//        total_withdrawal = (float)item.total_withdrawal,
	//		//        total_games_played = item.total_games_played,
	//		//        total_games_won = item.total_games_won
	//		//    });
	//		//}

	//		return usersList;
	//	}
	//}

	//public async Task<AdminPanelUsers> GetDetailsOfSingleUserForAdminPanel(int userId)
	//{
	//	using (var connection = new NpgsqlConnection(ConnectionString.CONNECTION_STRING))
	//	{
	//		await connection.OpenAsync();
	//		var sql = "SELECT users.id, is_blocked, username, google_userid , profile_url, email_id,  registration_datetime, last_updated, deposit_balance, win_balance, total_deposited_balance, total_won_balance, total_withdrawal, total_games_played, total_games_won from users, balances where users.id = balances.id and users.id = @a";
	//		DynamicParameters dp = new DynamicParameters();
	//		dp.Add("a", userId); // user id
	//		var user = await connection.QueryFirstAsync<AdminPanelUsers>(sql, dp);

	//		return user;
	//	}
	//}


	//public async Task<List<AdminPanelUsers>> GetAllBlockedUsersForAdminPanel(int count, int offset)
	//{
	//	using (var connection = new NpgsqlConnection(ConnectionString.CONNECTION_STRING))
	//	{
	//		await connection.OpenAsync();

	//		var sql = "SELECT users.id, is_blocked, username, google_userid , profile_url, email_id, registration_datetime, last_updated, deposit_balance, win_balance, total_deposited_balance, total_won_balance, total_withdrawal, total_games_played, total_games_won from users, balances where users.id = balances.id AND is_blocked = true order by users.id LIMIT @a OFFSET @b;";

	//		DynamicParameters dp = new DynamicParameters();
	//		dp.Add("a", count); // count
	//		dp.Add("b", offset); // offset
	//		var users = await connection.QueryAsync<AdminPanelUsers>(sql, dp);
	//		List<AdminPanelUsers> usersList = users.ToList();
	//		return usersList;
	//	}
	//}

	#endregion

	//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	#region update

	public async Task<int> UpdateUsernameAndProfileURL(int userId, string username, string newProfileURL)
	{

		using (var connection = new NpgsqlConnection(ConnectionString.CONNECTION_STRING))
		{
			await connection.OpenAsync();

			// insert query

			var sql = $"UPDATE users SET username = @a, profile_url = @b WHERE id = @c;";

			DynamicParameters dp = new DynamicParameters();
			dp.Add("a", username);
			dp.Add("b", newProfileURL);
			dp.Add("c", userId);

			int result = await connection.ExecuteAsync(sql, dp);

			Console.WriteLine($"result : {result}");
			return result;
		}
	}

	public async Task<int> UpdateUserProfileURL(int userId, string newProfileURL)
	{

		using (var connection = new NpgsqlConnection(ConnectionString.CONNECTION_STRING))
		{
			await connection.OpenAsync();

			// insert query

			var sql = $"UPDATE users SET profile_url = @a WHERE id = @b;";

			DynamicParameters dp = new DynamicParameters();
			dp.Add("a", newProfileURL);
			dp.Add("b", userId);

			int result = await connection.ExecuteAsync(sql, dp);

			Console.WriteLine($"result : {result}");
			return result;
		}
	}

	public async Task<int> UpdateUserFirebaseToken(int userId, string firebaseToken)
	{

		using (var connection = new NpgsqlConnection(ConnectionString.CONNECTION_STRING))
		{
			await connection.OpenAsync();

			// insert query

			var sql = $"UPDATE users SET firebase_token = @a WHERE id = @b;";

			DynamicParameters dp = new DynamicParameters();
			dp.Add("a", firebaseToken);
			dp.Add("b", userId);

			int result = await connection.ExecuteAsync(sql, dp);

			Console.WriteLine($"result : {result}");
			return result;
		}
	}

	public async Task<int> UpdateUserEmail(int userId, string email)
	{

		using (var connection = new NpgsqlConnection(ConnectionString.CONNECTION_STRING))
		{
			await connection.OpenAsync();

			// insert query

			var sql = $"UPDATE users SET email_id = @a WHERE id = @b;";

			DynamicParameters dp = new DynamicParameters();
			dp.Add("a", email);
			dp.Add("b", userId);

			int result = await connection.ExecuteAsync(sql, dp);

			Console.WriteLine($"result : {result}");
			return result;
		}
	}

	#endregion

	//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	#region delete

	public async Task DeleteUser(int userId)
	{

		using (var connection = new NpgsqlConnection(ConnectionString.CONNECTION_STRING))
		{
			await connection.OpenAsync();

			// insert query

			var sql = $"DELETE FROM users WHERE id = @a;";

			DynamicParameters dp = new DynamicParameters();
			dp.Add("a", userId);

			int result = await connection.ExecuteAsync(sql, dp);

			Console.WriteLine($"result : {result}");
		}
	}

	#endregion

	//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

}

// stored procedure
//var users = await connection.QueryAsync<CachedUserDetails>("get_user_for_cache", new { v_id = id }, commandType: System.Data.CommandType.StoredProcedure);
