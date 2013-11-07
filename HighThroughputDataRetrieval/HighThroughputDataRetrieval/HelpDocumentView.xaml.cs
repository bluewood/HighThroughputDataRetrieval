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
            //const string Path = @"\\LAMTAC-PC\Users\lamtac\HighThroughputDataRetrieval\HighThroughputDataRetrieval\HighThroughputDataRetrieval\Document\";
            //XpsDocument helpDocument = new XpsDocument(Path+ @"HelpDocument.xps", System.IO.FileAccess.Read);
            //HelpDocumentViewer.Document = helpDocument.GetFixedDocumentSequence();
        }
    }
}
