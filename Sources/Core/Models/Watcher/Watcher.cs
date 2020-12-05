using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive.Linq;

using STT = System.Threading.Tasks;

namespace FileManipulator
{
    public class Watcher : Task
    {
        #region Constructor

        public Watcher(IEnumerable<Task> tasks)
        {
            var generator = new TaskDefaultNameGenerator<Watcher>(tasks);
            Name = generator.Generate();
            ResetAsync().Wait();
            State = TaskState.Ready;

            this.pathObserver =
                PropertyChangedObservable
                .Where(propertyName => propertyName == nameof(Path))
                .Where(p => State != TaskState.Working && State != TaskState.Paused)
                .Subscribe(p =>
                {
                    this.watcher.Dispose();

                    if (CheckPathIsValid(Path))
                    {
                        this.watcher = new FileSystemWatcher(Path)
                        {
                            EnableRaisingEvents = false,
                            IncludeSubdirectories = true
                        };

                        ConfigureNewWatcher();
                    }

                    OnPropertyChanged(nameof(IncludeSubdirectories));
                });
        }

        #endregion

        #region Fields

        private string path;
        private FileSystemWatcher watcher;
        private readonly IDisposable pathObserver;

        #endregion

        #region Properties

        public string Path
        {
            get => this.path;
            set => SetProperty(ref this.path, value);
        }

        public bool IncludeSubdirectories
        {
            get => this.watcher?.IncludeSubdirectories ?? false;
            set
            {
                if(!Equals(this.watcher.IncludeSubdirectories, value))
                {
                    this.watcher.IncludeSubdirectories = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool CanPause => State == TaskState.Working;

        public bool CanStop => State == TaskState.Working || State == TaskState.Paused;

        public bool CanStart => State == TaskState.Stopped || State == TaskState.Ready;

        public ObservableCollection<WatcherAction> Actions { get; } = new ObservableCollection<WatcherAction>();

        #endregion

        #region Methods

        public override async STT.Task PauseAsync()
        {
            if (!CanPause) return;

            Pausing.OnNext(new TaskEventArgs(this));
            
            State = TaskState.Paused;
            this.watcher.EnableRaisingEvents = false;

            Paused.OnNext(new TaskEventArgs(this));
        }

        public override async STT.Task ResetAsync()
        {
            this.watcher?.Dispose();
            this.watcher = null;

            if (State == TaskState.Working || State == TaskState.Paused)
            {
                Stopping.OnNext(new TaskEventArgs(this));
                Stopped.OnNext(new TaskEventArgs(this));
            }

            State = TaskState.Ready;

            Actions.Clear();
            Progress.Report(0, Messages.ReadyState);
        }

        public override async STT.Task StartAsync()
        {
            if (!CanStart) return;

            Starting.OnNext(new TaskEventArgs(this));

            if (this.watcher == null)
                throw new InvalidOperationException("Watcher is null.");

            try
            {
                this.watcher.EnableRaisingEvents = true;
            }
            catch(Exception exc)
            {
                LastError = exc;
                State = TaskState.Error;
                Error.OnNext(new TaskErrorEventArgs(exc, this));
                this.watcher?.Dispose();
                this.watcher = null;
                return;
            }

            Started.OnNext(new TaskEventArgs(this));

            State = TaskState.Working;
        }

        public override async STT.Task StopAsync()
        {
            if (!CanStop) return;

            Stopping.OnNext(new TaskEventArgs(this));

            this.watcher.EnableRaisingEvents = false;

            Stopped.OnNext(new TaskEventArgs(this));

            State = TaskState.Stopped;
        }

        private void SetLastError(Exception exception)
        {
            this.watcher?.Dispose();
            this.watcher = null;
            LastError = exception;
            Error.OnNext(new TaskErrorEventArgs(exception, this));
            State = TaskState.Error;
        }

        private void ConfigureNewWatcher()
        {
            Observable.FromEventPattern<ErrorEventHandler, ErrorEventArgs>(
                handler => this.watcher.Error += handler,
                handler => this.watcher.Error -= handler
            )
            .Subscribe(args => SetLastError(args.EventArgs.GetException()));

            Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(
                handler => this.watcher.Created += handler,
                handler => this.watcher.Created -= handler
            )
            .Subscribe(args => Actions.Add(new ChangeWatcherAction
            {
                ChangeType = ChangeType.Created,
                Path = args.EventArgs.FullPath
            }));

            Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(
                handler => this.watcher.Changed += handler,
                handler => this.watcher.Changed -= handler
            )
            .Subscribe(args => Actions.Add(new ChangeWatcherAction
            {
                ChangeType = ChangeType.Modified,
                Path = args.EventArgs.FullPath
            }));

            Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(
                handler => this.watcher.Deleted += handler,
                handler => this.watcher.Deleted -= handler
            )
            .Subscribe(args => Actions.Add(new ChangeWatcherAction
            {
                ChangeType = ChangeType.Deleted,
                Path = args.EventArgs.FullPath
            }));

            Observable.FromEventPattern<RenamedEventHandler, RenamedEventArgs>(
                handler => this.watcher.Renamed += handler,
                handler => this.watcher.Renamed -= handler
            )
            .Subscribe(args => Actions.Add(new RenameWatcherAction
        {
                DestinationPath = args.EventArgs.FullPath,
                Path = args.EventArgs.OldFullPath
            }));
        }

        public override void Dispose()
        {
            this.pathObserver?.Dispose();
            this.watcher?.Dispose();
        }

        public static bool CheckPathIsValid(string path) =>
            !string.IsNullOrEmpty(path) && (Directory.Exists(path) || File.Exists(path));

        #endregion
    }
}
