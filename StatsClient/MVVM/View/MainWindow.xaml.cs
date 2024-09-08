using StatsClient.MVVM.Core;
using StatsClient.MVVM.ViewModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Application = System.Windows.Application;
using Button = System.Windows.Controls.Button;
using static StatsClient.MVVM.Core.LocalSettingsDB;
using static StatsClient.MVVM.Core.Functions;

namespace StatsClient.MVVM.View;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window, INotifyPropertyChanged
{
    
    public static event PropertyChangedEventHandler? PropertyChangedStatic;
    public event PropertyChangedEventHandler? PropertyChanged;

    public static void RaisePropertyChangedStatic([CallerMemberName] string? propertyname = null)
    {
        PropertyChangedStatic?.Invoke(typeof(ObservableObject), new PropertyChangedEventArgs(propertyname));
    }

    private static MainWindow? instance;
    public static MainWindow Instance
    {
        get => instance!;
        set
        {
            instance = value;
            RaisePropertyChangedStatic(nameof(Instance));
        }
    }

    private static PanColorCheckWindow? PanColorCheckWindowInstance;

    public MainWindow()
    {
        //Register Syncfusion license
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NBaF1cWWhIfEx1RHxQdld5ZFRHallYTnNWUj0eQnxTdEFjXH1fcH1QR2VdVE1xWw==");
        Instance = this;
        InitializeComponent();
        DataContext = MainViewModel.Instance;

        MainViewModel.Instance._MainWindow = this;
        pb3ShapeProgressBar.Value = 0;

        _ = int.TryParse(ReadLocalSetting("WindowWidth"), out int wWidth);
        _ = int.TryParse(ReadLocalSetting("WindowHeight"), out int wHeight);
        _ = int.TryParse(ReadLocalSetting("WindowTop"), out int wTop);
        _ = int.TryParse(ReadLocalSetting("WindowLeft"), out int wLeft);

        Width = wWidth;
        Height = wHeight;
        Top = wTop;
        Left = wLeft;

        string groupProp = ReadLocalSetting("GroupBy");
        if (groupProp != null)
        {
            GroupBy.SelectedItem = groupProp;
            MainViewModel.Instance.GroupList();
        }

        string filterUsed = ReadLocalSetting("FilterUsed");
        if (!string.IsNullOrEmpty(filterUsed)) 
        {
            MainViewModel.Instance.Search(filterUsed, true);
            MainViewModel.Instance.ShowNotificationMessage("Startup", "Last view was restored!");
        }

        tbSearch.PreviewKeyDown += new KeyEventHandler(HandleEsc);

        PanColorCheckWindowInstance = new();

        
        
    }


    private void Window_Closing(object sender, CancelEventArgs e)
    {
        Application.Current.Shutdown();
    }

    public void TitleBar_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount >= 2)
            BtnMaximize_Click(sender, e);

        if (e.ChangedButton == MouseButton.Left)
            try
            {
                this.DragMove();
            }
            catch { }
    }

    private void BtnMinimize_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void BtnMaximize_Click(object sender, RoutedEventArgs e)
    {
        MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;

        if (WindowState == WindowState.Maximized)
        {
            WindowState = WindowState.Normal;
            this.BorderThickness = new Thickness(0);
            btnMaximize.Content = "▣";
        }
        else if (WindowState == WindowState.Normal)
        {
            WindowState = WindowState.Maximized;
            this.BorderThickness = new Thickness(6, 6, 6, 6);
            btnMaximize.Content = "⧉";
        }

    }

    private void BtnCloseApplication_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;

        if (WindowState == WindowState.Maximized)
        {
            this.BorderThickness = new Thickness(6, 6, 6, 6);
            btnMaximize.Content = "⧉";
        }
        else if (WindowState == WindowState.Normal)
        {
            this.BorderThickness = new Thickness(0);
            btnMaximize.Content = "▣";
        }

        if (WindowState != WindowState.Minimized)
        {
            WriteLocalSetting("WindowWidth", Width.ToString());
            WriteLocalSetting("WindowHeight", Height.ToString());
        }
    }

    private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!ThreeShapeTab.IsSelected)
        {
            MainViewModel.Instance.ThreeShapeObject = null;
            MainViewModel.Instance.HideAllToolBarButtons();
            MainViewModel.Instance.ThreeShapeObject = null;
            MainViewModel.Instance.Is3ShapeTabSelected = false;
        }
        else
        {
            MainViewModel.Instance.Is3ShapeTabSelected = true;
        }
    }

    private void BtnFilter_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            Button? button = sender as Button;
            ContextMenu contextMenu = button!.ContextMenu;
            contextMenu.PlacementTarget = button;
            contextMenu.Placement = PlacementMode.Bottom;
            contextMenu.IsOpen = true;
            e.Handled = true;
        }
    }

    private void GridViewColumnHeader_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        // preventing icon column from resize
        e.Handled = true;
        ((GridViewColumnHeader)sender).Column.Width = 47;
    }

    private void Window_LocationChanged(object sender, EventArgs e)
    {
        WriteLocalSetting("WindowTop", Top.ToString());
        WriteLocalSetting("WindowLeft", Left.ToString());
    }

    private void MenuItemOpenUpColorCheckWindow_Click(object sender, RoutedEventArgs e)
    {
        ShowHidePanColorCheckWindow();
    }

    public void ShowHidePanColorCheckWindow()
    {
        if (PanColorCheckWindowInstance is not null)
        {
           // PanColorCheckWindowInstance.Owner = Instance;

            if (IsWindowIsShown<PanColorCheckWindow>("panColorCheckWindow"))
            {
                PanColorCheckWindowInstance.Hide();
                WriteLocalSetting("ColorCheckWindowPosTop", "");
                WriteLocalSetting("ColorCheckWindowPosLeft", "");
                WriteLocalSetting("ColorCheckWindowIsOpen", "false");

                MainViewModel.Instance.OpenUpColorCheckWindowMenuItemTitle = "Open up ColorCheck window";
            }
            else
            {
                PanColorCheckWindowInstance.Show();
                WriteLocalSetting("ColorCheckWindowIsOpen", "true");

                MainViewModel.Instance.OpenUpColorCheckWindowMenuItemTitle = "Close ColorCheck window";
            }
        }
    }

    private void listView3ShapeOrders_MouseDown(object sender, MouseButtonEventArgs e)
    {
        tbSearch.Focus();
    }

    private void HandleEsc(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
            tbSearch.Clear();
    }
}
