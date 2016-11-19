using System;
using System.Windows;
using System.Windows.Controls;
using consts = MetroSkinToolkit.MyApp.Constants;

namespace MetroSkinToolkit.Pages
{
    class Page_Settings : Page
    {
        MainWindow wnd;

        public Page_Settings(string name, FrameworkElement content, MainWindow mainWindow) : base(name, content, false)
        {
            wnd = mainWindow;

            wnd.MenuSettings_AccentColor.Click += onSettingsMenuItemClick;
            wnd.MenuSettings_FriendList.Click += onSettingsMenuItemClick;

            SubPages.Create(consts.page_Options_AccentColor, wnd.Settings_Page_AccentColor, true);
            SubPages.Create(consts.page_Options_FriendsList, wnd.Settings_Page_FriendList);
        }

        private void onSettingsMenuItemClick(object sender, RoutedEventArgs e)
        {
            RadioButton menuItem = sender as RadioButton;

            if (menuItem == wnd.MenuSettings_AccentColor)
            {
                SubPages.Get(consts.page_Options_AccentColor)?.Activate();
            }
            else if (menuItem == wnd.MenuSettings_FriendList)
            {
                SubPages.Get(consts.page_Options_FriendsList)?.Activate();
            }
            else
            {
                Console.WriteLine("UNHANDLED MENU ITEM!");
            }
        }
    }
}
