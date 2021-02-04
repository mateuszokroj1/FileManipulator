using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using FileManipulator.Models.Manipulator.FileInfos;

namespace FileManipulator.Models.Manipulator.Manipulations.NameManipulations
{
    public class SequentialNaming : SubTask, INameManipulation
    {
        public SequentialNaming(ICollection<IManipulation> collection)
        {
            this.collection = collection ?? throw new ArgumentNullException(nameof(collection));

            StartNumber = 1;
            Increment = 1;
            FixedPlaces = 1;
            SeparateWith = string.Empty;
        }

        private const string SimpleName = "NameManipulations.SequentialNaming";
        private readonly ICollection<IManipulation> collection;
        private bool addPrefix, addSuffix, isIndexing;
        private string prefix, suffix, separateWith;
        private uint startNumber, increment, fixedPlaces;

        public bool AddPrefix
        {
            get => this.addPrefix;
            set => SetProperty(ref this.addPrefix, value);
        }

        public string Prefix
        {
            get => this.prefix;
            set => SetProperty(ref this.prefix, value);
        }

        public bool AddSuffix
        {
            get => this.addSuffix;
            set => SetProperty(ref this.addSuffix, value);
        }

        public string Suffix
        {
            get => this.suffix;
            set => SetProperty(ref this.suffix, value);
        }

        public bool IsIndexing
        {
            get => this.isIndexing;
            set => SetProperty(ref this.isIndexing, value);
        }

        public uint StartNumber
        {
            get => this.startNumber;
            set => SetProperty(ref this.startNumber, value);
        }

        public uint Increment
        {
            get => this.increment;
            set => SetProperty(ref this.increment, value);
        }

        public uint FixedPlaces
        {
            get => this.fixedPlaces;
            set => SetProperty(ref this.fixedPlaces, value);
        }

        public string SeparateWith
        {
            get => this.separateWith;
            set => SetProperty(ref this.separateWith, value);
        }

        public async Task<IEnumerable<IDestinationFileInfo>> ManipulateAsync(IEnumerable<IDestinationFileInfo> inputFiles)
        {
            var outputList = new List<IDestinationFileInfo>();

            uint currentIndex = StartNumber;

            foreach(var fileInfo in inputFiles)
            {
                var newInfo = new DestinationFileInfo
                {
                    IsTextFile = fileInfo.IsTextFile,
                    SourceFileName = fileInfo.SourceFileName,
                    SourceFileContent = fileInfo.SourceFileContent,
                    DestinationFileContent = fileInfo.DestinationFileContent,
                };

                var filename = Path.GetFileNameWithoutExtension(fileInfo.DestinationFileName);

                var builder = new StringBuilder();

                if (AddPrefix && Prefix != null)
                    builder.Append(Prefix);

                builder.Append(filename);

                if(IsIndexing)
                {
                    builder.Append(SeparateWith ?? string.Empty);

                    var number = currentIndex.ToString();
                    number = number.PadLeft((int)FixedPlaces, '0');
                    builder.Append(number);

                    currentIndex += Increment;
                }

                if (AddSuffix && Suffix != null)
                    builder.Append(Suffix);

                newInfo.DestinationFileName = Path.Combine(
                    Path.GetDirectoryName(fileInfo.DestinationFileName),
                    builder.ToString() + Path.GetExtension(fileInfo.DestinationFileName));

                outputList.Add(newInfo);
            }

            return outputList;
        }

        public override bool LoadFromSimpleObject(dynamic simpleObject)
        {
            if (simpleObject == null)
                return false;

            if (simpleObject.Type.Value != SimpleName)
                return false;

            if (simpleObject.Parameters == null)
                return false;

            AddPrefix = simpleObject.Parameters.AddPrefix.Value;
            Prefix = simpleObject.Parameters.Prefix.Value;
            AddSuffix = simpleObject.Parameters.AddSuffix.Value;
            Suffix = simpleObject.Parameters.Suffix.Value;

            IsIndexing = simpleObject.Parameters.IsIndexing.Value;
            StartNumber = simpleObject.Parameters.StartNumber.Value;
            Increment = simpleObject.Parameters.Increment.Value;
            FixedPlaces = simpleObject.Parameters.FixedPlaces.Value;
            SeparateWith = simpleObject.Parameters.SeparateWith.Value;

            return true;
        }

        public override object GetSimpleObject()
        {
            return new
            {
                Type = SimpleName,
                Parameters = new
                {
                    AddPrefix,
                    Prefix = AddPrefix ? Prefix : null,
                    AddSuffix,
                    Suffix = AddSuffix,
                    IsIndexing,
                    StartNumber,
                    Increment,
                    FixedPlaces,
                    SeparateWith
                }
            };
        }

        public override void Close()
        {
            this.collection.Remove(this);
        }
    }
}
