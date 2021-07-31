using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Forms;


namespace MediaTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            var dialog = new FolderBrowserDialog();
            string path = "";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                path = dialog.SelectedPath;
            var item = new TreeViewItem()
            {
                Header = new FileInfo(path).Name,
                Tag = path
            };
            TreeView.Items.Add(item);
            Utilties.getAllFilesAbove(path, 20).ForEach((child) => setItems(item, child));
        }


        private void setItems(TreeViewItem parent, string path)
        {
            var info = new FileInfo(path);
            TreeViewItem item = new TreeViewItem()
            {
                Header = string.IsNullOrEmpty(info.Name) ? path : info.Name,
                Tag = path
            };
            parent.Items.Add(item);
            if (info.Attributes.HasFlag(FileAttributes.Directory))
            {
                Utilties.getAllFilesAbove(path,20).ForEach((file)=>
                {
                    setItems(item, file);
                });
            }
        }
    }
}
