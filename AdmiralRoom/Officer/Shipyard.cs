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
            Staff.RegisterHandler("api_get_member/kdock", x =>
                Staff.Current.Dispatcher.Invoke(() =>
                BuildingDocks.UpdateAll(x.Parse<getmember_kdock[]>().Data, api => api.api_id)));
            Staff.RegisterHandler("api_req_kousyou/createship", x => CreateShipHandler(x.Parse().Request));
            Staff.RegisterHandler("api_req_kousyou/getship", x => GetShipHandler(x.Parse<req_getship>().Data));
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

        #region BuildingDocks
        private IDTable<BuildingDock> _buildingdocks = new IDTable<BuildingDock>();
        public IDTable<BuildingDock> BuildingDocks
        {
            get { return _buildingdocks; }
            set
            {
                if (_buildingdocks != value)
                {
                    _buildingdocks = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        private int[] inndock = { };
        public void NDockHandler(getmember_ndock[] api)
        {
            Staff.Current.Dispatcher.Invoke(() => RepairDocks.UpdateAll(api, x => x.api_id));
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
            inndock[req.GetInt("api_ndock_id") - 1] = req.GetInt("api_ship_id");
        }
        void CreateShipHandler(NameValueCollection req)
        {
            int dockid = req.GetInt("api_kdock_id");
            BuildingDocks[dockid].IsLSC = req.GetInt("api_large_flag") != 0;
            BuildingDocks[dockid].Secratary = Staff.Current.Homeport.Secratary;
        }
        void GetShipHandler(req_getship api)
        {
            if (api.api_slotitem != null)
                foreach (var item in api.api_slotitem)
                    Staff.Current.Homeport.Equipments.Add(new Equipment(item));
            Staff.Current.Homeport.Ships.Add(new Ship(api.api_ship));
            Staff.Current.Admiral.ShipCount = Staff.Current.Homeport.Ships.Count;
            Staff.Current.Admiral.EquipCount = Staff.Current.Homeport.Equipments.Count;
        }
    }
}
