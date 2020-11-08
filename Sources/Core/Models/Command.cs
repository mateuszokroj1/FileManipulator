using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace FileManipulator
{
    /// <summary>
    /// Basic command
    /// </summary>
    public class Command : ICommand
    {
        #region Constructors

        /// <summary>Command that run <paramref name="toExecute"/> when <paramref name="canExecute"/> return <see langword="true"/>.</summary>
        /// <param name="canExecute">Determine that command can be executed.</param>
        /// <param name="toExecute">Action to be executed</param>
        /// <exception cref="ArgumentNullException"/>
        public Command(Func<object, bool> canExecute, Action<object> toExecute) : this()
        {
            this.canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
            this.toExecute = toExecute ?? throw new ArgumentNullException(nameof(toExecute));
        }

        /// <summary>Command that run <paramref name="toExecute"/> when <paramref name="toExecute"/> return <see langword="true"/>.</summary>
        /// <param name="canExecute">Determine that command can be executed.</param>
        /// <param name="toExecute">Action to be executed</param>
        /// <exception cref="NullReferenceException"/>
        public Command(Func<bool> canExecute, Action<object> toExecute)
            : this(parameter => canExecute(), toExecute ?? throw new NullReferenceException()) { }

        /// <summary>Command that run <paramref name="toExecute"/> when <paramref name="toExecute"/> return <see langword="true"/>.</summary>
        /// <param name="canExecute">Determine that command can be executed.</param>
        /// <param name="toExecute">Action to be executed</param>
        /// <exception cref="NullReferenceException"/>
        public Command(Func<bool> canExecute, Action toExecute)
            : this(parameter => canExecute(), parameter => toExecute()) { }

        /// <summary>Command that run <paramref name="toExecute"/>.</summary>
        /// <param name="toExecute">Action to be executed</param>
        /// <exception cref="ArgumentNullException"/>
        public Command(Action<object> toExecute)
            : this(parameter => true, toExecute) { }

        /// <summary>Command that run <paramref name="toExecute"/>.</summary>
        /// <param name="toExecute">Action to be executed</param>
        /// <exception cref="NullReferenceException"/>
        public Command(Action toExecute)
            : this(parameter => true, parameter => toExecute()) { }

        protected Command() { }

        #endregion

        #region Fields

        public event EventHandler CanExecuteChanged;
        protected Action<object> toExecute;
        protected Func<object, bool> canExecute;

        #endregion

        #region Methods

        /// <summary>
        /// Checks that <see cref="Command"/> can be executed.
        /// </summary>
        public bool CanExecute(object parameter) => this.canExecute(parameter);

        /// <summary>
        /// Execute action of <see cref="Command"/>.
        /// </summary>
        public void Execute(object parameter) => this.toExecute(parameter);

        /// <summary>
        /// Notify that CanExecute changed.
        /// </summary>
        public void RaiseCanExecuteChanged() =>
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        #endregion
    }
}
