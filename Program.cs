using System;
using System.Collections.Generic;
using SWDISK_ALG.Helpers;
using SWDISK_ALG.Model;

namespace SWDISK_ALG
{
    class Program
    {
        private static List<Coordinate> _coordinates;
        private static double[,] _throughputMatrix;
        
        static void Main(string[] args)
        {
            (_coordinates, _throughputMatrix) = ReadData.Read();
            var bruteForce = new BruteForce(_coordinates, _throughputMatrix);
            var nearestNeighbor = new NearestNeighbour(_coordinates, _throughputMatrix);
            var geneticAlgorithm = new GeneticAlgorithm(_coordinates, _throughputMatrix);
            var acoAlgorithm = new AntColonyOptimization(_coordinates, _throughputMatrix, nearestNeighbor.Result);
            
            Console.WriteLine(bruteForce.Result);
            bruteForce.ResultPath.VisualizePath();
            
            Console.WriteLine(nearestNeighbor.Result);
            nearestNeighbor.ResultPath.VisualizePath();
            
            Console.WriteLine(geneticAlgorithm.Result);
            geneticAlgorithm.ResultPath.VisualizePath();
            
            Console.WriteLine(acoAlgorithm.Result);
            acoAlgorithm.ResultPath.VisualizePath();
        }
    }
}