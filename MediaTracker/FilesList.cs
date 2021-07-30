using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MediaTracker
{
    class FilesList
    {
        private DirectoryInfo info;
        private List<string> files;
        public string current { set; get; }
        private int index;

        public FilesList(String directory)
        {
            // if directory does not exists throw exception
            if (!Directory.Exists(directory))
                throw new DirectoryNotFoundException();
            // initialize properties
            this.info = new DirectoryInfo(directory);
            // get directory files and sub directories
            this.files = new List<string>(Directory.GetFiles(directory));
            this.files.AddRange(Directory.GetDirectories(directory));
            // sorting them out
            this.files.Sort((path1, path2) =>
            {
                return path1.CompareTo(path2);
            });
            // removing all the files/subDirectories under 50mb in size
            this.files.RemoveAll((path) =>
            {
                return (Utilties.getSize(path) / Math.Pow(10, 6)) < 50;
            });
            // if there are no valid files, index is -1
            if (this.files.Count == 0)
                this.index = -1;
        }
        public FilesList(string directory, string current) : this(directory)
        {
            // initialize current file
            this.index = this.files.IndexOf(current);
            // if the given current does not exists, will initialize at the first valid file, if there are no files then index = -1
            if (index != -1)
                this.current = current;
            else if (index == -1 && this.files.Count != 0)
            {
                this.current = this.files[0];
                this.index = 0;
            }

        }

        /// <summary>
        /// moves the index by the offset,
        /// if the new offset is zero of count-1, returns true, else false
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public bool move(int offset)
        {
            return false;
        }
    }
}
