using System;
using System.Collections.Generic;
using System.Text;

using FileManipulator.Models.Manipulator;

namespace FileManipulator.ViewModels
{
    public class ManipulatorViewModel : ModelBase
    {
        public ManipulatorViewModel(Manipulator manipulator)
        {

        }

        public Manipulator Manipulator { get; }
    }
}
