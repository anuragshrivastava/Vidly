using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DatabaseModels
{
    [Serializable]
	public class Leaderboard
	{
		public int id { get; set; }
		public int user_id { get; set; }
		public int games_played { get; set; }
		public int stars_won { get; set; }
		public DateTime last_updated { get; set; }

	}
}
