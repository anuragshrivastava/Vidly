using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [Serializable]
    public class LeaderboardWithUserDetails
    {
        public string username { get; set; }
        public string profile_url { get; set; }
        public int games_played { get; set; }
        public int stars_won { get; set; }
        public DateTime last_updated { get; set; }

        public LeaderboardWithUserDetails() { }
        public LeaderboardWithUserDetails(string username, string profile_url, int games_played, int stars_won, DateTime last_updated)
		{
			this.username = username;
			this.profile_url = profile_url;
			this.games_played = games_played;
			this.stars_won = stars_won;
			this.last_updated = last_updated;
		}   
	}
}
