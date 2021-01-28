using System.Windows;
using System.Windows.Input;

using FileManipulator.ViewModels;

namespace FileManipulator.UI
{
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            SetValue(CloseCommandProperty, new Command(() => Close()));

            var model = DataContext as MainViewModel;

            model.RenameDialog = new RenameWindow();

            model.MessageOnCloseWhileTaskWorking = () =>
                MessageBox.Show("Zadanie jest w stanie pracującym.\n\rCzy na pewno chcesz je zamknąć?\n\rNiektóre zmiany nie mogą być cofnięte.", "File Manipulator", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No)
                == MessageBoxResult.Yes;
        }

        public static readonly DependencyProperty CloseCommandProperty = DependencyProperty.Register(
            "CloseCommand",
            typeof(ICommand),
            typeof(MainView));

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
