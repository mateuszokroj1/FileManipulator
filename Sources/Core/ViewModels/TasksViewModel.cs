using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

using FileManipulator.Models.Manipulator;
using FileManipulator.Models.Watcher;

namespace FileManipulator
{
    public class TasksViewModel : ModelBase
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
