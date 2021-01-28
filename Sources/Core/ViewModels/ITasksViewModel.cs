using System.Collections.ObjectModel;

namespace FileManipulator
{
    public interface ITasksViewModel
    {
        IViewModelWithModelProperty SelectedItem { get; set; }

        ObservableCollection<IViewModelWithModelProperty> TasksViewModels { get; }
    }
}