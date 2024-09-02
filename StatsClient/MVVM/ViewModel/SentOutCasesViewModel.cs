using StatsClient.MVVM.Core;
using StatsClient.MVVM.Model;
using static StatsClient.MVVM.Core.LocalSettingsDB;
using static StatsClient.MVVM.Core.DatabaseOperations;
using System.Windows;
using System.Timers;
using System.Text.RegularExpressions;
using System.Diagnostics;
using StatsClient.MVVM.View;
using System.Windows.Controls;
using Newtonsoft.Json;
using System.Net.Http;

namespace StatsClient.MVVM.ViewModel;

public partial class SentOutCasesViewModel : ObservableObject
{
    private bool panelsAddedalready = false;
    public bool PanelsAddedalready
    {
        get => panelsAddedalready;
        set
        {
            panelsAddedalready = value;
            RaisePropertyChanged(nameof(PanelsAddedalready));
        }
    }
    
    private SentOutCasesViewModel? instance;
    public SentOutCasesViewModel? Instance
    {
        get => instance;
        set
        {
            instance = value;
            RaisePropertyChanged(nameof(Instance));
        }
    }
    
    private static SentOutCasesViewModel? staticInstance;
    public static SentOutCasesViewModel? StaticInstance
    {
        get => staticInstance;
        set
        {
            staticInstance = value;
            RaisePropertyChangedStatic(nameof(StaticInstance));
        }
    }

    private int panelCount = 1;
    public int PanelCount
    {
        get => panelCount;
        set
        {
            panelCount = value;
            RaisePropertyChanged(nameof(PanelCount));
        }
    }

    private List<DesignerModel>? designersModel;
    public List<DesignerModel> DesignersModel
    {
        get => designersModel!;
        set
        {
            designersModel = value;
            RaisePropertyChanged(nameof(DesignersModel));
        }
    }





    

    private StatsDBSettingsModel? serverInfoModel;
    public StatsDBSettingsModel ServerInfoModel
    {
        get => serverInfoModel!;
        set
        {
            serverInfoModel = value;
            RaisePropertyChanged(nameof(ServerInfoModel));
        }
    }



    private string lastDBUpdate = "";
    public string LastDBUpdate
    {
        get => lastDBUpdate;
        set
        {
            lastDBUpdate = value;
            RaisePropertyChanged(nameof(LastDBUpdate));
        }
    }

    private string lastDBUpdateLocalTime = "Fetching data..";
    public string LastDBUpdateLocalTime
    {
        get => lastDBUpdateLocalTime;
        set
        {
            lastDBUpdateLocalTime = value;
            RaisePropertyChanged(nameof(LastDBUpdateLocalTime));
        }
    }

    private double updateTimeOpacity = 1;
    public double UpdateTimeOpacity
    {
        get => updateTimeOpacity;
        set
        {
            updateTimeOpacity = value;
            RaisePropertyChanged(nameof(UpdateTimeOpacity));
        }
    }

    private string statusColor = "LightGreen";
    public string StatusColor
    {
        get => statusColor;
        set
        {
            statusColor = value;
            RaisePropertyChanged(nameof(StatusColor));
        }
    }

    private string updateTimeColor = "LightGreen";
    public string UpdateTimeColor
    {
        get => updateTimeColor;
        set
        {
            updateTimeColor = value;
            RaisePropertyChanged(nameof(UpdateTimeColor));
        }
    }


    //private List<UserPanelViewModel> userPanelViewModels = [];
    //public List<UserPanelViewModel> UserPanelViewModels
    //{
    //    get => userPanelViewModels;
    //    set
    //    {
    //        userPanelViewModels = value;
    //        RaisePropertyChanged(nameof(UserPanelViewModels));
    //    }
    //}

    private List<CheckedOutCasesModel> sentOutCasesModel = [];
    public List<CheckedOutCasesModel> SentOutCasesModel
    {
        get => sentOutCasesModel;
        set
        {
            sentOutCasesModel = value;
            RaisePropertyChanged(nameof(SentOutCasesModel));
        }
    }

    //private Dictionary<string, UserPanel> userPanels = [];
    //public Dictionary<string, UserPanel> UserPanels
    //{
    //    get => userPanels;
    //    set
    //    {
    //        userPanels = value;
    //        RaisePropertyChanged(nameof(UserPanels));
    //    }
    //}

    private bool serverIsOnline = true;
    public bool ServerIsOnline
    {
        get => serverIsOnline;
        set
        {
            serverIsOnline = value;
            RaisePropertyChanged(nameof(ServerIsOnline));
        }
    }

  
    private int Counter = 10;
    public System.Timers.Timer _timer;
    //public System.Timers.Timer _orderTimer;

    public SentOutCasesViewModel()
    {
        Instance = this;
        StaticInstance = this;
        MainViewModel.Instance.SentOutCasesViewModel = this;


        _timer = new System.Timers.Timer(10000);
        _timer.Elapsed += Timer_Elapsed;
        _timer.Start();

        //_orderTimer = new System.Timers.Timer(60000);
        //_orderTimer.Elapsed += OrderTimer_Elapsed;
        //_orderTimer.Start();


        _ = GetServerInfo();

        _ = GetTheOrderInfos("both");
    }

    //private void OrderTimer_Elapsed(object? sender, ElapsedEventArgs e)
    //{
    //    if (!ServerInfoModel.ServerIsWritingDatabase)
    //    {
    //        LastDBUpdateLocalTime = DateTime.Now.ToString("MMM d - h:mm:ss tt");
    //        _ = GetTheOrderInfos("both");
    //    }
    //}


    private async void Timer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        if (ServerInfoModel is not null)
        {
            if (LastDBUpdate != ServerInfoModel.LastDBUpdate && !ServerInfoModel.ServerIsWritingDatabase)
            {
                LastDBUpdateLocalTime = DateTime.Now.ToString("MMM d - h:mm:ss tt");
                _ = GetTheOrderInfos("both");
                UpdateTimeColor = "LightGreen";
                UpdateTimeOpacity = 1;
            }

        
            _ = GetServerInfo();
            LastDBUpdate = ServerInfoModel.LastDBUpdate!;

            // creating panels corresponding to the number of designers
            if (!PanelsAddedalready)
            {
                DesignersModel = await GetDesignersModel();
                PanelCount = DesignersModel.Count;

                PanelsAddedalready = true;
                for (int i = 0; i < PanelCount; i++)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        string designerID = DesignersModel.ToArray()[i].DesignerID!;

                        UserPanel userPanel = new(designerID);
                        ColumnDefinition column = new();
                        column.Width = new GridLength(1, GridUnitType.Star);
                        column.Name = $"gridColumn{designerID}";
                        SentOutCasesPage.Instance.mainGrid.ColumnDefinitions.Add(column);
                        Grid.SetColumn(userPanel, i);
                    
                        SentOutCasesPage.Instance.mainGrid.Children.Add(userPanel);
                    });
                }
            }
        }
    }

    



    private async Task GetServerInfo()
    {
        try
        {
            ServerInfoModel = await Task.Run(GetStatsDBSettingsModel);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[{ex.LineNumber()}] {ex.Message}");
        }
    }

    private async Task GetTheOrderInfos(string designerID)
    {
        List<CheckedOutCasesModel> modelList = [];

        try
        {
        
            modelList = await GetCheckedOutCasesFromStatsDatabase(designerID);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[{ex.LineNumber()}] {ex.Message}");
        }
        

        SentOutCasesModel = modelList;
    }
}
