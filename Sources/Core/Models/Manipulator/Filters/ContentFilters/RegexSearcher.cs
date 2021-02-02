using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using FileManipulator.Models.Manipulator.FileInfos;

namespace FileManipulator.Models.Manipulator.Filters.ContentFilters
{
    public class RegexSearcher : SubTask, IContentFilter
    {
        public RegexSearcher(ICollection<IFilter> subTaskCollection)
        {
            this.collection = subTaskCollection ?? throw new ArgumentNullException(nameof(subTaskCollection));
        }

        private Regex regex;
        private readonly ICollection<IFilter> collection;

        public Regex Regex
        {
            get => this.regex;
            set => SetProperty(ref this.regex, value);
        }

        public async Task<IEnumerable<ISourceFileInfo>> FilterAsync(IEnumerable<ISourceFileInfo> inputList)
        {
            if (Regex != null)
                return inputList.Where(fileInfo => (fileInfo.IsTextFile && Regex.IsMatch(fileInfo.SourceFileContent.ReadToEnd())) || !fileInfo.IsTextFile);
            else
                return inputList.Where(fileInfo => !fileInfo.IsTextFile);
        }

        public override void Close()
        {
            this.collection.Remove(this);
        }
    }
}
