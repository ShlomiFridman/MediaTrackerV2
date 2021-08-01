using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MediaTracker
{
    static class Utilties
    {

        /// <summary>
        /// returns the path's (file or directory) size in bytes
        /// </summary>
        /// <param name="path"></param>
        /// <returns>long value that is the size in bytes</returns>
        public static long getSize(string path)
        {
            // if path does not exists, return 0
            if (!Directory.Exists(path) && !File.Exists(path))
                return 0;
            // get the file info
            FileInfo fileInfo = new FileInfo(path);
            // if directory calculate all its files sizes
            if (fileInfo.Attributes.HasFlag(FileAttributes.Directory))
            {
                long sum = 0;
                // get each file size, and add to sum
                foreach (string subPath in Directory.GetFiles(path))
                    sum += getSize(subPath);
                return sum;
            }
            // the path is a file, returnits length
            else
                return fileInfo.Length;
        }

        /// <summary>
        /// returns all the files paths, that their size is above the given size (in mb), in a list
        /// </summary>
        /// <param name="path"></param>
        /// <param name="size">the size in mb</param>
        /// <returns>List of strings</returns>
        public static List<string> getFilesAbove(string path, double size)
        {
            // if path is not a directory, throws exception
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException();
            // get all files in directory
            var files = new List<string>(Directory.GetFiles(path));
            // remove all files below the size given
            files.RemoveAll((path) =>
            {
                return (Utilties.getSize(path) / Math.Pow(10, 6)) < size;
            });
            // return valid files
            return files;
        }

        /// <summary>
        /// returns all the directories paths, that their size is above the given size (in mb), in a list
        /// </summary>
        /// <param name="path"></param>
        /// <param name="size">the size in mb</param>
        /// <returns>List of strings</returns>
        public static List<string> getDirectoriesAbove(string path, double size)
        {
            var files = new List<string>();
            try
            {
                // if path is not a directory, throws exception
                if (!Directory.Exists(path))
                    throw new DirectoryNotFoundException();
                // get all sub directories in directory
                files = new List<string>(Directory.GetDirectories(path));
                // remove all directories below the size given
                files.RemoveAll((path) =>
                {
                    return (Utilties.getSize(path) / Math.Pow(10, 6)) < size;
                });
            } catch (Exception e) {}
            // return valid directories
            return files;
        }

        /// <summary>
        /// returns all the directories and files paths, that their size is above the given size (in mb), in a list
        /// </summary>
        /// <param name="path"></param>
        /// <param name="size">the size in mb</param>
        /// <returns>List of strings</returns>
        public static List<string> getAllFilesAbove(string path, double size)
        {
            // get all valid files
            var files = getFilesAbove(path, size);
            // get all valid directories
            files.AddRange(getDirectoriesAbove(path, size));
            // sorting them out
            files.Sort((path1, path2) =>
            {
                return path1.CompareTo(path2);
            });
            // returning result
            return files;
        }

        public static string fixPath(string path)
        {
            return path.Replace('/', '\\');
        }

    }
}
