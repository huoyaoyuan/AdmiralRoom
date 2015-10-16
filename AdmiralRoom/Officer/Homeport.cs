using System.Collections.Generic;
using System.Collections.Specialized;
using Huoyaoyuan.AdmiralRoom.API;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class Homeport : NotifyBase
    {
        public Homeport()
        {
            Staff.RegisterHandler("api_port/port", x => PortHandler(x.Parse<port_port>().Data));
            Staff.RegisterHandler("api_get_member/deck", x => DecksHandler(x.Parse<getmember_deck[]>().Data));
            Staff.RegisterHandler("api_get_member/slot_item", x => ItemsHandler(x.Parse<getmember_slotitem[]>().Data));
            Staff.RegisterHandler("api_req_hensei/change", x => ChangeHandler(x.Parse<object>().Request));
        }

        public Material Material { get; } = new Material();

        #region Fleets
        private IDTable<Fleet> _fleets;
        public IDTable<Fleet> Fleets
        {
            get { return _fleets; }
            set
            {
                if (_fleets != value)
                {
                    _fleets = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Ships
        private IDTable<Ship> _ships = new IDTable<Ship>();
        public IDTable<Ship> Ships
        {
            get { return _ships; }
            set
            {
                if (_ships != value)
                {
                    _ships = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region SelectedFleet
        private int _selectedfleet = 0;
        public int SelectedFleet
        {
            get { return _selectedfleet; }
            set
            {
                if (_selectedfleet != value)
                {
                    _selectedfleet = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Equipments
        private IDTable<Equipment> _equipments;
        public IDTable<Equipment> Equipments
        {
            get { return _equipments; }
            set
            {
                if (_equipments != value)
                {
                    _equipments = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        void PortHandler(port_port api)
        {
            Material.GetMemberMaterial(api.api_material);
            Staff.Current.Admiral.BasicHandler(api.api_basic);
            var ships = new List<Ship>();
            foreach (var ship in api.api_ship)
                ships.Add(new Ship(ship));
            Ships = new IDTable<Ship>(ships);
            Staff.Current.Admiral.ShipCount = api.api_ship.Length;
            DecksHandler(api.api_deck_port);
        }

        void DecksHandler(getmember_deck[] api)
        {
            if (Fleets == null)
            {
                var fleets = new IDTable<Fleet>();
                foreach (var deck in api)
                    fleets.Add(new Fleet(deck));
                Fleets = fleets;
                SelectedFleet = 0;
            }
            else
            {
                Staff.Current.Dispatcher.Invoke(() =>
                {
                    foreach (var deck in api)
                    {
                        if (Fleets[deck.api_id] != null) Fleets[deck.api_id].Update(deck);
                        else Fleets[deck.api_id] = new Fleet(deck);
                    }
                });
            }
        }

        void ItemsHandler(getmember_slotitem[] api)
        {
            var equips = new IDTable<Equipment>();
            foreach(var equip in api)
            {
                equips.Add(new Equipment(equip));
            }
            Equipments = equips;
            Staff.Current.Admiral.EquipCount = api.Length;
        }
        void ChangeHandler(NameValueCollection api)
        {
            int idx = int.Parse(api["api_ship_idx"]);
            int fleetid = int.Parse(api["api_id"]);
            int shipid = int.Parse(api["api_ship_id"]);
            var fleet = Fleets[fleetid];
            Staff.Current.Dispatcher.Invoke(() =>
            {
                if (idx == -1)//旗艦以外全解除
                {
                    for (int i = 2; i <= fleet.Ships.Count; i++)
                    {
                        fleet.Ships[i].InFleet = null;
                        fleet.Ships.Remove(fleet.Ships[i]);
                    }
                }
                else
                {
                    if (shipid == -1)//はずす
                    {
                        fleet.Ships[idx].InFleet = null;
                        fleet.Ships.Remove(fleet.Ships[idx]);
                    }
                    else
                    {
                        var ship = Ships[shipid];
                        var destf = ship.InFleet;
                        if(destf != null)
                        {
                            destf.Ships[destf.Ships.IndexOf(ship)] = fleet.Ships[idx];
                        }
                        fleet.Ships[idx] = ship;
                    }
                }
            });
        }
    }
}
