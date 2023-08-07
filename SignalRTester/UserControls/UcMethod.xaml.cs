using System;
using System.Windows;
using System.Windows.Controls;
using SignalRTester.Components;
using SignalRTester.Models;

namespace SignalRTester.UserControls
{
    /// <summary>
    /// Interaction logic for UcMethod.xaml
    /// </summary>
    public partial class UcMethod : UserControl
    {

        public ITypesLoader TypesLoader
        {
            get { return (ITypesLoader)GetValue(TypesLoaderProperty); }
            set { SetValue(TypesLoaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TypesLoader.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TypesLoaderProperty =
            DependencyProperty.Register("TypesLoader", typeof(ITypesLoader), typeof(UcMethod), new PropertyMetadata(null));



        public UcMethod()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
