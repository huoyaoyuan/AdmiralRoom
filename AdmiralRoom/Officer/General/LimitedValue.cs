using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Huoyaoyuan.AdmiralRoom.Officer
{
    public struct LimitedValue
    {
        public int Current { get; set; }
        public int Max { get; set; }
        public bool IsMax => Current >= Max;
        public double Percentage => (double)Current / Max;
        public int Shortage => Max - Current;
        public LimitedValue(int current = 0,int max = 0)
        {
            Current = current;
            Max = max;
        }
        public LimitedValue(int[] value)
        {
            Current = value?[0] ?? 0;
            Max = value?[1] ?? 0;
        }
        public override string ToString()
        {
            return $"{Current}/{Max}";
        }
    }
}
