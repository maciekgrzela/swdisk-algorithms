using System;
using System.Collections.Generic;
using SWDISK_ALG.Helpers;
using SWDISK_ALG.Model;

namespace SWDISK_ALG.AntColonyOptimizationFiles
{
    public class Graph
    {
        public List<Coordinate> Coordinates { get; }
        private Dictionary<string, Edge> Edges { get; }
        public int Dimensions { get; }
        private double MinimumPheromone { get; }
        
        public Graph(List<Coordinate> coordinates, double minimumPheromone)
        {
            Edges = new Dictionary<string, Edge>();
            Coordinates = new List<Coordinate>();
            PrepareCoordinates(coordinates);
            MinimumPheromone = minimumPheromone;
            Dimensions = Coordinates.Count;
            ConstructEdges();
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
        
        private void ConstructEdges()
        {
            for (var i = 0; i < Coordinates.Count; i++)
            {
                for (var j = 0; j < Coordinates.Count; j++)
                {
                    if (i == j) continue;
                    
                    var edge = new Edge(Coordinates[i], Coordinates[j]);
                    Edges.Add(EdgesHelper.GenerateName(Coordinates[i], Coordinates[j]), edge);
                }
            }
        }
        
        public Edge GetEdge(int firstPointId, int secondPointId)
        {
            return Edges[EdgesHelper.GenerateName(firstPointId, secondPointId)];
        }

        public void ResetPheromone(double pheromoneValue)
        {
            foreach (var edge in Edges)
            {
                edge.Value.Pheromone = pheromoneValue;
            }
        }

        public void EvaporatePheromone(Edge edge, double value)
        {
            edge.Pheromone = Math.Max(MinimumPheromone, edge.Pheromone * value);
            var secondEdge = GetEdge(edge.End.Index, edge.Start.Index);
            secondEdge.Pheromone = Math.Max(MinimumPheromone, secondEdge.Pheromone * value);
        }

        public void DepositPheromone(Edge edge, double value)
        {
            edge.Pheromone += value;
            var secondEdge = GetEdge(edge.End.Index, edge.Start.Index);
            secondEdge.Pheromone += value;
        }
    }
}