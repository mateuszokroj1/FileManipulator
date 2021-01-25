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

using FileManipulator.Models.Watcher;

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
