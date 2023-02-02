using Kriptografija_Projekat_.Commands;
using Kriptografija_Projekat_.Model;
using Kriptografija_Projekat_.Service;
using Kriptografija_Projekat_.Stores;
using Kriptografija_Projekat_.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace Kriptografija_Projekat_.ViewModel
{
    public class FilesOverviewViewModel : ViewModelBase
    {
        private NavigationStore _navigationStore;
        private User _user;
        private ObservableCollection<UserFile> _userFiles;

        public ObservableCollection<UserFile> UserFile { get { return _userFiles; } set { _userFiles = value; NotifyPropertyChanged("UserFile"); } }
        public ICommand AddWindowCommand { get; set; }
        public FilesOverviewViewModel(NavigationStore navigationStore, User user) 
        {
            _navigationStore = navigationStore;
            AddWindowCommand = new RelayCommand(() => addWindow());
            _user = user;
        }

        private void addWindow()
        {
            AddFileWindow win = new AddFileWindow();
            win.DataContext = new AddFileViewModel(_user);
            AddFileViewModel vm = (AddFileViewModel)win.DataContext;
            vm.CloseWindow += (a, e) => {
                UserService userService = new UserService();
                UserFile? file = userService.UploadFile(_user, vm.FilePath);
                if(file!=null)
                {
                    UserFile.Add(file);
                    win.Close();
                }else
                {
                    MessageBox.Show("File doesn't exist!");
                }  
            };
            win.Show();
        }
    }
}
