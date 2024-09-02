using IWshRuntimeLibrary;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using File = System.IO.File;
using Path = System.IO.Path;

namespace StatsClientUpdater
{
    public partial class MainWindow : Window
    {
        public static readonly string LocalConfigFolderHelper = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\Stats_Client\";
        public System.Timers.Timer _timer;
        public int AppStartTryCount = 0;
        private static readonly string ProgramFiles = Environment.ExpandEnvironmentVariables("%ProgramW6432%");
        private string appPath = @$"{Path.Combine(ProgramFiles, "AmL", "StatsClient")}\";

        private ResourceDictionary lang = [];
        public ResourceDictionary Lang
        {
            get => lang;
            set
            {
                lang = value;
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            SetLanguageDictionary();

            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += Timer_Elapsed;
        }

        public void SetLanguageDictionary(string language = "")
        {
            if (language.Equals(""))
            {
                Lang.Source = Thread.CurrentThread.CurrentCulture.ToString() switch
                {
                    "en-US" => new Uri("\\Lang\\StringResources_English.xaml", UriKind.Relative),
                    "zh-Hans" => new Uri("\\Lang\\StringResources_Chinese.xaml", UriKind.Relative),
                    "zh-Hant" => new Uri("\\Lang\\StringResources_Chinese.xaml", UriKind.Relative),
                    "zh-CHT" => new Uri("\\Lang\\StringResources_Chinese.xaml", UriKind.Relative),
                    "zh-CN" => new Uri("\\Lang\\StringResources_Chinese.xaml", UriKind.Relative),
                    "zh-CHS" => new Uri("\\Lang\\StringResources_Chinese.xaml", UriKind.Relative),
                    "zh-HK" => new Uri("\\Lang\\StringResources_Chinese.xaml", UriKind.Relative),
                    _ => new Uri("\\Lang\\StringResources_English.xaml", UriKind.Relative),
                };
            }
            else
            {
                try
                {
                    Lang.Source = new Uri("\\Lang\\StringResources_" + language + ".xaml", UriKind.Relative);
                }
                catch (IOException)
                {
                    Lang.Source = new Uri("\\Lang\\StringResources_English.xaml", UriKind.Relative);
                }
            }

            this.Resources.MergedDictionaries.Add(lang);
        }

        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            Debug.WriteLine(appPath);
            _timer.Stop();
            StopMainApp();
        }

        private void StopMainApp()
        {
            var Processes = Process.GetProcesses()
                               .Where(pr => pr.ProcessName == "StatsClient");
            foreach (var process in Processes)
            {
                process.Kill();
            }
            
            

            Task.Run(DownloadUpdate).Wait();
            
            StartCaseApp();
        }

        private async void DownloadUpdate()
        {
            Thread.Sleep(2000);

            if (!Directory.Exists(appPath))
            {
                try
                {
                    Directory.CreateDirectory(appPath);
                }
                catch (Exception) 
                {
                    appPath = Environment.SpecialFolder.Desktop.ToString();
                }
            }

            try
            {
                Thread.Sleep(500);
                if (File.Exists($@"{LocalConfigFolderHelper}StatsClient_old.exe"))
                    File.Delete($@"{LocalConfigFolderHelper}StatsClient_old.exe");
                Thread.Sleep(500);
                if (File.Exists($@"{appPath}\StatsClient.exe"))
                    File.Move($@"{appPath}\StatsClient.exe", $@"{LocalConfigFolderHelper}StatsClient_old.exe");
                Thread.Sleep(2000);
                using var client = new HttpClient();
                using var s = await client.GetStreamAsync("https://raw.githubusercontent.com/aml-one/StatsClient-2025/master/StatsClient/Executable/StatsClient.exe");
                using var fs = new FileStream($@"{appPath}StatsClient.exe", FileMode.OpenOrCreate);
                await s.CopyToAsync(fs);

                CreateShortcut(appPath);
            }
            catch (Exception exx)
            {
                //try
                //{
                //    Thread.Sleep(500);
                //    using var client = new HttpClient();
                //    using var s = await client.GetStreamAsync("https://aml.one/CaseChecker---2025/CaseChecker.exe");
                //    using var fs = new FileStream($@"{appPath}StatsClient.exe", FileMode.OpenOrCreate);
                //    await s.CopyToAsync(fs);

                //    CreateShortcut(appPath);
                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                //    if (File.Exists($@"{LocalConfigFolderHelper}StatsClient_old.exe"))
                //        File.Move($@"{LocalConfigFolderHelper}StatsClient_old.exe", $@"{appPath}StatsClient.exe");
                //}

                MessageBox.Show(this, exx.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (File.Exists($@"{LocalConfigFolderHelper}StatsClient_old.exe"))
                    File.Move($@"{LocalConfigFolderHelper}StatsClient_old.exe", $@"{appPath}StatsClient.exe");
            }

            Thread.Sleep(3000);
        }

        private void CreateShortcut(string appFolder)
        {
            object shDesktop = (object)"Desktop";
            WshShell shell = new ();
            string shortcutAddress = (string)shell.SpecialFolders.Item(ref shDesktop) + @"\Stats Client 2025.lnk";
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutAddress);
            shortcut.Description = "Stats Client 2025";
            shortcut.Hotkey = "Ctrl+Shift+S";
            shortcut.TargetPath = @$"{appFolder}StatsClient.exe";
            shortcut.WorkingDirectory = appFolder;
            shortcut.Save();
        }

        private void StartCaseApp()
        {
            AppStartTryCount++;
            Thread.Sleep(3000);
            try
            {
                var p = new Process();

                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.Arguments = $"/c \"{appPath}StatsClient.exe\"";
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.Start();

                Thread.Sleep(2000);
                CloseThisApp();
            }
            catch (Exception)
            {
                MessageBox.Show((string)Lang["couldNotStart"], (string)Lang["error"], MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!CheckIfAppIsRunning() && AppStartTryCount < 4)
                StartCaseApp();
        }

        private static bool CheckIfAppIsRunning()
        {
            var Processes = Process.GetProcesses()
                               .Where(pr => pr.ProcessName == "StatsClient");
            foreach (var process in Processes)
            {
                if (process.Id != null)
                    return true;
            }

            return false;
        }


        private static void CloseThisApp()
        {
            Thread.Sleep(1000);
            Environment.Exit(0);
        }

        private void Label_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show((string)Lang["closeMessage"], (string)Lang["caseCheckerUpdater"], MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                if (File.Exists($@"{LocalConfigFolderHelper}StatsClient_old.exe"))
                    Task.Run(() => File.Move($@"{LocalConfigFolderHelper}StatsClient_old.exe", $@"{appPath}StatsClient.exe")).Wait();
                Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _timer.Start();
        }
    }
}