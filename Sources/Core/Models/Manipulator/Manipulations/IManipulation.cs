using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

using FileManipulator.Models.Manipulator.FileInfos;

namespace FileManipulator.Models.Manipulator.Manipulations
{
    public interface IManipulation : INotifyPropertyChanged, ISubTask
    {
        Task<IEnumerable<IDestinationFileInfo>> Manipulate(IEnumerable<IDestinationFileInfo> inputFiles);
    }
}
