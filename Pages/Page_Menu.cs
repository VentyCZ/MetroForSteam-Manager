using System.Windows;
using System.Windows.Controls;
using consts = MetroSkinToolkit.MyApp.Constants;

namespace MetroSkinToolkit.Pages
{
    class Page_Menu : Page
    {
        MainWindow wnd;
        Views.Menu view;

        public Page_Menu(string name, FrameworkElement content, MainWindow mainWindow) : base(name, content, true)
        {
            wnd = mainWindow;
            view = mainWindow.Page_Menu;

            view.Menu_Setup.Click += onMenuItemClick;
            view.Menu_Options.Click += onMenuItemClick;
            view.Menu_About.Click += onMenuItemClick;
        }

        private void onMenuItemClick(object sender, RoutedEventArgs e)
        {
            Button menuItem = (Button)sender;

            if (menuItem == view.Menu_Setup)
            {
                wnd.SetPage(consts.page_Setup);
            }
            else if (menuItem == view.Menu_Options)
            {
                wnd.SetPage(consts.page_Options);
            }
            else if (menuItem == view.Menu_About)
            {
                wnd.SetPage(consts.page_About);
            }
        }
    }
}
