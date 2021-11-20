using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using SWDISK_ALG.GeneticAlgorithmFiles;
using SWDISK_ALG.Helpers;
using SWDISK_ALG.Model;

namespace SWDISK_ALG.SimulatedAnnealingFiles
{
    public class SimulatedAnnealingThread
    {
        private Random _random;
        private static Timer _timer;
        private bool Timeout { get; set; }
        private List<Coordinate> Coordinates { get; set; }
        public List<Coordinate> BestCoordinates { get; set; }
        

        public SimulatedAnnealingThread(List<Coordinate> coordinates)
        {
            _random = new Random();
            Coordinates = new List<Coordinate>();
            BestCoordinates = new List<Coordinate>();
            PrepareCoordinates(coordinates);
            Timeout = false;
            
            _timer = new Timer(5000);
            _timer.Elapsed += (sender, args) => Timeout = true;
            _timer.AutoReset = false;
        }

        private void PrepareCoordinates(List<Coordinate> coordinates)
        {
            var i = 0;
            foreach (var coordinate in coordinates)
            {
                var coord = new Coordinate
                {
                    Index = i,
                    Latitude = coordinate.Latitude,
                    Longitude = coordinate.Longitude
                };
                Coordinates.Add(coord);
                i++;
            }
        }

        public void Run()
        {
            var distanceForCoordinatesLoaded = ComputeDistance.CalculateCost(Coordinates, Config.ThroughputMatrix);
            List<Coordinate> nextCoordinates = null;
            var solutionDifference = 0.0;
            var temperature = Config.StartingTemperature;
            
            _timer.Start();
            
            while (!Timeout)
            {
                nextCoordinates = ComputeNextRoadBasedOnPrevious(Coordinates);
                solutionDifference = ComputeDistance.CalculateCost(nextCoordinates, Config.ThroughputMatrix) - distanceForCoordinatesLoaded;

                if (solutionDifference < 0 || solutionDifference > 0 &&
                    Math.Exp(-solutionDifference / temperature) > _random.NextDouble())
                {
                    for (var i = 0; i < nextCoordinates.Count; i++)
                    {
                        Coordinates[i] = nextCoordinates[i];
                    }

                    distanceForCoordinatesLoaded += solutionDifference;
                }

                temperature *= Config.CoolingRate;
            }
            
            BestCoordinates = Coordinates;
        }

        private List<Coordinate> ComputeNextRoadBasedOnPrevious(List<Coordinate> previousCoordinates)
        {
            var nextCoords = previousCoordinates.ToList();

            var firstRandomIndex = _random.Next(1, nextCoords.Count);
            var secondRandomIndex = _random.Next(1, nextCoords.Count);

            var temp = nextCoords[firstRandomIndex];
            nextCoords[firstRandomIndex] = nextCoords[secondRandomIndex];
            nextCoords[secondRandomIndex] = temp;

            return nextCoords;
        }
    }
}