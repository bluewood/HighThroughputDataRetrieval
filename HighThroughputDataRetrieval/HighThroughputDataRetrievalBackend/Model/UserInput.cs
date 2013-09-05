using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighThroughputDataRetrievalBackend.Model
{
    public class UserInput
    {
        #region Fields
        private string _proteinID;
        private string _organism;
        private string _keyword;
        #endregion // Fields

        #region Properties
        /// <summary>
        /// Protein identifiers
        /// </summary>
        public string ProteinID
        {
            get { return _proteinID; }
            set { _proteinID = value; }
        }

        /// <summary>
        /// Organisms
        /// </summary>
        public string Organism
        {
            get { return _organism; }
            set { _organism = value; }
        }

        /// <summary>
        /// Keywords
        /// </summary>
        public string Keyword
        {
            get { return _keyword; }
            set { _keyword = value; }
        }
        #endregion // Properties
    }
}
