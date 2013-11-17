<<<<<<< HEAD
﻿using System.Windows.Xps.Packaging;
=======
﻿using System.Windows;
using System.Windows.Xps.Packaging;
>>>>>>> 332f1feb7b960cc2c16085b9d18efa27cfd77b5a

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
<<<<<<< HEAD

            const string path = "../../Document/HelpDocument.xps";

            var helpDocument = new XpsDocument(path, System.IO.FileAccess.Read);

            HelpDocumentViewer.Document = helpDocument.GetFixedDocumentSequence();
=======
           

            // consider this move to xaml file
            const string path = @"../../Document/HelpDocument.xps";

            var helpDocument = new XpsDocument(path, System.IO.FileAccess.Read);

            HelpDocumentViewer.Document = helpDocument.GetFixedDocumentSequence();


>>>>>>> 332f1feb7b960cc2c16085b9d18efa27cfd77b5a
        }
    }
}
