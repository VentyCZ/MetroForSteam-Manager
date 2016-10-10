using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MetroSkinToolkit
{
    public static class MyApp
    {
        private static Page activePage = null;
        private static Page activeSettingsPage = null;
        private static Dictionary<string, Page> Pages = new Dictionary<string, Page>();
        private static Dictionary<string, Page> Pages_Settings = new Dictionary<string, Page>();

        public static Page AddPage(string name, Grid content)
        {
            return new Page(name, content, Pages);
        }

        public static Page AddSettingsPage(string name, Grid content)
        {
            return new Page(name, content, Pages_Settings);
        }

        public static string Name { get { return Assembly.GetExecutingAssembly().GetName().Name; } }
        public static Version Version { get { return Assembly.GetExecutingAssembly().GetName().Version; } }
        public static string SmallVersion { get { return Version.smallVersion(); } }

        public class Page
        {
            public string Name { get; private set; }
            public Grid Content { get; private set; }

            public Page(string name, Grid content, Dictionary<string, Page> container)
            {
                Name = name;
                Content = content;

                container.Add(name, this);
            }


        }
    }
}
