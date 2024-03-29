﻿using Kriptografija_Projekat_.Commands;
using Kriptografija_Projekat_.Model;
using Kriptografija_Projekat_.Service;
using Kriptografija_Projekat_.Stores;
using Kriptografija_Projekat_.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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
        private string _fileContent;
        private UserFile? _selectedFile;
        private string _fileName = "";

        public string FileContent { get { return _fileContent; } set { _fileContent= value; NotifyPropertyChanged("FileContent"); } }
        public ObservableCollection<UserFile> UserFiles { get { return _userFiles; } set { _userFiles = value; NotifyPropertyChanged("UserFile"); } }
        public string FileName { get { return _fileName; } set { _fileName = value; NotifyPropertyChanged("FileName"); } }
        public UserFile? SelectedFile { get { return _selectedFile; } set { _selectedFile = value; NotifyPropertyChanged("SelectedFile"); } }
        public ICommand AddWindowCommand { get; set; }
        public ICommand DownloadWindowCommand { get; set; }
        public ICommand LoadFileContentCommand { get; set; }
        public ICommand SelectFileCommand { get; set; }
        public FilesOverviewViewModel(NavigationStore navigationStore, User user) 
        {
            _navigationStore = navigationStore;
            AddWindowCommand = new RelayCommand(addWindow);
            DownloadWindowCommand = new RelayCommand(downloadWindow);
            LoadFileContentCommand = new RelayCommand(loadFileContent);
            SelectFileCommand = new ParameterRelayCommand<UserFile>(selectFile);
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
                try
                {
                    FileSystemService fsService = new FileSystemService();
                    UserFile? file = fsService.AddFile(_user, vm.FilePath);
                    if (file != null)
                    {
                        UserService userService = new UserService();
                        userService.UploadInfo(_user, file);
                        UserFiles.Add(file);
                        win.Close();
                    }
                    else
                    {
                        MessageBox.Show("File doesn't exist!");
                    }
                }catch(Exception ex)
                {
                    MessageBox.Show("Error while adding file!");
                    Debug.WriteLine(ex.Message);
                }
            };
            win.Show();
        }

        public void downloadWindow()
        {
            DownloadFileWindow win = new DownloadFileWindow();
            DownloadFileViewModel vm = new DownloadFileViewModel();
            win.DataContext = vm;
            vm.CloseWindow += (a, e) =>
            {
                try
                {
                    UserFile? selected = null;
                    foreach (UserFile file in UserFiles)
                    {
                        if (file.IsSelected)
                        {
                            selected = file;
                            break;
                        }
                    }
                    if (selected != null && Directory.Exists(vm.PathName))
                    {
                        bool downloaded = selected.DownloadFile(vm.PathName, _user);
                        if (!downloaded)
                        {
                            MessageBox.Show("Your file is corrupted!");
                        }
                        else
                        {
                            win.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Error occured!");
                    }
                }catch(Exception ex)
                {
                    MessageBox.Show("Error while downloading the file!");
                    Debug.WriteLine(ex.Message);
                }
            };
            win.Show();
        }

        private void loadFileContent()
        {
            try
            {
                UserFile? file = null;
                foreach (UserFile userFile in UserFiles)
                {
                    if (userFile.IsSelected)
                    {
                        file = userFile;
                        break;
                    }
                }
                if (file == null)
                {
                    return;
                }
                byte[]? content = file.GetContent(_user);
                if (content == null)
                {
                    MessageBox.Show("This file has been corrupted!");
                }
                else
                {
                    FileContent = Encoding.UTF8.GetString(content);
                }
            }catch(Exception ex)
            {
                MessageBox.Show("Error while loading the file!");
                Debug.WriteLine(ex.Message);
            }
           
        }

        private void selectFile(UserFile file)
        {
            if(_selectedFile!=file)
            {
                FileContent = "";
                FileName = file.Name;
            }
        }

    }
}
