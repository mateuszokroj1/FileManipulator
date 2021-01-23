using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FileManipulator.Models.Watcher;
using FileManipulator.Models.Manipulator;

namespace FileManipulator
{
    /// <summary>
    /// Generates new name for task
    /// </summary>
    /// <exception cref="ArgumentNullException"/>
    public class TaskDefaultNameGenerator<TTask> : IGenerator<string>
        where TTask : Task
    {
        #region Constructor

        /// <summary>
        /// Creates new instance of <see cref="TaskDefaultNameGenerator{TTask}"/>.
        /// </summary>
        /// <param name="tasks">Collection of created tasks</param>
        public TaskDefaultNameGenerator(IEnumerable<Task> tasks)
        {
            Tasks = tasks ?? throw new ArgumentNullException(nameof(tasks));
        }

        #endregion

        #region Properties

        public IEnumerable<Task> Tasks { get; set; }

        #endregion

        #region Methods

        public string Generate()
        {
            if (typeof(TTask) == typeof(Watcher))
            {
                var regex = new Regex(@"^Watcher(\d+)$");

                var searchedValues = Tasks
                    ?.Where(task =>
                        task is Watcher &&
                        !string.IsNullOrEmpty(task.Name))
                    .Select(task => regex.Match(task.Name))
                    .Where(match => match.Success)
                    .Select(match => match.Groups[1].Captures[0].Value)
                    .Select(val => int.Parse(val));

                int maxSearchedValue = searchedValues.Count() > 0 ? searchedValues.Max() : 0;
                ++maxSearchedValue;

                return $"Watcher{maxSearchedValue}";
            }
            else
                if (typeof(TTask) == typeof(Manipulator))
            {
                throw new NotImplementedException();
            }
            else
                throw new InvalidOperationException("Not implemented for this type.");
        }

        #endregion
    }
}
