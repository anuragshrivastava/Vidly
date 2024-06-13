using Models;
using Orleans.Runtime;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans.Concurrency;

namespace StarSolitaireTournament.Grains
{
	public class LeaderboardGrain : Grain, ILeaderboardGrain
	{
		private IPersistentState<LeaderboardGrainState> state;
		ILogger<LeaderboardGrain> logger;

		//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

		#region logic

		public LeaderboardGrain(ILogger<LeaderboardGrain> _logger, [PersistentState("LeaderboardGrainState", "localStorage")] IPersistentState<LeaderboardGrainState> _state)
		{
			logger = _logger;
			state = _state;
		}

		public override async Task OnActivateAsync()
		{
			try
			{
				if (state.State.leaderboardItems == null)
				{
					state.State.leaderboardItems = new List<LeaderboardWithUserDetails>();
					await SaveState();
				}
			}
			catch (Exception ex)
			{
				logger.LogError($"LeaderboardGrain :: OnActivateAsync :: ex :: {ex}");
			}
			await base.OnActivateAsync();
		}


		public async Task<Task> AddItem(LeaderboardWithUserDetails item)
		{
			try
			{
				await state.ReadStateAsync();
				state.State.leaderboardItems.Add(item);
				await SaveState();
			}
			catch (Exception ex)
			{
				logger.LogError($"LeaderboardGrain :: AddItem :: ex :: {ex}");
			}
			return Task.CompletedTask;
		}

		public async Task<Task> UpdateItems(List<LeaderboardWithUserDetails> items)
		{
			try
			{
				await state.ReadStateAsync();
				state.State.leaderboardItems = items;
				state.State.isDataSet = true;
				await SaveState();
			}
			catch (Exception ex)
			{
				logger.LogError($"LeaderboardGrain :: AddItem :: ex :: {ex}");
			}
			return Task.CompletedTask;
		}

		public async Task<List<LeaderboardWithUserDetails>> GetLeaderboardItems()
		{
			try
			{
				return state.State.leaderboardItems;
			}
			catch (Exception ex)
			{
				logger.LogError($"LeaderboardGrain :: GetLeaderboardItems :: ex :: {ex}");
			}
			return null;
		}



		[ReadOnly]
		public async Task<bool> IsDataSet()
		{
			try
			{
				return state.State.isDataSet;
			}
			catch (Exception ex)
			{
				logger.LogError($"LeaderboardGrain :: IsDataSet :: ex :: {ex}");
			}
			return false;
		}

		#endregion

		//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

		#region helper functions

		public async Task SaveState()
		{
			try
			{
				await state.WriteStateAsync();
			}
			catch (Exception ex)
			{
				logger.LogError($"LeaderboardGrain :: WriteStateAsync :: ex :: {ex}");
			}
		}

		public async Task ClearState()
		{
			try
			{
				await state.ClearStateAsync();
			}
			catch (Exception ex)
			{
				logger.LogError($"LeaderboardGrain :: ClearState :: ex :: {ex}");
			}
		}

		public async Task ClearStateAndDeactivateGrain()
		{
			try
			{
				await state.ClearStateAsync();
				DeactivateOnIdle();
			}
			catch (Exception ex)
			{
				logger.LogError($"LeaderboardGrain :: ClearStateAndDeactivateGrain :: ex :: {ex}");
			}
		}

		#endregion

		//-------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	}
}
