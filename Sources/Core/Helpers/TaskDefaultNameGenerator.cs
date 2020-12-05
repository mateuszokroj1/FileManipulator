using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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
            this.taskType = typeof(TTask);
        }

        #endregion

        #region Fields

        private readonly Type taskType;

        #endregion

        #region Properties

        public IEnumerable<Task> Tasks { get; set; }

        #endregion

        #region Methods

        public string Generate()
        {
            /*if(this.taskType == typeof(WatcherTask))
            {
                var regex = new Regex(@"^Watcher(\d+)$");
                var searched = Tasks
                    ?.Where(task =>
                        task is WatcherTask &&
                        !string.IsNullOrEmpty(task.Name))
                    .Select(task => regex.Match(task.Name))
                    .Where(match => match.Success)
                    .Select(match => );
            }*/
            throw new NotImplementedException();
        }

        #endregion
    }
}
