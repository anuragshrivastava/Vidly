using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
	[Serializable]
	public class UserDetails
	{
		public bool isSuccess { get; set; }
		public string reason { get; set; }
		public string username { get; set; }
		public string profileUrl { get; set; }
		public int gamesPlayed { get; set; }
		public string token { get; set; }
		public int starsWon { get; set; }

		public DateTime lastUpdated { get; set; }

		public UserDetails() { }

		public UserDetails(bool isSuccess, string reason, string username, string profileUrl, int gamesPlayed, int starsWon, string token, DateTime lastUpdated)
		{
			this.isSuccess = isSuccess;
			this.reason = reason;
			this.username = username;
			this.profileUrl = profileUrl;
			this.gamesPlayed = gamesPlayed;
			this.starsWon = starsWon;
			this.token = token;
			this.lastUpdated = lastUpdated;
		}

		public UserDetails(bool isSuccess, string reason)
		{
			this.isSuccess = isSuccess;
			this.reason = reason;
		}
	}
}
