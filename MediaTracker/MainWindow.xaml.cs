using System;
using System.Collections.Generic;
using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
using System.Windows.Input;
/*
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
*/
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
//using System.Timers;
using System.Threading;
using Button = System.Windows.Controls.Button;

namespace MediaTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool autoAdvanceOnOpen;

        private TreeViewItem[] savedItems;

        private TrackTree trackTree;
        private TrackTree selectedFolder;
        private TrackTree selectedFile;
        private TrackTree randomFile;
        private char keySearched;
        private Queue<TreeViewItem> keyQueue;

        private System.Timers.Timer searchTimer;

        public MainWindow()
        {
            // check if there an already running instance of the app, if so will alert the user and kill current instance
            if (System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1)
            {
                System.Windows.Forms.MessageBox.Show("Error, the application is already running","Failed to launch", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
            InitializeComponent();
            // load Left, Top, Width, Height, and Root
            this.Left = Properties.Settings.Default.Left;
            this.Top = Properties.Settings.Default.Top;
            this.Width = Properties.Settings.Default.Width;
            this.Height = Properties.Settings.Default.Height;
            string root = Properties.Settings.Default.Root;
            // set the settings
            this.autoAdvanceSetting.IsChecked = Properties.Settings.Default.AutoAdvance;
            this.autoAdvanceOnOpen = Properties.Settings.Default.AutoAdvance;
            this.showRandomSectionSetting.IsChecked = Properties.Settings.Default.ShowRandomSection;
            // if random file section is hidden, can have less minSize
            if (!Properties.Settings.Default.ShowRandomSection)
                this.showRandomSectionSetting_Checked(null, null);
            if (Directory.Exists(root))
                this.setRoot(root);
        }

        #region event functions

        #region window events

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
            Properties.Settings.Default.Height = this.SettingsControl.IsExpanded? this.Height-40 : this.Height;
            Properties.Settings.Default.Width = this.Width;
            Properties.Settings.Default.Root = this.rootTextBox.Text;
            // saves the settings
            Properties.Settings.Default.AutoAdvance = this.autoAdvanceSetting.IsChecked == true? true:false;
            Properties.Settings.Default.ShowRandomSection = this.showRandomSectionSetting.IsChecked == true ? true : false;

            Properties.Settings.Default.Save();
            // saves the tracker tree
            saveTrackTree();
        }

        /// <summary>
        /// focus on the items (in root) that start with the same char pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // if the search box is focues, do nothing
            if (this.searchTextBox.IsFocused)
                return;
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
            if (this.keyQueue==null || this.keyQueue.Count == 0)
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

        #region UI events

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
        /// calls the checkTrees function on the root, in order to refresh it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void refreshTreeView_Click(object sender, RoutedEventArgs e)
        {
            // check if the tree is up to date
            this.checkTrees(this.trackTree);
        }

        /// <summary>
        /// filter the root treeview items, shows only the items that their Header contains the search string
        /// if the search string is empty, shows all items
        /// </summary>
        /// <param name="sender">search textBox</param>
        /// <param name="e"></param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // check if the timer is initialized
            // if so will reset the timer
            if (this.searchTimer != null)
            {
                this.searchTimer.Stop();
                this.searchTimer.Start();
            }
            // else initialize time and start it
            else
            {
                // set the timer to start in a second
                this.searchTimer = new System.Timers.Timer(1000);
                // add the event
                this.searchTimer.Elapsed += (source, e) =>
                {
                    // set the event in a new thread, because the search may take some time
                    new Thread(() =>
                    {
                        // set to run in background
                        Thread.CurrentThread.IsBackground = true;
                        // lock the treeViewItems
                        lock (this.TreeView.Items)
                        {
                            // update gui
                            this.Dispatcher.Invoke(() =>
                            {
                                // no selected root
                                if (savedItems == null)
                                    return;
                                // get the search string
                                string search = ((System.Windows.Controls.TextBox)sender).Text;
                                var items = this.TreeView.Items;    // get the root itemCollection ref
                                // clear previous items in root
                                items.Clear();
                                // if the search string is empty, return the original list to root
                                if (string.IsNullOrEmpty(search))
                                {
                                    foreach (var item in savedItems)
                                        items.Add(item);
                                }
                                // else add only the items that contain the search string, ignore cases
                                else
                                {
                                    // readd the matching items to root
                                    foreach (TrackTree tree in this.trackTree.Childrens)
                                    {
                                        searchTreeView(TreeView.Items, tree, search.ToLower());
                                    }
                                }
                            });
                        }
                    }).Start(); // starts the thread
                    // stops the timer after the thread start, so the timer will only run once
                    this.searchTimer.Stop();
                };
                // start the timer after initialization, it will run in a second
                this.searchTimer.Start();
            }
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
        /// set the autoAdvance flag, by the checkbox value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void autoAdvanceSetting_Checked(object sender, RoutedEventArgs e)
        {
            this.autoAdvanceOnOpen = this.autoAdvanceSetting.IsChecked == true? true:false;
        }

        /// <summary>
        /// hide/show the randomFile section according to the checkbox, also update the minHeight
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showRandomSectionSetting_Checked(object sender, RoutedEventArgs e)
        {
            // update visibility
            this.randomSection.Visibility = (this.showRandomSectionSetting.IsChecked == true)? Visibility.Visible: Visibility.Hidden;
            // if visible, increse minHeight
            if (this.randomSection.Visibility == Visibility.Visible)
            {
                // added the condition so the size won't change untill the TreeView was loaded
                if (this.TreeView.Items.Count == 0)
                    return;
                this.MinHeight += 70;
                // check if the new window size is out of bottom screen
                checkOutOfBottomScreen();
            }
            // if hidden, decrese minHeight
            else if (this.randomSection.Visibility == Visibility.Hidden)
            {
                var prevMinHeight = this.MinHeight;
                this.MinHeight -= 70;
                // update actual height if already was at minHeight
                if (this.Height == prevMinHeight)
                    this.Height -= 70;
            }
        }

        /// <summary>
        /// update the minHeight upon expansion\collapse of the settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingsControl_StateChange(object sender, RoutedEventArgs e)
        {
            var prevMinHeight = this.MinHeight;
            // update minHeight
            this.MinHeight += (this.SettingsControl.IsExpanded == true)? 40 : -40;
            // check if the new window size is out of bottom screen
            if (this.MinHeight > prevMinHeight)
                checkOutOfBottomScreen();
            // update actual height if already was at minHeight
            if (this.Height==prevMinHeight)
                this.Height -= 40;
        }

        /// <summary>
        /// check if the window size is out of bottom screen,
        /// if so will move it to be in bounds
        /// </summary>
        private void checkOutOfBottomScreen()
        {

            if (this.Top + this.Height >= SystemParameters.VirtualScreenHeight)
            {
                this.Top = SystemParameters.VirtualScreenHeight + SystemParameters.VirtualScreenTop - this.Height - 30;
            }
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
            try
            {
                // if file's folder not found, throw exception, and update tree
                if (!Directory.Exists(selectedFile.Parent.Path))
                    throw new FileNotFoundException();

                // check if the folder need updating
                // if 2 layers up is the root, update file's folder
                if (this.selectedFile.Parent.Parent != this.trackTree)
                    this.checkTrees(this.selectedFile.Parent.Parent);
                // else update the root
                else
                    this.checkTrees(this.selectedFile.Parent);
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
            catch (Exception exp)
            {
                this.checkTrees(this.trackTree);
                selectFolder(this.trackTree.SelectedPath);
            }
        }

        /// <summary>
        /// moves the tracker's offset in ether in the selected file's folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClick_ChangeFile(object sender, RoutedEventArgs e)
        {
            if (this.trackTree == null || this.selectedFile == null)
                return;
            try
            {
                // if file not found, throw exception, and update tree
                if (!File.Exists(selectedFile.Path))
                    throw new FileNotFoundException();
                // check if the folder need updating
                checkTrees(this.selectedFile.Parent);
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
                    // saves the updated trackTree, in case of an unexpected crash
                    saveTrackTree();
                }
            }
            catch (Exception exp)
            {
                checkTrees(this.trackTree);
                selectFolder(this.trackTree.SelectedPath);
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
            } while (hasMultipleFiles > 1 && newRandom == this.randomFile);
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
                // if the sender is the openFile button, open the file and advance tracker to next file (if autoAdvance is checked)
                if (sender == this.OpenFileBtn)
                {
                    Process.Start("explorer.exe", selectedFile.Path);
                    if (this.autoAdvanceOnOpen)
                    {
                        // advance to next file
                        this.ButtonClick_ChangeFile(this.NextFileBtn, null);
                    }
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
                this.checkTrees(this.trackTree);
                selectFolder(this.trackTree.SelectedPath);
            }
        }

        #endregion

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
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                Dispatcher.Invoke(() =>
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
                    // check if the tree needs updating
                    trackTree.checkChildren(false);
                    // set the children
                    trackTree.Childrens.ForEach((child) => setItems(TreeView.Items, child));
                    // saves the new tree
                    this.savedItems = new TreeViewItem[this.TreeView.Items.Count];
                    this.TreeView.Items.CopyTo(savedItems, 0);
                    // set the selected folder as the one the root is pointing at
                    selectFolder(this.trackTree.SelectedPath);
                });
            }).Start();
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
                // if the selected file not found, throw exception
                if (!Directory.Exists(path) && !File.Exists(path))
                    throw new FileNotFoundException();
                // if not a directory, do nothing
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
                this.checkTrees(this.trackTree);
                selectFolder(this.trackTree.SelectedPath);
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
            // if the item is a directory, add events
            if (tree.IsDirectory)
            {
                // add selectFolder events
                item.PreviewMouseLeftButtonDown += (sender, e) => { selectFolder(tree.Path); };
                item.KeyDown += (sender, e) => {
                    if (e.Key.Equals(Key.Enter))
                        selectFolder(tree.Path);
                };
                // add null item for the expanded event
                item.Items.Add(null);
                // add expanded event
                item.Expanded += (sender, e) =>
                {
                    // if already expanded, do nothing
                    if (item.Items.Count != 1 || item.Items[0] != null)
                        return;
                    // remove null item
                    item.Items.RemoveAt(0);
                    // add real children
                    tree.Childrens.ForEach((child) =>
                    {
                        setItems(item.Items, child);
                    });
                };
            }
            // else the item is a file, set the file events (open file via explorer.exe)
            else
            {
                item.MouseDoubleClick += this.onItemDoubleClick;
                item.KeyDown += this.onItemEnter;
            }
        }

        /// <summary>
        /// search the treeView for the item with the given path
        /// using the overloaded method for recursive search
        /// </summary>
        /// <param name="path"></param>
        /// <returns>TreeViewItem if the item was found, else null</returns>
        private TreeViewItem searchItem(string path)
        {
            foreach (TreeViewItem item in this.TreeView.Items)
            {
                var res = this.searchItem(item, path);
                if (res != null)
                    return res;
            }
            return null;
        }

        /// <summary>
        /// recursive search for the given path, if this item isn't it, search its children
        /// </summary>
        /// <param name="item"></param>
        /// <param name="path"></param>
        /// <returns>TreeViewItem if the item was found, else null</returns>
        private TreeViewItem searchItem(TreeViewItem item, string path)
        {
            if (item == null)
                return null;
            // if is the right item, return it
            if (item.Tag.ToString().Equals(path))
                return item;
            // else search the children
            foreach (TreeViewItem node in item.Items)
            {
                var res = this.searchItem(node, path);
                if (res != null)
                    return res;
            }
            return null;
        }

        /// <summary>
        /// update the treeViewItem with the children of the trackTree
        /// </summary>
        /// <param name="item"></param>
        /// <param name="tree"></param>
        private void updateTreeView(TreeViewItem item, TrackTree tree)
        {
            // clear previous subItems
            item.Items.Clear();
            // update the item
            tree.Childrens.ForEach((child) =>
            {
                this.setItems(item.Items, child);
            });
        }

        /// <summary>
        /// check if the trackTree need updating, if so will also update its opposite treeViewItem,
        /// if the tracktree is the root, and needed updating, will update the whole tree
        /// </summary>
        /// <param name="trackTree"></param>
        private bool checkTrees(TrackTree trackTree)
        {
            if (trackTree.checkChildren(true))
            {
                // if not the root, update the node
                if (trackTree != this.trackTree)
                {
                    TreeViewItem item = searchItem(trackTree.Path);
                    if (item != null)
                        this.updateTreeView(item, trackTree);
                }
                // else update the whole tree
                else
                {
                    // clear previous subItems
                    this.TreeView.Items.Clear();
                    // update the item
                    trackTree.Childrens.ForEach((child) =>
                    {
                        this.setItems(this.TreeView.Items, child);
                    });
                }
                // if the search textBox is empty, and the tree was updated, save the updated tree
                if (string.IsNullOrEmpty(this.searchTextBox.Text))
                {
                    this.savedItems = new TreeViewItem[this.TreeView.Items.Count];
                    this.TreeView.Items.CopyTo(savedItems, 0);
                }
                // saves the updated trackTree
                saveTrackTree();
                // return true, the trees were updated
                return true;
            }
            // no update was needed
            return false;
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
        private void saveTrackTree()
        {
            // the trackTree isn't initialize, do nothing, return true
            if (this.trackTree == null)
                return;
            // starts a new thread for the save
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                lock (this.trackTree)
                {
                    this.trackTree.save($"{this.trackTree.Path}/tracker.dat");
                }
            }).Start();
        }

        /*
        /// <summary>
        /// start the search of savedItems for all items with the matching header
        /// </summary>
        /// <param name="searchString"></param>
        /// <returns></returns>
        private List<TreeViewItem> searchTreeView(string searchString)
        {
            // initialize list
            var list = new List<TreeViewItem>();
            // start the search of all root files
            foreach (TreeViewItem item in this.savedItems)
                list.AddRange(searchTreeView(item, searchString.ToLower()));
            // return result
            return list;
        }
        */

        /// <summary>
        /// if the current item or one of its children matches the search string given, will add the file to the parent's collection
        /// </summary>
        /// <param name="item"></param>
        /// <param name="searchString"></param>
        /// <returns></returns>
        private bool searchTreeView(ItemCollection parent, TrackTree tree, string searchString)
        {
            bool flag = false;
            var clone = cloneItem(tree);
            // if current item's header matching, add to list
            if (tree.Name.ToLower().Contains(searchString)) 
                flag = true;
            // search all subItems
            foreach (TrackTree childTree in tree.Childrens)
                flag |= searchTreeView(clone.Items, childTree, searchString);
            if (flag)
            {
                parent.Add(clone);
            }
            // return result
            return flag;
        }

        /// <summary>
        /// clone the treeViewItem given and set its events
        /// </summary>
        /// <param name="item"></param>
        /// <returns>TreeViewItem clone</returns>
        private TreeViewItem cloneItem(TrackTree tree)
        {
            // init clone
            var clone = new TreeViewItem()
            {
                Header = tree.Name,
                Tag = tree.Path
            };
            // if file, set open on events
            if (File.Exists(tree.Path))
            {
                clone.MouseDoubleClick += this.onItemDoubleClick;
                clone.KeyDown += this.onItemEnter;
            }
            // if directory, add select folder events
            else if (Directory.Exists(tree.Path))
            {
                clone.PreviewMouseLeftButtonDown += (sender, e) => { selectFolder(tree.Path); };
                clone.KeyDown += (sender, e) => {
                    if (e.Key.Equals(Key.Enter))
                        selectFolder(tree.Path);
                };
            }
            return clone;
        }

        /// <summary>
        /// on double click open the treeViewItem file via explorer
        /// </summary>
        /// <param name="sender">TreeViewItem</param>
        /// <param name="e"></param>
        private void onItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Process.Start("explorer.exe", ((TreeViewItem)sender).Tag.ToString());
            }
            catch (Exception ex) { }
        }

        /// <summary>
        /// on enter key open the treeViewItem file via explorer
        /// </summary>
        /// <param name="sender">TreeViewItem</param>
        /// <param name="e"></param>
        private void onItemEnter(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                if (e.Key.Equals(Key.Enter))
                    Process.Start("explorer.exe", ((TreeViewItem)sender).Tag.ToString());
            }
            catch (Exception ex) { }
        }

        #endregion
    }
}
