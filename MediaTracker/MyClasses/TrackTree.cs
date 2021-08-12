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
        public string Path { private set; get; }
        /// <summary>
        /// the selected path of this tree, to which path this tree points at
        /// </summary>
        public string SelectedPath { get { return this.Tracker.SelectedPath; } }
        /// <summary>
        /// the selected trackTree of this tree, to which tree this tree points at
        /// </summary>
        public TrackTree SelectedTree
        {
            get
            {
                var tree = this.Childrens.Find((child) =>
                {
                    return this.SelectedPath.Equals(child.Path);
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
        public TrackTree(TrackTree parent, string name)
        {
            // inititalize properties
            this.Parent = parent;
            this.Name = name;
            this.Path = (parent!=null)? $"{parent.Path}/{Name}":Name;
            // fix path, replace '/' with '\'
            this.Path = Utilties.fixPath(this.Path);
            this.Childrens = new List<TrackTree>(); // initalize children list
            // initialize IsDirectory flag
            this.IsDirectory = File.GetAttributes(this.Path).HasFlag(FileAttributes.Directory);
            // if directory initialize FilesList and children trees
            if (IsDirectory)
            {
                this.Tracker = new TrackerList(Path);   // get files list
                // initalize children trees
                Tracker.FilesInfo.ForEach((file) => this.Childrens.Add(new TrackTree(this, file.Name)));
            }
        }

        /// <summary>
        /// initialize the tree with from the reader data, and the parent's info,
        /// if the path is a directory will initiate the children with the reader
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="reader"></param>
        private TrackTree(TrackTree parent, BinaryReader reader, string root)
        {
            // inititalize parent tree
            this.Parent = parent;
            // read the name
            this.Name = reader.ReadString();
            if (parent == null)
                this.Name = root;
            // build the tree Path
            this.Path = (parent != null) ? $"{parent.Path}\\{Name}" : Name;
            // fix path, replace '/' with '\'
            this.Path = Utilties.fixPath(this.Path);
            this.Childrens = new List<TrackTree>(); // initalize children list
            // read IsDirectory flag
            this.IsDirectory = reader.ReadBoolean();
            // if IsDirectory, initialize children (via reader) and tracker
            if (IsDirectory)
            {
                this.Tracker = new TrackerList(Path,reader.ReadString());
                int count = reader.ReadInt32();
                for (int ind = 0; ind < count; ind++)
                {
                    try
                    {
                        var childTracker = new TrackTree(this, reader,null);
                        this.Childrens.Add(childTracker);
                    } catch (Exception ex) { }
                }
                // check if all the children are up to date
                checkChildren();
            }
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
            if (this.Path.Equals(path))
                return this;
            // if IsDirectory is false, or this.Path isn't in the path given, return false, the given path is not in this route
            else if (!IsDirectory || !path.Contains(this.Path))
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
            else if (!IsDirectory || !dest.Path.Contains(this.Path))
                return false;
            // search each child for the one that contain the destination
            foreach (var child in Childrens)
            {
                // if the child returned true, the destination is him or on his tree
                if (child.setPathTo(dest))
                {
                    // point this.Tracker.selected at the child
                    this.Tracker.SelectedPath = child.Path;
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
        /// <returns>true if an updated was needed, else false</returns>
        public bool checkChildren()
        {
            if (!this.IsDirectory)
                return false;
            // update this tracker
            this.Tracker = new TrackerList(this.Path, this.SelectedPath);
            // init lists
            List<string> toAdd = new List<string>();
            List<string> toRemove = new List<string>();
            List<TrackTree> toUpdate = new List<TrackTree>();
            // get all current files and directories
            toAdd.AddRange(Utilties.getAllFilesAbove(this.Path, 20));
            // filter all children into 
            Childrens.ForEach((child) =>
            {
                // child still exists, check if need updating
                if (toAdd.Contains(child.Path))
                {
                    // no need to reAdd the child
                    toAdd.Remove(child.Path);
                    // to to update list
                    toUpdate.Add(child);
                }
                else
                    // the child not found, to be removed later
                    toRemove.Add(child.Path);
            });
            // add missing paths
            toAdd.ForEach((path) => this.Childrens.Add(new TrackTree(this, Utilties.getName(path))));
            // remove unFound paths
            Childrens.RemoveAll((child) => { return toRemove.Contains(child.Path); });
            // update already exist children
            toUpdate.ForEach((child) => child.checkChildren());
            // tree updated, return true
            return true;
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
                return this.Path;
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

        /// <summary>
        /// load a Tracktree, from given path,
        /// expects a .dat file, with the same format as the save method
        /// </summary>
        /// <param name="path">path to a .dat file containing the trackTree</param>
        /// <returns>TrackTree if the load was successful, else null</returns>
        public static TrackTree load(string path)
        {
            // if any error occurs during the load, will return null
            try
            {
                // create the reader, may throw fileNotExists error
                BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open));
                reader.ReadString();    // read the selected file string
                var tree = new TrackTree(null, reader, new FileInfo(path).DirectoryName); // initialize the root trackTree via reader
                // loading successful, close reader and stream
                reader.Close();
                // check if the tree is up to date
                tree.checkChildren();
                // return root
                return tree;
            }
            catch (Exception e)
            {
                // an error occured, return null
                return null;
            }
        }

        #endregion

        #endregion

    }
}
