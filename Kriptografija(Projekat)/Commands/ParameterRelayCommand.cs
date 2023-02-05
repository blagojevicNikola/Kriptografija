using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Kriptografija_Projekat_.Commands
{
    public class ParameterRelayCommand<T> : ICommand
    {
        private Action<T> _action;

        public ParameterRelayCommand(Action<T> action)
        {
            _action = action;
        }
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            _action.Invoke((T)parameter!);
        }
    }
}
