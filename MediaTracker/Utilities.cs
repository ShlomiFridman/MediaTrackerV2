using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MediaTracker
{
    static class Utilties
    {

        public static long getSize(string path)
        {
            if (!Directory.Exists(path) && !File.Exists(path))
                return 0;
            FileInfo fileInfo = new FileInfo(path);
            if (fileInfo.Attributes.HasFlag(FileAttributes.Directory))
            {
                long sum = 0;
                foreach (string subPath in Directory.GetFiles(path))
                    sum += getSize(subPath);
                return sum;
            }
            else
                return fileInfo.Length;
        }

    }
}
