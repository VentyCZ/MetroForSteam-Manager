using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MetroSkinToolkit
{
    public partial class MainWindow : Window
    {
        private SteamSkin control = new SteamSkin();

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            SourceInitialized += UI_Source;
            SourceUpdated += UI_Source;

            Header.MouseLeftButtonDown += UI_HeaderHandle;
            Button_Close.Click += UI_CloseClick;
            Button_Back.Click += UI_BackClick;

            Menu_Setup.Click += UI_MenuClick;
            Menu_Options.Click += UI_MenuClick;
            Menu_About.Click += UI_MenuClick;

            MenuSettings_AccentColor.Click += UI_Settings_MenuCLick;
            MenuSettings_FriendList.Click += UI_Settings_MenuCLick;

            UAC_Prompt.Click += UAC_Prompt_Click;

            control.InitComplete += Control_InitComplete;
            control.SetupStepStarted += Control_SetupStepStarted;
            control.DownloadProgress += Control_DownloadProgress;
            control.DownloadCompleted += Control_DownloadCompleted;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            control.StopSetup();
        }

        private Dictionary<string, Grid> Pages = new Dictionary<string, Grid>();
        private Dictionary<string, Grid> Pages_Settings = new Dictionary<string, Grid>();

        const string pageName_Menu = "MENU";
        const string pageName_Setup = "SETUP";
        const string pageName_Options = "OPTIONS";
        const string pageName_About = "ABOUT";
        private string activePageName = pageName_Menu;

        const string pageName_Settings_AccentColor = "SETTING_ACCENT_COLOR";
        const string pageName_Settings_FriendsList = "SETTING_FRIEND_LIST";
        private string activePageName_Settings = pageName_Settings_AccentColor;

        private string programName { get { return Assembly.GetExecutingAssembly().GetName().Name; } }
        private Version programVersion { get { return Assembly.GetExecutingAssembly().GetName().Version; } }
        private string theVersion { get { return programVersion.smallVersion(); } }

        private void UAC_Prompt_Click(object sender, RoutedEventArgs e)
        {
            UAC_Prompt.Visibility = Visibility.Collapsed;
            control.UACNotify.Set();
        }

        private void UI_CloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void UI_BackClick(object sender, RoutedEventArgs e)
        {
            if (activePageName == "")
            {

            }
            else
            {
                //Defaultly to MENU page
                UI_SetPage(pageName_Menu);
            }

            //Turn the button off
            UI_ToggleBackButton(false);
        }

        private void UI_ToggleBackButton(bool state)
        {
            if (Dispatcher.CheckAccess())
            {
                Button_Back.Visibility = (state ? Visibility.Visible : Visibility.Hidden);
            }
            else
            {
                Dispatcher.Invoke(delegate () { UI_ToggleBackButton(state); });
            }
        }

        private void UI_Settings_MenuCLick(object sender, RoutedEventArgs e)
        {
            RadioButton menuItem = sender as RadioButton;

            if (menuItem == MenuSettings_AccentColor) { UI_SetPage_Settings(pageName_Settings_AccentColor); }
            else if (menuItem == MenuSettings_FriendList) { UI_SetPage_Settings(pageName_Settings_FriendsList); }
        }

        private void UI_MenuClick(object sender, RoutedEventArgs e)
        {
            Button menuItem = (Button)sender;

            if (menuItem == Menu_Setup)
            {
                UI_SetPage(pageName_Setup);
                UAC_Prompt.Visibility = Visibility.Collapsed;

                //Corty.TestAnim();
                control.StartSetup();
            }
            else if (menuItem == Menu_Options)
            {
                UI_SetPage(pageName_Options);
            }
            else if (menuItem == Menu_About)
            {
                UI_SetPage(pageName_About);
            }
        }

        private void UI_HeaderHandle(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void UI_Source(object sender, EventArgs e)
        {
            this.Beautify();
        }

        private void UI_SetPage(string valPage)
        {
            if (Dispatcher.CheckAccess())
            {
                if (!Pages.ContainsKey(valPage)) { Console.WriteLine("Non-Existent Page Error"); return; }
                activePageName = valPage;

                foreach (var page in Pages)
                {
                    string pageName = page.Key;
                    Grid pageContents = page.Value;

                    pageContents.Visibility = ((pageName == valPage) ? Visibility.Visible : Visibility.Collapsed);
                }

                if (valPage == pageName_About || valPage == pageName_Options) { UI_ToggleBackButton(true); }

                Console.WriteLine("Page Set: " + valPage);
            }
            else
            {
                Dispatcher.Invoke(delegate () { UI_SetPage(valPage); });
            }
        }

        private void UI_SetPage_Settings(string valPage)
        {
            if (Dispatcher.CheckAccess())
            {
                if (!Pages_Settings.ContainsKey(valPage)) { Console.WriteLine("Non-Existent Page Error"); return; }
                activePageName_Settings = valPage;

                foreach (var page in Pages_Settings)
                {
                    string pageName = page.Key;
                    Grid pageContents = page.Value;

                    pageContents.Visibility = ((pageName == valPage) ? Visibility.Visible : Visibility.Collapsed);
                }

                Console.WriteLine("Page Set: " + valPage);
            }
            else
            {
                Dispatcher.Invoke(delegate () { UI_SetPage_Settings(valPage); });
            }
        }

        private void UI_SetStatus(string txt)
        {
            if (Dispatcher.CheckAccess())
            {
                txt = txt.ToUpper();

                StateText.Text = "STATUS: " + txt;
            }
            else
            {
                Dispatcher.Invoke(delegate () { UI_SetStatus(txt); });
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //Main UI
            Title = programName;

            //About page
            About_Version.Content = theVersion;
            About_ProgramName.Content = programName;

            //Basic page setup
            Pages.Add(pageName_Menu, Page_Menu);
            Pages.Add(pageName_Setup, Page_Setup);
            Pages.Add(pageName_Options, Page_Options);
            Pages.Add(pageName_About, Page_About);

            Pages_Settings.Add(pageName_Settings_AccentColor, Settings_Page_AccentColor);
            Pages_Settings.Add(pageName_Settings_FriendsList, Settings_Page_FriendList);

            UI_ToggleBackButton(false);
            UI_SetPage(pageName_Menu);
            UI_SetPage_Settings(pageName_Settings_AccentColor);

            //Data initialization
            control.Init();
        }

        private void InitComplete()
        {
            if (Dispatcher.CheckAccess())
            {
                Match m = Regex.Match(control.LatestProgramVersion, @"^([0-9\.]+)$");

                if (m.Success)
                {
                    Version latest = new Version(m.Value);

                    if (latest > programVersion)
                    {
                        Console.WriteLine("Better upgrade, son!");
                    }
                    else if (latest < programVersion)
                    {
                        Console.WriteLine("Using BETA, huh ?");
                    }
                    else if (latest == programVersion)
                    {
                        Console.WriteLine("You're good to go! :>");
                    }
                }
            }
            else
            {
                Dispatcher.Invoke(delegate () { InitComplete(); });
            }
        }

        private void SetupStepStart(SteamSkin.SetupMilestones step)
        {
            if (Dispatcher.CheckAccess())
            {
                if (step != SteamSkin.SetupMilestones.DownloadArchive && step != SteamSkin.SetupMilestones.ExtractArchive && step != SteamSkin.SetupMilestones.Completed)
                {
                    Corty.StopPulseAnim();
                    Corty.StartRotateAnim(1.25);
                }

                if (step == SteamSkin.SetupMilestones.DownloadArchive)
                {
                    Corty.FillUp(0);
                    Corty.SetInnerBrush(Colors.Transparent);
                    UI_SetStatus("Downloading archive");
                }
                else if (step == SteamSkin.SetupMilestones.ExtractArchive)
                {
                    UI_SetStatus("Extracting archive");
                    Corty.StartPulseAnim();
                }
                else if (step == SteamSkin.SetupMilestones.InstallFont)
                {
                    UI_SetStatus("Installing font\nPress Yes on prompt.");
                    UAC_Prompt.Visibility = Visibility.Visible;
                }
                else if (step == SteamSkin.SetupMilestones.BackupStyle)
                {
                    UI_SetStatus("Backing up");
                }
                else if (step == SteamSkin.SetupMilestones.DeleteExisting)
                {
                    UI_SetStatus("Deleting existing files");
                }
                else if (step == SteamSkin.SetupMilestones.CopyNew)
                {
                    UI_SetStatus("Copying new files");
                }
                else if (step == SteamSkin.SetupMilestones.RestoreStyle)
                {
                    UI_SetStatus("Restoring");
                }
                else if (step == SteamSkin.SetupMilestones.Completed)
                {
                    Corty.StopRotateAnim();
                    Corty.StopPulseAnim();
                    UI_SetStatus("Setup Complete");
                    Corty.SetInnerBrush((Color)ColorConverter.ConvertFromString("#FF28C92F"));
                    UI_ToggleBackButton(true);
                }
            }
            else
            {
                Dispatcher.Invoke(delegate () { SetupStepStart(step); });
            }
        }

        private void Control_InitComplete(object sender, EventArgs e)
        {
            Console.WriteLine((control.SteamInstalled) ? "Steam Found @ " + control.SteamDirectory : "Steam Not Installed");
            Console.WriteLine((control.SkinInstalled) ? "Installed MFS Version: " + control.SkinVersion : "Skin Is Not Installed");
            if (control.InfoObtained)
            {
                Console.WriteLine(string.Format("Latest Skin Version: {0} (installed: {1})", control.LatestSkinVersion, (control.LatestSkinVersionInstalled ? "yes" : "no")));
                Console.WriteLine("Latest {0} Version: " + control.LatestProgramVersion, programName);
            }
            else
            {
                Console.WriteLine("Latest info could not be obtained!");
            }

            InitComplete();
        }

        private void Control_SetupStepStarted(object sender, SteamSkin.SetupMilestones e)
        {
            Console.WriteLine("Setup step start: " + e.ToString());

            SetupStepStart(e);
        }

        private void Control_DownloadProgress(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            Console.WriteLine(string.Format("Downloading... {3}% {1}/{2} MBs", string.Empty, e.BytesReceived.toMBytes(), e.TotalBytesToReceive.toMBytes(), e.ProgressPercentage));
            Corty.FillUp(e.ProgressPercentage);

            //DownloadProgress(e);
        }

        private void Control_DownloadCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            Console.WriteLine((e.Error == null) ? "Download completed!" : "Download Error: " + e.Error.Message);

            //DownloadCompleted(e);
        }
    }
}
