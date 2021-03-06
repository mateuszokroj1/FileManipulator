﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FileManipulator.Models.Manipulator.FileInfos;

namespace FileManipulator.Models.Manipulator.Filters.NameFilters
{
    public sealed class AlphanumericSorting : SubTask, INameFilter
    {
        public AlphanumericSorting(ICollection<IFilter> subTaskCollection)
        {
            this.collection = subTaskCollection ?? throw new ArgumentNullException(nameof(subTaskCollection));
        }

        private const string SimpleName = "NameFilters.AlphanumericSorting";
        private SortMode sortMode;
        private readonly ICollection<IFilter> collection;

        public static IEnumerable<string> SortModesSource { get; } = new string[] { "Rosnąco", "Malejąco" };

        public SortMode SortMode
        {
            get => this.sortMode;
            set => SetProperty(ref this.sortMode, value);
        }

        public async Task<IEnumerable<ISourceFileInfo>> FilterAsync(IEnumerable<ISourceFileInfo> inputList)
        {
            if (SortMode == SortMode.Ascending)
                return inputList.OrderBy(item => item.SourceFileName, new AlphanumericComparer());
            else if (SortMode == SortMode.Descending)
                return inputList.OrderByDescending(item => item.SourceFileName, new AlphanumericComparer());
            else
                throw new InvalidOperationException("Invalid parameter value: SortMode.");
        }

        public override bool LoadFromSimpleObject(dynamic simpleObject)
        {
            if (simpleObject == null)
                return false;

            if (simpleObject.Type.Value != SimpleName)
                return false;

            if (simpleObject.Parameters == null)
                return false;

            SortMode = (SortMode)simpleObject.Parameters.SortMode.Value;

            return true;
        }

        public override object GetSimpleObject()
        {
            return new
            {
                Type = SimpleName,
                Parameters = new
                {
                    SortMode = SortMode
                }
            };
        }

        public override void Close()
        {
            this.collection.Remove(this);
        }
    }
}
