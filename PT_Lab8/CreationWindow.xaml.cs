using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;

namespace PT_Lab8
{
    public partial class CreationWindow : Window
    {
        private readonly string _directoryPath;
        private string _filePath;
        public CreationWindow(string dPath)
        {
            InitializeComponent();
            _directoryPath = dPath;
            _filePath = "";
        }

        public string GetFilePath()
        {
            return _filePath;
        }

        private void OkClicked(object sender, RoutedEventArgs eventArgs)
        {
            var path = _directoryPath + "\\" + FileInput.Text;
            var info = new DirectoryInfo(_directoryPath);
            var attributes = info.Attributes;

            if (FileRadio.IsChecked != null && (bool)FileRadio.IsChecked)
            {
                if (!Regex.IsMatch(FileInput.Text, @"([a-zA-Z0-9\s\\.\-\(\):]).(txt|php|html)$"))
                {
                    Console.WriteLine("Invalid file name.");
                    return;
                }
                
                if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    attributes = RemoveAttribute(info.Attributes, FileAttributes.ReadOnly);
                    File.SetAttributes(path, attributes);
                    Console.WriteLine(@"The {0} file is no longer ReadOnly", path);
                }
                
                File.Create(path);
                var fi = new FileInfo(path);
                
                if (RCheck.IsChecked != null && (bool)RCheck.IsChecked)
                {
                    fi.Attributes |= FileAttributes.ReadOnly;
                }
                
                if (ACheck.IsChecked != null && (bool)ACheck.IsChecked)
                {
                    fi.Attributes |=  FileAttributes.Archive;
                }
                
                if (HCheck.IsChecked != null && (bool)HCheck.IsChecked)
                {
                    fi.Attributes |=  FileAttributes.Hidden;
                }
                
                if (SCheck.IsChecked != null && (bool)SCheck.IsChecked)
                {
                    fi.Attributes |=  FileAttributes.System;
                }

                _filePath = path;
                
                Close();
            }
            
            else if(DirRadio.IsChecked != null && (bool)DirRadio.IsChecked)
            {
                if (!Regex.IsMatch(FileInput.Text, @"([a-zA-Z0-9\s\\.\-\(\):])$"))
                {
                    Console.WriteLine("Invalid directory name.");
                    return;
                }

                Directory.CreateDirectory(path);
                var di = new DirectoryInfo(path);
                
                if (RCheck.IsChecked != null && (bool)RCheck.IsChecked)
                {
                    di.Attributes |= FileAttributes.ReadOnly;
                }
                
                if (ACheck.IsChecked != null && (bool)ACheck.IsChecked)
                {
                    di.Attributes |= FileAttributes.Archive;
                }
                
                if (HCheck.IsChecked != null && (bool)HCheck.IsChecked)
                {
                    di.Attributes |= FileAttributes.Hidden;
                }
                
                if (SCheck.IsChecked != null && (bool)SCheck.IsChecked)
                {
                    di.Attributes |= FileAttributes.System;
                }

                _filePath = path;

                Close();
            }
            else
            {
                Console.WriteLine("You need to specify whether you want to create directory or file.");
            }
        }

        private void CancelClicked(object sender, RoutedEventArgs eventArgs) => Close();
        
        private static FileAttributes RemoveAttribute(FileAttributes attributes, FileAttributes attributesToRemove)
        {
            return attributes & ~attributesToRemove;
        }

    }
}