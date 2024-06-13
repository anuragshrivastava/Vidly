using StarSolitaireTournament.Authentication;
using StarSolitaireTournament.Managers;
using Microsoft.AspNetCore.Mvc;
using Models;
using Orleans;
using System.Security.Claims;
using StarSolitaireTournament.Grains;
using MasterServer.ClusterUniqueService;

namespace StarSolitaireTournament.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class PlayStoreController : ControllerBase
	{
		//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

		#region references and intialization

		private readonly ILogger<PlayStoreController> logger;
		IUserQueries userQueries;
		ILeaderboardWeeklyQueries leaderboardWeeklyQueries;
		ISignUpManager signUpManager;
		IJwtTokenManager jwtTokenManager;

		IClusterClient grainsClient;
		public PlayStoreController(ILogger<PlayStoreController> _logger, IUserQueries _userQueries, ILeaderboardWeeklyQueries _leaderboardWeeklyQueries, ISignUpManager _signUpManager, IJwtTokenManager _jwtTokenManager, IClusterClient _grainsClient)
		{
			logger = _logger;
			userQueries = _userQueries;
			leaderboardWeeklyQueries = _leaderboardWeeklyQueries;
			signUpManager = _signUpManager;
			jwtTokenManager = _jwtTokenManager;
			grainsClient = _grainsClient;
		}

		#endregion

		//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

		#region apis


		[HttpGet]
		public async Task<IActionResult> Get()
		{
			logger.Log(LogLevel.Information, "Status Called");
			return Ok();
		}

		[HttpPost, Route("userIdExists")]
		public async Task<SignUpResult> UserIdExists([FromForm] string userId)
		{
			try
			{
				logger.LogInformation($"PlayStoreController :: UserIdExists :: userId : {userId}");

				bool isLengthOk = false;

				if (!string.IsNullOrEmpty(userId))
				{
					int userIdInDB = await userQueries.GetUserIdByGoogleUserId(userId);
					logger.LogInformation($"PlayStoreController :: UserIdExists :: userId : {userId}, userIdInDB : {userIdInDB}");

					string token = string.Empty;
					if (userIdInDB != 0)
					{
						token = jwtTokenManager.GenerateJwtToken(userIdInDB.ToString(), Constants.Roles.LOBBY);
						return new SignUpResult(true, true, null, token);
					}
					else
					{
						return new SignUpResult(true, false, null, token);
					}
				}
				else
				{
					return new SignUpResult(false, false, "Invalid Credentials!");
				}
			}
			catch (Exception exception)
			{
				logger.LogError($"PlayStoreController :: UserIdExists :: userId : {userId}, exception : {exception}");
				return new SignUpResult(false, false, exception.Message);
			}
		}

		[HttpPost, Route("GetPlayerDetails")]
		public async Task<UserDetails> GetPlayerDetails([FromForm] string userId)
		{
			try
			{
				logger.LogInformation($"PlayStoreController :: GetPlayerDetails :: userId : {userId}");

				bool isLengthOk = false;

				if (!string.IsNullOrEmpty(userId))
				{
					int userIdInDB = await userQueries.GetUserIdByGoogleUserId(userId);
					logger.LogInformation($"PlayStoreController :: GetPlayerDetails :: userId : {userId}, userIdInDB : {userIdInDB}");

					if (userIdInDB != 0)
					{
						UserDetails userDetails = await userQueries.GetUserDetails(userIdInDB);
						userDetails.token = jwtTokenManager.GenerateJwtToken(userIdInDB.ToString(), Constants.Roles.LOBBY);
						return userDetails;
					}
					else
					{
						return new UserDetails(false, "Not Exists!");
					}
				}
				else
				{
					return new UserDetails(false, "Invalid Credentials!");
				}
			}
			catch (Exception exception)
			{
				logger.LogError($"PlayStoreController :: UserIdExists :: userId : {userId}, exception : {exception}");
				return new UserDetails(false, exception.Message);
			}
		}




		[HttpPost, Route("SetPlayerDetails")]
		public async Task<SignUpResult> SetPlayerDetails([FromForm] string userId, [FromForm] string signIntoken, [FromForm] string playerName, [FromForm] string profileURL, [FromForm] string email)
		{
			try
			{
				logger.LogInformation($"PlayStoreController :: SetPlayerNameAndProfile :: userId : {userId}, playerName : {playerName}, profileURL : {profileURL}");

				if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(playerName) || string.IsNullOrEmpty(profileURL))
				{
					return new SignUpResult(false, false, Constants.MessageToClient.PLEASE_TRY_TO_REGISTER_AGAIN);
				}
				else
				{
					if (await userQueries.IsGoogleUserIdExists(userId))
					{
						return new SignUpResult(false, false, Constants.MessageToClient.USER_EXISTS_TRY_TO_LOGIN);
					}
					else
					{
						return await signUpManager.SetPlayerNameAndProfile(userId, playerName, profileURL, email);
					}
				}

			}
			catch (Exception exception)
			{
				logger.LogError($"PlayStoreController :: SetPlayerNameAndProfile :: exception : {exception}");
				return new SignUpResult(false, false, Constants.MessageToClient.WRONG_OTP_ENTERED);
			}
		}

		[HttpPost, Route("UpdatePlayerNameAndProfile")]
		public async Task<UpdateResult> UpdatePlayerNameAndProfile([FromForm] string token, [FromForm] string playerName, [FromForm] string profileURL)
		{
			try
			{
				logger.LogInformation($"PlayStoreController :: AddTournamentStarsAndGetPlayers :: token : {token}");

				ClaimsPrincipal principal = jwtTokenManager.GetPrincipals(token);
				//
				if (string.IsNullOrEmpty(token) || principal == null)
				{

					return new UpdateResult(false, "Invalid Token 1");
				}
				else
				{
					if (int.TryParse(principal.Claims.First().Value, out int userId))
					{
						var temp = await grainsClient.GetGrain<ILeaderboardGrain>(userId / 50).GetLeaderboardItems();
						return new UpdateResult(true, (await userQueries.UpdateUsernameAndProfileURL(userId, playerName, profileURL)).ToString());
					}
					else
					{
						return new UpdateResult(false, "Invalid Token 2");
					}
				}
			}
			catch (Exception exception)
			{
				logger.LogError($"PlayStoreController :: SetPlayerNameAndProfile :: exception : {exception}");
				return new UpdateResult(false, exception.ToString());
			}
		}

		[HttpPost, Route("AddTournamentStarsAndGetPlayers")]
		public async Task<TournamentPlayers> AddTournamentStarsAndGetPlayers([FromForm] string token, [FromForm] int starsToAdd)
		{
			try
			{
				logger.LogInformation($"PlayStoreController :: AddTournamentStarsAndGetPlayers :: token : {token}, starsToAdd : {starsToAdd}");

				ClaimsPrincipal principal = jwtTokenManager.GetPrincipals(token);
				//
				if (string.IsNullOrEmpty(token) || principal == null)
				{

					return new TournamentPlayers(false, "Invalid Token 1", null);
				}
				else
				{
					if (int.TryParse(principal.Claims.First().Value, out int userId))
					{
						if (starsToAdd >= 1 && starsToAdd <= 3)
						{
							await leaderboardWeeklyQueries.UpdatePlayerStarsWon(userId, starsToAdd);
							await grainsClient.GetGrain<IUniqueServiceGrain>(Guid.Empty).UpdateBotsStars(userId / 50);
							return new TournamentPlayers(true, string.Empty, await grainsClient.GetGrain<ILeaderboardGrain>(userId / 50).GetLeaderboardItems());
						}
						else
						{
							return new TournamentPlayers(false, "Invalid Stars Count", null);
						}
					}
					else
					{
						return new TournamentPlayers(false, "Invalid Token 2", null);
					}
				}
			}
			catch (Exception exception)
			{
				logger.LogError($"PlayStoreController :: AddTournamentStarsAndGetPlayers :: token : {token}, exception : {exception}");
				return new TournamentPlayers(false, exception.ToString(), null);
			}
		}

		[HttpPost, Route("GetTournamentPlayers")]
		public async Task<TournamentPlayers> GetTournamentPlayers([FromForm] string token)
		{
			try
			{
				logger.LogInformation($"PlayStoreController :: AddTournamentStarsAndGetPlayers :: token : {token}");

				ClaimsPrincipal principal = jwtTokenManager.GetPrincipals(token);
				//
				if (string.IsNullOrEmpty(token) || principal == null)
				{

					return new TournamentPlayers(false, "Invalid Token 1", null);
				}
				else
				{
					if (int.TryParse(principal.Claims.First().Value, out int userId))
					{
						return new TournamentPlayers(true, string.Empty, await grainsClient.GetGrain<ILeaderboardGrain>(userId / 50).GetLeaderboardItems());
					}
					else
					{
						return new TournamentPlayers(false, "Invalid Token 2", null);
					}
				}
			}
			catch (Exception exception)
			{
				logger.LogError($"PlayStoreController :: AddTournamentStarsAndGetPlayers :: token : {token}, exception : {exception}");
				return new TournamentPlayers(false, exception.ToString(), null);
			}
		}





		[HttpPost, Route("GetLeaderboardPlayers")]
		public async Task<TournamentPlayers> GetLeaderboardPlayers([FromForm] int userId)
		{
			try
			{
				logger.LogInformation($"PlayStoreController :: GetLeaderboardPlayers :: userId : {userId}");

				return new TournamentPlayers(true, string.Empty, await grainsClient.GetGrain<ILeaderboardGrain>(userId / 50).GetLeaderboardItems());
			}
			catch (Exception exception)
			{
				logger.LogError($"PlayStoreController :: GetLeaderboardPlayers :: userId : {userId}, exception : {exception}");
				return new TournamentPlayers(false, exception.ToString(), null);
			}
		}




		//[HttpPost, Route("InAppProductPurchased")]
		//public async Task InAppProductPurchased([FromForm] string token, [FromForm] string productId, [FromForm] string transactionId)
		//{
		//    try
		//    {
		//        ClaimsPrincipal principal = jwtTokenManager.GetPrincipals(token);

		//        if (string.IsNullOrEmpty(token) || principal == null)
		//        {

		//        }
		//        else
		//        {
		//            int userId = int.Parse(principal.Claims.First().Value);
		//            logger.LogInformation($"PlayStoreController :: InAppProductPurchased :: token : {userId}, productId : {productId}, transactionId : {transactionId}");

		//            int coinsCount = 0;
		//            switch (productId)
		//            {
		//                case "com.pfcgaming.pfcpro.10000coins":
		//                    coinsCount = 10000;
		//                    break;
		//                case "com.pfcgaming.pfcpro.20000coins":
		//                    coinsCount = 20000;
		//                    break;
		//                case "com.pfcgaming.pfcpro.55000coins":
		//                    coinsCount = 55000;
		//                    break;
		//                case "com.pfcgaming.pfcpro.115000coins":
		//                    coinsCount = 115000;
		//                    break;
		//                case "com.pfcgaming.pfcpro.260000coins":
		//                    coinsCount = 260000;
		//                    break;
		//                case "com.pfcgaming.pfcpro.800000coins":
		//                    coinsCount = 800000;
		//                    break;
		//                case "com.pfcgaming.pfcpro.1800000coins":
		//                    coinsCount = 1800000;
		//                    break;
		//                case "com.pfcgaming.pfcpro.4000000coins":
		//                    coinsCount = 4000000;
		//                    break;
		//                default:
		//                    break;
		//            }

		//            await transactionsQueries.InsertTransaction(userId, Constants.TransactionType.CREDIT, transactionId, coinsCount);
		//            await balanceQueries.UpdatePlayerDepositBalance(userId, coinsCount);
		//            await grainsClient.GetGrain<IUserGrain>(userId).AddUserDepositBalance(coinsCount);

		//            string connectionId = await grainsClient.GetGrain<IPlayerIdToConnectionIdGrain>(userId).GetConnectionId();
		//            await gameClientsManager.GetCurrentUserBalancesAndGamesPlayed(connectionId, userId);
		//        }
		//    }
		//    catch (Exception exception)
		//    {
		//        logger.LogError($"PlayStoreController :: InAppProductPurchased :: exception : {exception}");
		//    }
		//}


		#endregion

		//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

		#region helper functions


		#endregion

		//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	}
}
