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
using System.Windows.Shapes;
using System.Windows.Threading;
using Path = System.IO.Path;

using Squirrel;

namespace Eizper_Launcher_NetFramework
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        DispatcherTimer dt = new DispatcherTimer();
        UpdateManager manager;

        public SplashScreen()
        {
            InitializeComponent();

            Loaded += SplashScreen_Loaded;
        }

        private async void SplashScreen_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                using (manager = await UpdateManager.GitHubUpdateManager(@"https://github.com/IndraYudha22/eizper-launcher-v2"))
                {
                    CheckForUpdateLauncher();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private async void CheckForUpdateLauncher()
        {
            var updateInfo = await manager.CheckForUpdate();
            Console.WriteLine("CHECK UPDATE");

            if (updateInfo.ReleasesToApply.Count > 0)
            {
                var lastVersion = updateInfo.ReleasesToApply.OrderBy(x => x.Version).Last();
                await manager.DownloadReleases(updateInfo.ReleasesToApply);
                await manager.ApplyReleases(updateInfo);
                var latestExe = Path.Combine(manager.RootAppDirectory, string.Concat("app-", lastVersion.Version), "EizperChainLauncher.exe");
                manager.Dispose();
                UpdateManager.RestartApp(latestExe);
            }
            else
            {
                dt.Tick += new EventHandler(dt_tick);
                dt.Interval = new TimeSpan(0, 0, 2);
                dt.Start();
            }
        }

        private void dt_tick(object sender, EventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();

            dt.Stop();
            this.Close();
            manager.Dispose();
        }
    }
}
