using System.Windows;
using System.Windows.Controls;

namespace MetroSkinToolkit.Components
{
    public partial class MainMenuItem : Button
    {
        public static readonly DependencyProperty HeadlineProperty = DependencyProperty.Register("Headline", typeof(string), typeof(MainMenuItem), new FrameworkPropertyMetadata("Headline text"));
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(MainMenuItem), new FrameworkPropertyMetadata("Description text"));

        public string Headline
        {
            get
            {
                return (string)GetValue(HeadlineProperty);
            }
            set
            {
                SetValue(HeadlineProperty, value);
            }
        }

        public string Description
        {
            get
            {
                return (string)GetValue(DescriptionProperty);
            }
            set
            {
                SetValue(DescriptionProperty, value);
            }
        }

        public MainMenuItem()
        {
            InitializeComponent();
        }
    }
}
