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

using Squirrel;

namespace Eizper_Launcher_NetFramework
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        string LINK_GITHUB = @"https://github.com/IndraYudha22/eizper-launcher-v2";

        DispatcherTimer dt = new DispatcherTimer();
        UpdateManager manager;

        public SplashScreen()
        {
            InitializeComponent();

            Loaded += SplashScreen_Loaded;

            //dt.Tick += new EventHandler(dt_tick);
            //dt.Interval = new TimeSpan(0, 0, 2);
            //dt.Start();
        }

        private async void SplashScreen_Loaded(object sender, RoutedEventArgs e)
        {
            manager = await UpdateManager.GitHubUpdateManager(LINK_GITHUB);
            CheckForUpdateLauncher();
        }

        private async void CheckForUpdateLauncher()
        {
            var updateInfo = await manager.CheckForUpdate();
            
            if (updateInfo.ReleasesToApply.Count > 0)
            {
                UpdateLauncher();
            }
        }

        private async void UpdateLauncher()
        {
            await manager.UpdateApp();

            if (manager.UpdateApp().IsCompleted)
            {
                MainWindow mw = new MainWindow();
                mw.Show();

                this.Close();
            }
        }

        private void dt_tick(object sender, EventArgs e)
        {
            MainWindow mw = new MainWindow();
            mw.Show();

            dt.Stop();
            this.Close();
        }
    }
}
