using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FileManipulator.Models.Manipulator.FileInfos;

namespace FileManipulator.Models.Manipulator.Filters.NameFilters
{
    public class ClassicSorting : SubTask, INameFilter
    {
        public ClassicSorting(ICollection<IFilter> subTasksCollection)
        {
            this.collection = subTasksCollection ?? throw new ArgumentNullException(nameof(subTasksCollection));
        }

        private const string SimpleName = "NameFilters.ClassicSorting";
        private SortMode sortMode;
        private readonly ICollection<IFilter> collection;

        public SortMode SortMode
        {
            get => this.sortMode;
            set => SetProperty(ref this.sortMode, value);
        }

        public static IEnumerable<string> SortModesSource { get; } = new string[] { "Rosnąco", "Malejąco" };

        public async Task<IEnumerable<ISourceFileInfo>> FilterAsync(IEnumerable<ISourceFileInfo> inputList)
        {
            if (SortMode == SortMode.Ascending)
                return inputList.OrderBy(item => item.SourceFileName, StringComparer.InvariantCulture);
            else if (SortMode == SortMode.Descending)
                return inputList.OrderByDescending(item => item.SourceFileName, StringComparer.InvariantCulture);
            else
                throw new InvalidOperationException("Invalid parameter value: SortMode.");
        }

        public override bool LoadFromSimpleObject(dynamic simpleObject)
        {
            if (simpleObject == null)
                return false;

            if (simpleObject.Type != SimpleName)
                return false;

            if (simpleObject.Properties == null)
                return false;

            SortMode = (SortMode)simpleObject.Properties.SortMode;

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
