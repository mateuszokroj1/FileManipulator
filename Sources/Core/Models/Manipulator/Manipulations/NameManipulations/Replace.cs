using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using FileManipulator.Models.Manipulator.FileInfos;

namespace FileManipulator.Models.Manipulator.Manipulations.NameManipulations
{
    public class Replace : SubTask, INameManipulation
    {
        public Replace(ICollection<IManipulation> collection)
        {
            this.collection = collection ?? throw new ArgumentNullException(nameof(collection));
        }

        private readonly ICollection<IManipulation> collection;
        private string from, to;
        private bool mustClear;

        public string From
        {
            get => this.from;
            set => SetProperty(ref this.from, value);
        }

        public string To
        {
            get => this.to;
            set => SetProperty(ref this.to, value);
        }

        public bool MustClear
        {
            get => this.mustClear;
            set => SetProperty(ref this.mustClear, value);
        }

        public async Task<IEnumerable<IDestinationFileInfo>> ManipulateAsync(IEnumerable<IDestinationFileInfo> inputFiles)
        {
            return inputFiles.Select(fileInfo => new DestinationFileInfo
            {
                SourceFileName = fileInfo.SourceFileName,
                SourceFileContent = fileInfo.SourceFileContent,
                IsTextFile = fileInfo.IsTextFile,
                DestinationFileContent = fileInfo.DestinationFileContent,
                DestinationFileName = Path.Combine(
                    Path.GetDirectoryName(fileInfo.DestinationFileName),
                    Path.GetFileNameWithoutExtension(fileInfo.DestinationFileName)
                        .Replace(From ?? string.Empty, !MustClear ? To ?? string.Empty : string.Empty),
                    Path.GetExtension(fileInfo.DestinationFileName)
                )
            });
        }

        public override void Close()
        {
            this.collection.Remove(this);
        }
    }
}
