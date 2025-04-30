using StatsClient.MVVM.Core;
using StatsClient.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StatsClient.MVVM.ViewModel;

public class ImportHistoryAnnouncementsViewModel : ObservableObject
{

    private static ImportHistoryAnnouncementsViewModel? instance;
    public static ImportHistoryAnnouncementsViewModel Instance
    {
        get => instance!;
        set
        {
            instance = value;
            RaisePropertyChangedStatic(nameof(Instance));
        }
    }

    private double multiplier = 1;
    public double Multiplier
    {
        get => multiplier;
        set
        {
            if (multiplier == value) return;
            multiplier = value;
            RaisePropertyChanged(nameof(Multiplier));
        }
    }

    private List<ImportHistoryModel>? importHistoryList = [];
    public List<ImportHistoryModel>? ImportHistoryList
    {
        get => importHistoryList;
        set
        {
            if (importHistoryList == value) return;
            importHistoryList = value;
            RaisePropertyChanged(nameof(ImportHistoryList));
        }
    }

    public ImportHistoryAnnouncementsViewModel()
    {
        Instance = this;
    }
}
