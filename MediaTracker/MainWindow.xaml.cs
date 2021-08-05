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
using Button = System.Windows.Controls.Button;

namespace MediaTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private TreeViewItem[] savedItems;

        private TrackTree trackTree;
        private TrackTree selectedFolder;
        private TrackTree selectedFile;
        private TrackTree randomFile;
        private char keySearched;
        private Queue<TreeViewItem> keyQueue;

        public MainWindow()
        {
            InitializeComponent();
            // load Left, Top, Width, Height, and Root
            this.Left = Properties.Settings.Default.Left;
            this.Top = Properties.Settings.Default.Top;
            this.Width = Properties.Settings.Default.Width;
            this.Height = Properties.Settings.Default.Height;
            string root = Properties.Settings.Default.Root;
            if (Directory.Exists(root))
                this.setRoot(root);
        }

        #region event functions

        /// <summary>
        /// open a folder browser dialog, set the new root as the one selected, and saves the old treeTracker beforehand
        /// </summary>
        /// <param name="sender">selectRoot button</param>
        /// <param name="e"></param>
        private void selectRootBtn_Click(object sender, RoutedEventArgs e)
        {
            // init dialog
            var dialog = new FolderBrowserDialog();
            // if selected folder set path
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                setRoot(dialog.SelectedPath);
            else
                return; // haven't selected, end method
        }

        /// <summary>
        /// filter the root treeview items, shows only the items that their Header contains the search string
        /// if the search string is empty, shows all items
        /// </summary>
        /// <param name="sender">search textBox</param>
        /// <param name="e"></param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // no selected root
            if (savedItems == null)
                return;
            // get the search string
            string search = ((System.Windows.Controls.TextBox)sender).Text;
            var items = this.TreeView.Items;    // get the items reference
            // clear selected items
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

        /// <summary>
        /// before closing the window, save the window position, size, and the trackTree
        /// </summary>
        /// <param name="sender">main Window</param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // saves the window position and size
            Properties.Settings.Default.Top = this.Top;
            Properties.Settings.Default.Left = this.Left;
            Properties.Settings.Default.Height = this.Height;
            Properties.Settings.Default.Width = this.Width;
            Properties.Settings.Default.Root = this.rootTextBox.Text;
            Properties.Settings.Default.Save();
            // saves the tracker tree
            saveTrackTree();
        }

        /// <summary>
        /// update the text in the textBoxes
        /// </summary>
        private void updateText()
        {
            this.selectedFolderText.Text = this.selectedFolder.Name;
            this.selectedFileText.Text = this.selectedFile.Name;
            this.selectedFileFolderText.Text = this.selectedFile.Parent.Name;
        }

        /// <summary>
        /// focus on the items (in root) that start with the same char pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // get typed char
            char key;
            Char.TryParse(e.Key.ToString(), out key);
            TreeViewItem tvi;
            // if a new key was typed, get all the items that start with the char
            if (this.keySearched != key)
            {
                this.keySearched = key;
                // if the queue is null, initialize it
                if (this.keyQueue == null)
                    this.keyQueue = new Queue<TreeViewItem>();
                // else clear it
                else
                    this.keyQueue.Clear();
                // enqueue the items
                foreach (var item in this.TreeView.Items)
                {
                    tvi = (TreeViewItem)item;
                    if (tvi.Header.ToString().ToLower()[0] == char.ToLower(this.keySearched))
                        this.keyQueue.Enqueue(tvi);
                }
            }
            // if the queue is empty, return
            if (this.keyQueue.Count == 0)
                return;
            // get the head
            tvi = this.keyQueue.Dequeue();
            // get focus on the head
            tvi.Focus();
            this.selectFolder(tvi.Tag.ToString());
            // return head to tail
            this.keyQueue.Enqueue(tvi);
        }

        #endregion

        #region control buttons events

        /// <summary>
        /// moves the tracker's offset in ether in the selected file's folder's folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClick_ChangeFolder(object sender, RoutedEventArgs e)
        {
            // if trackTree or selectedFile is null, do nothing
            if (this.trackTree == null || this.selectedFile == null)
                return;
            // check if the folder need updating
            // if 2 layers up is the root, update file's folder
            if (this.selectedFile.Parent.Parent != this.trackTree)
                this.selectedFile.Parent.Parent.checkChildren();
            // else update the root
            else
                this.selectedFile.Parent.checkChildren();
            var btn = (Button)sender;
            bool atBounds = false;
            // called by nextFolder button, offset +1
            if (btn.Name.Equals(this.NextFolderBtn.Name))
            {
                // if the selected file is not 2 layers under root, change the tracker 2 layers up tracker
                if (this.selectedFile.Parent.Parent != this.trackTree)
                    atBounds = moveTracker(this.selectedFile.Parent.Parent, 1);
                // if the selected file is 2 layers under root, change the file's folder's tracker
                else
                    atBounds = moveTracker(this.selectedFile.Parent, 1);
            }
            // called by previousFolder button, offset -1
            else if (btn.Name.Equals(this.PreviousFolderBtn.Name))
            {
                // if the selected file is not 2 layers under root, change the tracker 2 layers up tracker
                if (this.selectedFile.Parent.Parent != this.trackTree)
                    atBounds = moveTracker(this.selectedFile.Parent.Parent, -1);
                // if the selected file is 2 layers under root, change the file's folder's tracker
                else
                    atBounds = moveTracker(this.selectedFile.Parent, -1);
            }
            // if not at bounds, update selected file, and folder
            if (!atBounds)
            {
                // update selected file
                this.selectedFile = this.selectedFile.Parent.Parent.getSelectedTree();
                // set the textBox texts
                this.updateText();
            }
        }

        /// <summary>
        /// moves the tracker's offset in ether in the selected file's folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClick_ChangeFile(object sender, RoutedEventArgs e)
        {
            // if trackTree or selectedFile is null, do nothing
            if (this.trackTree == null || this.selectedFile == null)
                return;
            // check if the folder need updating
            this.selectedFile.Parent.checkChildren();
            var btn = (Button)sender;
            bool atBounds = false;
            // called by nextFile button, offset +1
            if (btn.Name.Equals(this.NextFileBtn.Name))
            {
                atBounds = moveTracker(this.selectedFile.Parent, 1);
            }
            // called by next5File button, offset +5
            else if (btn.Name.Equals(this.Next5FileBtn.Name))
            {
                atBounds = moveTracker(this.selectedFile.Parent, 5);
            }
            // called by previousFile button, offset -1
            else if (btn.Name.Equals(this.PreviousFileBtn.Name))
            {
                atBounds = moveTracker(this.selectedFile.Parent, -1);
            }
            // called by previous5File button,, offset -5
            else if (btn.Name.Equals(this.Previous5FileBtn.Name))
            {
                atBounds = moveTracker(this.selectedFile.Parent, -5);
            }
            // if not at bounds, update selected file, and folder
            if (!atBounds)
            {
                // update selected file
                this.selectedFile = this.selectedFile.Parent.getSelectedTree();
                // set the textBox texts
                this.updateText();
            }
        }

        /// <summary>
        /// get a random file from the currently selected folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GenRandomBtn_Click(object sender, RoutedEventArgs e)
        {
            // if the trackTree isn't initialize, do nothing
            if (this.trackTree == null)
                return;
            TrackTree newRandom;
            int hasMultipleFiles = this.selectedFolder.Childrens.Count;
            do
            {
                newRandom = this.selectedFolder.getRandom();
            } while (hasMultipleFiles>1 && newRandom == this.randomFile);
            this.randomFile = newRandom;
            this.RandomText.Text = this.randomFile.Name;
        }

        /// <summary>
        /// Open file method, can be called by openFile, openFolder, openRandom buttons
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClick_open(object sender, RoutedEventArgs e)
        {
            // if the trackTree isn't initialize, do nothing
            if (this.trackTree == null)
                return;
            // if attenpt to open failed, do nothing
            try
            {
                // if the sender is the openFile button, open the file and advance tracker to next file
                if (sender == this.OpenFileBtn)
                {
                    Process.Start("explorer.exe", selectedFile.Path);
                    this.ButtonClick_ChangeFile(this.NextFileBtn, null);
                }
                // else if it was the openFolder button, open the file's folder (parent tree path)
                else if (sender == this.OpenFolderBtn)
                    Process.Start("explorer.exe", selectedFile.Parent.Path);
                // else if it is openRandom, will open randomFile path
                else if (this.randomFile != null && sender == this.OpenRandomBtn)
                    Process.Start("explorer.exe", randomFile.Path);
            }
            catch (Exception exception)
            {
                // if an exception was thrown, check if the tree is up to date
                this.trackTree.checkTree();
                selectFolder(this.trackTree.Path);
            }
        }

        #endregion

        #region TreeView methods

        /// <summary>
        /// set the new root, clear all previous items,
        /// the method fetches all the valid files and folders with size greater or equals to 20mb,
        /// save the new items (valid files\folders) to savedItems, in case of a search input,
        /// get the new root trackTree, if there isn't one it will be created and saved, and set the selected folder as the new root
        /// </summary>
        /// <param name="path">new root path</param>
        private void setRoot(string path)
        {
            path = Utilties.fixPath(path);
            // clear previous items
            TreeView.Items.Clear();
            // set the textBox text
            this.rootTextBox.Text = path;

            // saves the old trackTree
            this.saveTrackTree();

            // load tracker, stored in root, if none found, create one and save
            this.trackTree = TrackTree.load($"{path}/tracker.dat");
            // if the load returned null (no trackTree)
            if (this.trackTree == null)
            {
                this.trackTree = new TrackTree(null, path);
                this.saveTrackTree();
            }

            this.trackTree.Childrens.ForEach((child) => this.setItems(this.TreeView.Items, child));

            // saves the new tree
            this.savedItems = new TreeViewItem[this.TreeView.Items.Count];
            this.TreeView.Items.CopyTo(savedItems, 0);
            // set the selected folder as the one the root is pointing at
            selectFolder(this.trackTree.SelectedPath);
        }

        /// <summary>
        /// set the selected folder as the given path,
        /// get its selectedly selected (saved in trackList) tracked path
        /// if the selected folder is not the root, will navigate the root trackTree to the selected selected
        /// </summary>
        /// <param name="path"></param>
        private void selectFolder(string path)
        {
            path = Utilties.fixPath(path);
            try
            {
                if (!File.GetAttributes(path).HasFlag(FileAttributes.Directory))
                    return;
                // get selected tree and its selected
                this.selectedFolder = this.trackTree.search(path);
                this.selectedFile = this.selectedFolder.getSelectedTree();
                // set the textBox texts
                this.updateText();
                // set focus to openFile button
                this.OpenFileBtn.Focus();

                // if not root, changed selected in parent
                if (selectedFolder.Path != trackTree.Path)
                {
                    // update the selected path
                    this.trackTree.setPathTo(selectedFolder);
                    // get generate random file from selected folder
                    this.GenRandomBtn_Click(null, null);
                }
            } catch (Exception exp)
            {
                // if an exception was thrown, check if the tree is up to date
                this.trackTree.checkTree();
                selectFolder(this.trackTree.Path);
            }
        }

        /// <summary>
        /// creates a TreeViewItem with given path, add it to the parent's collection,
        /// if the item is a directory, add all it's valid children and set on left click event (selectFolder),
        /// else the item is a file, set on double click event to open the file via explorer.exe
        /// </summary>
        /// <param name="parent">the parent's itemCollection to add the new item to</param>
        /// <param name="path">the path of the new item</param>
        private void setItems(ItemCollection parent, TrackTree tree)
        {
            // initialize the path treeViewItem, header is the name, tag the full path
            TreeViewItem item = new TreeViewItem()
            {
                Header = tree.Name,
                Tag = tree.Path
            };
            // add the item to the parent
            parent.Add(item);
            // if the item is a directory, add all its children with size above 20mb, and set on left click event (selectFolder)
            if (tree.IsDirectory)
            {
                item.PreviewMouseLeftButtonDown += (sender, e) => { selectFolder(tree.Path); };
                item.KeyDown += (sender, e) => { 
                    if (e.Key.Equals(Key.Enter))
                        selectFolder(tree.Path);
                };
                tree.Childrens.ForEach((child) =>
                {
                    setItems(item.Items, child);
                });
            }
            // else the item is a file, set the double click event (open file via explorer.exe)
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
                item.KeyDown += (sender, e) =>
                {
                    try
                    {
                        if (e.Key.Equals(Key.Enter))
                            Process.Start("explorer.exe", ((TreeViewItem)sender).Tag.ToString());
                    }
                    catch (Exception ex) { }
                };
            }
        }

        /// <summary>
        /// calls the move method in the tree's tracker
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private bool moveTracker(TrackTree tree, int offset)
        {
            return tree.Tracker.move(offset);
        }

        /// <summary>
        /// saves the trackTree in a .dat file which will be located in the root path
        /// </summary>
        /// <returns>true if saved successfully, else false</returns>
        private bool saveTrackTree()
        {
            // the trackTree isn't initialize, do nothing, return true
            if (this.trackTree == null)
                return true;
            // save
            return this.trackTree.save($"{this.trackTree.Path}/tracker.dat");
        }

        #endregion

    }
}
