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
            OnValueChanged =
                this.propertyChangedObservable
                .Where(propertyName => propertyName == nameof(ProgressValue))
                .Select(p => ProgressValue);

            OnStatusChanged =
                this.propertyChangedObservable
                .Where(propertyName => propertyName == nameof(Status))
                .Select(p => Status);
        }

        #endregion

        #region Fields

        private float progressValue;
        private string status;

        private readonly IObservable<string> propertyChangedObservable;

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

        public IObservable<float> OnValueChanged { get; }

        public IObservable<string> OnStatusChanged { get; }

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
