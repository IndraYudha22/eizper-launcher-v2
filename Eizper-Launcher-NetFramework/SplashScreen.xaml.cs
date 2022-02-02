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

namespace Eizper_Launcher_NetFramework
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        DispatcherTimer dt = new DispatcherTimer();

        public SplashScreen()
        {
            InitializeComponent();
            dt.Tick += new EventHandler(dt_tick);
            dt.Interval = new TimeSpan(0, 0, 2);
            dt.Start();
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
