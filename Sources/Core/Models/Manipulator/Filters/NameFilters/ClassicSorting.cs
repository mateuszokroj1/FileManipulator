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

        private SortMode sortMode;
        private readonly ICollection<IFilter> collection;

        public SortMode SortMode
        {
            get => this.sortMode;
            set => SetProperty(ref this.sortMode, value);
        }

        public static IEnumerable<SortMode> SortModesSource { get; } = Enum.GetValues(typeof(SortMode)).Cast<SortMode>();

        public async Task<IEnumerable<ISourceFileInfo>> FilterAsync(IEnumerable<ISourceFileInfo> inputList)
        {
            if (SortMode == SortMode.Ascending)
                return inputList.OrderBy(item => item.SourceFileName, StringComparer.InvariantCulture);
            else if (SortMode == SortMode.Descending)
                return inputList.OrderByDescending(item => item.SourceFileName, StringComparer.InvariantCulture);
            else
                throw new InvalidOperationException("Invalid parameter value: SortMode.");
        }

        public override void Close()
        {
            this.collection.Remove(this);
        }
    }
}
