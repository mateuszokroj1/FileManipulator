using System;

namespace FileManipulator
{
    public class TaskProgress : ModelBase, IProgress<float>
    {
        #region Fields

        private float progressValue;
        private string status;

        #endregion

        #region Properties

        public string Status
        {
            get => this.status;
            set => SetProperty(ref this.status, value);
        }

        public float ProgressValue
        {
            get => this.progressValue;
            set => SetProperty(ref this.progressValue, value);
        }

        #endregion

        #region Methods

        public void Report(float value)
        {
            ProgressValue = value;
        }

        public void Report(float value, string status)
        {
            Report(value);
            Status = status;
        }

        #endregion
    }
}
