using StatsClient.MVVM.Core;
using StatsClient.MVVM.Model;
using static StatsClient.MVVM.Core.DatabaseOperations;
using static StatsClient.MVVM.Core.Functions;
using System.Collections.ObjectModel;
using System.Timers;
using StatsClient.MVVM.View;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using Syncfusion.XPS;
using System.Diagnostics;



namespace StatsClient.MVVM.ViewModel;

public partial class SmartOrderNamesViewModel : ObservableObject
{
    private static SmartOrderNamesViewModel? staticInstance;
    public static SmartOrderNamesViewModel StaticInstance
    {
        get => staticInstance!;
        set
        {
            staticInstance = value;
            RaisePropertyChangedStatic(nameof(StaticInstance));
        }
    }

    private ObservableCollection<ThreeShapeOrdersModel> newOrdersByMe = [];
    public ObservableCollection<ThreeShapeOrdersModel> NewOrdersByMe
    {
        get => newOrdersByMe;
        set
        {
            newOrdersByMe = value;
            RaisePropertyChanged(nameof(NewOrdersByMe));
            if (AutoSelectFirstOrder && newOrdersByMe.Count > 0)
            {
                SelectedOrder = newOrdersByMe[0];
            }
        }
    }

    private readonly List<string> digitalSystems = ["None", "ABUTMENT-ONLY", "CARESTREAM", "DEXIS", "DSCORE", "DROPBOX", "EMAIL", "MEDIT", "TRIOS", "iTERO", "IS3D", "SIRONA"  ];
    public List<string> DigitalSystems
    {
        get => digitalSystems;
    }

    private string logMessage = "";
    public string LogMessage
    {
        get => logMessage!;
        set
        {
            logMessage = value;
            RaisePropertyChanged(nameof(LogMessage));
        }
    }

    private List<string> logMessages = [];
    public List<string> LogMessages
    {
        get => logMessages!;
        set
        {
            logMessages = value;
            RaisePropertyChanged(nameof(LogMessages));
        }
    }
    
    private List<string> customerSuggestionsList = [];
    public List<string> CustomerSuggestionsList
    {
        get => customerSuggestionsList!;
        set
        {
            customerSuggestionsList = value;
            RaisePropertyChanged(nameof(CustomerSuggestionsList));
        }
    }

    private string selectedCustomerName;
    public string SelectedCustomerName
    {
        get => selectedCustomerName;
        set
        {
            selectedCustomerName = value;
            RaisePropertyChanged(nameof(SelectedCustomerName));
            BuildName();
            FocusOnRenameButton();
        }
    }
    
    private string panNumber;
    public string PanNumber
    {
        get => panNumber;
        set
        {
            panNumber = value;
            RaisePropertyChanged(nameof(PanNumber));
        }
    }

    private bool namingCustomerFirst = false;
    public bool NamingCustomerFirst
    {
        get => namingCustomerFirst;
        set
        {
            namingCustomerFirst = value;
            RaisePropertyChanged(nameof(NamingCustomerFirst));
        }
    }
    
    private bool autoSelectFirstOrder = true;
    public bool AutoSelectFirstOrder
    {
        get => autoSelectFirstOrder;
        set
        {
            autoSelectFirstOrder = value;
            RaisePropertyChanged(nameof(AutoSelectFirstOrder));
        }
    }
    
    private bool namingCustomerLast = true;
    public bool NamingCustomerLast
    {
        get => namingCustomerLast;
        set
        {
            namingCustomerLast = value;
            RaisePropertyChanged(nameof(NamingCustomerLast));
        }
    }
    
    private bool isScrewRetained = false;
    public bool IsScrewRetained
    {
        get => isScrewRetained;
        set
        {
            isScrewRetained = value;
            RaisePropertyChanged(nameof(IsScrewRetained));
        }
    }

    private bool showCasesWithoutNumber = true;
    public bool ShowCasesWithoutNumber
    {
        get => showCasesWithoutNumber;
        set
        {
            showCasesWithoutNumber = value;
            RaisePropertyChanged(nameof(ShowCasesWithoutNumber));
        }
    }
    
    private bool hitRenameOnShadeSelect = false;
    public bool HitRenameOnShadeSelect
    {
        get => hitRenameOnShadeSelect;
        set
        {
            hitRenameOnShadeSelect = value;
            RaisePropertyChanged(nameof(HitRenameOnShadeSelect));
        }
    }

    private string selectedDigitalSystem = "";
    public string SelectedDigitalSystem
    {
        get => selectedDigitalSystem;
        set
        {
            selectedDigitalSystem = value;
            RaisePropertyChanged(nameof(SelectedDigitalSystem));
            FocusOnRenameButton();
        }
    }

    private string originalOrderID = "";
    public string OriginalOrderID
    {
        get => originalOrderID!;
        set
        {
            originalOrderID = value;
            RaisePropertyChanged(nameof(OriginalOrderID));
        }
    }

    private string selectedShade = "";
    public string SelectedShade
    {
        get => selectedShade;
        set
        {
            selectedShade = value;
            RaisePropertyChanged(nameof(SelectedShade));
        }
    }
    
    private string orderNamePreview = "";
    public string OrderNamePreview
    {
        get => orderNamePreview;
        set
        {
            orderNamePreview = value;
            RaisePropertyChanged(nameof(OrderNamePreview));
        }
    }
    
    private ThreeShapeOrdersModel selectedOrder;
    public ThreeShapeOrdersModel SelectedOrder
    {
        get => selectedOrder;
        set
        {
            if (value is not null)
            {
                selectedOrder = value;
                ResetNameForm();
                PreviouslySelectedOrder = selectedOrder;
                RaisePropertyChanged(nameof(SelectedOrder));
                FocusOnPanNumberBox();
                SelectedOrder = null;
            }

        }
    }
      

    private ThreeShapeOrdersModel previouslySelectedOrder;
    public ThreeShapeOrdersModel PreviouslySelectedOrder
    {
        get => previouslySelectedOrder;
        set
        {
            previouslySelectedOrder = value;
            RaisePropertyChanged(nameof(PreviouslySelectedOrder));
        }
    }

    private string threeShapeDirectoryHelper = "";
    public string ThreeShapeDirectoryHelper
    {
        get => threeShapeDirectoryHelper!;
        set
        {
            threeShapeDirectoryHelper = value;
            RaisePropertyChanged(nameof(ThreeShapeDirectoryHelper));
        }
    }

    private string toothNumbersString = "";
    public string ToothNumbersString
    {
        get => toothNumbersString!;
        set
        {
            toothNumbersString = value;
            RaisePropertyChanged(nameof(ToothNumbersString));
        }
    }

    private bool controlsEnabled = true;
    public bool ControlsEnabled
    {
        get => controlsEnabled!;
        set
        {
            controlsEnabled = value;
            RaisePropertyChanged(nameof(ControlsEnabled));
        }
    }

    private bool orderIDIsValid = true;
    public bool OrderIDIsValid
    {
        get => orderIDIsValid!;
        set
        {
            orderIDIsValid = value;
            RaisePropertyChanged(nameof(OrderIDIsValid));
        }
    }


    public RelayCommand RefreshCommand { get; set; }
    public RelayCommand BuildNameCommand { get; set; }
    public RelayCommand ShadeButtonClickedCommand { get; set; }
    public RelayCommand RenameOrderCommand { get; set; }
    public RelayCommand AddCustomerSuggestionCommand { get; set; }

    public System.Timers.Timer _timer;

    public SmartOrderNamesViewModel()
    {
        StaticInstance = this;
        MainViewModel.Instance.SmartOrderNamesViewModel = this;

        CustomerSuggestionsList = [];

        _timer = new System.Timers.Timer(10000);
        _timer.Elapsed += Timer_Elapsed;
        _timer.Start();

        RefreshCommand = new RelayCommand(o => Refresh());
        BuildNameCommand = new RelayCommand(o => BuildName());
        ShadeButtonClickedCommand = new RelayCommand(o => ShadeButtonClicked(o));
        RenameOrderCommand = new RelayCommand(o => RenameOrder());
        AddCustomerSuggestionCommand = new RelayCommand(o => AddCustomerSuggestion());

        ThreeShapeDirectoryHelper = GetServerFileDirectory();


        Timer_Elapsed(null, null);
    }

    private void FocusOnPanNumberBox()
    {
        Application.Current.Dispatcher.Invoke(() => {
            SmartOrderNamesPage.StaticInstance!.panNumberBox.Focus();
        });
    }
    
    private void FocusOnRenameButton()
    {
        Application.Current.Dispatcher.Invoke(() => { 
            if (SmartOrderNamesPage.StaticInstance is not null)
                SmartOrderNamesPage.StaticInstance!.renameButton.Focus();
        });
    }

    private async void AddCustomerSuggestion()
    {
        if (SelectedOrder is null)
        {
            AddCustomerSuggestionsWindow addCustomerSuggestionWindow = new()
            {
                Owner = MainWindow.Instance
            };
            addCustomerSuggestionWindow.ShowDialog();
        }
        else
        {
            AddCustomerSuggestionsWindow addCustomerSuggestionWindow = new(SelectedOrder.Customer)
            {
                Owner = MainWindow.Instance
            };
            addCustomerSuggestionWindow.ShowDialog();
        }

        
        
        if (SelectedOrder is not null)
        {
            CustomerSuggestionsList = await CustomerHasSuggestedName(SelectedOrder.Customer!);
        }
       
    }

    private void ShadeButtonClicked(object obj)
    {
        SelectedShade = (string)obj;
        BuildName("shade");
        SmartOrderNamesPage.StaticInstance!.renameButton.Focus();
    }

    private async void ResetNameForm()
    {
        Application.Current.Dispatcher.Invoke(() => {
            PanNumber = "";
            IsScrewRetained = false;
            SelectedDigitalSystem = "None";
            SelectedShade = "";
            OrderNamePreview = string.Empty;
            SelectedOrder = null;
            PreviouslySelectedOrder = null;
            CustomerSuggestionsList = [];   
        });

        await Refresh();
    }

    private async void BuildName(string obj = "")
    {
        if (SelectedOrder is null || string.IsNullOrEmpty(PanNumber))
            return;

        string finalName = "";
        string screwRetained = "";
        string patientName = $"-{SelectedOrder.Patient_LastName!}";
        string customer = SelectedOrder.Customer!;
        string shade = $"-{SelectedShade}";
        string digiSystem = $"-{SelectedDigitalSystem}";


        patientName = patientName.Replace(" ", "_")
                                .Replace(",", "")
                                .Replace("'", "_")
                                .Replace("\"", "_")
                                .Replace("+", "_")
                                .Replace("\\", "_")
                                .Replace("/", "_")
                                .Replace(":", "_")
                                .Replace("*", "_")
                                .Replace("?", "_")
                                .Replace("<", "_")
                                .Replace(">", "_")
                                .Replace("&", "-")
                                .Replace("|", "_")
                                .Trim();

        if (patientName == "-" || patientName == "--")
        {
            patientName = $"-{SelectedOrder.Patient_FirstName!}";
            patientName = patientName.Replace(" ", "_")
                                .Replace(",", "")
                                .Replace("'", "_")
                                .Replace("\"", "_")
                                .Replace("+", "_")
                                .Replace("\\", "_")
                                .Replace("/", "_")
                                .Replace(":", "_")
                                .Replace("*", "_")
                                .Replace("?", "_")
                                .Replace("<", "_")
                                .Replace(">", "_")
                                .Replace("&", "-")
                                .Replace("|", "_")
                                .Trim();

            if (patientName == "-" || patientName == "--")
                patientName = "-NONAME";
        }

        List<string> customerSuggestions = await CustomerHasSuggestedName(customer);
        if (customerSuggestions.Count > 0)
        {
            if (SelectedCustomerName is null)
            {
                customer = customerSuggestions[0];
                if (customerSuggestions.Count > 1)
                {
                    if (string.IsNullOrEmpty(SelectedCustomerName))
                        SelectedCustomerName = customerSuggestions[0];

                    CustomerSuggestionsList = customerSuggestions;
                }
            }
            else
            {
                customer = SelectedCustomerName;
            }
        }


        customer = $"-{CleanUpCustomerName(customer)}";

        ToothNumbersString = await GetToothNumbersString(SelectedOrder!.IntOrderID!);

        if (!string.IsNullOrEmpty(ToothNumbersString))
            ToothNumbersString = $"-{ToothNumbersString}";

        if (SelectedDigitalSystem.Equals("None"))
            digiSystem = "";
        if (shade.Equals("-"))
            shade = "";

        // check if pan number is valid or not..!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!



        if (IsScrewRetained)
            screwRetained = "-SCR";

        if (NamingCustomerFirst)
            finalName = $"{PanNumber}{customer}{patientName}{ToothNumbersString}{shade}{digiSystem}{screwRetained}";

        if (NamingCustomerLast)
            finalName = $"{PanNumber}{ToothNumbersString}{shade}{patientName}{customer}{digiSystem}{screwRetained}";

        finalName = finalName.Replace(" ", "_")
                             .Replace("'","")
                             .Replace("%","")
                             .Replace("*","")
                             .Replace(",","")
                             .Replace(".","")
                             .Replace("&","")
                             .Replace("@","")
                             .Replace("$","")
                             .Replace("+","")
                             .ToUpper();

        OrderNamePreview = finalName;

        if (obj == "shade" && HitRenameOnShadeSelect)
            RenameOrder();
    }

    

    private async Task Refresh()
    {
        NewOrdersByMe = await GetNewOrdersCreatedByMe(ShowCasesWithoutNumber);
    }

    private async void Timer_Elapsed(object? sender, ElapsedEventArgs e)
    {
        NewOrdersByMe = await GetNewOrdersCreatedByMe(ShowCasesWithoutNumber);
    }


    private async void RenameOrder()
    {
        OriginalOrderID = PreviouslySelectedOrder!.IntOrderID!;
        if (!CheckIfOrderIDIsUnique(OrderNamePreview))
        {
            MessageBox.Show(MainWindow.Instance, "It's not possible to rename the order.\nAn another order in 3Shape has the same name already.\n\nPlease ensure that the order number is unique.", "Stats Client", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }


        ControlsEnabled = false;
        OrderIDIsValid = false;
        await RenamingProcess();
        ResetNameForm();
        OrderNamePreview = string.Empty;
    }


    public async Task RenamingProcess()
    {

        ThreeShapeOrderInspectionModel inspectedOrder = InspectThreeShapeOrder(PreviouslySelectedOrder!.IntOrderID!);
        bool error = false;
        string NewFileName = OrderNamePreview;
        string NewFolderName = NewFileName;

        await LockOrderIn3Shape(NewFileName);
        //
        // starting renaming process 
        //

        try
        {

            // renaming Order's folder to the new name
            try
            {
                Directory.Move($"{ThreeShapeDirectoryHelper}{OriginalOrderID}", $"{ThreeShapeDirectoryHelper}{NewFileName}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[{ex.LineNumber()}] {ex.Message}");
                LogMessage = $"Couldn't rename the order's folder! (some app might still use it or 3Shape has a folder named as the order's new desired name)";
                ControlsEnabled = true;
                OrderIDIsValid = true;
                
                return;
            }

            // renaming the XML file to the new name
            File.Move(@$"{ThreeShapeDirectoryHelper}{NewFileName}\{OriginalOrderID}.xml", @$"{ThreeShapeDirectoryHelper}{NewFolderName}\{NewFileName}.xml");

            //
            // renaming the 3ML file if exists (designed orders only)
            //
            try
            {
                if (File.Exists(@$"{ThreeShapeDirectoryHelper}{NewFileName}\{OriginalOrderID}_3pl.3ml"))
                    File.Move(@$"{ThreeShapeDirectoryHelper}{NewFileName}\{OriginalOrderID}_3pl.3ml", @$"{ThreeShapeDirectoryHelper}{NewFolderName}\{NewFileName}_3pl.3ml");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[{ex.LineNumber()}] {ex.Message}");
            }
            //
            // END
            //



            // 
            // dealing with the XML file
            //
            string XMLFileContent = "";

            try
            {
                // opening up the XML file
                XMLFileContent = File.ReadAllText(@$"{ThreeShapeDirectoryHelper}{NewFolderName}\{NewFileName}.xml");

                // replacing all the entry in the text where the original filename presented to the new name
                XMLFileContent = XMLFileContent.Replace(OriginalOrderID, NewFileName);

                // saving the XML file
                File.WriteAllText(@$"{ThreeShapeDirectoryHelper}{NewFolderName}\{NewFileName}.xml", XMLFileContent);



                //
                // Renaming in the database
                //

                try
                {


                    ///
                    /// renaming
                    /// 
                    /// [PrintJobItem] [OrderID]
                    /// [OrderHistory] [OrderID]
                    /// [OrderExchangeElement] [OrderID]
                    /// [ImageOverlay] [OrderID]
                    /// [CustomData] [OrderID]
                    ///

                    string connectionString = DatabaseConnection.ConnectionStrFor3Shape();

                    string queryCopyLine = @$"INSERT INTO Orders ( 
                                             [IntOrderID] 
                                            ,[ExtOrderID] 
                                            ,[ClientID] 
                                            ,[ClientOrderNo] 
                                            ,[OrderDate] 
                                            ,[OrderImportanceID] 
                                            ,[Patient_RefNo] 
                                            ,[Patient_FirstName]
                                            ,[Patient_LastName] 
                                            ,[DeliveryAddress1] 
                                            ,[DeliveryAddress2] 
                                            ,[DeliveryZip] 
                                            ,[DeliveryCity] 
                                            ,[DeliveryState] 
                                            ,[DeliveryCountryID] 
                                            ,[DeliveryType] 
                                            ,[ShipToDeliveryAddress] 
                                            ,[ClientContactPerson] 
                                            ,[LabID] 
                                            ,[LabOperator] 
                                            ,[OrderComments] 
                                            ,[CreatedFromApp] 
                                            ,[RelativePos] 
                                            ,[OperatorID] 
                                            ,[DisplayOrderID] 
                                            ,[NumOrderID] 
                                            ,[DesignModuleID] 
                                            ,[ScanModuleID] 
                                            ,[FaceScanModuleID] 
                                            ,[Items] 
                                            ,[OperatorName] 
                                            ,[Customer] 
                                            ,[ManufName] 
                                            ,[OrderRelativePositionClass] 
                                            ,[ShipToERPCustNo] 
                                            ,[ERPCustomerNo] 
                                            ,[ShipToID] 
                                            ,[ModelManufacturingID] 
                                            ,[CacheMaterialName] 
                                            ,[ScanSource] 
                                            ,[ImprovementProgramSendDate] 
                                            ,[GroupFolder] 
                                            ,[CacheColor] 
                                            ,[OriginalOrderID] 
                                            ,[ImportOrderID] 
                                            ,[CacheMaxScanDate] 
                                            ,[TraySystemType]
                                            ,[ExternalLabID] 
                                            ,[ShipToDifferentAddress] 
                                            ,[PatientGuid]) 

                                        SELECT '{NewFileName}'
                                            ,[ExtOrderID] 
                                            ,[ClientID] 
                                            ,[ClientOrderNo] 
                                            ,[OrderDate] 
                                            ,[OrderImportanceID] 
                                            ,[Patient_RefNo] 
                                            ,[Patient_FirstName] 
                                            ,[Patient_LastName] 
                                            ,[DeliveryAddress1] 
                                            ,[DeliveryAddress2] 
                                            ,[DeliveryZip] 
                                            ,[DeliveryCity] 
                                            ,[DeliveryState] 
                                            ,[DeliveryCountryID] 
                                            ,[DeliveryType] 
                                            ,[ShipToDeliveryAddress] 
                                            ,[ClientContactPerson] 
                                            ,[LabID] 
                                            ,[LabOperator] 
                                            ,[OrderComments] 
                                            ,[CreatedFromApp] 
                                            ,[RelativePos] 
                                            ,[OperatorID] 
                                            ,[DisplayOrderID] 
                                            ,[NumOrderID] 
                                            ,[DesignModuleID] 
                                            ,[ScanModuleID] 
                                            ,[FaceScanModuleID] 
                                            ,[Items] 
                                            ,[OperatorName] 
                                            ,[Customer] 
                                            ,[ManufName] 
                                            ,[OrderRelativePositionClass] 
                                            ,[ShipToERPCustNo] 
                                            ,[ERPCustomerNo] 
                                            ,[ShipToID] 
                                            ,[ModelManufacturingID] 
                                            ,[CacheMaterialName] 
                                            ,[ScanSource] 
                                            ,[ImprovementProgramSendDate] 
                                            ,[GroupFolder] 
                                            ,[CacheColor] 
                                            ,[OriginalOrderID] 
                                            ,[ImportOrderID] 
                                            ,[CacheMaxScanDate] 
                                            ,[TraySystemType] 
                                            ,[ExternalLabID] 
                                            ,[ShipToDifferentAddress] 
                                            ,[PatientGuid] FROM Orders WHERE IntOrderID = '{OriginalOrderID}'";

                    await RunCommandAsynchronouslyWithLogging(queryCopyLine, connectionString);




                    string query6 = $"UPDATE ModelJob SET OrderID = '{NewFileName}' WHERE OrderID = '{OriginalOrderID}'";
                    await RunCommandAsynchronouslyWithLogging(query6, connectionString);

                    string query2 = $"UPDATE OrderHistory SET OrderID = '{NewFileName}' WHERE OrderID = '{OriginalOrderID}'";
                    await RunCommandAsynchronouslyWithLogging(query2, connectionString);

                    string query5 = $"UPDATE CustomData SET OrderID = '{NewFileName}' WHERE OrderID = '{OriginalOrderID}'";
                    await RunCommandAsynchronouslyWithLogging(query5, connectionString);

                    string query1 = $"UPDATE PrintJobItem SET OrderID = '{NewFileName}' WHERE OrderID = '{OriginalOrderID}'";
                    await RunCommandAsynchronouslyWithLogging(query1, connectionString);

                    string query7 = $"UPDATE CommunicateOrders SET OrderID = '{NewFileName}' WHERE OrderID = '{OriginalOrderID}'";
                    await RunCommandAsynchronouslyWithLogging(query7, connectionString);

                    string query3 = $"UPDATE OrderExchangeElement SET OrderID = '{NewFileName}' WHERE OrderID = '{OriginalOrderID}'";
                    await RunCommandAsynchronouslyWithLogging(query3, connectionString);

                    string query4 = $"UPDATE ImageOverlay SET OrderID = '{NewFileName}' WHERE OrderID = '{OriginalOrderID}'";
                    await RunCommandAsynchronouslyWithLogging(query4, connectionString);




                    UpdateLastModifyDateinDatabase(NewFileName);




                    string queryRemoveOriginalLine = $"DELETE FROM Orders WHERE IntOrderID = '{OriginalOrderID}'";
                    await RunCommandAsynchronouslyWithLogging(queryRemoveOriginalLine, connectionString);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[{ex.LineNumber()}] {ex.Message}");
                    LogMessage = $"Error ({ex.LineNumber()}): [{ex.Message}]";
                    LogMessages.Add(LogMessage);
                    error = true;
                }

                //
                // END
                //





            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[{ex.LineNumber()}] {ex.Message}");
                error = true;
                LogMessage = $"Error ({ex.LineNumber()}): [{ex.Message}]";
                LogMessages.Add(LogMessage);
                MessageBox.Show(ex.Message, "Something went wrong", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            //
            // END
            //


        }
        catch (Exception e)
        {
            Debug.WriteLine($"[{e.LineNumber()}] {e.Message}");
            error = true;
            LogMessage = $"Error ({e.LineNumber()}): [{e.Message}]";
            LogMessages.Add(LogMessage);
        }


        //
        // returning every form control to original stage
        //

        if (!error)
        {
            LogMessage = $"\nRenaming finised with no issues.";
            LogMessages.Add(LogMessage);

            if (LogMessages.Count > 0)
            {
                string message = "";
                foreach (string line in LogMessages)
                    message += line + "\n";
                try
                {
                    File.WriteAllText(@$"{ThreeShapeDirectoryHelper}{NewFolderName}\OrderRename.log", message);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[{ex.LineNumber()}] {ex.Message}");
                }
            }

            //openOrderIdHelper = NewFileName;
            await UnLockOrderIn3Shape(NewFileName);
            ResetNameForm();
        }
        else
        {
            LogMessage = $"\nEncountered some issues during renaming..";
            LogMessages.Add(LogMessage);

            if (LogMessages.Count > 0)
            {
                string message = "";
                foreach (string line in LogMessages)
                    message += line + "\n";
                try
                {
                    File.WriteAllText(@$"{ThreeShapeDirectoryHelper}{NewFolderName}\OrderRename.log", message);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[{ex.LineNumber()}] {ex.Message}");
                }
            }
        }


        ControlsEnabled = true;
        OrderIDIsValid = true;
        //
        // END
        //
    }




    private async Task RunCommandAsynchronouslyWithLogging(string commandText, string connectionString)
    {
        using SqlConnection connection = new(connectionString);
        try
        {
            SqlCommand command = new(commandText, connection);
            connection.Open();

            IAsyncResult result = command.BeginExecuteNonQuery();
            while (!result.IsCompleted)
            {
                Thread.Sleep(100);
            }
            LogMessage = $"Command complete. Affected [{command.EndExecuteNonQuery(result)}] rows.";
            LogMessages.Add(LogMessage);
            await Task.Delay(20);
        }
        catch (SqlException ex)
        {
            Debug.WriteLine($"[{ex.LineNumber()}] {ex.Message}");
            LogMessage = $"Error Exception ({ex.LineNumber()}): [{ex.Message}]";
            LogMessages.Add(LogMessage);
            await Task.Delay(300);
        }
        catch (InvalidOperationException ex)
        {
            Debug.WriteLine($"[{ex.LineNumber()}] {ex.Message}");
            LogMessage = $"Error ({ex.LineNumber()}): [{ex.Message}]";
            LogMessages.Add(LogMessage);
            await Task.Delay(300);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[{ex.LineNumber()}] {ex.Message}");
            LogMessage = $"Error General ({ex.LineNumber()}): [{ex.Message}]";
            LogMessages.Add(LogMessage);
            await Task.Delay(300);
        }
    }
}
