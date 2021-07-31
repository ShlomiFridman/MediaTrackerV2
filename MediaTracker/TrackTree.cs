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

        public bool IsDirectory { get { return Tracker != null; } }
        public bool IsEmpty { get { return Tracker != null && Tracker.IsEmpty; } }


        public TrackTree(TrackTree parent, string name)
        {
            this.Parent = parent;
            this.Name = name;
            this.Path = (parent!=null)? $"{parent.Path}/{Name}":Name;
            this.Childrens = new List<TrackTree>(); // initalize children list
            // if directory initialize FilesList and children trees
            if (IsDirectory)
            {
                this.Tracker = new TrackerList(Path);   // get files list
                // initalize children trees
                Tracker.FilesInfo.ForEach((file) => this.Childrens.Add(new TrackTree(this, file.Name)));
            }
        }

        public TrackTree(TrackTree parent, BinaryReader reader)
        {
            this.Parent = parent;
            // write this name
            this.Name = reader.ReadString();
            this.Path = (parent != null) ? $"{parent.Path}/{Name}" : Name;
            this.Childrens = new List<TrackTree>(); // initalize children list
            // if directory reads true, initialize children and tracker
            if (reader.ReadBoolean())
            {
                this.Tracker = new TrackerList(Path,reader.ReadString());
                int count = reader.ReadInt32();
                for (int ind = 0; ind < count; ind++)
                    this.Childrens.Add(new TrackTree(this, reader));
            }
        }

        #region openFunctions

        public void open()
        {
            Process.Start("explorer.exe", this.Path);
        }

        public void openParent()
        {
            Process.Start("explorer.exe", this.Parent.Path);
        }

        #endregion

        #region save

        public bool save()
        {
            BinaryWriter writer = new BinaryWriter(File.Open("./TrackerTree.dat", FileMode.OpenOrCreate));
            bool flag = this.save(writer);
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

        public static TrackTree load()
        {
            BinaryReader reader;
            try
            {
                reader = new BinaryReader(File.Open("./TrackerTree.dat", FileMode.Open));
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
