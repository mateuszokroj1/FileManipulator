using System;
using System.Collections.Generic;
using System.Windows.Input;

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

        private ICollection<Task> tasks;
        private Task selectedItem;

        #endregion

        #region Properties

        public ICollection<Task> Tasks
        {
            get => this.tasks;
            set => SetProperty(ref this.tasks, value);
        }

        public Task SelectedItem
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
