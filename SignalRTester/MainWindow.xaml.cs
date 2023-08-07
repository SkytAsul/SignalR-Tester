using MahApps.Metro.Controls;
using Microsoft.Win32;
using SignalRTester.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;

namespace SignalRTester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly MainWindowViewModel _vm;

        public MainWindow()
        {
            InitializeComponent();
            _vm = (MainWindowViewModel)DataContext;
        }

        private async void ButtonConnect_Click(object sender, RoutedEventArgs e)
        {
            await _vm.ConnectAsync();
        }

        private async void ButtonDisconnect_Click(object sender, RoutedEventArgs e)
        {
            await _vm.DisconnectAsync();
        }

        private void ItemClearOutput_Click(object sender, RoutedEventArgs e)
        {
            _vm.ClearOutput();
        }

        private void MetroWindow_Closed(object sender, System.EventArgs e)
        {
            _vm.SaveSettings();
        }

        private void Console_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            Console.ScrollToEnd();
        }

        private void CommandCloseMethod_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {

        }

        private void CommandCloseMethod_CanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            if (e.Parameter is TabItem item)
            {
                e.CanExecute = _vm.CanRemove((Models.Method)item.DataContext);
            }
        }

        private void ButtonDllLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new()
            {
                Multiselect = true,
                CheckFileExists = true,
                DefaultExt = ".dll",
                Title = "Open external dll"
            };

            if (ofd.ShowDialog(Owner) == true)
            {
                _vm.LoadDlls(ofd.FileNames);
            }
        }

        private async void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog ofd = new()
            {
                AddExtension = true,
                DefaultExt = ".json",
                Title = "Save to..."
            };

            if (ofd.ShowDialog(Owner) == true)
            {
                await _vm.SaveTo(ofd.FileName);
            }
        }

        private async void ButtonLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new()
            {
                AddExtension = true,
                DefaultExt = ".json",
                Title = "Load application"
            };

            if (ofd.ShowDialog(Owner) == true)
            {
                await _vm.Load(ofd.FileName);
            }
        }
    }
}
