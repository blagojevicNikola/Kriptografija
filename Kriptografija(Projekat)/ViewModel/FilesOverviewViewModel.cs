using Kriptografija_Projekat_.Commands;
using Kriptografija_Projekat_.Model;
using Kriptografija_Projekat_.Service;
using Kriptografija_Projekat_.Stores;
using Kriptografija_Projekat_.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

        public ObservableCollection<UserFile> UserFiles { get { return _userFiles; } set { _userFiles = value; NotifyPropertyChanged("UserFile"); } }
        public ICommand AddWindowCommand { get; set; }
        public FilesOverviewViewModel(NavigationStore navigationStore, User user) 
        {
            _navigationStore = navigationStore;
            AddWindowCommand = new RelayCommand(() => addWindow());
            _user = user;
            UserService userServ = new UserService();
            _userFiles = (ObservableCollection<UserFile>)userServ.GetUserFiles(user);
            //Debug.WriteLine(_userFiles.Count);
        }

        private void addWindow()
        {
            AddFileWindow win = new AddFileWindow();
            win.DataContext = new AddFileViewModel(_user);
            AddFileViewModel vm = (AddFileViewModel)win.DataContext;
            vm.CloseWindow += (a, e) => {
                FileSystemService fsService = new FileSystemService();
                UserFile? file = fsService.AddFile(_user, vm.FilePath);
                if(file!=null)
                {
                    UserService userService = new UserService();
                    userService.UploadInfo(_user, file);
                    UserFiles.Add(file);
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
