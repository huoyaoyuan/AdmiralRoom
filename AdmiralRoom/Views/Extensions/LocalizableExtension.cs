using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Huoyaoyuan.AdmiralRoom.Views.Extensions
{
    class LocalizableExtension : Binding
    {
        private string _resourcekey;
        [ConstructorArgument("resourcekey")]
        public string ResourceKey
        {
            get { return _resourcekey; }
            set
            {
                _resourcekey = value;
                Path = new PropertyPath("Resources." + value);
            }
        }
        public LocalizableExtension(string resourcekey)
        {
            ResourceKey = resourcekey;
            Source = ResourceService.Current;
            Mode = BindingMode.OneWay;
        }
    }
}
