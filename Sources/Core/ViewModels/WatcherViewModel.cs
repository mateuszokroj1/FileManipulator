using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Text;

using FileManipulator.Models.Watcher;

namespace FileManipulator.ViewModels
{
    public class WatcherViewModel : ModelBase
    {
        #region Constructors

        public WatcherViewModel()
        {
            this.propertyChangedObservable =
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

        private Watcher watcher;
        private readonly IObservable<string> propertyChangedObservable;

        #endregion

        #region Properties

        public Watcher Watcher
        {
            get => this.watcher;
            set => SetProperty(ref this.watcher, value);
        }

        #endregion

        #region Methods



        #endregion
    }
}
