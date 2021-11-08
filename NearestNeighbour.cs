using System;
using System.Collections.Generic;
using System.Linq;
using SWDISK_ALG.Helpers;
using SWDISK_ALG.Model;

namespace SWDISK_ALG
{
    public class NearestNeighbour
    {
        private List<Coordinate> _coordinates;
        private readonly double[,] _throughputMatrix;
        public double Result { get; private set; }
        public List<Coordinate> ResultPath { get; private set; }

        public NearestNeighbour(List<Coordinate> coordinates, double[,] throughputMatrix)
        {
            _coordinates = coordinates;
            _throughputMatrix = throughputMatrix;
            (Result, ResultPath) = Compute();
        }

        private (double, List<Coordinate>) Compute()
        {
            var minimalPath = new List<Coordinate>();
            minimalPath.Add(_coordinates.First());
            var minimalResult = 0.0;
            var visited = new int[_coordinates.Count];
            visited[0] = 1;
            var lastVisited = 0;

            for (var i = 0; i < _coordinates.Count - 1; i++)
            {
                var unvisitedNeighbors = _coordinates.Where(p => p.Index != _coordinates[lastVisited].Index && visited[p.Index] == 0).ToList();
                var minimalDistanceToNeighbors = double.MaxValue;
                Coordinate nearestNeighbor = unvisitedNeighbors.First();
                
                foreach (var neighbor in unvisitedNeighbors)
                {
                    var distanceToNeighbor = ComputeDistance.ComputeDistanceAndThroughput(_coordinates[lastVisited], neighbor, _throughputMatrix);
                    if (!(distanceToNeighbor < minimalDistanceToNeighbors)) continue;
                    
                    minimalDistanceToNeighbors = distanceToNeighbor;
                    nearestNeighbor = neighbor;
                }


                minimalResult += minimalDistanceToNeighbors;
                minimalPath.Add(nearestNeighbor);
                visited[nearestNeighbor.Index] = 1;
                lastVisited = nearestNeighbor.Index;
            }

            minimalResult +=
                ComputeDistance.ComputeDistanceAndThroughput(_coordinates[lastVisited], _coordinates[0],
                    _throughputMatrix);

            return (minimalResult, minimalPath);
        }
    }
}