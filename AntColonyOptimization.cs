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
        
        public AntColonyOptimization(List<Coordinate> coordinates, double[,] throughputMatrix, double twoOptResult, int beta, double globalEvaporationRatio, double localEvaporationRatio, double q0, int antCount, double t0)
        {
            _coordinates = coordinates;
            _throughputMatrix = throughputMatrix;
            _twoOptResult = twoOptResult;
            
            try
            {
                (Result, ResultPath) = Compute(beta, globalEvaporationRatio, localEvaporationRatio, q0, antCount, t0);
            }
            catch (Exception e)
            {
                Result = -1;
                ResultPath = new List<Coordinate>();
                Console.WriteLine(e.Message);
            }
        }

        private (double, List<Coordinate>) Compute(int beta = 0, double globalEvaporationRatio = 0, double localEvaporationRatio = 0, double q0 = 0, int antCount = 0, double t0 = 0)
        {
            Config.Beta = beta == 0 ? 2 : beta;
            Config.GlobalEvaporationRatio = globalEvaporationRatio == 0 ? 0.1 : globalEvaporationRatio;
            Config.LocalEvaporationRatio = localEvaporationRatio == 0 ? 0.01 : localEvaporationRatio;
            Config.Q0 = q0 == 0 ? 0.9 : q0;
            Config.AntCount = antCount == 0 ? 20 : antCount;
            Config.T0 = t0 == 0 ? 1.0 / (_coordinates.Count * _twoOptResult) : t0;
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