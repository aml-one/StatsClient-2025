using StatsClient.MVVM.Core;
using StatsClient.MVVM.Model;
using static StatsClient.MVVM.Core.DatabaseOperations;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace StatsClient.MVVM.ViewModel;

public class AvailablePanNumberNotificationsViewModel : ObservableObject
{

    private static AvailablePanNumberNotificationsViewModel? instance;
    public static AvailablePanNumberNotificationsViewModel Instance
    {
        get => instance!;
        set
        {
            instance = value;
            RaisePropertyChangedStatic(nameof(Instance));
        }
    }

    private List<AvailablePanCountModel>? availablePanNumberCountList = [];
    public List<AvailablePanCountModel>? AvailablePanNumberCountList
    {
        get => availablePanNumberCountList;
        set
        {
            if (availablePanNumberCountList == value) return;
            availablePanNumberCountList = value;
            RaisePropertyChanged(nameof(AvailablePanNumberCountList));
        }
    }

    private double windowOpacity = 1;
    public double WindowOpacity
    {
        get => windowOpacity;
        set
        {
            if (windowOpacity == value) return;
            windowOpacity = value;
            RaisePropertyChanged(nameof(WindowOpacity));
        }
    }

    private double listOpacity = 1;
    public double ListOpacity
    {
        get => listOpacity;
        set
        {
            if (listOpacity == value) return;
            listOpacity = value;
            RaisePropertyChanged(nameof(ListOpacity));
        }
    }

    private bool blinkerOn = false;
    public bool BlinkerOn
    {
        get => blinkerOn;
        set
        {
            if (blinkerOn == value) return;
            blinkerOn = value;
            RaisePropertyChanged(nameof(BlinkerOn));
        }
    }

    private double numberFontSize = 30;
    public double NumberFontSize
    {
        get => numberFontSize;
        set
        {
            if (numberFontSize == value) return;
            numberFontSize = value;
            RaisePropertyChanged(nameof(NumberFontSize));
        }
    }

    private double titleFontSize = 20;
    public double TitleFontSize
    {
        get => titleFontSize;
        set
        {
            if (titleFontSize == value) return;
            titleFontSize = value;
            RaisePropertyChanged(nameof(TitleFontSize));
        }
    }

    private double namesFontSize = 20;
    public double NamesFontSize
    {
        get => namesFontSize;
        set
        {
            if (namesFontSize == value) return;
            namesFontSize = value;
            RaisePropertyChanged(nameof(NamesFontSize));
        }
    }

    private VerticalAlignment badgePositionVertical = VerticalAlignment.Bottom;
    public VerticalAlignment BadgePositionVertical
    {
        get => badgePositionVertical;
        set
        {
            if (badgePositionVertical == value) return;
            badgePositionVertical = value;
            RaisePropertyChanged(nameof(BadgePositionVertical));
        }
    }

    private HorizontalAlignment badgePositionHorizontal = HorizontalAlignment.Right;
    public HorizontalAlignment BadgePositionHorizontal
    {
        get => badgePositionHorizontal;
        set
        {
            if (badgePositionHorizontal == value) return;
            badgePositionHorizontal = value;
            RaisePropertyChanged(nameof(BadgePositionHorizontal));
        }
    }



    private readonly DispatcherTimer GeneralTimer = new();


    public AvailablePanNumberNotificationsViewModel()
    {
        Instance = this;
        double sHeight = SystemParameters.WorkArea.Height;
        NumberFontSize = sHeight / 15;
        TitleFontSize = sHeight / 55;
        NamesFontSize = sHeight / 65;

        GeneralTimer.Tick += GeneralTimer_Tick;
        GeneralTimer.Interval = new TimeSpan(0, 0, 1);
        GeneralTimer.Start();

    }


    private async void GeneralTimer_Tick(object? sender, EventArgs e)
    {
        int seconds = DateTime.Now.Second;
        int minutes = DateTime.Now.Minute;

        if (seconds % 5 == 0)
        {
            double sHeight = SystemParameters.WorkArea.Height;
            NumberFontSize = sHeight / 15;
            TitleFontSize = sHeight / 55;
            NamesFontSize = sHeight / 65;
        }

        if (BlinkerOn)
        {
            if (ListOpacity == 0)
                ListOpacity = 1;
            else
                ListOpacity = 0;
        }

        if (seconds % 10 == 0)
        {
            if (BadgePositionVertical == VerticalAlignment.Bottom && BadgePositionHorizontal == HorizontalAlignment.Right)
            {
                BadgePositionVertical = VerticalAlignment.Top;
                BadgePositionHorizontal = HorizontalAlignment.Right;
            }

            else if (BadgePositionVertical == VerticalAlignment.Top && BadgePositionHorizontal == HorizontalAlignment.Right)
            {
                BadgePositionVertical = VerticalAlignment.Top;
                BadgePositionHorizontal = HorizontalAlignment.Left;
            }

            else if (BadgePositionVertical == VerticalAlignment.Top && BadgePositionHorizontal == HorizontalAlignment.Left)
            {
                BadgePositionVertical = VerticalAlignment.Bottom;
                BadgePositionHorizontal = HorizontalAlignment.Left;
            }

            else if (BadgePositionVertical == VerticalAlignment.Bottom && BadgePositionHorizontal == HorizontalAlignment.Left)
            {
                BadgePositionVertical = VerticalAlignment.Bottom;
                BadgePositionHorizontal = HorizontalAlignment.Right;
            }
        }
        AvailablePanNumberCountList = await GetBackAllAvailablePanNumberListCount(NumberFontSize, TitleFontSize, NamesFontSize);


        if (AvailablePanNumberCountList.Any(x => x.Count < 11) && minutes % 5 == 0)
            WindowOpacity = 1;
        else
            WindowOpacity = 0;

        if (AvailablePanNumberCountList.Any(x => x.Count < 6))
            BlinkerOn = true;
        else
            BlinkerOn = false;
    }
}
