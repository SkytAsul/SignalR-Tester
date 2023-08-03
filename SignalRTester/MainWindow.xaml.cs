using System.Windows;

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
    }
}
