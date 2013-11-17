using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using HighThroughputDataRetrieval;


namespace HighThroughputDataRetrieval
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainWindowViewModel viewModel = new MainWindowViewModel();
            Closing += viewModel.MyWindow_Closing;// Subsribes to close window event
        }
         private void dataGrid1_GotFocus(object sender, RoutedEventArgs e)
        {

            if (MessageBox.Show("Go to PubMed ?", "Message", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Hyperlink link = (Hyperlink)e.OriginalSource;
                Process.Start(link.NavigateUri.AbsoluteUri);
            }

        }
    }
}
