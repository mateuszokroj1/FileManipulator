﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;

using FileManipulator.Models.Manipulator;
using FileManipulator.Models.Watcher;

using Microsoft.WindowsAPICodePack.Dialogs;

using Newtonsoft.Json;

namespace FileManipulator.ViewModels
{
    public class MainViewModel : ModelBase
    {
        #region Constructor

        public MainViewModel()
        {
            TasksViewModel = new TasksViewModel(Tasks);

            SelectedItemChanged = CreatePropertyChangedObservable(nameof(SelectedItem), () => SelectedItem);

            SelectedItemChanged.Subscribe(value => TasksViewModel.SelectedItem = value);
            TasksViewModel.SelectedItemChanged.Subscribe(value => SelectedItem = value);

            CreateNewWatcherTaskCommand = new Command(() => CreateNew<Watcher>());
            CreateNewManipulatorTaskCommand = new Command(() => CreateNew<Manipulator>());
            HelpCommand = new Command(() => OpenHelp());

            EditTaskNameCommand = new ReactiveCommand(
                SelectedItemChanged.Select(_ => SelectedItem != null),
                () => EditTaskName()
            );

            CloseTaskCommand = new ReactiveCommand(
                SelectedItemChanged.Select(_ => SelectedItem != null),
                () => CloseTask()
            );

            SaveTaskToFileCommand = new ReactiveCommand(
                SelectedItemChanged.Select(_ => SelectedItem != null),
                () => SaveTaskToFile()
            );

            LoadTaskFromFileCommand = new Command(() => LoadTaskFromFile());
        }

        #endregion

        #region Fields

        private IViewModelWithModelProperty selectedItem;

        #endregion

        #region Properties

        public ObservableCollection<ITask> Tasks { get; set; } = new ObservableCollection<ITask>();

        public TasksViewModel TasksViewModel { get; }

        public IViewModelWithModelProperty SelectedItem
        {
            get => this.selectedItem;
            set => SetProperty(ref this.selectedItem, value);
        }

        public IObservable<IViewModelWithModelProperty> SelectedItemChanged { get; }

        public bool IsAnyWorkingTasks =>
            Tasks
            .Where(task => task.State == TaskState.Paused || task.State == TaskState.Working)
            .Count() > 0;

        public ICommand CreateNewWatcherTaskCommand { get; }

        public ICommand CreateNewManipulatorTaskCommand { get; }

        public ICommand EditTaskNameCommand { get; }

        public ICommand SaveTaskToFileCommand { get; }

        public ICommand LoadTaskFromFileCommand { get; }

        public ICommand CloseTaskCommand { get; }

        public ICommand HelpCommand { get; }

        public Func<bool> MessageOnCloseWhileTaskWorking { get; set; }

        public Func<string,string> RenameDialogAction { get; set; }

        public static Func<bool> ConfirmationOnStart { get; set; }

        #endregion

        #region Methods

        public void CreateNew<TaskType>()
            where TaskType : Task
        {
            if (Tasks == null || Tasks.Count > 100)
                return;

            ITask newTask;
            if (typeof(TaskType) == typeof(Watcher))
            {
                newTask = new Watcher(Tasks);
            }
            else if (typeof(TaskType) == typeof(Manipulator))
            {
                newTask = new Manipulator(Tasks, MessageOnCloseWhileTaskWorking);
            }
            else
                throw new InvalidOperationException("Invalid task type.");

            Tasks.Add(newTask);
        }

        public void EditTaskName()
        {
            if (SelectedItem == null)
                return;

            ITask task = TasksViewModel.GetSelectedTask();

            task.Name = RenameDialogAction(task.Name);
        }

        public async void SaveTaskToFile()
        {
            if (SelectedItem == null)
                return;

            var task = TasksViewModel.GetSelectedTask();

            var path = string.Empty;

            using (var dialog = new CommonSaveFileDialog())
            {
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                dialog.Title = "Zapisywanie do pliku";
                var filter = new CommonFileDialogFilter();
                filter.DisplayName = "JSON";
                filter.Extensions.Add("json");
                dialog.Filters.Add(filter);
                dialog.DefaultExtension = "json";

                if (dialog.ShowDialog() != CommonFileDialogResult.Ok)
                    return;

                path = dialog.FileName;
            }

            if (!Directory.Exists(Path.GetDirectoryName(path)))
                return;

            FileStream file = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);

            file.SetLength(0);

            using (var writer = new StreamWriter(file))
            {
                var content = task.GenerateJson();

                await writer.WriteAsync(content);

                await writer.FlushAsync();
            }

            file.Close();
        }

        public void LoadTaskFromFile()
        {
            var path = string.Empty;

            using (var dialog = new CommonOpenFileDialog())
            {
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                dialog.Title = "Wczytywanie z pliku";
                var filter = new CommonFileDialogFilter();
                filter.DisplayName = "JSON";
                filter.Extensions.Add("json");
                dialog.Filters.Add(filter);
                dialog.EnsureFileExists = true;
                dialog.Multiselect = false;

                if (dialog.ShowDialog() != CommonFileDialogResult.Ok)
                    return;

                path = dialog.FileName;
            }

            var content = File.ReadAllText(path);

            ITask task = new Watcher(Tasks);
            if(!task.LoadJson(content))
            {
                task = new Manipulator(Tasks, MessageOnCloseWhileTaskWorking);
                if (!task.LoadJson(content))
                    return;
            }

            TasksViewModel.Model.Add(task);
        }

        public void OpenHelp()
        {
            var path = Path.Combine(Environment.CurrentDirectory, "Pomoc.docx");

            if (!File.Exists(path)) return;

            try
            {
                Process.Start(path);
            }
            catch(Win32Exception) // When not setted default app for DOCX
            {
                Process.Start("explorer.exe", $"/select, \"{path}\"");
            }
        }

        public void CloseTask()
        {
            if(SelectedItem is WatcherViewModel watcherModel)
            {
                watcherModel.Model.Close();
            }
            else if(SelectedItem is ManipulatorViewModel manipulatorModel)
            {
                manipulatorModel.Model.Close();
            }
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
