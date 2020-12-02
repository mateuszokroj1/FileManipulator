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
            e.Cancel = true;
            var model = DataContext as MainViewModel;

            model?.Close(() => Close());
        }
    }
}
