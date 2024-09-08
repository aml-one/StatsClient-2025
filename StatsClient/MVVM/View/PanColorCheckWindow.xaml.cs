using StatsClient.MVVM.Core;
using StatsClient.MVVM.ViewModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;


namespace StatsClient.MVVM.View
{
    /// <summary>
    /// Interaction logic for PanColorCheckWindow.xaml
    /// </summary>
    public partial class PanColorCheckWindow : Window
    {
        private static PanColorCheckWindow? staticInstance;
        public static PanColorCheckWindow StaticInstance
        {
            get => staticInstance!;
            set
            {
                staticInstance = value;
            }
        }

        public PanColorCheckWindow()
        {
            StaticInstance = this;
            InitializeComponent();
        }

        private void Border_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                try
                {
                    this.DragMove();
                    panNumberBox.Focus();
                    SaveWindowPosition();
                }
                catch { }
        }

        private void SaveWindowPosition()
        {
            double MaxTopPosition = SystemParameters.MaximizedPrimaryScreenHeight - (StaticInstance.Height + 17);
            double MaxLeftPosition = SystemParameters.MaximizedPrimaryScreenWidth - (StaticInstance.Width + 20);

            if (Top > MaxTopPosition)
                Top = MaxTopPosition;

            if (Left > MaxLeftPosition)
                Left = MaxLeftPosition;

            if (Left < 0)
                Left = 0;

            if (Top < 0)
                Top = 0;

            LocalSettingsDB.WriteLocalSetting("ColorCheckWindowPosTop", Top.ToString());
            LocalSettingsDB.WriteLocalSetting("ColorCheckWindowPosLeft", Left.ToString());
            LocalSettingsDB.WriteLocalSetting("ColorCheckWindowIsOpen", "true");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _ = int.TryParse(LocalSettingsDB.ReadLocalSetting("ColorCheckWindowPosTop"), out int wTop);
            _ = int.TryParse(LocalSettingsDB.ReadLocalSetting("ColorCheckWindowPosLeft"), out int wLeft);

            if (wTop != 0 && wLeft != 0)
            {
                Top = wTop;
                Left = wLeft;
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {   
            PanColorCheckViewModel.StaticInstance.PcPanColor = "#777777";
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            PanColorCheckViewModel.StaticInstance.PcPanColor = "#565656";
            PanColorCheckViewModel.StaticInstance.HideLabelVisibility = Visibility.Hidden;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            return;
        }

        private void PanNumberBox_MouseEnter(object sender, MouseEventArgs e)
        {
            PanColorCheckViewModel.StaticInstance.HideLabelVisibility = Visibility.Visible;
        }

        private void PanNumberBox_MouseLeave(object sender, MouseEventArgs e)
        {
            PanColorCheckViewModel.StaticInstance.HideLabelVisibility = Visibility.Hidden;
        }
    }
}
