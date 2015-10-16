using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
