using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FileManipulator.Models.Manipulator.FileInfos;

namespace FileManipulator.Models.Manipulator.Filters.NameFilters
{
    public class AlphanumericSorting : SubTask, INameFilter
    {
        private SortMode sortMode;

        public IEnumerable<SortMode> SortModesSource { get; } = Enum.GetValues(typeof(SortMode)).Cast<SortMode>();

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
    }
}
