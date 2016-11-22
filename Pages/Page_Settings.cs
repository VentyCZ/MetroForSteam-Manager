using System;
using System.Windows;
using System.Windows.Controls;
using consts = MetroSkinToolkit.MyApp.Constants;

namespace MetroSkinToolkit.Pages
{
    class Page_Settings : Page
    {
        Views.Settings view;

        public Page_Settings(string name, FrameworkElement content, MainWindow mainWindow) : base(name, content, false)
        {
            view = mainWindow.Page_Settings;

            view.MenuSettings_AccentColor.Click += onSettingsMenuItemClick;
            view.MenuSettings_FriendList.Click += onSettingsMenuItemClick;

            SubPages.Create(consts.page_Settings_AccentColor, view.Settings_Page_AccentColor, true);
            SubPages.Create(consts.page_Settings_FriendsList, view.Settings_Page_FriendList);
        }

        private void onSettingsMenuItemClick(object sender, RoutedEventArgs e)
        {
            RadioButton menuItem = sender as RadioButton;

            if (menuItem == view.MenuSettings_AccentColor)
            {
                SubPages.Get(consts.page_Settings_AccentColor)?.Activate();
            }
            else if (menuItem == view.MenuSettings_FriendList)
            {
                SubPages.Get(consts.page_Settings_FriendsList)?.Activate();
            }
            else
            {
                Console.WriteLine("UNHANDLED MENU ITEM!");
            }
        }
    }
}
