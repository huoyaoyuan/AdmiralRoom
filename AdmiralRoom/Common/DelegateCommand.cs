using System;
using System.Windows.Input;

#pragma warning disable CS0067

namespace Huoyaoyuan.AdmiralRoom
{
    class DelegateCommand : ICommand
    {
        private readonly Action action;
        public DelegateCommand(Action action)
        {
            this.action = action;
        }
        public event EventHandler CanExecuteChanged;
        public bool CanExecute(object parameter) => true;
        public void Execute(object parameter) => action();
    }
}
