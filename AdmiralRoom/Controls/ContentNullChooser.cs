using System.Windows;
using System.Windows.Controls;

namespace Huoyaoyuan.AdmiralRoom.Controls
{
    class ContentNullChooser : ContentControl
    {
        public object NullContent
        {
            get { return GetValue(NullContentProperty); }
            set { SetValue(NullContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NullContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NullContentProperty =
            DependencyProperty.Register("NullContent", typeof(object), typeof(ContentNullChooser), new PropertyMetadata(null, OnPropertyChanged));

        public object NotNullContent
        {
            get { return GetValue(NotNullContentProperty); }
            set { SetValue(NotNullContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NotNullContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NotNullContentProperty =
            DependencyProperty.Register("NotNullContent", typeof(object), typeof(ContentNullChooser), new PropertyMetadata(null, OnPropertyChanged));

        public object ContentChooser
        {
            get { return GetValue(ContentChooserProperty); }
            set { SetValue(ContentChooserProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContentSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContentChooserProperty =
            DependencyProperty.Register("ContentChooser", typeof(object), typeof(ContentNullChooser), new PropertyMetadata(null, OnPropertyChanged));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as ContentNullChooser).UpdateContent();
        private void UpdateContent()
        {
            if (ContentChooser == null) Content = NullContent;
            else Content = NotNullContent;
        }
    }
}
