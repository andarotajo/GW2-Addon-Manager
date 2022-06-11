using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GW2_Addon_Manager.App.Configuration;

namespace GW2_Addon_Manager
{
    /// <summary>
    /// Interaction logic for Updating.xaml
    /// </summary>
    public partial class UpdatingView : Page
    {
        private readonly IConfigurationManager _configurationManager;

        /// <summary>
        /// Sets the page's DataContext, initializes it, and begins the update process.
        /// </summary>
        public UpdatingView()
        {
            _configurationManager = new ConfigurationManager();
            DataContext = UpdatingViewModel.GetInstance;
            InitializeComponent();

            LoaderSetup settingUp = new LoaderSetup(new ConfigurationManager());
            Task.Run(() => UpdateHelpers.UpdateAll());

            launchOnClose.IsChecked = _configurationManager.UserConfig.LaunchGame;
            launchCustomOnClose.IsChecked = _configurationManager.UserConfig.LaunchCustomExecutable;
        }

        /***************************** Titlebar Window Drag *****************************/
        private void TitleBar_MouseHeld(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                Application.Current.MainWindow.DragMove();
        }

        /***************************** Button Controls *****************************/

        private void close_clicked(object sender, RoutedEventArgs e)
        {
            SelfUpdate.startUpdater();

            if ((bool)launchOnClose.IsChecked)
            {
                string exeLocation = Path.Combine(_configurationManager.UserConfig.GamePath, _configurationManager.UserConfig.ExeName);
                try
                {
                    Process.Start(exeLocation, "-autologin");
                }
                catch (System.ComponentModel.Win32Exception)
                {
                    MessageBox.Show($"Unable to launch game as {_configurationManager.UserConfig.ExeName} is missing.",
                                     "Unable to Launch Game",
                                     MessageBoxButton.OK,
                                     MessageBoxImage.Error);
                }
            }
            else if ((bool)launchCustomOnClose.IsChecked)
            {
                string exe = _configurationManager.UserConfig.CustomExecutable;
                try
                {
                    string arguments = "";
                    if (exe.Contains("Blish"))
                    {
                        arguments = "-g 2";
                    }

                    Process.Start(exe, arguments);
                }
                catch (System.ComponentModel.Win32Exception)
                {
                    MessageBox.Show($"Unable to launch custom executable.",
                                     "Unable to Launch Custom Executable",
                                     MessageBoxButton.OK,
                                     MessageBoxImage.Error);
                }
            }

            if (_configurationManager.UserConfig.LaunchGame != launchOnClose.IsChecked)
            {
                _configurationManager.UserConfig.LaunchGame = (bool)launchOnClose.IsChecked;
                _configurationManager.SaveConfiguration();
            }

            if (_configurationManager.UserConfig.LaunchCustomExecutable != launchCustomOnClose.IsChecked)
            {
                _configurationManager.UserConfig.LaunchCustomExecutable = (bool)launchCustomOnClose.IsChecked;
                _configurationManager.SaveConfiguration();
            }
            
            Application.Current.Shutdown();
        }

        private void xbutton_clicked(object sender, RoutedEventArgs e)
        {
            SelfUpdate.startUpdater();
            System.Windows.Application.Current.Shutdown();
        }

        private void minimize_clicked(object sender, RoutedEventArgs e)
        {
            (this.Parent as Window).WindowState = WindowState.Minimized;
        }


        /***** Hyperlink Handler *****/
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void back_clicked(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("UI/OpeningPage/OpeningView.xaml", UriKind.Relative));
        }

    }
}

