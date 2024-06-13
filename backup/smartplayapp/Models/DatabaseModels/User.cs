using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DatabaseModels
{
    [Serializable]
    public class User
    {
        public int id { get; set; }
        public string firebase_token { get; set; }
        public string google_userid { get; set; }
        public string username { get; set; }
        public string profile_url { get; set; }
        public string email_id { get; set; }
        public DateTime registration_datetime { get; set; }
    }
}
