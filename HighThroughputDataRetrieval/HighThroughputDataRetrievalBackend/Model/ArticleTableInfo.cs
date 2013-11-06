using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HighThroughputDataRetrievalBackend.Model
{
    public class ArticleTableInfo
    {
        public string ArticleTitle { get; set; }
        public string Url { get; set; }
        public string Author { get; set; }
        public int Year { get; set; }
        public string Journal { get; set; }
    }
}
