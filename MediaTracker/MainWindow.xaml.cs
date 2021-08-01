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
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;


namespace MediaTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private TreeViewItem[] savedItems;

        private TrackTree trackTree;
        private TrackTree selected;
        private TrackTree selectedCurrent;

        public MainWindow()
        {
            InitializeComponent();
            // load Left, and Top, Root, and TreeView
            // load trackTree
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            /*
            var dialog = new FolderBrowserDialog();
            string path = "";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                path = dialog.SelectedPath;
            var item = new TreeViewItem()
            {
                Header = new FileInfo(path).Name,
                Tag = path
            };
            TreeView.Items.Add(item);
            Utilties.getAllFilesAbove(path, 20).ForEach((child) => setItems(item, child));
            */
        }

        private void setItems(ItemCollection parent, string path)
        {
            var info = new FileInfo(path);
            TreeViewItem item = new TreeViewItem()
            {
                Header = string.IsNullOrEmpty(info.Name) ? path : info.Name,
                Tag = Utilties.fixPath(path)
            };
            parent.Add(item);
            if (info.Attributes.HasFlag(FileAttributes.Directory))
            {
                item.PreviewMouseLeftButtonDown += (sender, e) => { selectFolder(path); };
                Utilties.getAllFilesAbove(path,20).ForEach((file)=>
                {
                    setItems(item.Items, file);
                });
            }
            else
            {
                item.MouseDoubleClick += (sender, e) =>
                {
                    try
                    {
                        Process.Start("explorer.exe", ((TreeViewItem)sender).Tag.ToString());
                    }
                    catch (Exception ex) { }
                };
            }
        }

        private void selectRootBtn_Click(object sender, RoutedEventArgs e)
        {
            // init dialog
            var dialog = new FolderBrowserDialog();
            string path = "";
            // if selected folder set path
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                setRoot(dialog.SelectedPath);
            else
                return; // haven't selected, end method
        }

        private void setRoot(string path)
        {
            path = Utilties.fixPath(path);
            // clear previous items
            TreeView.Items.Clear();
            // set the children
            Utilties.getAllFilesAbove(path, 20).ForEach((child) => setItems(this.TreeView.Items, child));
            // set the textBox text
            this.rootTextBox.Text = path;

            // saves the old trackTree
            this.saveTrackTree();

            // saves the new tree
            this.savedItems = new TreeViewItem[this.TreeView.Items.Count];
            this.TreeView.Items.CopyTo(savedItems, 0);

            // load tracker, stored in root, if none found, create one and save
            this.trackTree = TrackTree.load($"{path}/tracker.dat");
            // if the load returned null (no trackTree)
            if (this.trackTree == null)
            {
                this.trackTree = new TrackTree(null, path);
                this.trackTree.save($"{path}/tracker.dat");
            }

            selectFolder(path);
        }

        private void selectFolder(string path)
        {
            path = Utilties.fixPath(path);
            if (!File.GetAttributes(path).HasFlag(FileAttributes.Directory))
                return;
            // get selected tree and its current
            this.selected = this.trackTree.search(path);
            this.selectedCurrent = this.selected.getCurrentTree();

            this.selectedFolderText.Text = this.selected.Name;
            this.selectedFileText.Text = this.selectedCurrent.Name;

            // if not root, changed selected in parent
            if (selected.Path != trackTree.Path)
            {
                this.trackTree.setPathTo(selected);
                saveTrackTree();
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // no selected root
            if (savedItems == null)
                return;
            // get the search string
            string search = ((System.Windows.Controls.TextBox)sender).Text;
            var items = this.TreeView.Items;    // get the items reference
            // clear current items
            items.Clear();
            // if the search string is empty, return to original list
            if (string.IsNullOrEmpty(search))
            {
                foreach (var item in savedItems)
                    items.Add(item);
            }
            // else add only the items that contain the search string, ignore cases
            else
            {
                foreach (var item in savedItems)
                {
                    //System.Diagnostics.Debug.WriteLine($"{item.Header.ToString()} ? {search} = {item.Header.ToString().Contains(search)}");
                    if (item.Header.ToString().ToLower().Contains(search.ToLower()))
                        items.Add(item);
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            saveTrackTree();
        }

        private bool saveTrackTree()
        {
            if (this.trackTree == null)
                return true;
            return this.trackTree.save($"{this.trackTree.Path}/tracker.dat");
        }
    }
}
