using MetroSkinToolkit.Components;
using System;
using System.Windows;

namespace MetroSkinToolkit
{
    public class TabWindow : Window
    {
        private HeaderBar header = null;

        private readonly PageCollection tabs = new PageCollection();

        public TabWindow()
        {
            SourceInitialized += onSourceUpdate;
            SourceUpdated += onSourceUpdate;
        }

        private void onSourceUpdate(object sender, EventArgs e)
        {
            this.Beautify();
        }

        protected void Connect(HeaderBar header)
        {
            this.header = header;

            header.Connect(this);
        }

        protected Page AddPage(string name, FrameworkElement contents, bool isDefault = false)
        {
            return AddPage(tabs.Create(name, contents, isDefault));
        }

        protected Page AddPage(Page tab)
        {
            return tabs.Add(tab);
        }

        public void SetPage(string valPage)
        {
            SetPage(tabs.Get(valPage));
        }

        protected void SetPage(Page pg)
        {
            if (pg == null)
                return;

            pg.Activate();

            if (header != null && !pg.IsDefault)
                header.ToggleBackButton(true);

            Console.WriteLine(pg != null ? "Page Set: " + pg.Name : "Non-Existent Page Error");
        }

        public void GoBack()
        {
            tabs.DefaultPage?.Activate();
            header.ToggleBackButton(false);
        }
    }
}