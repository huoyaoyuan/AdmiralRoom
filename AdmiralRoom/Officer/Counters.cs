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
	public class PracticeCounter : CounterBase
	{
		private PracticeCounter()
		{
			Staff.API("api_req_practice/battle_result")
				.Subscribe((Session x) => Increase());
		}
		public static PracticeCounter Instance { get; } = new PracticeCounter();
	}
	public class PracticeWinCounter : CounterBase
	{
		private PracticeWinCounter()
		{
			Staff.API("api_req_practice/battle_result")
				.Where<sortie_battleresult>(x => ConstData.RanksWin.Contains(x.api_win_rank))
				.Subscribe((x) => Increase());
		}
		public static PracticeWinCounter Instance { get; } = new PracticeWinCounter();
	}
	public class ItemDestroyCounter : CounterBase
	{
		private ItemDestroyCounter()
		{
			Staff.API("api_req_kousyou/destroyitem2")
				.Subscribe((Session x) => Increase());
		}
		public static ItemDestroyCounter Instance { get; } = new ItemDestroyCounter();
	}
	public class ExpeditionCounter : CounterBase
	{
		private ExpeditionCounter()
		{
			Staff.API("api_req_mission/result")
				.Where<mission_result>(x => x.api_clear_result>0)
				.Subscribe((x) => Increase());
		}
		public static ExpeditionCounter Instance { get; } = new ExpeditionCounter();
	}
	public class ExpeditionTokyoCounter : CounterBase
	{
		private ExpeditionTokyoCounter()
		{
			Staff.API("api_req_mission/result")
				.Where<mission_result>(x => x.api_quest_name.Contains("東京急行"))
				.Subscribe((x) => Increase());
		}
		public static ExpeditionTokyoCounter Instance { get; } = new ExpeditionTokyoCounter();
	}
}