using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace MediaTracker
{
    /// <summary>
    /// A class that represents a File\Directory in the file system,
    /// the class contains all the sub trees, and to which it is pointing
    /// </summary>
    class TrackTree
    {
        #region properties

        /// <summary>
        /// the parent's tree
        /// </summary>
        public TrackTree Parent { private set; get; }
        /// <summary>
        /// all the children of this tree,
        /// if this is a directroy the children are all the files\subDirectories that are greater then 20mb
        /// </summary>
        public List<TrackTree> Childrens { private set; get; }
        /// <summary>
        /// this tree's TrackerList object
        /// </summary>
        public TrackerList Tracker { private set; get; }
        /// <summary>
        /// the name of this tree's path, e.g., C:\path....\abc, the name is "abc"
        /// </summary>
        public string Name { private set; get; }
        /// <summary>
        /// the full path of this tree
        /// </summary>
        public string FilePath { private set; get; }
        /// <summary>
        /// the selected path of this tree, to which path this tree points at
        /// </summary>
        public string SelectedPath { get {
                return (this.Tracker!=null)? this.Tracker.SelectedPath : this.Name;
            } }
        /// <summary>
        /// the selected trackTree of this tree, to which tree this tree points at
        /// </summary>
        public TrackTree SelectedTree
        {
            get
            {
                var tree = this.Childrens.Find((child) =>
                {
                    return this.SelectedPath.Equals(child.FilePath);
                });
                return tree != null ? tree : this;
            }
        }
        /// <summary>
        /// if this tree is a directory, else it is a file
        /// </summary>
        public bool IsDirectory { private set; get; }
        /// <summary>
        /// if this tree is a directory and if it is empty, if this tree is a file then return false
        /// </summary>
        public bool IsEmpty { get { return IsDirectory && Tracker.IsEmpty; } }

        #endregion

        #region constractors

        /// <summary>
        /// initialize the tree with the the name given, and the parent's info,
        /// if the path is a directory will initiate the children also
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        private TrackTree(TrackTree parent, string name)
        {
            // inititalize properties
            this.Parent = parent;
            this.Name = name;
            this.FilePath = (parent!=null)? $"{parent.FilePath}/{Name}":Name;
            // fix path, replace '/' with '\'
            this.FilePath = Utilties.fixPath(this.FilePath);
            this.Childrens = new List<TrackTree>(); // initalize children list
            // initialize IsDirectory flag
            this.IsDirectory = File.GetAttributes(this.FilePath).HasFlag(FileAttributes.Directory);
            // if directory initialize FilesList and children trees
            if (IsDirectory)
            {
                this.Tracker = new TrackerList(FilePath);   // get files list
                // initalize children trees
                Tracker.FilesInfo.ForEach((file) => this.Childrens.Add(new TrackTree(this, file.Name)));
            }
        }

        /**
         * old load constructor used that was used in the old load function
        /// <summary>
        /// initialize the tree with from the reader data, and the parent's info,
        /// if the path is a directory will initiate the children with the reader
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="reader"></param>
        private TrackTree(TrackTree parent, BinaryReader reader, string root)
        {
            // if at end of stream, return
            if (reader.BaseStream.Position == reader.BaseStream.Length)
                return;
            // inititalize parent tree
            this.Parent = parent;
            // read the name
            this.Name = reader.ReadString();
            if (parent == null)
                this.Name = root;
            // build the tree Path
            this.FilePath = (parent != null) ? $"{parent.FilePath}\\{Name}" : Name;
            // if the folder/file no longer exists, return
            if (!Directory.Exists(this.FilePath) && !File.Exists(this.FilePath))
                return;
            this.Childrens = new List<TrackTree>(); // initalize children list
            // read IsDirectory flag
            this.IsDirectory = reader.ReadBoolean();
            // if IsDirectory, initialize children (via reader) and tracker
            if (IsDirectory)
            {
                this.Tracker = new TrackerList(FilePath,reader.ReadString());
                int count = reader.ReadInt32();
                for (int ind = 0; ind < count; ind++)
                {
                    try
                    {
                        var childTracker = new TrackTree(this, reader,null);
                        // if the child no longer exists, or is null, continue to the next
                        if (childTracker == null || (!Directory.Exists(childTracker.FilePath) && !File.Exists(childTracker.FilePath)))
                            continue;
                        this.Childrens.Add(childTracker);
                    } catch (Exception ex) { }
                }
            }
        }
        */

        /// <summary>
        /// a constructor used for the static load function, for a file
        /// </summary>
        /// <param name="parent">the parent's path</param>
        /// <param name="name">the file's name</param>
        private TrackTree(string parent, string name)
        {
            // create the path
            this.FilePath = $"{parent}\\{name}";
            this.Name = name;
            // initalize children list
            this.Childrens = new List<TrackTree>();
            // if the path does not exists, throw exception
            if (!Directory.Exists(this.FilePath) && !File.Exists(this.FilePath))
                throw new FileNotFoundException("File not found, can not create TrackTree");
        }
        /// <summary>
        /// a constructor used for the static load function, for a directory
        /// </summary>
        /// <param name="parent">the parent's path</param>
        /// <param name="name">the directory's name</param>
        /// <param name="selected">the selected file's name</param>
        private TrackTree(string parent, string name, string selected) : this(parent,name)
        {
            // only directories have selected
            this.IsDirectory = true;
            // if directory, initialize tracker
            this.Tracker = new TrackerList(this.FilePath, selected);
        }

        #endregion

        #region methods

        /// <summary>
        /// searching the tree and its children for the given path, if found will return the TrackTree with the matching path, else null
        /// </summary>
        /// <param name="path">the path of the desired tree</param>
        /// <returns>TrackTree with the same path, if non found will return null</returns>
        public TrackTree search(string path)
        {
            // if this.Path equal the path given, return true
            if (this.FilePath.Equals(path))
                return this;
            // if IsDirectory is false, or this.Path isn't in the path given, return false, the given path is not in this route
            else if (!IsDirectory || !path.Contains(this.FilePath))
                return null;
            TrackTree result = null;  // end result of children
            // search each child for the path
            foreach (var child in Childrens)
            {
                // get the search result of child
                result = child.search(path);
                // if the result isn't null, then the route is via the child
                if (result != null)
                    return result;  // returning the result tree
            }
            return result;  // returning children search result
        }

        /// <summary>
        /// setting the path to the destination tree given,
        /// if the destination was found, point each parent's Tracker.Selected to the child along the path
        /// </summary>
        /// <param name="dest"></param>
        /// <returns>true if the tree contains the destination, else false</returns>
        public bool setPathTo(TrackTree dest)
        {
            // if this is the destination, return true
            if (dest == this)
                return true;
            // else if this is a file, or the destination does not comtains the this.Path, return false
            // the destination is not on this tree, no need to continue search
            else if (!IsDirectory || !dest.FilePath.Contains(this.FilePath))
                return false;
            // search each child for the one that contain the destination
            foreach (var child in Childrens)
            {
                // if the child returned true, the destination is him or on his tree
                if (child.setPathTo(dest))
                {
                    // point this.Tracker.selected at the child
                    this.Tracker.SelectedPath = child.FilePath;
                    // returning true, that this tree contains the path
                    return true;
                }
            }
            // no child is or contain the destination, return false
            return false;
        }

        /// <summary>
        /// check if the children in this tree are up to date, if there are some that missing
        /// </summary>
        /// <param name="updateChildren">if true, update children also</param>
        /// <returns>true if an updated was needed, else false</returns>
        public bool checkChildren(bool updateChildren)
        {
            if (!this.IsDirectory)
                return false;
            bool updateNeeded = false;
            // update this tracker
            this.Tracker = new TrackerList(this.FilePath, this.SelectedTree.Name);
            // init lists
            List<string> toAdd = new List<string>();
            List<string> toRemove = new List<string>();
            List<TrackTree> toUpdate = new List<TrackTree>();
            // get all current files and directories
            toAdd.AddRange(Utilties.getAllFilesAbove(this.FilePath, 4));
            // filter all children into 
            Childrens.ForEach((child) =>
            {
                // child still exists, check if need updating
                if (toAdd.Contains(child.FilePath))
                {
                    // no need to reAdd the child
                    toAdd.Remove(child.FilePath);
                    // to to update list
                    toUpdate.Add(child);
                }
                else
                    // the child not found, to be removed later
                    toRemove.Add(child.FilePath);
            });
            // add missing paths
            toAdd.ForEach((path) => this.Childrens.Add(new TrackTree(this, Utilties.getName(path))));
            // remove unFound paths
            Childrens.RemoveAll((child) => { return toRemove.Contains(child.FilePath); });
            // if updateChildren is true update already exist children, if one of the children needed updating, return true
            if (updateChildren)
                toUpdate.ForEach((child) => updateNeeded |= child.checkChildren(updateChildren));
            // if a new child was added, resort children
            if (toAdd.Count > 0)
                this.Childrens.Sort((childA, childB) =>
                {
                    return childA.Name.ToLower().CompareTo(childB.Name.ToLower());
                });
            // tree updated, return true if one of the children needed updating or this tree needed
            return updateNeeded | (toAdd.Count>0 || toRemove.Count>0);
        }

        #region get selected\random methods

        /// <summary>
        /// returning the string of end of the line selected path,
        /// if this.Tracker.Selecte equals "EMPTY" returning this.Path
        /// </summary>
        /// <returns>string</returns>
        public string getSelected()
        {
            // if this tree is a of a file, or the directory is empty, return this.Path
            if (!IsDirectory || this.IsEmpty)
                return this.FilePath;
            // find the selected child, and get it's selected, return end result
            return this.SelectedTree.getSelected();
        }

        /// <summary>
        /// returning the tree of end of the line selected path,
        /// if this.Tracker.Selected equals "EMPTY" returning this.Path
        /// </summary>
        /// <returns></returns>
        public TrackTree getSelectedTree()
        {
            // if this tree is a of a file, or the directory is empty, return this tree
            if (!IsDirectory || this.IsEmpty)
                return this;
            // find the selected child, and get it's selected, return end result
            return this.SelectedTree.getSelectedTree();
        }

        /// <summary>
        /// if this tree is a file, return this, else return a random child.
        /// </summary>
        /// <returns>random file in the tree</returns>
        public TrackTree getRandom()
        {
            if (!IsDirectory || this.IsEmpty)
                return this;
            return this.Childrens[new Random().Next(this.Childrens.Count)].getRandom();
        }

        #endregion

        #region save

        public bool save()
        {
            try
            {
                // get the directory of the current root in appData saves, if there is non it will be created
                var dir = Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MediaTracker", Utilties.toHexString(this.FilePath)));
                // get all the dir files
                var fiArray = dir.GetFiles();
                // sort them by creationDate
                Array.Sort(fiArray, (a, b) =>
                {
                    return a.CreationTime.CompareTo(b.CreationTime);
                });
                BinaryWriter writer;
                // if the latest file wasn't created today, create new file
                if (fiArray.Length == 0 || fiArray[fiArray.Length - 1].CreationTime.CompareTo(DateTime.Today) < 0)
                {
                    // create the new file
                    var saveFile = File.Create(Path.Combine(dir.FullName, $"{DateTime.Today.ToString("yyyy-MM-dd")} Tracker.dat"));
                    // init writer with the new file
                    writer = new BinaryWriter(saveFile);
                }
                else
                    // init writer with today's file
                    writer = new BinaryWriter(fiArray[fiArray.Length - 1].OpenWrite());

                // saves the final selected, for the user
                writer.Write($"{this.getSelected()}\n");
                // saves the tree
                this.save(writer);
                // flush and close writer
                writer.Flush();
                writer.Close();

                // if there are over 30 save files, delete the oldests
                for (int i = 0; i < fiArray.Length - 30; i++)
                    try
                    {
                        // tries to open file to use, if throws exception the file is in use and will not be deleted
                        using (FileStream stream = fiArray[i].Open(FileMode.Open, FileAccess.Read, FileShare.None))
                        {
                            stream.Close();
                        }
                        // the file is not in use, ok to delete
                        fiArray[i].Delete();
                    }
                    catch (Exception e) {
                        
                    }
                return true;
            }
            catch (Exception e) { }
            return false;
        }

        /*
         *  the old save function
        /// <summary>
        /// initialize the trackTree save to given path, expects a path to a .dat file,
        /// if the file does not exists it will be created and flagged as hidden
        /// </summary>
        /// <param name="path">path to save file</param>
        /// <returns>true if the save was successful, else false</returns>
        public bool save(string path)
        {
            // if not exists, create the file and set it as hidden
            if (!File.Exists(path))
            {
                var stream = File.Create(path);
                // flag the file as hidden
                File.SetAttributes(path, File.GetAttributes(path) | FileAttributes.Hidden);
                stream.Close(); // close creation stream
            }
            // initalize writer
            BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Open));
            // saves the selected file, for user comfort
            writer.Write($"{this.getSelected()}\n");
            // saves
            bool flag = this.save(writer);
            writer.Flush(); // flush writer
            writer.Close(); // close writer and stream
            return flag;    // return save result
        }
        */

        /// <summary>
        /// recursive save via the writer given,
        /// saves this, then all children
        /// </summary>
        /// <param name="writer"></param>
        /// <returns>true if the save was successful, else false</returns>
        private bool save(BinaryWriter writer)
        {
            bool flag = true;   // save flag, true if successful, else false
            // write this.Name
            writer.Write(Name);
            // write IsDirectory value
            writer.Write(IsDirectory);
            // if IsDirectory, write t he selectedly tracked path, amount of children, and the children themselves
            if (IsDirectory)
            {
                if (Tracker.SelectedInfo != null)
                    writer.Write(Tracker.SelectedInfo.Name);
                else
                    writer.Write("NONE");
                writer.Write(Childrens.Count);
                foreach (var child in Childrens)
                    flag &= child.save(writer);
            }
            return flag;
        }

        #endregion

        #region static load method
        /*
         * the old load function 
        /// <summary>
        /// load a Tracktree, from given path,
        /// expects a .dat file, with the same format as the save method
        /// </summary>
        /// <param name="root">path to a .dat file containing the trackTree</param>
        /// <returns>TrackTree if the load was successful, else null</returns>
        public static TrackTree load(string root)
        {
            // if any error occurs during the load, will return null
            try
            {
                // create the reader, may throw fileNotExists error
                BinaryReader reader = new BinaryReader(File.Open(root, FileMode.Open));
                reader.ReadString();    // read the selected file string
                // TODO check if root exists
                var tree = new TrackTree(null, reader, new FileInfo(root).DirectoryName); // initialize the root trackTree via reader
                // loading successful, close reader and stream
                reader.Close();
                // return root
                return tree;
            }
            catch (Exception e)
            {
                // an error occured, return null
                return null;
            }
        }
        */

        /// <summary>
        /// load trackTree of the given root, if there isn't a save, will create a new Track tree
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public static TrackTree Load(string root)
        {
            string rootPath = Path.GetFullPath(root);
            // check if the root exists
            if (!Directory.Exists(rootPath))
            {
                // if not throw an exception
                throw new DirectoryNotFoundException();
            }

            // get the directory of the root in appData saves, if there is non it will be created
            var dir = Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MediaTracker", Utilties.toHexString(rootPath)));
            // get all the dir files
            var fiArray = dir.GetFiles();
            // sort them by creationDate
            Array.Sort(fiArray, (a, b) =>
            {
                return a.CreationTime.CompareTo(b.CreationTime);
            });
            // initiate the reader as null
            BinaryReader reader = null;
            // set the index of lastest save file
            int lastSaveFileInd = fiArray.Length - 1;
            // try to read it, if not succeed try the previous one
            while (lastSaveFileInd >= 0)
            {
                try
                {
                    // initialize the reader
                    reader = new BinaryReader(fiArray[lastSaveFileInd].OpenRead());
                    // init tree via the save file

                    // flush the final selected file, its only for the user's convenience
                    reader.ReadString();
                    // flush the root's name
                    reader.ReadString();
                    // flush the Isdirectory value, root is always a directory
                    reader.ReadBoolean();
                    // read the root selected child
                    string rootSelected = reader.ReadString();
                    // initialize the rootTree
                    TrackTree rootTree = new TrackTree(Directory.GetParent(rootPath).FullName, Path.GetFileName(rootPath), rootSelected);
                    // get children count
                    int count = reader.ReadInt32();

                    // read children
                    while (count-- > 0)
                        recLoad(rootTree, reader);

                    // get all the directory files from tracker
                    var dirFiles = rootTree.Tracker.FilesInfo;
                    bool needSorting = false;
                    // check each file
                    dirFiles.ForEach(file =>
                    {
                        // if the file is not in the tree's children it will be created and added
                        if (rootTree.Childrens.Find(child => child.FilePath.Equals(file.FullName)) == null)
                        {
                            // create and add the child
                            rootTree.Childrens.Add(new TrackTree(rootTree, file.Name));
                            // the children need sorting
                            needSorting = true;
                        }
                    });
                    if (needSorting)
                        rootTree.Childrens.Sort((a, b) =>
                        {
                            return a.Name.CompareTo(b.Name);
                        });

                    // loading successful, close reader and stream
                    reader.Close();
                    // return root
                    return rootTree;
                } catch (Exception e)
                {
                    // an exception was thrown, cannot open save file or the file was corrupted
                    // closing the reader
                    reader.Close();
                    // moving the index to the previous save file
                    lastSaveFileInd--;
                }
            }
            // there is no valid save file for the root, init a new tree
            TrackTree nullTree = null;  // need TrackTree null value for the constractur
            TrackTree newRoot = new TrackTree(nullTree, rootPath);  // create a new tree
            newRoot.save(); // saves it
            return newRoot;
        }

        /// <summary>
        /// recursive load function to read the tree's data and init it,
        /// if all went well the tree will be added to its parent children, else the tree will be flushed along with its children
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static bool recLoad(TrackTree parent, BinaryReader reader)
        {
            // if at end of stream, throw error, its not suppose to happen
            if (reader.BaseStream.Position == reader.BaseStream.Length)
                throw new EndOfStreamException();
            // read the file/dir name
            string name = reader.ReadString();
            // read if directory
            bool isDirectory = reader.ReadBoolean();
            // check if the tree exists, if not flush the tree and its children, they no longer exists
            if (!isDirectory && !File.Exists(Path.Combine(parent.FilePath, name)))
            {
                // if the tree is directory, will flush all its children
                if (isDirectory)
                {
                    // flush this tree's selected
                    reader.ReadString();
                    // read this tree's children count
                    int count = reader.ReadInt32();
                    // flush children
                    while (count-- > 0)
                        flushLoad(reader);
                }
                return false;
            }
            // else init tree and its children
            else if (isDirectory)
            {
                // read this tree's selected
                string selected = reader.ReadString();
                // create a directory tree
                TrackTree tree = new TrackTree(parent.FilePath, name, selected);
                // add tree to parent's children, and set the parent
                parent.Childrens.Add(tree);
                tree.Parent = parent;
                // initialize children
                int count = reader.ReadInt32();
                while (count-- > 0)
                    recLoad(tree, reader);

                // get all the directory files from tracker
                var dirFiles = tree.Tracker.FilesInfo;
                bool needSorting = false;
                // check each file
                dirFiles.ForEach(file =>
                {
                    // if the file is not in the tree's children it will be created and added
                    if (tree.Childrens.Find(child => child.FilePath.Equals(file.FullName)) == null)
                    {
                        // create and add the child
                        tree.Childrens.Add(new TrackTree(tree, file.Name));
                        // the children need sorting
                        needSorting = true;
                    }
                });
                if (needSorting)
                    tree.Childrens.Sort((a, b) =>
                    {
                        return a.Name.CompareTo(b.Name);
                    });
            }
            else
            {
                // create a file tree
                TrackTree tree = new TrackTree(parent.FilePath, name);
                // add tree to parent's children, and set the parent
                parent.Childrens.Add(tree);
                tree.Parent = parent;
            }
            return true;
        }

        /// <summary>
        /// flush the tree data from the reader, used when the file/directory no longer exists
        /// </summary>
        /// <param name="reader"></param>
        private static void flushLoad(BinaryReader reader)
        {
            // if at end of stream, throw error, its not suppose to happen
            if (reader.BaseStream.Position == reader.BaseStream.Length)
                throw new EndOfStreamException();
            // flush name
            reader.ReadString();
            // read isDirectory
            bool isDirectory = reader.ReadBoolean();
            if (isDirectory)
            {
                // flush selected
                reader.ReadString();
                // read children count
                int count = reader.ReadInt32();
                // flush each child
                while (count-- > 0)
                    flushLoad(reader);
            }
        }

        #endregion

        #endregion

    }
}
