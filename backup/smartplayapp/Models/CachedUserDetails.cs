using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [Serializable]
    public class CachedUserDetails
    {
        public string id { get; set; }
        public string firebase_token { get; set; }
        public string google_userid { get; set; }
        public string username { get; set; }
        public string profile_url { get; set; }
        public string email_id { get; set; }
        public int leaderboard_id { get; set; }
        public int games_played { get; set; }
        public int stars_won { get; set; }
        public int last_updated { get; set; }
    }
}
