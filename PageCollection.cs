using System;
using System.Collections.Generic;
using System.Windows;

namespace MetroSkinToolkit
{
    public class Page
    {
        public string Name { get; private set; }
        public bool IsDefault { get; private set; }
        public FrameworkElement Content { get; private set; }
        public PageCollection Container { get; private set; }
        public PageCollection SubPages { get; private set; }

        private void setVisible(bool visible)
        {
            if (Content != null)
                Content.Dispatcher.Invoke(delegate ()
                {
                    Content.Visibility = (visible ? Visibility.Visible : Visibility.Hidden);
                });
        }

        public Page(string name, FrameworkElement content, bool isDefault = false)
        {
            Name = name;
            IsDefault = isDefault;
            Content = content;
            SubPages = new PageCollection();
        }

        protected internal void SetContainer(PageCollection container)
        {
            Container = container;
        }

        public void Hide(bool trigger = true)
        {
            setVisible(false);

            if (trigger)
                OnDeactivation();
        }

        public void Show(bool trigger = true)
        {
            setVisible(true);

            if (trigger)
                OnActivation();
        }

        public void Activate()
        {
            if (Container != null)
                Container.SetActive(this);
        }

        protected virtual void OnActivation() { }
        protected virtual void OnDeactivation() { }
    }

    public class PageCollection
    {
        private Dictionary<string, Page> pages;
        public Page ActivePage { get; private set; }
        public Page DefaultPage { get; private set; }

        public PageCollection()
        {
            pages = new Dictionary<string, Page>();
        }

        public Page Get(string name)
        {
            return (pages.ContainsKey(name) ? pages[name] : null);
        }

        public Page Create(string name, FrameworkElement content, bool isDefault = false)
        {
            return Add(new Page(name, content, isDefault));
        }

        public Page Add(Page pg)
        {
            if (pg.Container != this)
            {
                //NOTE: Should remove it from the other container tho

                pages[pg.Name] = pg;
                pg.SetContainer(this);
            }

            if (!pg.IsDefault)
                pg.Hide(false);

            if (pg.IsDefault)
            {
                DefaultPage = pg;
                pg.Activate();
            }

            return pg;
        }

        public bool SetActive(Page pg)
        {
            if (pg != null && pg.Container == this)
            {
                if (ActivePage != null)
                    ActivePage.Hide();

                ActivePage = pg;
                ActivePage.Show();

                return true;
            }

            return false;
        }

        public bool Go(string pgname)
        {
            return SetActive(Get(pgname));
        }
    }
}