using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace FileManipulator.UI
{
    /// <summary>
    /// Logika interakcji dla klasy TasksView.xaml
    /// </summary>
    public partial class TasksView : UserControl
    {
        public TasksView()
        {
            InitializeComponent();

            var model = DataContext as TasksViewModel;

            var binding = new Binding("Tasks");
            binding.Source = model;
            binding.Mode = BindingMode.TwoWay;
            SetBinding(TasksProperty, binding);

            binding = new Binding("SelectedItem");
            binding.Source = model;
            binding.Mode = BindingMode.TwoWay;
            SetBinding(SelectedItemProperty, binding);
        }

        #region Fields

        public static readonly DependencyProperty TasksProperty = DependencyProperty.Register(
            "Tasks",
            typeof(ObservableCollection<Task>),
            typeof(TasksView));

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
            "SelectedItem",
            typeof(Task),
            typeof(TasksView)
            );

        #endregion

        #region Properties

        public ObservableCollection<Task> Tasks
        {
            get => GetValue(TasksProperty) as ObservableCollection<Task>;
            set => SetValue(TasksProperty, value);
        }

        public Task SelectedItem
        {
            get => GetValue(SelectedItemProperty) as Task;
            set => SetValue(SelectedItemProperty, value);
        }

        #endregion
    }
}
