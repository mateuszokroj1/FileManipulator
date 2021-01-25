using System.Collections.ObjectModel;

namespace FileManipulator
{
    public class TasksViewModel : ModelBase, ITasksViewModel
    {
        #region Constructor

        public TasksViewModel()
        {
        }

        #endregion

        #region Fields

        private ObservableCollection<ITask> tasks;
        private ITask selectedItem;

        #endregion

        #region Properties

        public ObservableCollection<ITask> Tasks
        {
            get => this.tasks;
            set => SetProperty(ref this.tasks, value);
        }

        public ITask SelectedItem
        {
            get => this.selectedItem;
            set => SetProperty(ref this.selectedItem, value);
        }

        #endregion

        #region Methods

        #endregion
    }
}
