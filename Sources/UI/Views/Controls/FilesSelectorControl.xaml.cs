using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace FileManipulator.UI
{
    /// <summary>
    /// Logika interakcji dla klasy FilesSelectorControl.xaml
    /// </summary>
    public partial class FilesSelectorControl : UserControl
    {
        public FilesSelectorControl()
        {
            InitializeComponent();

            var model = DataContext as FilesSelectorViewModel;
            var binding = new Binding("SelectedFiles");
            binding.Source = model;
            binding.Mode = BindingMode.TwoWay;
            SetBinding(SelectedFilesProperty, binding);
        }

        public static readonly DependencyProperty SelectedFilesProperty = DependencyProperty.Register(
            "SelectedFiles",
            typeof(IEnumerable<string>),
            typeof(FilesSelectorControl),
            new PropertyMetadata(null)
        );

        public IEnumerable<string> SelectedFiles => GetValue(SelectedFilesProperty) as IEnumerable<string>;
    }
}
