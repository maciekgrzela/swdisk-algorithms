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
        
        public SimulatedAnnealing(List<Coordinate> coordinates, double[,] throughputMatrix, double startTemp, double coolingRate)
        {
            _coordinates = coordinates;
            _throughputMatrix = throughputMatrix;
            
            try
            {
                (Result, ResultPath) = Compute(startTemp, coolingRate);
            }
            catch (Exception e)
            {
                Result = -1;
                ResultPath = new List<Coordinate>();
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }

        private (double, List<Coordinate>) Compute(double startTemp = 0, double coolingRate = 0)
        {
            Config.StartingTemperature = startTemp == 0 ? 10000.0 : startTemp;
            Config.CoolingRate = coolingRate == 0 ? 0.990 : coolingRate;
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