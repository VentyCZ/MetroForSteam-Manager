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

        public readonly static PageCollection Pages = new PageCollection();
        public readonly static PageCollection Pages_Settings = new PageCollection();

        private static Dispatcher UI_Dispatcher = Application.Current.Dispatcher;
        public static void WorkOnUI(Action deleg)
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
            public const string page_Options = "OPTIONS";
            public const string page_Options_AccentColor = "SETTING_ACCENT_COLOR";
            public const string page_Options_FriendsList = "SETTING_FRIEND_LIST";
            public const string page_About = "ABOUT";
        }
    }
}