using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PT_Lab8
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenClicked(object sender, RoutedEventArgs eventArgs)
        {
            using (var dlg = new System.Windows.Forms.FolderBrowserDialog())
            {
                var result = dlg.ShowDialog();
                
                if (result.ToString() == string.Empty) return;

                var root = GetDirectoryTree(dlg.SelectedPath);
                
                TreeView.Items.Add(root);
            }
        }

        private void ExitClicked(object sender, RoutedEventArgs eventArgs)
        {
            Application.Current.Shutdown();
        }

        private TreeViewItem GetDirectoryTree(string path)
        {
            var root = new TreeViewItem
            {
                Header = Path.GetFileName(path),
                Tag = path
            };

            root.PreviewMouseDown += DisplayRahsAttributes;
            
            var currentDirectoryFiles = Directory.GetFiles(path);
            var currentDirectoryDirectories = Directory.GetDirectories(path);

            foreach (var f in currentDirectoryFiles)
            {
                var cm = new ContextMenu();
                var open = new MenuItem
                {
                    Header = "Open",
                    Tag = f
                };

                var delete = new MenuItem
                {
                    Header = "Delete",
                    Tag = f
                };

                open.Click += OpenFileClicked;
                delete.Click += DeleteClicked;

                cm.Items.Add(open);
                cm.Items.Add(delete);

                var itemToAdd = new TreeViewItem()
                {
                    Header = Path.GetFileName(f),
                    Tag = f,
                    ContextMenu = cm
                };

                itemToAdd.PreviewMouseDown += DisplayRahsAttributes;
                root.Items.Add(itemToAdd);
            }

            foreach (var d in currentDirectoryDirectories)
            {
                var cm = new ContextMenu();
                var create = new MenuItem
                {
                    Header = "Create",
                    Tag = d
                };

                var delete = new MenuItem
                {
                    Header = "Delete",
                    Tag = d
                };
                
                create.Click += CreateFileClicked;
                delete.Click += DeleteClicked;

                cm.Items.Add(create);
                cm.Items.Add(delete);
                
                var item = GetDirectoryTree(d);
                item.ContextMenu = cm;
                root.Items.Add(item);
            }
            
            return root;
        }

        private void OpenFileClicked(object sender, EventArgs eventArgs)
        {
            var file = (MenuItem)sender;
            var path = (string)file.Tag;

            if (!File.Exists(path)) throw new Exception("File doesn't exist");

            if (Path.GetExtension(path) == ".txt")
            {
                TextDisplay.Text = File.ReadAllText(path);
            }
            else
            {
                var ib = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri(path))
                };
                TextDisplay.Background = ib;
            }
        }

        private void CreateFileClicked(object sender, EventArgs eventArgs)
        {
            var file = (MenuItem)sender;
            var path = (string)file.Tag;
            var cm = (ContextMenu)file.Parent;
            var root = (TreeViewItem)cm.PlacementTarget;
            
            
            var creation = new CreationWindow(path);
            creation.ShowDialog();

            var filePath = creation.GetFilePath();
            
            if(filePath == "") return;
            
            var itemToAdd = new TreeViewItem
            {
                Header = Path.GetFileName(filePath),
                Tag = filePath
            };

            itemToAdd.PreviewMouseDown += DisplayRahsAttributes;

            var upperOption = new MenuItem();
            var fi = new FileInfo(filePath);
            
            upperOption.Tag = filePath;

            if ((fi.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
            {
                upperOption.Header = "Create";
                upperOption.Click += CreateFileClicked;

            }
            else
            {
                upperOption.Header = "Open";
                upperOption.Click += OpenFileClicked;
            }

            var ncm = new ContextMenu();

            var delete = new MenuItem
            {
                Header = "Delete",
                Tag = filePath
            };
                
            delete.Click += DeleteClicked;

            ncm.Items.Add(upperOption);
            ncm.Items.Add(delete);

            itemToAdd.ContextMenu = ncm;
            
            root.Items.Add(itemToAdd);
        }
        
        private static void DeleteClicked(object sender, EventArgs eventArgs)
        {
            var file = (MenuItem)sender;
            var path = (string)file.Tag;
            
            var attributes = File.GetAttributes(path);

            if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                attributes = RemoveAttribute(attributes, FileAttributes.ReadOnly);
                File.SetAttributes(path, attributes);
                Console.WriteLine(@"The {0} file is no longer ReadOnly", path);
            }
            
            if (file.Parent is ContextMenu cm)
            {
                var item = cm.PlacementTarget as TreeViewItem;
                if (item?.Parent is TreeViewItem root) root.Items.Remove(item);
            }

            if ((attributes & FileAttributes.Directory) == FileAttributes.Directory)
            {
                Directory.Delete(path, true);
            }
            else
            {
                File.Delete(path);
            }
        }
        
        private void DisplayRahsAttributes(object sender, EventArgs eventArgs)
        {
            var file = (TreeViewItem)sender;
            var path = (string)file.Tag;

            var resultString = "";
            var fi = new FileInfo(path);

            if ((fi.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                resultString += "r";
            }
            else
            {
                resultString += "-";
            }
            
            if ((fi.Attributes & FileAttributes.Archive) == FileAttributes.Archive)
            {
                resultString += "a";
            }
            else
            {
                resultString += "-";
            }
            
            if ((fi.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
            {
                resultString += "h";
            }
            else
            {
                resultString += "-";
            }
            
            if ((fi.Attributes & FileAttributes.System) == FileAttributes.System)
            {
                resultString += "s";
            }
            else
            {
                resultString += "-";
            }

            RahsAttributes.Text = resultString;
        }

        private static FileAttributes RemoveAttribute(FileAttributes attributes, FileAttributes attributesToRemove)
        {
            return attributes & ~attributesToRemove;
        }

      
    }
}