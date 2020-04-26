using System;
using System.Collections.Generic;
using System.Text;

namespace KeyForge.Core
{
    public class Postdata
    {
        public string Frame { get; set; }
        public string Scanned { get; set; }
        public int WinnerID { get; set; }
        public int LoserID { get; set; }
        public string WinnerName { get; set; }
        public string LoserName { get; set; }
    }
    public class PostdataLesser
    {
        public string Frame { get; set; } 
        public int PlayerID { get; set; } 
        public string PlayerName { get; set; } 
    }
}
