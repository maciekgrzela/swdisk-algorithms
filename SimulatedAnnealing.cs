using System;
using System.Collections.Generic;
using System.Threading;
using SWDISK_ALG.Helpers;
using SWDISK_ALG.Model;
using SWDISK_ALG.SimulatedAnnealingFiles;

namespace SWDISK_ALG
{
    public class SimulatedAnnealing
    {
        private readonly List<Coordinate> _coordinates;
        private readonly double[,] _throughputMatrix;
        public double Result { get; private set; }
        public List<Coordinate> ResultPath { get; private set; }

        public SimulatedAnnealing(List<Coordinate> coordinates, double[,] throughputMatrix)
        {
            _coordinates = coordinates;
            _throughputMatrix = throughputMatrix;
            
            try
            {
                (Result, ResultPath) = Compute();
            }
            catch (Exception e)
            {
                Result = -1;
                ResultPath = new List<Coordinate>();
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        private (double, List<Coordinate>) Compute()
        {
            Config.StartingTemperature = 10000.0;
            Config.CoolingRate = 0.9999;
            Config.ThroughputMatrix = _throughputMatrix;

            var thread = new SimulatedAnnealingThread(_coordinates);

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