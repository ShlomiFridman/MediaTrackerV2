using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MediaTracker
{
    /// <summary>
    /// A class that keep track of the files in the directory, and the currently selected file
    /// </summary>
    class TrackerList
    {

        #region properties

        /// <summary>
        /// list of strings that represents all the files/directories in this directory
        /// </summary>
        public List<string> FilesStrings { private set; get; }
        /// <summary>
        /// index of currently selected file
        /// </summary>
        private int index;
        /// <summary>
        /// list of FileInfos that represents the info of all the files/directories in this directory 
        /// </summary>
        public List<FileInfo> FilesInfo { get; }
        /// <summary>
        /// this directory's info
        /// </summary>
        public DirectoryInfo Info { get; }
        /// <summary>
        /// bool value of if this directory is empty
        /// </summary>
        public bool IsEmpty { get { return index == -1; } }
        /// <summary>
        /// string path of currently selected file, if the folder is empty the value is "NONE"
        /// </summary>
        public string Selected { set
            {
                this.index = FilesStrings.IndexOf(value);
            }
            get { return (index != -1) ? FilesStrings[index] : "NONE"; } }

        #endregion

        #region constructors

        /// <summary>
        /// Initialize the class with the directory path given
        /// </summary>
        /// <param name="directory"></param>
        public TrackerList(String directory)
        {
            // if directory does not exists throw exception
            if (!Directory.Exists(directory))
                throw new DirectoryNotFoundException();
            // initialize properties
            this.Info = new DirectoryInfo(directory);
            // get directory files and sub directories
            this.FilesStrings = Utilties.getAllFilesAbove(directory, 20);
            // if there are no valid files, index is -1
            if (this.FilesStrings.Count == 0)
                this.index = -1;
            // fix the paths
            for (int i = 0; i < FilesStrings.Count; i++)
                FilesStrings[i] = Utilties.fixPath(FilesStrings[i]);
            // initialize FilesInfo list
            this.FilesInfo = new List<FileInfo>();
            foreach (string path in FilesStrings)
            {
                this.FilesInfo.Add(new FileInfo(path));
            }
        }
        /// <summary>
        /// Initialize the class with the directory path given,
        /// and set the currently selected file as the one given.
        /// If the file was not found, the selected file will be the first one in the list (if there is one).
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="selected"></param>
        public TrackerList(string directory, string selected) : this(directory)
        {
            // initialize selected file
            this.index = this.FilesStrings.IndexOf(selected);
            // if the given selected does not exists, will initialize at the first valid file, if there are no files then index = -1
            if (index == -1)
                this.index = (this.FilesStrings.Count != 0)? 0:-1;

        }

        #endregion

        #region methods

        /// <summary>
        /// moves the index by the offset,
        /// if the new offset is zero of count-1, returns true, else false
        /// </summary>
        /// <param name="offset"></param>
        /// <returns>true if the new index is zero or last file, else returns false</returns>
        public bool move(int offset)
        {
            // saves the old index
            int oldIndex = this.index;
            this.index += offset;   // add the offset
            // if the new index out of bounds, set the index as the bound value (first or last file)
            if (this.index < 0)
                this.index = 0;
            else if (this.index >= this.FilesStrings.Count)
                this.index = this.FilesStrings.Count - 1;
            // return bool value of if the new index is at the bounds (first or last file), and it was not beforehand
            return index == oldIndex;
        }

        #endregion
    }
}
