using System.Windows;
using System.Windows.Xps.Packaging;

namespace HighThroughputDataRetrieval
{
    /// <summary>
    /// Interaction logic for HelpDocumentView.xaml
    /// </summary>
    public partial class HelpDocumentView : Window
    {
        public HelpDocumentView()
        {
            InitializeComponent();
           
            const string Path = @"../../Document/HelpDocument.xps";

            XpsDocument helpDocument = new XpsDocument(Path, System.IO.FileAccess.Read);

            HelpDocumentViewer.Document = helpDocument.GetFixedDocumentSequence();

        }
    }
}
