using StatsClient.MVVM.Core;
using StatsClient.MVVM.Model;
using StatsClient.MVVM.View;
using System.Diagnostics;
using System.Windows;

namespace StatsClient.MVVM.ViewModel;

public class MainMenuViewModel : ObservableObject
{
    private static MainMenuViewModel? staticInstance;
    public static MainMenuViewModel StaticInstance
    {
        get => staticInstance!;
        set
        {
            staticInstance = value;
            RaisePropertyChangedStatic(nameof(StaticInstance));
        }
    }

    private MainMenuViewModel? instance;
    public MainMenuViewModel Instance
    {
        get => instance!;
        set
        {
            instance = value;
            RaisePropertyChanged(nameof(Instance));
        }
    }

    private List<MainMenuItemModel> menuItems = [];
    public List<MainMenuItemModel> MenuItems
    {
        get => menuItems;
        set
        {
            menuItems = value;
            RaisePropertyChanged(nameof(MenuItems));
        }
    }
    
    private MainMenuItemModel? selectedMenuItem;
    public MainMenuItemModel? SelectedMenuItem
    {
        get => selectedMenuItem;
        set
        {
            selectedMenuItem = value;
            RaisePropertyChanged(nameof(SelectedMenuItem));
            if (value != null)
            { 
                RunCommand();
            }
        }
    }

    public RelayCommand CloseMainMenuCommand { get; set; }
    public RelayCommand RunCommandCommand { get; set; }

    public MainMenuViewModel()
    {
        StaticInstance = this;
        Instance = this;

        CloseMainMenuCommand = new RelayCommand(o => CloseMainMenu());
        RunCommandCommand = new RelayCommand(o => RunCommand());

        MenuItems =
        [
            new MainMenuItemModel { Icon = "/Images/ToolBar/update.png", Header = "Look for update", Command = "lookForUpdate" },
            new MainMenuItemModel { Icon = "/Images/ToolBar/folder.png", Header = "Open Manufacturing folder", Command = "openManufFolder"},
            new MainMenuItemModel { Icon = "/Images/ToolBar/folder.png", Header = "Open Trios Inbox folder", Command = "openTriosInbox"},
            new MainMenuItemModel { Icon = "/Images/ToolBar/rename.png", Header = "Open Smart Rename Window", Command = "openSmartRenameWindw"},
        ];
    }

    public void ShowSmartRenameMenuItem()
    {
        Debug.WriteLine("hit 2");

        List<MainMenuItemModel> menu = MenuItems;

        menu.FirstOrDefault(x => x.Command == "openSmartRenameWindw").Visible = Visibility.Visible;

        MenuItems = menu;
    }
    
    public void HideSmartRenameMenuItem()
    {
        List<MainMenuItemModel> menu = MenuItems;

        foreach (var item in menu)
        {
            if (item.Command == "openSmartRenameWindw")
            {
                item.Visible = Visibility.Collapsed;
            }
        }

        MenuItems = menu;
    }

    private void CloseMainMenu()
    {
        if (SelectedMenuItem is null)
            MainViewModel.Instance.MainMenuOpen = Visibility.Hidden;
        
    }

    private void RunCommand()
    {
        if (SelectedMenuItem is not null)
        {
            MainViewModel.Instance.RunMainMenuCommand(SelectedMenuItem.Command!);
            MainViewModel.Instance.MainMenuOpen = Visibility.Hidden;
            MainMenu.StaticInstance.mainMenuListView.UnselectAll();
        }

        SelectedMenuItem = null;
    }
}
