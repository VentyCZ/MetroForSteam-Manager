using System;
using System.Threading;
using System.Windows;
using System.Windows.Media;

namespace MetroSkinToolkit.Pages
{
    internal class Page_Setup : Page
    {
        Views.Setup view;

        public Page_Setup(string name, Views.Setup content) : base(name, content, false)
        {
            view = content;

            //Handlers
            view.UAC_Prompt.Click += onPromptClick;

            MyApp.Engine.SetupStepStarted += onControlStepStart;
            MyApp.Engine.DownloadProgress += onControlProgress;
            MyApp.Engine.DownloadCompleted += onControlComplete;
        }

        private void onPromptClick(object sender, RoutedEventArgs e)
        {
            view.UAC_Prompt.Visibility = Visibility.Collapsed;
            MyApp.Engine.UACNotify.Set();
        }

        private Thread th_testAnim;
        protected override void OnActivation()
        {
            view.UAC_Prompt.Visibility = Visibility.Collapsed;

            th_testAnim = view.Corty.TestAnim();

            //MyApp.Engine.StartSetup();
        }

        protected override void OnDeactivation()
        {
            //Stop everything here!
            th_testAnim?.Abort();
        }

        private void SetStatus(string txt)
        {
            MyApp.ExecuteOnUI(delegate ()
            {
                view.StateText.Text = "STATUS: " + txt.ToUpper();
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
            view.Corty.FillUp(e.ProgressPercentage);

            //DownloadProgress(e);
        }

        private void onControlComplete(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            Console.WriteLine((e.Error == null) ? "Download completed!" : "Download Error: " + e.Error.Message);

            //DownloadCompleted(e);
        }

        private void SetupStepStart(SteamSkin.SetupMilestones step)
        {
            if (step != SteamSkin.SetupMilestones.DownloadArchive && step != SteamSkin.SetupMilestones.ExtractArchive && step != SteamSkin.SetupMilestones.Completed)
            {
                view.Corty.StopPulseAnim();
                view.Corty.StartRotateAnim(1.25);
            }

            if (step == SteamSkin.SetupMilestones.DownloadArchive)
            {
                view.Corty.FillUp(0);
                view.Corty.SetInnerBrush(Colors.Transparent);
                SetStatus("Downloading archive");
            }
            else if (step == SteamSkin.SetupMilestones.ExtractArchive)
            {
                SetStatus("Extracting archive");
                view.Corty.StartPulseAnim();
            }
            else if (step == SteamSkin.SetupMilestones.InstallFont)
            {
                SetStatus("Installing font\nPress Yes on prompt.");
                view.UAC_Prompt.Visibility = Visibility.Visible;
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
                view.Corty.StopRotateAnim();
                view.Corty.StopPulseAnim();
                SetStatus("Setup Complete");
                view.Corty.SetInnerBrush((Color)ColorConverter.ConvertFromString("#FF28C92F"));
                MyApp.Header.ToggleBackButton(true);
            }
        }
    }
}