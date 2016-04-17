using System;
using System.Linq;
using System.Linq.Expressions;

namespace Huoyaoyuan.AdmiralRoom
{
    public static class EnumParseHelper
    {
        public static Expression GetParser(Type enumType)
        {
            if (!enumType.IsEnum) throw new ArgumentException(nameof(enumType));
            var names = Enum.GetNames(enumType);
            var values = Enum.GetValues(enumType);
            var input = Expression.Parameter(typeof(string));
            var cases = values.Cast<Enum>().Zip(names, (value, name) =>
                Expression.SwitchCase(Expression.Constant(value, enumType), Expression.Constant(name))).ToArray();
            var switchcase = Expression.Switch(input, Expression.Constant(Activator.CreateInstance(enumType)), cases);
            var exp = Expression.Lambda(switchcase, input);
            return exp;
        }
    }
}
