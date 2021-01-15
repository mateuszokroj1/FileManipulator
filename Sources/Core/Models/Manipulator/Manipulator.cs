using System.Collections;
using System.Collections.Generic;

using STT = System.Threading.Tasks;

namespace FileManipulator
{
    public class Manipulator : Task, IManipulator
    {
        #region Constructor

        public Manipulator(IEnumerable<Task> tasks)
        {

        }

        #endregion

        #region Methods

        public override STT.Task PauseAsync()
        {
            throw new System.NotImplementedException();
        }

        public override STT.Task ResetAsync()
        {
            throw new System.NotImplementedException();
        }

        public override STT.Task StartAsync()
        {
            throw new System.NotImplementedException();
        }

        public override STT.Task StopAsync()
        {
            throw new System.NotImplementedException();
        }

        public override void Dispose()
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}
