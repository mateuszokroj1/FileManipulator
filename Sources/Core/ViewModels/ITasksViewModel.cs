﻿using System.Collections.ObjectModel;

namespace FileManipulator
{
    public interface ITasksViewModel
    {
        ITask SelectedItem { get; set; }

        ObservableCollection<ModelBase> TasksViewModels { get; }
    }
}