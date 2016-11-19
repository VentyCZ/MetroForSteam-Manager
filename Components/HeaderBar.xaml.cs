using System.Windows;
using System.Windows.Controls;

namespace MetroSkinToolkit.Components
{
    public partial class HeaderBar : UserControl
    {
        TabWindow wnd = null;

        public HeaderBar()
        {
            InitializeComponent();

            Button_Close.Click += onClose;
            Button_Back.Click += onBack;
            
            MouseLeftButtonDown += (object sender, System.Windows.Input.MouseButtonEventArgs e) =>
            {
                wnd.DragMove();
            };

            ToggleBackButton(false);
        }

        private void onBack(object sender, RoutedEventArgs e)
        {
            if (wnd != null)
                wnd.GoBack();
        }

        private void onClose(object sender, RoutedEventArgs e)
        {
            if (wnd != null)
                wnd.Close();
        }

        public void ToggleCloseButton(bool state)
        {
            MyApp.ExecuteOnUI(delegate ()
            {
                Button_Back.IsEnabled = state;
            });
        }

        public void ToggleBackButton(bool state)
        {
            MyApp.ExecuteOnUI(delegate ()
            {
                Button_Back.Visibility = (state ? Visibility.Visible : Visibility.Hidden);
            });
        }

        public void SetTitle(string title)
        {
            MyApp.ExecuteOnUI(delegate () {
                WindowTitler.Text = title;
            });
        }

        public void Connect(TabWindow window)
        {
            if (wnd != null)
                return;

            wnd = window;
            SetTitle(window.Title);

            //Register events, if necessary
        }
    }
}
