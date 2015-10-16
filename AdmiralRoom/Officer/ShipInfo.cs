﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Huoyaoyuan.AdmiralRoom.API;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class ShipInfo : GameObject<api_mst_ship>, IIdentifiable
    {
        public int Id => rawdata.api_id;
        public string Name => rawdata.api_name ?? "？？？";
        public int SortNo => rawdata.api_sort_no;
        public string Yomi => rawdata.api_yomi;
        public ShipSpeed Speed => (ShipSpeed)rawdata.api_soku;
        public bool IsAbyssal => SortNo == 0;
        public AbysallClass AbyssalClass
        {
            get
            {
                if (!IsAbyssal) return AbysallClass.Normal;
                if (Yomi == "elite") return AbysallClass.Elite;
                if (Yomi == "flagship")
                {
                    if (Name.EndsWith("改")) return AbysallClass.KFlagship;
                    return AbysallClass.Flagship;
                }
                return AbysallClass.Normal;
            }
        }
        public bool CanUpgrade => rawdata.api_aftershipid != 0;
        public int UpgradeLevel => rawdata.api_afterlv;
        public int UpgradeTo => rawdata.api_aftershipid;
        public LimitedValue FirePower { get; private set; }
        public LimitedValue Torpedo { get; private set; }
        public LimitedValue AA { get; private set; }
        public LimitedValue Armor { get; private set; }
        public LimitedValue Luck { get; private set; }
        public int MaxHP => rawdata.api_tyku?[0] ?? 0;
        public ShootRange Range => (ShootRange)rawdata.api_leng;
        public int SlotNum => rawdata.api_slot_num;
        public int[] AirCraft => rawdata.api_maxeq;
        public TimeSpan BuildTime => TimeSpan.FromMinutes(rawdata.api_buildtime);
        public int MaxFuel => rawdata.api_fuel_max;
        public int MaxBull => rawdata.api_bull_max;
        public int[] DestroyMaterial => rawdata.api_broken;
        public int[] MordenizeValue => rawdata.api_powup;
        public int Rare => rawdata.api_backs;
        public string Message => rawdata.api_getmes;
        public ShipType ShipType => Staff.Current.MasterData.ShipTypes[rawdata.api_stype];
        public ShipInfo(api_mst_ship api) : base(api) { }
        protected override void UpdateProp()
        {
            FirePower = new LimitedValue(rawdata.api_houg);
            Torpedo = new LimitedValue(rawdata.api_raig);
            AA = new LimitedValue(rawdata.api_tyku);
            Armor = new LimitedValue(rawdata.api_souk);
        }
    }
    public enum ShipSpeed { None = 0, Low = 5, High = 10 }
    public enum AbysallClass { Normal, Elite, Flagship, KFlagship }
}