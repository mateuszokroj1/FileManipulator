using System;
using System.Collections.Generic;
using System.Text;

using FileManipulator.Models.Watcher;

namespace FileManipulator.Helpers
{
    public static class WatcherActionTypeConverter
    {
        public static string GetString(Type type)
        {
            if (type == typeof(ChangeWatcherAction))
                return "Zmiana";
            else if (type == typeof(RenameWatcherAction))
                return "Zmiana nazwy";
            else
                return null;
        }
    }
}
