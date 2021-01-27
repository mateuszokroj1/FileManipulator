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
        }

        private void Root_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(DataContext is WatcherViewModel model)
                model.OnInvalidInput = () => MessageBox.Show("Podano niepoprawną ścieżkę.", "Watcher", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
