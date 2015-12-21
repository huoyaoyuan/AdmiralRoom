using System.Windows;
using System.Windows.Documents;

namespace Huoyaoyuan.AdmiralRoom.Controls
{
    public class Hyperlink2 : Hyperlink
    {
        static Hyperlink2()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Hyperlink2), new FrameworkPropertyMetadata(typeof(Hyperlink2)));
        }

        public string CommandLineString
        {
            get { return (string)GetValue(CommandLineStringProperty); }
            set { SetValue(CommandLineStringProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandLineString.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandLineStringProperty =
            DependencyProperty.Register("CommandLineString", typeof(string), typeof(Hyperlink2), new PropertyMetadata(null));

        protected override void OnClick()
        {
            if (CommandLineString != null)
                System.Diagnostics.Process.Start(CommandLineString);
        }
    }
}
