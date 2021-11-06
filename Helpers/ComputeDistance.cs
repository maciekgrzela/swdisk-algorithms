using System;
using SWDISK_ALG.Model;

namespace SWDISK_ALG.Helpers
{
    public static class ComputeDistance
    {
        public static double ComputeDistanceAndThroughput(Coordinate coord1, Coordinate coord2, double[,] throughputMatrix)
        {
            return Math.Sqrt(Math.Pow(coord2.Latitude - coord1.Latitude, 2) + Math.Pow(coord2.Longitude - coord1.Longitude, 2)) *
                   (1 / throughputMatrix[coord1.Index, coord2.Index]);
        }
    }
}