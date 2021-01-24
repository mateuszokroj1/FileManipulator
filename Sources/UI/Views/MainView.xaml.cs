using System.Windows;
using System.Windows.Input;

namespace FileManipulator.UI
{
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            SetValue(CloseCommandProperty, new Command(() => Close()));
        }

        public static readonly DependencyPropertyKey CloseCommandProperty = DependencyProperty.RegisterReadOnly(
            "CloseCommand",
            typeof(ICommand),
            typeof(MainView), new PropertyMetadata());

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
