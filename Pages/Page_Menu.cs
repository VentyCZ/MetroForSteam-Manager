using System.Windows;
using System.Windows.Controls;
using consts = MetroSkinToolkit.MyApp.Constants;

namespace MetroSkinToolkit.Pages
{
    class Page_Menu : Page
    {
        MainWindow wnd;

        public Page_Menu(string name, FrameworkElement content, MainWindow mainWindow) : base(name, content, true)
        {
            wnd = mainWindow;

            wnd.Menu_Setup.Click += onMenuItemClick;
            wnd.Menu_Options.Click += onMenuItemClick;
            wnd.Menu_About.Click += onMenuItemClick;
        }

        private void onMenuItemClick(object sender, RoutedEventArgs e)
        {
            Button menuItem = (Button)sender;

            if (menuItem == wnd.Menu_Setup)
            {
                wnd.SetPage(consts.page_Setup);
            }
            else if (menuItem == wnd.Menu_Options)
            {
                wnd.SetPage(consts.page_Options);
            }
            else if (menuItem == wnd.Menu_About)
            {
                wnd.SetPage(consts.page_About);
            }
        }
    }
}
