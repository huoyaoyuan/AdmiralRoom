using System;
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
            Staff.RegisterHandler("api_req_hensei/change", x => ChangeHandler(x.Parse().Request));
            Staff.RegisterHandler("api_get_member/ship3", x => Ship3Handler(x.Parse<getmember_ship_deck>().Data));
            Staff.RegisterHandler("api_get_member/ship2", x => Ship2Handler(x.Parse<getmember_ship2>().Data));
            Staff.RegisterHandler("api_get_member/ship_deck", x => Ship3Handler(x.Parse<getmember_ship_deck>().Data));
            Staff.RegisterHandler("api_req_hokyu/charge", x => ChargeHandler(x.Parse<hokyu_charge>().Data));
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
        private IDTable<Ship> _ships;
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
        public Ship Secratary => Fleets[1].Ships[0];

        public void UpdateCounts()
        {
            Staff.Current.Admiral.ShipCount = Ships.Count;
            Staff.Current.Admiral.EquipCount = Equipments.Count;
        }

        public void RemoveShip(Ship ship)
        {
            if (ship.InFleet != null)
                Staff.Current.Dispatcher.Invoke(() => ship.InFleet.Ships.Remove(ship));
            foreach (var slot in ship.Slots)
                if (slot.HasItem)
                    Equipments.Remove(slot.Item);
            if (ship.SlotEx.HasItem)
                Equipments.Remove(ship.SlotEx.Item);
            Ships.Remove(ship);
            UpdateCounts();
        }

        void PortHandler(port_port api)
        {
            System.Runtime.GCSettings.LargeObjectHeapCompactionMode = System.Runtime.GCLargeObjectHeapCompactionMode.CompactOnce;
            Material.GetMemberMaterial(api.api_material);
            Staff.Current.Admiral.BasicHandler(api.api_basic);
            if(Ships == null)
                Ships = new IDTable<Ship>(api.api_ship.ArrayOperation(x => new Ship(x)));
            else Ships.UpdateAll(api.api_ship, x => x.api_id);
            Staff.Current.Admiral.ShipCount = api.api_ship.Length;
            Staff.Current.Shipyard.NDockHandler(api.api_ndock);
            DecksHandler(api.api_deck_port);
        }

        void DecksHandler(getmember_deck[] api)
        {
            if (Fleets == null)
            {
                Fleets = new IDTable<Fleet>(api.ArrayOperation(x => new Fleet(x)));
                SelectedFleet = 0;
            }
            else
            {
                Staff.Current.Dispatcher.Invoke(() => Fleets.UpdateAll(api, x => x.api_id));
            }
        }

        void ItemsHandler(getmember_slotitem[] api)
        {
            if (Equipments == null)
                Equipments = new IDTable<Equipment>(api.ArrayOperation(x => new Equipment(x)));
            else Equipments.UpdateAll(api, x => x.api_id);
            Staff.Current.Admiral.EquipCount = api.Length;
            Ships?.ArrayOperation(x => x.Update());
        }

        void ChangeHandler(NameValueCollection api)
        {
            int idx = api.GetInt("api_ship_idx");
            int fleetid = api.GetInt("api_id");
            int shipid = api.GetInt("api_ship_id");
            var fleet = Fleets[fleetid];
            Staff.Current.Dispatcher.Invoke(() =>
            {
                if (idx == -1)//旗艦以外全解除
                {
                    for (int i = fleet.Ships.Count - 1; i > 0; i--) 
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
                        if (idx < fleet.Ships.Count) fleet.Ships[idx].InFleet = destf;
                        if(destf != null)
                        {
                            if (idx < fleet.Ships.Count)
                                destf.Ships[destf.Ships.IndexOf(ship)] = fleet.Ships[idx];
                            else destf.Ships.Remove(ship);
                        }
                        if (idx >= fleet.Ships.Count) fleet.Ships.Add(ship);
                        else fleet.Ships[idx] = ship;
                        ship.InFleet = fleet;
                    }
                }
            });
        }

        void Ship3Handler(getmember_ship_deck api)
        {
            Ships.UpdateWithoutRemove(api.api_ship_data, x => x.api_id);
            Fleets.UpdateWithoutRemove(api.api_deck_data, x => x.api_id);
        }

        void Ship2Handler(getmember_ship2 api)
        {
            Ships.UpdateWithoutRemove(api.api_data, x => x.api_id);
            Fleets.UpdateWithoutRemove(api.api_data_deck, x => x.api_id);
        }

        void ChargeHandler(hokyu_charge api)
        {
            foreach (var ship in api.api_ship)
            {
                var Ship = Ships[ship.api_id];
                Ship.Fuel = new LimitedValue(ship.api_fuel, Ship.Fuel.Max);
                Ship.Bull = new LimitedValue(ship.api_bull, Ship.Bull.Max);
                for (int i = 0; i < Ship.Slots.Count; i++)
                    Ship.Slots[i].AirCraft = new LimitedValue(ship.api_onslot[i], Ship.Slots[i].AirCraft.Max);
            }
            Material.Fuel = api.api_material[0];
            Material.Bull = api.api_material[1];
            Material.Steel = api.api_material[2];
            Material.Bauxite = api.api_material[3];
        }
    }
}
