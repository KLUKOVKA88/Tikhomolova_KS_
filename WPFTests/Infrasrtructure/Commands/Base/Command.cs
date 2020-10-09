using System;
using System.Windows.Input;

namespace WPFTests.Infrasrtructure.Commands.Base
{
    abstract class Command : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        bool ICommand.CanExecute(object paramenter) => CanExecute(paramenter);

        void ICommand.Execute(object parameter) => Execute(parameter);

        protected virtual bool CanExecute(object p) => true;

        protected abstract void Execute(object p);
    }
}
