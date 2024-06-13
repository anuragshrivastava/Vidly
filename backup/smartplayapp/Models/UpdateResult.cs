using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
	[Serializable]
	public class UpdateResult
	{
		public bool isSuccess { get; set; }
		public string reason { get; set; }
		public UpdateResult() { }
		public UpdateResult(bool isSuccess, string reason)
		{
			this.isSuccess = isSuccess;
			this.reason = reason;
		}
	}
}
