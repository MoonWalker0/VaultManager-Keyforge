using System;
using System.Collections.Generic;
using System.Text;

namespace KeyForge.Core
{
    public class ELOCalculation
    {
        static double ExpectationToWin(int winnerRating, int loserRating)
        {
            return 1 / (1 + Math.Pow(10, (loserRating - winnerRating) / 400.0));
        }
        public static void CalculateELO(ref ELOTable winnerRating, ref ELOTable loserRating, int ELOFactor)
        {
            int delta = (int)(ELOFactor * (1 - ExpectationToWin(winnerRating.ELOScore, loserRating.ELOScore))); 

            winnerRating.ELOScore += (delta + (int)Math.Round((decimal)delta / 10, 0, MidpointRounding.AwayFromZero));
            loserRating.ELOScore -= delta;
        }
    }
}
