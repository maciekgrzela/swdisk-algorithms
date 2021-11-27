using System;
using System.Collections.Generic;
using System.Linq;
using SWDISK_ALG.Helpers;
using SWDISK_ALG.Model;

namespace SWDISK_ALG.AntColonyOptimizationFiles
{
    public class Ant
    {
        private Graph Graph { get; }
        private int Beta { get; }
        private double Q0 { get; }
        public double Distance { get; private set; }
        public List<Coordinate> VisitedCoordinates { get; }
        private List<Coordinate> UnvisitedCoordinates { get; set; }
        public List<Edge> Road { get; }
        
        
        public Ant(Graph graph, int beta, double q0)
        {
            Graph = graph;
            Beta = beta;
            Q0 = q0;
            VisitedCoordinates = new List<Coordinate>();
            UnvisitedCoordinates = new List<Coordinate>();
            Road = new List<Edge>();
        }
        
        public void Init(int startNodeIndex)
        {
            Distance = 0;
            VisitedCoordinates.Add(Graph.Coordinates.First(x => x.Index == startNodeIndex));
            UnvisitedCoordinates = Graph.Coordinates.Where(x => x.Index != startNodeIndex).ToList();
            Road.Clear();
        }

        private Coordinate CurrentCoordinate()
        {
            return VisitedCoordinates[^1];
        }

        public Edge Move()
        {
            Coordinate endCoordinate;
            var startPoint = CurrentCoordinate();

            if (UnvisitedCoordinates.Count == 0)
            {
                endCoordinate = VisitedCoordinates[0];
            }
            else
            {
                endCoordinate = ChooseNextPoint();
                VisitedCoordinates.Add(endCoordinate);
                UnvisitedCoordinates.RemoveAt(UnvisitedCoordinates.FindIndex(x => x.Index == endCoordinate.Index));
            }

            var edge = Graph.GetEdge(startPoint.Index, endCoordinate.Index);
            Road.Add(edge);
            Distance += edge.Length;
            return edge;
        }
        
        private Coordinate ChooseNextPoint()
        {
            var edgesWithWeight = new List<Edge>();
            var bestEdge = new Edge();
            var currentNodeId = CurrentCoordinate().Index;

            foreach (var node in UnvisitedCoordinates)
            {
                var edge = Graph.GetEdge(currentNodeId, node.Index);
                edge.Weight = Weight(edge);

                if (edge.Weight > bestEdge.Weight)
                {
                    bestEdge = edge;
                }

                edgesWithWeight.Add(edge);
            }

            var random = RandomGenerator.Instance.Random.NextDouble();
            return random < Q0 ? Exploitation(bestEdge) : Exploration(edgesWithWeight);
        }
        
        private double Weight(Edge edge)
        {
            var heuristic = 1 / edge.Length;
            return edge.Pheromone * Math.Pow(heuristic, Beta);
        }

        private Coordinate Exploitation(Edge bestEdge)
        {
            return bestEdge.End;
        }

        private Coordinate Exploration(List<Edge> edgesWithWeight)
        {
            var totalSum = edgesWithWeight.Sum(x => x.Weight);
            var edgeProbabilities = edgesWithWeight.Select(w => { w.Weight = (w.Weight / totalSum); return w; }).ToList();
            var cumSum = EdgesHelper.EdgeCumulativeSum(edgeProbabilities);
            var chosenPoint = EdgesHelper.GetRandomEdge(cumSum);

            return chosenPoint;
        }
    }
}