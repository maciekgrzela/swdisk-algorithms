using SWDISK_ALG.Helpers;
using SWDISK_ALG.Model;

namespace SWDISK_ALG.AntColonyOptimizationFiles
{
    public class Edge
    {
        public Coordinate Start { get; }
        public Coordinate End { get; }
        public double Length { get; }
        public double Pheromone { get; set; }
        public double Weight { get; set; }

        public Edge() { }

        public Edge(Coordinate start, Coordinate end)
        {
            Start = start;
            End = end;
            Length = ComputeDistance.ComputeDistanceAndThroughput(start, end, Config.ThroughputMatrix);
        }
    }
}