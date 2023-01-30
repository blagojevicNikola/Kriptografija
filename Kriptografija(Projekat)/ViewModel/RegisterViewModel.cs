using Kriptografija_Projekat_.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kriptografija_Projekat_.ViewModel
{
    public class RegisterViewModel : ViewModelBase
    {
        private NavigationStore _navigationStore;

        public RegisterViewModel(NavigationStore navigationStore)
        {
            _navigationStore = navigationStore;
        }   
    }
}
