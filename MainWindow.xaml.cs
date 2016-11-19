using MetroSkinToolkit.Pages;
using System;
using System.Windows;
using consts = MetroSkinToolkit.MyApp.Constants;

namespace MetroSkinToolkit
{
    public partial class MainWindow : TabWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += onLoad;
            Closing += onWindowClosing;

            MyApp.Engine.InitComplete += onControlInitialized;
        }

        private void onWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //TODO: Ask if user wants to cancel the setup

            MyApp.Engine.StopSetup();
        }

        private void onLoad(object sender, RoutedEventArgs e)
        {
            //Window title - Taskbar only, displayed title is controlled by Header
            Title = MyApp.Name;

            //Basic page setup
            AddPage(new Page_Menu(consts.page_Menu, Page_Menu, this));
            AddPage(new Page_Setup(consts.page_Setup, Page_Setup, this));
            AddPage(new Page_Settings(consts.page_Options, Page_Options, this));
            AddPage(new Page_About(consts.page_About, Page_About, this));

            //Connect the header - Sets the displayed title, among other things
            Connect(Header);

            //Starts up the update check and stuff
            MyApp.Engine.Init();
        }

        private void onControlInitialized(object sender, EventArgs e)
        {
            Console.WriteLine(MyApp.Engine.SteamInstalled ? "Steam Found @ " + MyApp.Engine.SteamDirectory : "Steam Not Installed");
            Console.WriteLine(MyApp.Engine.SkinInstalled ? "Installed MFS Version: " + MyApp.Engine.SkinVersion : "Skin Is Not Installed");

            if (MyApp.Engine.InfoObtained)
            {
                Console.WriteLine(string.Format("Latest Skin Version: {0} (installed: {1})", MyApp.Engine.LatestSkinVersion, (MyApp.Engine.LatestSkinVersionInstalled ? "yes" : "no")));
                Console.WriteLine("Latest {0} Version: " + MyApp.Engine.LatestProgramVersion, MyApp.Name);
            }
            else
            {
                Console.WriteLine("Latest info could not be obtained!");
            }

            Version latest;
            if (Version.TryParse(MyApp.Engine.LatestProgramVersion, out latest))
            {
                if (latest > MyApp.Version)
                {
                    Console.WriteLine("Better upgrade, son!");
                }
                else if (latest < MyApp.Version)
                {
                    Console.WriteLine("Using BETA, huh ?");
                }
                else if (latest == MyApp.Version)
                {
                    Console.WriteLine("You're good to go! :>");
                }
            }
        }
    }
}