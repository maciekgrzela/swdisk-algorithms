using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Timers;
using SWDISK_ALG.Helpers;
using SWDISK_ALG.Model;

namespace SWDISK_ALG.SimulatedAnnealingFiles
{
    public class SimulatedAnnealingThread
    {
        private static Timer _timer;
        private bool Timeout { get; set; }
        private List<Coordinate> Coordinates { get; set; }
        public List<Coordinate> BestCoordinates { get; private set; }
        

        public SimulatedAnnealingThread(List<Coordinate> coordinates)
        {
            Coordinates = new List<Coordinate>();
            BestCoordinates = new List<Coordinate>();
            PrepareCoordinates(coordinates);
            Timeout = false;
            
            _timer = new Timer(5000);
            _timer.Elapsed += (_, _) => Timeout = true;
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

        [SuppressMessage("ReSharper.DPA", "DPA0001: Memory allocation issues")]
        public void Run()
        {
            var distanceForCoordinatesLoaded = ComputeDistance.CalculateCost(Coordinates, Config.ThroughputMatrix);
            var temperature = Config.StartingTemperature;
            
            _timer.Start();
            
            while (!Timeout)
            {
                var nextCoordinates = ComputeNextRoadBasedOnPrevious(Coordinates);
                var solutionDifference = ComputeDistance.CalculateCost(nextCoordinates, Config.ThroughputMatrix) - distanceForCoordinatesLoaded;

                if (solutionDifference < 0 || solutionDifference > 0 &&
                    Math.Exp(-solutionDifference / temperature) > RandomGenerator.Instance.Random.NextDouble())
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

        private static List<Coordinate> ComputeNextRoadBasedOnPrevious(List<Coordinate> previousCoordinates)
        {
            var firstRandomIndex = RandomGenerator.GetRandomInt(1, previousCoordinates.Count);
            var secondRandomIndex = RandomGenerator.GetRandomInt(1, previousCoordinates.Count);

            var temp = previousCoordinates[firstRandomIndex];
            previousCoordinates[firstRandomIndex] = previousCoordinates[secondRandomIndex];
            previousCoordinates[secondRandomIndex] = temp;

            return previousCoordinates;
        }
    }
}