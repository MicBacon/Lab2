using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.IO;


namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenButton_Click(object sender, EventArgs e)
        {
            var dlg = new FolderBrowserDialog()
            {
                Description = "Select Directory to open"
            };
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TreeView.Items.Clear();
                string path = dlg.SelectedPath;
                var root = CreateTreeDirectory(path);
                TreeView.Items.Add(root);
            }
        }

        

        private static TreeViewItem CreateTreeDirectory(string path)
        {
            var root = new TreeViewItem { 
                Header = System.IO.Path.GetFileName(path),
                Tag = path
            };

                       
            var currentDirFiles = Directory.GetFiles(path);

            foreach (var file in currentDirFiles)
            {
                var item = new TreeViewItem()
                {
                    Header = System.IO.Path.GetFileName(file),
                    Tag = file
                };

                var cm = new ContextMenu();
                var open = new MenuItem { Header = "Open", Tag = file };
                var delete = new MenuItem { Header = "Delete", Tag = file };

                open.Click += OpenMenuItem;
                delete.Click += DeleteMenuItem;
                /*var anInstanceofMyClass = new MainWindow();
                delete.Click += anInstanceofMyClass.DeleteMenuItem;*/

                cm.Items.Add(open);
                cm.Items.Add(delete);
                item.ContextMenu = cm;
                root.Items.Add(item);

            }

            var currentDirDirectories = Directory.GetDirectories(path);

            foreach (var directory in currentDirDirectories)
            {
                var cm = new ContextMenu();
                var create = new MenuItem {Header="Create",Tag = directory };
                var delete = new MenuItem { Header = "Delete", Tag = directory };

                create.Click += CreateNewMenuItem;
                delete.Click += DeleteMenuItem;
                /*var anInstanceofMyClass = new MainWindow();
                delete.Click += anInstanceofMyClass.DeleteMenuItem;*/
               

                cm.Items.Add(create);
                cm.Items.Add(delete);

                var item = CreateTreeDirectory(directory);
                item.ContextMenu = cm;
                root.Items.Add(item);
            }
            return root;
        }

        private static void OpenMenuItem(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void DeleteMenuItem(object sender, RoutedEventArgs e)
        {
            var file = (MenuItem)sender;
            var path = (string)file.Tag;

            var attributes = File.GetAttributes(path);

            if((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                // Make the file RW
                attributes = RemoveAttribute(attributes, FileAttributes.ReadOnly);
                File.SetAttributes(path, attributes);
                Console.WriteLine("The {0} file is no longer RO.", path);   
            }
            //Deleting a directory
            if((attributes & FileAttributes.Directory) == FileAttributes.Directory)
            {
                DelDir(path);
            }
            else
            {//Deleting a file
                File.Delete(path);
            }
            TreeViewItem item = (TreeViewItem)TreeView.SelectedItem;
            
          
        }

        private static void DelDir(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            foreach(var subdir in dir.GetDirectories())
            {
                DelDir(subdir.FullName);
            }
            foreach (var file in dir.GetFiles())
            {
                File.Delete(file.FullName);
            }
            Directory.Delete(path);
        }

        private static FileAttributes RemoveAttribute(FileAttributes attributes, FileAttributes attributesToRemove)
        {
            return attributes & ~attributesToRemove;
        }

        private static void CreateNewMenuItem(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
           
        }
    }
}
