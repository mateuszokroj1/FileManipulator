using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;

namespace FileManipulator
{
    public abstract class ModelBase : INotifyPropertyChanged
    {
        #region Constructor

        protected ModelBase()
        {
            PropertyChangedObservable =
                Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangingEventArgs>
                (
                    handler => PropertyChanged += handler,
                    handler => PropertyChanged -= handler
                )
                .Where(args => !string.IsNullOrWhiteSpace(args?.EventArgs?.PropertyName))
                .Select(args => args.EventArgs.PropertyName);
                
        }

        #endregion

        #region Fields

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        public IObservable<string> PropertyChangedObservable { get; }

        #endregion

        #region Methods

        protected void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            if(!string.IsNullOrWhiteSpace(propertyName))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void SetProperty<T>(ref T destination, T newValue, [CallerMemberName] string propertyName = "")
        {
            if (Equals(destination, newValue)) return;

            destination = newValue;
            OnPropertyChanged(propertyName);
        }

        protected void SetProperty<T>(ref T destination, T newValue, params string[] propertiesNames)
        {
            if (Equals(destination, newValue)) return;

            destination = newValue;

            if (propertiesNames?.Length > 0)
                foreach (var name in propertiesNames)
                    OnPropertyChanged(name);
        }

        #endregion
    }
}
