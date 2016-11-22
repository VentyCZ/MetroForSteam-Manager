using System.Windows;

namespace MetroSkinToolkit.Pages
{
    class Page_About : Page
    {
        private Views.About view;

        public Page_About(string name, FrameworkElement content, MainWindow mainWindow) : base(name, content, false)
        {
            view = mainWindow.Page_About;
            
            view.About_Version.Content = MyApp.SmallVersion;
            view.About_ProgramName.Content = MyApp.Name;
        }
    }
}
