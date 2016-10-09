using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using System.IO.Compression;
using MetroSkinToolkit;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace MetroForSteamManager
{
    class SteamSkin
    {
        public event EventHandler InitComplete;
        public event EventHandler<SetupMilestones> SetupStepStarted;
        public event EventHandler<DownloadProgressChangedEventArgs> DownloadProgress;
        public event EventHandler<System.ComponentModel.AsyncCompletedEventArgs> DownloadCompleted;

        private string steamInstallationPath;
        private string steamSkinsDirectory { get { return pathCheck(steamInstallationPath + "/skins"); } }

        private string folderName_skin;
        private string folderName_font;
        private string fileName_font;

        private string pathDataDir { get { return pathCheck(steamSkinsDirectory + "/data"); } }
        private string pathSkinDownloadDir { get { return pathCheck(pathDataDir + "/download"); } }
        private string pathSkinDownloadFilename { get { return pathCheck(pathSkinDownloadDir + "/" + Path.GetFileName(infoSkinDownloadUrl)); } }
        private string pathSkinExtractDir { get { return pathCheck(pathDataDir + "/extract"); } }
        private string pathFontDir { get { return pathCheck(pathSkinExtractDir + "/" + folderName_font); } }
        private string pathFontFilename { get { return pathCheck(pathFontDir + "/" + fileName_font); } }
        private string pathFontInSys { get { return pathCheck(@"C:\Windows\Fonts\" + fileName_font); } }

        private string metroSkinVersion;
        private string metroSkinDirectory { get { return pathCheck(steamSkinsDirectory + "/" + folderName_skin); } }
        private string metroSkinMenuFile { get { return pathCheck(metroSkinDirectory + "/resource/menus/steam.menu"); } }
        private string metroCustomStylesFile { get { return pathCheck(metroSkinDirectory + "/custom.styles"); } }

        private string infoUrl = "http://data.spawnpoint.cz/info/MFS.xml";
        private string infoRawXml;
        private string infoSkinDownloadUrl;
        private string infoLatestSkin;
        private string infoLatestProgram;

        private string customStylesContent;

        private bool isLatestSkinIInstalled { get { return infoLatestSkin == metroSkinVersion; } }
        private AutoResetEvent SkinDownloadCompleted = new AutoResetEvent(false);
        public AutoResetEvent UACNotify = new AutoResetEvent(false);

        private bool InfoDownloadedSuccessfully { get { return infoRawXml != null; } }

        private Thread SetupThread;

        #region Hidden Fuctions
        /// <summary>
        /// Returns fixed path separators - replaces forward slash with the right one (Path.DirectorySeparatorChar), if needed
        /// </summary>
        /// <param name="path">Path to fix</param>
        /// <returns>Fixed path</returns>
        private string pathCheck(string path)
        {
            return path.Replace('/', Path.DirectorySeparatorChar);
        }

        private string getRegistryInfoAboutSteam(string key)
        {
            return (string)Registry.GetValue(string.Format(@"HKEY_LOCAL_MACHINE\SOFTWARE\{0}Valve\Steam", (Environment.Is64BitOperatingSystem ? @"Wow6432Node\" : string.Empty)), key, string.Empty);
        }

        private string getSkinVersionFromMenu()
        {
            if (File.Exists(metroSkinMenuFile))
            {
                string contents = File.ReadAllText(metroSkinMenuFile);

                Match m = Regex.Match(contents, @"\""Metro For Steam - ([0-9\.]+)\""");
                if (m.Success) { return m.Groups[1].Value; }
            }

            return string.Empty;
        }

        private void registryLookup()
        {
            steamInstallationPath = getRegistryInfoAboutSteam("InstallPath").TrimEnd(Path.DirectorySeparatorChar);
        }

        private void downloadInfo()
        {
            using (WebClient wc = new WebClient())
            {
                try
                {
                    infoRawXml = wc.DownloadString(infoUrl);
                    XmlDocument xml = new XmlDocument(); xml.LoadXml(infoRawXml);
                    infoSkinDownloadUrl = xml.SelectSingleNode("/info/skin/download").InnerText;
                    infoLatestSkin = xml.SelectSingleNode("/info/skin/version").InnerText;
                    infoLatestProgram = xml.SelectSingleNode("/info/installer/version").InnerText;
                }
                catch (Exception)
                {
                    infoRawXml = null;
                }
            }
        }

        private void backupStyle()
        {
            if (!File.Exists(metroCustomStylesFile)) { return; }
            customStylesContent = File.ReadAllText(metroCustomStylesFile);
        }

        private bool canRestoreStyle()
        {
            return (customStylesContent != null && customStylesContent.Trim() != string.Empty);
        }

        private void restoreStyle()
        {
            if (!canRestoreStyle()) { return; }

            if (!File.Exists(metroCustomStylesFile)) { File.Create(metroCustomStylesFile); }
            using (StreamWriter sr = new StreamWriter(metroCustomStylesFile, false))
            {
                sr.Write(customStylesContent);
            }
        }

        private Exception installFont()
        {
            try
            {
                //string winP = @"C:\Windows\Fonts\" + fileName_font;

                if (File.Exists(pathFontInSys)) { return null; }

                Process copyProc = new Process();
                string command = string.Format("/c copy \"{0}\" \"{1}\"", pathFontFilename, pathFontInSys);
                ProcessStartInfo copyNfo = new ProcessStartInfo("cmd", command);
                copyNfo.CreateNoWindow = true;
                copyNfo.Verb = "runas";
                copyProc.StartInfo = copyNfo;
                copyProc.Start();

                CWindows.Functions.AddFontResource(pathFontInSys);
                CWindows.Functions.SendMessage(CWindows.HWMD.HWND_BROADCAST, (uint)CWindows.WM.WM_FONTCHANGE, (IntPtr)0, (IntPtr)0);

                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        private void testUnregFont()
        {
            CWindows.Functions.RemoveFontResource(@"C:\Program Files (x86)\Steam\skins\data\extract\INSTALL THIS FONT (WINDOWS 7 AND OLDER)\segoeuisl.ttf");
        }

        private void extractSkin()
        {
            DirectoryInfo extractDir = new DirectoryInfo(pathSkinExtractDir);
            extractDir.DeleteContents();

            ZipFile.ExtractToDirectory(pathSkinDownloadFilename, pathSkinExtractDir);
        }

        private void downloadSkin()
        {
            using (WebClient wc = new WebClient())
            {
                wc.DownloadProgressChanged += Skin_DownloadProgress;
                wc.DownloadFileCompleted += Skin_DownloadComplete;
                Directory.CreateDirectory(pathSkinDownloadDir);
                wc.DownloadFileAsync(new Uri(infoSkinDownloadUrl), pathSkinDownloadFilename);
            }
        }

        private void Skin_DownloadProgress(object sender, DownloadProgressChangedEventArgs e)
        {
            if (DownloadProgress != null) { DownloadProgress.Invoke(this, e); }
        }

        private void Skin_DownloadComplete(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (DownloadCompleted != null) { DownloadCompleted.Invoke(this, e); }
            SkinDownloadCompleted.Set();
        }

        private void prepExtractDirNames()
        {
            var subDirs = new DirectoryInfo(pathSkinExtractDir).GetDirectories();

            foreach (var subDir in subDirs)
            {
                var fontFiles = subDir.GetFiles("*.ttf");
                var isFontDir = fontFiles.Length > 0;
                var isSkinDir = File.Exists(pathCheck(subDir.FullName + "/resource/styles/steam.styles"));

                if (isFontDir) { folderName_font = subDir.Name; fileName_font = fontFiles.FirstOrDefault().Name; }
                if (isSkinDir) { folderName_skin = subDir.Name; }
            }
        }

        private bool checkFontExistance()
        {
            return File.Exists(pathFontInSys);
        }

        private bool steamIsRunning()
        {
            var procs = Process.GetProcessesByName("Steam");
            bool isRunning = procs.Length > 0;

            return isRunning;
        }

        private void SignalizeSetupStepStart(SetupMilestones step)
        {
            if (SetupStepStarted != null) { SetupStepStarted.Invoke(this, step); }
        }
        #endregion

        #region Publics
        public enum SetupMilestones
        {
            DownloadArchive,
            ExtractArchive,
            InstallFont,
            BackupStyle,
            DeleteExisting,
            CopyNew,
            RestoreStyle,
            Completed
        }
        #endregion

        #region Public functions
        public void Init()
        {
            new Thread(delegate ()
            {
                registryLookup();
                metroSkinVersion = getSkinVersionFromMenu();
                downloadInfo();

                if (InitComplete != null) { InitComplete.Invoke(this, null); }
            }).Start();
        }


        public void StopSetup()
        {
            if (SetupThread != null) { SetupThread.Abort(); }
        }

        public void StartSetup()
        {
            SetupThread = new Thread(delegate ()
            {
                //Step 1) Skin Download
                SignalizeSetupStepStart(SetupMilestones.DownloadArchive);
                downloadSkin();
                SkinDownloadCompleted.WaitOne();

                //Step 2) Archive Extraction
                SignalizeSetupStepStart(SetupMilestones.ExtractArchive);
                extractSkin();
                prepExtractDirNames();

                //Step 3) Install Font - Segoe UI Semilight
                if (!checkFontExistance())
                {
                    SignalizeSetupStepStart(SetupMilestones.InstallFont);
                    UACNotify.WaitOne();
                    var fontInstallExc = installFont();
                }

                //Step 4) Backup Existing Skin Data - esp. custom skin style
                SignalizeSetupStepStart(SetupMilestones.BackupStyle);

                DirectoryInfo skinFolder = new DirectoryInfo(metroSkinDirectory);
                if (skinFolder.Exists)
                {
                    //Step 5) Delete Already Installed Skin Data
                    SignalizeSetupStepStart(SetupMilestones.DeleteExisting);

                    skinFolder.DeleteContents();
                }

                //Step 6) Copy Latest Skin Version Data
                SignalizeSetupStepStart(SetupMilestones.CopyNew);

                try
                {
                    Directory.Move(pathSkinExtractDir + @"\" + folderName_skin, metroSkinDirectory);
                }
                catch (Exception)
                {
                    Directory.Move(pathSkinExtractDir + @"\" + folderName_skin, metroSkinDirectory);
                }                

                if (canRestoreStyle())
                {
                    //Step 7) Restory Custom Skin Style
                    SignalizeSetupStepStart(SetupMilestones.RestoreStyle);

                    restoreStyle();
                }

                //Step 8) SIGNALIZE COMPLETION
                new DirectoryInfo(pathSkinExtractDir).DeleteContents();
                SignalizeSetupStepStart(SetupMilestones.Completed);

                SetupThread = null;
            });

            SetupThread.Start();
        }
        #endregion

        #region Public props
        public bool FontInstalled { get { return checkFontExistance(); } }

        public bool InfoObtained { get { return InfoDownloadedSuccessfully; } }

        public bool SteamInstalled { get { return steamInstallationPath != null; } }

        public bool SteamRunning { get { return steamIsRunning(); } }

        public string SteamDirectory { get { return steamInstallationPath; } }

        public bool SkinInstalled { get { return metroSkinVersion != null; } }

        public string SkinVersion { get { return metroSkinVersion; } }

        public string LatestSkinVersion { get { return infoLatestSkin; } }

        public string LatestProgramVersion { get { return infoLatestProgram; } }

        public bool LatestSkinVersionInstalled { get { return isLatestSkinIInstalled; } }
        #endregion
    }
}
