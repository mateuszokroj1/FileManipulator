using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FileManipulator.Models.Manipulator.FileInfos;

namespace FileManipulator.Models.Manipulator.Manipulations.ContentManipulations
{
    public class Replace : SubTask, IContentManipulation
    {
        public Replace(ICollection<IManipulation> collection)
        {
            this.collection = collection ?? throw new ArgumentNullException(nameof(collection));
        }

        private readonly ICollection<IManipulation> collection;
        private string from, to;

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

        public async Task<IEnumerable<IDestinationFileInfo>> ManipulateAsync(IEnumerable<IDestinationFileInfo> inputFiles)
        {
            var outputList = new List<IDestinationFileInfo>(inputFiles.Count());
            foreach(var fileInfo in inputFiles)
            {
                if (fileInfo.IsTextFile)
                {
                    var newInfo = new DestinationFileInfo();
                    newInfo.SourceFileName = fileInfo.SourceFileName;
                    newInfo.SourceFileContent = fileInfo.SourceFileContent;
                    newInfo.IsTextFile = fileInfo.IsTextFile;
                    newInfo.DestinationFileName = fileInfo.DestinationFileName;
                    newInfo.DestinationFileContent = fileInfo.DestinationFileContent.Replace(From ?? string.Empty, To ?? string.Empty);

                    outputList.Add(newInfo);
                }
                else
                    outputList.Add(fileInfo);
            }

            return outputList;
        }

        public override void Close()
        {
            this.collection.Remove(this);
        }
    }
}
