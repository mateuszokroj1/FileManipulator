using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using FileManipulator.Models.Manipulator.Manipulations.NameManipulations;

namespace FileManipulator.UI.Views.Controls.Manipulator.Manipulations
{
    /// <summary>
    /// Logika interakcji dla klasy Replace.xaml
    /// </summary>
    public partial class ReplaceName : UserControl
    {
        public ReplaceName()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty IsTextBoxEnabledProperty = DependencyProperty.Register(
            "IsTextBoxEnabled",
            typeof(bool), typeof(ReplaceName));

        public bool IsTextBoxEnabled
        {
            get => (bool)GetValue(IsTextBoxEnabledProperty);
            set => SetValue(IsTextBoxEnabledProperty, value);
        }

        private void Root_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(DataContext is Replace model)
            {
                IsTextBoxEnabled = !model.IsClearMode;
                model.IsClearModeChanged.Subscribe(value => IsTextBoxEnabled = !value);
            }
        }
    }
}
