using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using consts = MetroSkinToolkit.MyApp.Constants;

namespace MetroSkinToolkit
{
    public partial class MainWindow : Window
    {
        private SteamSkin control = new SteamSkin();

        public MainWindow()
        {
            InitializeComponent();

            Loaded += OnLoad;
            Closing += OnWindowClosing;
            SourceInitialized += OnSourceUpdate;
            SourceUpdated += OnSourceUpdate;

            Header.MouseLeftButtonDown += OhHandleMove;
            Button_Close.Click += OnClose;
            Button_Back.Click += OnBack;

            Menu_Setup.Click += OnMenuItemClick;
            Menu_Options.Click += OnMenuItemClick;
            Menu_About.Click += OnMenuItemClick;

            MenuSettings_AccentColor.Click += OnSettingsItemClick;
            MenuSettings_FriendList.Click += OnSettingsItemClick;

            UAC_Prompt.Click += OnPromptClick;

            control.InitComplete += Control_InitComplete;
            control.SetupStepStarted += Control_SetupStepStarted;
            control.DownloadProgress += Control_DownloadProgress;
            control.DownloadCompleted += Control_DownloadCompleted;
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            control.StopSetup();
        }

        private void OnPromptClick(object sender, RoutedEventArgs e)
        {
            UAC_Prompt.Visibility = Visibility.Collapsed;
            control.UACNotify.Set();
        }

        private void OnClose(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnBack(object sender, RoutedEventArgs e)
        {
            //Defaultly to MENU page
            SetPage(consts.page_Menu);

            //Turn the button off
            ToggleBackButton(false);
        }

        private void OnSettingsItemClick(object sender, RoutedEventArgs e)
        {
            RadioButton menuItem = sender as RadioButton;

            if (menuItem == MenuSettings_AccentColor) { SetPage_Settings(consts.page_Options_AccentColor); }
            else if (menuItem == MenuSettings_FriendList) { SetPage_Settings(consts.page_Options_FriendsList); }
        }

        private void OnMenuItemClick(object sender, RoutedEventArgs e)
        {
            Button menuItem = (Button)sender;

            if (menuItem == Menu_Setup)
            {
                SetPage(consts.page_Setup);
                UAC_Prompt.Visibility = Visibility.Collapsed;

                //Corty.TestAnim();
                control.StartSetup();
            }
            else if (menuItem == Menu_Options)
            {
                SetPage(consts.page_Options);
            }
            else if (menuItem == Menu_About)
            {
                SetPage(consts.page_About);
            }
        }

        private void OhHandleMove(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void OnSourceUpdate(object sender, EventArgs e)
        {
            this.Beautify();
        }

        private void ToggleBackButton(bool state)
        {
            MyApp.WorkOnUI(delegate ()
            {
                Button_Back.Visibility = (state ? Visibility.Visible : Visibility.Hidden);
            });
        }

        private void SetPage(string valPage)
        {
            MyApp.WorkOnUI(delegate ()
            {
                var pg = MyApp.Pages.SetActive(valPage);
                if (pg.Name == consts.page_About || pg.Name == consts.page_Options) { ToggleBackButton(true); }
                Console.WriteLine(pg != null ? "Page Set: " + pg.Name : "Non-Existent Page Error");
            });
        }

        private void SetPage_Settings(string valPage)
        {
            MyApp.WorkOnUI(delegate ()
            {
                var pg = MyApp.Pages_Settings.SetActive(valPage);
                Console.WriteLine(pg != null ? "Page Set: " + pg.Name : "Non-Existent Page Error");
            });
        }

        private void SetStatus(string txt)
        {
            MyApp.WorkOnUI(delegate ()
            {
                StateText.Text = "STATUS: " + txt.ToUpper();
            });
        }

        private void OnLoad(object sender, RoutedEventArgs e)
        {
            //Main UI
            Title = MyApp.Name;

            //About page
            About_Version.Content = MyApp.SmallVersion;
            About_ProgramName.Content = MyApp.Name;

            //Basic page setup
            MyApp.Pages.Create(consts.page_Menu, Page_Menu);
            MyApp.Pages.Create(consts.page_Setup, Page_Setup);
            MyApp.Pages.Create(consts.page_Options, Page_Options);
            MyApp.Pages.Create(consts.page_About, Page_About);

            MyApp.Pages_Settings.Create(consts.page_Options_AccentColor, Settings_Page_AccentColor);
            MyApp.Pages_Settings.Create(consts.page_Options_FriendsList, Settings_Page_FriendList);

            ToggleBackButton(false);
            SetPage(consts.page_Menu);
            SetPage_Settings(consts.page_Options_AccentColor);

            //Data initialization
            control.Init();
        }

        private void InitComplete()
        {
            MyApp.WorkOnUI(delegate ()
            {
                Match m = Regex.Match(control.LatestProgramVersion, @"^([0-9\.]+)$");

                if (m.Success)
                {
                    Version latest = new Version(m.Value);

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
            });
        }

        private void SetupStepStart(SteamSkin.SetupMilestones step)
        {
            MyApp.WorkOnUI(delegate ()
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
                    SetStatus("Downloading archive");
                }
                else if (step == SteamSkin.SetupMilestones.ExtractArchive)
                {
                    SetStatus("Extracting archive");
                    Corty.StartPulseAnim();
                }
                else if (step == SteamSkin.SetupMilestones.InstallFont)
                {
                    SetStatus("Installing font\nPress Yes on prompt.");
                    UAC_Prompt.Visibility = Visibility.Visible;
                }
                else if (step == SteamSkin.SetupMilestones.BackupStyle)
                {
                    SetStatus("Backing up");
                }
                else if (step == SteamSkin.SetupMilestones.DeleteExisting)
                {
                    SetStatus("Deleting existing files");
                }
                else if (step == SteamSkin.SetupMilestones.CopyNew)
                {
                    SetStatus("Copying new files");
                }
                else if (step == SteamSkin.SetupMilestones.RestoreStyle)
                {
                    SetStatus("Restoring");
                }
                else if (step == SteamSkin.SetupMilestones.Completed)
                {
                    Corty.StopRotateAnim();
                    Corty.StopPulseAnim();
                    SetStatus("Setup Complete");
                    Corty.SetInnerBrush((Color)ColorConverter.ConvertFromString("#FF28C92F"));
                    ToggleBackButton(true);
                }
            });
        }

        private void Control_InitComplete(object sender, EventArgs e)
        {
            Console.WriteLine((control.SteamInstalled) ? "Steam Found @ " + control.SteamDirectory : "Steam Not Installed");
            Console.WriteLine((control.SkinInstalled) ? "Installed MFS Version: " + control.SkinVersion : "Skin Is Not Installed");
            if (control.InfoObtained)
            {
                Console.WriteLine(string.Format("Latest Skin Version: {0} (installed: {1})", control.LatestSkinVersion, (control.LatestSkinVersionInstalled ? "yes" : "no")));
                Console.WriteLine("Latest {0} Version: " + control.LatestProgramVersion, MyApp.Name);
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