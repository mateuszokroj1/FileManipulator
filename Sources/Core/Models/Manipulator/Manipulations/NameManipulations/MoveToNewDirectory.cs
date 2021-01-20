using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using FileManipulator.Models.Manipulator.FileInfos;

namespace FileManipulator.Models.Manipulator.Manipulations.NameManipulations
{
    public class MoveToNewDirectory : ModelBase, INameManipulation
    {
        private SubTaskState state;

        public SubTaskState State
        {
            get => this.state;
            set => SetProperty(ref this.state, value);
        }

        public Task<IEnumerable<IDestinationFileInfo>> Manipulate(IEnumerable<IDestinationFileInfo> inputFiles)
        {
            throw new NotImplementedException();
        }
    }
}
