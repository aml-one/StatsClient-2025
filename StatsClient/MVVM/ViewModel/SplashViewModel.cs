using StatsClient.MVVM.Core;
using StatsClient.MVVM.View;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using static StatsClient.MVVM.Core.LocalSettingsDB;
using static StatsClient.MVVM.Core.Functions;

namespace StatsClient.MVVM.ViewModel;

public class SplashViewModel : ObservableObject
{
    public MainWindow? mainWindow;
    public bool isEverythingOkay = true;
    private readonly DispatcherTimer timerCheckServerConnectionFirstTime = new ();
    
    private static SplashViewModel? instance;
    public static SplashViewModel Instance
    {
        get => instance!;
        set
        {
            instance = value;
            RaisePropertyChangedStatic(nameof(Instance));
        }
    }

    private string loadingText = "";
    public string LoadingText
    {
        get => loadingText;
        set
        {
            loadingText = value;
            RaisePropertyChanged(nameof(LoadingText));
        }
    }

    private string softwareVersion = "0";
    public string SoftwareVersion
    {
        get => softwareVersion;
        set
        {
            softwareVersion = value;
            RaisePropertyChanged(nameof(SoftwareVersion));
        }
    }

    private bool finishedWithServerConnectionCheck;
    public bool FinishedWithServerConnectionCheck
    {
        get => finishedWithServerConnectionCheck;
        set
        {
            finishedWithServerConnectionCheck = value;
            RaisePropertyChanged(nameof(FinishedWithServerConnectionCheck));
        }
    }

    private bool cbSettingGlassyEffect = true;
    public bool CbSettingGlassyEffect
    {
        get => cbSettingGlassyEffect;
        set
        {
            cbSettingGlassyEffect = value;
            RaisePropertyChanged(nameof(CbSettingGlassyEffect));
        }
    }






    public SplashViewModel()
    {
        Instance = this;

        timerCheckServerConnectionFirstTime.Tick += TimerCheckServerConnectionFirstTime_Tick;
        timerCheckServerConnectionFirstTime.Interval = new TimeSpan(0, 0, 1);

        _ = bool.TryParse(ReadLocalSetting("GlassyEffect"), out bool GlassyEffect);
        CbSettingGlassyEffect = GlassyEffect;
    }

    private async void TimerCheckServerConnectionFirstTime_Tick(object? sender, EventArgs e)
    {
        SoftwareVersion = await GetAppVersion();
        timerCheckServerConnectionFirstTime.Stop();
        CheckStatsServerConnection();
    }

    internal async void StartLoading()
    {
        await Task.Run(DatabaseConnection.SetCredentials); // getting credentials for SQL server from BaseSettings.Config file

        LoadingText = "Checking local configs..";
        await Task.Run(CreatingLocalConfigFiles); // first try.. initialize database

        timerCheckServerConnectionFirstTime.Start();
    }

    private async void CheckStatsServerConnection()
    {
        isEverythingOkay = true;
        await Task.Run(CreatingLocalConfigFiles); // double tap.. to make sure database initialized correctly
        LoadingText = "Checking server connection..";
        await Task.Run(() => Thread.Sleep(500));
        try
        {
            using (var connection = new SqlConnection(DatabaseConnection.ConnectionStrToStatsDatabase()))
            {
                var query = "select 1";
                var command = new SqlCommand(query, connection);
                connection.Open();
                command.ExecuteScalar();
            }
            FinishedWithServerConnectionCheck = true;
            LoadingText = "Successfully connected to server!";
            await Task.Run(() => Thread.Sleep(500));
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[{ex.LineNumber()}] {ex.Message}");
            if (ex.Message.Contains("Login failed for user"))
            {
                MessageBox.Show(ex.Message + "\n\nApplication will shutdown!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                SplashWindow.Instance.Close();
                return;
            }
            else
            {
                try
                {
                    DatabaseConnection.StatsdbInstance = "";
                    using (var connection = new SqlConnection(DatabaseConnection.ConnectionStrToStatsDatabase()))
                    {
                        var query = "select 1";
                        var command = new SqlCommand(query, connection);
                        connection.Open();
                        command.ExecuteScalar();
                    }
                    FinishedWithServerConnectionCheck = true;
                    LoadingText = "Successfully connected to server!";
                    await Task.Run(() => Thread.Sleep(500));
                }
                catch (Exception exx)
                {
                    Debug.WriteLine($"[{exx.LineNumber()}] {exx.Message}");
                    isEverythingOkay = false;
                    LoadingText = "Couldn't connect to server..";
                    await Task.Run(() => Thread.Sleep(500));
                    LoadingText = exx.Message;
                }
            }
        }

        FinishedWithServerConnectionCheck = isEverythingOkay;
        AfterServerConnectionChecked();
    }

    private void AfterServerConnectionChecked()
    {
        if (!isEverythingOkay)
        {
            MessageBoxResult dg = MessageBox.Show("Could not connect to DataBase server!\nServer might be offline or not accessible.\n\nClick OK to retry.", "Could'n connect..", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (dg == MessageBoxResult.OK)
            {
                isEverythingOkay = true;
                CheckStatsServerConnection();
            }
            else if (dg == MessageBoxResult.Cancel)
            {
                SplashWindow.Instance.Close();
            }
        }
        else
        {
            mainWindow = new();
            mainWindow.Show();
            mainWindow.Hide();

            MainViewModel.StartInitialTasks();
        }
    }
}
