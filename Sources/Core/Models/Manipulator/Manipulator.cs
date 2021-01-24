using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using FileManipulator.Models.Manipulator.FileInfos;
using FileManipulator.Models.Manipulator.Filters;
using FileManipulator.Models.Manipulator.Manipulations;

using STT = System.Threading.Tasks;

namespace FileManipulator.Models.Manipulator
{
    public class Manipulator : Task, IManipulator
    {
        #region Constructor

        public Manipulator(IEnumerable<Task> tasks)
        {
            var generator = new TaskDefaultNameGenerator<Manipulator>(tasks);
            Name = generator.Generate();

            ResetAsync().Wait();
        }

        #endregion

        #region Fields

        private CancellationTokenSource cancellationTokenSource;

        #endregion

        #region Properties

        public IEnumerable<string> FilePaths { get; set; }

        public SynchronizationContext SynchronizationContext { get; set; } = SynchronizationContext.Current;

        public ObservableCollection<IFilter> Filters { get; } = new ObservableCollection<IFilter>();

        public ObservableCollection<IManipulation> Manipulations { get; } = new ObservableCollection<IManipulation>();

        #endregion

        #region Methods

        private async Task<IEnumerable<IDestinationFileInfo>> FilterFilesAsync(CancellationToken cancellationToken)
        {
            if (FilePaths == null)
                return null;

            if (FilePaths.Count() < 1)
                return Enumerable.Empty<IDestinationFileInfo>();

            cancellationToken.ThrowIfCancellationRequested();

            SynchronizationContext.Send(state =>
            {
                foreach (var filter in Filters)
                    filter.State = SubTaskState.Pending;
            }, null);

            IEnumerable<ISourceFileInfo> sourceFileInfos = null;

            try
            {
                sourceFileInfos = FilePaths
                    .Select(path =>
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        var info = new ISourceFileInfo
                        {
                            SourceFileName = path,
                            SourceFileContent = new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)),
                        };

                        info.IsTextFile = new TextFileIdentifier(info.SourceFileContent.BaseStream).Check();

                        return info;
                    });

                cancellationToken.ThrowIfCancellationRequested();

                foreach (var filter in Filters)
                    if (filter is INameFilter)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        SynchronizationContext.Send(state => filter.State = SubTaskState.Working, null);
                        sourceFileInfos = await filter.Filter(sourceFileInfos);
                        SynchronizationContext.Send(state => filter.State = SubTaskState.Done, null);
                    }

                cancellationToken.ThrowIfCancellationRequested();

                foreach (var filter in Filters)
                    if (filter is IContentFilter)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        SynchronizationContext.Send(state => filter.State = SubTaskState.Working, null);
                        sourceFileInfos = await filter.Filter(sourceFileInfos);
                        SynchronizationContext.Send(state => filter.State = SubTaskState.Done, null);
                    }
            }
            catch(OperationCanceledException)
            {
                if(sourceFileInfos != null)
                    foreach(var fileInfo in sourceFileInfos)
                    {

                    }

                throw;
            }

            return sourceFileInfos.Select(info => new DestinationFileInfo(info));
        }

        private async STT.Task ManipulateFilesAsync(IEnumerable<IDestinationFileInfo> files, CancellationToken cancellationToken)
        {
            
        }

        public override STT.Task PauseAsync()
        {
            throw new InvalidOperationException();
        }

        public async override STT.Task ResetAsync()
        {
            SynchronizationContext.Send(async state =>
            {
                if(State == TaskState.Working || State == TaskState.Paused)
                {
                    Stopping.OnNext(new TaskEventArgs(this));
                    await StopAsync();
                    Stopped.OnNext(new TaskEventArgs(this));

                    State = TaskState.Ready;
                    LastError = null;
                }
            }, null);

            this.cancellationTokenSource = new CancellationTokenSource();
        }

        public async override STT.Task StartAsync()
        {
            var cancelToken = this.cancellationTokenSource.Token;

            if (FilePaths?.Count() < 1 || Filters.Count < 1 && Manipulations.Count < 1)
                return;

            try
            {
                cancelToken.ThrowIfCancellationRequested();

                var destinationFilesInfos = await FilterFilesAsync(cancelToken);

                await ManipulateFilesAsync(destinationFilesInfos, cancelToken);
            }
            catch(OperationCanceledException)
            {
                Stopped.OnNext(new TaskEventArgs(this));
            }
            catch(Exception exc)
            {
                SynchronizationContext.Send(state =>
                {
                    State = TaskState.Error;
                    LastError = exc;
                }, null);
                Error.OnNext(new TaskErrorEventArgs(exc, this));
            }
            finally
            {

            }
        }

        public async override STT.Task StopAsync()
        {
            if(State == TaskState.Working || State == TaskState.Paused)
            {
                Stopping.OnNext(new TaskEventArgs(this));
                this.cancellationTokenSource.Cancel();
            }
        }

        public override void Dispose()
        {
            
        }

        #endregion
    }
}
