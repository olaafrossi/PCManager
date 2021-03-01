using ModernWpf.Controls;
using System.Windows;
using System.Windows.Controls;

namespace PCManager.WPFUI.ControlPages
{
    public partial class WindowPage
    {
        public WindowPage()
        {
            InitializeComponent();
        }

        private void ResizeWindow(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is MenuItem menuItem)
            {
                var size = Size.Parse(((string)menuItem.Header).Replace('×', ','));
                var window = Window.GetWindow(this);
                window.Width = size.Width;
                window.Height = size.Height;
            }
        }


    }
}
