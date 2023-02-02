using Kriptografija_Projekat_.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Kriptografija_Projekat_.ViewModel
{
    public class AddFileViewModel : ViewModelBase
    {
        public event EventHandler CloseWindow;
        public ICommand AddCommand { get; set; }
        public AddFileViewModel() 
        {
            AddCommand = new RelayCommand(() => add());
        }


        private void add()
        {
            CloseWindow.Invoke(this, new EventArgs());
        }
    }
}
