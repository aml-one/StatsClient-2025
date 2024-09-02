using StatsClient.MVVM.Core;
using StatsClient.MVVM.Model;
using StatsClient.MVVM.View;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using static StatsClient.MVVM.Core.MessageBoxes;
using static StatsClient.MVVM.Core.DatabaseOperations;
using static StatsClient.MVVM.Core.DatabaseConnection;
using static StatsClient.MVVM.Core.LocalSettingsDB;
using static StatsClient.MVVM.Core.Functions;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using MessageBox = System.Windows.MessageBox;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Pdf;
using System.Xml;
using TesseractOCR;
using TesseractOCR.Enums;
using Bitmap = System.Drawing.Bitmap;
using Syncfusion.PdfToImageConverter;
using System.IO.Compression;
using System.Media;
using System.Windows.Input;
using Clipboard = System.Windows.Clipboard;
using System.Net.Http;



namespace StatsClient.MVVM.ViewModel;

public class MainViewModel : ObservableObject
{
    public static readonly string LocalConfigFolderHelper = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + @"\Stats_Client\";

    #region Properties
    private static MainViewModel? instance;
    public static MainViewModel Instance
    {
        get => instance!;
        set
        {
            instance = value;
            RaisePropertyChangedStatic(nameof(Instance));
        }
    }

    private MainWindow? _mainWindow;
    public MainWindow _MainWindow
    {
        get => _mainWindow!;
        set
        {
            _mainWindow = value;
            RaisePropertyChanged(nameof(_MainWindow));
        }
    }

    private SentOutCasesViewModel? sentOutCasesViewModel;
    public SentOutCasesViewModel SentOutCasesViewModel
    {
        get => sentOutCasesViewModel!;
        set
        {
            sentOutCasesViewModel = value;
            RaisePropertyChanged(nameof(SentOutCasesViewModel));
        }
    }

    private SmartOrderNamesViewModel? smartOrderNamesViewModel;
    public SmartOrderNamesViewModel SmartOrderNamesViewModel
    {
        get => smartOrderNamesViewModel!;
        set
        {
            smartOrderNamesViewModel = value;
            RaisePropertyChanged(nameof(SmartOrderNamesViewModel));
        }
    }


    private bool appJustStarted = true;
    public bool AppJustStarted
    {
        get => appJustStarted;
        set
        {
            appJustStarted = value;
            RaisePropertyChanged(nameof(AppJustStarted));
        }
    }
    
    private bool autoUpdateAtStart = false;
    public bool AutoUpdateAtStart
    {
        get => autoUpdateAtStart;
        set
        {
            autoUpdateAtStart = value;
            RaisePropertyChanged(nameof(AutoUpdateAtStart));
        }
    }
    
    private bool updateMessagePresented = false;
    public bool UpdateMessagePresented
    {
        get => updateMessagePresented;
        set
        {
            updateMessagePresented = value;
            RaisePropertyChanged(nameof(UpdateMessagePresented));
        }
    }

    
    

    private double appVersionDouble = 0;
    public double AppVersionDouble
    {
        get => appVersionDouble;
        set
        {
            appVersionDouble = value;
            RaisePropertyChanged(nameof(AppVersionDouble));
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
    
    private string latestAppVersion = "0";
    public string LatestAppVersion
    {
        get => latestAppVersion;
        set
        {
            latestAppVersion = value;
            RaisePropertyChanged(nameof(LatestAppVersion));
        }
    }
    
    private bool threeShapeServerIsDown = false;
    public bool ThreeShapeServerIsDown
    {
        get => threeShapeServerIsDown;
        set
        {
            threeShapeServerIsDown = value;
            RaisePropertyChanged(nameof(ThreeShapeServerIsDown));
        }
    }
    
    private bool serverIsWritingDatabase = false;
    public bool ServerIsWritingDatabase
    {
        get => serverIsWritingDatabase;
        set
        {
            serverIsWritingDatabase = value;
            RaisePropertyChanged(nameof(ServerIsWritingDatabase));
        }
    }
    
    private bool updateAvailable = false;
    public bool UpdateAvailable
    {
        get => updateAvailable;
        set
        {
            updateAvailable = value;
            RaisePropertyChanged(nameof(UpdateAvailable));
        }
    }
    
    private bool appIsFullyLoaded = false;
    public bool AppIsFullyLoaded
    {
        get => appIsFullyLoaded;
        set
        {
            appIsFullyLoaded = value;
            RaisePropertyChanged(nameof(AppIsFullyLoaded));
        }
    }
    
    private bool firstRun = true;
    public bool FirstRun
    {
        get => firstRun;
        set
        {
            firstRun = value;
            RaisePropertyChanged(nameof(FirstRun));
        }
    }

    private bool listUpdateable = false;
    public bool ListUpdateable
    {
        get => listUpdateable;
        set
        {
            listUpdateable = value;
            RaisePropertyChanged(nameof(ListUpdateable));
        }
    }
    
    private bool designerOpen = false;
    public bool DesignerOpen
    {
        get => designerOpen;
        set
        {
            designerOpen = value;
            RaisePropertyChanged(nameof(DesignerOpen));
        }
    }
    
    private string designerOpenToolTip = "";
    public string DesignerOpenToolTip
    {
        get => designerOpenToolTip;
        set
        {
            designerOpenToolTip = value;
            RaisePropertyChanged(nameof(DesignerOpenToolTip));
        }
    }
    
    private string triosInboxFolder = "";
    public string TriosInboxFolder
    {
        get => triosInboxFolder;
        set
        {
            triosInboxFolder = value;
            RaisePropertyChanged(nameof(TriosInboxFolder));
        }
    }
    
    private bool showUnitNumbers = false;
    public bool ShowUnitNumbers
    {
        get => showUnitNumbers;
        set
        {
            showUnitNumbers = value;
            RaisePropertyChanged(nameof(ShowUnitNumbers));
        }
    }
    
    private bool showBottomInfoBar = false;
    public bool ShowBottomInfoBar
    {
        get => showBottomInfoBar;
        set
        {
            showBottomInfoBar = value;
            RaisePropertyChanged(nameof(ShowBottomInfoBar));
        }
    }

    private string serverStatus = "Idle";
    public string ServerStatus
    {
        get => serverStatus;
        set
        {
            serverStatus = value;
            RaisePropertyChanged(nameof(ServerStatus));
        }
    }
    
    private int bottomBarSize = 35;
    public int BottomBarSize
    {
        get => bottomBarSize;
        set
        {
            bottomBarSize = value;
            RaisePropertyChanged(nameof(BottomBarSize));
        }
    }
    
        
    private int digiPrescriptionsTodayCount = 0;
    public int DigiPrescriptionsTodayCount
    {
        get => digiPrescriptionsTodayCount;
        set
        {
            digiPrescriptionsTodayCount = value;
            RaisePropertyChanged(nameof(DigiPrescriptionsTodayCount));
        }
    }
    
    private int digiCasesIn3ShapeTodayCount = 0;
    public int DigiCasesIn3ShapeTodayCount
    {
        get => digiCasesIn3ShapeTodayCount;
        set
        {
            digiCasesIn3ShapeTodayCount = value;
            RaisePropertyChanged(nameof(DigiCasesIn3ShapeTodayCount));
        }
    }
    
    private int sentOutIssuesCount = 0;
    public int SentOutIssuesCount
    {
        get => sentOutIssuesCount;
        set
        {
            sentOutIssuesCount = value;
            RaisePropertyChanged(nameof(SentOutIssuesCount));
        }
    }

    private List<IssuesWithCasesModel> issuesWithCasesList = [];
    public List<IssuesWithCasesModel> IssuesWithCasesList
    {
        get => issuesWithCasesList;
        set
        {
            issuesWithCasesList = value;
            RaisePropertyChanged(nameof(IssuesWithCasesList));
        }
    }

    private List<DebugMessagesModel> debugMessages = [];
    public List<DebugMessagesModel> DebugMessages
    {
        get => debugMessages;
        set
        {
            debugMessages = value;
            RaisePropertyChanged(nameof(DebugMessages));
        }
    }
    

    private string activeSearchString = "";
    public string ActiveSearchString
    {
        get => activeSearchString;
        set
        {
            activeSearchString = value;
            RaisePropertyChanged(nameof(ActiveSearchString));
        }
    }
    
    private string activeFilterInUse = "";
    public string ActiveFilterInUse
    {
        get => activeFilterInUse;
        set
        {
            activeFilterInUse = value;
            RaisePropertyChanged(nameof(ActiveFilterInUse));
        }
    }

    private string filterString = "";
    public string FilterString
    {
        get => filterString;
        set
        {
            filterString = value;
            RaisePropertyChanged(nameof(FilterString));
        }
    }

    private string threeShapeDirectoryHelper = "";
    public string ThreeShapeDirectoryHelper
    {
        get => threeShapeDirectoryHelper;
        set
        {
            threeShapeDirectoryHelper = value;
            RaisePropertyChanged(nameof(ThreeShapeDirectoryHelper));
        }
    }

    private string serverFriendlyNameHelper = "";
    public string ServerFriendlyNameHelper
    {
        get => serverFriendlyNameHelper;
        set
        {
            serverFriendlyNameHelper = value;
            RaisePropertyChanged(nameof(ServerFriendlyNameHelper));
        }
    }

    private bool easeUpSearch = false;
    public bool EaseUpSearch
    {
        get => easeUpSearch;
        set
        {
            easeUpSearch = value;
            RaisePropertyChanged(nameof(EaseUpSearch));
        }
    }
    
    private bool isContextMenuOpen = false;
    public bool IsContextMenuOpen
    {
        get => isContextMenuOpen;
        set
        {
            isContextMenuOpen = value;
            RaisePropertyChanged(nameof(IsContextMenuOpen));
        }
    }

    private int searchLimit = 100;
    public int SearchLimit
    {
        get => searchLimit;
        set
        {
            searchLimit = value;
            RaisePropertyChanged(nameof(SearchLimit));
        }
    }

    private string thisSite = "";
    public string ThisSite
    {
        get => thisSite;
        set
        {
            thisSite = value;
            RaisePropertyChanged(nameof(ThisSite));
        }
    }

    private int todayCasesCount = 0;
    public int TodayCasesCount
    {
        get => todayCasesCount;
        set
        {
            todayCasesCount = value;
            RaisePropertyChanged(nameof(TodayCasesCount));
        }
    }

    private bool tempSearchLimitIgnore = false;
    public bool TempSearchLimitIgnore
    {
        get => tempSearchLimitIgnore;
        set
        {
            tempSearchLimitIgnore = value;
            RaisePropertyChanged(nameof(TempSearchLimitIgnore));
        }
    }



    private Visibility loadingPanelVisibility = Visibility.Hidden;
    public Visibility LoadingPanelVisibility
    {
        get => loadingPanelVisibility;
        set
        {
            loadingPanelVisibility = value;
            RaisePropertyChanged(nameof(LoadingPanelVisibility));
        }
    }

    #region NOTIFICATION MESSAGE PROPERTIES
    

    private Visibility isNotificationMessageShows = Visibility.Hidden;
    public Visibility IsNotificationMessageShows
    {
        get => isNotificationMessageShows;
        set
        {
            isNotificationMessageShows = value;
            RaisePropertyChanged(nameof(IsNotificationMessageShows));
        }
    }

    private Thickness notificationMessagePosition = new (15,0,0,20);
    public Thickness NotificationMessagePosition
    {
        get => notificationMessagePosition;
        set
        {
            notificationMessagePosition = value;
            RaisePropertyChanged(nameof(NotificationMessagePosition));
        }
    }
    
    private string notificationMessageTitle = "";
    public string NotificationMessageTitle
    {
        get => notificationMessageTitle;
        set
        {
            notificationMessageTitle = value;
            RaisePropertyChanged(nameof(NotificationMessageTitle));
        }
    }

    private string notificationMessageBody = "";
    public string NotificationMessageBody
    {
        get => notificationMessageBody;
        set
        {
            notificationMessageBody = value;
            RaisePropertyChanged(nameof(NotificationMessageBody));
        }
    }
    
    private string notificationMessageGridPosition = "1";
    public string NotificationMessageGridPosition
    {
        get => notificationMessageGridPosition;
        set
        {
            notificationMessageGridPosition = value;
            RaisePropertyChanged(nameof(NotificationMessageGridPosition));
        }
    }
    
    private VerticalAlignment notificationMessageVertAlignment = VerticalAlignment.Bottom;
    public VerticalAlignment NotificationMessageVertAlignment
    {
        get => notificationMessageVertAlignment;
        set
        {
            notificationMessageVertAlignment = value;
            RaisePropertyChanged(nameof(NotificationMessageVertAlignment));
        }
    }

    private string notificationMessageIcon = @"\Images\MessageIcons\Info.png";
    public string NotificationMessageIcon
    {
        get => notificationMessageIcon;
        set
        {
            notificationMessageIcon = value;
            RaisePropertyChanged(nameof(NotificationMessageIcon));
        }
    }
    #endregion NOTIFICATION MESSAGE PROPERTIES

    private List<ThreeShapeOrdersModel> current3ShapeOrderList = [];
    public List<ThreeShapeOrdersModel> Current3ShapeOrderList
    {
        get => current3ShapeOrderList;
        set
        {
            current3ShapeOrderList = value;
            RaisePropertyChanged(nameof(Current3ShapeOrderList));
        }
    }

    #region FOLDER SUBSCRIPTION & PENDING DIGI CASES PROPERTIES
    
    private List<string> pendingDigiNumbersWaitingToCollect = [];
    public List<string> PendingDigiNumbersWaitingToCollect
    {
        get => pendingDigiNumbersWaitingToCollect;
        set
        {
            pendingDigiNumbersWaitingToCollect = value;
            RaisePropertyChanged(nameof(PendingDigiNumbersWaitingToCollect));
        }
    }
    
    private List<ProcessedPanNumberModel> pendingDigiNumbersWaitingToProcess = [];
    public List<ProcessedPanNumberModel> PendingDigiNumbersWaitingToProcess
    {
        get => pendingDigiNumbersWaitingToProcess;
        set
        {
            pendingDigiNumbersWaitingToProcess = value;
            RaisePropertyChanged(nameof(PendingDigiNumbersWaitingToProcess));
        }
    }
    
    private int pendingDigiNumbersWaitingToCollectInt = 0;
    public int PendingDigiNumbersWaitingToCollectInt
    {
        get => pendingDigiNumbersWaitingToCollectInt;
        set
        {
            pendingDigiNumbersWaitingToCollectInt = value;
            RaisePropertyChanged(nameof(PendingDigiNumbersWaitingToCollectInt));
        }
    }
    
    private int pendingDigiNumbersWaitingToProcessInt = 0;
    public int PendingDigiNumbersWaitingToProcessInt
    {
        get => pendingDigiNumbersWaitingToProcessInt;
        set
        {
            pendingDigiNumbersWaitingToProcessInt = value;
            RaisePropertyChanged(nameof(PendingDigiNumbersWaitingToProcessInt));
        }
    }
    
    private string selectedPendingDigiNumber = "";
    public string SelectedPendingDigiNumber
    {
        get => selectedPendingDigiNumber;
        set
        {
            selectedPendingDigiNumber = value;
            RaisePropertyChanged(nameof(SelectedPendingDigiNumber));
        }
    }
    
    private string pendingDigiCasesReplacementName = "";
    public string PendingDigiCasesReplacementName
    {
        get => pendingDigiCasesReplacementName;
        set
        {
            pendingDigiCasesReplacementName = value;
            RaisePropertyChanged(nameof(PendingDigiCasesReplacementName));
            if (!string.IsNullOrEmpty(value))
                WriteLocalSetting("PendingDigiCasesReplacementName", value);
        }
    }
    
    private string fsubscrTargetFolder = "";
    public string FsubscrTargetFolder
    {
        get => fsubscrTargetFolder;
        set
        {
            fsubscrTargetFolder = value;
            RaisePropertyChanged(nameof(FsubscrTargetFolder));
        }
    }
    
    private List<string> newlyArrivedDigitalCasesList = [];
    public List<string> NewlyArrivedDigitalCasesList
    {
        get => newlyArrivedDigitalCasesList;
        set
        {
            newlyArrivedDigitalCasesList = value;
            RaisePropertyChanged(nameof(NewlyArrivedDigitalCasesList));
        }
    }
    
    private string fsSearchString = "";
    public string FsSearchString
    {
        get => fsSearchString;
        set
        {
            fsSearchString = value;
            RaisePropertyChanged(nameof(FsSearchString));
        }
    }

    private string fsLastDatabaseUpdate = "";
    public string FsLastDatabaseUpdate
    {
        get => fsLastDatabaseUpdate;
        set
        {
            fsLastDatabaseUpdate = value;
            RaisePropertyChanged(nameof(FsLastDatabaseUpdate));
        }
    }
    
    private string fsCountedEntries = "-";
    public string FsCountedEntries
    {
        get => fsCountedEntries;
        set
        {
            fsCountedEntries = value;
            RaisePropertyChanged(nameof(FsCountedEntries));
        }
    }

    private List<FolderSubscriptionModel> folderSubscriptionList = [];
    public List<FolderSubscriptionModel> FolderSubscriptionList
    {
        get => folderSubscriptionList;
        set
        {
            folderSubscriptionList = value;
            RaisePropertyChanged(nameof(FolderSubscriptionList));
        }
    }

    private FolderSubscriptionModel? fsSelectedFolderObject;
    public FolderSubscriptionModel FsSelectedFolderObject
    {
        get => fsSelectedFolderObject!;
        set
        {
            fsSelectedFolderObject = value;
            RaisePropertyChanged(nameof(FsSelectedFolderObject));
        }
    }

    private bool fsCopyPanelShows = false;
    public bool FsCopyPanelShows
    {
        get => fsCopyPanelShows;
        set
        {
            fsCopyPanelShows = value;
            RaisePropertyChanged(nameof(FsCopyPanelShows));
            if (value == true)
                fsNotificationTimer.Start();
        }
    }

    private string fsCustomNumber = "";
    public string FsCustomNumber
    {
        get => fsCustomNumber;
        set
        {
            fsCustomNumber = value;
            RaisePropertyChanged(nameof(FsCustomNumber));
        }
    }

    #endregion FOLDER SUBSCRIPTION & PENDING DIGI CASES PROPERTIES

    #region BuildingUpDates properties
    private string restDayStart = " 0:01:00.000";
    public string RestDayStart
    {
        get => restDayStart;
        set
        {
            restDayStart = value;
            RaisePropertyChanged(nameof(RestDayStart));
        }
    }
    private string restDayEnd = " 23:59:59.999";
    public string RestDayEnd
    {
        get => restDayEnd;
        set
        {
            restDayEnd = value;
            RaisePropertyChanged(nameof(RestDayEnd));
        }
    }

    private string? dtLastTwoDayNames;
    public string? DtLastTwoDayNames
    {
        get => dtLastTwoDayNames;
        set
        {
            dtLastTwoDayNames = value;
            RaisePropertyChanged(nameof(DtLastTwoDayNames));
        }
    }
    private string? dtLastThreeDayNames;
    public string? DtLastThreeDayNames
    {
        get => dtLastThreeDayNames;
        set
        {
            dtLastThreeDayNames = value;
            RaisePropertyChanged(nameof(DtLastThreeDayNames));
        }
    }

    private string? dtToday;
    public string? DtToday
    {
        get => dtToday;
        set
        {
            dtToday = value;
            RaisePropertyChanged(nameof(DtToday));
        }
    }

    private string? dtYesterday;
    public string? DtYesterday
    {
        get => dtYesterday;
        set
        {
            dtYesterday = value;
            RaisePropertyChanged(nameof(DtYesterday));
        }
    }
    private string? dtLastFriday;
    public string? DtLastFriday
    {
        get => dtLastFriday;
        set
        {
            dtLastFriday = value;
            RaisePropertyChanged(nameof(DtLastFriday));
        }
    }
    private string? dtThisMonday;
    public string? DtThisMonday
    {
        get => dtThisMonday;
        set
        {
            dtThisMonday = value;
            RaisePropertyChanged(nameof(DtThisMonday));
        }
    }
    private string? dtLastWeekFriday;
    public string? DtLastWeekFriday
    {
        get => dtLastWeekFriday;
        set
        {
            dtLastWeekFriday = value;
            RaisePropertyChanged(nameof(DtLastWeekFriday));
        }
    }
    private string? dtLastWeekMonday;
    public string? DtLastWeekMonday
    {
        get => dtLastWeekMonday;
        set
        {
            dtLastWeekMonday = value;
            RaisePropertyChanged(nameof(DtLastWeekMonday));
        }
    }
    private string? dtLastWeekSunday;
    public string? DtLastWeekSunday
    {
        get => dtLastWeekSunday;
        set
        {
            dtLastWeekSunday = value;
            RaisePropertyChanged(nameof(DtLastWeekSunday));
        }
    }
    private string? dtOneMonthBack;
    public string? DtOneMonthBack
    {
        get => dtOneMonthBack;
        set
        {
            dtOneMonthBack = value;
            RaisePropertyChanged(nameof(DtOneMonthBack));
        }
    }
    private string? dtTwoMonthsBack;
    public string? DtTwoMonthsBack
    {
        get => dtTwoMonthsBack;
        set
        {
            dtTwoMonthsBack = value;
            RaisePropertyChanged(nameof(DtTwoMonthsBack));
        }
    }
    private string? dtLastTwoDays;
    public string? DtLastTwoDays
    {
        get => dtLastTwoDays;
        set
        {
            dtLastTwoDays = value;
            RaisePropertyChanged(nameof(DtLastTwoDays));
        }
    }
    private string? dtLastThreeDays;
    public string? DtLastThreeDays
    {
        get => dtLastThreeDays;
        set
        {
            dtLastThreeDays = value;
            RaisePropertyChanged(nameof(DtLastThreeDays));
        }
    }
    private string? dtLastSevenDays;
    public string? DtLastSevenDays
    {
        get => dtLastSevenDays;
        set
        {
            dtLastSevenDays = value;
            RaisePropertyChanged(nameof(DtLastSevenDays));
        }
    }
    #endregion BuildingUpDates properties

    private bool allowToShowProgressBar = true;
    public bool AllowToShowProgressBar
    {
        get => allowToShowProgressBar;
        set
        {
            allowToShowProgressBar = value;
            RaisePropertyChanged(nameof(AllowToShowProgressBar));
        }
    }

    private bool is3ShapeTabSelected = true;
    public bool Is3ShapeTabSelected
    {
        get => is3ShapeTabSelected;
        set
        {
            is3ShapeTabSelected = value;
            RaisePropertyChanged(nameof(Is3ShapeTabSelected));
        }
    }

    private bool allowThreeShapeOrderListUpdates = true;
    public bool AllowThreeShapeOrderListUpdates
    {
        get => allowThreeShapeOrderListUpdates;
        set
        {
            allowThreeShapeOrderListUpdates = value;
            RaisePropertyChanged(nameof(AllowThreeShapeOrderListUpdates));
        }
    }

    private bool digiCase = true;
    public bool DigiCase
    {
        get => digiCase;
        set
        {
            digiCase = value;
            RaisePropertyChanged(nameof(DigiCase));
        }
    }

    private bool searchOnlyInFileNames = true;
    public bool SearchOnlyInFileNames
    {
        get => searchOnlyInFileNames;
        set
        {
            searchOnlyInFileNames = value;
            RaisePropertyChanged(nameof(SearchOnlyInFileNames));
        }
    }

    private int serverID;
    public int ServerID
    {
        get => serverID;
        set
        {
            serverID = value;
            RaisePropertyChanged(nameof(ServerID));
        }
    }

    private string orderBeingWatched = "";
    public string OrderBeingWatched
    {
        get => orderBeingWatched;
        set
        {
            orderBeingWatched = value;
            RaisePropertyChanged(nameof(OrderBeingWatched));
        }
    }
    
    
    private string lastDCASUpdate = "";
    public string LastDCASUpdate
    {
        get => lastDCASUpdate;
        set
        {
            lastDCASUpdate = value;
            RaisePropertyChanged(nameof(LastDCASUpdate));
        }
    }
    
    private bool isDCASIsActive = false;
    public bool IsDCASIsActive
    {
        get => isDCASIsActive;
        set
        {
            isDCASIsActive = value;
            RaisePropertyChanged(nameof(IsDCASIsActive));
        }
    }



 
    private int orderCount = 0;
    public int OrderCount
    {
        get => orderCount;
        set
        {
            orderCount = value;
            RaisePropertyChanged(nameof(OrderCount));
        }
    }

    private string orderCountText = "";
    public string OrderCountText
    {
        get => orderCountText;
        set
        {
            orderCountText = value;
            RaisePropertyChanged(nameof(OrderCountText));
            if (value.StartsWith("0"))
                _MainWindow.pb3ShapeProgressBar.Value = 0;
        }
    }

    private string searchString = "";
    public string SearchString
    {
        get => searchString;
        set
        {
            searchString = value;
            RaisePropertyChanged(nameof(SearchString));
        }
    }

    private string[] groupBy = ["None", "Customer", "Scan Source", "Case Status", "Last touched By"];
    public string[] GroupBy
    {
        get => groupBy;
        set
        {
            groupBy = value;
            RaisePropertyChanged(nameof(GroupBy));
        }
    }

    private ThreeShapeOrdersModel? threeShapeObject;
    public ThreeShapeOrdersModel? ThreeShapeObject
    {
        get => threeShapeObject;
        set
        {
            threeShapeObject = value;
            RaisePropertyChanged(nameof(ThreeShapeObject));
        }
    }

    private ThreeShapeOrdersModel? selectedItem;
    public ThreeShapeOrdersModel? SelectedItem
    {
        get => selectedItem;
        set
        {
            selectedItem = value;
            RaisePropertyChanged(nameof(SelectedItem));
        }
    }

    private ICollectionView? dataView;
    private ICollectionView? DataView
    {
        get => dataView;
        set
        {
            dataView = value;
            RaisePropertyChanged(nameof(DataView));
        }
    }

    private Dictionary<string, bool> expandStates = [];
    public Dictionary<string, bool> ExpandStates
    {
        get => expandStates;
        set
        {
            expandStates = value;
            RaisePropertyChanged(nameof(ExpandStates));
        }
    }

    private Dictionary<string, Brush> itemBackground = [];
    public Dictionary<string, Brush> ItemBackground
    {
        get => itemBackground;
        set
        {
            itemBackground = value;
            RaisePropertyChanged(nameof(ItemBackground));
        }
    }

    private Dictionary<Brush, bool> digiSystemColors = [];
    public Dictionary<Brush, bool> DigiSystemColors
    {
        get => digiSystemColors;
        set
        {
            digiSystemColors = value;
            RaisePropertyChanged(nameof(DigiSystemColors));
        }
    }

    private int newTriosCaseInInboxCount = 0;
    public int NewTriosCaseInInboxCount
    {
        get => newTriosCaseInInboxCount;
        set
        {
            newTriosCaseInInboxCount = value;
            RaisePropertyChanged(nameof(NewTriosCaseInInboxCount));
        }
    }
    
    private int newDigiCaseArrivedCount = 0;
    public int NewDigiCaseArrivedCount
    {
        get => newDigiCaseArrivedCount;
        set
        {
            newDigiCaseArrivedCount = value;
            RaisePropertyChanged(nameof(NewDigiCaseArrivedCount));
        }
    }
    
    private int totalNewDigiCaseWithoutInHouseCases = 0;
    public int TotalNewDigiCaseWithoutInHouseCases
    {
        get => totalNewDigiCaseWithoutInHouseCases;
        set
        {
            totalNewDigiCaseWithoutInHouseCases = value;
            RaisePropertyChanged(nameof(TotalNewDigiCaseWithoutInHouseCases));
        }
    }
    
    private string openUpColorCheckWindowMenuItemTitle = "Open up ColorCheck window";
    public string OpenUpColorCheckWindowMenuItemTitle
    {
        get => openUpColorCheckWindowMenuItemTitle;
        set
        {
            openUpColorCheckWindowMenuItemTitle = value;
            RaisePropertyChanged(nameof(OpenUpColorCheckWindowMenuItemTitle));
        }
    }

    #region PRESCRIPTION MAKER PROPERTIES
    private FileStream documentStreamPixelCheck;
    public FileStream DocumentStreamPixelCheck
    {
        get => documentStreamPixelCheck;
        set
        {
            documentStreamPixelCheck = value;
            RaisePropertyChanged(nameof(DocumentStreamPixelCheck));
        }
    }
    
    private FileStream documentStreamFinalPrescription;
    public FileStream DocumentStreamFinalPrescription
    {
        get => documentStreamFinalPrescription;
        set
        {
            documentStreamFinalPrescription = value;
            RaisePropertyChanged(nameof(DocumentStreamFinalPrescription));
        }
    }

    private Bitmap prescriptionImageForProcess;
    public Bitmap PrescriptionImageForProcess
    {
        get => prescriptionImageForProcess;
        set
        {
            prescriptionImageForProcess = value;
            RaisePropertyChanged(nameof(PrescriptionImageForProcess));
        }
    }


    private Stream documentStreamUser;
    public Stream DocumentStreamUser
    {
        get => documentStreamUser;
        set
        {
            documentStreamUser = value;
            RaisePropertyChanged(nameof(DocumentStreamUser));
        }
    }

    
    private double pmLastPrescriptionSize = 0;
    public double PmLastPrescriptionSize
    {
        get => pmLastPrescriptionSize;
        set
        {
            pmLastPrescriptionSize = value;
            RaisePropertyChanged(nameof(PmLastPrescriptionSize));
        }
    }
    
    private string pmNextPanNumberInList = "";
    public string PmNextPanNumberInList
    {
        get => pmNextPanNumberInList;
        set
        {
            pmNextPanNumberInList = value;
            RaisePropertyChanged(nameof(PmNextPanNumberInList));
        }
    }
    
    private string lastIteroZipFileId = "";
    public string LastIteroZipFileId
    {
        get => lastIteroZipFileId;
        set
        {
            lastIteroZipFileId = value;
            RaisePropertyChanged(nameof(LastIteroZipFileId));
        }
    }
    
    private string pmLastTakenPanNumber = "";
    public string PmLastTakenPanNumber
    {
        get => pmLastTakenPanNumber;
        set
        {
            pmLastTakenPanNumber = value;
            RaisePropertyChanged(nameof(PmLastTakenPanNumber));
        }
    }
    
    private Visibility processingDigiPrescriptionNow = Visibility.Collapsed;
    public Visibility ProcessingDigiPrescriptionNow
    {
        get => processingDigiPrescriptionNow;
        set
        {
            processingDigiPrescriptionNow = value;
            RaisePropertyChanged(nameof(ProcessingDigiPrescriptionNow));
        }
    }
    
    private Visibility showingTakeANumberPanel = Visibility.Hidden;
    public Visibility ShowingTakeANumberPanel
    {
        get => showingTakeANumberPanel;
        set
        {
            showingTakeANumberPanel = value;
            RaisePropertyChanged(nameof(ShowingTakeANumberPanel));
        }
    }
    
    private Visibility pmRushButtonShows = Visibility.Hidden;
    public Visibility PmRushButtonShows
    {
        get => pmRushButtonShows;
        set
        {
            pmRushButtonShows = value;
            RaisePropertyChanged(nameof(PmRushButtonShows));
        }
    }
    
    private Visibility pmSendToButtonShows = Visibility.Hidden;
    public Visibility PmSendToButtonShows
    {
        get => pmSendToButtonShows;
        set
        {
            pmSendToButtonShows = value;
            RaisePropertyChanged(nameof(PmSendToButtonShows));
        }
    }

    private List<string> pmSendToList = [];
    public List<string> PmSendToList
    {
        get => pmSendToList;
        set
        {
            pmSendToList = value;
            RaisePropertyChanged(nameof(PmSendToList));
        }
    }
    
    private string pmNewSentToName = "";
    public string PmNewSentToName
    {
        get => pmNewSentToName;
        set
        {
            pmNewSentToName = value;
            RaisePropertyChanged(nameof(PmNewSentToName));
        }
    }
    
    private string pmSelectedSentTo = "";
    public string PmSelectedSentTo
    {
        get => pmSelectedSentTo;
        set
        {
            pmSelectedSentTo = value;
            RaisePropertyChanged(nameof(PmSelectedSentTo));
        }
    }
    
    private string pmSelectedSendToEntry = "";
    public string PmSelectedSendToEntry
    {
        get => pmSelectedSendToEntry;
        set
        {
            pmSelectedSendToEntry = value;
            RaisePropertyChanged(nameof(PmSelectedSendToEntry));
        }
    }
    
    private List<string> pmPanNumberList = [];
    public List<string> PmPanNumberList
    {
        get => pmPanNumberList;
        set
        {
            pmPanNumberList = value;
            RaisePropertyChanged(nameof(PmPanNumberList));
            if (value.Count > 0)
                PmNextPanNumberInList = value[0];
        }
    }
    
    private ImageSource? pmSavedPrescription;
    public ImageSource PmSavedPrescription
    {
        get => pmSavedPrescription!;
        set
        {
            pmSavedPrescription = value;
            RaisePropertyChanged(nameof(PmSavedPrescription));
        }
    }
    
    private string pmWatchedPdfFolder = "Click here to browse..";
    public string PmWatchedPdfFolder
    {
        get => pmWatchedPdfFolder;
        set
        {
            pmWatchedPdfFolder = value;
            RaisePropertyChanged(nameof(PmWatchedPdfFolder));
        }
    }
    
    private string pmAddNewNumber = "";
    public string PmAddNewNumber
    {
        get => pmAddNewNumber;
        set
        {
            pmAddNewNumber = value;
            RaisePropertyChanged(nameof(PmAddNewNumber));
        }
    }
    
    private string nextPanNumberGlobal = "";
    public string NextPanNumberGlobal
    {
        get => nextPanNumberGlobal;
        set
        {
            nextPanNumberGlobal = value;
            RaisePropertyChanged(nameof(NextPanNumberGlobal));
        }
    }
    
    private string fullPathGlobal = "";
    public string FullPathGlobal
    {
        get => fullPathGlobal;
        set
        {
            fullPathGlobal = value;
            RaisePropertyChanged(nameof(FullPathGlobal));
        }
    }
    
    private string sironaOrderNumber = "";
    public string SironaOrderNumber
    {
        get => sironaOrderNumber;
        set
        {
            sironaOrderNumber = value;
            RaisePropertyChanged(nameof(SironaOrderNumber));
        }
    }
    
    private string pDFTemp = "";
    public string PDFTemp
    {
        get => pDFTemp;
        set
        {
            pDFTemp = value;
            RaisePropertyChanged(nameof(PDFTemp));
        }
    }
    
    private bool pmOpenUpPrescriptionsBool = false;
    public bool PmOpenUpPrescriptionsBool
    {
        get => pmOpenUpPrescriptionsBool;
        set
        {
            pmOpenUpPrescriptionsBool = value;
            RaisePropertyChanged(nameof(PmOpenUpPrescriptionsBool));
        }
    }
    
    private string pageHeaderIsHigh = "";
    public string PageHeaderIsHigh
    {
        get => pageHeaderIsHigh;
        set
        {
            pageHeaderIsHigh = value;
            RaisePropertyChanged(nameof(PageHeaderIsHigh));
        }
    }
    
    private string pdfPageCount = "";
    public string PdfPageCount
    {
        get => pdfPageCount;
        set
        {
            pdfPageCount = value;
            RaisePropertyChanged(nameof(PdfPageCount));
        }
    }
    
    private bool isItSironaPrescription = false;
    public bool IsItSironaPrescription
    {
        get => isItSironaPrescription;
        set
        {
            isItSironaPrescription = value;
            RaisePropertyChanged(nameof(IsItSironaPrescription));
        }
    }
    
    private string pmFinalPrescriptionsFolder = "Click here to browse..";
    public string PmFinalPrescriptionsFolder
    {
        get => pmFinalPrescriptionsFolder;
        set
        {
            pmFinalPrescriptionsFolder = value;
            RaisePropertyChanged(nameof(PmFinalPrescriptionsFolder));
        }
    }
    
    private string pmSironaScansFolder = "Click here to browse..";
    public string PmSironaScansFolder
    {
        get => pmSironaScansFolder;
        set
        {
            pmSironaScansFolder = value;
            RaisePropertyChanged(nameof(PmSironaScansFolder));
        }
    }
    
    private string pmIteroExportFolder = "Click here to browse..";
    public string PmIteroExportFolder
    {
        get => pmIteroExportFolder;
        set
        {
            pmIteroExportFolder = value;
            RaisePropertyChanged(nameof(PmIteroExportFolder));
        }
    }
    
    private string pmDownloadFolder = "Click here to browse..";
    public string PmDownloadFolder
    {
        get => pmDownloadFolder;
        set
        {
            pmDownloadFolder = value;
            RaisePropertyChanged(nameof(PmDownloadFolder));
        }
    }
    
    private double notificationProgressBarValue = 0;
    public double NotificationProgressBarValue
    {
        get => notificationProgressBarValue;
        set
        {
            notificationProgressBarValue = value;
            RaisePropertyChanged(nameof(NotificationProgressBarValue));
        }
    }


    #endregion PRESCRIPTION MAKER PROPERTIES

    #region PAN COLOR CHECKER PROPERTIES
    private string pcPanColor = "#56595F";
    public string PcPanColor
    {
        get => pcPanColor;
        set
        {
            pcPanColor = value;
            RaisePropertyChanged(nameof(PcPanColor));
        }
    }
    
    private string pcPanColorFriendlyName = "Check pan color";
    public string PcPanColorFriendlyName
    {
        get => pcPanColorFriendlyName;
        set
        {
            pcPanColorFriendlyName = value;
            RaisePropertyChanged(nameof(PcPanColorFriendlyName));
        }
    }
    
    private string pcPanNumber = "";
    public string PcPanNumber
    {
        get => pcPanNumber;
        set
        {
            pcPanNumber = value;
            RaisePropertyChanged(nameof(PcPanNumber));
        }
    }

    #endregion PAN COLOR CHECKER PROPERTIES

    #region ToolBar Buttons
    private Visibility tBBOpenDetails = Visibility.Hidden;
    public Visibility TBBOpenDetails
    {
        get => tBBOpenDetails;
        set
        {
            tBBOpenDetails = value;
            RaisePropertyChanged(nameof(TBBOpenDetails));
        }
    }

    private Visibility tBBGenerateStCopy = Visibility.Hidden;
    public Visibility TBBGenerateStCopy
    {
        get => tBBGenerateStCopy;
        set
        {
            tBBGenerateStCopy = value;
            RaisePropertyChanged(nameof(TBBGenerateStCopy));
        }
    }


    private Visibility tBBRename = Visibility.Hidden;
    public Visibility TBBRename
    {
        get => tBBRename;
        set
        {
            tBBRename = value;
            RaisePropertyChanged(nameof(TBBRename));
        }
    }
    #endregion ToolBar Buttons


    #region Settings Tab Properties
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
    
    private bool cbSettingShowEmptyPanCount = true;
    public bool CbSettingShowEmptyPanCount
    {
        get => cbSettingShowEmptyPanCount;
        set
        {
            cbSettingShowEmptyPanCount = value;
            RaisePropertyChanged(nameof(CbSettingShowEmptyPanCount));
        }
    }
    
    private bool cbSettingShowDigiCases = true;
    public bool CbSettingShowDigiCases
    {
        get => cbSettingShowDigiCases;
        set
        {
            cbSettingShowDigiCases = value;
            RaisePropertyChanged(nameof(CbSettingShowDigiCases));
        }
    }
    
    private bool cbSettingShowPendingDigiCases = false;
    public bool CbSettingShowPendingDigiCases
    {
        get => cbSettingShowPendingDigiCases;
        set
        {
            cbSettingShowPendingDigiCases = value;
            RaisePropertyChanged(nameof(CbSettingShowPendingDigiCases));
        }
    }
    
    private bool cbSettingIncludePendingDigiCasesInNewlyArrived = true;
    public bool CbSettingIncludePendingDigiCasesInNewlyArrived
    {
        get => cbSettingIncludePendingDigiCasesInNewlyArrived;
        set
        {
            cbSettingIncludePendingDigiCasesInNewlyArrived = value;
            RaisePropertyChanged(nameof(CbSettingIncludePendingDigiCasesInNewlyArrived));
        }
    }
    
    private bool cbSettingShowDigiPrescriptionsCount = true;
    public bool CbSettingShowDigiPrescriptionsCount
    {
        get => cbSettingShowDigiPrescriptionsCount;
        set
        {
            cbSettingShowDigiPrescriptionsCount = value;
            RaisePropertyChanged(nameof(CbSettingShowDigiPrescriptionsCount));
        }
    }
    
    private bool cbSettingShowDigiCasesIn3ShapeTodayCount = true;
    public bool CbSettingShowDigiCasesIn3ShapeTodayCount
    {
        get => cbSettingShowDigiCasesIn3ShapeTodayCount;
        set
        {
            cbSettingShowDigiCasesIn3ShapeTodayCount = value;
            RaisePropertyChanged(nameof(CbSettingShowDigiCasesIn3ShapeTodayCount));
        }
    }
    
    private bool cbSettingModuleFolderSubscription = false;
    public bool CbSettingModuleFolderSubscription
    {
        get => cbSettingModuleFolderSubscription;
        set
        {
            cbSettingModuleFolderSubscription = value;
            RaisePropertyChanged(nameof(CbSettingModuleFolderSubscription));
        }
    }
    
    private bool cbSettingModuleAccountInfos = false;
    public bool CbSettingModuleAccountInfos
    {
        get => cbSettingModuleAccountInfos;
        set
        {
            cbSettingModuleAccountInfos = value;
            RaisePropertyChanged(nameof(CbSettingModuleAccountInfos));
        }
    }
    
    private bool cbSettingModuleSmartOrderNames = false;
    public bool CbSettingModuleSmartOrderNames
    {
        get => cbSettingModuleSmartOrderNames;
        set
        {
            cbSettingModuleSmartOrderNames = value;
            RaisePropertyChanged(nameof(CbSettingModuleSmartOrderNames));
        }
    }
    
    private bool cbSettingModulePrescriptionMaker = false;
    public bool CbSettingModulePrescriptionMaker
    {
        get => cbSettingModulePrescriptionMaker;
        set
        {
            cbSettingModulePrescriptionMaker = value;
            RaisePropertyChanged(nameof(CbSettingModulePrescriptionMaker));
        }
    }
    
    private bool cbSettingModulePendingDigitals = false;
    public bool CbSettingModulePendingDigitals
    {
        get => cbSettingModulePendingDigitals;
        set
        {
            cbSettingModulePendingDigitals = value;
            RaisePropertyChanged(nameof(CbSettingModulePendingDigitals));
        }
    }
    
    private bool cbSettingShowDigiDetails = true;
    public bool CbSettingShowDigiDetails
    {
        get => cbSettingShowDigiDetails;
        set
        {
            cbSettingShowDigiDetails = value;
            RaisePropertyChanged(nameof(CbSettingShowDigiDetails));
        }
    }
    
    private bool cbSettingWatchFolderPrescriptionMaker = true;
    public bool CbSettingWatchFolderPrescriptionMaker
    {
        get => cbSettingWatchFolderPrescriptionMaker;
        set
        {
            cbSettingWatchFolderPrescriptionMaker = value;
            RaisePropertyChanged(nameof(CbSettingWatchFolderPrescriptionMaker));
        }
    }
    
    private bool cbSettingOpenUpSironaScanFolder = true;
    public bool CbSettingOpenUpSironaScanFolder
    {
        get => cbSettingOpenUpSironaScanFolder;
        set
        {
            cbSettingOpenUpSironaScanFolder = value;
            RaisePropertyChanged(nameof(CbSettingOpenUpSironaScanFolder));
        }
    }
    
    private bool cbSettingExtractIteroZipFiles = true;
    public bool CbSettingExtractIteroZipFiles
    {
        get => cbSettingExtractIteroZipFiles;
        set
        {
            cbSettingExtractIteroZipFiles = value;
            RaisePropertyChanged(nameof(CbSettingExtractIteroZipFiles));
        }
    }
    #endregion Settings Tab Properties

    private string windowBackground = "#56595F";
    public string WindowBackground
    {
        get => windowBackground;
        set
        {
            windowBackground = value;
            RaisePropertyChanged(nameof(WindowBackground));
        }
    }
    
    private List<AccountInfoModel> accountInfoList = [];
    public List<AccountInfoModel> AccountInfoList
    {
        get => accountInfoList;
        set
        {
            accountInfoList = value;
            RaisePropertyChanged(nameof(AccountInfoList));
        }
    }

    private string searchInAccountInfos = "";
    public string SearchInAccountInfos
    {
        get => searchInAccountInfos;
        set
        {
            searchInAccountInfos = value;
            RaisePropertyChanged(nameof(SearchInAccountInfos));
            SearchInAccountInfosMethod();
        }
    }

    private Dictionary<string, string> bgBorderColors = [];
    public Dictionary<string, string> BgBorderColors
    {
        get => bgBorderColors;
        set
        {
            bgBorderColors = value;
            RaisePropertyChanged(nameof(BgBorderColors));
        }
    }
    
    private string selectedAccountInfoCategory = "All";
    public string SelectedAccountInfoCategory
    {
        get => selectedAccountInfoCategory;
        set
        {
            selectedAccountInfoCategory = value;
            RaisePropertyChanged(nameof(SelectedAccountInfoCategory));
            GetAccountInfos();
        }
    }
    
    private List<string> accountInfoCategories = [];
    public List<string> AccountInfoCategories
    {
        get => accountInfoCategories;
        set
        {
            accountInfoCategories = value;
            RaisePropertyChanged(nameof(AccountInfoCategories));
        }
    }

    

    #endregion Properties


    #region RelayCommands

    #region Settings Tab RelayCommands
    public RelayCommand CbSettingGlassyEffectCommand { get; set; }
    public RelayCommand CbSettingShowUnitNumbersCommand { get; set; }
    public RelayCommand CbSettingShowBottomInfoBarCommand { get; set; }
    public RelayCommand CbSettingShowDigiCasesCommand { get; set; }
    public RelayCommand CbSettingShowDigiDetailsCommand { get; set; }
    public RelayCommand CbSettingShowEmptyPanCountCommand { get; set; }
    public RelayCommand CbSettingWatchFolderPrescriptionMakerCommand { get; set; }
    public RelayCommand CbSettingOpenUpSironaScanFolderCommand { get; set; }
    public RelayCommand CbSettingExtractIteroZipFilesCommand { get; set; }
    public RelayCommand CbSettingShowPendingDigiCasesCommand { get; set; }
    public RelayCommand CbSettingIncludePendingDigiCasesInNewlyArrivedCommand { get; set; }
    public RelayCommand CbSettingShowDigiPrescriptionsCountCommand { get; set; }
    public RelayCommand CbSettingShowDigiCasesIn3ShapeTodayCountCommand { get; set; }
    public RelayCommand CbSettingModuleFolderSubscriptionCommand { get; set; }
    public RelayCommand CbSettingModuleAccountInfosCommand { get; set; }
    public RelayCommand CbSettingModuleSmartOrderNamesCommand { get; set; }
    public RelayCommand CbSettingModulePrescriptionMakerCommand { get; set; }
    public RelayCommand CbSettingModulePendingDigitalsCommand { get; set; }

    #endregion Settings Tab RelayCommands


    public RelayCommand StartProgramUpdateCommand { get; set; }
    public RelayCommand FilterMenuItemCommand { get; set; }
    public RelayCommand ExpanderLoadedCommand { get; set; }
    public RelayCommand ExpanderCollapsedCommand { get; set; }
    public RelayCommand ItemClickedCommand { get; set; }
    public RelayCommand ItemRightClickedCommand { get; set; }
    public RelayCommand GetInfoOn3ShapeOrderCommand { get; set; }
    public RelayCommand ExploreOrderSourceCommand { get; set; }
    public RelayCommand ExploreOrderCAMCommand { get; set; }
    public RelayCommand GroupBySelectionChangedCommand { get; set; }
    public RelayCommand SearchFieldClickedCommand { get; set; }
    public RelayCommand SearchFieldKeyDownCommand { get; set; }
    public RelayCommand HideNotificationCommand { get; set; }

    public RelayCommand OpenUpOrderInfoWindowCommand { get; set; }
    public RelayCommand GenerateStCopyCommand { get; set; }
    public RelayCommand OpenUpRenameOrderWindowCommand { get; set; }
    public RelayCommand SelectTargetFolderCommand { get; set; }

    public RelayCommand PmAddNewPanNumberCommand { get; set; }
    public RelayCommand PmSelectTargetFolderCommand { get; set; }
    public RelayCommand PmOpenUpPrescriptionsCommand { get; set; }
    public RelayCommand PmMarkCaseAsRushCommand { get; set; }
    public RelayCommand PmAddToSentToListCommand { get; set; }
    public RelayCommand PmRemoveFromSentToListCommand { get; set; }
    public RelayCommand PmAddSendToLabelCommand { get; set; }
    public RelayCommand TakeAPanNumberCommand { get; set; }
    public RelayCommand PmCancelTakingANumberCommand { get; set; }
    public RelayCommand GrabAPanNumberCommand { get; set; }
    public RelayCommand ClickOnPanNumberCommand { get; set; }
    
   
    

    public RelayCommand PcCheckPanColorCommand { get; set; }
    
    #region Folder Subscription RelayCommands
    public RelayCommand FsCreateTodayFolderCommand { get; set; }
    public RelayCommand FsSearchFoldersCommand { get; set; }
    public RelayCommand ForceUpdatePendingDigiNumberListCommand { get; set; }
    public RelayCommand FsItemClickedCommand { get; set; }
    public RelayCommand FsHideNotificationCommand { get; set; }
    public RelayCommand FsCopyFolderOverCommand { get; set; }
    public RelayCommand FsOpenFolderCommand { get; set; }
    public RelayCommand FsTriggerUpdateRequestCommand { get; set; }
    #endregion Folder Subscription RelayCommands


    public RelayCommand RunNotificationProgressCommand { get; set; }

    public RelayCommand SwitchToPrescriptionMakerTabCommand { get; set; }
    public RelayCommand SwitchToFolderSubscriptionTabCommand { get; set; }
    public RelayCommand SwitchToPendingDigiCasesTabCommand { get; set; }
    public RelayCommand RequestDCASUpdateCommand { get; set; }

    #region AccountInfos RelayCommands
    public RelayCommand OpenWebsiteCommand { get; set; }
    public RelayCommand StartApplicationCommand { get; set; }
    public RelayCommand CopyUserNameToClipboardCommand { get; set; }
    public RelayCommand CopyPasswordToClipboardCommand { get; set; }
    public RelayCommand ShowPasswordCommand { get; set; }
    public RelayCommand ClearAccInfoSearchCommand { get; set; }
    
    #endregion AccountInfos RelayCommands




    #endregion RelayCommands



    private readonly DispatcherTimer GeneralTimer = new();
    private readonly DispatcherTimer listUpdateTimer = new();
    private readonly DispatcherTimer notificationTimer = new();
    private readonly DispatcherTimer fsNotificationTimer = new();
    private readonly DispatcherTimer UpdateCheckTimer = new();

    private static readonly BackgroundWorker bwListCases = new();
    private static readonly BackgroundWorker bwBackgroundTasks = new();
    private static readonly BackgroundWorker bwGetSentOutIssues = new();
    private static readonly BackgroundWorker bwInitialTasks = new();
    private static readonly FileSystemWatcher fswPrescriptionMaker = new();
    private static readonly FileSystemWatcher fswTriosFolderWatcher = new();
    private static readonly FileSystemWatcher fswIteroZipFileWhatcher = new();
    private PdfToImageConverter imageConverter = new();


    public MainViewModel()
    {
        Instance = this;

        GeneralTimer.Tick += GeneralTimer_Tick;
        GeneralTimer.Interval = new TimeSpan(0, 0, 1);
        GeneralTimer.Start();

        listUpdateTimer.Tick += ListUpdateTimer_Tick;
        listUpdateTimer.Interval = new TimeSpan(0, 0, 30);
        listUpdateTimer.Start();

        notificationTimer.Tick += NotificationTimer_Tick;
        notificationTimer.Interval = new TimeSpan(0, 0, 10);
        
        fsNotificationTimer.Tick += FsNotificationTimer_Tick;
        fsNotificationTimer.Interval = new TimeSpan(0, 0, 30);

        UpdateCheckTimer.Tick += UpdateCheckTimer_Tick;
        UpdateCheckTimer.Interval = new TimeSpan(0, 0, 30);
        UpdateCheckTimer.Start();

        StartProgramUpdateCommand = new RelayCommand(o => StartProgramUpdate());
        FilterMenuItemCommand = new RelayCommand(o => FilterMenuItemClicked(o));
        ExpanderLoadedCommand = new RelayCommand(o => ExpanderLoaded(o));
        ExpanderCollapsedCommand = new RelayCommand(o => ExpanderCollapsed(o));
        ItemClickedCommand = new RelayCommand(o => ItemClicked(o));
        ItemRightClickedCommand = new RelayCommand(o => ItemRightClicked(o));
        GetInfoOn3ShapeOrderCommand = new RelayCommand(o => GetInfoOn3ShapeOrder(o));
        ExploreOrderSourceCommand = new RelayCommand(o => ExploreOrderSource(o));
        ExploreOrderCAMCommand = new RelayCommand(o => ExploreOrderCAM(o));
        GroupBySelectionChangedCommand = new RelayCommand(o => GroupList());
        SearchFieldClickedCommand = new RelayCommand(o => _MainWindow.tbSearch.Focus());
        SearchFieldKeyDownCommand = new RelayCommand(o => SearchFieldKeyDown());
        HideNotificationCommand = new RelayCommand(o => HideNotification());

        OpenUpOrderInfoWindowCommand = new RelayCommand(o => OpenUpOrderInfoWindow());
        GenerateStCopyCommand = new RelayCommand(o => GenerateStCopy());
        OpenUpRenameOrderWindowCommand = new RelayCommand(o => OpenUpRenameOrderWindow());

        PmAddNewPanNumberCommand = new RelayCommand(o => PmAddNewPanNumber(o));
        PmSelectTargetFolderCommand = new RelayCommand(o => PmSelectTargetFolder(o));
        PmOpenUpPrescriptionsCommand = new RelayCommand(o => PmOpenUpPrescriptions());
        PmMarkCaseAsRushCommand = new RelayCommand(o => PmMarkCaseAsRush());
        PmAddToSentToListCommand = new RelayCommand(o => PmAddToSentToList());
        PmRemoveFromSentToListCommand = new RelayCommand(o => PmRemoveFromSentToList());
        PmAddSendToLabelCommand = new RelayCommand(o => PmMarkCaseWithLabelSendTo());
        TakeAPanNumberCommand = new RelayCommand(o => TakeAPanNumber());
        ClickOnPanNumberCommand = new RelayCommand(o => ClickOnPanNumber(o));

        OpenWebsiteCommand = new RelayCommand(o => OpenWebsite(o));
        StartApplicationCommand = new RelayCommand(o => StartApplication(o));
        CopyUserNameToClipboardCommand = new RelayCommand(o => CopyUserNameToClipboard(o));
        CopyPasswordToClipboardCommand = new RelayCommand(o => CopyPasswordToClipboard(o));
        ShowPasswordCommand = new RelayCommand(o => ShowPassword(o));
        ClearAccInfoSearchCommand = new RelayCommand(o => ClearAccInfoSearch());
        
        PmCancelTakingANumberCommand = new RelayCommand(o => { ShowingTakeANumberPanel = Visibility.Hidden; });
        GrabAPanNumberCommand = new RelayCommand(o => { 
            ShowingTakeANumberPanel = Visibility.Visible;
            PmSavedPrescription = null;
        });


        #region Folder Subscription RelayCommands
        SelectTargetFolderCommand = new RelayCommand(o => SelectTargetFolder());
        FsCreateTodayFolderCommand = new RelayCommand(o => FsCreateTodayFolder());
        FsSearchFoldersCommand = new RelayCommand(o => FsSearchFolders());
        FsItemClickedCommand = new RelayCommand(o => FsItemClicked(o));
        ForceUpdatePendingDigiNumberListCommand = new RelayCommand(o => FillUpPendingDigiCaseNumberList(true));
        FsHideNotificationCommand = new RelayCommand(o => FsHideNotification());
        FsCopyFolderOverCommand = new RelayCommand(o => FsCopyFolderOver(o));
        FsOpenFolderCommand = new RelayCommand(o => FsOpenFolder(o));
        FsTriggerUpdateRequestCommand = new RelayCommand(o => FsTriggerUpdateRequest());
        #endregion Folder Subscription RelayCommands

        CbSettingGlassyEffectCommand = new RelayCommand(o => CbSettingGlassyEffectMethod());
        CbSettingShowUnitNumbersCommand = new RelayCommand(o => CbSettingShowUnitNumbersMethod());
        CbSettingShowBottomInfoBarCommand = new RelayCommand(o => CbSettingShowBottomInfoBarMethod());
        CbSettingShowDigiCasesCommand = new RelayCommand(o => CbSettingShowDigiCasesMethod());
        CbSettingShowDigiDetailsCommand = new RelayCommand(o => CbSettingShowDigiDetailsMethod());
        CbSettingWatchFolderPrescriptionMakerCommand = new RelayCommand(o => CbSettingWatchFolderPrescriptionMakerMethod());
        CbSettingOpenUpSironaScanFolderCommand = new RelayCommand(o => CbSettingOpenUpSironaScanFolderMethod());
        CbSettingExtractIteroZipFilesCommand = new RelayCommand(o => CbSettingExtractIteroZipFilesMethod());
        CbSettingShowPendingDigiCasesCommand = new RelayCommand(o => CbSettingShowPendingDigiCasesMethod());
        CbSettingIncludePendingDigiCasesInNewlyArrivedCommand = new RelayCommand(o => CbSettingIncludePendingDigiCasesInNewlyArrivedMethod());
        CbSettingShowEmptyPanCountCommand = new RelayCommand(o => CbSettingShowEmptyPanCountMethod());
        CbSettingShowDigiPrescriptionsCountCommand = new RelayCommand(o => CbSettingShowDigiPrescriptionsCountMethod());
        CbSettingShowDigiCasesIn3ShapeTodayCountCommand = new RelayCommand(o => CbSettingShowDigiCasesIn3ShapeTodayCountMethod());
        CbSettingModuleFolderSubscriptionCommand = new RelayCommand(o => CbSettingModuleFolderSubscriptionMethod());
        CbSettingModuleAccountInfosCommand = new RelayCommand(o => CbSettingModuleAccountInfosMethod());
        CbSettingModuleSmartOrderNamesCommand = new RelayCommand(o => CbSettingModuleSmartOrderNamesMethod());
        CbSettingModulePrescriptionMakerCommand = new RelayCommand(o => CbSettingModulePrescriptionMakerMethod());
        CbSettingModulePendingDigitalsCommand = new RelayCommand(o => CbSettingModulePendingDigitalsMethod());


        RunNotificationProgressCommand = new RelayCommand(o => BlinkWindow());
        SwitchToPrescriptionMakerTabCommand = new RelayCommand(o => SwitchToPrescriptionMakerTab());
        SwitchToFolderSubscriptionTabCommand = new RelayCommand(o => SwitchToFolderSubscriptionTab());
        SwitchToPendingDigiCasesTabCommand = new RelayCommand(o => SwitchToPendingDigiCasesTab());

        RequestDCASUpdateCommand = new RelayCommand(o => RequestDCASUpdate());


        PcCheckPanColorCommand = new RelayCommand(o => PcCheckPanColor());


        bwInitialTasks.DoWork += InitialTasksAtApplicationStartup_DoWork;
        bwInitialTasks.RunWorkerCompleted += InitialTasksAtApplicationStartup_RunWorkerCompleted;

        bwListCases.DoWork += ListCases_DoWork;
        bwListCases.RunWorkerCompleted += ListCases_RunWorkerCompleted;
        bwListCases.WorkerSupportsCancellation = true;

        bwBackgroundTasks.DoWork += BwBackgroundTasks_DoWork;
        bwBackgroundTasks.RunWorkerCompleted += BwBackgroundTasks_RunWorkerCompleted;

        bwGetSentOutIssues.DoWork += BwGetSentOutIssues_DoWork;
        bwGetSentOutIssues.RunWorkerCompleted += BwGetSentOutIssues_RunWorkerCompleted;
        bwGetSentOutIssues.WorkerSupportsCancellation = true;


        DataView = CollectionViewSource.GetDefaultView(Current3ShapeOrderList);

        // for accountinfo bordercolors by category
        bgBorderColors.TryAdd("#466f69", "");
        bgBorderColors.TryAdd("#78804f", "");
        bgBorderColors.TryAdd("#7d5f48", "");
        bgBorderColors.TryAdd("#7a504c", "");
        bgBorderColors.TryAdd("#485c7a", "");
        bgBorderColors.TryAdd("#46596F", "");
        bgBorderColors.TryAdd("#7a4076", "");
        bgBorderColors.TryAdd("#67467d", "");
        bgBorderColors.TryAdd("#7a464f", "");
    }

    




    #region AccountInfos
    private void SearchInAccountInfosMethod()
    {
        List<AccountInfoModel> accountInfoModels = AccountInfoList;
        if (!string.IsNullOrEmpty(SearchInAccountInfos))
            AccountInfoList = accountInfoModels.Where(x => x.FriendlyName!.Contains(SearchInAccountInfos, StringComparison.CurrentCultureIgnoreCase) ||
                                                           x.SubCategory!.Contains(SearchInAccountInfos, StringComparison.CurrentCultureIgnoreCase)).ToList();
        else
            GetAccountInfos();
    }
    
    private void ClearAccInfoSearch()
    {
        SearchInAccountInfos = "";
        _MainWindow.tbSearchAccInfos.Focus();
    }


    private async void GetAccountInfos()
    {
        AccountInfoCategories = await GetAccountInfoCategories();

        List<AccountInfoModel> list = await GetAccountInfoList(BgBorderColors);

        if (string.IsNullOrEmpty(SelectedAccountInfoCategory) || SelectedAccountInfoCategory == "All")
            AccountInfoList = list;
        else
            AccountInfoList = list.Where(x => x.Category == SelectedAccountInfoCategory).ToList();
    }
    
    private void OpenWebsite(object obj)
    {
        string url = (string)obj;
        Debug.WriteLine(url);
        try
        {
            Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
        }
        catch (Exception)
        {
        }
    }
    
    private void StartApplication(object obj)
    {
        string appPath = (string)obj;
        Debug.WriteLine(appPath);
        try
        {
            Process.Start(new ProcessStartInfo { FileName = appPath, UseShellExecute = true });
        }
        catch (Exception)
        {
        }
    }

    private void CopyUserNameToClipboard(object obj)
    {
        string userName = (string)obj;
        
        Clipboard.SetText(userName);

        var item = new System.Windows.Forms.NotifyIcon()
        {
            Visible = true,
            Icon = System.Drawing.SystemIcons.Information
        };
        item.ShowBalloonTip(40000, "", $"Username was copied to clipboard!", System.Windows.Forms.ToolTipIcon.Info);
        
    }
    
    private void CopyPasswordToClipboard(object obj)
    {
        string password = (string)obj;
        
        Clipboard.SetText(password);

        var item = new System.Windows.Forms.NotifyIcon()
        {
            Visible = true,
            Icon = System.Drawing.SystemIcons.Information
        };
        item.ShowBalloonTip(40000, "", "Password was copied to clipboard!", System.Windows.Forms.ToolTipIcon.Info);
        
    }
    
    private void ShowPassword(object obj)
    {
        Border border = (Border)obj;
        Debug.WriteLine(border.Tag.ToString());
    }
    #endregion AccountInfos



    private void RequestDCASUpdate()
    {
        if (LastDCASUpdate.Contains("minute"))
        {
            WriteStatsSetting("dcas_CheckForEmails", "true");
            ShowNotificationMessage("Success", "Request for DCAS update sent!", NotificationIcon.Success, false, true);
        }
    }
    
    private void SwitchToPrescriptionMakerTab()
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            _MainWindow.applicationsTabControl.SelectedItem = _MainWindow.prescriptionMakerTab;
            _MainWindow.mainTabControl.SelectedItem = _MainWindow.applicationsTab;
        });
    }
    
    private void SwitchToFolderSubscriptionTab()
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            _MainWindow.applicationsTabControl.SelectedItem = _MainWindow.folderSubscriptionTab;
            _MainWindow.mainTabControl.SelectedItem = _MainWindow.applicationsTab;
        });
    }
    
    private void SwitchToPendingDigiCasesTab()
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            _MainWindow.applicationsTabControl.SelectedItem = _MainWindow.pendingDigiCasesTab;
            _MainWindow.mainTabControl.SelectedItem = _MainWindow.applicationsTab;
        });
    }

    private async Task BlinkWindow(string color = "yellow")
    {
        string FinalColor = "#56595F";

        if (color == "yellow")
        {
            WindowBackground = "#56595F";
            await Task.Delay(50);
            WindowBackground = "#66595F";
            await Task.Delay(50);
            WindowBackground = "#76795F";
            await Task.Delay(50);
            WindowBackground = "#86895F";
            await Task.Delay(50);
            WindowBackground = "#96995F";
            await Task.Delay(50);
            WindowBackground = "#A6A95F";
            await Task.Delay(50);
            WindowBackground = "#96995F";
            await Task.Delay(50);
            WindowBackground = "#86895F";
            await Task.Delay(50);
            WindowBackground = "#76795F";
            await Task.Delay(50);
            WindowBackground = "#66695F";
            await Task.Delay(50);
        }
        
        if (color == "green")
        {
            WindowBackground = "#56595F";
            await Task.Delay(50);
            WindowBackground = "#56695F";
            await Task.Delay(50);
            WindowBackground = "#56795F";
            await Task.Delay(50);
            WindowBackground = "#56895F";
            await Task.Delay(50);
            WindowBackground = "#56995F";
            await Task.Delay(50);
            WindowBackground = "#56A95F";
            await Task.Delay(50);
            WindowBackground = "#56995F";
            await Task.Delay(50);
            WindowBackground = "#56895F";
            await Task.Delay(50);
            WindowBackground = "#56795F";
            await Task.Delay(50);
            WindowBackground = "#56695F";
            await Task.Delay(50);
        }

        WindowBackground = FinalColor;
    }


    private async void PcCheckPanColor()
    {
        string panNumber = PcPanNumber;
        if (panNumber.Length < 1)
            return;

        if (!int.TryParse(panNumber, out int num))
        {
            PcPanNumber = "";
            return;
        }

        string rgbColor = GetPanColorByNumber(num);
        if (rgbColor == "0-0-0")
        {
            PcPanColor = "Black";
            PcPanColorFriendlyName = "Number not found!";
            PcPanNumber = "";
            
            await Task.Delay(200);
            PcPanColor = "White";
            await Task.Delay(200);
            PcPanColor = "Black";
            await Task.Delay(200);
            PcPanColor = "White";
            await Task.Delay(200);
            PcPanColor = "Black";
            await Task.Delay(200);
            PcPanColor = "White";
            await Task.Delay(200);
            
            PcPanColor = "#06090F";
            await Task.Delay(100);
            
            PcPanColor = "#16191F";
            await Task.Delay(100);
            
            PcPanColor = "#26292F";
            await Task.Delay(100);
            
            PcPanColor = "#36393F";
            await Task.Delay(100);
            
            PcPanColor = "#46494F";
            await Task.Delay(100);
            
            PcPanColor = "#56554F";
            



            await Task.Delay(500);
            PcPanColor = "#56595F";
            PcPanColorFriendlyName = "Check pan color";
        }
        else
        {
            string[] panColorParts = rgbColor.Split('-');

            _ = int.TryParse(panColorParts[0], out int colorR);
            _ = int.TryParse(panColorParts[1], out int colorG);
            _ = int.TryParse(panColorParts[2], out int colorB);

            Brush panColor = new SolidColorBrush(Color.FromArgb(255, (byte)colorR, (byte)colorG, (byte)colorB));
            PcPanColor = panColor.ToString();
            PcPanColorFriendlyName = GetPanColorNameByNumber(num);
            PcPanNumber = "";

            await Task.Delay(3500);
            PcPanColor = "#56595F";
            PcPanColorFriendlyName = "Check pan color";
        }
    }








    #region FOLDER SUBSCRIPTION & PENDING DIGI CASES METHODS

    private void FsTriggerUpdateRequest()
    {
        WriteStatsSetting("fs_RescanNow", "true");
    }

    private static void FsOpenFolder(object obj)
    {
        try
        {
            string folder = (string)obj!;
            Process.Start("explorer.exe", "\"" + folder + "\"");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[{ex.LineNumber()}] {ex.Message}");
        }
    }

    private async void FsCopyFolderOver(object obj)
    {
        string number = obj.ToString()!;
        FsHideNotification();

        if (await Task.Run(() => CopyDirectory(FsSelectedFolderObject.Path!, $@"{FsubscrTargetFolder}{number}-{FsSelectedFolderObject.FolderName}")))
        {
            ShowNotificationMessage("Success", $"Folder with name: '{number}-{FsSelectedFolderObject.FolderName}' copied over successfully", NotificationIcon.Success, true);
            FsSearchString = "";
            await Task.Run(() => MarkPanNumberAsCollected(number));
            FillUpPendingDigiCaseNumberList(true);
        }
        else
            ShowNotificationMessage("Error", "Error occured during the copy process!", NotificationIcon.Error, true);
    }

    
    private void FsSearchFolders()
    {
        if (FsSearchString.Length < 1)
            return;

        FolderSubscriptionList = GetFolderSubscriptions(FsSearchString);
        FolderSubscriptionList.Sort((x, y) => x.AgeForSorting!.CompareTo(y.AgeForSorting));
        FsSearchString = "";
    }

    private void FsCreateTodayFolder()
    {
        try
        {
            _ = int.TryParse(FsubscrTargetFolder.AsSpan(FsubscrTargetFolder.Length - 3, 2), out int FolderCheck);
            string parentDir = "";

            if (FolderCheck > 0)
                parentDir = Directory.GetParent(Directory.GetParent(FsubscrTargetFolder)!.ToString())!.ToString() + "\\";
            else
                parentDir = FsubscrTargetFolder;

            string TDDir = parentDir + DateTime.Now.ToString("MM-dd");
            Directory.CreateDirectory(TDDir);
            Directory.CreateDirectory(@$"{TDDir}\DN");
            FsubscrTargetFolder = TDDir + "\\";
            WriteLocalSetting("SubscriptionCopyFolder", TDDir + "\\");
        }
        catch (Exception ex)
        {
                Debug.WriteLine($"[{ex.LineNumber()}] {ex.Message}");
                MessageBox.Show(ex.Message);
        }

        try
        {
            Process.Start("explorer.exe", "\"" + FsubscrTargetFolder + "\"");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[{ex.LineNumber()}] {ex.Message}");
        }
    }

    private void SelectTargetFolder()
    {
        var folderDialog = new OpenFolderDialog
        {
            Title = "Select the target folder where you want the app to copy over scan files"
        };

        if (folderDialog.ShowDialog() == true)
        {
            var folderName = folderDialog.FolderName;
            FsubscrTargetFolder = folderName + @"\";
            WriteLocalSetting("SubscriptionCopyFolder", folderName + @"\");
        }
    }

    private void FsHideNotification()
    {
        FsNotificationTimer_Tick(null, null);
    }

    private void FsNotificationTimer_Tick(object? sender, EventArgs e)
    {
        FsCustomNumber = "";
        FsCopyPanelShows = false;
        fsNotificationTimer.Stop();
    }

    private async void FsItemClicked(object obj)
    {
        try
        {
            if (await Task.Run(() => Directory.Exists(((FolderSubscriptionModel)obj).Path)))
            {
                FsSelectedFolderObject = (FolderSubscriptionModel)obj;
                FsCopyPanelShows = true;
            }
            else
            {
                ShowNotificationMessage("Folder not found", "The folder you're selected does not exist anymore", NotificationIcon.Error, true);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[{ex.LineNumber()}] {ex.Message}");
            ShowNotificationMessage("No access", "The folder you're selected is not accessible!", NotificationIcon.Error, true);
        }
    }

    #endregion FOLDER SUBSCRIPTION & PENDING DIGI CASES METHODS


    #region PRESCRIPTION MAKER METHODS

    private void PmAddNewPanNumber(object obj)
    {
        string number = obj.ToString()!;
        _ = int.TryParse(number, out int num);

        if (GetPanColorByNumber(num) != "")
        {
            if (!PmPanNumberList.Contains(number))
            {
                AddNewPanNumber(number);
                FillUpEmptyPanNumberPanel(false, number);

                RaisePropertyChanged(nameof(PmPanNumberList));
            }
            else
            {
                ShowNotificationMessage("Already Exists", $"{number} is already present in the list above!", NotificationIcon.Info);
            }
            PmAddNewNumber = "";
        }
        else
        {
            ShowNotificationMessage("Not registered", $"{number} is not registered whitin the system. Please enter a valid pan number!", NotificationIcon.Warning);
            PmAddNewNumber = "";
        }
    }

    private void TakeAPanNumber()
    {
        if (PmPanNumberList.Count > 0 && PmNextPanNumberInList is not null)
        {
            foreach (var item in _MainWindow.pmPanelPanek.Children.OfType<Button>().ToList())
            {
                if (item.Tag == PmNextPanNumberInList)
                    _MainWindow.pmPanelPanek.Children.Remove(item);
            }

            PmLastTakenPanNumber = PmNextPanNumberInList;
            PmPanNumberList.Remove(PmNextPanNumberInList);
            RaisePropertyChanged(nameof(PmPanNumberList));
            RemovePanNumberFromAvailablePans(PmNextPanNumberInList);
            FillUpEmptyPanNumberPanel();
            ShowingTakeANumberPanel = Visibility.Hidden;

            if (PmPanNumberList.Count == 0)
                PmNextPanNumberInList = "";
        }
    }

    private void ClickOnPanNumber(object obj)
    {
        string panNumberStr = (string)obj;

        MessageBoxResult dlg = MessageBox.Show(_MainWindow, $"Are you sure you want to delete this pan: {panNumberStr}?", "Delete pan", MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (dlg == MessageBoxResult.No)
            return;

        foreach (var item in _MainWindow.pmPanelPanek.Children.OfType<Button>().ToList())
        {
            if (item.Tag == panNumberStr)
                _MainWindow.pmPanelPanek.Children.Remove(item);
        }

        PmPanNumberList.Remove(panNumberStr);
        RaisePropertyChanged(nameof(PmPanNumberList));
        RemovePanNumberFromAvailablePans(panNumberStr);
        FillUpEmptyPanNumberPanel();
        ShowingTakeANumberPanel = Visibility.Hidden;

        if (PmPanNumberList.Count == 0)
            PmNextPanNumberInList = null;
    }

    private void FillUpEmptyPanNumberPanel(bool clearBeforeAdd = true, string panNumbr = "")
    {

        if (clearBeforeAdd)
        {
            PmPanNumberList = GetPanNumbers();
            MainWindow.Instance.pmPanelPanek.Children.Clear();

            foreach (string number in PmPanNumberList)
            {
                try
                {
                    _ = int.TryParse(number, out int num);
                    string[] panColorParts = GetPanColorByNumber(num).Split('-');
                    _ = int.TryParse(panColorParts[0], out int colorR);
                    _ = int.TryParse(panColorParts[1], out int colorG);
                    _ = int.TryParse(panColorParts[2], out int colorB);

                    Brush panColor = new SolidColorBrush(Color.FromArgb(255, (byte)colorR, (byte)colorG, (byte)colorB));

                    Button btn = new()
                    {
                        Tag = number,
                        Margin = new Thickness(1),
                        Background = Brushes.Transparent,
                        Padding = new Thickness(0),
                        BorderThickness = new Thickness(0),
                        Width = 76,
                        Height = 42,
                        Command = ClickOnPanNumberCommand,
                        CommandParameter = number,
                        Style = Application.Current.Resources["panBoxStyle"] as Style
                    };

                    Border panBack = new()
                    {
                        CornerRadius = new CornerRadius(2),
                        Margin = new Thickness(3),
                        Background = panColor,
                        Width = 70,
                        Height = 36,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top,

                    };

                    Border stickerBorder = new()
                    {
                        BorderBrush = Brushes.Silver,
                        BorderThickness = new Thickness(1),
                    };

                    Grid panSticker = new()
                    {
                        Background = Brushes.White,
                        Margin = new Thickness(8, 6, 8, 6),
                    };

                    TextBlock panNumber = new()
                    {
                        Text = number,
                        FontSize = 16,
                        FontWeight = FontWeights.SemiBold,
                        Foreground = Brushes.Black,
                        Cursor = Cursors.Hand,
                        HorizontalAlignment = HorizontalAlignment.Center,
                    };


                    panSticker.Children.Add(panNumber);
                    stickerBorder.Child = panSticker;
                    panBack.Child = stickerBorder;
                    btn.Content = panBack;

                    MainWindow.Instance.pmPanelPanek.Children.Add(btn);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[{ex.LineNumber()}] {ex.Message}");
                }
            }
        }
        else if (panNumbr.Length > 0)
        {
            try
            {
                _ = int.TryParse(panNumbr, out int num);
                string[] panColorParts = GetPanColorByNumber(num).Split('-');
                _ = int.TryParse(panColorParts[0], out int colorR);
                _ = int.TryParse(panColorParts[1], out int colorG);
                _ = int.TryParse(panColorParts[2], out int colorB);

                Brush panColor = new SolidColorBrush(Color.FromArgb(255, (byte)colorR, (byte)colorG, (byte)colorB));

                Button btn = new()
                {
                    Tag = panNumbr,
                    Margin = new Thickness(0),
                    Background = Brushes.Transparent,
                    Padding = new Thickness(0),
                    BorderThickness = new Thickness(0),
                    Width = 76,
                    Height = 42,
                    ClipToBounds = true,
                    Command = ClickOnPanNumberCommand,
                    CommandParameter = panNumbr,
                    Style = Application.Current.Resources["panBoxStyle"] as Style
                };

                Border panBack = new()
                {
                    CornerRadius = new CornerRadius(2),
                    Margin = new Thickness(3),
                    Background = panColor,
                    Width = 70,
                    Height = 36,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top
                };

                Border stickerBorder = new()
                {
                    BorderBrush = Brushes.Silver,
                    BorderThickness = new Thickness(1),
                };

                Grid panSticker = new()
                {
                    Background = Brushes.White,
                    Margin = new Thickness(8, 6, 8, 6),
                };

                TextBlock panNumber = new()
                {
                    Text = panNumbr,
                    FontSize = 16,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = Brushes.Black,
                    HorizontalAlignment = HorizontalAlignment.Center,
                };

                panSticker.Children.Add(panNumber);
                stickerBorder.Child = panSticker;
                panBack.Child = stickerBorder;
                btn.Content = panBack;

                MainWindow.Instance.pmPanelPanek.Children.Add(btn);

                PmPanNumberList.Add(panNumbr);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[{ex.LineNumber()}] {ex.Message}");
            }
        }

        if (PmPanNumberList.Count > 0)
            PmNextPanNumberInList = PmPanNumberList[0];
    }

    

    private void PmRemoveFromSentToList()
    {
        if (PmSelectedSendToEntry is not null)
            RemoveNameFromSentToList(PmSelectedSendToEntry);

        PmSendToList = GetAllSendToEnties();
    }

    private void PmAddToSentToList()
    {
        AddNameToSentToList(PmNewSentToName);
        PmNewSentToName = "";
        PmSendToList = GetAllSendToEnties();
    }

    

    private async void FswPrescriptionMaker_Created(object sender, FileSystemEventArgs e)
    {
        if (CbSettingWatchFolderPrescriptionMaker)
        {
            if (PmFinalPrescriptionsFolder != "" && !PmFinalPrescriptionsFolder.StartsWith("Click"))
            {
                Directory.CreateDirectory(PmFinalPrescriptionsFolder + DateTime.Now.ToString("MM-dd"));
                CleanTempFolder();

                try
                {
                    if (PmPanNumberList.Count > 0)
                    {
                        int i = 0;
                        FileInfo file = new(e.FullPath);
                        while (IsFileLocked(file) || i > 10)
                        {
                            await Task.Delay(1000);
                            i++;
                        }
                        PmLastTakenPanNumber = "";
                    }
                    else
                    {
                        MessageBox.Show("No more available numbers!", "No more numbers", MessageBoxButton.OK, MessageBoxImage.Error);
                        SironaOrderNumber = "";
                        IsItSironaPrescription = false;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[{ex.LineNumber()}] {ex.Message}");
                    //MessageBox.Show("No more available numbers!", "No more numbers", MessageBoxButton.OK, MessageBoxImage.Error);
                    //MessageBox.Show(ex.Message, "Error #PM-1", MessageBoxButton.OK, MessageBoxImage.Error);
                    SironaOrderNumber = "";
                    IsItSironaPrescription = false;
                    return;
                }
            }

        }
    }
    
    private async void FswPrescriptionMaker_Changed(object sender, FileSystemEventArgs e)
    {
        if (CbSettingWatchFolderPrescriptionMaker)
        {
            if (PmFinalPrescriptionsFolder != "" && !PmFinalPrescriptionsFolder.StartsWith("Click"))
            {
                Directory.CreateDirectory(PmFinalPrescriptionsFolder + DateTime.Now.ToString("MM-dd"));
                CleanTempFolder();

                try
                {
                    if (PmPanNumberList.Count > 0)
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() => {
                            ProcessingDigiPrescriptionNow = Visibility.Visible;
                            PmSavedPrescription = null;
                            NextPanNumberGlobal = PmPanNumberList[0].ToString();
                            FullPathGlobal = e.FullPath;
                            DocumentStreamPixelCheck = new FileStream(e.FullPath, FileMode.OpenOrCreate);

                            PmRushButtonShows = Visibility.Visible;
                            PmSendToButtonShows = Visibility.Visible;

                            //Load the PDF document as a stream
                            using FileStream inputStream = DocumentStreamPixelCheck;
                            imageConverter.Load(inputStream);
                            //Convert PDF to Image.
                            using (Stream outputStreamForPixelCheck = imageConverter.Convert(0, false, false))
                            {
                                PrescriptionImageForProcess = new(outputStreamForPixelCheck);
                            }
                            Stream[] outputStream = imageConverter.Convert(0, imageConverter.PageCount - 1, 200, 200, false, false);

                        }));

                        if (e.Name!.Contains("Workticket_"))
                            IsItSironaPrescription = true;

                        SironaOrderNumber = "";

                        StartProcessingPrescription();
                    }
                    else
                    {
                        MessageBox.Show("No more available numbers!", "No more numbers", MessageBoxButton.OK, MessageBoxImage.Error);
                        SironaOrderNumber = "";
                        IsItSironaPrescription = false;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[{ex.LineNumber()}] {ex.Message}");
                    //MessageBox.Show("No more available numbers!", "No more numbers", MessageBoxButton.OK, MessageBoxImage.Error);
                    //MessageBox.Show(ex.Message, "Error #PM-1", MessageBoxButton.OK, MessageBoxImage.Error);
                    SironaOrderNumber = "";
                    IsItSironaPrescription = false;
                    return;
                }
            }

        }
    }

    private bool IsFileLocked(FileInfo file)
    {
        FileStream stream = null;

        try
        {
            stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        }
        catch (IOException io)
        {
            Debug.WriteLine($"[{io.LineNumber()}] {io.Message}");
            //the file is unavailable because it is:
            //still being written to
            //or being processed by another thread
            //or does not exist (has already been processed)
            return true;
        }
        finally
        {
            stream?.Close();
        }

        //file is not locked
        return false;
    }

    private async void StartProcessingPrescription()
    {
        await Task.Run(() => IsPixelisWhite(PrescriptionImageForProcess)); 
    }

    private async Task<bool> IsPixelisWhite(Bitmap img)
    {
        Debug.WriteLine("########################");
        Debug.WriteLine("W" + img.Width);
        Debug.WriteLine("H" + img.Height);
        Debug.WriteLine("########################");
        

        string PixelColor = img.GetPixel(10, 56).ToString()
                                                    .Replace("Color [", "")
                                                    .Replace("]", "")
                                                    .Replace("A=", "")
                                                    .Replace(" R=", "")
                                                    .Replace(" G=", "")
                                                    .Replace(" B=", "")
                                                    .Replace(",", "");
        Debug.WriteLine("PixelColor:"+PixelColor);

        string PixelIS3DColor = img.GetPixel(2, 30).ToString()
                                                    .Replace("Color [", "")
                                                    .Replace("]", "")
                                                    .Replace("A=", "")
                                                    .Replace(" R=", "")
                                                    .Replace(" G=", "")
                                                    .Replace(" B=", "")
                                                    .Replace(",", "");

        Debug.WriteLine("PixelIS3DColor:" + PixelIS3DColor);
        
        string PixelDSCoreColor = img.GetPixel(12, 12).ToString()
                                                    .Replace("Color [", "")
                                                    .Replace("]", "")
                                                    .Replace("A=", "")
                                                    .Replace(" R=", "")
                                                    .Replace(" G=", "")
                                                    .Replace(" B=", "")
                                                    .Replace(",", "");

        Debug.WriteLine("PixelDSCoreColor:" + PixelDSCoreColor);

        string PixelSironaColor = img.GetPixel(750, 40).ToString()
                                                    .Replace("Color [", "")
                                                    .Replace("]", "")
                                                    .Replace("A=", "")
                                                    .Replace(" R=", "")
                                                    .Replace(" G=", "")
                                                    .Replace(" B=", "")
                                                    .Replace(",", "");
        Debug.WriteLine("PixelSironaColor:" + PixelSironaColor);

        string PixelMeditColor = img.GetPixel(686, 32).ToString()
                                                    .Replace("Color [", "")
                                                    .Replace("]", "")
                                                    .Replace("A=", "")
                                                    .Replace(" R=", "")
                                                    .Replace(" G=", "")
                                                    .Replace(" B=", "")
                                                    .Replace(",", "");
        Debug.WriteLine("PixelMeditColor:" + PixelMeditColor);


        


        if (PixelMeditColor == "255107154240") // Medit
        {
            PageHeaderIsHigh = "5";
            await Task.Run(() => EditPDF(FullPathGlobal, NextPanNumberGlobal));
            return false;
        }
        else if (PixelSironaColor == "2552441671") // Sirona
        {
            PageHeaderIsHigh = "4";
            await Task.Run(() => EditPDF(FullPathGlobal, NextPanNumberGlobal));
            return false;
        }
        else if (PixelIS3DColor == "25585135255") // IS3D
        {
            PageHeaderIsHigh = "3";
            await Task.Run(() => EditPDF(FullPathGlobal, NextPanNumberGlobal));
            return false;
        }
        else if (PixelDSCoreColor == "255858789") // DSCore
        {
            PageHeaderIsHigh = "6";
            await Task.Run(() => EditPDF(FullPathGlobal, NextPanNumberGlobal));
            return false;
        }
        else
        {
            if (PixelColor == "255255255255") // iTero
            {
                PageHeaderIsHigh = "0";
                await Task.Run(() => EditPDF(FullPathGlobal, NextPanNumberGlobal));
                return true;
            }
            else
            {
                PageHeaderIsHigh = "1";
                await Task.Run(() => EditPDF(FullPathGlobal, NextPanNumberGlobal));
                return false;
            }
        }
    }


    private async void TriggerSironaFolderRename(string PanNumber, string FinalLocation)
    {
        string image = FinalLocation + "\\" + DateTime.Now.ToString("MM-dd") + "\\" + PanNumber + ".png";

        try
        {
            if (!File.Exists(DataBaseFolder + "eng.traineddata"))
                await WriteResourceToFile("eng.traineddata", DataBaseFolder + "eng.traineddata");

            using var engine = new Engine(DataBaseFolder, Language.English, EngineMode.Default);
            using var img = TesseractOCR.Pix.Image.LoadFromFile(image);
            using var page = engine.Process(img);
            string text = page.Text;
            if (text.Contains("Connect Case Center", StringComparison.CurrentCultureIgnoreCase))
            {
                IsItSironaPrescription = true;
                SironaOrderNumber = text.Substring(text.IndexOf("Order Number:"), 23).Replace("Order Number:", "").Trim();
            }
            Debug.WriteLine(SironaOrderNumber);
                
            if (text.Contains("CORE", StringComparison.CurrentCultureIgnoreCase) && text.Contains("Case Sheet", StringComparison.CurrentCultureIgnoreCase))
            {
                string DSCoreOrderNumber = text.Substring(text.IndexOf("Order Number:"), 48)
                                               .Replace("Order Number:", "")
                                               .Replace("Order Date:", "")
                                               .Replace("Date Due:", "")
                                               .Trim();

                if (DSCoreOrderNumber.Contains(' '))
                    DSCoreOrderNumber = DSCoreOrderNumber[..DSCoreOrderNumber.IndexOf(' ')];

                
                string PatientNameRaw = "";
                string PatientName = "";

                foreach (var str in text.Substring(text.IndexOf("Patient"), 100).Replace("Patient", "").Trim().Split("\n", StringSplitOptions.RemoveEmptyEntries))
                {
                    if (PatientNameRaw == "")
                        PatientNameRaw = str;
                }

                string[] patientNameParts = PatientNameRaw.Split(" ");

                PatientName = patientNameParts[^2] + " " + patientNameParts[^1];

                SendSironaInfoToServer(PanNumber, PatientName, DSCoreOrderNumber, "DSCORE");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[{ex.LineNumber()}] {ex.Message}");
        }

        if (IsItSironaPrescription && !string.IsNullOrEmpty(SironaOrderNumber))
        {
            string sironaFolder = ReadLocalSetting("SironaScansFolder");
            string PatientName = "";
            if (Directory.Exists(sironaFolder))
            {
                string finalFolderName = "";
                var directories = Directory.GetDirectories(sironaFolder, "*", SearchOption.TopDirectoryOnly);
                string currentDirectory = directories.FirstOrDefault(x => x.Contains($"{SironaOrderNumber}_"))!;

                if (File.Exists($@"{currentDirectory}\DentalCase.xml"))
                {
                    _ = bool.TryParse(ReadLocalSetting("OpenUpSironaScanFolder"), out bool OpenUpFolder);

                    XmlDocument doc = new();
                    doc.Load($@"{currentDirectory}\DentalCase.xml");

                    XmlElement root = doc.DocumentElement!;
                    List<XmlNode> nodes = [];
                    List<XmlNode> childNodes = [];

                    foreach (XmlNode node in root.ChildNodes)
                    {
                        if (!nodes.Contains(node))
                            nodes.Add(node);
                    }

                    XmlNode xmlNode = nodes.FirstOrDefault(x => x.Name.Equals("Patient"))!;

                    foreach (XmlNode node in xmlNode.ChildNodes)
                    {
                        if (!childNodes.Contains(node))
                            childNodes.Add(node);
                    }
                    
                    foreach (XmlNode node in childNodes)
                    {
                        if (node.Name.Equals("FullName"))
                        {
                            PatientName = node.InnerText;

                            if (PatientName.Contains('(') && PatientName.Contains(')'))
                            {
                                string firstPart = PatientName.Substring(0, PatientName.IndexOf("("));
                                string secondPart = PatientName.Substring(PatientName.IndexOf(")") + 1);

                                PatientName = firstPart.Trim() + " " + secondPart.Trim();
                                PatientName = PatientName.TrimEnd().TrimStart().Replace(" ", "_");
                            }
                        }
                    }

                    finalFolderName = $"{PanNumber}-{PatientName}_{SironaOrderNumber}";

                    

                    try
                    {
                        Directory.Move($@"{currentDirectory}", $@"{sironaFolder}{finalFolderName}");

                        try
                        {
                            if (OpenUpFolder)
                                Process.Start("explorer.exe", "\"" + $@"{sironaFolder}{finalFolderName}\" + "\"");
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"[{ex.LineNumber()}] {ex.Message}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"[{ex.LineNumber()}] {ex.Message}");
                        MessageBox.Show("#78f: ", ex.Message);
                    }
                }
            }

            SendSironaInfoToServer(PanNumber, PatientName, SironaOrderNumber, "SIRONA");
        }



        IsItSironaPrescription = false;
    }

    private void EditPDF(string FilePath, string NextPanNumber = "", bool MarkAsRush = false, bool MarkAsSentTo = false, string SentTo = "")
    {
        string SavedPDF = "";
        string SavedPDFCopy;
        
        try
        {
            // INJECTING PAN NUMBER TO PDF
            if (!MarkAsRush && !MarkAsSentTo && !string.IsNullOrEmpty(NextPanNumber))
            {
                //Load a PDF document.
                PdfLoadedDocument doc = new (FilePath);
                Thread.Sleep(600);
                //Get first page from document.
                PdfLoadedPage page = (doc.Pages[0] as PdfLoadedPage)!;
                //Create PDF graphics for the page
                PdfGraphics graphics = page.Graphics;

                //Set the standard font.
                PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 40, PdfFontStyle.Bold);
                //Draw the text.
                if (PageHeaderIsHigh == "1")
                    graphics.DrawString(NextPanNumber, font, PdfBrushes.Black, new System.Drawing.PointF(500, 55));
                else if (PageHeaderIsHigh == "0")
                    graphics.DrawString(NextPanNumber, font, PdfBrushes.Black, new System.Drawing.PointF(500, 38));
                else if (PageHeaderIsHigh == "3") // blue header Intelliscan / IS3D / Shining 3D type of prescr
                    graphics.DrawString(NextPanNumber, font, PdfBrushes.Black, new System.Drawing.PointF(130, 22));
                else if (PageHeaderIsHigh == "4") // Sirona type of prescr
                    graphics.DrawString(NextPanNumber, font, PdfBrushes.Black, new System.Drawing.PointF(210, 40));
                else if (PageHeaderIsHigh == "5") // Medit type of prescr
                    graphics.DrawString(NextPanNumber, font, PdfBrushes.Black, new System.Drawing.PointF(310, 25));
                else if (PageHeaderIsHigh == "6") // DSCore type of prescr
                    graphics.DrawString(NextPanNumber, font, PdfBrushes.Black, new System.Drawing.PointF(250, 30));

                //Save the document.
                SavedPDF = PDFTemp + "\\" + NextPanNumber + ".pdf";
                SavedPDFCopy = PDFTemp + "\\" + NextPanNumber + "-copy.pdf";
            
                doc.Save(SavedPDF);
                doc.Save(SavedPDFCopy);
            
                doc.Close(true);


                WriteLocalSetting("BaseFile", FilePath.Replace("'", "|"));
                WriteLocalSetting("LastFile", PDFTemp + "\\" + NextPanNumber + ".pdf");
                WriteLocalSetting("LastPanNumber", NextPanNumber);


                SavePrescriptionFromPdfToImage(SavedPDFCopy);
            }
            // INJECTING RUSH TO PDF
            else if (MarkAsRush)
            {
                string lastPanNr = ReadLocalSetting("LastPanNumber");
                string LastFile = ReadLocalSetting("LastFile");


                //Load a PDF document.
                PdfLoadedDocument doc = new(LastFile);
                Thread.Sleep(600);
                //Get first page from document.
                PdfLoadedPage page = (doc.Pages[0] as PdfLoadedPage)!;
                //Create PDF graphics for the page
                PdfGraphics graphics = page.Graphics;

                //Set the standard font.
                PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 40, PdfFontStyle.Bold);
                //Draw the text.
                if (PageHeaderIsHigh == "1")
                    graphics.DrawString("RUSH", font, PdfBrushes.DarkRed, new System.Drawing.PointF(120, 219));
                else if (PageHeaderIsHigh == "0")
                    graphics.DrawString("RUSH", font, PdfBrushes.DarkRed, new System.Drawing.PointF(120, 201));
                else if (PageHeaderIsHigh == "3")
                    graphics.DrawString("RUSH", font, PdfBrushes.DarkRed, new System.Drawing.PointF(306, 22));
                else if (PageHeaderIsHigh == "4")
                    graphics.DrawString("RUSH", font, PdfBrushes.DarkRed, new System.Drawing.PointF(210, 92));
                else if (PageHeaderIsHigh == "5")
                    graphics.DrawString("RUSH", font, PdfBrushes.DarkRed, new System.Drawing.PointF(310, 92));
                else if (PageHeaderIsHigh == "6")
                    graphics.DrawString("RUSH", font, PdfBrushes.DarkRed, new System.Drawing.PointF(375, 30));

                //Save the document.
                SavedPDF = PDFTemp + "\\" + lastPanNr + ".pdf";
                SavedPDFCopy = PDFTemp + "\\" + lastPanNr + "-copy.pdf";

                doc.Save(SavedPDF);
                doc.Save(SavedPDFCopy);

                doc.Close(true);

                SavePrescriptionFromPdfToImage(SavedPDFCopy, true);
            }
            // INJECTING SENT TO LABEL TO PDF
            else if (MarkAsSentTo && !string.IsNullOrEmpty(SentTo))
            {
                string lastPanNr = ReadLocalSetting("LastPanNumber");
                string LastFile = ReadLocalSetting("LastFile");


                //Load a PDF document.
                PdfLoadedDocument doc = new(LastFile);
                Thread.Sleep(600);
                //Get first page from document.
                PdfLoadedPage page = (doc.Pages[0] as PdfLoadedPage)!;
                //Create PDF graphics for the page
                PdfGraphics graphics = page.Graphics;

                //Set the standard font.
                PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 30, PdfFontStyle.Bold);
                //Draw the text.
                if (PageHeaderIsHigh == "1")
                    graphics.DrawString("Sent to " + SentTo + " " + DateTime.Now.ToString("MM/dd"), font, PdfBrushes.Black, new System.Drawing.PointF(260, 225));
                else if (PageHeaderIsHigh == "0")
                    graphics.DrawString("Sent to " + SentTo + " " + DateTime.Now.ToString("MM/dd"), font, PdfBrushes.Black, new System.Drawing.PointF(260, 205));
                else if (PageHeaderIsHigh == "3")
                    graphics.DrawString("Sent to " + SentTo + " " + DateTime.Now.ToString("MM/dd"), font, PdfBrushes.Black, new System.Drawing.PointF(120, 383));
                else if (PageHeaderIsHigh == "4")
                    graphics.DrawString("Sent to " + SentTo + " " + DateTime.Now.ToString("MM/dd"), font, PdfBrushes.Black, new System.Drawing.PointF(210, 310));
                else if (PageHeaderIsHigh == "5")
                    graphics.DrawString("Sent to " + SentTo + " " + DateTime.Now.ToString("MM/dd"), font, PdfBrushes.Black, new System.Drawing.PointF(190, 235));
                else if (PageHeaderIsHigh == "6")
                    graphics.DrawString("Sent to " + SentTo + " " + DateTime.Now.ToString("MM/dd"), font, PdfBrushes.Black, new System.Drawing.PointF(190, 205));


                //Save the document.
                SavedPDF = PDFTemp + "\\" + lastPanNr + ".pdf";
                SavedPDFCopy = PDFTemp + "\\" + lastPanNr + "-copy.pdf";

                doc.Save(SavedPDF);
                doc.Save(SavedPDFCopy);

                doc.Close(true);

                SavePrescriptionFromPdfToImage(SavedPDFCopy, true);
            }

        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[{ex.LineNumber()}] {ex.Message}");
        }

        

        try
        {
            
            if (PmOpenUpPrescriptionsBool && File.Exists(SavedPDF))
            {
                var p = new Process();
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.Arguments = $"/c start msedge {SavedPDF}";
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[{ex.LineNumber()}] {ex.Message}");
            MessageBox.Show(ex.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        
    }

    private async void PmMarkCaseAsRush()
    {
        MessageBoxResult dlg = MessageBox.Show(_MainWindow, "Sure you want to mark the case RUSH?",
                                                                        "Stats Client", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (dlg == MessageBoxResult.Yes)
        {
            PmRushButtonShows = Visibility.Hidden;
            await Task.Run(() => EditPDF("", "", true));
        }
        else
            return;
    }
    
    private async void PmMarkCaseWithLabelSendTo()
    {
        if (PmSelectedSentTo is null || PmSelectedSentTo == "")
        {
            MessageBox.Show("Please select a name from the list to add to the prescription", "Stats Client", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        MessageBoxResult dlg = MessageBox.Show(_MainWindow, $"Sure you want to add label '{PmSelectedSentTo}' to prescription?",
                                                                        "Stats Client", MessageBoxButton.YesNo, MessageBoxImage.Warning);
        if (dlg == MessageBoxResult.Yes)
        {
            PmSendToButtonShows = Visibility.Hidden;
            await Task.Run(() => EditPDF("", "", false, true, PmSelectedSentTo));
        }
        else
            return;
    }

    

    private void SavePrescriptionFromPdfToImage(string savedPDFCopy, bool IgnoreExistingImage = false)
    {
        string FinalLocation = ReadLocalSetting("FinalPrescriptionsFolder");
        string NextPanNumber = ReadLocalSetting("LastPanNumber");
        string BaseFile = ReadLocalSetting("BaseFile");

        Application.Current.Dispatcher.Invoke(new Action(async () =>
        {
            DocumentStreamFinalPrescription = new FileStream(savedPDFCopy, FileMode.OpenOrCreate);

            try
            {
                Stream[] outputStream;
                //Load the PDF document as a stream
                using (FileStream inputStream = DocumentStreamFinalPrescription)
                {
                    imageConverter.Load(inputStream);
                    outputStream = imageConverter.Convert(0, imageConverter.PageCount - 1, 200, 200, false, false);
                }
                //Convert PDF to Image.

                for (int i = 0; i < outputStream.Length; i++)
                {
                    Bitmap image = new(outputStream[i]);
                    image.Save($@"{DataBaseFolder}Temp\{NextPanNumber}-{i}.png");
                    image.Dispose();
                }

                //if (!PwRush)
                //buttonMakeItRush.Visible = true;


                int count = outputStream.Length;
                PdfPageCount = count.ToString();



                /// till checked

                // checking if any empty pages in it
                bool LastPageIsEmpty = false;
                if (count > 1)
                {
                    for (int i = 0; i < count; i++)
                    {
                        try
                        {
                            string SavedPDF = ReadLocalSetting("LastFile");
                            //Load the PDF document.
                            PdfLoadedDocument loadedDocument = new(SavedPDF);
                            //Gets the page.
                            PdfPageBase loadedPage = loadedDocument.Pages[i] as PdfPageBase;
                            //Get the page is blank or not.
                            LastPageIsEmpty = loadedPage.IsBlank;

                            //Close the document.
                            loadedDocument.Close(true);
                            // END
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"[{ex.LineNumber()}] {ex.Message}");
                            MessageBox.Show("#8\n\n" + ex.Message);
                        }

                    }

                    // decreasing count if last page is empty page
                    if (LastPageIsEmpty)
                        count--;
                }

                if (count == 1)
                {
                    if (FinalLocation != "")
                    {
                        if (NextPanNumber.Length > 0)
                        {
                            try
                            {
                                Bitmap image = new(outputStream[0]);
                                // Save the image.

                                if (!File.Exists(FinalLocation + "\\" + DateTime.Now.ToString("MM-dd") + "\\" + NextPanNumber + ".png"))
                                {
                                    PngBitmapEncoder encoder = new();

                                    string photolocation = FinalLocation + "\\" + DateTime.Now.ToString("MM-dd") + "\\" + NextPanNumber + ".png";
                                    image.Save(photolocation, System.Drawing.Imaging.ImageFormat.Png);
                                    image.Dispose();

                                    // if after questionary we choose that the current prescription is the same as the last one, deleting the newly made paper and returning the used pan number as new
                                    if (await Task.Run(() => CheckIfCurrentPrescriptionIsSameAsLastOne(photolocation)))
                                    {
                                        PmAddNewPanNumber(NextPanNumber);
                                        File.Delete(photolocation);
                                        return;
                                    }

                                    Bitmap img;
                                    using (var bmpTemp = new Bitmap(FinalLocation + "\\" + DateTime.Now.ToString("MM-dd") + "\\" + NextPanNumber + ".png"))
                                    {
                                        img = new Bitmap(bmpTemp);
                                    }

                                    ImageSource imageSource = ImageSourceFromBitmap(img);
                                    
                                    PmSavedPrescription = imageSource;

                                    //if (IsItSironaPrescription)
                                    TriggerSironaFolderRename(NextPanNumber, FinalLocation);
                                }
                                else
                                {
                                    if (IgnoreExistingImage)
                                    {
                                        try
                                        {
                                            File.Delete(FinalLocation + "\\" + DateTime.Now.ToString("MM-dd") + "\\" + NextPanNumber + ".png");
                                        }
                                        catch (Exception ex)
                                        {
                                            Debug.WriteLine($"[{ex.LineNumber()}] {ex.Message}");
                                        }

                                        PngBitmapEncoder encoder = new();

                                        string photolocation = FinalLocation + "\\" + DateTime.Now.ToString("MM-dd") + "\\" + NextPanNumber + ".png";
                                        image.Save(photolocation, System.Drawing.Imaging.ImageFormat.Png);
                                        image.Dispose();

                                        // if after questionary we choose that the current prescription is the same as the last one, deleting the newly made paper and returning the used pan number as new
                                        if (await Task.Run(() => CheckIfCurrentPrescriptionIsSameAsLastOne(photolocation)))
                                        {
                                            PmAddNewPanNumber(NextPanNumber);
                                            File.Delete(photolocation);
                                            return;
                                        }

                                        Bitmap img;
                                        using (var bmpTemp = new Bitmap(FinalLocation + "\\" + DateTime.Now.ToString("MM-dd") + "\\" + NextPanNumber + ".png"))
                                        {
                                            img = new Bitmap(bmpTemp);
                                        }

                                        ImageSource imageSource = ImageSourceFromBitmap(img);
                                        
                                        PmSavedPrescription = imageSource;

                                        //if (IsItSironaPrescription)
                                        TriggerSironaFolderRename(NextPanNumber, FinalLocation);
                                    }
                                    else
                                    {
                                        MessageBoxResult dlg = MessageBox.Show("This number is already used for a prescription, would you like to overwrite the original file?",
                                                                        "Stats Client", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                                        if (dlg == MessageBoxResult.Yes)
                                        {
                                            PngBitmapEncoder encoder = new();

                                            string photolocation = FinalLocation + "\\" + DateTime.Now.ToString("MM-dd") + "\\" + NextPanNumber + ".png";
                                            image.Save(photolocation, System.Drawing.Imaging.ImageFormat.Png);
                                            image.Dispose();

                                            // if after questionary we choose that the current prescription is the same as the last one, deleting the newly made paper and returning the used pan number as new
                                            if (await Task.Run(() => CheckIfCurrentPrescriptionIsSameAsLastOne(photolocation)))
                                            {
                                                PmAddNewPanNumber(NextPanNumber);
                                                File.Delete(photolocation);
                                                return;
                                            }

                                            Bitmap img;
                                            using (var bmpTemp = new Bitmap(FinalLocation + "\\" + DateTime.Now.ToString("MM-dd") + "\\" + NextPanNumber + ".png"))
                                            {
                                                img = new Bitmap(bmpTemp);
                                            }

                                            ImageSource imageSource = ImageSourceFromBitmap(img);
                                            
                                            PmSavedPrescription = imageSource;

                                            //if (IsItSironaPrescription)
                                            TriggerSironaFolderRename(NextPanNumber, FinalLocation);
                                        }
                                        else
                                            return;
                                    }

                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"[{ex.LineNumber()}] {ex.Message}");
                                MessageBox.Show("#9\n\n" + ex.Message);
                            }


                            if (!IgnoreExistingImage)
                            {
                                foreach (Border item in _MainWindow.pmPanelPanek.Children.OfType<Border>().ToList())
                                {
                                    if (item.Tag == NextPanNumber)
                                        _MainWindow.pmPanelPanek.Children.Remove(item);
                                }

                                PmPanNumberList.Remove(NextPanNumber);
                                RemovePanNumberFromAvailablePans(NextPanNumber);
                                FillUpEmptyPanNumberPanel();
                            }
                        }
                    }
                }
                else
                {
                    if (FinalLocation != "")
                    {
                        if (NextPanNumber.Length > 0)
                        {
                            List<string> files = [];
                            for (int i = 0; i < count; i++)
                            {
                                try
                                {
                                    Bitmap image = new(outputStream[i]);

                                    Bitmap bitImage = Crop(image, PageHeaderIsHigh);
                                    // Save the image.
                                    string file = DataBaseFolder + "Temp\\" + NextPanNumber + "-" + i.ToString() + ".png";
                                    bitImage.Save(file, System.Drawing.Imaging.ImageFormat.Png);
                                    files.Add(file);
                                    bitImage.Dispose();
                                    image.Dispose();
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine($"[{ex.LineNumber()}] {ex.Message}");
                                    MessageBox.Show("#10\n\n" + ex.Message);
                                }
                            }

                            try
                            {
                                if (!File.Exists(FinalLocation + "\\" + DateTime.Now.ToString("MM-dd") + "\\" + NextPanNumber + ".png"))
                                {
                                    await Task.Run(() => CombineImages(files).Save(FinalLocation + "\\" + DateTime.Now.ToString("MM-dd") + "\\" + NextPanNumber + ".png", System.Drawing.Imaging.ImageFormat.Png));

                                    Bitmap img;
                                    using (var bmpTemp = new Bitmap(FinalLocation + "\\" + DateTime.Now.ToString("MM-dd") + "\\" + NextPanNumber + ".png"))
                                    {
                                        img = new Bitmap(bmpTemp);
                                    }

                                    // if after questionary we choose that the current prescription is the same as the last one, deleting the newly made paper and returning the used pan number as new
                                    if (CheckIfCurrentPrescriptionIsSameAsLastOne(FinalLocation + "\\" + DateTime.Now.ToString("MM-dd") + "\\" + NextPanNumber + ".png"))
                                    {
                                        PmAddNewPanNumber(NextPanNumber);
                                        File.Delete(FinalLocation + "\\" + DateTime.Now.ToString("MM-dd") + "\\" + NextPanNumber + ".png");
                                        return;
                                    }

                                    ImageSource imageSource = ImageSourceFromBitmap(img);
                                    //ImageSource imageSource = new BitmapImage(new Uri(FinalLocation + "\\" + DateTime.Now.ToString("MM-dd") + "\\" + NextPanNumber + ".png"));
                                    PmSavedPrescription = imageSource;

                                    TriggerSironaFolderRename(NextPanNumber, FinalLocation);
                                }
                                else
                                {
                                    if (IgnoreExistingImage)
                                    {
                                        try
                                        {
                                            File.Delete(FinalLocation + "\\" + DateTime.Now.ToString("MM-dd") + "\\" + NextPanNumber + ".png");
                                        }
                                        catch (Exception ex)
                                        {
                                            Debug.WriteLine($"[{ex.LineNumber()}] {ex.Message}");
                                        }

                                        await Task.Run(() => CombineImages(files).Save(FinalLocation + "\\" + DateTime.Now.ToString("MM-dd") + "\\" + NextPanNumber + ".png", System.Drawing.Imaging.ImageFormat.Png));

                                        Bitmap img;
                                        using (var bmpTemp = new Bitmap(FinalLocation + "\\" + DateTime.Now.ToString("MM-dd") + "\\" + NextPanNumber + ".png"))
                                        {
                                            img = new Bitmap(bmpTemp);
                                        }

                                        // if after questionary we choose that the current prescription is the same as the last one, deleting the newly made paper and returning the used pan number as new
                                        if (await Task.Run(() => CheckIfCurrentPrescriptionIsSameAsLastOne(FinalLocation + "\\" + DateTime.Now.ToString("MM-dd") + "\\" + NextPanNumber + ".png")))
                                        {
                                            PmAddNewPanNumber(NextPanNumber);
                                            File.Delete(FinalLocation + "\\" + DateTime.Now.ToString("MM-dd") + "\\" + NextPanNumber + ".png");
                                            return;
                                        }

                                        ImageSource imageSource = ImageSourceFromBitmap(img);
                                        //ImageSource imageSource = new BitmapImage(new Uri(FinalLocation + "\\" + DateTime.Now.ToString("MM-dd") + "\\" + NextPanNumber + ".png"));
                                        PmSavedPrescription = imageSource;
                                        //if (IsItSironaPrescription)
                                        TriggerSironaFolderRename(NextPanNumber, FinalLocation);
                                    }
                                    else
                                    {
                                        MessageBoxResult dlg = MessageBox.Show("This number is already used for a prescription, would you like to overwrite the original file?",
                                                                        "Stats Client", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                                        if (dlg == MessageBoxResult.Yes)
                                        {
                                            await Task.Run(() => CombineImages(files).Save(FinalLocation + "\\" + DateTime.Now.ToString("MM-dd") + "\\" + NextPanNumber + ".png", System.Drawing.Imaging.ImageFormat.Png));


                                            // if after questionary we choose that the current prescription is the same as the last one, deleting the newly made paper and returning the used pan number as new
                                            if (await Task.Run(() => CheckIfCurrentPrescriptionIsSameAsLastOne(FinalLocation + "\\" + DateTime.Now.ToString("MM-dd") + "\\" + NextPanNumber + ".png")))
                                            {
                                                PmAddNewPanNumber(NextPanNumber);
                                                File.Delete(FinalLocation + "\\" + DateTime.Now.ToString("MM-dd") + "\\" + NextPanNumber + ".png");
                                                return;
                                            }

                                            Bitmap img;
                                            using (var bmpTemp = new Bitmap(FinalLocation + "\\" + DateTime.Now.ToString("MM-dd") + "\\" + NextPanNumber + ".png"))
                                            {
                                                img = new Bitmap(bmpTemp);
                                            }

                                            ImageSource imageSource = ImageSourceFromBitmap(img);
                                            //ImageSource imageSource = new BitmapImage(new Uri(FinalLocation + "\\" + DateTime.Now.ToString("MM-dd") + "\\" + NextPanNumber + ".png"));
                                            PmSavedPrescription = imageSource;
                                            //if (IsItSironaPrescription)
                                            TriggerSironaFolderRename(NextPanNumber, FinalLocation);
                                        }
                                        else
                                            return;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"[{ex.LineNumber()}] {ex.Message}");
                                MessageBox.Show("#11s\n\n" + ex.Message);
                            }

                            if (!IgnoreExistingImage)
                            {

                                foreach (Border item in _MainWindow.pmPanelPanek.Children.OfType<Border>().ToList())
                                {
                                    if (item.Tag == NextPanNumber)
                                        _MainWindow.pmPanelPanek.Children.Remove(item);
                                }

                                PmPanNumberList.Remove(NextPanNumber);
                                RemovePanNumberFromAvailablePans(NextPanNumber);

                                FillUpEmptyPanNumberPanel();
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[{ex.LineNumber()}] {ex.Message}");
                MessageBox.Show("#11d\n\n" + ex.Message);
            }

            try
            {
                ProcessingDigiPrescriptionNow = Visibility.Collapsed;
                DocumentStreamPixelCheck?.Dispose();
                //need to replace apostrophe to | for Medit type of prescription names..
                File.Delete(BaseFile.Replace("|", "'"));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[{ex.LineNumber()}] {ex.Message}");
                MessageBox.Show("#12\n\n" + ex.Message);
            }

            SystemSounds.Beep.Play();
            await BlinkWindow("yellow");
        }));
    }

    private bool CheckIfCurrentPrescriptionIsSameAsLastOne(string photolocation)
    {
        double ActualSize = new FileInfo(photolocation).Length;

        if (PmLastPrescriptionSize == ActualSize)
        {
            MessageBoxResult dlg = MessageBox.Show("This case's prescription might be a duplicate..\nWould you like to open up last saved prescription to see if they are the same?", "Stats Client", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (dlg == MessageBoxResult.Yes)
            {
                try
                {
                    //TODO: change this to open with hidden cmd window
                    Process.Start("cmd.exe", "/C \"" + photolocation + "\"");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[{ex.LineNumber()}] {ex.Message}");
                }
                MessageBoxResult res = MessageBox.Show("Is they are the same?", "Stats Client", MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (res == MessageBoxResult.Yes)
                {
                    PmSavedPrescription = null;
                    return true;
                }
                
            }
        }

        PmLastPrescriptionSize = ActualSize;
        return false;
    }

    private void PmOpenUpPrescriptions()
    {
        WriteLocalSetting("PmOpenUpPrescriptions", PmOpenUpPrescriptionsBool.ToString());
    }

    private static void PmSelectTargetFolder(object obj)
    {
        string folderIdentifier = obj.ToString()!;

        var folderDialog = new OpenFolderDialog
        {
            Title = "Please select a folder"
        };

        if (folderDialog.ShowDialog() == true)
        {
            var folderName = folderDialog.FolderName;
            WriteLocalSetting(folderIdentifier, folderName + @"\");

            StartInitialTasks();
        }
    }
    #endregion PRESCRIPTION MAKER METHODS

    private void FillUpPendingDigiCaseNumberList(bool forceUpdate = false)
    {
        List<ProcessedPanNumberModel> list = GetAllNotCollectedNumbers();
        if (CbSettingShowPendingDigiCases)
            PendingDigiNumbersWaitingToCollectInt = list.Count;
        else
            PendingDigiNumbersWaitingToCollectInt = 0;

        if (!forceUpdate)
        {
            if (list.Count != PendingDigiNumbersWaitingToCollect.Count)
            {
                PendingDigiNumbersWaitingToCollect = [];
                foreach(ProcessedPanNumberModel item in list)
                    PendingDigiNumbersWaitingToCollect.Add(item.PanNumber!);

                MainWindow.Instance.listviewPendingDigiNumbers.ItemsSource = PendingDigiNumbersWaitingToCollect;
                MainWindow.Instance.listviewPendingDigiNumbers.Items.Refresh();

                if (list.Count > 0)
                {
                    SelectedPendingDigiNumber = PendingDigiNumbersWaitingToCollect[0];
                    MainWindow.Instance.listviewPendingDigiNumbers.SelectedIndex = 0;   
                }
            }
        }
        else
        {
            PendingDigiNumbersWaitingToCollect = [];
            foreach (ProcessedPanNumberModel item in list)
                PendingDigiNumbersWaitingToCollect.Add(item.PanNumber!);

            MainWindow.Instance.listviewPendingDigiNumbers.ItemsSource = PendingDigiNumbersWaitingToCollect;
            MainWindow.Instance.listviewPendingDigiNumbers.Items.Refresh();


            if (list.Count > 0)
            {
                SelectedPendingDigiNumber = PendingDigiNumbersWaitingToCollect[0];
                MainWindow.Instance.listviewPendingDigiNumbers.SelectedIndex = 0;
            }           
        }

        List<ProcessedPanNumberModel> allPendingDigi = GetAllPendingDigiNumbersInLast30Days();
        PendingDigiNumbersWaitingToProcess = [];

        foreach (ProcessedPanNumberModel item in allPendingDigi)
        {
            ProcessedPanNumberModel model = new()
            {
                PanNumber = item.PanNumber,
                Comment = item.Comment
            };

            if (item.IsProcessed == "true")
                model.IsProcessed = "✓";
            else
                model.IsProcessed = "";
                        
            if (item.IsCollected == "true")
                model.IsCollected = "✓";
            else
                model.IsCollected = "";

            _ = DateTime.TryParse(item.PostedTime!, out DateTime postedDateTime);
            
            string postedTime = postedDateTime.ToString("M/d - h:mm tt");

            if (postedDateTime.ToString("yyyy-MM-dd") == DateTime.Now.ToString("yyyy-MM-dd"))
                postedTime = "Today";
            else if (postedDateTime.ToString("yyyy-MM-dd") == DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"))
                postedTime = "Yesterday";

            string processedTime;
            if (DateTime.TryParse(item.ProcessedTime!, out DateTime processedDateTime))
            {
                processedTime = processedDateTime.ToString("M/d - h:mm tt");
                if (processedDateTime.ToString("yyyy-MM-dd") == DateTime.Now.ToString("yyyy-MM-dd"))
                    processedTime = "Today";
                else if (processedDateTime.ToString("yyyy-MM-dd") == DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"))
                    processedTime = "Yesterday";
            }
            else
                processedTime = "-";

            model.ProcessedTime = processedTime;
            model.PostedTime = postedTime;
            model.ProcessedBy = ReadComputerName(item.ProcessedBy!);
            model.PostedBy = ReadComputerName(item.PostedBy!);
            model.PostedTimeForSorting = item.PostedTimeForSorting;

            if ((postedDateTime.ToString("yyyy-MM-dd") == DateTime.Now.ToString("yyyy-MM-dd")) || string.IsNullOrEmpty(model.IsProcessed))
            {
                if (!string.IsNullOrEmpty(model.IsProcessed))
                    model.LineColor = "LightGreen"; // Today - processed
                else
                {
                    if (string.IsNullOrEmpty(model.IsCollected))
                        model.LineColor = "#fa91c5"; // Today - not collected yet
                    else
                        model.LineColor = "Yellow"; // Today - collected but not processed yet
                }
            }
            else if (postedDateTime.ToString("yyyy-MM-dd") == DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"))
                model.LineColor = "#c2effc"; // Yesterday


            PendingDigiNumbersWaitingToProcess.Add(model);

        }
        PendingDigiNumbersWaitingToProcess = PendingDigiNumbersWaitingToProcess.OrderBy(x => x.IsProcessed).ThenByDescending(x => x.PostedTimeForSorting).ToList();
    }

    private void FswTriosFolderWatcher_Created(object sender, FileSystemEventArgs e)
    {
        CountTriosCases();
    }

    private void CountTriosCases()
    {
        int directoryCount = Directory.GetDirectories(TriosInboxFolder).Length;
        NewTriosCaseInInboxCount = directoryCount;
    }

    private void FswTriosFolderWatcher_Deleted(object sender, FileSystemEventArgs e)
    {
        int directoryCount = Directory.GetDirectories(TriosInboxFolder).Length;
        NewTriosCaseInInboxCount = directoryCount;
    }

    private void GeneralTimer_Tick(object? sender, EventArgs e)
    {
        DateTime dtime = DateTime.Now;
        int hours = dtime.Hour;
        int minutes = dtime.Minute;
        int seconds = dtime.Second;
        

        if (hours > 12)
            _ = 12;

       

        // Run background tasks
        if (!bwBackgroundTasks.IsBusy && AppIsFullyLoaded)
            bwBackgroundTasks.RunWorkerAsync(argument: minutes.ToString() + "|" + seconds.ToString());
    }

    #region SETTINGS TAB METHODS
    

    public void OpenUpOrderInfoWindow()
    {
        OrderInfoWindow orderInfoWindow = new(ThreeShapeObject!)
        {
            Owner = _MainWindow
        };
        orderInfoWindow.ShowDialog();
    }
    
    public async void OpenUpRenameOrderWindow()
    {
        OrderRenameWindow orderRenameWindow = new(ThreeShapeObject!)
        {
            Owner = _MainWindow
        };
        await LockOrderIn3Shape(ThreeShapeObject!.IntOrderID!);
        orderRenameWindow.ShowDialog();
        ListUpdateTimer_Tick(null, null);
    }

    
    private void CbSettingGlassyEffectMethod()
    {
        WriteLocalSetting("GlassyEffect", CbSettingGlassyEffect.ToString());
    }
    
    private void CbSettingShowUnitNumbersMethod()
    {
        WriteLocalSetting("ShowUnitNumbers", ShowUnitNumbers.ToString());
    }
    
    private void CbSettingShowBottomInfoBarMethod()
    {
        WriteLocalSetting("ShowBottomInfoBar", ShowBottomInfoBar.ToString());
        if (ShowBottomInfoBar)
            BottomBarSize = 120;
        else
        {
            CbSettingShowDigiDetails = false;
            WriteLocalSetting("ShowDigiDetails", CbSettingShowDigiDetails.ToString());
            BottomBarSize = 35;
        }
    }
    
    private void CbSettingShowDigiCasesMethod()
    {
        WriteLocalSetting("ShowDigiCases", CbSettingShowDigiCases.ToString());
        
    }
    
    private void CbSettingShowPendingDigiCasesMethod()
    {
        WriteLocalSetting("ShowPendingDigiCases", CbSettingShowPendingDigiCases.ToString());
    }
    
    private void CbSettingIncludePendingDigiCasesInNewlyArrivedMethod()
    {
        WriteLocalSetting("IncludePendingDigiCases", CbSettingIncludePendingDigiCasesInNewlyArrived.ToString());    
    }
    
    private void CbSettingShowDigiDetailsMethod()
    {
        if (!ShowBottomInfoBar)
        {
            CbSettingShowDigiDetails = false;
            ShowNotificationMessage("Cannot activate!", "This option only works when the \"Show bottom info bar\" option is active", NotificationIcon.Warning);
        }
        WriteLocalSetting("ShowDigiDetails", CbSettingShowDigiDetails.ToString());
    }
    
    private void CbSettingWatchFolderPrescriptionMakerMethod()
    {
        WriteLocalSetting("ActivePrescriptionMaker", CbSettingWatchFolderPrescriptionMaker.ToString());
        if (!string.IsNullOrEmpty(fswPrescriptionMaker.Path) && Directory.Exists(fswPrescriptionMaker.Path))
        {
            if (CbSettingWatchFolderPrescriptionMaker)
            {
                fswPrescriptionMaker.Path = PmWatchedPdfFolder;
                fswPrescriptionMaker.Filter = "*.pdf";
                fswPrescriptionMaker.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
                fswPrescriptionMaker.Created += new FileSystemEventHandler(FswPrescriptionMaker_Created);
                fswPrescriptionMaker.Changed += new FileSystemEventHandler(FswPrescriptionMaker_Changed);
                fswPrescriptionMaker.EnableRaisingEvents = true;
            }

            fswPrescriptionMaker.EnableRaisingEvents = CbSettingWatchFolderPrescriptionMaker;
        }
        else
            fswPrescriptionMaker.EnableRaisingEvents = CbSettingWatchFolderPrescriptionMaker;
    }
    
    private void CbSettingOpenUpSironaScanFolderMethod()
    {
        WriteLocalSetting("OpenUpSironaScanFolder", CbSettingOpenUpSironaScanFolder.ToString());
    }
    
    private void CbSettingShowEmptyPanCountMethod()
    {
        WriteLocalSetting("ShowEmptyPanCount", CbSettingShowEmptyPanCount.ToString());
    }
    
    private void CbSettingShowDigiPrescriptionsCountMethod()
    {
        WriteLocalSetting("ShowDigiPrescriptionsCount", CbSettingShowDigiPrescriptionsCount.ToString());
    }
    
    private void CbSettingShowDigiCasesIn3ShapeTodayCountMethod()
    {
        WriteLocalSetting("ShowDigiCasesIn3ShapeTodayCount", CbSettingShowDigiCasesIn3ShapeTodayCount.ToString());
    }

    private void CbSettingModuleFolderSubscriptionMethod()
    {
        WriteLocalSetting("ModuleFolderSubscription", CbSettingModuleFolderSubscription.ToString());
    }
    
    private void CbSettingModuleAccountInfosMethod()
    {
        WriteLocalSetting("ModuleAccountInfos", CbSettingModuleAccountInfos.ToString());
        if (CbSettingModuleAccountInfos)
            GetAccountInfos();
    }
    
    private void CbSettingModuleSmartOrderNamesMethod()
    {
        WriteLocalSetting("ModuleSmartOrderNames", CbSettingModuleSmartOrderNames.ToString());
    }
    
    private void CbSettingModulePrescriptionMakerMethod()
    {
        WriteLocalSetting("ModulePrescriptionMaker", CbSettingModulePrescriptionMaker.ToString());

        if (!CbSettingModulePrescriptionMaker)
        {
            CbSettingWatchFolderPrescriptionMaker = false;
            CbSettingExtractIteroZipFiles = false;
            CbSettingWatchFolderPrescriptionMakerMethod();
            CbSettingExtractIteroZipFilesMethod();
        }
    }
    
    private void CbSettingModulePendingDigitalsMethod()
    {
        WriteLocalSetting("ModulePendingDigitals", CbSettingModulePendingDigitals.ToString());
    }
    
    private void CbSettingExtractIteroZipFilesMethod()
    {
        WriteLocalSetting("ExtractIteroZipFiles", CbSettingExtractIteroZipFiles.ToString());

        if (fswIteroZipFileWhatcher.Path is null)
        {
            if (CbSettingExtractIteroZipFiles && Directory.Exists(PmDownloadFolder) && Directory.Exists(PmIteroExportFolder))
            {
                fswIteroZipFileWhatcher.Path = PmDownloadFolder;
                fswIteroZipFileWhatcher.Filter = "iTero_Export_*.zip";
                fswIteroZipFileWhatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
                fswIteroZipFileWhatcher.Created += new FileSystemEventHandler(FswIteroZipFileWhatcher_Created);
                fswIteroZipFileWhatcher.Changed += new FileSystemEventHandler(FswIteroZipFileWhatcher_Created);
                fswIteroZipFileWhatcher.EnableRaisingEvents = true;
            }
        }
    }

    #endregion SETTINGS TAB METHODS

    private void HideNotification()
    {
        NotificationTimer_Tick(null, null);
    }
    
    
    private void NotificationTimer_Tick(object? sender, EventArgs e)
    {
        IsNotificationMessageShows = Visibility.Hidden;
        NotificationMessageTitle = "";
        NotificationMessageBody = "";
        NotificationMessageIcon = @"\Images\MessageIcons\Info.png";
        notificationTimer.Stop();
    }

    

    public void GenerateStCopy()
    {
        string SelectedOrderID = ThreeShapeObject!.IntOrderID!;
        if (string.IsNullOrEmpty(SelectedOrderID))
            return;

        try
        {
            bool regenerate = false;
            if ((ThreeShapeDirectoryHelper.Length > 1) && CheckFolderIsWritable(ThreeShapeDirectoryHelper + SelectedOrderID)) // if 3Shape dir not set it up or it is setted up but the case folder not writable (maybe doesn't exist)
            {
                if (!File.Exists($@"{ThreeShapeDirectoryHelper}\{SelectedOrderID}\Manufacturers.stCopy"))
                    File.Copy($@"{ThreeShapeDirectoryHelper}\{SelectedOrderID}\Manufacturers.3ml", $@"{ThreeShapeDirectoryHelper}\{SelectedOrderID}\Manufacturers.stCopy");
                else
                {
                    File.Move($@"{ThreeShapeDirectoryHelper}\{SelectedOrderID}\Manufacturers.stCopy", $@"{ThreeShapeDirectoryHelper}\{SelectedOrderID}\Manufacturers.stCopy{DateTime.Now:HHmmss}");
                    File.Copy($@"{ThreeShapeDirectoryHelper}\{SelectedOrderID}\Manufacturers.3ml", $@"{ThreeShapeDirectoryHelper}\{SelectedOrderID}\Manufacturers.stCopy");
                    regenerate = true;
                }

                if (!File.Exists($@"{ThreeShapeDirectoryHelper}\{SelectedOrderID}\{SelectedOrderID}.stCopy"))
                    File.Copy($@"{ThreeShapeDirectoryHelper}\{SelectedOrderID}\{SelectedOrderID}.xml", $@"{ThreeShapeDirectoryHelper}\{SelectedOrderID}\{SelectedOrderID}.stCopy");
                else
                {
                    File.Move($@"{ThreeShapeDirectoryHelper}\{SelectedOrderID}\{SelectedOrderID}.stCopy", $@"{ThreeShapeDirectoryHelper}\{SelectedOrderID}\{SelectedOrderID}.stCopy{DateTime.Now:HHmmss}");
                    File.Copy($@"{ThreeShapeDirectoryHelper}\{SelectedOrderID}\{SelectedOrderID}.xml", $@"{ThreeShapeDirectoryHelper}\{SelectedOrderID}\{SelectedOrderID}.stCopy");
                    regenerate = true;
                }

                if (!File.Exists($@"{ThreeShapeDirectoryHelper}\{SelectedOrderID}\client.info"))
                    File.WriteAllText($@"{ThreeShapeDirectoryHelper}\{SelectedOrderID}\client.info", ThreeShapeObject.Customer);
            }

            if (regenerate)
                ShowNotificationMessage("StCopy", "StCopy successfully regenerated!", NotificationIcon.Info);
            else
                ShowNotificationMessage("StCopy", "StCopy successfully generated!", NotificationIcon.Info);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[{ex.LineNumber()}] {ex.Message}");
            ShowNotificationMessage("Error", ex.Message, NotificationIcon.Error);
        }
    }

    private void ListUpdateTimer_Tick(object? sender, EventArgs e)
    {
        if (ListUpdateable && AllowThreeShapeOrderListUpdates)
        {
            AllowToShowProgressBar = false;
            if (!string.IsNullOrEmpty(ActiveFilterInUse))
                Search(ActiveFilterInUse, true);
            if (!string.IsNullOrEmpty(ActiveSearchString))
                Search(ActiveSearchString);
        }
    }

    private void SearchFieldKeyDown()
    {
        if (SearchString.Length > 0)
        {
            _MainWindow.pb3ShapeProgressBar.Value = 0;
            Current3ShapeOrderList.Clear();
            _MainWindow.listView3ShapeOrders.ItemsSource = Current3ShapeOrderList;
            _MainWindow.listView3ShapeOrders.Items.Refresh();
            GroupList();

            Search(SearchString.Trim());
            ActiveFilterInUse = "";
            ActiveSearchString = SearchString;
            SearchString = "";

        }
    }

    private void ExploreOrderCAM(object o)
    {
        throw new NotImplementedException();
    }

    private void ExploreOrderSource(object o)
    {
        try
        {
            //Process.Start("explorer.exe", DatabaseConnection.GetServerFileDirectory(ServerID) + (string)obj);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[{ex.LineNumber()}] {ex.Message}");
        }
    }

    private void GetInfoOn3ShapeOrder(object obj)
    {
        MessageBox.Show("item:" + (string)obj);

        //OrderBeingWatched = (string)obj;
        //ThreeShapeObject = Current3ShapeOrderList.FirstOrDefault(x => x.IntOrderID == OrderBeingWatched);
        //if (ThreeShapeObject is null)
        //    return;
        ////orderDetailsWindow = new OrderDetailsWindow();
        ////orderDetailsWindow.ShowDialog(this, ThreeShapeObject);
        //OrderBeingWatched = "";
    }

    private void ItemClicked(object obj)
    {
        ThreeShapeOrdersModel model = (ThreeShapeOrdersModel)obj;
        Debug.WriteLine(model.IntOrderID);
        FocusOnSearchField();

        
        OrderBeingWatched = model.IntOrderID!;
        ThreeShapeObject = model;

        if (ThreeShapeObject is null)
            return;

        TurnOnOffToolBarButtons(ThreeShapeObject);

    }

    private async void FocusOnSearchField()
    {
        await Task.Delay(150);
        _MainWindow.tbSearch.Focus();
    }

    private void ItemRightClicked(object obj)
    {
        ThreeShapeOrdersModel model = (ThreeShapeOrdersModel)obj;
        Debug.WriteLine(model.IntOrderID);
        
        OrderBeingWatched = model.IntOrderID!;
        ThreeShapeObject = model;

        if (ThreeShapeObject is null)
            return;

        TurnOnOffToolBarButtons(ThreeShapeObject);
    }


    private void TurnOnOffToolBarButtons(ThreeShapeOrdersModel? threeShapeObject)
    {
        HideAllToolBarButtons();
        if (threeShapeObject is null)
            return;

        string SelectedOrderID = threeShapeObject.IntOrderID!;

        if (!string.IsNullOrEmpty(SelectedOrderID))
        {
            TBBOpenDetails = Visibility.Visible;
            TBBGenerateStCopy = Visibility.Visible;
        }

        #region Rename
        bool isTheFilesAccessible = false;
        bool itIsACopiedCase = false;
        // checking if the folder for that case are exist and writable (in 3Shape orders folder)
        if ((ThreeShapeDirectoryHelper.Length < 1) || !CheckFolderIsWritable(ThreeShapeDirectoryHelper + SelectedOrderID)) // if 3Shape dir not set it up or it is setted up but the case folder not writable (maybe doesn't exist)
        {
            isTheFilesAccessible = false;
            TBBGenerateStCopy = Visibility.Collapsed;
        }
        else
        {

            isTheFilesAccessible = true;

            string XMLFile = ThreeShapeDirectoryHelper + SelectedOrderID + @"\" + SelectedOrderID + ".xml";
            string originalFileName = "";
            try
            {
                string[] lines = File.ReadAllLines(XMLFile);
                foreach (string line in lines.Where(l => l.Contains("OriginalOrderID")))
                    originalFileName = line.Replace("<Property name=\"OriginalOrderID\" value=\"", "").Replace("\"", "").Replace("/>", "").Trim();

                if (originalFileName != "")
                    itIsACopiedCase = true;
                else
                    itIsACopiedCase = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[{ex.LineNumber()}] {ex.Message}");
                itIsACopiedCase = false;
            }
        }


        ThreeShapeOrderInspectionModel inspectedOrder = InspectThreeShapeOrder(SelectedOrderID);

        string caseStatus = inspectedOrder.CaseStatus!;


        if (((caseStatus == "psCreated" && !inspectedOrder.IsCaseWereDesigned) ||
             (caseStatus == "psScanned" && !inspectedOrder.IsCaseWereDesigned) ||
             (caseStatus == "psModelled" && itIsACopiedCase)) &&
             !inspectedOrder.IsLocked && !inspectedOrder.IsCheckedOut && isTheFilesAccessible)
        {
            TBBRename = Visibility.Visible;
        }

        
        if (inspectedOrder.IsCheckedOut)
            TBBGenerateStCopy = Visibility.Collapsed;
        #endregion Rename

    }

    public void HideAllToolBarButtons()
    {
        TBBOpenDetails = Visibility.Collapsed;
        TBBGenerateStCopy = Visibility.Collapsed;
        TBBRename = Visibility.Collapsed;
    }

    private void GetInfoOn3ShapeOrder(string OrderID)
    {
        OrderBeingWatched = OrderID;
        ThreeShapeObject = Current3ShapeOrderList!.FirstOrDefault(x => x.IntOrderID == OrderBeingWatched)!;
        if (ThreeShapeObject is null)
            return;
        //orderDetailsWindow = new OrderDetailsWindow();
        //orderDetailsWindow.ShowDialog(this, ThreeShapeObject);
        OrderBeingWatched = "";
    }


    private void ExpanderCollapsed(object obj)
    {
        var expander = (Expander)obj;
        var dc = (CollectionViewGroup)expander.DataContext;
        var groupName = dc.Name.ToString();
        ExpandStates[groupName!] = expander.IsExpanded;
    }

    private void ExpanderLoaded(object obj)
    {
        var expander = (Expander)obj;
        var dc = (CollectionViewGroup)expander.DataContext;
        var groupName = dc.Name.ToString();
        if (ExpandStates.TryGetValue(groupName!, out var value))
            expander.IsExpanded = value;
    }

    private async void ListCases_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
    {
        _MainWindow.listView3ShapeOrders.ItemsSource = Current3ShapeOrderList;
        _MainWindow.listView3ShapeOrders.Items.Refresh();
        _MainWindow.pb3ShapeProgressBar.Value = 0;
        await Task.Run(() => GroupList());

        // Order count in list
        OrderCountText = Current3ShapeOrderList.Count == 1 ? Current3ShapeOrderList.Count + " order" : Current3ShapeOrderList.Count + " orders";
        OrderCount = Current3ShapeOrderList.Count;
        AllowToShowProgressBar = true;


        if (OrderBeingWatched.Length > 0)
        {
            ThreeShapeObject = Current3ShapeOrderList.FirstOrDefault(x => x.IntOrderID == OrderBeingWatched)!;
            //if (!IsTheSame(orderDetailsWindow.OrderObject, ThreeShapeObject))
            //{
            //    orderDetailsWindow.OrderObject = ThreeShapeObject;
            //    orderDetailsWindow.UpdateForm();
            //}
        }
    }

    public void GroupList()
    {
        Application.Current.Dispatcher.Invoke(new Action(() => {
            _MainWindow.pb3ShapeProgressBar.Value = 0;

            _MainWindow.listView3ShapeOrders.Items.GroupDescriptions.Clear();
            var property = _MainWindow.GroupBy.SelectedItem as string;

            WriteLocalSetting("GroupBy", property!);

            if (property == "None" || property is null)
            {
                if (DataView is not null)
                {
                    DataView.SortDescriptions.Clear();
                    SortDescription sd = new("IntOrderID", ListSortDirection.Ascending);
                    DataView.SortDescriptions.Add(sd);
                    DataView.Refresh();
                }
                return;
            }

            property = property.Replace(" ", "");

            if (property == "LasttouchedBy")
            {
                property = "LastModifiedComputerName";
            }

            if (property == "ScanSource")
            {
                property = "ScanSourceFriendlyName";
            }

            if (DataView is not null)
            {
                PropertyGroupDescription groupDescription = new(property);
                DataView.GroupDescriptions.Add(groupDescription);

                DataView.SortDescriptions.Clear();
                SortDescription sd;
                sd = new SortDescription(property, ListSortDirection.Ascending);
                DataView.SortDescriptions.Add(sd);

                sd = new SortDescription("LastModificationForSorting", ListSortDirection.Descending);
                DataView.SortDescriptions.Add(sd);

                sd = new SortDescription("CreateDateForSorting", ListSortDirection.Descending);
                DataView.SortDescriptions.Add(sd);
                DataView.Refresh();

            }
        }));
    }

    private bool IsTheSame(ThreeShapeOrdersModel FirstObject, ThreeShapeOrdersModel? SecondObject)
    {
        if (FirstObject == null || SecondObject == null)
            return false;

        if (FirstObject.IntOrderID != SecondObject.IntOrderID) return false;
        if (FirstObject.Patient_FirstName != SecondObject.Patient_FirstName) return false;
        if (FirstObject.Patient_LastName != SecondObject.Patient_LastName) return false;
        if (FirstObject.OrderComments != SecondObject.OrderComments) return false;
        if (FirstObject.Items != SecondObject.Items) return false;
        if (FirstObject.OperatorName != SecondObject.OperatorName) return false;
        if (FirstObject.Customer != SecondObject.Customer) return false;
        if (FirstObject.ManufName != SecondObject.ManufName) return false;
        if (FirstObject.CacheMaterialName != SecondObject.CacheMaterialName) return false;
        if (FirstObject.ScanSource != SecondObject.ScanSource) return false;
        if (FirstObject.CacheMaxScanDate != SecondObject.CacheMaxScanDate) return false;
        if (FirstObject.TraySystemType != SecondObject.TraySystemType) return false;
        if (FirstObject.MaxCreateDate != SecondObject.MaxCreateDate) return false;
        if (FirstObject.MaxProcessStatusID != SecondObject.MaxProcessStatusID) return false;
        if (FirstObject.ProcessStatusID != SecondObject.ProcessStatusID) return false;
        if (FirstObject.AltProcessStatusID != SecondObject.AltProcessStatusID) return false;
        if (FirstObject.ProcessLockID != SecondObject.ProcessLockID) return false;
        if (FirstObject.WasSent != SecondObject.WasSent) return false;
        if (FirstObject.ModificationDate != SecondObject.ModificationDate) return false;
        if (FirstObject.ImageSource != SecondObject.ImageSource) return false;
        if (FirstObject.ListViewGroup != SecondObject.ListViewGroup) return false;
        if (FirstObject.PanColor != SecondObject.PanColor) return false;
        if (FirstObject.PanColorName != SecondObject.PanColorName) return false;
        if (FirstObject.CaseStatus != SecondObject.CaseStatus) return false;
        if (FirstObject.PanNumber != SecondObject.PanNumber) return false;
        if (FirstObject.LastModificationForSorting != SecondObject.LastModificationForSorting) return false;
        if (FirstObject.LastModifiedComputerName != SecondObject.LastModifiedComputerName) return false;
        if (FirstObject.CreateDateForSorting != SecondObject.CreateDateForSorting) return false;
        if (FirstObject.ScanSourceFriendlyName != SecondObject.ScanSourceFriendlyName) return false;
        if (FirstObject.CacheMaxScanDateFriendly != SecondObject.CacheMaxScanDateFriendly) return false;
        if (FirstObject.MaxCreateDateFriendly != SecondObject.MaxCreateDateFriendly) return false;
        if (FirstObject.IsCaseWereDesigned != SecondObject.IsCaseWereDesigned) return false;
        if (FirstObject.IsLocked != SecondObject.IsLocked) return false;
        if (FirstObject.IsCheckedOut != SecondObject.IsCheckedOut) return false;

        return true;
    }

    private void ListCases_DoWork(object? sender, DoWorkEventArgs e)
    {
        ThreeShapeServerIsDown = false;
        var data = (SearchData)e.Argument!;
        string keyWordOrFilter = data.KeyWordOrFilter!;
        bool FilterInUse = data.FilterInUse;

        string keyWord = "";
        string Filter = "";


        if (FilterInUse)
            Filter = keyWordOrFilter;
        else
            keyWord = keyWordOrFilter;


        string sFilter = "";
        string sOrderBy;
        string panNumber;

        //sOrderBy = "i.MaxCreateDate DESC, oh.ModificationDate DESC, i.MaxProcessStatusID DESC ";
        sOrderBy = "IntOrderID ASC, oh.ModificationDate DESC ";


        #region >> searching for string / keyword
        if (!FilterInUse)
        {
            sFilter = "WHERE ( ";


            if (keyWord.StartsWith('@'))
            {
                sFilter += $@" (o.Patient_RefNo LIKE '%{keyWord.Replace("@","").Trim()}%' OR
                                o.ExtOrderID LIKE '%{keyWord.Replace("@", "").Trim()}%') ";
            }
            else
            {
                if (SearchOnlyInFileNames)
                {
                    sFilter += "o.IntOrderID LIKE '%" + keyWord + "%' OR " +
                              "o.Patient_RefNo LIKE '%" + keyWord + "%' OR " +
                              "o.ExtOrderID LIKE '%" + keyWord + "%' OR " +
                              "o.Patient_FirstName LIKE '%" + keyWord + "%' OR " +
                              "o.Patient_LastName LIKE '%" + keyWord + "%' ";
                }
                else
                {
                    sFilter += " (o.IntOrderID LIKE '%" + keyWord + "%' OR " +
                            "o.Patient_FirstName LIKE '%" + keyWord + "%' OR " +
                            "o.Patient_LastName LIKE '%" + keyWord + "%' OR " +
                            "o.Patient_RefNo LIKE '%" + keyWord + "%' OR " +
                            "o.ExtOrderID LIKE '%" + keyWord + "%' OR " +
                            "o.OrderComments LIKE '%" + keyWord + "%' OR " +
                            "o.Items LIKE '%" + keyWord + "%' OR " +
                            "o.Customer LIKE '%" + keyWord + "%' OR " +
                            "o.ManufName LIKE '%" + keyWord + "%' OR " +
                            "o.CacheMaterialName LIKE '%" + keyWord + "%' OR " +
                            "i.MaxCreateDate LIKE '%" + keyWord + "%') ";
                }
            }


            sFilter += ")";

            Debug.WriteLine(sFilter);
        }
        #endregion

        #region >> searching by Filter

        if (FilterInUse)
        {

            string sManufacturingFilterWithoutThisSite;
            string sOpenedForDesignFilter = "";





            sManufacturingFilterWithoutThisSite = "AND " +
                                   "  ( " +
                                   "    (o.ManufName NOT LIKE '" + ThisSite + "' AND o.CacheMaterialName NOT LIKE '%Vulcan Zirconia%') " +
                                   "    OR " +
                                   "    ( " +
                                   "      ( " +
                                   "        o.CacheMaterialName LIKE '%Neoss_Ti%' OR " +
                                   "        o.CacheMaterialName LIKE '%,Ti%' OR " +
                                   "        o.CacheMaterialName LIKE '%Ti,%' OR " +
                                   "        o.CacheMaterialName LIKE '%Tita%' OR " +
                                   "        o.CacheMaterialName LIKE '%CoCr%' OR " +
                                   "        o.CacheMaterialName LIKE '%TAN%' OR " +
                                   "        o.CacheMaterialName LIKE '%BellaTek%' OR " +
                                   "        o.CacheMaterialName LIKE 'Ti' " +
                                   "      ) " +
                                   "     AND " +
                                   "      ( " +
                                   "        o.Items LIKE '%Abutment%' " +
                                   "      ) " +
                                   "    ) " +
                                   "    OR " +
                                   "    ( " +
                                   "        o.Items LIKE '%Abutment%' " +
                                   "      AND " +
                                   "        o.CacheMaterialName NOT LIKE '%Bruxzir%' " +
                                   "    ) " +
                                   "    OR " +
                                   "    ( " +
                                   "        o.Items LIKE '%Post and Core%' " +
                                   "    ) " +
                                   "    OR " +
                                   "    ( " +
                                   "        o.CacheMaterialName LIKE '%Argen%' " +
                                   "    ) " +
                                   "    OR " +
                                   "    ( " +
                                   "        o.CacheMaterialName LIKE '%Gold%' " +
                                   "    ) " +
                                   "    OR " +
                                   "    ( " +
                                   "      ( " +
                                   "        o.CacheMaterialName LIKE '%Argen%' OR " +
                                   "        o.CacheMaterialName LIKE '%Wax%' " +
                                   "      ) AND " +
                                   "        (o.OrderComments LIKE '%gold%' OR o.OrderComments LIKE '%Gold%' OR o.OrderComments LIKE '%GOLD%') " +
                                   "    ) " +
                                   "  ) ";



            switch (Filter)
            {
                case "Today":
                    sFilter = "WHERE(i.MaxCreateDate > '" + DtToday + RestDayStart + "' AND i.MaxCreateDate < '" + DtToday + RestDayEnd + "') " +
                              sOpenedForDesignFilter;
                    TempSearchLimitIgnore = true;
                    TodayCasesCount = DatabaseOperations.GetBackTodayCasesCount();
                    break;

                case "Yesterday":
                    sFilter = "WHERE(i.MaxCreateDate > '" + DtYesterday + RestDayStart + "' AND i.MaxCreateDate < '" + DtYesterday + RestDayEnd + "') ";
                    TempSearchLimitIgnore = true;
                    break;

                case "LastTwoDays":
                    sFilter = "WHERE(i.MaxCreateDate > '" + DtYesterday + RestDayStart + "' AND i.MaxCreateDate < '" + DtToday + RestDayEnd + "') " +
                              sOpenedForDesignFilter;
                    TempSearchLimitIgnore = true;
                    break;

                case "ThisWeek":
                    sFilter = "WHERE(i.MaxCreateDate > '" + DtThisMonday + RestDayStart + "' AND i.MaxCreateDate < '" + DtToday + RestDayEnd + "') ";
                    TempSearchLimitIgnore = true;
                    break;

                case "ThisAndLastWeek":
                    sFilter = "WHERE(i.MaxCreateDate > '" + DtLastWeekMonday + RestDayStart + "' AND i.MaxCreateDate < '" + DtToday + RestDayEnd + "') ";
                    TempSearchLimitIgnore = true;
                    break;

                case "LastMonth":
                    sFilter = "WHERE(i.MaxCreateDate > '" + DtOneMonthBack + RestDayStart + "' AND i.MaxCreateDate < '" + DtToday + RestDayEnd + "') ";
                    TempSearchLimitIgnore = true;
                    break;

                case "LastTwoMonths":
                    sFilter = "WHERE(i.MaxCreateDate > '" + DtTwoMonthsBack + RestDayStart + "' AND i.MaxCreateDate < '" + DtToday + RestDayEnd + "') ";
                    TempSearchLimitIgnore = true;
                    break;


                case "Created":
                    sFilter = "WHERE i.MaxProcessStatusID LIKE 'psCreated' ";
                    break;

                case "Scanned":
                    sFilter = "WHERE (i.MaxProcessStatusID LIKE 'psScanned' OR i.MaxProcessStatusID LIKE 'psScanning') ";
                    break;

                case "NotCheckedOut":
                    sFilter = "WHERE (i.MaxProcessStatusID LIKE 'psScanned' AND me.ProcessLockID <> 'plCheckedOut' AND me.ProcessLockID <> 'plLocked') ";
                    break;


                case "ImpressionScans":
                    sFilter = "WHERE (i.MaxProcessStatusID LIKE 'psScanned' OR i.MaxProcessStatusID LIKE 'psScanning') AND (o.TraySystemType NOT LIKE 'stNone') ";
                    break;

                case "ModelScans":
                    sFilter = "WHERE (i.MaxProcessStatusID LIKE 'psScanned' OR i.MaxProcessStatusID LIKE 'psScanning') AND (o.TraySystemType LIKE 'stNone') AND " +
                              "(" +
                              "(o.ScanSource NOT LIKE 'ssImportThirdPartySTL')  AND " +
                              "(o.ScanSource NOT LIKE 'ssImportPLY')  AND " +
                              "(o.ScanSource NOT LIKE 'ssImport')  AND " +
                              "(o.ScanSource NOT LIKE 'ssItero')  AND " +
                              "(o.ScanSource NOT LIKE 'ssTRIOS')  AND " +
                              "(o.ScanSource NOT LIKE 'ssImport3ShapeSTL')" +
                              ") ";
                    break;

                case "DigitalScans":
                    sFilter = "WHERE (i.MaxProcessStatusID LIKE 'psScanned' OR i.MaxProcessStatusID LIKE 'psScanning') AND " +
                              "(" +
                              "(o.ScanSource LIKE 'ssImportThirdPartySTL')  OR " +
                              "(o.ScanSource LIKE 'ssImportPLY')  OR " +
                              "(o.ScanSource LIKE 'ssImport')  OR " +
                              "(o.ScanSource LIKE 'ssItero')  OR " +
                              "(o.ScanSource LIKE 'ssTRIOS')  OR " +
                              "(o.ScanSource LIKE 'ssImport3ShapeSTL')" +
                              ") ";
                    break;



                case "Designed":
                    sFilter = "WHERE i.MaxProcessStatusID LIKE 'psModelled' ";
                    break;

                case "Designing":
                    sFilter = "WHERE i.MaxProcessStatusID LIKE 'psModelling' ";
                    break;

                case "Sent":
                    sFilter = "WHERE (i.MaxProcessStatusID LIKE 'psSent' OR i.MaxProcessStatusID LIKE 'psAccepted' OR i.MaxProcessStatusID LIKE 'psRejected') ";
                    break;

                case "Closed":
                    sFilter = "WHERE i.MaxProcessStatusID LIKE 'psClosed' ";
                    break;

                case "CheckedOut":
                    sFilter = "WHERE me.ProcessLockID LIKE 'plCheckedOut' ";
                    break;





                case "ImpressionAll":
                    sFilter = "WHERE (o.TraySystemType NOT LIKE 'stNone') ";
                    break;

                case "ModelAll":
                    sFilter = "WHERE (o.TraySystemType LIKE 'stNone') AND " +
                              "(" +
                              "(o.ScanSource NOT LIKE 'ssImportThirdPartySTL')  AND " +
                              "(o.ScanSource NOT LIKE 'ssImportPLY')  AND " +
                              "(o.ScanSource NOT LIKE 'ssImport')  AND " +
                              "(o.ScanSource NOT LIKE 'ssItero')  AND " +
                              "(o.ScanSource NOT LIKE 'ssTRIOS')  AND " +
                              "(o.ScanSource NOT LIKE 'ssImport3ShapeSTL')" +
                              ") ";
                    break;

                case "DigitalAll":
                    sFilter = "WHERE " +
                              "(" +
                              "(o.ScanSource LIKE 'ssImportThirdPartySTL')  OR " +
                              "(o.ScanSource LIKE 'ssImportPLY')  OR " +
                              "(o.ScanSource LIKE 'ssImport')  OR " +
                              "(o.ScanSource LIKE 'ssItero')  OR " +
                              "(o.ScanSource LIKE 'ssTRIOS')  OR " +
                              "(o.ScanSource LIKE 'ssImport3ShapeSTL')" +
                              ") ";
                    break;


                case "DigitalModels":
                    sFilter = "WHERE (o.Items LIKE '%model%') ";
                    break;


                case "NeverSentAbutments":
                    sFilter = "WHERE (i.MaxCreateDate > '" + DtLastSevenDays + RestDayStart + "' AND i.MaxCreateDate < '" + DtToday + RestDayEnd + "') AND (i.MaxProcessStatusID NOT LIKE 'psCreated' AND i.MaxProcessStatusID NOT LIKE 'psScanned' AND i.MaxProcessStatusID NOT LIKE 'psScanning' AND i.MaxProcessStatusID NOT LIKE 'psClosed') " + //AND (o.Items LIKE '%Abutment%') " +
                              sManufacturingFilterWithoutThisSite;
                    sOrderBy = "i.MaxCreateDate DESC, o.ManufName DESC ";
                    break;





                case "ManufToday":
                    sFilter = "WHERE (i.MaxCreateDate > '" + DtToday + RestDayStart + "' AND i.MaxCreateDate < '" + DtToday + RestDayEnd + "') " +
                              sManufacturingFilterWithoutThisSite;
                    sOrderBy = "o.ManufName DESC, i.MaxCreateDate DESC ";
                    break;

                case "ManufYesterday":
                    sFilter = "WHERE (i.MaxCreateDate > '" + DtYesterday + RestDayStart + "' AND i.MaxCreateDate < '" + DtYesterday + RestDayEnd + "') " +
                              sManufacturingFilterWithoutThisSite;
                    sOrderBy = "o.ManufName DESC, i.MaxCreateDate DESC ";
                    break;

                case "ManufLastTwoDays":
                    sFilter = "WHERE (i.MaxCreateDate > '" + DtLastTwoDays + RestDayStart + "' AND i.MaxCreateDate < '" + DtYesterday + RestDayEnd + "') " +
                              sManufacturingFilterWithoutThisSite;
                    sOrderBy = "i.MaxCreateDate DESC, o.ManufName DESC ";
                    break;

                case "ManufLastThreeDays":
                    sFilter = "WHERE (i.MaxCreateDate > '" + DtLastThreeDays + RestDayStart + "' AND i.MaxCreateDate < '" + DtYesterday + RestDayEnd + "') " +
                              sManufacturingFilterWithoutThisSite;
                    sOrderBy = "i.MaxCreateDate DESC, o.ManufName DESC ";
                    break;



                case "ManufLastSevenDays":
                    sFilter = "WHERE (i.MaxCreateDate > '" + DtLastSevenDays + RestDayStart + "' AND i.MaxCreateDate < '" + DtYesterday + RestDayEnd + "') " +
                              sManufacturingFilterWithoutThisSite;
                    sOrderBy = "i.MaxCreateDate DESC, o.ManufName DESC ";
                    break;


                case "ManufLast30Days":
                    sFilter = "WHERE (i.MaxCreateDate > '" + DtOneMonthBack + RestDayStart + "' AND i.MaxCreateDate < '" + DtYesterday + RestDayEnd + "') " +
                              sManufacturingFilterWithoutThisSite;
                    sOrderBy = "i.MaxCreateDate DESC, o.ManufName DESC ";
                    break;

            }
        }
        #endregion


        string queryString = "SELECT ";

        if (!TempSearchLimitIgnore && (Filter != "NeverSentAbutments"))
            queryString += " TOP (" + SearchLimit.ToString() + @") ";

        if (EaseUpSearch)
        {
            queryString += "  IntOrderID, " +
                        "  Patient_FirstName, " +
                        "  Patient_LastName, " +
                        "  Patient_RefNo, " +
                        "  o.ExtOrderID, " +
                        "  o.OriginalOrderID, " +
                        "  OrderComments, " +
                        "  o.Items, " +
                        "  OperatorName, " +
                        "  Customer, " +
                        "  o.ManufName, " +
                        "  o.CacheMaterialName, " +
                        "  me.ModelHeight, " +
                        "  ScanSource, " +
                        "  CacheMaxScanDate, " +
                        "  TraySystemType, " +
                        "  MaxCreateDate, " +
                        "  MaxProcessStatusID, " +
                        "  ModificationDate, " +
                        "  UserID ";

            if (Filter != "NeverSentAbutments")
            {
                queryString += ",  ModificationDate, " +
                               "   UserID ";
            }

            queryString += "FROM Orders o " +
                        "FULL OUTER JOIN OrdersInfo i ON i.OrderID = o.IntOrderID " +
                        "FULL OUTER JOIN OrderHistory oh ON oh.OrderID = o.IntOrderID " +

                        sFilter;

            if (Filter != "NeverSentAbutments")
            {
                queryString += "GROUP BY " +
                             "  IntOrderID, " +
                             "  Patient_FirstName, " +
                             "  Patient_LastName, " +
                             "  Patient_RefNo, " +
                             "  o.ExtOrderID, " +
                             "  o.OriginalOrderID, " +
                             "  OrderComments, " +
                             "  o.Items, " +
                             "  OperatorName, " +
                             "  Customer, " +
                             "  o.ManufName, " +
                             "  o.CacheMaterialName, " +
                             "  me.ModelHeight, " +
                             "  ScanSource, " +
                             "  CacheMaxScanDate, " +
                             "  TraySystemType, " +
                             "  MaxCreateDate, " +
                             "  MaxProcessStatusID, " +
                             "  ModificationDate, " +
                             "  UserID ";
            }
        }
        else
        {

            queryString += "  IntOrderID, " +
                        "  Patient_FirstName, " +
                        "  Patient_LastName, " +
                        "  Patient_RefNo, " +
                        "  o.ExtOrderID, " +
                        "  o.OriginalOrderID, " +
                        "  OrderComments, " +
                        "  o.Items, " +
                        "  OperatorName, " +
                        "  Customer, " +
                        "  o.ManufName, " +
                        "  o.CacheMaterialName, " +
                        "  me.ModelHeight, " +
                        "  ScanSource, " +
                        "  CacheMaxScanDate, " +
                        "  TraySystemType, " +
                        "  MaxCreateDate, " +
                        "  MaxProcessStatusID, " +
                        "  ProcessStatusID, " +
                        "  AltProcessStatusID, " +
                        "  ProcessLockID,  " +
                        "  WasSent, " +
                        "  ModificationDate, " +
                        "  UserID ";


            queryString += "FROM Orders o " +
                        "FULL OUTER JOIN OrdersInfo i ON i.OrderID = o.IntOrderID " +
                        "FULL OUTER JOIN ModelJob m ON m.OrderID = o.IntOrderID " +
                        "FULL OUTER JOIN ModelElement me ON me.ModelJobID = m.ModelJobID " +
                        "FULL OUTER JOIN OrderHistory oh ON oh.OrderID = o.IntOrderID " +

                        sFilter;

            if (Filter != "NeverSentAbutments")
            {
                queryString += "GROUP BY " +
                        "  IntOrderID, " +
                        "  Patient_FirstName, " +
                        "  Patient_LastName, " +
                        "  Patient_RefNo, " +
                        "  o.ExtOrderID, " +
                        "  o.OriginalOrderID, " +
                        "  OrderComments, " +
                        "  o.Items, " +
                        "  OperatorName, " +
                        "  Customer, " +
                        "  o.ManufName, " +
                        "  o.CacheMaterialName, " +
                        "  me.ModelHeight, " +
                        "  ScanSource, " +
                        "  CacheMaxScanDate, " +
                        "  TraySystemType, " +
                        "  MaxCreateDate, " +
                        "  MaxProcessStatusID, " +
                        "  ProcessStatusID, " +
                        "  AltProcessStatusID, " +
                        "  ProcessLockID,  " +
                        "  WasSent, " +
                        "  ModificationDate, " +
                        "  UserID ";
            }

        }


        queryString += "ORDER BY " + sOrderBy;

        //if (!TempSearchLimitIgnore)
        //    queryString += " OFFSET 0 ROWS FETCH FIRST " + SearchLimit.ToString() + @" ROWS ONLY;";


        string countingQueryString = "select count(*) " +
                                    "from " +
                                    "( " +
                                    "select count(IntOrderID) tot " +
                                    "FROM Orders o " +
                                    "FULL OUTER JOIN OrdersInfo i ON i.OrderID = o.IntOrderID " +
                                    "FULL OUTER JOIN ModelJob m ON m.OrderID = o.IntOrderID " +
                                    "FULL OUTER JOIN ModelElement me ON me.ModelJobID = m.ModelJobID " +
                                    "FULL OUTER JOIN OrderHistory oh ON oh.OrderID = o.IntOrderID " +
                                    sFilter +
                                    "  group by IntOrderID " +
                                    ")  src;";
        int countedResults = DatabaseOperations.Counting_result(countingQueryString);

        if (!TempSearchLimitIgnore && countedResults > SearchLimit)
            countedResults = SearchLimit;

        Application.Current.Dispatcher.Invoke(new Action(() => {
            if (countedResults < 1)
            {
                countedResults = 1;
                HideAllToolBarButtons();
            }
            _MainWindow.pb3ShapeProgressBar.Maximum = countedResults;
        }));



        string connectionString = DatabaseConnection.ConnectionStrFor3Shape();

        List<string> list = [];


        Application.Current.Dispatcher.Invoke(new Action(() => {
            FilterString = keyWordOrFilter.Trim();
            //if (FilterInUse)
            //    tbFilterString.Foreground = Brushes.DarkGreen;
            //else
            //    tbFilterString.Foreground = Brushes.SteelBlue;

            _MainWindow.pb3ShapeProgressBar.Value = 0;
            if (AllowToShowProgressBar)
                _MainWindow.pb3ShapeProgressBar.Visibility = Visibility.Visible;
        }));

        TempSearchLimitIgnore = false;

        Current3ShapeOrderList.Clear();

        try
        {
            
            using SqlConnection connection = new(connectionString);
            SqlCommand command = new(queryString, connection);
            connection.Open();

            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                if (Filter == "NeverSentAbutments" && reader["ProcessLockID"].ToString() == "plSent")
                {
                    if (!list.Contains(reader["IntOrderID"].ToString()!))
                    {
                        list.Add(reader["IntOrderID"].ToString()!);
                    }

                    if (Current3ShapeOrderList.Where(x => x.IntOrderID == reader["IntOrderID"].ToString()).FirstOrDefault() != null)
                    {
                        if (Current3ShapeOrderList.Where(x => x.IntOrderID == reader["IntOrderID"].ToString()).FirstOrDefault()!.IntOrderID == reader["IntOrderID"].ToString())
                            Current3ShapeOrderList.RemoveAt(Current3ShapeOrderList.IndexOf(Current3ShapeOrderList.Where(x => x.IntOrderID == reader["IntOrderID"].ToString()).FirstOrDefault()!));
                    }
                    continue;
                }

                if (!list.Contains(reader["IntOrderID"].ToString()!))
                {
                    list.Add(reader["IntOrderID"].ToString()!);

                    string AlternateColoring = "";

                    string MaxProcessStatusID = reader["MaxProcessStatusID"].ToString()!;
                    string ScanSource = reader["ScanSource"].ToString()!;
                    string ProcessStatusID = "";
                    string ProcessLockID = "";
                    string AltProcessStatusID = "";
                    string WasSent = "";

                    if (!EaseUpSearch)
                    {
                        ProcessLockID = reader["ProcessLockID"].ToString()!;
                        ProcessStatusID = reader["ProcessStatusID"].ToString()!;
                        AltProcessStatusID = reader["AltProcessStatusID"].ToString()!;
                        WasSent = reader["WasSent"].ToString()!;
                    }




                    #region >> Determining the pan number

                    string orderIDHelpr = reader["IntOrderID"].ToString()!;
                    List<string> orderIDDarabolt = [];
                    orderIDDarabolt = [.. orderIDHelpr.Split('-')];

                    bool foundPanNumber = int.TryParse(orderIDDarabolt[0].ToString(), out int panNr);

                    if (foundPanNumber)
                    {
                        panNumber = panNr.ToString();
                    }
                    else
                    {
                        // checking if we can find any pan number in the patient name section
                        string orderIDHelprFromPtName = reader["Patient_LastName"].ToString()!;
                        List<string> orderIDHelprFromPtNameDarabolt = [];
                        orderIDHelprFromPtNameDarabolt = [.. orderIDHelprFromPtName.Split('-')];
                        bool foundPanNumber2 = int.TryParse(orderIDHelprFromPtNameDarabolt[0].ToString(), out int panNr2);
                        if (foundPanNumber2)
                        {
                            panNumber = panNr2.ToString();
                        }
                        else
                        {
                            orderIDHelprFromPtName = reader["Patient_FirstName"].ToString()!;
                            orderIDHelprFromPtNameDarabolt = [];
                            orderIDHelprFromPtNameDarabolt = [.. orderIDHelprFromPtName.Split('-')];
                            panNr2 = 0;
                            foundPanNumber2 = int.TryParse(orderIDHelprFromPtNameDarabolt[0].ToString(), out panNr2);

                            if (foundPanNumber2)
                            {
                                panNumber = panNr2.ToString();
                            }
                            else
                                panNumber = "";
                        }

                    }
                    #endregion

                    bool isAbutmentCase = false;







                    string CaseStatus = CaseStatusSelect(MaxProcessStatusID, ScanSource, ProcessLockID);
                    string ImageSource = @"\Images\ListViewIcons\" + IconSelect(MaxProcessStatusID, ScanSource, ProcessLockID) + ".png";
                    string PanColorName = GetBackPanColorName(panNumber);
                    string PanColor = GetBackPanColorHEX(panNumber);

                    if (panNumber == "" || PanColor == "#FFFFFF")
                        PanColor = "Transparent";

                    string Patient_FirstName = reader["Patient_FirstName"].ToString()!.Trim();
                    string Patient_LastName = reader["Patient_LastName"].ToString()!.Trim();
                    if (Patient_FirstName == "")
                        Patient_FirstName = "-";

                    string manufName = "";
                    if (ThisSite.Length > 0)
                    {
                        manufName = reader["ManufName"].ToString()!
                                            .Replace(ThisSite + "/", "")
                                            .Replace("/" + ThisSite, "")
                                            .Replace(ThisSite, "");
                    }
                    else
                    {
                        manufName = reader["ManufName"].ToString()!;
                    }


                    _ = DateTime.TryParse(reader["ModificationDate"].ToString(), out DateTime LastModificationForSortingDateTime);
                    string LastModificationForSorting = "";
                    if (reader["ModificationDate"].ToString() != "")
                        LastModificationForSorting = LastModificationForSortingDateTime.ToString("yyyy-MM-dd-HHmmss");

                    _ = DateTime.TryParse(reader["MaxCreateDate"].ToString(), out DateTime CreateDateForSortingDateTime);
                    string CreateDateForSorting = "";
                    CreateDateForSorting = CreateDateForSortingDateTime.ToString("yyyy-MM-dd-HHmmss");

                    string ScanSourceFriendlyName = GetScanner(ScanSource);




                    string CacheMaxScanDate = reader["CacheMaxScanDate"].ToString()!;
                    string CacheMaxScanDateFriendly = CacheMaxScanDate;
                    if (IsItToday(CacheMaxScanDate))
                    {
                        _ = DateTime.TryParse(CacheMaxScanDate, out DateTime CacheMaxScanDateDT);
                        CacheMaxScanDateFriendly = CacheMaxScanDateDT.ToString("h:mm tt");
                    }
                    else if (IsItThisYear(CacheMaxScanDate))
                    {
                        _ = DateTime.TryParse(CacheMaxScanDate, out DateTime CacheMaxScanDateDT);
                        CacheMaxScanDateFriendly = CacheMaxScanDateDT.ToString("MM/dd - h:mm tt");
                    }

                    if (CacheMaxScanDateFriendly.StartsWith("000"))
                        CacheMaxScanDateFriendly = CacheMaxScanDate;




                    string MaxCreateDate = reader["MaxCreateDate"].ToString()!;
                    string MaxCreateDateFriendly = MaxCreateDate;
                    if (IsItToday(MaxCreateDate))
                    {
                        _ = DateTime.TryParse(MaxCreateDate, out DateTime MaxCreateDateDT);
                        MaxCreateDateFriendly = MaxCreateDateDT.ToString("h:mm tt");
                    }
                    else if (IsItThisYear(MaxCreateDate))
                    {
                        _ = DateTime.TryParse(MaxCreateDate, out DateTime MaxCreateDateDT);
                        MaxCreateDateFriendly = MaxCreateDateDT.ToString("MM/dd - h:mm tt");
                    }







                    string ModificationDate = reader["ModificationDate"].ToString()!;
                    _ = DateTime.TryParse(ModificationDate, out DateTime ModificationDateDT);
                    _ = DateTime.TryParse(DtThisMonday, out DateTime dtLastWeekSundayDT);
                    dtLastWeekSundayDT = dtLastWeekSundayDT.AddDays(-1);

                    if (IsItToday(ModificationDate))
                    {
                        ModificationDate = ModificationDateDT.ToString("h:mm tt");
                    }
                    else if (ModificationDateDT > dtLastWeekSundayDT)
                    {
                        ModificationDate = ModificationDateDT.ToString("dddd - h:mm tt");
                    }
                    else if (IsItThisYear(ModificationDate))
                    {
                        ModificationDate = ModificationDateDT.ToString("MM/dd - h:mm tt");
                    }

                    string CacheMaterialName = reader["CacheMaterialName"].ToString()!.Replace("\"", "");

                    string LastModifiedComputerName = ReadComputerName(reader["UserID"].ToString()!);

                    string Items = RemoveChineseCharacters(reader["Items"].ToString()!);

                    string[] CaseStatusByManufacturerParts = manufName.Split('/');
                    string CaseStatusByManufacturer = CaseStatusByManufacturerParts[0];
                    if (CaseStatusByManufacturer == "")
                    {
                        if (Items.Contains("Abutment") && CacheMaterialName.Contains("Ti"))
                            CaseStatusByManufacturer = "Abutments (3rd Party)";
                        else
                            CaseStatusByManufacturer = "Miscellaneous";
                    }
                    if (Filter == "NeverSentAbutments")
                        CaseStatus = CaseStatusByManufacturer;


                    if (Items.Contains("Abutment") &&
                                        !IsItEncodeUnit(reader["ManufName"].ToString()!, reader["CacheMaterialName"].ToString()!) &&
                                                MaxProcessStatusID == "psModelled" && ProcessLockID != "plLocked")
                        isAbutmentCase = true;



                    // alternate coloring
                    if (PanColor == "#FFFFFF")
                        AlternateColoring = "nopancolor";

                    if (CacheMaterialName.Contains("NO MATERIAL"))
                    {
                        AlternateColoring = "encode";
                        isAbutmentCase = false;
                    }



                    string ExtOrderID = reader["ExtOrderID"].ToString()!;
                    var isNumeric = int.TryParse(ExtOrderID, out _);
                    if (isNumeric)
                        ExtOrderID = "";


                    #region Context MenuItems Visibility

                    Visibility ToolBarButton_exploreOrderCAM = Visibility.Collapsed;
                    Visibility ToolBarButton_secureAbutmentDesign = Visibility.Collapsed;
                    Visibility ToolBarButton_removeSecureAbutmentDesign = Visibility.Collapsed;
                    Visibility ToolBarButton_renameOrder = Visibility.Collapsed;


                    if (MaxProcessStatusID == "psModelled")
                        ToolBarButton_exploreOrderCAM = Visibility.Visible;

                    if (isAbutmentCase)
                        ToolBarButton_secureAbutmentDesign = Visibility.Visible;

                    if (isAbutmentCase)
                        ToolBarButton_removeSecureAbutmentDesign = Visibility.Visible;

                    bool isTheFilesAccessible = true;               
                    bool generateStCopy = true;
                    bool IsLocked = false;
                    bool IsCheckedOut = false;
                    bool IsCaseWereDesigned = false;

                    if (reader["ProcessLockID"].ToString() == "plLocked")
                        IsLocked = true;

                    // checking if case is checked out
                    if (reader["ProcessLockID"].ToString() == "plCheckedOut")
                        IsCheckedOut = true;

                    // checking if the folder for that case are exist and writable (in 3Shape orders folder)
                    if (ThreeShapeDirectoryHelper.Length < 1 || 
                        !CheckFolderIsWritable(ThreeShapeDirectoryHelper + reader["IntOrderID"].ToString()) ||// if 3Shape dir not set it up or it is setted up but the case folder not writable (maybe doesn't exist)
                        IsCheckedOut
                        ) 
                    {
                        isTheFilesAccessible = false;
                        generateStCopy = false;
                    }

                    



                    if (reader["ModelHeight"].ToString() != "0" && string.IsNullOrEmpty(reader["OriginalOrderID"].ToString()))
                        IsCaseWereDesigned = true;
                    
                    bool canBeRenamed = false;
                    if (((MaxProcessStatusID == "psCreated" && !IsCaseWereDesigned) ||
                         (MaxProcessStatusID == "psScanned" && !IsCaseWereDesigned) ||
                         (MaxProcessStatusID == "psModelled" && !string.IsNullOrEmpty(reader["OriginalOrderID"].ToString()))) &&
                         !IsLocked && !IsCheckedOut && isTheFilesAccessible)
                    {
                        canBeRenamed = true;
                    }
                    #endregion


#pragma warning disable CS8604 // Possible null reference argument.
                    Current3ShapeOrderList.Add(new ThreeShapeOrdersModel
                    {
                        IntOrderID = reader["IntOrderID"].ToString(),
                        Patient_FirstName = Patient_FirstName,
                        Patient_LastName = Patient_LastName,
                        Patient_RefNo = reader["Patient_RefNo"].ToString(),
                        ExtOrderID = ExtOrderID,
                        OrderComments = reader["OrderComments"].ToString(),
                        Items = Items,
                        OperatorName = reader["OperatorName"].ToString(),
                        Customer = reader["Customer"].ToString(),
                        ManufName = manufName,
                        CacheMaterialName = CacheMaterialName,
                        ScanSource = ScanSource,
                        CacheMaxScanDate = CacheMaxScanDate,
                        TraySystemType = reader["TraySystemType"].ToString(),
                        MaxCreateDate = MaxCreateDate,
                        MaxProcessStatusID = MaxProcessStatusID,
                        ProcessStatusID = ProcessStatusID,
                        AltProcessStatusID = AltProcessStatusID,
                        ProcessLockID = ProcessLockID,
                        WasSent = WasSent,
                        ModificationDate = ModificationDate,
                        ImageSource = ImageSource,
                        ListViewGroup = "",
                        PanColor = PanColor,
                        PanColorName = PanColorName,
                        CaseStatus = CaseStatus,
                        PanNumber = panNumber,
                        LastModificationForSorting = LastModificationForSorting,
                        LastModifiedComputerName = LastModifiedComputerName,
                        CreateDateForSorting = CreateDateForSorting,
                        ScanSourceFriendlyName = ScanSourceFriendlyName,
                        CacheMaxScanDateFriendly = CacheMaxScanDateFriendly,
                        MaxCreateDateFriendly = MaxCreateDateFriendly,
                        CaseStatusByManufacturer = CaseStatusByManufacturer,
                        AlternateColoring = AlternateColoring,
                        OriginalOrderID = reader["OriginalOrderID"].ToString(),

                        IsCaseWereDesigned = IsCaseWereDesigned,
                        IsLocked = IsLocked,
                        IsCheckedOut = IsCheckedOut,
                        CanBeRenamed = canBeRenamed,
                        CanGenerateStCopy = generateStCopy,
                    });
#pragma warning restore CS8604 // Possible null reference argument.

                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        OrderCount = Current3ShapeOrderList.Count;
                        OrderCountText = Current3ShapeOrderList.Count.ToString() + " orders";
                        if (AllowToShowProgressBar)
                            _MainWindow.pb3ShapeProgressBar.Value += 1;
                        else
                            _MainWindow.pb3ShapeProgressBar.Value = 0;
                    }));
                }
            }

        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[{ex.LineNumber()}] {ex.Message}");
            Application.Current.Dispatcher.Invoke(new Action(() => {
                if (ex.Message.Contains("A network-related or instance-specific error", StringComparison.CurrentCultureIgnoreCase))
                    ThreeShapeServerIsDown = true;
                else
                    ShowMessage(MainWindow.Instance, "Exception", "Exception occured", ex.Message, Ookii.Dialogs.Wpf.TaskDialogIcon.Error, Buttons.Ok);
            }));
        }

        Application.Current.Dispatcher.Invoke(new Action(() => {
            _MainWindow.pb3ShapeProgressBar.Value = 0;
        }));
    }

    private void FilterMenuItemClicked(object obj)
    {
        _MainWindow.pb3ShapeProgressBar.Value = 0;
        Current3ShapeOrderList.Clear();
        _MainWindow.listView3ShapeOrders.ItemsSource = Current3ShapeOrderList;
        _MainWindow.listView3ShapeOrders.Items.Refresh();
        //GroupList();
        string filter = (string)obj;
        WriteLocalSetting("FilterUsed", filter);
        Search(filter, true);
    }

    private void Search(string keyWord)
    {
        WriteLocalSetting("FilterUsed", "");

        ListUpdateable = true;
        ActiveFilterInUse = "";
        ActiveSearchString = keyWord;
        BuildingUpDates();
        if (bwListCases.IsBusy != true)
        {
            bwListCases.RunWorkerAsync(new SearchData
            {
                FilterInUse = false,
                KeyWordOrFilter = keyWord
            });
        }
        else
            bwListCases.CancelAsync();
    }

    public void Search(string Filter, bool SearchWithFilter)
    {
        _MainWindow.pb3ShapeProgressBar.Value = 0;
        ListUpdateable = true;
        ActiveFilterInUse = Filter;
        ActiveSearchString = "";
        BuildingUpDates();
        if (bwListCases.IsBusy != true)
        {
            bwListCases.RunWorkerAsync(new SearchData
            {
                FilterInUse = true,
                KeyWordOrFilter = Filter
            });
        }
        else
        {
            bwListCases.CancelAsync();
            _MainWindow.pb3ShapeProgressBar.Value = 0;
        }
    }

    private void StartProgramUpdate()
    {
        var Processes = Process.GetProcesses()
                           .Where(pr => pr.ProcessName == "StatsClientUpdater");
        foreach (var process in Processes)
        {
            process.Kill();
        }

        Task.Run(DownloadUpdater).Wait();

        StartUpdaterApp();
    }

    public void BuildingUpDates()
    {
        string TodayName = DateTime.Now.ToString("dddd");
        int lstFd, thisMd;
        lstFd = 0;
        thisMd = 0;

        RestDayStart = " 0:01:00.000";
        RestDayEnd = " 23:59:59.999";

        switch (TodayName)
        {
            case "Monday": lstFd = 3; thisMd = 0; break;
            case "Tuesday": lstFd = 4; thisMd = 1; break;
            case "Wednesday": lstFd = 5; thisMd = 2; break;
            case "Thursday": lstFd = 6; thisMd = 3; break;
            case "Friday": lstFd = 7; thisMd = 4; break;
            case "Saturday": lstFd = 1; thisMd = 5; break;
            case "Sunday": lstFd = 2; thisMd = 6; break;
        }

        DtLastTwoDayNames = DateTime.Now.AddDays(-2).ToString("dddd") + " and " + DateTime.Now.AddDays(-1).ToString("dddd");
        DtLastThreeDayNames = DateTime.Now.AddDays(-3).ToString("dddd") + ", " + DateTime.Now.AddDays(-2).ToString("dddd") + " and " + DateTime.Now.AddDays(-1).ToString("dddd");

        DtToday = DateTime.Now.ToString("yyyy-MM-dd");

        DtYesterday = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
        DtLastFriday = DateTime.Now.AddDays(-lstFd).ToString("yyyy-MM-dd");
        DtThisMonday = DateTime.Now.AddDays(-thisMd).ToString("yyyy-MM-dd");
        DtLastWeekFriday = DateTime.Now.AddDays(-(lstFd + 7)).ToString("yyyy-MM-dd");
        DtLastWeekMonday = DateTime.Now.AddDays(-(lstFd + 11)).ToString("yyyy-MM-dd");
        DtLastWeekSunday = DateTime.Now.AddDays(-(lstFd + 5)).ToString("yyyy-MM-dd");
        DtOneMonthBack = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
        DtTwoMonthsBack = DateTime.Now.AddDays(-60).ToString("yyyy-MM-dd");
        DtLastTwoDays = DateTime.Now.AddDays(-2).ToString("yyyy-MM-dd");
        DtLastThreeDays = DateTime.Now.AddDays(-3).ToString("yyyy-MM-dd");
        DtLastSevenDays = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
    }

    public static bool CheckFolderIsWritable(string folder)
    {
        string lastCharacter = folder[^1..];

        try
        {
            if (lastCharacter != "\\")
            {
                File.WriteAllText(folder + "\\write.test", "Write test");
                File.Delete(folder + "\\write.test");
            }
            else
            {
                File.WriteAllText(folder + "write.test", "Write test");
                File.Delete(folder + "write.test");
            }
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[{ex.LineNumber()}] {ex.Message}");
            return false;
        }
    }

    public void ShowNotificationMessage(string title, string message, NotificationIcon notificationIcon = NotificationIcon.Info, bool notificationWindowPulledIn = false, bool notificationWindowOnInfoBar = false)
    {

        if (notificationWindowOnInfoBar)
        {
            NotificationMessageGridPosition = "2";
            NotificationMessagePosition = new Thickness(1, 10, 0, 20);
            NotificationMessageVertAlignment = VerticalAlignment.Top;
        }
        else
        {
            NotificationMessageVertAlignment = VerticalAlignment.Bottom;
            if (notificationWindowPulledIn)
                NotificationMessagePosition = new Thickness(145, 0, 0, 20);
            else
                NotificationMessagePosition = new Thickness(15, 0, 0, 20);
            NotificationMessageGridPosition = "1";
        }

        NotificationMessageTitle = title;
        NotificationMessageBody = message;
        IsNotificationMessageShows = Visibility.Visible;
        NotificationMessageIcon = $@"\Images\MessageIcons\{notificationIcon}.png";
        notificationTimer.Start();
    }

    public enum NotificationIcon
    {
        Info,
        Warning,
        Error,
        Success
    }


    private void ResetDigiSystemColors()
    {
        DigiSystemColors.Clear();
        DigiSystemColors.Add((new BrushConverter().ConvertFromString("#FFfcf0cc") as SolidColorBrush)!, true);
        DigiSystemColors.Add((new BrushConverter().ConvertFromString("#FFfcccfc") as SolidColorBrush)!, true);
        DigiSystemColors.Add((new BrushConverter().ConvertFromString("#FFfcd1cc") as SolidColorBrush)!, true);
        DigiSystemColors.Add((new BrushConverter().ConvertFromString("#FFcce1fc") as SolidColorBrush)!, true);
        DigiSystemColors.Add((new BrushConverter().ConvertFromString("#FFeffccc") as SolidColorBrush)!, true);
        DigiSystemColors.Add((new BrushConverter().ConvertFromString("#FFcefccc") as SolidColorBrush)!, true);
        DigiSystemColors.Add((new BrushConverter().ConvertFromString("#FFcccefc") as SolidColorBrush)!, true);
        DigiSystemColors.Add((new BrushConverter().ConvertFromString("#FFfce2cc") as SolidColorBrush)!, true);
        DigiSystemColors.Add((new BrushConverter().ConvertFromString("#FFccfced") as SolidColorBrush)!, true);
        DigiSystemColors.Add((new BrushConverter().ConvertFromString("#FFccf6fc") as SolidColorBrush)!, true);
        DigiSystemColors.Add((new BrushConverter().ConvertFromString("#FFe9ccfc") as SolidColorBrush)!, true);
        DigiSystemColors.Add(Brushes.WhiteSmoke, true);
        DigiSystemColors.Add(Brushes.LightYellow, true);
        DigiSystemColors.Add(Brushes.LightPink, true);
        DigiSystemColors.Add(Brushes.Beige, true);
        DigiSystemColors.Add(Brushes.LightCoral, true);
        DigiSystemColors.Add(Brushes.Yellow, true);
        DigiSystemColors.Add(Brushes.LightSeaGreen, true);
        DigiSystemColors.Add(Brushes.CornflowerBlue, true);
        DigiSystemColors.Add(Brushes.DeepPink, true);
        DigiSystemColors.Add(Brushes.Violet, true);
    }

    private void FillUpDigiCasePanel()
    {
        if (CbSettingShowDigiCases)
        {
            double fontSize = 10;
            double fontSizeCopy = 14;
            
            PendingDigiNumbersWaitingToProcessInt = GetAllNotProcessedNumbers().Count;
            

            Dictionary<string, int> catchedEmails = GetEWCategoriesAndCounts();
            Dictionary<string, int> meditCases = GetMeditCasesWithCounts();

            ResetDigiSystemColors();

            MainWindow.Instance.panelDigiCases.Children.Clear();
            MainWindow.Instance.panelNewlyArrivedDigitalCasesList.Children.Clear();

            int countedCases = 0;

            if (PendingDigiNumbersWaitingToProcessInt > 0 && CbSettingIncludePendingDigiCasesInNewlyArrived)
            {
                string pendingDigiName = PendingDigiCasesReplacementName.Trim();
                if (pendingDigiName == "")
                    pendingDigiName = "PendingDigi";

                TextBlock textBlock = new()
                {
                    Text = $"{pendingDigiName} ⇢ Got {PendingDigiNumbersWaitingToProcessInt} new case (In-house)",
                    FontSize = fontSize,
                    Foreground = new BrushConverter().ConvertFromString("#FF8ce4ff") as SolidColorBrush,
                    FontWeight = FontWeights.SemiBold,
                };


                MainWindow.Instance.panelDigiCases.Children.Add(textBlock);
            }
            
            if (NewTriosCaseInInboxCount > 0)
            {
                TextBlock textBlock = new()
                {
                    Text = $"Trios ⇢ Got {NewTriosCaseInInboxCount} new case",
                    FontSize = fontSize,
                    Foreground = Brushes.LightGreen,
                    FontWeight = FontWeights.SemiBold,
                };
                
                TextBlock textBlockCopy = new()
                {
                    Text = $"Trios ⇢ Got {NewTriosCaseInInboxCount} new case",
                    FontSize = fontSizeCopy,
                    Foreground = Brushes.LightGreen,
                    FontWeight = FontWeights.SemiBold,
                };

                MainWindow.Instance.panelDigiCases.Children.Add(textBlock);

                MainWindow.Instance.panelNewlyArrivedDigitalCasesList.Children.Add(textBlockCopy);
            }


            foreach (var item in catchedEmails)
            {
                Brush textColor = Brushes.LightGreen;
                foreach (var color in DigiSystemColors)
                {
                    if (color.Value == true)
                    {
                        textColor = color.Key;
                        break;
                    }
                }

                if (textColor != null)
                    DigiSystemColors[textColor] = false;

                string digiSystem = "";
                string labIdentifierAllScan = "";
                if (item.Key.Contains('-'))
                {
                    string[] identifierParts = item.Key.Split('-');
                    digiSystem = identifierParts[0].Trim();
                    labIdentifierAllScan = identifierParts[1].Trim();
                }

                TextBlock textBlock = new()
                {
                    Text = $"{digiSystem} ⇢ Got {item.Value} new case ({labIdentifierAllScan})",
                    FontSize = fontSize,
                    Foreground = textColor!,
                    FontWeight = FontWeights.SemiBold,
                };
                
                TextBlock textBlockCopy = new()
                {
                    Text = $"{digiSystem} ⇢ Got {item.Value} new case ({labIdentifierAllScan})",
                    FontSize = fontSizeCopy,
                    Foreground = textColor!,
                    FontWeight = FontWeights.SemiBold,
                };

                countedCases += item.Value;
                MainWindow.Instance.panelDigiCases.Children.Add(textBlock);
                
                MainWindow.Instance.panelNewlyArrivedDigitalCasesList.Children.Add(textBlockCopy);
            }


            foreach (var item in meditCases)
            {
                Brush textColor = Brushes.LightSteelBlue;
                foreach (var color in DigiSystemColors)
                {
                    if (color.Value == true)
                    {
                        textColor = color.Key;
                        break;
                    }
                }

                if (textColor != null)
                    DigiSystemColors[textColor] = false;

                string labIdentifier = GetSingleEWIdentifier(item.Key);

                TextBlock textBlock = new()
                {
                    Text = $"Medit ⇢ Got {item.Value} new case ({labIdentifier})",
                    FontSize = fontSize,
                    Foreground = textColor!,
                    FontWeight = FontWeights.SemiBold,
                };
                
                TextBlock textBlockCopy = new()
                {
                    Text = $"Medit ⇢ Got {item.Value} new case ({labIdentifier})",
                    FontSize = fontSizeCopy,
                    Foreground = textColor!,
                    FontWeight = FontWeights.SemiBold,
                };

                countedCases += item.Value;
                MainWindow.Instance.panelDigiCases.Children.Add(textBlock);

                MainWindow.Instance.panelNewlyArrivedDigitalCasesList.Children.Add(textBlockCopy);
            }

            if (CbSettingIncludePendingDigiCasesInNewlyArrived)
                NewDigiCaseArrivedCount = NewTriosCaseInInboxCount + PendingDigiNumbersWaitingToProcessInt + countedCases;
            else
                NewDigiCaseArrivedCount = NewTriosCaseInInboxCount + countedCases;

            TotalNewDigiCaseWithoutInHouseCases = NewTriosCaseInInboxCount + countedCases;
        }
    }



    private void BwBackgroundTasks_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
    {

    }

    private void BwBackgroundTasks_DoWork(object? sender, DoWorkEventArgs e)
    {
        string arg = (string)e.Argument!;
        string[] argParts = arg.Split('|');
        _ = int.TryParse(argParts[0], out int minute);
        _ = int.TryParse(argParts[1], out int second);

        Application.Current.Dispatcher.Invoke(new Action(() => {
            ServerStatus = GetStatsServerStatus();
            ServerIsWritingDatabase = CheckIfServerIsWritingDatabase();
            if (CbSettingModuleFolderSubscription)
                FsLastDatabaseUpdate = GetLastDatabaseUpdate();
            
            if (ShowBottomInfoBar)
                LastDCASUpdate = GetLastDCASUpdate();
        }));


        if (second % 15 == 1 || FirstRun)
        //if (FirstRun)
        {
            FirstRun = false;
            Application.Current.Dispatcher.Invoke(new Action(() => {
                int casesDesigningNow = GetOpenedForDesignCasesCount(ServerID);
                if (casesDesigningNow > 0)
                {
                    DesignerOpenToolTip = casesDesigningNow.ToString();
                    
                    if (MainWindow.Instance.panelDesignerOpen.Children.Count > casesDesigningNow ||
                        MainWindow.Instance.panelDesignerOpen.Children.Count < casesDesigningNow)
                    {
                        MainWindow.Instance.panelDesignerOpen.Children.Clear();

                        for (int i = 0; i < casesDesigningNow; i++)
                        {
                            //Effect bitmapEffect = new DropShadowEffect
                            //{
                            //    ShadowDepth = 1,
                            //    Opacity = 0.55,
                            //    Color = Colors.Orange,
                            //    Direction = 270
                            //};

                            Image image = new()
                            {
                                Source = new BitmapImage(new Uri("pack://application:,,,/Images/ListViewIcons/psModelling.png")),
                                Width = 16,
                                Height = 16,
                                ToolTip = casesDesigningNow.ToString() + " case is open for design",
                                Margin = new Thickness(0, 3, 1, 0),
                                //Effect = bitmapEffect,
                            };
                            MainWindow.Instance.panelDesignerOpen.Children.Add(image);
                        }
                    }

                    DesignerOpen = true;
                }
                else
                {
                    MainWindow.Instance.panelDesignerOpen.Children.Clear();
                    DesignerOpen = false;
                }

                FsCountedEntries = GetBackFolderSubscriptionCountedEntries();

                _ = int.TryParse(GetSentOutIssuesCount(), out int sentOutIssues);
                SentOutIssuesCount = sentOutIssues;


                if (CbSettingShowDigiPrescriptionsCount)
                    DigiPrescriptionsTodayCount = GetCurrentDigiPrescriptionCount();
                
                if (CbSettingShowDigiCasesIn3ShapeTodayCount)
                    DigiCasesIn3ShapeTodayCount = GetDigiCasesIn3ShapeTodayCount();

                FillUpDigiCasePanel();

                
                FillUpPendingDigiCaseNumberList();

                _ = bool.TryParse(ReadStatsSetting("dcas_EmailWatcherActive"), out bool isDCASIsActive);
                IsDCASIsActive = isDCASIsActive;
            }));
        }

        if (second % 59 == 1)
        {

            Application.Current.Dispatcher.Invoke(new Action(() => {
                if (bwGetSentOutIssues.IsBusy != true)
                {
                    bwGetSentOutIssues.RunWorkerAsync();
                }
            }));

            GC.Collect();
        }
    }

    private void BwGetSentOutIssues_DoWork(object? sender, DoWorkEventArgs e)
    {
        try
        {
            string connectionString = DatabaseConnection.ConnectionStrToStatsDatabase();
            string query = @"SELECT * FROM dbo.SentOutIssues";

            using SqlConnection connection = new(connectionString);
            SqlCommand command = new(query, connection);
            connection.Open();

            IssuesWithCasesList.Clear();

            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {

                string IconSource = "";
                switch (reader["Level"].ToString())
                {
                    case "0": IconSource = @"\Images\IssuesIcons\info.png"; break;
                    case "1": IconSource = @"\Images\IssuesIcons\warning.png"; break;
                    case "2": IconSource = @"\Images\IssuesIcons\error.png"; break;
                }

                IssuesWithCasesList.Add(new IssuesWithCasesModel(
                    reader["Level"].ToString()!,
                    reader["OrderID"].ToString()!,
                    reader["SkipReason"].ToString()!.Replace("&apos;", "'"),
                    reader["ForeColor"].ToString()!,
                    reader["CreateDate"].ToString()!,
                    IconSource
                ));
            }

        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[{ex.LineNumber()}] {ex.Message}");
        }
    }

    private void BwGetSentOutIssues_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
    {
        //listViewOrderIssues.Items.Refresh();
    }

    #region >> Initial Tasks at startup
    private void InitialTasksAtApplicationStartup_DoWork(object? sender, DoWorkEventArgs e)
    {
        Application.Current.Dispatcher.Invoke(new Action(() => 
        {
            SplashViewModel.Instance.LoadingText = "Gathering info from database..";
            ThisSite = DatabaseOperations.GetServerSiteName();
            ThreeShapeDirectoryHelper = DatabaseOperations.GetServerFileDirectory();
            ServerFriendlyNameHelper = DatabaseOperations.GetServerName();

            _ = bool.TryParse(ReadLocalSetting("GlassyEffect"), out bool GlassyEffect);
            _ = bool.TryParse(ReadLocalSetting("ShowUnitNumbers"), out bool showUnitNumbers);
            _ = bool.TryParse(ReadLocalSetting("ShowBottomInfoBar"), out bool showBottomInfoBar);
            _ = bool.TryParse(ReadLocalSetting("ShowDigiDetails"), out bool showDigiDetails);
            _ = bool.TryParse(ReadLocalSetting("ShowDigiCases"), out bool showDigiCases);
            _ = bool.TryParse(ReadLocalSetting("ActivePrescriptionMaker"), out bool activePrescriptionMaker);
            _ = bool.TryParse(ReadLocalSetting("OpenUpSironaScanFolder"), out bool openUpSironaScanFolder);
            _ = bool.TryParse(ReadLocalSetting("ShowEmptyPanCount"), out bool showEmptyPanCount);
            _ = bool.TryParse(ReadLocalSetting("ExtractIteroZipFiles"), out bool extractIteroZipFiles);
            _ = bool.TryParse(ReadLocalSetting("PmOpenUpPrescriptions"), out bool pmOpenUpPrescriptions);
            _ = bool.TryParse(ReadLocalSetting("ShowPendingDigiCases"), out bool showPendingDigiCases);
            _ = bool.TryParse(ReadLocalSetting("ShowDigiPrescriptionsCount"), out bool showDigiPrescriptionsCount);
            _ = bool.TryParse(ReadLocalSetting("ShowDigiCasesIn3ShapeTodayCount"), out bool showDigiCasesIn3ShapeTodayCount);

            _ = bool.TryParse(ReadLocalSetting("ModuleFolderSubscription"), out bool moduleFolderSubscription);
            _ = bool.TryParse(ReadLocalSetting("ModuleAccountInfos"), out bool moduleAccountInfos);
            _ = bool.TryParse(ReadLocalSetting("ModuleSmartOrderNames"), out bool moduleSmartOrderNames);
            _ = bool.TryParse(ReadLocalSetting("ModulePrescriptionMaker"), out bool modulePrescriptionMaker);
            _ = bool.TryParse(ReadLocalSetting("ModulePendingDigitals"), out bool modulePendingDigitals);

            _ = bool.TryParse(ReadStatsSetting("dcas_EmailWatcherActive"), out bool isDCASIsActive);
            
            _ = bool.TryParse(ReadLocalSetting("ColorCheckWindowIsOpen"), out bool isColorCheckWindowOpen);


            if (!bool.TryParse(ReadLocalSetting("IncludePendingDigiCases"), out bool includePendingDigiCases))
                CbSettingIncludePendingDigiCasesInNewlyArrived = true;

            CbSettingGlassyEffect = GlassyEffect;
            ShowUnitNumbers = showUnitNumbers;
            ShowBottomInfoBar = showBottomInfoBar;
            CbSettingShowDigiDetails = showDigiDetails;
            CbSettingShowDigiCases = showDigiCases;
            CbSettingWatchFolderPrescriptionMaker = activePrescriptionMaker;
            CbSettingOpenUpSironaScanFolder = openUpSironaScanFolder;
            CbSettingShowEmptyPanCount = showEmptyPanCount;
            CbSettingExtractIteroZipFiles = extractIteroZipFiles;
            PmOpenUpPrescriptionsBool = pmOpenUpPrescriptions;
            CbSettingShowPendingDigiCases = showPendingDigiCases;
            CbSettingIncludePendingDigiCasesInNewlyArrived = includePendingDigiCases;
            CbSettingShowDigiPrescriptionsCount = showDigiPrescriptionsCount;
            CbSettingShowDigiCasesIn3ShapeTodayCount = showDigiCasesIn3ShapeTodayCount;

            CbSettingModuleFolderSubscription = moduleFolderSubscription;
            CbSettingModuleAccountInfos = moduleAccountInfos;
            CbSettingModuleSmartOrderNames = moduleSmartOrderNames;
            CbSettingModulePrescriptionMaker = modulePrescriptionMaker;
            CbSettingModulePendingDigitals = modulePendingDigitals;

            IsDCASIsActive = isDCASIsActive;

            TriosInboxFolder = ThreeShapeDirectoryHelper + @"3ShapeCommunicate\Inbox";

            if (Directory.Exists(TriosInboxFolder))
            {
                fswTriosFolderWatcher.Path = TriosInboxFolder;
                fswTriosFolderWatcher.Filter = "*.*";
                fswTriosFolderWatcher.NotifyFilter = NotifyFilters.DirectoryName;
                fswTriosFolderWatcher.Created += new FileSystemEventHandler(FswTriosFolderWatcher_Created);
                fswTriosFolderWatcher.Deleted += new FileSystemEventHandler(FswTriosFolderWatcher_Deleted);
                fswTriosFolderWatcher.EnableRaisingEvents = true;
                CountTriosCases();
            }

            PendingDigiCasesReplacementName = ReadLocalSetting("PendingDigiCasesReplacementName");
            if (string.IsNullOrEmpty(PendingDigiCasesReplacementName))
                PendingDigiCasesReplacementName = "PendingDigi";

            FsubscrTargetFolder = ReadLocalSetting("SubscriptionCopyFolder");
            PmWatchedPdfFolder = ReadLocalSetting("PmWatchedPdfFolder");
            if (string.IsNullOrEmpty(PmWatchedPdfFolder))
                PmWatchedPdfFolder = "Click here to setup..";
            
            PmFinalPrescriptionsFolder = ReadLocalSetting("FinalPrescriptionsFolder");
            if (string.IsNullOrEmpty(PmFinalPrescriptionsFolder))
                PmFinalPrescriptionsFolder = "Click here to setup..";

            PmSironaScansFolder = ReadLocalSetting("SironaScansFolder");
            if (string.IsNullOrEmpty(PmSironaScansFolder))
                PmSironaScansFolder = "Click here to setup..";

            PmIteroExportFolder = ReadLocalSetting("IteroExportFolder");
            if (string.IsNullOrEmpty(PmIteroExportFolder))
                PmIteroExportFolder = "Click here to setup..";

            PmDownloadFolder = ReadLocalSetting("PmDownloadFolder");
            if (string.IsNullOrEmpty(PmDownloadFolder))
                PmDownloadFolder = "Click here to setup..";

            if (PmDownloadFolder.Contains("Click here to"))
            {
                PmDownloadFolder = Environment.GetEnvironmentVariable("USERPROFILE") + @"\" + @"Downloads\";
            }

            if (ShowBottomInfoBar)
                BottomBarSize = 120;
            else
                BottomBarSize = 35;

            SetAppVersion();

            ResetDigiSystemColors();

            FillUpEmptyPanNumberPanel();

            if (CbSettingModuleAccountInfos)
                GetAccountInfos();

            PDFTemp = DataBaseFolder + @"PDFTemp";

            if (Directory.Exists(PDFTemp))
                Directory.Delete(PDFTemp, true);

            Directory.CreateDirectory(PDFTemp);

            if (CbSettingWatchFolderPrescriptionMaker && Directory.Exists(PmWatchedPdfFolder))
            {
                fswPrescriptionMaker.Path = PmWatchedPdfFolder;
                fswPrescriptionMaker.Filter = "*.pdf";
                fswPrescriptionMaker.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
                fswPrescriptionMaker.Created += new FileSystemEventHandler(FswPrescriptionMaker_Created);
                fswPrescriptionMaker.Changed += new FileSystemEventHandler(FswPrescriptionMaker_Changed);
                fswPrescriptionMaker.EnableRaisingEvents = true;
            }

            if (CbSettingExtractIteroZipFiles && Directory.Exists(PmDownloadFolder) && Directory.Exists(PmIteroExportFolder))
            {
                fswIteroZipFileWhatcher.Path = PmDownloadFolder;
                fswIteroZipFileWhatcher.Filter = "iTero_Export_*.zip";
                fswIteroZipFileWhatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
                fswIteroZipFileWhatcher.Created += new FileSystemEventHandler(FswIteroZipFileWhatcher_Created);
                fswIteroZipFileWhatcher.Changed += new FileSystemEventHandler(FswIteroZipFileWhatcher_Created);
                fswIteroZipFileWhatcher.EnableRaisingEvents = true;

            }

            PmSendToList = GetAllSendToEnties();

            if (isColorCheckWindowOpen)
            {
                MainWindow.Instance.ShowHidePanColorCheckWindow();
            }
        }));
    }

    private async void SetAppVersion()
    {
        SoftwareVersion = await GetAppVersion();
        _ = double.TryParse(SoftwareVersion, out double appVersionDouble);
        AppVersionDouble = appVersionDouble;
    }

    public void InitialTasksAtApplicationStartup_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
    {
        SplashViewModel.Instance.LoadingText = "Loading finished!";
        SplashViewModel.Instance.mainWindow!.Show();
        SplashWindow.Instance.Hide();
        AppIsFullyLoaded = true;

        GeneralTimer_Tick(sender, e);

        AddDebugLine("Start", null, "Application started");
    }


    private async void FswIteroZipFileWhatcher_Created(object sender, FileSystemEventArgs e)
    {
        if (!CbSettingExtractIteroZipFiles)
            return;

        string exportFolder = PmIteroExportFolder;

        if (string.IsNullOrEmpty(exportFolder))
            return;

        bool success = false;
        try
        {
            if (Directory.Exists($@"{exportFolder}\{e.Name!.Replace(".zip", "")}"))
            {
                try
                {
                    Directory.Delete($@"{exportFolder}\{e.Name!.Replace(".zip", "")}", true);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[{ex.LineNumber()}] {ex.Message}");
                }
            }

            await Task.Run(() => ZipFile.ExtractToDirectory(e.FullPath, $@"{exportFolder}\{e.Name!.Replace(".zip", "")}"));

            LastIteroZipFileId = e.Name!.Replace(".zip", "").Replace("iTero_Export_", "");
            success = true;
            if (success)
                File.Delete(e.FullPath);

            ShowNotificationMessage("iTero Case Downloaded", $"There is a new Itero case placed into Export folder! Id: {LastIteroZipFileId}", NotificationIcon.Success, false, true);
            SystemSounds.Beep.Play();
            await BlinkWindow("green");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[{ex.LineNumber()}] {ex.Message}");
        }
    }



    public void AddDebugLine(string location, Exception? ex, string message)
    {
        string time = DateTime.Now.ToString("HH:mm:ss");
        string lineNumber = ex.LineNumber().ToString();


        DebugMessages.Add(new DebugMessagesModel(
            location,
            lineNumber,
            time,
            message
            ));
    }

    internal static void StartInitialTasks()
    {
        if (!bwInitialTasks.IsBusy)
        {
            bwInitialTasks.RunWorkerAsync();
        }
    }
    #endregion >> Initial Tasks at startup

    #region Looking for Update
    private async void LookForUpdate()
    {
        double remoteVersion = 0;
        try
        {
            string result = await new HttpClient().GetStringAsync("https://raw.githubusercontent.com/aml-one/StatsClient-2025/master/StatsClient/version.txt");
            _ = double.TryParse(result, out remoteVersion);
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                string remVersion = remoteVersion.ToString();
                LatestAppVersion = remVersion;
            }));
        }
        catch (Exception)
        {
            //try
            //{
            //    string result = await new HttpClient().GetStringAsync("https://aml.one/CaseChecker/version.txt");
            //    _ = double.TryParse(result[..result.IndexOf('-')].Trim(), out remoteVersion);
            //    Application.Current.Dispatcher.Invoke(new Action(() =>
            //    {
            //        string remVersion = remoteVersion.ToString();
            //        if (!remVersion.Contains('.'))
            //            remVersion += ".0";
            //        versionLabel.ToolTip = $"{(string)Lang["lastAvailableVersion"]}: v{remVersion}";
            //    }));
            //}
            //catch (Exception ex)
            //{
            //    Application.Current.Dispatcher.Invoke(new Action(() =>
            //    {
            //        MainViewModel.Instance.AddToDebug("#10e: " + ex.Message);
            //    }));
            //}
        }

        if (remoteVersion > AppVersionDouble)
        {
            UpdateAvailable = true;
            if (!AppJustStarted)
            {
                if (!UpdateMessagePresented)
                {
                    UpdateMessagePresented = true;
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        MessageBoxResult result = MessageBox.Show(_MainWindow, "New version available! Would you like to update the program to the new version?", "Stats Client - Update found", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            StartProgramUpdate();
                        }
                    }));
                }
            }

            if (!AutoUpdateAtStart)
            {
                Application.Current.Dispatcher.Invoke(new Action(StartProgramUpdate));
            }

            AppJustStarted = false;
        }
        else
            UpdateAvailable = false;

        AutoUpdateAtStart = true;
    }

    private void UpdateCheckTimer_Tick(object? sender, EventArgs e)
    {
        UpdateCheckTimer.Interval = new TimeSpan(0, 0, 5);
        LookForUpdate();
    }


    private async void DownloadUpdater()
    {
        Thread.Sleep(500);

        string appPath = Path.GetDirectoryName(AppContext.BaseDirectory)!;
        try
        {
            if (File.Exists($@"{LocalConfigFolderHelper}StatsClientUpdater.exe"))
                File.Delete($@"{LocalConfigFolderHelper}StatsClientUpdater.exe");

            Thread.Sleep(500);
            if (!File.Exists($@"{LocalConfigFolderHelper}StatsClientUpdater.exe"))
            {
                using var client = new HttpClient();
                using var s = await client.GetStreamAsync("https://raw.githubusercontent.com/aml-one/StatsClient-2025/master/StatsClient/Executable/StatsClientUpdater.exe");
                using var fs = new FileStream($@"{LocalConfigFolderHelper}StatsClientUpdater.exe", FileMode.OpenOrCreate);
                await s.CopyToAsync(fs);
            }
        }
        catch (Exception)
        {
            //try
            //{
            //    if (File.Exists($@"{LocalConfigFolderHelper}StatsClientUpdater.exe"))
            //        File.Delete($@"{LocalConfigFolderHelper}StatsClientUpdater.exe");

            //    if (!File.Exists($@"{LocalConfigFolderHelper}StatsClientUpdater.exe"))
            //    {
            //        using var client = new HttpClient();
            //        using var s = await client.GetStreamAsync("https://aml.one/CaseChecker---2025/CaseCheckerUpdater.exe");
            //        using var fs = new FileStream($@"{LocalConfigFolderHelper}StatsClientUpdater.exe", FileMode.OpenOrCreate);
            //        await s.CopyToAsync(fs);
            //    }
            //}
            //catch (Exception ex)
            //{
            //}
        }

        Thread.Sleep(3000);
    }

    private void StartUpdaterApp()
    {
        Thread.Sleep(3000);
        try
        {
            var p = new Process();

            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = $"/c \"{LocalConfigFolderHelper}StatsClientUpdater.exe\"";
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.Start();

            Thread.Sleep(2000);
        }
        catch (Exception ex)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(_MainWindow, ex.Message, "Stats Client - Error", MessageBoxButton.OK, MessageBoxImage.Error);
            });
        }
    }
    #endregion Looking for Update
}
