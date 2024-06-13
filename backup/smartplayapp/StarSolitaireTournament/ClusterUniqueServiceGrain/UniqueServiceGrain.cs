using System.Threading.Tasks;
using Orleans;
using Orleans.Runtime;
using Microsoft.Extensions.Logging;
using Orleans.Concurrency;
using System;
using StarSolitaireTournament.Grains;
using Models;

namespace MasterServer.ClusterUniqueService
{
	public class UniqueServiceGrain : Grain, IUniqueServiceGrain, IRemindable
	{
		//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

		#region references and initialization

		IPersistentState<UniqueServiceGrainState> state;
		ILogger<UniqueServiceGrain> logger;
		IUserQueries userQueries;
		ILeaderboardWeeklyQueries leaderboardWeeklyQueries;


		public UniqueServiceGrain(ILogger<UniqueServiceGrain> _logger, IUserQueries _userQueries,
			ILeaderboardWeeklyQueries _leaderboardWeeklyQueries, [PersistentState("UniqueServiceGrainState", "localStorage")] IPersistentState<UniqueServiceGrainState> _state)
		{
			logger = _logger;
			userQueries = _userQueries;
			leaderboardWeeklyQueries = _leaderboardWeeklyQueries;
			state = _state;
		}

		#endregion

		//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

		#region logic

		public override async Task OnActivateAsync()
		{
			try
			{
				UniqueServiceGrainState reference = state.State;

				await SaveState();
				await base.OnActivateAsync();
			}
			catch (Exception exception)
			{
				logger.LogError($"UniqueServiceGrain :: OnActivateAsync :: exception :: {exception}");
			}
		}

		public Task StartReminderToUpdateLeaderboard()
		{
			try
			{
				UniqueServiceGrainState reference = state.State;
				logger.LogInformation($"Reminder started for StartReminderToUpdateLeaderboard");

				//reference.fetchMatchestimerStartedTime = DateTime.Now;
				RegisterOrUpdateReminder(Constants.GrainsReminderNames.UNIQUESERVICE_UPDATE_LEADERBOARD_REMINDER,
						   TimeSpan.FromMilliseconds(0),
						   TimeSpan.FromMinutes(1));
			}
			catch (Exception exception)
			{
				logger.LogError($"UniqueServiceGrain :: StartReminderToUpdateLeaderboard :: exception :: {exception}");
			}
			return Task.CompletedTask;
		}

		//public Task StartReminderToGetUserIdToLeaderboardId()
		//{
		//	try
		//	{
		//		UniqueServiceGrainState reference = state.State;
		//		logger.LogInformation($"Reminder started for StartReminderToGetUserIdToLeaderboardId");

		//		//reference.fetchMatchestimerStartedTime = DateTime.Now;
		//		RegisterOrUpdateReminder(Constants.GrainsReminderNames.UNIQUESERVICE_UPDATE_USERID_TO_TOURNAMENT_PLACEID_REMINDER,
		//				   TimeSpan.FromMilliseconds(0),
		//				   TimeSpan.FromMinutes(5));
		//	}
		//	catch (Exception exception)
		//	{
		//		logger.LogError($"UniqueServiceGrain :: StartReminderToGetUserIdToLeaderboardId :: exception :: {exception}");
		//	}
		//	return Task.CompletedTask;
		//}


		public async Task StartReminderForLeaderboardWeeklyCreditWinnersBalances()
		{
			try
			{
				IGrainReminder reminder = await GetReminder(Constants.GrainsReminderNames.UNIQUESERVICE_LEADERBOARD_WEEKLY_CREDIT_WINNERS_BALANCES_REMINDER);
				if (reminder != null)
				{
					await UnregisterReminder(reminder);
				}

				UniqueServiceGrainState reference = state.State;
				logger.LogInformation($"Reminder started for StartReminderForLeaderboardWeeklyCreditWinnersBalances");

				DateTime now = DateTime.Now;
				DateTime target = GetNextDateTime(DateTime.Now, DayOfWeek.Sunday, new TimeSpan(00, 00, 0));
				TimeSpan untilNextSunday = target.Subtract(now);
				double remainingSeconds = untilNextSunday.TotalSeconds;
				//reference.fetchMatchestimerStartedTime = DateTime.Now;
				await RegisterOrUpdateReminder(Constants.GrainsReminderNames.UNIQUESERVICE_LEADERBOARD_WEEKLY_CREDIT_WINNERS_BALANCES_REMINDER,
							   TimeSpan.FromSeconds(remainingSeconds),
							   TimeSpan.FromSeconds(remainingSeconds));
			}
			catch (Exception exception)
			{
				logger.LogError($"UniqueServiceGrain :: StartReminderForLeaderboardWeeklyCreditWinnersBalances :: exception :: {exception}");
			}
		}

		public Task StartReminderToUpdateBotsStar()
		{
			try
			{
				UniqueServiceGrainState reference = state.State;
				logger.LogInformation($"Reminder started for StartReminderToUpdateBotsStar");

				//reference.fetchMatchestimerStartedTime = DateTime.Now;
				RegisterOrUpdateReminder(Constants.GrainsReminderNames.UNIQUESERVICE_LEADERBOARD_WEEKLY_UPDATE_BOTS_STARS_REMINDER,
						   TimeSpan.FromMilliseconds(0),
						   TimeSpan.FromMinutes(60));
			}
			catch (Exception exception)
			{
				logger.LogError($"UniqueServiceGrain :: StartReminderToUpdateBotsStar :: exception :: {exception}");
			}
			return Task.CompletedTask;
		}

		public DateTime GetNextDateTime(DateTime now, DayOfWeek targetDay, TimeSpan targetTime)
		{
			DateTime target = now.Date.Add(targetTime);

			while (target < now || target.DayOfWeek != targetDay)
			{
				target = target.AddDays(1.0);
			}

			return target;
		}

		public async Task UpdateLeaderboardItems()
		{
			try
			{
				logger.LogInformation($"UniqueServiceGrain :: UpdateLeaderboardItems called");

				int totalUsersCount = await userQueries.GetTotalPlayers();
				int numberOfParts = totalUsersCount / 50;
				List<LeaderboardWithUserDetails> userDetails;
				for (int i = 0; i <= numberOfParts; i++)
				{
					userDetails = await leaderboardWeeklyQueries.GetAllPlayers(i * 50, (i * 50) + 50);
					await GrainFactory.GetGrain<ILeaderboardGrain>(i).UpdateItems(userDetails);
				}
				//int remainingPlayers = totalUsersCount - (numberOfParts * 50);
				//userDetails = await leaderboardWeeklyQueries.GetAllPlayers(numberOfParts * 50, (numberOfParts * 50) + 50);
				//await GrainFactory.GetGrain<ILeaderboardGrain>(numberOfParts).UpdateItems(userDetails);
			}
			catch (Exception exception)
			{
				logger.LogError($"UniqueServiceGrain :: UpdateLeaderboardItems :: exception :: {exception}");
			}
		}



		public async Task LeaderboardWeeklyCreditWinnersBalances()
		{
			try
			{
				logger.LogInformation($"UniqueServiceGrain :: LeaderboardWeeklyCreditWinnersBalances called");

				await leaderboardWeeklyQueries.MoveDataToPreviousTable();
				//int[] weeklyWinnersIds = await leaderboardWeeklyQueries.GetWinnersIdToCreditAmount();
				//int[] winAmounts = await GrainFactory.GetGrain<ISettingsGrain>(Guid.Empty).GetWeeklyWinAmount();

				// ten winners in weekly leaderboard
				//for (int i = 0; i < 10; i++)
				//{
				//	int userId = weeklyWinnersIds[i];
				//	float winAmount = winAmounts[i];
				//	await leaderboardUserWinAmount.InsertUserWinAmount(userId, Constants.LeaderboardType.WEEKLY, i + 1, winAmount);
				//	await balanceQueries.UpdatePlayerDepositBalance(userId, winAmount);
				//	await GrainFactory.GetGrain<IUserGrain>(userId).AddUserDepositBalance(winAmount);
				//}

				int totalUsersCount = await userQueries.GetTotalPlayers();
				int numberOfParts = totalUsersCount / 50;
				List<LeaderboardWithUserDetails> userDetails;
				for (int i = 0; i <= numberOfParts; i++)
				{
					userDetails = await leaderboardWeeklyQueries.GetAllPlayers(i * 50, (i * 50) + 50);
					await GrainFactory.GetGrain<ILeaderboardGrain>(i).UpdateItems(userDetails);
				}
				//int remainingPlayers = totalUsersCount - (numberOfParts * 50);
				//userDetails = await leaderboardWeeklyQueries.GetAllPlayers(numberOfParts * 50, (numberOfParts * 50) + 50);
				//await GrainFactory.GetGrain<ILeaderboardGrain>(numberOfParts).UpdateItems(userDetails);
			}
			catch (Exception exception)
			{
				logger.LogError($"UniqueServiceGrain :: LeaderboardWeeklyCreditWinnersBalances :: exception :: {exception}");
			}
		}

		public async Task UpdateBotsStars(int customRange = -1)
		{
			try
			{
				logger.LogInformation($"UniqueServiceGrain :: UpdateBotsStars called");
				int totalUsersCount = await userQueries.GetTotalPlayers();
				List<int> range = new List<int>();
				int numberOfParts = totalUsersCount / 50;
				if (customRange == -1)
				{
					for (int i = 0; i <= numberOfParts; i++)
					{
						range.Add(i);
					}
				}
				else
				{
					range.Add(customRange);
				}
				List<LeaderboardForBotUpdate> userDetails;
				Random rand = new Random();
				foreach (var i in range)
				{
					userDetails = await leaderboardWeeklyQueries.GetPlayersForBotUpdate(i * 50, (i * 50) + 50);
					List<LeaderboardForBotUpdate> nonBotsPlayer = userDetails.FindAll(u => u.is_bot == false);
					if (nonBotsPlayer.Count > 0)
					{
						nonBotsPlayer = nonBotsPlayer.OrderByDescending(u => u.stars_won).ToList();
						int starsWonOfTopUsers = 0;
						int counter = 0;
						int firstNonBotPlayerScore = nonBotsPlayer[0].stars_won;
						foreach (var item in nonBotsPlayer)
						{
							if (counter <= 5)
							{
								starsWonOfTopUsers += item.stars_won;
							}
							++counter;
						}
						//int averageStarsWonOfTopUsers = starsWonOfTopUsers / counter;
						int averageStarsWonOfTopUsers = firstNonBotPlayerScore;

						List<LeaderboardForBotUpdate> botsPlayer = userDetails.FindAll(u => u.is_bot == true);
						botsPlayer = botsPlayer.OrderByDescending(u => u.stars_won).ToList();
						counter = 0;
						int starsWonOfTopBotUsers = botsPlayer[0].stars_won;
						foreach (var item in botsPlayer)
						{
							if (counter <= 5)
							{
								starsWonOfTopBotUsers += item.stars_won;
							}
							++counter;
						}

						int averageStarsWonOfTopBotUsers = starsWonOfTopBotUsers / counter;
						if (averageStarsWonOfTopBotUsers > averageStarsWonOfTopUsers)
						{
							// do nothing already bot users scores are high
						}
						else
						{
							foreach (var item in botsPlayer)
							{
								int gamesPlayed = rand.Next(1, 4);
								int starsWon = (gamesPlayed * 2) + rand.Next(0, 1);
								await leaderboardWeeklyQueries.UpdatePlayerStarsWonForBot(item.user_id, gamesPlayed, starsWon, DateTime.UtcNow.AddMinutes(-rand.Next(0, 60)));
								logger.LogInformation($"UniqueServiceGrain :: UpdateBotsStars :: user_id : {item.user_id}, gamesPlayed : {gamesPlayed}, starsWon : {starsWon}");
							}
						}
						logger.LogInformation($"UniqueServiceGrain :: UpdateBotsStars :: averageStarsWonOfTopUsers : {averageStarsWonOfTopUsers}, averageStarsWonOfTopBotUsers : {averageStarsWonOfTopBotUsers}," +
							$" starsWonOfTopUsers : {starsWonOfTopUsers}, firstNonBotPlayerScore : {firstNonBotPlayerScore}");

						if (firstNonBotPlayerScore > starsWonOfTopBotUsers)
						{
							int gamesPlayed = rand.Next(1, 4);
							int starsWon = (gamesPlayed * 2) + rand.Next(0, 1);
							await leaderboardWeeklyQueries.UpdatePlayerStarsWonForBot(botsPlayer[0].user_id, gamesPlayed, starsWon, DateTime.UtcNow.AddMinutes(-rand.Next(0, 60)));
							logger.LogInformation($"UniqueServiceGrain :: UpdateBotsStars :: user_id : {botsPlayer[0].user_id}, gamesPlayed : {gamesPlayed}, starsWon : {starsWon}");
						}
					}
				}
			}
			catch (Exception exception)
			{
				logger.LogError($"UniqueServiceGrain :: UpdateBotsStars :: exception :: {exception}");
			}
		}

		public async Task ReceiveReminder(string reminderName, TickStatus status)
		{
			try
			{
				logger.LogInformation($"UniqueServiceGrain :: ReceiveReminder :: reminderName :: {reminderName}, status : {status}");

				switch (reminderName)
				{
					case Constants.GrainsReminderNames.UNIQUESERVICE_UPDATE_LEADERBOARD_REMINDER:
						await UpdateLeaderboardItems();
						break;

					//case Constants.GrainsReminderNames.UNIQUESERVICE_UPDATE_USERID_TO_TOURNAMENT_PLACEID_REMINDER:
					//	break;

					case Constants.GrainsReminderNames.UNIQUESERVICE_LEADERBOARD_WEEKLY_CREDIT_WINNERS_BALANCES_REMINDER:
						await LeaderboardWeeklyCreditWinnersBalances();
						await StartReminderForLeaderboardWeeklyCreditWinnersBalances();
						break;

					case Constants.GrainsReminderNames.UNIQUESERVICE_LEADERBOARD_WEEKLY_UPDATE_BOTS_STARS_REMINDER:
						await UpdateBotsStars();
						break;

					default:
						break;
				}
			}
			catch (Exception exception)
			{
				logger.LogError($"UniqueServiceGrain :: ReceiveReminder :: exception :: {exception}");
			}
		}

		#endregion


		//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

		#region common functionality
		public async Task StartReminders()
		{
			try
			{
				await StartReminderToUpdateLeaderboard();
				//await StartReminderToGetUserIdToLeaderboardId();
				await StartReminderForLeaderboardWeeklyCreditWinnersBalances();
				await StartReminderToUpdateBotsStar();

				await UpdateLeaderboardItems();
				await UpdateBotsStars();

				//if (!(bool.Parse(Constants.EnvironmentVariablesFromHost.isToTestGameServer)))
				//{
				//    await StartReminderForLeaderboardWeeklyCreditWinnersBalances();
				//    await StartReminderForLeaderboardMonthlyCreditWinnersBalances();
				//    await StartReminderForLeaderboardYearlyCreditWinnersBalances();
				//}

				logger.LogInformation("UniqueServiceGrain :: StartReminder() called");
			}
			catch (Exception exception)
			{
				logger.LogError($"UniqueServiceGrain :: StartReminders :: exception :: {exception}");
			}
		}
		public async Task ReadState()
		{
			try
			{
				await state.ReadStateAsync();
			}
			catch (Exception ex)
			{
				logger.LogError($"UserGrain :: ReadState :: exception :: {ex}");
			}
		}

		public async Task<Task> SaveState()
		{
			try
			{
				await state.WriteStateAsync();
			}
			catch (Exception exception)
			{
				logger.LogError($"UniqueServiceGrain :: SaveState :: exception :: {exception}");
			}
			return Task.CompletedTask;
		}

		public async Task<Task> ClearState()
		{
			try
			{
				await state.ClearStateAsync();
				DeactivateOnIdle();
			}
			catch (Exception exception)
			{
				logger.LogError($"UniqueServiceGrain :: ClearState :: exception :: {exception}");
			}
			return Task.CompletedTask;
		}

		#endregion

		//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	}
}
