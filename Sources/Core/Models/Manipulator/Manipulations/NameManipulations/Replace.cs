﻿using System;
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

            IsClearModeChanged = CreatePropertyChangedObservable(nameof(IsClearMode), () => IsClearMode);
        }

        private const string SimpleName = "NameManipulations.Replace";
        private readonly ICollection<IManipulation> collection;
        private string from, to;
        private bool isClearMode;

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

        public bool IsClearMode
        {
            get => this.isClearMode;
            set => SetProperty(ref this.isClearMode, value);
        }

        public IObservable<bool> IsClearModeChanged { get; }

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
                    (!IsClearMode
                        ? Path.GetFileNameWithoutExtension(fileInfo.DestinationFileName)
                            .Replace(From ?? string.Empty, To ?? string.Empty)
                        : string.Empty)
                    + Path.GetExtension(fileInfo.DestinationFileName)
                )
            });
        }

        public override bool LoadFromSimpleObject(dynamic simpleObject)
        {
            if (simpleObject == null)
                return false;

            if (simpleObject.Type.Value != SimpleName)
                return false;

            if (simpleObject.Parameters == null)
                return false;

            From = simpleObject.Parameters.From.Value;
            To = simpleObject.Parameters.To.Value;
            IsClearMode = simpleObject.Parameters.IsClearMode.Value;

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
                    To = To,
                    IsClearMode = IsClearMode
                }
            };
        }

        public override void Close()
        {
            this.collection.Remove(this);
        }
    }
}
