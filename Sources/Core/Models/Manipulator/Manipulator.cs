using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using FileManipulator.Models.Manipulator.FileInfos;
using FileManipulator.Models.Manipulator.Filters;
using FileManipulator.Models.Manipulator.Filters.ContentFilters;
using FileManipulator.Models.Manipulator.Filters.NameFilters;
using FileManipulator.Models.Manipulator.Manipulations;
using FileManipulator.Models.Manipulator.Manipulations.NameManipulations;

using Newtonsoft.Json;

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

            SynchronizationContext.Send(_ =>
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

                        SynchronizationContext.Send(_ => filter.State = SubTaskState.Working, null);
                        sourceFileInfos = await filter.FilterAsync(sourceFileInfos);
                        SynchronizationContext.Send(_ => filter.State = SubTaskState.Done, null);
                    }

                cancellationToken.ThrowIfCancellationRequested();

                foreach (var filter in Filters)
                    if (filter is IContentFilter)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        SynchronizationContext.Send(_ => filter.State = SubTaskState.Working, null);
                        sourceFileInfos = await filter.FilterAsync(sourceFileInfos);
                        SynchronizationContext.Send(_ => filter.State = SubTaskState.Done, null);
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

                    SynchronizationContext.Send(_ => manipulation.State = SubTaskState.Working, null);
                    files = await manipulation.ManipulateAsync(files);
                    SynchronizationContext.Send(_ => manipulation.State = SubTaskState.Done, null);
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

                    SynchronizationContext.Send(_ => manipulation.State = SubTaskState.Working, null);
                    files = await manipulation.ManipulateAsync(files);
                    SynchronizationContext.Send(_ => manipulation.State = SubTaskState.Done, null);
                }

            Parallel.ForEach(files, async (fileInfo, state) =>
            {
                var stream = fileInfo?.SourceFileContent?.BaseStream;
                fileInfo.SourceFileContent?.Dispose();
                fileInfo.SourceFileContent = null;
                stream?.Dispose();
                stream = null;

                FileStream file = null;

                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (fileInfo.SourceFileName != fileInfo.DestinationFileName)
                    {
                        if (!fileInfo.IsTextFile)
                        {
                            File.Move(fileInfo.SourceFileName, fileInfo.DestinationFileName);
                        }
                        else
                        {
                            file = new FileStream(fileInfo.DestinationFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
                            try
                            {
                                File.Delete(fileInfo.SourceFileName);
                            }
                            catch(IOException) { }
                        }
                    }

                    if (fileInfo.IsTextFile)
                    {
                        if (file == null)
                            file = new FileStream(fileInfo.DestinationFileName, FileMode.Open, FileAccess.ReadWrite, FileShare.None);

                        file.SetLength(0);

                        using (var writer = new StreamWriter(file))
                        {
                            await writer.WriteAsync(fileInfo.DestinationFileContent);

                            await writer.FlushAsync();
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    state.Stop();
                    throw;
                }
                finally
                {
                    file?.Close();
                }
            });
        }

        public override STT.Task PauseAsync()
        {
            throw new InvalidOperationException();
        }

        public async override STT.Task ResetAsync()
        {
            SynchronizationContext.Send(async _ =>
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
                SynchronizationContext.Send(_ =>
                {
                    State = TaskState.Error;
                    LastError = exc;
                }, null);

                Error.OnNext(new TaskErrorEventArgs(exc, this));
            }

            SynchronizationContext.Send(_ =>
            {
                State = TaskState.Done;
                Progress.Report(1, "Zadanie ukończone");
            }, null);
        }

        public async override STT.Task StopAsync()
        {
            if(State == TaskState.Working || State == TaskState.Paused)
            {
                Stopping.OnNext(new TaskEventArgs(this));
                this.cancellationTokenSource.Cancel();
            }
        }

        public override string GenerateJson()
        {
            var filters = Filters.Select(filter => filter.GetSimpleObject());
            var manipulations = Manipulations.Select(manipulation => manipulation.GetSimpleObject());

            return JsonConvert.SerializeObject(new
            {
                Type = "Manipulator",
                Parameters = new {
                    FilePaths = FilePaths,
                    DestinationDir = DestinationDir,
                    Filters = filters,
                    Manipulations = manipulations
                }
            });
        }

        public override bool LoadJson(string content)
        {
            if (string.IsNullOrEmpty(content))
                return false;

            try
            {
                dynamic deserialized = JsonConvert.DeserializeObject(content);

                if (deserialized == null)
                    return false;

                if (deserialized.Type != "Manipulator")
                    return false;

                if (deserialized.Parameters == null)
                    return false;

                var files = new List<string>();

                foreach (var file in deserialized.Parameters.FilePaths)
                    files.Add(file?.Value);

                FilePaths = files;

                DestinationDir = deserialized.Parameters.DestinationDir;

                dynamic filters = deserialized.Parameters.Filters;
                if(filters != null)
                    foreach (var filterSimpleObject in filters)
                    {
                        IFilter createdFilter = LoadFilterFromSimpleObject(filterSimpleObject, Filters);

                        if (createdFilter != null)
                            Filters.Add(createdFilter);
                    }

                dynamic manipulations = deserialized.Parameters.Manipulations;
                if (manipulations != null)
                    foreach (var manipulationSimpleObject in manipulations)
                    {
                        IManipulation createdManipulation = LoadManipulationFromSimpleObject(manipulationSimpleObject, Manipulations);

                        if (createdManipulation != null)
                            Manipulations.Add(createdManipulation);
                    }
            }
            catch (Exception) { return false; }

            return true;
        }

        private static IFilter LoadFilterFromSimpleObject(dynamic simpleObject, ICollection<IFilter> collection)
        {
            var filterTypes = new Type[]
            {
                typeof(Filters.NameFilters.RegexSearcher),
                typeof(Filters.ContentFilters.RegexSearcher),
                typeof(AlphanumericSorting),
                typeof(ClassicSorting)
            };

            IEnumerable<ISubTask> initializedFilters = filterTypes.Select(type => Activator.CreateInstance(type, collection) as ISubTask);

            try
            {
                foreach (var filterToLoad in initializedFilters)
                    if (filterToLoad.LoadFromSimpleObject(simpleObject))
                        return filterToLoad as IFilter;
            }
            catch(Exception) { return null; }

            return null;
        }

        private static IManipulation LoadManipulationFromSimpleObject(dynamic simpleObject, ICollection<IManipulation> collection)
        {
            var manipulationTypes = new Type[]
            {
                typeof(Replace),
                typeof(Manipulations.ContentManipulations.Replace),
                typeof(SequentialNaming)
            };

            IEnumerable<ISubTask> initializedManipulations = manipulationTypes.Select(type => Activator.CreateInstance(type, collection) as ISubTask);

            try
            {
                foreach (var manipulationToLoad in initializedManipulations)
                    if (manipulationToLoad.LoadFromSimpleObject(simpleObject))
                        return manipulationToLoad as IManipulation;
            }
            catch (Exception) { return null; }

            return null;
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
