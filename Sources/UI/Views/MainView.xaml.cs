using System.Windows;

namespace FileManipulator.UI
{
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var model = DataContext as MainViewModel;

            model?.Close(() =>
            {
                return MessageBox.Show(
                    Messages.Komunikat_przy_zamykaniu_aplikacji,
                    "File Manipulator",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning,
                    MessageBoxResult.No) == MessageBoxResult.Yes;
            },
            e);
        }
    }
}
