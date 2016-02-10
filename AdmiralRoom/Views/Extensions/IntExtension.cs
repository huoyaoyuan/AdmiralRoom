using System;
using System.Linq;
using System.Windows.Markup;

namespace Huoyaoyuan.AdmiralRoom.Views.Extensions
{
    class IntExtension : MarkupExtension
    {
        [ConstructorArgument("string")]
        public string String { get; set; }
        public IntExtension(string @string) { String = @string; }
        public override object ProvideValue(IServiceProvider serviceProvider) => int.Parse(String);
    }
    class IntsExtension : MarkupExtension
    {
        [ConstructorArgument("string")]
        public string String { get; set; }
        public IntsExtension(string @string) { String = @string; }
        public override object ProvideValue(IServiceProvider serviceProvider)
            => String.Split(',').Select(int.Parse).ToArray();
    }
}
