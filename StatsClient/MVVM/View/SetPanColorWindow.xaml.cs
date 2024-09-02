using StatsClient.MVVM.Core;
using StatsClient.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StatsClient.MVVM.View
{
    /// <summary>
    /// Interaction logic for SetPanColorWindow.xaml
    /// </summary>
    public partial class SetPanColorWindow : Window, INotifyPropertyChanged
    {
        public static event PropertyChangedEventHandler? PropertyChangedStatic;
        public event PropertyChangedEventHandler? PropertyChanged;

        public static void RaisePropertyChangedStatic([CallerMemberName] string? propertyname = null)
        {
            PropertyChangedStatic?.Invoke(typeof(ObservableObject), new PropertyChangedEventArgs(propertyname));
        }

        public void RaisePropertyChanged([CallerMemberName] string? propertyname = null)
        {
            PropertyChanged?.Invoke(typeof(ObservableObject), new PropertyChangedEventArgs(propertyname));
        }

        private static SetPanColorWindow? staticInstance;
        public static SetPanColorWindow StaticInstance
        {
            get => staticInstance!;
            set
            {
                staticInstance = value;
                RaisePropertyChangedStatic(nameof(StaticInstance));
            }
        }
        
        private SetPanColorWindow? instance;
        public SetPanColorWindow Instance
        {
            get => instance!;
            set
            {
                instance = value;
                RaisePropertyChangedStatic(nameof(Instance));
            }
        }

        public SetPanColorWindow(string? panNumber = "", string? originalColor = "")
        {
            Instance = this;
            StaticInstance = this;
            InitializeComponent();
            SetPanColorViewModel.StaticInstance!.PanNumber = panNumber!;

            if (originalColor == "0-0-0")
            {
                SetPanColorViewModel.StaticInstance!.WindowTitle = "Pick a color:";
                SetPanColorViewModel.StaticInstance!.OriginalColor = "255-255-255";
            }
            else
            {
                SetPanColorViewModel.StaticInstance!.WindowTitle = "Pick the new color:";
                SetPanColorViewModel.StaticInstance!.OriginalColor = originalColor!;
            }
        }
    }
}
