using Kriptografija_Projekat_.Commands;
using Kriptografija_Projekat_.Model;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Kriptografija_Projekat_.ViewModel
{
    public class DownloadFileViewModel : ViewModelBase
    {
        private string _pathName = "";
        public event EventHandler CloseWindow;

        public string PathName { get { return _pathName; } set { _pathName = value; NotifyPropertyChanged("PathName"); } }
        public ICommand DownloadCommand { get; set; }
        public ICommand FindPathCommand { get; set; }
        public DownloadFileViewModel() 
        {
            DownloadCommand = new RelayCommand(() => download());
            FindPathCommand = new RelayCommand(() => findPath());
        }

        public void download()
        {
            CloseWindow.Invoke(this, EventArgs.Empty);  
        }

        public void findPath() 
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            dialog.IsFolderPicker = true;

            if(dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                PathName = dialog.FileName;
            }
        }
    }
}
