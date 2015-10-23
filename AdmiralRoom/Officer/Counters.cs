using System.Collections.Specialized;
using Fiddler;
namespace Huoyaoyuan.AdmiralRoom.Officer.Counters
{
	public class ChargeCounter : CounterBase
	{
		private ChargeCounter()
		{
			Staff.API("api_req_hokyu/charge")
				.Subscribe((Session x) => Increase());
		}
		public static ChargeCounter Instance { get; } = new ChargeCounter();
	}
	public class RepairCounter : CounterBase
	{
		private RepairCounter()
		{
			Staff.API("api_req_nyukyo/start")
				.Subscribe((Session x) => Increase());
		}
		public static RepairCounter Instance { get; } = new RepairCounter();
	}
}