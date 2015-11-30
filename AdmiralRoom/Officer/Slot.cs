using System;
using System.Linq;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public class Slot : NotificationObject
    {
        #region IsLocked
        private bool _islocked;
        public bool IsLocked
        {
            get { return _islocked; }
            set
            {
                if (_islocked != value)
                {
                    _islocked = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Item
        private Equipment _item;
        public Equipment Item
        {
            get { return _item; }
            set
            {
                if (_item != value)
                {
                    _item = value;
                    OnAllPropertyChanged();
                }
            }
        }
        #endregion

        #region AirCraft
        private LimitedValue _ac;
        public LimitedValue AirCraft
        {
            get { return _ac; }
            set
            {
                _ac = value;
                OnAllPropertyChanged();
            }
        }
        #endregion

        public bool HasItem => Item != null;
        public override string ToString()
        {
            if (HasItem && Item.EquipInfo.EquipType.IsAirPlane)
                return Item.ToString() + " (" + AirCraft.ToString() + ")";
            else return Item?.ToString();
        }
        private static readonly int[] fighttype = { 6, 7, 8, 11 };
        private static readonly int[] bonus1 = { 0, 0, 2, 5, 9, 14, 14, 22 };//舰战
        private static readonly int[] bonus2 = { 0, 0, 1, 1, 1, 3, 3, 6 };//水爆
        private static readonly int[] inner = { 0, 10, 25, 40, 55, 70, 85, 100, 121 };
        public bool CanProvideAirFightPower => fighttype.Contains(Item?.EquipInfo.EquipType.Id ?? 0);
        /// <summary>
        /// [0]:总min [1]:总max [2]:除攻击机min [3]:除攻击机max [4]:裸 [5]:除攻击机裸 [6]:熟练度加成min [7]:熟练度加成max
        /// </summary>
        public double[] AirFightPower
        {
            get
            {
                double[] res = new double[8];
                int itemtype = Item?.EquipInfo.EquipType.Id ?? 0;
                if (CanProvideAirFightPower)
                    res[4] = Math.Sqrt(AirCraft.Current) * Item.EquipInfo.AA;
                else return res;
                if (AirCraft.Current == 0) return res;
                int level = Item.AirProficiency;
                res[5] = itemtype == 6 ? res[4] : 0;
                if (itemtype == 6) res[6] = res[7] = bonus1[level];
                else if (itemtype == 11) res[6] = res[7] = bonus2[level];
                res[6] += Math.Sqrt(inner[level] / 10.0);
                res[7] += Math.Sqrt((inner[level + 1] - 1) / 10.0);
                res[3] = itemtype == 6 ? res[4] + res[7] : 0;
                res[2] = itemtype == 6 ? res[4] + res[6] : 0;
                res[1] = res[4] + res[7];
                res[0] = res[4] + res[6];
                return res;
            }
        }
        public double LoSInMap
        {
            get
            {
                if (!HasItem) return 0;
                double factor = 0;
                switch (Item.EquipInfo.EquipType.Id)
                {
                    case 7://艦上爆撃機
                        factor = 1.04;
                        break;
                    case 8://艦上攻撃機
                        factor = 1.37;
                        break;
                    case 9://艦上偵察機
                        factor = 1.66;
                        break;
                    case 10://水上偵察機
                        factor = 2.00;
                        break;
                    case 11://水上爆撃機
                        factor = 1.78;
                        break;
                    case 12://小型電探
                        factor = 1.00;
                        break;
                    case 13://大型電探
                        factor = 0.99;
                        break;
                    case 29://探照灯
                        factor = 0.91;
                        break;
                    case 42://大型探照灯
                        factor = 0.91;
                        break;
                    case 93://大型電探(II)
                        factor = 0.99;
                        break;
                }
                return factor * Item.EquipInfo.LoS;
            }
        }
    }
}
