using System.Windows;
using System.Windows.Controls;
using consts = MetroSkinToolkit.MyApp.Constants;

namespace MetroSkinToolkit.Pages
{
    class Page_Menu : Page
    {
        Views.Menu view;

        public Page_Menu(string name, Views.Menu content) : base(name, content, true)
        {
            view = content;

            view.Menu_Setup.Click += onMenuItemClick;
            view.Menu_Settings.Click += onMenuItemClick;
            view.Menu_About.Click += onMenuItemClick;
        }

        private void onMenuItemClick(object sender, RoutedEventArgs e)
        {
            Button menuItem = (Button)sender;

            if (menuItem == view.Menu_Setup)
            {
                MyApp.MainWindow.SetPage(consts.page_Setup);
            }
            else if (menuItem == view.Menu_Settings)
            {
                MyApp.MainWindow.SetPage(consts.page_Settings);
            }
            else if (menuItem == view.Menu_About)
            {
                MyApp.MainWindow.SetPage(consts.page_About);
            }
        }
    }
}
