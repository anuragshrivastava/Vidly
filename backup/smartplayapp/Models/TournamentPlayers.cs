using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
	[Serializable]
	public class TournamentPlayers
	{
		public bool success { get; set; }
		public string reason { get; set; }
		public List<LeaderboardWithUserDetails> players { get; set; }
		public TournamentPlayers(bool _success, string _reason = null, List<LeaderboardWithUserDetails> _players = null)
		{
			success = _success;
			reason = _reason;
			players = _players;
		}
	}
}
