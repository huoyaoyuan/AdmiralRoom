using System;
using System.Windows.Input;

#pragma warning disable CS0067

namespace Huoyaoyuan.AdmiralRoom
{
    class DelegateCommand : ICommand
    {
        private readonly Action action;
        public DelegateCommand(Action action, bool canexecute = true)
        {
            this.action = action;
            _canexecute = canexecute;
        }
        public event EventHandler CanExecuteChanged;
        bool ICommand.CanExecute(object parameter) => CanExecute;
        void ICommand.Execute(object parameter) => action();

        #region CanExecute
        private bool _canexecute;
        public bool CanExecute
        {
            get { return _canexecute; }
            set
            {
                if (_canexecute != value)
                {
                    _canexecute = value;
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        #endregion
    }
}
