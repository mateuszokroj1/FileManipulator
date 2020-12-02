using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using STT = System.Threading.Tasks;

namespace FileManipulator
{
    public class Watcher : Task
    {
        #region Constructor

        public Watcher(IEnumerable<Task> tasks)
        {
            var generator = new TaskDefaultNameGenerator<Watcher>(tasks);
            Name = generator.Generate();
            ResetAsync().Wait();
            State = TaskState.Ready;
        }

        #endregion

        #region Fields

        private string path;

        #endregion

        #region Properties

        public string Path
        {
            get => this.path;
            set => SetProperties(ref this.path, value, nameof(Path), nameof());
        }

        public bool PathIsValid 

        #endregion

        #region Methods

        public override STT.Task PauseAsync()
        {
            throw new NotImplementedException();
        }

        public override STT.Task ResetAsync()
        {
            throw new NotImplementedException();
        }

        public override STT.Task StartAsync()
        {
            throw new NotImplementedException();
        }

        public override STT.Task StopAsync()
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
