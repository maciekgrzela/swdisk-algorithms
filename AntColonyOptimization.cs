using System;
using System.Collections.Generic;
using System.Threading;
using SWDISK_ALG.AntColonyOptimizationFiles;
using SWDISK_ALG.Helpers;
using SWDISK_ALG.Model;

namespace SWDISK_ALG
{
    public class AntColonyOptimization
    {
        private readonly List<Coordinate> _coordinates;
        private readonly double[,] _throughputMatrix;
        private readonly double _twoOptResult;
        public double Result { get; private set; }
        public List<Coordinate> ResultPath { get; private set; }
        
        public AntColonyOptimization(List<Coordinate> coordinates, double[,] throughputMatrix, double twoOptResult)
        {
            _coordinates = coordinates;
            _throughputMatrix = throughputMatrix;
            _twoOptResult = twoOptResult;
            
            try
            {
                (Result, ResultPath) = Compute();
            }
            catch (Exception e)
            {
                Result = -1;
                ResultPath = new List<Coordinate>();
                Console.WriteLine(e.Message);
            }
        }

        private (double, List<Coordinate>) Compute()
        {
            Config.Beta = 2;
            Config.GlobalEvaporationRatio = 0.1;
            Config.LocalEvaporationRatio = 0.01;
            Config.Q0 = 0.9;
            Config.AntCount = 20;
            Config.Iterations = 10000;
            Config.T0 = 1.0 / (_coordinates.Count * _twoOptResult);
            Config.ThroughputMatrix = _throughputMatrix;

            var graph = new Graph(_coordinates, Config.T0);

            var thread = new AntColonyOptimizationThread(_coordinates, graph);

            var runnableThread = new Thread(thread.Run);
            
            runnableThread.Start();
            runnableThread.Join();

            if (thread.BestCoordinates.Count == 0)
            {
                throw new Exception("Unable to find optimal route");
            }

            return (ComputeDistance.CalculateCost(thread.BestCoordinates, _throughputMatrix), thread.BestCoordinates);
        }
    }
}