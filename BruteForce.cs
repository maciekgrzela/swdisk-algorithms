using System;
using System.Collections.Generic;
using System.Linq;
using SWDISK_ALG.Helpers;
using SWDISK_ALG.Model;

namespace SWDISK_ALG
{
    public class BruteForce
    {
        private static List<List<Coordinate>> _permutations;
        private readonly double[,] _throughputMatrix;
        private double _minimalCost;
        public double Result { get; private set; }

        public BruteForce(List<Coordinate> coordinates, double[,] throughputMatrix)
        {
            _permutations = new List<List<Coordinate>>();
            _minimalCost = Double.MaxValue;
            _throughputMatrix = throughputMatrix;
            GeneratePermutations(coordinates);
            Result = Compute();
        }

        private static void SwapCoordinates(ref List<Coordinate> list, int a, int b)
        {
            var tmp = list[a];
            list[a] = list[b];
            list[b] = tmp;
        }
        
        private void GeneratePermutations(List<Coordinate> coordinates)
        {
            var currentPermutation = new Coordinate[coordinates.Count];
            var selected = new bool[coordinates.Count];

            var results = new List<List<Coordinate>>();

            PermuteItems(coordinates, selected,
                currentPermutation, results, 0);

            _permutations = results;
        }
        
        private void PermuteItems(List<Coordinate> coordinates, bool[] selected,
            Coordinate[] currentPermutation, List<List<Coordinate>> results,
            int nextPosition)
        {
            if (nextPosition == coordinates.Count)
            {
                results.Add(currentPermutation.ToList());
            }
            else
            {
                for (var i = 0; i < coordinates.Count; i++)
                {
                    if (selected[i]) continue;
                    
                    selected[i] = true;
                    currentPermutation[nextPosition] = coordinates[i];

                    PermuteItems(coordinates, selected,
                        currentPermutation, results,
                        nextPosition + 1);

                    selected[i] = false;
                }
            }
        }
        
        private double Compute()
        {
            foreach (var cost in _permutations.Select(t => CalculateCost(t)))
            {
                if (cost < _minimalCost)
                {
                    _minimalCost = cost;
                }
            }

            return _minimalCost;
        }

        private double CalculateCost(List<Coordinate> permutation)
        {
            var cost = 0.0;
            for (var i = 0; i < permutation.Count - 1; i++)
            {
                cost += ComputeDistance.ComputeDistanceAndThroughput(permutation[i], permutation[i + 1], _throughputMatrix);
            }
            cost += ComputeDistance.ComputeDistanceAndThroughput(permutation[^1], permutation[0], _throughputMatrix);

            return cost;
        }
    }
}