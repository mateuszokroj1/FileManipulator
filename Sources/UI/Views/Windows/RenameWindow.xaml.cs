using System.Windows;
using System.Windows.Input;

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
            if(string.IsNullOrWhiteSpace(Text.Text))
            {
                MessageBox.Show("Nazwa musi zawierać jakiś znak widzialny.", "Błędna wartość", MessageBoxButton.OK, MessageBoxImage.Warning);
                this.Text.Focus();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Text.Text))
            {
                MessageBox.Show("Nazwa musi zawierać jakiś znak widzialny.", "Błędna wartość", MessageBoxButton.OK, MessageBoxImage.Warning);
                this.Text.Focus();
                return;
            }

            Value = this.Text.Text;
            DialogResult = true;
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DialogResult = false;
                Close();
            }
            else if (e.Key == Key.Enter)
                Button_Click(this, e);
        }

        private void Window_Activated(object sender, System.EventArgs e)
        {
            Text.Text = Value;
        }
    }
}
