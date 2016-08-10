using System;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;

namespace Huoyaoyuan.AdmiralRoom.Views.Extensions
{
    [MarkupExtensionReturnType(typeof(object))]
    class Localizable2Extension : MarkupExtension
    {
        [ConstructorArgument("pathsource")]
        public string PathSource { get; set; }
        public Localizable2Extension() { }
        public Localizable2Extension(string pathsource)
        {
            PathSource = pathsource;
        }
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            IProvideValueTarget service = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            if (service == null) return null;
            targetelement = service.TargetObject as FrameworkElement;
            dp = service.TargetProperty as DependencyProperty;
            if (targetelement == null || dp == null) return this;
            targetelement.DataContextChanged += (s, e) => CreateBinding(s as DependencyObject, e.NewValue);
            CreateBinding(targetelement, targetelement.DataContext);
            return null;
        }
        private FrameworkElement targetelement;
        private DependencyProperty dp;
        private void CreateBinding(DependencyObject target, object datacontext)
        {
            if (datacontext == null || target == null) return;
            Type type = datacontext.GetType();
            PropertyInfo property = type.GetProperty(PathSource);
            if (property == null) return;
            ResourceService.Current.SetStringTableBinding(target, dp, property.GetValue(datacontext).ToString());
        }
    }
}
