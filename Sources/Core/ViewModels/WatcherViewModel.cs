﻿using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Input;

using FileManipulator.Models.Watcher;

using Microsoft.WindowsAPICodePack.Dialogs;

namespace FileManipulator.ViewModels
{
    public class WatcherViewModel : ViewModelWithModelProperty<Watcher>, IDisposable
    {
        #region Constructors

        public WatcherViewModel(Watcher watcher) : base(watcher)
        {
            ClearCommand = new Command(() => Clear());
            BrowseCommand = new Command(() => Browse());

            if(!Model.CanStart) // BUG: ustawienie ścieżki chwilę przed stworzeniem komendy Start, przez co przycisk zostaje zablokowany, TODO: poczekać z przypisaniem ścieżki do momentu gotowości ViewModelu
                StartCommand = new ReactiveCommand(Model.CanStartChanged, () => Start());
            else
                StartCommand = new Command(() => Start());
            PauseCommand = new ReactiveCommand(Model.CanPauseChanged, () => Pause());
            StopCommand = new ReactiveCommand(Model.CanStopChanged, () => Stop());

            this.watcherObservers[0] = Model.IncludeSubdirectoriesChanged
            .ObserveOn(SynchronizationContext.Current)
            .Subscribe(_ => OnPropertyChanged(nameof(IncludeSubdirectories)));

            this.watcherObservers[1] = Model.StateChanged
            .Subscribe(_ => OnPropertyChanged(nameof(CanEditSettings)));

            PathChanged = CreatePropertyChangedObservable(nameof(Path), () => Path);

            PathChanged
                .Subscribe(path => Model.Path = path);

            IsDirectoryPath = true;

            Model.PathChanged.Subscribe(_ =>
                {
                    if(File.Exists(Model.Path))
                    {
                        IsDirectoryPath = false;
                        Path = Model.Path;
                    }
                    else if(Directory.Exists(Model.Path))
                    {
                        IsDirectoryPath = true;
                        Path = Model.Path;
                    }
                    else
                    {
                        IsDirectoryPath = false;
                        Path = null;

                        if(Path != Model.Path)
                            Model.Path = null;
                    }
                });

            Path = watcher.Path;
            IncludeSubdirectories = watcher.IncludeSubdirectories;
        }

        #endregion

        #region Fields

        private bool isDirectoryPath;
        private string path;
        private bool includeSubdirectories;
        private readonly IDisposable[] watcherObservers = new IDisposable[3];
        private ICommand startCommand;

        #endregion

        #region Properties

        public Action OnInvalidInput { get; set; }

        public Action<Exception> OnError { get; set; }

        public string Path
        {
            get => this.path;
            set => SetProperty(ref this.path, value);
        }

        public bool IncludeSubdirectories
        {
            get => this.includeSubdirectories;
            set => SetProperty(ref this.includeSubdirectories, value);
        }

        public ObservableCollection<WatcherAction> Actions => Model.Actions ?? new ObservableCollection<WatcherAction>();

        public bool IsDirectoryPath
        {
            get => this.isDirectoryPath;
            set => SetProperty(ref this.isDirectoryPath, value);
        }

        public ICommand StartCommand
        {
            get => this.startCommand;
            set => SetProperty(ref this.startCommand, value);
        }

        public ICommand PauseCommand { get; }

        public ICommand StopCommand { get; }

        public ICommand ClearCommand { get; }

        public ICommand BrowseCommand { get; }

        public bool CanEditSettings => Model.State == TaskState.Ready;

        public IObservable<string> PathChanged { get; }

        #endregion

        #region Methods

        public async void Clear()
        {
            await Model.StopAsync();
            await Model.ResetAsync();
        }

        private bool CheckPathIsValid()
        {
            if (IsDirectoryPath)
            {
                if (!Directory.Exists(Path))
                    return false;
            }
            else if (!File.Exists(Path))
                return false;

            return true;
        }

        public async void Start()
        {
            if (Model == null)
                return;

            if(!CheckPathIsValid())
            {
                OnInvalidInput?.Invoke();
                return;
            }

            Model.Path = Path;
            Model.IncludeSubdirectories = IsDirectoryPath && IncludeSubdirectories;
            
            if(Model.LastError != null)
            {
                OnError?.Invoke(Model.LastError);
                return;
            }

            StartCommand = new ReactiveCommand(Model.CanStartChanged, () => Start());

            if (Model.State != TaskState.Ready || !Model.CanStart)
                return;

            if (Model.State == TaskState.Ready)
                Clear();

            await Model.StartAsync();
        }

        public async void Pause()
        {
            await Model?.PauseAsync();
        }

        public async void Stop()
        {
            if (Model != null && Model.CanStop)
                await Model.StopAsync();
        }

        public void Browse()
        {
            using (var dialog = new CommonOpenFileDialog())
            {
                dialog.Title = "Przeglądaj";
                dialog.AllowNonFileSystemItems = false;
                dialog.IsFolderPicker = IsDirectoryPath;
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                dialog.EnsureReadOnly = false;
                dialog.EnsurePathExists = true;
                dialog.Multiselect = false;
                dialog.ShowPlacesList = true;

                if (dialog.ShowDialog() != CommonFileDialogResult.Ok)
                    return;

                Path = dialog.FileName;
            }
        }

        public void Dispose()
        {
            foreach (var observer in this.watcherObservers)
                observer?.Dispose();
        }

        #endregion
    }
}
