using System.Windows;
using ValidationSample.ViewModels;

namespace ValidationSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            TabItem1.DataContext = new Tab1ViewModel();
            TabItem2.DataContext = new Tab2ViewModel();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var viewModel1 = (Tab1ViewModel)TabItem1.DataContext;
            var viewModel2 = (Tab2ViewModel)TabItem2.DataContext;
            viewModel1.TestText1 = "Added data";
            viewModel2.TestText2 = "Added data";
        }
    }
}
