using System;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Huoyaoyuan.AdmiralRoom.Views.Extensions
{
    [MarkupExtensionReturnType(typeof(object))]
    class RuntimePathBindingExtension : MarkupExtension
    {
        [ConstructorArgument("pathsource")]
        public string PathSource { get; set; }
        public object Source { get; set; }
        public BindingMode Mode { get; set; }
        public RuntimePathBindingExtension() { }
        public RuntimePathBindingExtension(string pathsource)
        {
            PathSource = pathsource;
        }
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            IProvideValueTarget service = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            if (service == null) return null;
            target = service.TargetObject as FrameworkElement;
            property = service.TargetProperty as DependencyProperty;
            if (target == null || property == null) return this;
            target.DataContextChanged += (_, e) => CreateBinding(e);
            CreateBinding(target.DataContext);
            return null;
        }
        private FrameworkElement target;
        private DependencyProperty property;
        private void CreateBinding(object datacontext)
        {
            if (datacontext == null) return;
            Type type = datacontext.GetType();
            PropertyInfo property = type.GetProperty(this.PathSource);
            Binding binding = new Binding { Path = new PropertyPath(property.GetValue(datacontext).ToString()), Source = this.Source ?? datacontext, Mode = this.Mode };
            BindingOperations.SetBinding(this.target, this.property, binding);
        }
    }
}
