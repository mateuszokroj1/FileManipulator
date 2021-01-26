using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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

            var model = DataContext as WatcherViewModel;

            var binding = new Binding("Watcher");
            binding.Source = model;
            binding.Mode = BindingMode.OneWayToSource;
            SetBinding(WatcherProperty, binding);

            model.OnInvalidInput = () => MessageBox.Show("Podano niepoprawną ścieżkę.", "Watcher", MessageBoxButton.OK, MessageBoxImage.Warning);
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
