using System;
using System.Collections.Generic;
using System.Text;

namespace KeyForge.Core
{
    public class vHistory
    { 
        public string Tournament { get; set; }
        public string PlayerWin { get; set; }
        public string PlayerLose { get; set; }
        public int WinIncrease { get; set; }
        public int LoseDecrease { get; set; }
        public int WinFinalELO { get; set; }
        public int LoseFinalELO { get; set; }
        public DateTime Date { get; set; }
    }
}
