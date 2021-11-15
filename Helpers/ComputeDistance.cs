using System;
using System.Collections.Generic;
using SWDISK_ALG.Model;

namespace SWDISK_ALG.Helpers
{
    public static class ComputeDistance
    {
        public static double CalculateCost(List<Coordinate> coordinates, double[,] throughputMatrix)
        {
            var cost = 0.0;
            for (var i = 0; i < coordinates.Count - 1; i++)
            {
                cost += ComputeDistanceAndThroughput(coordinates[i], coordinates[i + 1], throughputMatrix);
            }
            cost += ComputeDistanceAndThroughput(coordinates[^1], coordinates[0], throughputMatrix);

            return cost;
        }
        
        public static double ComputeDistanceAndThroughput(Coordinate coord1, Coordinate coord2, double[,] throughputMatrix)
        {
            return Math.Sqrt(Math.Pow(coord2.Latitude - coord1.Latitude, 2) + Math.Pow(coord2.Longitude - coord1.Longitude, 2)) *
                   (1 / throughputMatrix[coord1.Index, coord2.Index]);
        }
    }
}