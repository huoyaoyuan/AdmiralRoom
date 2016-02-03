using System;

namespace Huoyaoyuan.AdmiralRoom.Logger
{
    public struct DiffNumber : IFormattable
    {
        public int Value { get; set; }
        public int Diff { get; set; }
        public static DiffNumber Parse(string s) =>
            new DiffNumber { Value = int.Parse(s), Diff = 0 };
        public override string ToString() => Value.ToString();
        public string ToString(string format, IFormatProvider formatProvider = null)
        {
            if (format?.ToUpper() == "D")
                return Diff >= 0 ?
                    $"{Value}(+{Diff})" :
                    $"{Value}({Diff})";
            return ToString();
        }
        public static DiffNumber operator -(DiffNumber a, DiffNumber b)
            => new DiffNumber { Value = a.Value, Diff = a.Value - b.Value };
        public static implicit operator DiffNumber(int value)
            => new DiffNumber { Value = value, Diff = 0 };
        public static implicit operator int(DiffNumber number) => number.Value;
    }
}
