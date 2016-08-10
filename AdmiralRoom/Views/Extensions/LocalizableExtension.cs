using System.Windows;
using System.Windows.Markup;

namespace Huoyaoyuan.AdmiralRoom.Views.Extensions
{
    class LocalizableExtension : DynamicResourceExtension
    {
        private string _resourcekey;
        [ConstructorArgument("resourcekey")]
        public new string ResourceKey
        {
            get { return _resourcekey; }
            set
            {
                _resourcekey = value;
                base.ResourceKey = "LocalizedString_" + _resourcekey;
            }
        }
        public LocalizableExtension() { }
        public LocalizableExtension(string resourcekey) : base("LocalizedString_" + resourcekey) { }
    }
}
