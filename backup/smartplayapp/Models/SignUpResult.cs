using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [Serializable]
    public class SignUpResult
    {
        public bool success { get; set; }
        public bool isExists { get; set; }
        public string reason { get; set; }
        public string token { get; set; }

        public SignUpResult(bool _success, bool _isExists, string _reason = null, string _token = null)
        {
            success = _success;
            isExists = _isExists;
            reason = _reason;
            token = _token;
        }
    }
}
