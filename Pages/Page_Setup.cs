using System;
using System.Windows;
using System.Windows.Media;

namespace MetroSkinToolkit.Pages
{
    internal class Page_Setup : Page
    {
        private MainWindow wnd;

        public Page_Setup(string name, FrameworkElement content, MainWindow mainWindow) : base(name, content, false)
        {
            wnd = mainWindow;

            //Handlers
            wnd.UAC_Prompt.Click += onPromptClick;

            MyApp.Engine.SetupStepStarted += onControlStepStart;
            MyApp.Engine.DownloadProgress += onControlProgress;
            MyApp.Engine.DownloadCompleted += onControlComplete;
        }

        protected override void OnActivation()
        {
            Console.WriteLine("Got activated!!! :D");

            wnd.UAC_Prompt.Visibility = Visibility.Collapsed;

            //Corty.TestAnim();
            //MyApp.Engine.StartSetup();
        }

        private void onPromptClick(object sender, RoutedEventArgs e)
        {
            wnd.UAC_Prompt.Visibility = Visibility.Collapsed;
            MyApp.Engine.UACNotify.Set();
        }

        private void SetStatus(string txt)
        {
            MyApp.ExecuteOnUI(delegate ()
            {
                wnd.StateText.Text = "STATUS: " + txt.ToUpper();
            });
        }

        private void onControlStepStart(object sender, SteamSkin.SetupMilestones e)
        {
            Console.WriteLine("Setup step start: " + e.ToString());

            SetupStepStart(e);
        }

        private void onControlProgress(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            Console.WriteLine(string.Format("Downloading... {3}% {1}/{2} MBs", string.Empty, e.BytesReceived.toMBytes(), e.TotalBytesToReceive.toMBytes(), e.ProgressPercentage));
            wnd.Corty.FillUp(e.ProgressPercentage);

            //DownloadProgress(e);
        }

        private void onControlComplete(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            Console.WriteLine((e.Error == null) ? "Download completed!" : "Download Error: " + e.Error.Message);

            //DownloadCompleted(e);
        }

        private void SetupStepStart(SteamSkin.SetupMilestones step)
        {
            MyApp.ExecuteOnUI(delegate ()
            {
                if (step != SteamSkin.SetupMilestones.DownloadArchive && step != SteamSkin.SetupMilestones.ExtractArchive && step != SteamSkin.SetupMilestones.Completed)
                {
                    wnd.Corty.StopPulseAnim();
                    wnd.Corty.StartRotateAnim(1.25);
                }

                if (step == SteamSkin.SetupMilestones.DownloadArchive)
                {
                    wnd.Corty.FillUp(0);
                    wnd.Corty.SetInnerBrush(Colors.Transparent);
                    SetStatus("Downloading archive");
                }
                else if (step == SteamSkin.SetupMilestones.ExtractArchive)
                {
                    SetStatus("Extracting archive");
                    wnd.Corty.StartPulseAnim();
                }
                else if (step == SteamSkin.SetupMilestones.InstallFont)
                {
                    SetStatus("Installing font\nPress Yes on prompt.");
                    wnd.UAC_Prompt.Visibility = Visibility.Visible;
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
                    wnd.Corty.StopRotateAnim();
                    wnd.Corty.StopPulseAnim();
                    SetStatus("Setup Complete");
                    wnd.Corty.SetInnerBrush((Color)ColorConverter.ConvertFromString("#FF28C92F"));
                    wnd.Header.ToggleBackButton(true);
                }
            });
        }
    }
}