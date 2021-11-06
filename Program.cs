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
            Console.WriteLine(bruteForce.Result);
        }
    }
}