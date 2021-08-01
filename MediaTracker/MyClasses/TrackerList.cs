using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MediaTracker
{
    class TrackerList
    {
        private List<string> files;
        private int index;
        public List<FileInfo> FilesInfo { get; }    // public fileInfo list
        public DirectoryInfo Info { get; }
        public bool IsEmpty { get { return index == -1; } }
        public string Current { set
            {
                this.index = files.IndexOf(value);
            }
            get { return (index != -1) ? files[index] : "NONE"; } }

        #region constructors

        public TrackerList(String directory)
        {
            // if directory does not exists throw exception
            if (!Directory.Exists(directory))
                throw new DirectoryNotFoundException();
            // initialize properties
            this.Info = new DirectoryInfo(directory);
            // get directory files and sub directories
            this.files = Utilties.getAllFilesAbove(directory, 20);
            // if there are no valid files, index is -1
            if (this.files.Count == 0)
                this.index = -1;
            // fix the paths
            for (int i = 0; i < files.Count; i++)
                files[i] = Utilties.fixPath(files[i]);
            // initialize FilesInfo list
            this.FilesInfo = new List<FileInfo>();
            foreach (string path in files)
            {
                this.FilesInfo.Add(new FileInfo(path));
            }
        }
        public TrackerList(string directory, string current) : this(directory)
        {
            // initialize current file
            this.index = this.files.IndexOf(current);
            // if the given current does not exists, will initialize at the first valid file, if there are no files then index = -1
            if (index == -1)
                this.index = (this.files.Count != 0)? 0:-1;

        }

        #endregion

        /// <summary>
        /// moves the index by the offset,
        /// if the new offset is zero of count-1, returns true, else false
        /// </summary>
        /// <param name="offset"></param>
        /// <returns>true if the new index is zero or last file, else returns false</returns>
        public bool move(int offset)
        {
            int oldIndex = this.index;
            this.index += offset;
            if (this.index < 0)
                this.index = 0;
            else if (this.index >= this.files.Count)
                this.index = this.files.Count - 1;
            return index != oldIndex && (index == 0 || index == files.Count - 1);
        }
    }
}
