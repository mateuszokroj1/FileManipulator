using System;

namespace FileManipulator
{
    public sealed class ReactiveCommand : Command
    {
        #region Constructor

        /// <summary>Command that run <paramref name="toExecute"/> when <paramref name="canExecute"/> return <see langword="true"/>.</summary>
        /// <param name="canExecute">Determine that command can be executed.</param>
        /// <param name="toExecute">Action to be executed</param>
        /// <exception cref="ArgumentNullException" />
        public ReactiveCommand(IObservable<bool> canExecute, Action<object> toExecute) : base()
        {
            this.unsubscriber = canExecute?.Subscribe(newValue =>
            { 
                this.canExecuteValue = newValue;
                RaiseCanExecuteChanged();
            })
            ?? throw new ArgumentNullException(nameof(canExecute));
            this.canExecute = parameter => this.canExecuteValue;

            this.toExecute = toExecute ?? throw new ArgumentNullException(nameof(toExecute));
        }

        /// <summary>Command that run <paramref name="toExecute"/> when <paramref name="canExecute"/> return <see langword="true"/>.</summary>
        /// <param name="canExecute">Determine that command can be executed.</param>
        /// <param name="toExecute">Action to be executed</param>
        /// <exception cref="NullReferenceException" />
        public ReactiveCommand(IObservable<bool> canExecute, Action toExecute)
        : this(canExecute ?? throw new NullReferenceException(), parameter => toExecute()) { }

        #endregion

        #region Fields

        private readonly IDisposable unsubscriber;
        private bool canExecuteValue;

        #endregion
    }
}
