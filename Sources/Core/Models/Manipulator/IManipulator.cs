using System.Collections.Generic;
using System.Collections.ObjectModel;

using FileManipulator.Models.Manipulator.Filters;
using FileManipulator.Models.Manipulator.Manipulations;

namespace FileManipulator.Models.Manipulator
{
    public interface IManipulator : ITask
    {
        IEnumerable<string> FilePaths { get; set; }

        ObservableCollection<IFilter> Filters { get; }

        ObservableCollection<IManipulation> Manipulations { get; }
    }
}