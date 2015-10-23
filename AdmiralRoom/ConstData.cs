using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huoyaoyuan.AdmiralRoom
{
    static class ConstData
    {
        static int[] f99exp;
        static int[] ad100exp =
        {
            1300000,1600000,1900000,2200000,2600000,3000000,
            3500000,4000000,4600000,5200000,5900000,
            6600000,7400000,8200000,9100000,10000000,
            11000000,12000000,13000000,14000000,15000000
        };
        static int[] ship100exp;
        public static int GetShipExp(int level)
        {
            try
            {
                if (level < 99) return f99exp[level - 1];
                else return ship100exp[level - 100];
            }
            catch { return 0; }
        }
        public static int GetAdmiralExp(int level)
        {
            try
            {
                if (level <= 99) return f99exp[level - 1];
                else return ad100exp[level - 100];
            }
            catch { return 0; }
        }
        public static string[] AdmiralRanks { get; } = { "", "元帥", "大将", "中将", "少将", "大佐", "中佐", "新米中佐", "少佐", "中堅少佐", "新米少佐" };
        public static string[] RanksWin = { "S", "A", "B" };
    }
}
