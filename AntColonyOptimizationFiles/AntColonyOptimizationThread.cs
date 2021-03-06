using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using SWDISK_ALG.Helpers;
using SWDISK_ALG.Model;

namespace SWDISK_ALG.AntColonyOptimizationFiles
{
    public class AntColonyOptimizationThread
    {
        private static Timer _timer;
        private bool Timeout { get; set; }
        private List<Coordinate> Coordinates { get; }
        public List<Coordinate> BestCoordinates { get; private set; }
        private Graph Graph { get; set; }
        private Ant GlobalBestAnt { get; set; }

        public AntColonyOptimizationThread(List<Coordinate> coordinates, Graph graph)
        {
            Graph = graph;
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

        public void Run()
        {
            Graph.ResetPheromone(Config.T0);
            
            _timer.Start();
            
            while (!Timeout)
            {
                var antColony = CreateAnts();
                GlobalBestAnt ??= antColony[0];

                var localBestAnt = BuildTours(antColony);
                if (Math.Round(localBestAnt.Distance, 2) < Math.Round(GlobalBestAnt.Distance, 2))
                {
                    GlobalBestAnt = localBestAnt;
                }
            }

            BestCoordinates = GlobalBestAnt.VisitedCoordinates;
        }

        private List<Ant> CreateAnts()
        {
            var antColony = new List<Ant>();
            var randomPoints = RandomGenerator.GenerateRandom(Config.AntCount, 0, Graph.Dimensions - 1);
            foreach (var random in randomPoints)
            {
                var ant = new Ant(Graph, Config.Beta, Config.Q0);
                ant.Init(random);
                antColony.Add(ant);
            }
            return antColony;
        }

        private Ant BuildTours(List<Ant> antColony)
        {
            for (var i = 0; i < Graph.Dimensions; i++)
            {
                foreach (var ant in antColony)
                {
                    var edge = ant.Move();
                    LocalUpdate(edge);
                }
            }

            GlobalUpdate();

            return antColony.OrderBy(x => x.Distance).FirstOrDefault();
        }

        private void LocalUpdate(Edge edge)
        {
            var evaporate = 1 - Config.LocalEvaporationRatio;
            Graph.EvaporatePheromone(edge, evaporate);

            var deposit = Config.LocalEvaporationRatio * Config.T0;
            Graph.DepositPheromone(edge, deposit);
        }

        private void GlobalUpdate()
        {
            var deltaR = 1 / GlobalBestAnt.Distance;
            foreach (var edge in GlobalBestAnt.Road)
            {
                var evaporate = (1 - Config.GlobalEvaporationRatio);
                Graph.EvaporatePheromone(edge, evaporate);

                var deposit = Config.GlobalEvaporationRatio * deltaR;
                Graph.DepositPheromone(edge, deposit);
            }
        }
    }
}