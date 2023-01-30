using Kriptografija_Projekat_.Stores;
using Kriptografija_Projekat_.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Kriptografija_Projekat_.Commands
{
    public class NavigateCommand<TViewModel> : ICommand
        where TViewModel : ViewModelBase
    {
        public event EventHandler? CanExecuteChanged;
        private NavigationStore _navigationStore;
        private Func<TViewModel> _func;

        public NavigateCommand(NavigationStore navigationStore, Func<TViewModel> func)
        {
            _navigationStore = navigationStore;
            _func = func;
        }
    
        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            _navigationStore.CurrentViewModel = _func();
        }
    }
}
