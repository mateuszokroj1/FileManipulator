using System;
using System.Collections.Generic;
using System.Text;

namespace FileManipulator
{
    public interface ITextDialog
    {
        bool ShowDialog();

        string Value { get; set; }
    }
}
