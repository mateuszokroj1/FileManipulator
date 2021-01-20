using System.ComponentModel;

namespace FileManipulator.Models.Manipulator
{
    public interface ISubTask : INotifyPropertyChanged
    {
        SubTaskState State { get; set; }
    }
}
