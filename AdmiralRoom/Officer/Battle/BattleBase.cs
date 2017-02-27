using System.Collections.Generic;
using System.Linq;
using Meowtrix.ComponentModel;

namespace Huoyaoyuan.AdmiralRoom.Officer.Battle
{
    public class BattleBase : NotificationObject
    {
        public CombinedFleetType FleetType { get; set; }
        public ShipInBattle[] Fleet1 { get; set; }
        public ShipInBattle[] Fleet2 { get; set; }
        public ShipInBattle[] EnemyFleet { get; set; }
        public ShipInBattle[] EnemyFleet2 { get; set; }
        public int[] EnemyShipIds { get; protected set; }
        public IEnumerable<ShipInBattle> AllFriends => Fleet1.ConcatNotNull(Fleet2);
        public IEnumerable<ShipInBattle> AllEnemies => EnemyFleet.ConcatNotNull(EnemyFleet2);
        public virtual bool IsBattling => false;

        #region GetShip
        private string _getship;
        public string GetShip
        {
            get { return _getship; }
            set
            {
                if (_getship != value)
                {
                    _getship = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        protected BattleBase() { }
        public BattleBase(BattleManager source)
        {
            Fleet1 = source.SortieFleet1.Ships
                .Select((x, i) => new ShipInBattle(x) { Index = i + 1 }).ToArray();
            Fleet2 = source.SortieFleet2?.Ships
                .Select((x, i) => new ShipInBattle(x) { Index = i + 7 }).ToArray();
        }
    }
}
