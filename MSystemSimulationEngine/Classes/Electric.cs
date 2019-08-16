using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSystemSimulationEngine.Classes
{
    static class Electric
    {
        /// <summary>
        /// Calculates charges of all tiles in a cTAM and re-colors tiles accordingly
        /// TODO generalize for 2D when formulas are available
        /// </summary>
        /// <param name="mSystem">M system which simulates a cTAM</param>
        public static void CalculateCharges(MSystem mSystem)
        {
            foreach (var tile in mSystem.SeedTiles)
            {
                CalculateLadder(tile, mSystem);
            }
        }



        /// <summary>
        /// For a cTAM 1D electric ladder, calculates charges of all tiles and re-colors tiles accordingly
        /// </summary>
        /// <param name="tile">Starting tile of the ladder</param>
        /// <param name="mSystem">M system which simulates a cTAM</param>
        private static void CalculateLadder(TileInSpace tile, MSystem mSystem)
        {
            var ladder = new List<TileInSpace>() {null};    // Element ladder[0] is unused, to agree with paper formulas 

            // The cycle must terminate as the tiles are passed in increasing X-coordinate order
            while (tile != null)
            {
                ladder.Add(tile);
                // Next tile to the east from the current one
                tile = tile.EastConnector?.ConnectedTo?.OnTile;
            }
            var t = ladder.Count - 1;
            if (t <= 0)
                return;

            var coefR = new double[t+1];
            coefR[t] = 1;
            var coefV = new double[t+1];
            coefV[0] = 1;

            for (int k = t-1; k >= 1; k--)
            {
                coefR[k] = 1 - 1/(coefR[k + 1] + ladder[k + 1].AlphaRatio + 1);
            }
            for (int k = 1; k <= t; k++)
            {
                tile = ladder[k];
                coefV[k] = coefV[k - 1]*coefR[k]/(coefR[k] + tile.AlphaRatio);
                tile.EastConnector.Voltage = mSystem.Nu0 * coefV[k];

                // Color change: lower voltage -> darker, but no less than 1/3 of the original light
                var coef = 0.75 / Math.Max(1 - mSystem.Tau / mSystem.Nu0, 0.01);
                var darkRatio = Math.Max((coefV[k] - 1) * coef + 1, 0);
                var origColor = ((Tile)tile).Color;
                tile.SetNewColor(origColor.A, (int)(origColor.R * darkRatio), (int)(origColor.G * darkRatio), (int)(origColor.B * darkRatio));
            }
        }
    }
}

// The following formulas were due to the paper "Harmonic Circuit Self-assembly in cTAM Models"
// by Yan, Garzon, Deaton (2019), Theorem 2

// double rho = -(2 + alpha + Math.Sqrt(alpha * (alpha + 4))) / 2;
// double coef = (rho * rho - 1) * (t - k + 1) / Math.Pow(Math.Abs(rho), k + 1); 

/*    double coef = 0;
for (int j = 0; j <= t-k; j++)
{
    coef += Math.Pow(-1, j)/(A(rho, k + j - 1)*A(rho, k + j));
}
coef *= Math.Pow(-1, k - 1)*A(rho, k - 1); 

private static double A(double rho, int k)
{
    return Math.Pow(rho, 2*k+2) / ((rho * rho - 1)* Math.Pow(rho, k));
}

*/

