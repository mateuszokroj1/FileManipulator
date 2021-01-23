using System.Windows;

namespace FileManipulator.UI
{
    /// <summary>
    /// Logika interakcji dla klasy RenameWindow.xaml
    /// </summary>
    public partial class RenameWindow : Window
    {
        public RenameWindow()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(RenameWindow));

        public string Value
        {
            get => GetValue(ValueProperty) as string;
            set => SetValue(ValueProperty, value);
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrWhiteSpace(Value))
            {
                MessageBox.Show("Nazwa musi zawierać jakiś znak widzialny.", "Błędna wartość", MessageBoxButton.OK, MessageBoxImage.Warning);
                this.Text.Focus();
            }
        }
    }
}
