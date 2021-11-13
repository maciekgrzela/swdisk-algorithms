using System.Collections.Generic;
using System.Timers;
using SWDISK_ALG.Model;

namespace SWDISK_ALG.GeneticAlgorithm
{
    public class GeneticAlgorithmThread
    {
        private Road _bestRoad;
        private static Timer _timer;
        private bool Timeout { get; set; }
        private List<Coordinate> Coordinates { get; set; }
        private List<Coordinate> BestCoordinates { get; set; }

        public GeneticAlgorithmThread(List<Coordinate> coordinates)
        {
            _bestRoad = null;
            Coordinates = new List<Coordinate>();
            BestCoordinates = new List<Coordinate>();
            PrepareCoordinates(coordinates);
            Timeout = false;
            _timer = new Timer(60000);
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
            var startSolution = new Road(Coordinates);
            var population = Population.Randomized(startSolution, Config.PopulationSize);
            var better = true;
            
            _timer.Start();
            
            while (!Timeout)
            {
                if (better)
                    SetBestRoad(population);
            
                better = false;
                var oldFit = population.MaxFit;
            
                population = population.Evolve();
                if (population.MaxFit > oldFit)
                    better = true;
            }

            PrepareBestRoadCoords();
        }

        private void PrepareBestRoadCoords()
        {
            var i = 1;
            if (_bestRoad == null) return;
            
            foreach (var coord in _bestRoad.Coordinates)
            {
                var bestCoord = new Coordinate
                {
                    Index = i,
                    Latitude = coord.Latitude,
                    Longitude = coord.Longitude
                };
                BestCoordinates.Add(bestCoord);
                i++;
            }
        }

        private void SetBestRoad(Population p)
        {
            Road best = p.FindBest();
            _bestRoad = best;
        }
    }
}