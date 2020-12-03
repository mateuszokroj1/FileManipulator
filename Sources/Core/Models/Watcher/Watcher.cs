using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive.Linq;
using System.Text;

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

        public ObservableCollection<WatcherAction> Actions { get; } = new ObservableCollection<WatcherAction>();

        #endregion

        #region Methods

        public override async STT.Task PauseAsync()
        {
            if (State != TaskState.Working) return;

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

        public override STT.Task StartAsync()
        {
            throw new NotImplementedException();
        }

        public override async STT.Task StopAsync()
        {
            
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
