using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace MetroSkinToolkit
{
    public class Page
    {
        public string Name { get; private set; }
        public Grid Content { get; private set; }
        public PageCollection Container { get; private set; }

        public Page(string name, Grid content)
        {
            Name = name;
            Content = content;
        }

        public void SetContainer(PageCollection container)
        {
            Container = container;
            container.Add(this);
        }

        public void Hide()
        {
            if (Content != null)
                Content.Visibility = Visibility.Hidden;
        }

        public void Show()
        {
            if (Content != null)
                Content.Visibility = Visibility.Visible;
        }
    }

    public class PageCollection
    {
        private Dictionary<string, Page> pages;
        private Page activePage;

        public PageCollection()
        {
            pages = new Dictionary<string, Page>();
        }

        public Page Create(string name, Grid content)
        {
            return Add(new Page(name, content));
        }

        public Page Add(Page pg)
        {
            pages[pg.Name] = pg;
            pg.Hide();
            return pg;
        }

        public Page SetActive(Page pg)
        {
            if (pg != null)
            {
                if (activePage != null)
                    activePage.Hide();

                activePage = pg;
                activePage.Show();

                return activePage;
            }

            return null;
        }

        public Page SetActive(string pgname)
        {
            if (pages.ContainsKey(pgname))
                return SetActive(pages[pgname]);

            return null;
        }
    }
}
