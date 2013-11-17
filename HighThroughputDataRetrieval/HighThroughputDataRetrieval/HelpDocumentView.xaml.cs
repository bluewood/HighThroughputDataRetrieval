using System.Windows.Xps.Packaging;

namespace HighThroughputDataRetrieval
{
    /// <summary>
    /// Interaction logic for HelpDocumentView.xaml
    /// </summary>
    public partial class HelpDocumentView
    {
        public HelpDocumentView()
        {
            InitializeComponent();

            const string path = "../../Document/HelpDocument.xps";

            var helpDocument = new XpsDocument(path, System.IO.FileAccess.Read);

            HelpDocumentViewer.Document = helpDocument.GetFixedDocumentSequence();
        }
    }
}
