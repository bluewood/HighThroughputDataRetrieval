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

namespace HighThroughputDateRetrieval
{
    /// <summary>
    /// Interaction logic for HitCountTable.xaml
    /// </summary>
    public partial class HitCountTable : Window
    {
        public HitCountTable()
        {
            InitializeComponent();

            //DataGridTextColumn hitCounTextColumn = new DataGridTextColumn();
            //hitCounTextColumn.Header = "Number of Publication";
            //DataGridTextColumn inpuTextColumn = new DataGridTextColumn();
            //inpuTextColumn.Header = "Input";
            ////hitCounTextColumn.Binding = new Binding("YourBindingField");
            //HitCountTableDataGrid.Columns.Add(hitCounTextColumn);
        }
    }
}
