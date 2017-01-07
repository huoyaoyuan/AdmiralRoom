using System;
using System.Linq;
using Meowtrix.ComponentModel;

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
        public override string ToString() => HasItem && Item.EquipInfo.EquipType.IsAirPlane ? Item + " (" + AirCraft + ")" : Item?.ToString();
        private static readonly int[] fighttype = { 6, 7, 8, 11, 45, 56, 57, 58 };
        private static readonly int[] bonus1 = { 0, 0, 2, 5, 9, 14, 14, 22 };//舰战，水战
        private static readonly int[] bonus2 = { 0, 0, 1, 1, 1, 3, 3, 6 };//水爆
        private static readonly int[] inner = { 0, 10, 25, 40, 55, 70, 85, 100, 121 };
        public bool CanProvideAirFightPower => fighttype.Contains(Item?.EquipInfo.EquipType.Id ?? 0);
        public AirFightPower AirFightPower
        {
            get
            {
                if (Item == null) return new AirFightPower();
                if (AirCraft.Current == 0) return new AirFightPower();
                double raw, bonusmin, bonusmax;
                int itemtype = Item.EquipInfo.EquipType.Id;
                int level = Item.AirProficiency;
                bool isfighter;
                switch (itemtype)
                {
                    case 6://艦上戦闘機
                    case 45://水上戦闘機
                    case 56://噴式戦闘機
                        bonusmin = bonusmax = bonus1[level];
                        isfighter = true;
                        break;
                    case 11://水上爆撃機
                        bonusmin = bonusmax = bonus2[level];
                        isfighter = false;
                        break;
                    case 7://艦上爆撃機
                    case 8://艦上攻撃機
                    case 57://噴式戦闘爆撃機
                    case 58://噴式攻撃機
                        bonusmin = bonusmax = 0;
                        isfighter = false;
                        break;
                    default:
                        return new AirFightPower();
                }
                raw = Math.Sqrt(AirCraft.Current) * Item.EquipInfo.AA;
                bonusmin += Math.Sqrt(inner[level] / 10.0) + Math.Sqrt(AirCraft.Current) * Item.ImproveLevel * 0.2;
                bonusmax += Math.Sqrt((inner[level + 1] - 1) / 10.0) + Math.Sqrt(AirCraft.Current) * Item.ImproveLevel * 0.2;
                return new AirFightPower(isfighter, raw, bonusmin, bonusmax);
            }
        }
        public double LoSInMap
        {
            get
            {
                if (!HasItem) return 0;
                double factor = 0;
                double improvementfactor = 0;
                switch (Config.Current.LosCalcType)
                {
                    case LosCalcType.SimpleSum:
                        return Item.EquipInfo.LoS;
                    case LosCalcType.Formula14Q3:
                        switch (Item.EquipInfo.EquipType.Id)
                        {
                            case 9://艦上偵察機
                            case 94://艦上偵察機(II)
                            case 10://水上偵察機
                            case 11://水上爆撃機
                                return 2 * Item.EquipInfo.LoS;
                            case 12://小型電探
                            case 13://大型電探
                            case 93://大型電探(II)
                                factor = 1;
                                return Item.EquipInfo.LoS;
                            default:
                                return 0;
                        }
                    case LosCalcType.Formula14Q4:
                        switch (Item.EquipInfo.EquipType.Id)
                        {
                            case 7://艦上爆撃機
                                factor = 1.04;
                                break;
                            case 8://艦上攻撃機
                                factor = 1.37;
                                break;
                            case 9://艦上偵察機
                            case 94://艦上偵察機(II)
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
                            case 93://大型電探(II)
                                factor = 0.99;
                                break;
                            case 29://探照灯
                            case 42://大型探照灯
                                factor = 0.91;
                                break;
                            default:
                                return 0;
                        }
                        break;
                    case LosCalcType.Formula16Q1:
                        switch (Item.EquipInfo.EquipType.Id)
                        {
                            case 8://艦上攻撃機
                                factor = 0.8;
                                break;
                            case 9://艦上偵察機
                            case 94://艦上偵察機(II)
                                factor = 1.0;
                                break;
                            case 10://水上偵察機
                                factor = 1.2;
                                improvementfactor = 1.2;
                                break;
                            case 11://水上爆撃機
                                factor = 1.1;
                                break;
                            case 6://艦上戦闘機
                            case 7://艦上爆撃機
                            case 29://探照灯
                            case 42://大型探照灯
                            case 45://水上戦闘機
                            case 41://大型飛行艇
                            case 14://ソナー
                            case 40://大型ソナー
                            case 39://水上艦要員
                            case 26://対潜哨戒機
                            case 34://司令部施設
                                factor = 0.6;
                                break;
                            case 12://小型電探
                            case 13://大型電探
                            case 93://大型電探(II)
                                factor = 0.6;
                                improvementfactor = 1.25;
                                break;
                            default:
                                return 0;
                        }
                        break;
                }
                return factor * Item.EquipInfo.LoS + improvementfactor * Math.Sqrt(Item.ImproveLevel);
            }
        }
    }
    public struct AirFightPower
    {
        public double TotalMin => Raw + BonusMin;
        public double TotalMax => Raw + BonusMax;
        public double FighterMin => FighterRaw + FighterBonusMin;
        public double FighterMax => FighterRaw + FighterBonusMax;
        public double Raw => FighterRaw + BomberRaw;
        public double FighterRaw { get; set; }
        public double BomberRaw { get; set; }
        public double FighterBonusMin { get; set; }
        public double FighterBonusMax { get; set; }
        public double BomberBonusMin { get; set; }
        public double BomberBonusMax { get; set; }
        public double BonusMin => FighterBonusMin + BomberBonusMin;
        public double BonusMax => FighterBonusMax + BomberBonusMax;
        public static AirFightPower operator +(AirFightPower a, AirFightPower b)
            => new AirFightPower
            {
                FighterRaw = a.FighterRaw + b.FighterRaw,
                BomberRaw = a.BomberRaw + b.BomberRaw,
                FighterBonusMin = a.FighterBonusMin + b.FighterBonusMin,
                FighterBonusMax = a.FighterBonusMax + b.FighterBonusMax,
                BomberBonusMin = a.BomberBonusMin + b.BomberBonusMin,
                BomberBonusMax = a.BomberBonusMax + b.BomberBonusMax
            };
        public AirFightPower(bool isfighter, double raw, double bonusmin, double bonusmax)
        {
            if (isfighter)
            {
                FighterRaw = raw;
                FighterBonusMin = bonusmin;
                FighterBonusMax = bonusmax;
                BomberRaw = 0;
                BomberBonusMin = 0;
                BomberBonusMax = 0;
            }
            else
            {
                FighterRaw = 0;
                FighterBonusMin = 0;
                FighterBonusMax = 0;
                BomberRaw = raw;
                BomberBonusMin = bonusmin;
                BomberBonusMax = bonusmax;
            }
        }
    }
}
