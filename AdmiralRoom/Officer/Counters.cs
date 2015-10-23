using System.Collections.Specialized;
using System.Linq;
using Fiddler;
using Huoyaoyuan.AdmiralRoom.API;

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
	public class SortieCounter : CounterBase
	{
		private SortieCounter()
		{
			Staff.API("api_req_map/start")
				.Subscribe((Session x) => Increase());
		}
		public static SortieCounter Instance { get; } = new SortieCounter();
	}
	public class BattleCounter : CounterBase
	{
		private BattleCounter()
		{
			Staff.API("api_req_sortie/battleresult")
				.Subscribe((Session x) => Increase());
		}
		public static BattleCounter Instance { get; } = new BattleCounter();
	}
	public class BattleWinCounter : CounterBase
	{
		private BattleWinCounter()
		{
			Staff.API("api_req_sortie/battleresult")
				.Where<sortie_battleresult>(x => ConstData.RanksWin.Contains(x.api_win_rank))
				.Subscribe((x) => Increase());
		}
		public static BattleWinCounter Instance { get; } = new BattleWinCounter();
	}
}