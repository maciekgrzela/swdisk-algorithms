using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Timers;
using SWDISK_ALG.Model;

namespace SWDISK_ALG.GeneticAlgorithmFiles
{
    public class GeneticAlgorithmThread
    {
        private Road _bestRoad;
        private static Timer _timer;
        private bool Timeout { get; set; }
        private List<Coordinate> Coordinates { get; }
        public List<Coordinate> BestCoordinates { get; }

        public GeneticAlgorithmThread(List<Coordinate> coordinates)
        {
            _bestRoad = null;
            Coordinates = new List<Coordinate>();
            BestCoordinates = new List<Coordinate>();
            PrepareCoordinates(coordinates);
            Timeout = false;
            _timer = new Timer(5000);
            _timer.Elapsed += (_, _) => Timeout = true;
            _timer.AutoReset = false;
        }

        private void PrepareCoordinates(IEnumerable<Coordinate> coordinates)
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
            var startSolution = new Road(Coordinates);
            var population = Population.Randomized(startSolution, Config.PopulationSize);
            var betterPopulationExists = true;
            
            _timer.Start();
            
            while (!Timeout)
            {
                if (betterPopulationExists)
                    SetBestRoad(population);
            
                betterPopulationExists = false;
                var oldFitnessFunctionVal = population.MaximumFitness;
            
                population = population.Evolve();
                if (population.MaximumFitness > oldFitnessFunctionVal)
                    betterPopulationExists = true;
            }

            PrepareBestRoadCoords();
        }

        private void PrepareBestRoadCoords()
        {
            var i = 0;
            if (_bestRoad == null) return;
            
            foreach (var bestCoord in _bestRoad.Coordinates.Select(coord => new Coordinate
            {
                Index = i,
                Latitude = coord.Latitude,
                Longitude = coord.Longitude
            }))
            {
                BestCoordinates.Add(bestCoord);
                i++;
            }
        }

        private void SetBestRoad(Population population)
        {
            var best = population.FindBest();
            _bestRoad = best;
        }
    }
}