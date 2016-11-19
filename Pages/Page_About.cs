using System.Windows;

namespace MetroSkinToolkit.Pages
{
    class Page_About : Page
    {
        private MainWindow wnd;

        public Page_About(string name, FrameworkElement content, MainWindow mainWindow) : base(name, content, false)
        {
            wnd = mainWindow;

            wnd.About_Version.Content = MyApp.SmallVersion;
            wnd.About_ProgramName.Content = MyApp.Name;
        }
    }
}
