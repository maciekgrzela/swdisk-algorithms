using System;
using System.Collections.Generic;
using System.Threading;
using SWDISK_ALG.GeneticAlgorithmFiles;
using SWDISK_ALG.Helpers;
using SWDISK_ALG.Model;

namespace SWDISK_ALG
{
    public class GeneticAlgorithm
    {
        private readonly List<Coordinate> _coordinates;
        private readonly double[,] _throughputMatrix;
        public double Result { get; private set; }
        public List<Coordinate> ResultPath { get; private set; }

        public GeneticAlgorithm(List<Coordinate> coordinates, double[,] throughputMatrix)
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
            Config.MutationProbability = 0.01;
            Config.PopulationSize = Convert.ToInt32(Math.Floor(0.75 * Convert.ToDouble(_coordinates.Count)));
            Config.NumberOfCoordinates = _coordinates.Count;
            Config.NumberOfDominantsInNextGeneration =
                Convert.ToInt32(Math.Floor(0.25 * Convert.ToDouble(_coordinates.Count)));
            Config.ThroughputMatrix = _throughputMatrix;

            var thread = new GeneticAlgorithmThread(_coordinates);

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