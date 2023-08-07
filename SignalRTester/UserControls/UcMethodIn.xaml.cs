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
    public partial class UcMethodIn : UserControl
    {

        public ITypesLoader TypesLoader
        {
            get { return (ITypesLoader)GetValue(TypesLoaderProperty); }
            set { SetValue(TypesLoaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TypesLoader.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TypesLoaderProperty =
            DependencyProperty.Register("TypesLoader", typeof(ITypesLoader), typeof(UcMethodIn), new PropertyMetadata(null));



        public bool IsEditable
        {
            get { return (bool)GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsEditable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsEditableProperty =
            DependencyProperty.Register("IsEditable", typeof(bool), typeof(UcMethodIn), new PropertyMetadata(true));



        public UcMethodIn()
        {
            InitializeComponent();
        }
    }
}
