using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace MediaTracker
{
    class TrackTree
    {

        public TrackTree Parent { private set; get; }
        public List<TrackTree> Childrens { private set; get; }

        public TrackerList Tracker { private set; get; }
        public string Name { private set; get; }
        public string Path { private set; get; }

        public bool IsDirectory { private set; get; }
        public bool IsEmpty { get { return Tracker != null && Tracker.IsEmpty; } }


        public TrackTree(TrackTree parent, string name)
        {
            this.Parent = parent;
            this.Name = name;
            this.Path = (parent!=null)? $"{parent.Path}/{Name}":Name;
            this.Path = Utilties.fixPath(this.Path);
            this.Childrens = new List<TrackTree>(); // initalize children list
            this.IsDirectory = File.GetAttributes(this.Path).HasFlag(FileAttributes.Directory);
            // if directory initialize FilesList and children trees
            if (IsDirectory)
            {
                this.Tracker = new TrackerList(Path);   // get files list
                // initalize children trees
                Tracker.FilesInfo.ForEach((file) => this.Childrens.Add(new TrackTree(this, file.Name)));
            }
        }

        private TrackTree(TrackTree parent, BinaryReader reader)
        {
            this.Parent = parent;
            // write this name
            this.Name = reader.ReadString();
            this.Path = (parent != null) ? $"{parent.Path}/{Name}" : Name;
            this.Path = Utilties.fixPath(this.Path);
            this.Childrens = new List<TrackTree>(); // initalize children list
            this.IsDirectory = reader.ReadBoolean();
            // if directory reads true, initialize children and tracker
            if (IsDirectory)
            {
                this.Tracker = new TrackerList(Path,reader.ReadString());
                int count = reader.ReadInt32();
                for (int ind = 0; ind < count; ind++)
                    this.Childrens.Add(new TrackTree(this, reader));
            }
        }

        public TrackTree search(string path)
        {
            if (this.Path.Equals(path))
                return this;
            else if (!IsDirectory || !path.Contains(this.Path))
                return null;
            TrackTree next = null;
            foreach (var child in Childrens)
            {
                next = child.search(path);
                if (next != null)
                    return next;
            }
            return next;
        }

        public string getCurrent()
        {
            if (!IsDirectory)
                return this.Path;
            string current = this.Childrens.Find((child) =>
            {
                return this.Tracker.Current == child.Path;
            }).getCurrent();

            return current.Equals("EMPTY") ? this.Path : current;
        }

        public TrackTree getCurrentTree()
        {
            if (!IsDirectory || this.IsEmpty)
                return this;
            var current = this.Childrens.Find((child) =>
            {
                return this.Tracker.Current == child.Path;
            });
            return current == null ? this : current.getCurrentTree();
        }

        public bool setPathTo(TrackTree dest)
        {
            if (dest == this)
                return true;
            else if (!IsDirectory)
                return false;
            foreach (var child in Childrens)
            {
                if (child.setPathTo(dest))
                {
                    this.Tracker.Current = child.Path;
                    return true;
                }
            }
            return false;
        }

        #region save

        public bool save(string path)
        {
            BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate));
            bool flag = this.save(writer);
            writer.Flush();
            writer.Close();
            return flag;
        }

        private bool save(BinaryWriter writer)
        {
            bool flag = true;
            // write this name
            writer.Write(Name);
            // if directory writes true, the currently tracked file, and all the children
            if (IsDirectory)
            {
                writer.Write(true);
                writer.Write(Tracker.Current);
                writer.Write(Childrens.Count);
                foreach (var child in Childrens)
                    flag &= child.save(writer);
            }
            else
                writer.Write(false);
            return flag;
        }

        #endregion

        #region staticLoad

        public static TrackTree load(string path)
        {
            BinaryReader reader;
            try
            {
                reader = new BinaryReader(File.Open(path, FileMode.Open));
            } catch (Exception e)
            {
                return null;
            }
            var tree = new TrackTree(null,reader);
            reader.Close();
            return tree;
        }

        #endregion
    }
}
