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
                Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                    handler => PropertyChanged += handler,
                    handler => PropertyChanged -= handler
                )
                .Select(args => args?.EventArgs.PropertyName);
        }

        #endregion

        #region Fields

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        protected IObservable<string> PropertyChangedObservable { get; }

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

        public IObservable<TValue> CreatePropertyChangedObservable<TValue>(string propertyName, Func<TValue> resultValue) =>
            PropertyChangedObservable.Where(name => name == propertyName).Select(_ => resultValue());

        #endregion
    }
}
