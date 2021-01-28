using System;
using System.Collections.Generic;
using System.Text;

using FileManipulator.Models.Manipulator;

namespace FileManipulator.ViewModels
{
    public class ManipulatorViewModel : ViewModelWithModelProperty<Manipulator>
    {
        public ManipulatorViewModel(Manipulator manipulator) : base(manipulator)
        {

        }

    }
}
