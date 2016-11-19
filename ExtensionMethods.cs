using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Shell = System.Windows.Shell;

namespace MetroSkinToolkit
{
    public static class ExtensionMethods
    {
        public static string smallVersion(this Version ver, int minimum = 2)
        {
            string[] parts = ver.ToString().Split('.');
            List<string> retVer = new List<string>();

            bool foundFirstReal = false;
            for (int x = parts.Length - 1; x >= 0; x--)
            {
                //Skip if: is not mid 0 or end 0 & this is not the One
                if (!foundFirstReal && parts[x] == "0" && !(x + 1 == minimum))
                    continue;

                foundFirstReal = true;
                
                retVer.Add(parts[x]);
            }

            retVer.Reverse();
            return string.Join(".", retVer);
        }

        public static double toKBytes(this long val, int decPlaces = 2)
        {
            return Math.Round(val / Math.Pow(1000, 1), decPlaces);
        }

        public static double toMBytes(this long val, int decPlaces = 2)
        {
            return Math.Round(val / Math.Pow(1000, 2), decPlaces);
        }

        public static void DeleteContents(this DirectoryInfo dir)
        {
            if (dir.Exists)
            {
                foreach (var directory in dir.GetDirectories())
                {
                    directory.DeleteContents();
                }

                try
                {
                    dir.Delete(true);
                }
                catch (IOException)
                {
                    dir.Delete(true);
                }
                catch (UnauthorizedAccessException)
                {
                    dir.Delete(true);
                }

            }

            if (!dir.Exists) { dir.Create(); }
        }

        public static void Beautify(this Window wnd)
        {
            wnd.WindowStyle = WindowStyle.SingleBorderWindow;

            Shell.WindowChrome.SetWindowChrome(wnd, new Shell.WindowChrome
            {
                ResizeBorderThickness = new Thickness(0),
                GlassFrameThickness = new Thickness(-1),
                UseAeroCaptionButtons = false,
                CaptionHeight = 0,
                CornerRadius = new CornerRadius(0),
                NonClientFrameEdges = Shell.NonClientFrameEdges.None
            });
        }
    }
}
