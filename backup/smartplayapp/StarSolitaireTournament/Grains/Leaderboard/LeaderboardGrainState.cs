using Models;

namespace StarSolitaireTournament.Grains
{
	[Serializable]
	public class LeaderboardGrainState
	{
		public List<LeaderboardWithUserDetails> leaderboardItems { get; set; }
		public bool isDataSet { get; set; }

		public LeaderboardGrainState()
		{

		}
		public LeaderboardGrainState(List<LeaderboardWithUserDetails> leaderboardItems, bool isDataSet)
		{
			this.leaderboardItems = leaderboardItems;
			this.isDataSet = isDataSet;
		}
	}
}
