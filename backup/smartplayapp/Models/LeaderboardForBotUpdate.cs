using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
	public class LeaderboardForBotUpdate
	{
		public int user_id { get; set; }
		public bool is_bot { get; set; }
		public int games_played { get; set; }
		public int stars_won { get; set; }
		public LeaderboardForBotUpdate() { }
		public LeaderboardForBotUpdate(int user_id, bool is_bot, int games_played, int stars_won)
		{
			this.user_id = user_id;
			this.is_bot = is_bot;
			this.games_played = games_played;
			this.stars_won = stars_won;
		}
	}
}
