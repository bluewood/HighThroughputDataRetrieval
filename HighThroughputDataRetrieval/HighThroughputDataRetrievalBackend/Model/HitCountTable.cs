using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighThroughputDataRetrievalBackend.Model
{
    public class HitCountTable
    {
        #region Data members
        public int CountInHitCountTable { get; set; }
        public string ProteinInHitCountTable { get; set; }
        #endregion

        #region Constructor

        public HitCountTable(int newCount, string newProtein)
        {
            CountInHitCountTable = newCount;
            ProteinInHitCountTable = newProtein;
        }

        #endregion
    }
}
