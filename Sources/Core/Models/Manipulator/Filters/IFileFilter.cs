using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace FileManipulator
{
    public interface IFileFilter : INotifyPropertyChanged
    {
        Task<IEnumerable<FileInfo>> Filter(IEnumerable<FileInfo> inputList);
    }
}
