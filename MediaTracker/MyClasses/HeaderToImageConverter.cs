using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace MediaTracker.MyClasses
{
    class HeaderToImageConverter : IValueConverter
    {
        public static HeaderToImageConverter Instance = new HeaderToImageConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // get the string value
            string path = (string)value;
            string image = null;  // default value
            // check if logical drive
            foreach (var logicalPath in Environment.GetLogicalDrives())
                if (logicalPath.Equals(path))
                {
                    image = "drive";  // is logical drive
                    break;
                }
            // if not logical drive
            if (string.IsNullOrEmpty(image))
            {
                // if file\directory not found
                if (!File.Exists(path) && !Directory.Exists(path))
                    image = "notFound";
                // if is directory
                else if (File.GetAttributes(path).HasFlag(FileAttributes.Directory))
                    image = "folder";   // is directory
                else
                    image = "file";     // is file
            }
            return new BitmapImage(new Uri($"pack://application:,,,/assets/images/{image}.png"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
