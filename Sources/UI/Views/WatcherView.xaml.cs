using System.Windows;
using System.Windows.Controls;

using FileManipulator.Models.Watcher;
using FileManipulator.ViewModels;

namespace FileManipulator.UI
{
    /// <summary>
    /// Logika interakcji dla klasy WatcherView.xaml
    /// </summary>
    public partial class WatcherView : UserControl
    {
        public WatcherView()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty WatcherProperty = DependencyProperty.Register(
            "Watcher",
            typeof(Watcher),
            typeof(WatcherView));

        public Watcher Watcher
        {
            get => GetValue(WatcherProperty) as Watcher;
            set => SetValue(WatcherProperty, value);
        }
    }
}
