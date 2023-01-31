using Kriptografija_Projekat_.Commands;
using Kriptografija_Projekat_.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Kriptografija_Projekat_.ViewModel
{
    public class RegisterViewModel : ViewModelBase
    {
        private NavigationStore _navigationStore;
        public ICommand NavigateBackCommand { get; set; }

        public RegisterViewModel(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
            NavigateBackCommand = new NavigateCommand<CheckSertificateViewModel>(navigationStore, () => new CheckSertificateViewModel(navigationStore));
        }
    }
}
