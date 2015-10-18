using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Huoyaoyuan.AdmiralRoom.API;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class Shipyard : NotifyBase
    {
        public Shipyard()
        {
            Staff.RegisterHandler("api_get_member/ndock", x => NDockHandler(x.Parse<getmember_ndock[]>().Data));
            Staff.RegisterHandler("api_req_nyukyo/start", x => NStartHandler(x.Parse().Request));
        }

        #region RepairDocks
        private IDTable<RepairDock> _repairdocks = new IDTable<RepairDock>();
        public IDTable<RepairDock> RepairDocks
        {
            get { return _repairdocks; }
            set
            {
                if (_repairdocks != value)
                {
                    _repairdocks = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        private int[] inndock = { };
        public void NDockHandler(getmember_ndock[] api)
        {
            RepairDocks.UpdateAll(api, x => x.api_id);
            var newindock = api.ArrayOperation(x => x.api_ship_id).ToArray();
            foreach(var ship in Staff.Current.Homeport.Ships)
            {
                if (newindock.Contains(ship.Id))
                    ship.IsRepairing = true;
                else if (inndock.Contains(ship.Id))
                {
                    ship.IsRepairing = false;
                    ship.RepairingHP = ship.HP.Max;
                }
                else ship.IsRepairing = false;
            }
            inndock = newindock;
        }
        void NStartHandler(NameValueCollection req)
        {
            inndock[int.Parse(req["api_ndock_id"]) - 1] = int.Parse(req["api_ship_id"]);
        }
    }
}
