using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace MediaTracker
{
    class TrackDictionary
    {

        private static TrackDictionary instance;

        public Dictionary<string, string> Dictionary { private set; get; }
        private FileInfo tracker;

        private string current;

        public TrackerList FilesList { set; get; }

        public string Root {
            set
            {
                if (Dictionary.ContainsKey("root"))
                    Dictionary["root"] = value;
                else
                    Dictionary.Add("root", value);
                save();
            }
            get
            {
                if (!Dictionary.ContainsKey("root"))
                    return Environment.CurrentDirectory;
                else
                    return Dictionary["root"];
            }
        }

        private TrackDictionary()
        {
            // initalize dictionary and tracker file
            this.Dictionary = new Dictionary<string, string>();
            this.tracker = new FileInfo("./tracker.dat");
            // if tracker does not exists create one
            if (!tracker.Exists)
                tracker.Create();
            // else load the dictionary stored in it
            else
                load();

        }

        #region changeCurrentAndMove

        public TrackerList changeCurrent(string path)
        {
            // if the new path does not exists or isn't a dictionary
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException();
            // save given path to current
            this.current = path;
            // update current in dictionary
            if (this.Dictionary.ContainsKey("current"))
                this.Dictionary["current"] = current;
            // no current in dictionary, initialize with given path
            else
                this.Dictionary.Add("current", current);
            // if already have the path value, get the up to date list
            if (Dictionary.ContainsKey(current))
            {
                this.FilesList = new TrackerList(current, Dictionary[current]);
                // the value was not found, get updated current value
                if (FilesList.Current != Dictionary[current])
                    Dictionary[current] = FilesList.Current;
            }
            // the path was not found in dictionary, create a new entry
            else
            {
                this.FilesList = new TrackerList(current);  // get list with initiale value
                this.Dictionary.Add(current, FilesList.Current);  // saves the value in dictionary
            }
            save();
            return this.FilesList;
        }

        public bool move(int offset)
        {
            // if no current path, returns false
            if (Dictionary.ContainsKey("current"))
                return false;
            // moves the list, and saves bool value
            bool flag = this.FilesList.move(offset);
            // update entry in dictionary
            this.Dictionary[current] = this.FilesList.Current;
            // saves the updated dictionary
            save();
            // return bool value if at edges
            return flag;    
        }

        #endregion

        #region saveAndLoad

        /// <summary>
        /// saves the dictionary into tracker.dat file,
        /// first line is the amount of entries
        /// then all the entries, key than value
        /// </summary>
        /// <returns></returns>
        public bool save()
        {
            // open stream
            Stream stream = File.Open(tracker.FullName,FileMode.OpenOrCreate);
            // creates writer
            BinaryWriter writer = new BinaryWriter(stream);
            // write the dictionary count
            writer.Write(Dictionary.Count);
            // write each entry
            foreach (var kvp in Dictionary)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value);
            }
            // flush writer
            writer.Flush();
            // closes stream
            writer.Close();
            return true;    // saved successfully
        }

        /// <summary>
        /// load the dictionary from the tracker.dat file
        /// </summary>
        /// <returns></returns>
        private bool load()
        {
            // clear current dictionary
            this.Dictionary.Clear();
            // open stream
            Stream stream = File.Open(tracker.FullName, FileMode.OpenOrCreate);
            // creates writer
            BinaryReader reader = new BinaryReader(stream);
            // get the amount of entries
            int count = reader.ReadInt32();
            // read each entry, key -> value, and add them to the dictionary
            for (int ind = 0; ind < count; ind++)
            {
                string key = reader.ReadString();
                string value = reader.ReadString();
                Dictionary.Add(key, value);
            }
            // closes stream
            reader.Close();
            // if has current, updates current and list properties
            if (Dictionary.ContainsKey("current"))
            {
                this.current = Dictionary["current"];
                this.FilesList = new TrackerList(current, Dictionary[current]);
            }
            return true;    // loaded successfully
        }

        #endregion


        public static TrackDictionary getInstance()
        {
            if (instance == null)
                instance = new TrackDictionary();
            return instance;
        }

    }


}
