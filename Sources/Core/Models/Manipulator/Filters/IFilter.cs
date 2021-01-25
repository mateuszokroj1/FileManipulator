using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

using FileManipulator.Models.Manipulator.FileInfos;

namespace FileManipulator.Models.Manipulator.Filters
{
    public interface IFilter : INotifyPropertyChanged, ISubTask
    {
        Task<IEnumerable<ISourceFileInfo>> FilterAsync(IEnumerable<ISourceFileInfo> inputList);
    }
}
