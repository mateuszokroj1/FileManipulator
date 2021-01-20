using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using FileManipulator.Models.Manipulator;
using FileManipulator.Models.Manipulator.FileInfos;
using FileManipulator.Models.Manipulator.Filters;
using FileManipulator.Models.Manipulator.Manipulations;

using STT = System.Threading.Tasks;

namespace FileManipulator
{
    public class Manipulator : Task, IManipulator
    {
        #region Constructor

        public Manipulator(IEnumerable<Task> tasks)
        {
            var generator = new TaskDefaultNameGenerator<Manipulator>(tasks);
            Name = generator.Generate();
        }

        #endregion

        #region Properties

        public IEnumerable<string> FilePaths { get; set; }

        public ObservableCollection<IFilter> Filters { get; } = new ObservableCollection<IFilter>();

        public ObservableCollection<IManipulation> Manipulations { get; } = new ObservableCollection<IManipulation>();

        #endregion

        #region Methods

        private async Task<IEnumerable<IDestinationFileInfo>> FilterFilesAsync()
        {
            if (FilePaths == null)
                return null;

            if (FilePaths.Count() < 1)
                return Enumerable.Empty<IDestinationFileInfo>();

            foreach (var filter in Filters)
                filter.State = SubTaskState.Pending;

            var sourceFileInfos = FilePaths
                .Select(path =>
                {
                    var info = new SourceFileInfo
                    {
                        SourceFileName = path,
                        SourceFileContent = new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)),
                    };

                    info.IsTextFile = new TextFileIdentifier(info.SourceFileContent.BaseStream).Check();

                    return info;
                });

            foreach (var filter in Filters)
                if (filter is INameFilter)
                {
                    filter.State = SubTaskState.Working;
                    sourceFileInfos = await filter.Filter(sourceFileInfos);
                    filter.State = SubTaskState.Done;
                }

            foreach (var filter in Filters)
                if (filter is IContentFilter)
                {
                    filter.State = SubTaskState.Working;
                    sourceFileInfos = await filter.Filter(sourceFileInfos);
                    filter.State = SubTaskState.Done;
                }

            return sourceFileInfos.Select(info => new DestinationFileInfo(info));
        }

        private async STT.Task ManipulateFilesAsync()
        {
            //TODO
        }

        public override STT.Task PauseAsync()
        {
            throw new System.NotImplementedException();
        }

        public override STT.Task ResetAsync()
        {
            throw new System.NotImplementedException();
        }

        public override STT.Task StartAsync()
        {
            throw new System.NotImplementedException();
        }

        public override STT.Task StopAsync()
        {
            throw new System.NotImplementedException();
        }

        public override void Dispose()
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
