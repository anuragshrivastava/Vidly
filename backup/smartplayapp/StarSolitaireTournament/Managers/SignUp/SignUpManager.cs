using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Models;
using Orleans;
using StarSolitaireTournament.Authentication;
using StarSolitaireTournament.Grains;

namespace StarSolitaireTournament.Managers
{
	public class SignUpManager : ISignUpManager
	{
		//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

		#region references

		// dependencies
		IJwtTokenManager jwtTokenManager;
		IUserQueries userQueries;
		ILeaderboardWeeklyQueries leaderboardWeeklyQueries;
		IClusterClient grainsClient;

		#endregion

		//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

		#region intialization

		public SignUpManager(IUserQueries _userQueries, ILeaderboardWeeklyQueries _leaderboardWeeklyQueries, IJwtTokenManager _jwtTokenManager, IClusterClient _grainsClient)
		{
			userQueries = _userQueries;
			leaderboardWeeklyQueries = _leaderboardWeeklyQueries;
			jwtTokenManager = _jwtTokenManager;
			grainsClient = _grainsClient;
		}

		#endregion

		//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

		#region client called functions

		public async Task<SignUpResult> SetPlayerNameAndProfile(string googleSignInUserId, string playerName, string profileURL, string email)
		{
			int userId = await userQueries.InsertUser(string.Empty, googleSignInUserId, playerName, profileURL, false, email);
			string token = jwtTokenManager.GenerateJwtToken(userId.ToString(), Constants.Roles.LOBBY);
			await grainsClient.GetGrain<ILeaderboardGrain>(userId / 50).AddItem(new LeaderboardWithUserDetails(playerName, profileURL, 0, 0, DateTime.Now));
			if (userId % 50 == 0)
			{
				await InsertBotsRandomly();
			}
			return new SignUpResult(true, false, null, token);
		}

		#endregion

		//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

		#region helper functions

		public async Task InsertBotsRandomly()
		{
			Random rand = new Random();
			for (int i = 0; i < 10; i++)
			{
				await userQueries.InsertUser(string.Empty, string.Empty, Constants.RandomName.GetRandomName(), rand.Next(0, 11).ToString(), true, string.Empty);
			}
		}

		#endregion

		//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	}
}
