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
            CreateNewWatcherCommand = new Command(() => CreateNew<Watcher>());
            CreateNewManipulatorCommand = new Command(() => CreateNew<Manipulator>());
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

        public ICommand CreateNewWatcherCommand { get; }

        public ICommand CreateNewManipulatorCommand { get; }

        #endregion

        #region Methods

        public void CreateNew<TaskType>()
            where TaskType : Task
        {
            if (Tasks == null || Tasks.Count > 100)
                return;

            Task newTask;
            if (typeof(TaskType) == typeof(Watcher))
            {
                newTask = new Watcher(Tasks);
            }
            else if (typeof(TaskType) == typeof(Manipulator))
            {
                newTask = new Manipulator(Tasks);
            }
            else
                throw new InvalidOperationException("Invalid task type.");

            Tasks.Add(newTask);
            SelectedItem = newTask;
        }

        #endregion
    }
}
