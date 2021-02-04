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

        private const string SimpleName = "ContentManipulations.Replace";
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
            var _locker = new object();
            var outputList = new List<IDestinationFileInfo>(inputFiles.Count());

            Parallel.ForEach(inputFiles, fileInfo => 
            {
                if (fileInfo.IsTextFile)
                {
                    var newInfo = new DestinationFileInfo();
                    newInfo.SourceFileName = fileInfo.SourceFileName;
                    newInfo.SourceFileContent = fileInfo.SourceFileContent;
                    newInfo.IsTextFile = fileInfo.IsTextFile;
                    newInfo.DestinationFileName = fileInfo.DestinationFileName;
                    newInfo.DestinationFileContent = fileInfo.DestinationFileContent.Replace(From ?? string.Empty, To ?? string.Empty);

                    lock(_locker) outputList.Add(newInfo);
                }
                else
                   lock(_locker) outputList.Add(fileInfo);
            });

            return outputList;
        }

        public override bool LoadFromSimpleObject(dynamic simpleObject)
        {
            if (simpleObject == null)
                return false;

            if (simpleObject.Type != SimpleName)
                return false;

            if (simpleObject.Properties == null)
                return false;

            From = simpleObject.Properties.From;
            To = simpleObject.Properties.To;

            return true;
        }

        public override object GetSimpleObject()
        {
            return new
            {
                Type = SimpleName,
                Parameters = new
                {
                    From = From,
                    To = To
                }
            };
        }

        public override void Close()
        {
            this.collection.Remove(this);
        }
    }
}
