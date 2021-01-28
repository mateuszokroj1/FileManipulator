using System.Windows;
using System.Windows.Controls;

using FileManipulator.Models.Manipulator;
using FileManipulator.Models.Watcher;
using FileManipulator.ViewModels;

namespace FileManipulator.UI
{
    public class TasksViewTemplateSelector : DataTemplateSelector
    {
        #region Properties

        public DataTemplate WatcherTemplate { get; set; }

        public DataTemplate ManipulatorTemplate { get; set; }

        public DataTemplate UndefinedTemplate { get; set; }

        #endregion

        #region Methods

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is WatcherViewModel)
                return WatcherTemplate;
            else if (item is ManipulatorViewModel)
                return ManipulatorTemplate;
            else
                return UndefinedTemplate;
        }

        #endregion
    }
}
