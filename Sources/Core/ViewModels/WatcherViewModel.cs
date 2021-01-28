using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

            StartCommand = new ReactiveCommand(Model.CanStartChanged, () => Start());
            PauseCommand = new ReactiveCommand(Model.CanPauseChanged, () => Pause());
            StopCommand = new ReactiveCommand(Model.CanStopChanged, () => Stop());

            this.watcherObservable1 = Model.IncludeSubdirectoriesChanged
            .ObserveOn(SynchronizationContext.Current)
            .Subscribe(neValue => OnPropertyChanged(nameof(IncludeSubdirectories)));

            this.watcherObservable2 = Model.CanStartChanged
                .Merge(Model.CanStopChanged)
                .Subscribe(_ => OnPropertyChanged(nameof(CanEditSettings)));

            PathChanged = CreatePropertyChangedObservable(nameof(Path), () => Path);

            IsDirectoryPath = true;
        }

        #endregion

        #region Fields

        private bool isDirectoryPath;
        private string path;
        private bool includeSubdirectories;
        private ICommand startCommand;
        private ICommand pauseCommand;
        private ICommand stopCommand;
        private IDisposable watcherObservable1, watcherObservable2;

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

        public ICommand PauseCommand
        {
            get => this.pauseCommand;
            set => SetProperty(ref this.pauseCommand, value);
        }

        public ICommand StopCommand
        {
            get => this.stopCommand;
            set => SetProperty(ref this.stopCommand, value);
        }

        public ICommand ClearCommand { get; }

        public ICommand BrowseCommand { get; }

        public bool CanEditSettings => !Model.CanStop && Model.CanStart;

        public IObservable<string> PathChanged { get; }

        #endregion

        #region Methods

        public void Clear() => Actions?.Clear();

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
            this.watcherObservable1?.Dispose();
            this.watcherObservable2?.Dispose();
        }

        #endregion
    }
}
