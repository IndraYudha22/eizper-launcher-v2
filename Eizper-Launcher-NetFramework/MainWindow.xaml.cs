using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
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
using Path = System.IO.Path;

namespace Eizper_Launcher_NetFramework
{
    enum LauncherStatus
    {
        ready, failed, downloadingGame, downloadingUpdate, extractGame
    }

    public partial class MainWindow : Window
    {
        public ProgressBar downloadProgress;

        private string URL_VERSION = "https://drive.google.com/uc?export=download&id=1xen01l4RJCG0Tdc2nHWrNZTRWOgZPgMK";
        private string URL_GAME = "https://drive.google.com/uc?export=download&id=1XwBPQWBh_r2ynyXeY5q6xvUxicZVyTVW";
        private string NAME_GAME_EXE = "EizperChain.exe";

        #region URL SOCIAL MEDIA
        private string URL_TWITTER = "https://twitter.com/EizperChain";
        private string URL_DISCORD = "https://discord.gg/CZbe53j767";
        private string URL_IG = "https://www.instagram.com/eizperchain/";
        private string URL_WEB = "https://www.eizperchain.com/";
        private string URL_MEDIUM = "https://medium.com/@eizperchain";
        private string URL_FB = "https://www.facebook.com/eizperchain";
        #endregion

        private string rootPath;
        private string versionFile;
        private string gameZip;
        private string gameExe;

        private string game_url;
        private string version_url;

        private bool extractStatus;

        private LauncherStatus _status;
        internal LauncherStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                switch (_status)
                {
                    case LauncherStatus.ready:
                        PlayButton.Content = "Launch";
                        break;
                    case LauncherStatus.failed:
                        PlayButton.Content = "Update Failed - Retry";
                        break;
                    case LauncherStatus.downloadingGame:
                        PlayButton.Content = "Downloading Game";
                        break;
                    case LauncherStatus.downloadingUpdate:
                        PlayButton.Content = "Downloading Update";
                        break;
                    case LauncherStatus.extractGame:
                        PlayButton.Content = "Extract Game";
                        break;
                    default:
                        break;
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            rootPath = Directory.GetCurrentDirectory();
            versionFile = Path.Combine(rootPath, "patch_version");
            gameZip = Path.Combine(rootPath, "bin.zip");
            gameExe = Path.Combine(rootPath, "bin", NAME_GAME_EXE);
        }

        private void Downloader_DownloadProgressChanged(object sender, FileDownloader.DownloadProgress progress)
        {
            downloadProgress.Value = progress.ProgressPercentage;
        }

        private void ProgressBar_Initialized(object sender, EventArgs e)
        {
            downloadProgress = (ProgressBar)sender;
        }

        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private async void CheckForUpdates()
        {
            if (File.Exists(versionFile))
            {
                Version localVersion = new Version(File.ReadAllText(versionFile));
                VersionText.Text = $"Ver {localVersion}";

                try
                {
                    WebClient webClient = new WebClient();

                    webClient.UseDefaultCredentials = true;
                    webClient.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;

                    await Task.Run(() => {
                        version_url = webClient.DownloadString(URL_VERSION);
                        return version_url;
                    });

                    Version onlineVersion = new Version(await Task.Run(()=> webClient.DownloadString(version_url)));

                    if (onlineVersion.IsDifferentThan(localVersion))
                    {
                        InstallGameFiles(true, onlineVersion);
                    }
                    else
                    {
                        Status = LauncherStatus.ready;
                    }

                }
                catch (Exception ex)
                {
                    Status = LauncherStatus.failed;
                    MessageBox.Show($"Error checking for game updates: {ex}");
                }
            }
            else
            {
                InstallGameFiles(false, Version.zero);
            }
        }

        private async void InstallGameFiles(bool _isUpdate, Version _onlineVersion)
        {
            try
            {
                WebClient webClient = new WebClient();
                FileDownloader downloader = new FileDownloader();

                webClient.UseDefaultCredentials = true;
                webClient.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;

                await Task.Run(() => {
                    version_url = webClient.DownloadString(URL_VERSION);
                    return version_url;
                });


                Console.WriteLine($"INI VERSION : {version_url}");

                await Task.Run(() => {
                    game_url = webClient.DownloadString(URL_GAME);
                    return game_url;
                });

                Console.WriteLine($"INI GAME : {game_url}");


                if (_isUpdate)
                {
                    Status = LauncherStatus.downloadingUpdate;
                }
                else
                {
                    Status = LauncherStatus.downloadingGame;
                    await Task.Run(() => {
                        _onlineVersion = new Version(webClient.DownloadString(version_url));
                        return _onlineVersion;
                    });
                }

                //webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(Downloader_DownloadProgressChanged);
                //webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadGameCompletedCallback);
                //webClient.DownloadFileAsync(new Uri(URL_GAME), gameZip, _onlineVersion);
                downloader.DownloadFileCompleted += DownloadGameCompletedCallback;
                downloader.DownloadProgressChanged += Downloader_DownloadProgressChanged;
                downloader.DownloadFileAsync(new Uri(game_url).ToString(), gameZip, _onlineVersion);
            }
            catch (Exception ex)
            {
                Status = LauncherStatus.failed;
                MessageBox.Show($"Error installing game files: {ex}");
            }
        }


        private async void DownloadGameCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                Status = LauncherStatus.extractGame;

                string onlineVersion = ((Version)e.UserState).ToString();

                if (Directory.Exists(Path.Combine(rootPath, "bin")))
                {
                    await Task.Run(() => Directory.Delete(Path.Combine(rootPath, "bin"), true));
                }

                await Task.Run(() => ZipFile.ExtractToDirectory(gameZip, rootPath));
                await Task.Run(() => File.Delete(gameZip));
                await Task.Run(() => File.WriteAllText(versionFile, onlineVersion));

                VersionText.Text = $"Ver {onlineVersion}";

                Status = LauncherStatus.ready;
            } 
            catch (Exception ex)
            {
                Status = LauncherStatus.failed;
                MessageBox.Show($"Error finishing download: {ex}");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(gameExe) && Status == LauncherStatus.ready)
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(gameExe);
                startInfo.WorkingDirectory = Path.Combine(rootPath, "bin");
                Process.Start(startInfo);

                Close();
            }
            else if (Status == LauncherStatus.failed)
            {
                CheckForUpdates();
            }
        }

        private void OpenURL(string url)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            CheckForUpdates();
        }

        #region CONTROLLER
        private void Button_Minimize(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Button_Close(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Drag(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        #endregion


        private void Click_Twitter(object sender, RoutedEventArgs e)
        {
            OpenURL(URL_TWITTER);
        }

        private void Click_Discord(object sender, RoutedEventArgs e)
        {
            OpenURL(URL_DISCORD);
        }

        private void Click_IG(object sender, RoutedEventArgs e)
        {
            OpenURL(URL_IG);
        }

        private void Click_Browser(object sender, RoutedEventArgs e)
        {
            OpenURL(URL_WEB);
        }

        private void Click_Medium(object sender, RoutedEventArgs e)
        {
            OpenURL(URL_MEDIUM);
        }

        private void Click_Fb(object sender, RoutedEventArgs e)
        {
            OpenURL(URL_FB);
        }
    }

    struct Version
    {
        internal static Version zero = new Version(0, 0, 0);

        private short major;
        private short minor;
        private short subMinor;

        internal Version(short _major, short _minor, short _subMinor)
        {
            major = _major;
            minor = _minor;
            subMinor = _subMinor;
        }
        internal Version(string _version)
        {
            string[] versionStrings = _version.Split('.');
            if (versionStrings.Length != 3)
            {
                major = 0;
                minor = 0;
                subMinor = 0;
                return;
            }

            major = short.Parse(versionStrings[0]);
            minor = short.Parse(versionStrings[1]);
            subMinor = short.Parse(versionStrings[2]);
        }

        internal bool IsDifferentThan(Version _otherVersion)
        {
            if (major != _otherVersion.major)
            {
                return true;
            }
            else
            {
                if (minor != _otherVersion.minor)
                {
                    return true;
                }
                else
                {
                    if (subMinor != _otherVersion.subMinor)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override string ToString()
        {
            return $"{major}.{minor}.{subMinor}";
        }
    }
}
