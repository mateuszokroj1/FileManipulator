using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive.Linq;
using System.Threading;

using STT = System.Threading.Tasks;

namespace FileManipulator.Models.Watcher
{
    public class Watcher : Task
    {
        #region Constructor

        public Watcher(ICollection<ITask> tasks) : base()
        {
            this.tasksCollection = tasks ?? throw new ArgumentNullException(nameof(tasks));

            var generator = new TaskDefaultNameGenerator<Watcher>(tasks);
            Name = generator.Generate();
            ResetAsync().Wait();

            this.pathObserver =
                CreatePropertyChangedObservable(nameof(Path), () => Path)
                .Where(_ => State != TaskState.Working && State != TaskState.Paused)
                .Subscribe(async p =>
                {
                    this.watcher?.Dispose();

                    if (CheckPathIsValid(Path))
                    {
                        await ResetAsync();
                    }

                    OnPropertyChanged(nameof(IncludeSubdirectories));
                    OnPropertyChanged(nameof(CanStart));
                });

            this.stateObserver = StateChanged.Subscribe(_ =>
                    {
                        OnPropertyChanged(nameof(CanStart));
                        OnPropertyChanged(nameof(CanStop));
                        OnPropertyChanged(nameof(CanPause));
                    });

            IncludeSubdirectoriesChanged = CreatePropertyChangedObservable(nameof(IncludeSubdirectories), () => IncludeSubdirectories);
            CanStartChanged = CreatePropertyChangedObservable(nameof(CanStart), () => CanStart);
            CanStopChanged = CreatePropertyChangedObservable(nameof(CanStop), () => CanStop);
            CanPauseChanged = CreatePropertyChangedObservable(nameof(CanPause), () => CanPause);

            CloseCommand = new Command(() => Close());
        }

        #endregion

        #region Fields

        private string path;
        private FileSystemWatcher watcher;
        private readonly IDisposable pathObserver;
        private readonly IDisposable stateObserver;
        private readonly ICollection<ITask> tasksCollection;
        private readonly IDisposable[] watcherObservers = new IDisposable[5];
        
        #endregion

        #region Properties

        public IObservable<bool> IncludeSubdirectoriesChanged { get; }

        public IObservable<bool> CanPauseChanged { get; }

        public IObservable<bool> CanStartChanged { get; }

        public IObservable<bool> CanStopChanged { get; }

        public string Path
        {
            get => this.path;
            set => SetProperty(ref this.path, value, nameof(Path), nameof(CanStart));
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

        public SynchronizationContext SynchronizationContext { get; set; } = SynchronizationContext.Current;

        public bool CanStop => State == TaskState.Working || State == TaskState.Paused;

        public bool CanStart => (State == TaskState.Stopped || State == TaskState.Ready) && CheckPathIsValid(Path);

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
            foreach (var observer in this.watcherObservers)
                observer?.Dispose();

            if(this.watcher != null)
                this.watcher.EnableRaisingEvents = false;

            foreach (var observer in this.watcherObservers)
                observer?.Dispose();

            this.watcher?.Dispose();
            this.watcher = null;

            if (State == TaskState.Working || State == TaskState.Paused)
            {
                Stopping.OnNext(new TaskEventArgs(this));
                Stopped.OnNext(new TaskEventArgs(this));
            }

            try
            {
                if(CheckPathIsValid(Path))
                    this.watcher = new FileSystemWatcher(Path);
            }
            catch (Exception exc)
            {
                SynchronizationContext.Send(state =>
                {
                    LastError = exc;
                    State = TaskState.Error;
                }, null);

                Error.OnNext(new TaskErrorEventArgs(exc, this));
            }

            SynchronizationContext.Send(state =>
            {
                State = TaskState.Ready;

                Actions.Clear();
                Progress.Report(0, Messages.ReadyState);
            }, null);
        }

        public override async STT.Task StartAsync()
        {
            if (!CanStart) return;

            Starting.OnNext(new TaskEventArgs(this));

            if (this.watcher == null)
                throw new InvalidOperationException("Watcher is null.");

            ConfigureWatcher();

            try
            {
                this.watcher.EnableRaisingEvents = true;
            }
            catch(Exception exc)
            {
                SynchronizationContext.Send(state =>
                {
                    LastError = exc;
                    State = TaskState.Error;
                    Error.OnNext(new TaskErrorEventArgs(exc, this));
                    this.watcher?.Dispose();
                    this.watcher = null;
                }, null);
                
                return;
            }

            Started.OnNext(new TaskEventArgs(this));

            SynchronizationContext.Send(state => State = TaskState.Working, null);
        }

        public override async STT.Task StopAsync()
        {
            if (!CanStop) return;

            Stopping.OnNext(new TaskEventArgs(this));

            this.watcher.EnableRaisingEvents = false;

            Stopped.OnNext(new TaskEventArgs(this));

            SynchronizationContext.Send(state => State = TaskState.Stopped, null);
        }

        private void SetLastError(Exception exception)
        {
            this.watcher?.Dispose();
            this.watcher = null;
            LastError = exception;
            Error.OnNext(new TaskErrorEventArgs(exception, this));
            State = TaskState.Error;
        }

        private void ConfigureWatcher()
        {
            if (this.watcher == null)
                return;

            this.watcherObservers[0] = Observable.FromEventPattern<ErrorEventHandler, ErrorEventArgs>(
                handler => this.watcher.Error += handler,
                handler => this.watcher.Error -= handler
            )
            .ObserveOn(SynchronizationContext)
            .Subscribe(args => SetLastError(args.EventArgs.GetException()));

            this.watcherObservers[1] = Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(
                handler => this.watcher.Created += handler,
                handler => this.watcher.Created -= handler
            )
            .ObserveOn(SynchronizationContext)
            .Subscribe(args => Actions.Add(new ChangeWatcherAction
            {
                ChangeType = ChangeType.Created,
                Path = args.EventArgs.FullPath
            }));

            this.watcherObservers[2] = Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(
                handler => this.watcher.Changed += handler,
                handler => this.watcher.Changed -= handler
            )
            .ObserveOn(SynchronizationContext)
            .Subscribe(args => Actions.Add(new ChangeWatcherAction
            {
                ChangeType = ChangeType.Modified,
                Path = args.EventArgs.FullPath
            }));

            this.watcherObservers[3] = Observable.FromEventPattern<FileSystemEventHandler, FileSystemEventArgs>(
                handler => this.watcher.Deleted += handler,
                handler => this.watcher.Deleted -= handler
            )
            .ObserveOn(SynchronizationContext)
            .Subscribe(args => Actions.Add(new ChangeWatcherAction
            {
                ChangeType = ChangeType.Deleted,
                Path = args.EventArgs.FullPath
            }));

            this.watcherObservers[4] = Observable.FromEventPattern<RenamedEventHandler, RenamedEventArgs>(
                handler => this.watcher.Renamed += handler,
                handler => this.watcher.Renamed -= handler
            )
            .ObserveOn(SynchronizationContext)
            .Subscribe(args => Actions.Add(new RenameWatcherAction
            {
                DestinationPath = args.EventArgs.FullPath,
                Path = args.EventArgs.OldFullPath
            }));

            OnPropertyChanged(nameof(IncludeSubdirectories));
        }

        public async void Close()
        {
            await StopAsync();
            base.Close(this.tasksCollection);
        }

        public override void Dispose()
        {
            this.pathObserver?.Dispose();
            this.stateObserver?.Dispose();

            foreach (var observer in this.watcherObservers)
                observer?.Dispose();

            this.watcher?.Dispose();

            Starting.OnCompleted();
            Started.OnCompleted();
            Pausing.OnCompleted();
            Paused.OnCompleted();
            Error.OnCompleted();
            Stopping.OnCompleted();
            Stopped.OnCompleted();
        }

        public static bool CheckPathIsValid(string path) =>
            !string.IsNullOrEmpty(path) && (Directory.Exists(path) || File.Exists(path));

        #endregion
    }
}
