using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;

using FileManipulator.Models.Manipulator;
using FileManipulator.Models.Watcher;

namespace FileManipulator.UI

{
    public class MainViewModel : ModelBase
    {
        #region Constructor

        public MainViewModel()
        {
            CreateNewWatcherTaskCommand = new Command(() => CreateNew<Watcher>());
            CreateNewManipulatorTaskCommand = new Command(() => CreateNew<Manipulator>());
            HelpCommand = new Command(() => Process.Start(Path.Combine(Environment.CurrentDirectory, "Pomoc.docx")));
            EditTaskNameCommand = new ReactiveCommand(
                Observable.FromEventPattern(this, "PropertyChanged")
                .Select(args => (args.EventArgs as PropertyChangedEventArgs).PropertyName)
                .Where(name => name == nameof(SelectedTask))
                .Select(name => SelectedTask != null),
                () => EditTaskName());
        }

        #endregion

        #region Fields

        private Task selectedTask;

        #endregion

        #region Properties

        public ObservableCollection<Task> Tasks { get; set; } = new ObservableCollection<Task>();

        public Task SelectedTask
        {
            get => this.selectedTask;
            set => SetProperty(ref this.selectedTask, value);
        }

        public bool IsAnyWorkingTasks =>
            Tasks
            .Where(task => task.State == TaskState.Paused || task.State == TaskState.Working)
            .Count() > 0;

        public ICommand CreateNewWatcherTaskCommand { get; }

        public ICommand CreateNewManipulatorTaskCommand { get; }

        public ICommand EditTaskNameCommand { get; }

        public ICommand HelpCommand { get; }

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
            SelectedTask = newTask;
        }

        public void EditTaskName()
        {
            if (SelectedTask == null)
                return;

            RenameWindow window = new RenameWindow();

            
        }

        public async void Close(Func<bool> canClose, CancelEventArgs e)
        {
            if (IsAnyWorkingTasks)
            {
                if (!canClose())
                {
                    e.Cancel = true;
                    return;
                }

                foreach (var task in Tasks)
                    await task.StopAsync();

                Tasks.Clear();
            }
        }

        #endregion

    }
}
