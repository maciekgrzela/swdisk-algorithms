using System;
using System.Collections.Generic;
using System.Linq;
using SWDISK_ALG.Helpers;
using SWDISK_ALG.Model;

namespace SWDISK_ALG.GeneticAlgorithm
{
    public class Road
    {
        private double Distance { get; set; } = 0.0;
        public double FitnessRatio { get; set; }
        public List<Coordinate> Coordinates { get; set; }

        private static Random RandomGenerator { get; set; }


        public Road(List<Coordinate> coordinates)
        {
            Coordinates = coordinates;
            PrepareRoadParams();
            RandomGenerator = new Random();
        }

        private void PrepareRoadParams()
        {
            Distance = CalculateDistance();
            FitnessRatio = CalculateFitnessRatio();
        }

        private double CalculateDistance()
        {
            var totalDistance = 0.0;
            for (var i = 0; i < Coordinates.Count; i++)
            {
                var nextItemIndex = (i + 1) % Coordinates.Count;
                totalDistance += ComputeDistance.ComputeDistanceAndThroughput(Coordinates[i],
                    Coordinates[nextItemIndex], Config.ThroughputMatrix);
            }

            return totalDistance;
        }

        private double CalculateFitnessRatio()
        {
            if (Distance == 0)
            {
                Distance = CalculateDistance();
            }

            return 1.0 / Distance;
        }
        
        public override string ToString()
        {
            var roadString = Coordinates.Aggregate(string.Empty, (current, t) => current + $"{t} -> ");
            roadString += $"{Coordinates[0]}";
            return roadString;
        }

        public Road PerformMutation()
        {
            var coords = new List<Coordinate>(Coordinates);
            var prob = RandomGenerator.NextDouble();
            Road road = null;

            if (Config.MutationProbability > prob)
            {
                var swappedIndexOne = RandomGenerator.Next(0, Coordinates.Count);
                var swappedIndexTwo = RandomGenerator.Next(0, Coordinates.Count);

                var temp = coords[swappedIndexOne];
                coords[swappedIndexOne] = coords[swappedIndexTwo];
                coords[swappedIndexTwo] = temp;
            }
            
            road = new Road(coords);
            
            return road;
        }

        public Road PerformCrossing(Road road)
        {
            var i = RandomGenerator.Next(0, road.Coordinates.Count);
            var j = RandomGenerator.Next(i, road.Coordinates.Count);
            Road returnedRoad = null;
            
            var s = Coordinates.GetRange(i, j - i + 1);
            var ms = road.Coordinates.Except(s).ToList();
            var c = ms.Take(i)
                .Concat(s)
                .Concat( ms.Skip(i) )
                .ToList();
            
            returnedRoad = new Road(c);
            
            return returnedRoad;
        }

        public Road Rearrange()
        {
            var tmp = new List<Coordinate>(Coordinates);
            var n = tmp.Count;

            while (n > 1)
            {
                n--;
                var k = RandomGenerator.Next( n + 1);
                var v = tmp[k];
                tmp[k] = tmp[n];
                tmp[n] = v;
            }

            return new Road(tmp);
        }
    }
}