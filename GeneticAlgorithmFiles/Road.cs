using System.Collections.Generic;
using System.Linq;
using SWDISK_ALG.Helpers;
using SWDISK_ALG.Model;

namespace SWDISK_ALG.GeneticAlgorithmFiles
{
    public class Road
    {
        private double Distance { get; set; }
        public double FitnessRatio { get; private set; }
        public List<Coordinate> Coordinates { get; }

        public Road(List<Coordinate> coordinates)
        {
            Coordinates = coordinates;
            PrepareRoadParams();
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

        public Road PerformMutation()
        {
            var coords = new List<Coordinate>(Coordinates);
            var mutationProbability = RandomGenerator.Instance.Random.NextDouble();

            if (Config.MutationProbability > mutationProbability)
            {
                var swappedIndexOne =  RandomGenerator.GetRandomInt(0, Coordinates.Count);
                var swappedIndexTwo = RandomGenerator.GetRandomInt(0, Coordinates.Count);

                var temp = coords[swappedIndexOne];
                coords[swappedIndexOne] = coords[swappedIndexTwo];
                coords[swappedIndexTwo] = temp;
            }
            
            var road = new Road(coords);
            
            return road;
        }

        public Road PerformCrossing(Road road)
        {
            var firstPoint = RandomGenerator.GetRandomInt(0, road.Coordinates.Count);
            var secondPoint = RandomGenerator.GetRandomInt(firstPoint, road.Coordinates.Count);

            var firstPart = Coordinates.GetRange(firstPoint, secondPoint - firstPoint + 1);
            var exceptFirstPart = road.Coordinates.Except(firstPart).ToList();
            var permutation = exceptFirstPart.Take(firstPoint)
                .Concat(firstPart)
                .Concat( exceptFirstPart.Skip(firstPoint) )
                .ToList();
            
            var returnedRoad = new Road(permutation);
            
            return returnedRoad;
        }

        public Road Rearrange()
        {
            var coords = new List<Coordinate>(Coordinates);
            var coordsCount = coords.Count;

            while (coordsCount > 1)
            {
                coordsCount--;
                var first = RandomGenerator.Instance.Random.Next(coordsCount + 1);
                var v = coords[first];
                coords[first] = coords[coordsCount];
                coords[coordsCount] = v;
            }

            return new Road(coords);
        }
    }
}