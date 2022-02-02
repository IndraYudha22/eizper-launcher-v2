using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace eizper_launcher.LauncherData
{
    public class VersionManager
    {
        //public Dictionary<string, string> versionLinkPairs;
        //private MainWindow windowClass;

        //public VersionManager(MainWindow windowsClass)
        //{
        //    this.windowClass = windowsClass;

        //    versionLinkPairs = new Dictionary<string, string>();
        //    windowClass.playButton.IsEnabled = false;
        //    windowClass.versionSelector.IsEnabled = false;

        //    Init();
        //}

        //private void Init()
        //{
        //    WebClient web = new WebClient();
        //    web.DownloadStringCompleted += Web_DownloadStringCompleted;
        //    web.DownloadStringAsync(new Uri("https://drive.google.com/uc?export=download&id=11IIZ0MFOqzWFJldYONIQIYRR8grbdFCU"));
        //}

        //private void Web_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        //{
        //    string temp = e.Result;
        //    string[] versionLinks = temp.Split(Environment.NewLine);

        //    ObservableCollection<string> versionToDisplay = new ObservableCollection<string>();
        //    for (int i = 0; i < versionLinks.Length; i++)
        //    {
        //        string[] version_link = versionLinks[i].Split(' ');
        //        versionLinkPairs.Add(version_link[0], version_link[1]);
        //        versionToDisplay.Add(version_link[0]);
        //    }

        //    windowClass.versionSelector.ItemsSource = versionToDisplay;
        //    windowClass.versionSelector.Items.Refresh();

        //    windowClass.playButton.IsEnabled = true;
        //    windowClass.versionSelector.IsEnabled = true;
        //}
    }
}
