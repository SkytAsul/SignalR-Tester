using MahApps.Metro.Controls;
using Microsoft.Win32;
using SignalRTester.Models;
using SignalRTester.UserControls;
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

        private async void ButtonConnection_Click(object sender, RoutedEventArgs e)
        {
            if (_vm.IsConnected)
            {
                await _vm.DisconnectAsync();
            }
            else
            {
                await _vm.ConnectAsync();
            }
        }

        private void ItemClearOutput_Click(object sender, RoutedEventArgs e)
        {
            _vm.ClearOutput();
        }

        private async void MetroWindow_Closed(object sender, System.EventArgs e)
        {
            if (_vm.IsConnected)
            {
                await _vm.DisconnectAsync();
            }
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
            if (!_vm.IsEditable)
            {
                e.CanExecute = false;
                return;
            }

            if (e.Parameter is TabItem item)
            {
                if (item.DataContext is MethodIn methodIn)
                {
                    e.CanExecute = _vm.CanRemoveIn(methodIn);
                }
                else if (item.DataContext is MethodOut methodOut)
                {
                    e.CanExecute = _vm.CanRemoveOut(methodOut);
                }
            }
        }

        private void ButtonDllLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new()
            {
                Multiselect = true,
                CheckFileExists = true,
                DefaultExt = ".dll",
                Filter = "DLL Files (*.dll)|*.dll",
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
                Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
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
                Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                Title = "Load application"
            };

            if (ofd.ShowDialog(Owner) == true)
            {
                await _vm.Load(ofd.FileName);
            }
        }

        private void UcMethodOut_SendMethod(object sender, EventArgs e)
        {
            MethodOut method = (MethodOut)((UcMethodOut)sender).DataContext;
            _vm.SendMethod(method);
        }
    }
}
