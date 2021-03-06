namespace SWDISK_ALG.GeneticAlgorithmFiles
{
    public static class Config
    {
        public static double MutationProbability { get; set; }
        public static int NumberOfCoordinates { get; set; }
        public static int NumberOfDominantsInNextGeneration { get; set; }
        public static int PopulationSize { get; set; }
        public static double[,] ThroughputMatrix { get; set; }
    }

    public class OverridenConfig
    {
        public double MutationProbability { get; set; }
        public int NumberOfDominantsInNextGeneration { get; set; }
        public int PopulationSize { get; set; }
    }
}