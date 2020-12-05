using STT = System.Threading.Tasks;

namespace FileManipulator
{
    public class Manipulator : Task, IManipulator
    {
        
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
    }
}
