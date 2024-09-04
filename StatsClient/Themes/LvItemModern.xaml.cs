using StatsClient.MVVM.View;
using StatsClient.MVVM.ViewModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace StatsClient.Themes;

public partial class LvItemModern
{
    public LvItemModern()
    {
        InitializeComponent();
    }

    private void MenuItemOpenUpOrderInfoWindow_Click(object sender, RoutedEventArgs e)
    {
        MainViewModel.Instance.OpenUpOrderInfoWindow();
    }

    private void MenuItemOpenUpRenameOrderWindow_Click(object sender, RoutedEventArgs e)
    {
        MainViewModel.Instance.OpenUpRenameOrderWindow();
    }
    
    private void MenuItemGenerateStCopy_Click(object sender, RoutedEventArgs e)
    {
        MainViewModel.Instance.GenerateStCopy();
    }

    private void ContextMenu_Opened(object sender, RoutedEventArgs e)
    {
        MainViewModel.Instance.ListUpdateable = false;
    }

    private void ContextMenu_Closed(object sender, RoutedEventArgs e)
    {
        MainViewModel.Instance.ListUpdateable = true;
    }

    private void MenuItemExploreFolder_Click(object sender, RoutedEventArgs e)
    {
        MainViewModel.Instance.ExploreOrderFolder();
    }

    private void AccountInfosShowPassword_Button_Click(object sender, RoutedEventArgs e)
    {
        if (((Button)sender).Tag is not null)
        {
            string password = ((Button)sender).Tag.ToString()!;
            if ((TextBlock)((Grid)((Button)sender).Parent).Children[3] is not null) {
                try
                {
                    TextBlock passwordTb = (TextBlock)((Grid)((Button)sender).Parent).Children[3];

                    ShowPassword(passwordTb, password);
                }
                catch (Exception)
                {
                }
            }
        }
        else
        {
            MessageBox.Show(MainWindow.Instance, "No password found!", "Password missing", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private async void ShowPassword(TextBlock passwordTb, string password)
    {
        passwordTb.Text = password;
        await Task.Delay(2000);

        passwordTb.Text = "------";
    }

    
}