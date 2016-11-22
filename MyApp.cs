using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;

namespace MetroSkinToolkit
{
    public static class MyApp
    {
        public static string Name { get { return Assembly.GetExecutingAssembly().GetName().Name; } }
        public static Version Version { get { return Assembly.GetExecutingAssembly().GetName().Version; } }
        public static string SmallVersion { get { return Version.smallVersion(); } }
        public static SteamSkin Engine = new SteamSkin();
        public static MainWindow MainWindow { get { return Application.Current.MainWindow as MainWindow; } }
        public static Components.HeaderBar Header { get { return MainWindow.Header; } }

        private static Dispatcher UI_Dispatcher = Application.Current.Dispatcher;

        public static void ExecuteOnUI(Action deleg)
        {
            if (deleg != null)
            {
                if (UI_Dispatcher.CheckAccess())
                    deleg();
                else
                    UI_Dispatcher.Invoke(deleg);
            }
        }

        public static string PrepPath(string path)
        {
            return path.Replace('/', Path.DirectorySeparatorChar);
        }

        public static class Constants
        {
            public const string page_Menu = "MENU";
            public const string page_Setup = "SETUP";
            public const string page_Settings = "SETTINGS_HOME";
            public const string page_Settings_AccentColor = "SETTING_ACCENT_COLOR";
            public const string page_Settings_FriendsList = "SETTING_FRIEND_LIST";
            public const string page_About = "ABOUT";
        }
    }
}