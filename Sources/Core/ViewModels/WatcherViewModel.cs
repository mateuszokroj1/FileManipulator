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
    public class WatcherViewModel : ModelBase, IDisposable
    {
        #region Constructors

        public WatcherViewModel(Watcher watcher)
        {
            Watcher = watcher ?? throw new ArgumentNullException(nameof(watcher));

            ClearCommand = new Command(() => Clear());
            BrowseCommand = new Command(() => Browse());

            SetCommands();


                    this.watcherObservable2 = Watcher?.IncludeSubdirectoriesChanged
                    .ObserveOn(SynchronizationContext.Current)
                    .Subscribe(neValue => OnPropertyChanged(nameof(IncludeSubdirectories)));

                    this.watcherObservable3 = Watcher?.CanStartChanged.Merge(Watcher?.CanStopChanged).Subscribe(_ =>
                        OnPropertyChanged(nameof(CanEditSettings))
                    );

                    SetCommands();


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
        private IDisposable watcherObservable1,
                            watcherObservable2,
                            watcherObservable3;

        #endregion

        #region Properties

        public Action OnInvalidInput { get; set; }

        public Action<Exception> OnError { get; set; }

        public Watcher Watcher { get; }

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

        public ObservableCollection<WatcherAction> Actions => Watcher?.Actions ?? new ObservableCollection<WatcherAction>();

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

        public bool CanEditSettings => !(Watcher?.CanStop ?? false) && (Watcher?.CanStart ?? false);

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
            if (Watcher == null)
                return;

            if(!CheckPathIsValid())
            {
                OnInvalidInput?.Invoke();
                return;
            }

            Watcher.Path = Path;
            Watcher.IncludeSubdirectories = IsDirectoryPath && IncludeSubdirectories;
            
            if(Watcher.LastError != null)
            {
                OnError?.Invoke(Watcher.LastError);
                return;
            }

            if (Watcher.State != TaskState.Ready || !Watcher.CanStart)
                return;

            if (Watcher.State == TaskState.Ready)
                Clear();

            await Watcher.StartAsync();
        }

        public async void Pause()
        {
            await Watcher?.PauseAsync();
        }

        public async void Stop()
        {
            if (Watcher != null && Watcher.CanStop)
                await Watcher.StopAsync();
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

        private void SetCommands()
        {
            if(Watcher != null)
            {
                StartCommand = new ReactiveCommand(Watcher.CanStartChanged, () => Start());
                PauseCommand = new ReactiveCommand(Watcher.CanPauseChanged, () => Pause());
                StopCommand = new ReactiveCommand(Watcher.CanStopChanged, () => Stop());
            }
            else
            {
                StartCommand = new Command(() => false, () => Clear());
                PauseCommand = new Command(() => false, () => Clear());
                StopCommand = new Command(() => false, () => Clear());
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
