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
<<<<<<< HEAD
=======
<<<<<<< HEAD
           
            const string Path = @"../../Document/HelpDocument.xps";

            XpsDocument helpDocument = new XpsDocument(Path, System.IO.FileAccess.Read);

=======
<<<<<<< HEAD
            //const string Path = @"\\LAMTAC-PC\Users\lamtac\HighThroughputDataRetrieval\HighThroughputDataRetrieval\HighThroughputDataRetrieval\Document\";
            //XpsDocument helpDocument = new XpsDocument(Path+ @"HelpDocument.xps", System.IO.FileAccess.Read);
            //HelpDocumentViewer.Document = helpDocument.GetFixedDocumentSequence();
=======
<<<<<<< HEAD
            const string Path = @"..";
=======
<<<<<<< HEAD
            //const string Path = @"C:\Users\Owner\HighThroughputDataRetrieval\HighThroughputDataRetrieval\HighThroughputDataRetrieval\Document\";
            XpsDocument helpDocument = new XpsDocument( @"../Document\HelpDocument.xps", System.IO.FileAccess.Read);
=======
            const string Path = @"\\LAMTAC-PC\Users\lamtac\HighThroughputDataRetrieval\HighThroughputDataRetrieval\HighThroughputDataRetrieval\Document\";
>>>>>>> 53f4ec6e2fec155a7fb0844b533267e4df51fdb4
            XpsDocument helpDocument = new XpsDocument(Path+ @"HelpDocument.xps", System.IO.FileAccess.Read);
>>>>>>> cd29e245a4ad6c8f8372289a3a92284f2c7d0b16
>>>>>>> 2c9ff56905d392a7214dcfe8fe1c2912986ae4ae
            HelpDocumentViewer.Document = helpDocument.GetFixedDocumentSequence();
>>>>>>> a3be286fe852bb8bb7a1312965571f7fb33f2dbd

        }
    }
}
