using System;
using System.ComponentModel;
using System.Reactive.Linq;

namespace FileManipulator
{
    public class TaskProgress : ModelBase, IProgress<float>
    {
        #region Constructor

        public TaskProgress()
        {
            ProgressValueChanged = CreatePropertyChangedObservable(nameof(ProgressValue), () => ProgressValue);
            StatusChanged = CreatePropertyChangedObservable(nameof(Status), () => Status);
        }

        #endregion

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

        public IObservable<float> ProgressValueChanged { get; }

        public IObservable<string> StatusChanged { get; }

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
