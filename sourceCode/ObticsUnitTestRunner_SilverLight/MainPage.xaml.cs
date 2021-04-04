using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ObticsUnitTestRunner_SilverLight
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            _ViewModel = new ViewModel(new TestRunContext(), typeof(ObticsUnitTest.Obtics.Regression).Assembly);
            
            InitializeComponent();
        }

        ViewModel _ViewModel;

        public ViewModel ViewModel { get { return _ViewModel; } }


        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            _ViewModel.Run();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _ViewModel.Cancel();
        }

        private void CheckBox_UpdateSource(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;

            if (checkbox != null)
            {
                var bexp = checkbox.GetBindingExpression(CheckBox.IsCheckedProperty);

                if (bexp != null)
                    bexp.UpdateSource();
            }
        }
    }
}
