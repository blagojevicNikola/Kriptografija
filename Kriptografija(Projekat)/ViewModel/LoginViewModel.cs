using Kriptografija_Projekat_.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kriptografija_Projekat_.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        private NavigationStore _navigationStore;

        public LoginViewModel(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
        }
    }
}
