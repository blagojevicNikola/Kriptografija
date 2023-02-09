using Kriptografija_Projekat_.Commands;
using Kriptografija_Projekat_.Model;
using Kriptografija_Projekat_.Service;
using Microsoft.Win32;
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
        private User _user;
        private string _filePath = "";
        public event EventHandler CloseWindow;

        public string FilePath { get { return _filePath; } set { _filePath = value;  NotifyPropertyChanged("FilePath"); } }
        public ICommand AddCommand { get; set; }
        public ICommand FindFileCommand { get; set; }
        public AddFileViewModel(User user) 
        {
            AddCommand = new RelayCommand(() => add());
            FindFileCommand = new RelayCommand(() => findFile());
            _user = user;
        }

        private void add()
        {
            CloseWindow.Invoke(this, new EventArgs());
        }

        private void findFile() 
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if(dialog.ShowDialog() == true)
            {
                FilePath = dialog.FileName; 
            }
        }
    }
}
