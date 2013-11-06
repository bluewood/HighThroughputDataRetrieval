using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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
            HelpDocumentViewer.Document = helpDocument.GetFixedDocumentSequence();

        }
    }
}
