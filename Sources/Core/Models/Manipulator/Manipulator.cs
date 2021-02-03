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

        public Manipulator(ICollection<ITask> tasks, Func<bool> onCloseWhileWorking)
        {
            this.tasks = tasks ?? throw new ArgumentNullException(nameof(tasks));
            this.onCloseWhileWorking = onCloseWhileWorking ?? throw new ArgumentNullException(nameof(onCloseWhileWorking));

            var generator = new TaskDefaultNameGenerator<Manipulator>(tasks);
            Name = generator.Generate();

            ResetAsync().Wait();

            CloseCommand = new Command(() => Close());
        }

        #endregion

        #region Fields

        private CancellationTokenSource cancellationTokenSource;
        private readonly ICollection<ITask> tasks;
        private readonly Func<bool> onCloseWhileWorking;

        #endregion

        #region Properties

        public IEnumerable<string> FilePaths { get; set; }

        public string DestinationDir { get; set; }

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

                        var info = new SourceFileInfo
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
                        sourceFileInfos = await filter.FilterAsync(sourceFileInfos);
                        SynchronizationContext.Send(state => filter.State = SubTaskState.Done, null);
                    }

                cancellationToken.ThrowIfCancellationRequested();

                foreach (var filter in Filters)
                    if (filter is IContentFilter)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        SynchronizationContext.Send(state => filter.State = SubTaskState.Working, null);
                        sourceFileInfos = await filter.FilterAsync(sourceFileInfos);
                        SynchronizationContext.Send(state => filter.State = SubTaskState.Done, null);
                    }
            }
            catch(OperationCanceledException)
            {
                if(sourceFileInfos != null)
                    foreach(var fileInfo in sourceFileInfos)
                    {
                        var stream = fileInfo.SourceFileContent?.BaseStream;
                        fileInfo.SourceFileContent?.Dispose();
                        stream?.Dispose();
                    }

                throw;
            }

            return sourceFileInfos.Select(info => new DestinationFileInfo(info));
        }

        private async STT.Task ManipulateFilesAsync(IEnumerable<IDestinationFileInfo> files, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            foreach(var manipulation in Manipulations)
                if(manipulation is INameManipulation)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    SynchronizationContext.Send(state => manipulation.State = SubTaskState.Working, null);
                    files = await manipulation.ManipulateAsync(files);
                    SynchronizationContext.Send(state => manipulation.State = SubTaskState.Done, null);
                }

            if(!string.IsNullOrEmpty(DestinationDir))
            {
                if (!Directory.Exists(DestinationDir))
                    Directory.CreateDirectory(DestinationDir);

                foreach(var file in files)
                {
                    file.DestinationFileName = Path.Combine(DestinationDir, Path.GetFileName(file.DestinationFileName));
                }
            }

            foreach (var fileInfo in files)
                if (fileInfo.SourceFileName != fileInfo.DestinationFileName &&
                    !string.IsNullOrWhiteSpace(fileInfo.DestinationFileName) &&
                    File.Exists(fileInfo.DestinationFileName))
                    throw new IOException("Destination file exists.");

            foreach(var manipulation in Manipulations)
                if(manipulation is IContentManipulation)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    SynchronizationContext.Send(state => manipulation.State = SubTaskState.Working, null);
                    await manipulation.ManipulateAsync(files);
                    SynchronizationContext.Send(state => manipulation.State = SubTaskState.Done, null);
                }

            Parallel.ForEach(files, async (fileInfo, state) =>
            {
                var stream = fileInfo?.SourceFileContent?.BaseStream;
                fileInfo?.SourceFileContent?.Dispose();
                stream?.Dispose();

                FileStream file = null;

                if(cancellationToken.IsCancellationRequested)
                {
                    state.Stop();
                    return;
                }

                if (fileInfo.SourceFileName != fileInfo.DestinationFileName)
                {
                    if(!fileInfo.IsTextFile)
                    {
                        File.Move(fileInfo.SourceFileName, fileInfo.DestinationFileName);
                    }
                    else
                    {
                        file = new FileStream(fileInfo.DestinationFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
                    }
                }

                if (file == null)
                    file = new FileStream(fileInfo.DestinationFileContent, FileMode.Open, FileAccess.ReadWrite, FileShare.None);

                file.SetLength(0);

                using (var writer = new StreamWriter(file))
                {
                    await writer.WriteAsync(fileInfo.DestinationFileContent);

                    await writer.FlushAsync();
                }
            });

            cancellationToken.ThrowIfCancellationRequested();
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
                }

                State = TaskState.Ready;
                LastError = null;
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

            SynchronizationContext.Send(state => State = TaskState.Done, null);
        }

        public async override STT.Task StopAsync()
        {
            if(State == TaskState.Working || State == TaskState.Paused)
            {
                Stopping.OnNext(new TaskEventArgs(this));
                this.cancellationTokenSource.Cancel();
            }
        }

        public async void Close()
        {
            if(State == TaskState.Working || State == TaskState.Paused)
            {
                if (!this.onCloseWhileWorking())
                    return;

                await StopAsync();
            }

            while (State == TaskState.Working || State == TaskState.Paused);

            base.Close(this.tasks);
        }

        public async override void Dispose()
        {
            if (State == TaskState.Working || State == TaskState.Paused)
                await StopAsync();
        }

        #endregion
    }
}
