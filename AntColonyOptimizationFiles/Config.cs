namespace SWDISK_ALG.AntColonyOptimizationFiles
{
    public static class Config
    {
        public static int Beta { get; set; }
        public static double GlobalEvaporationRatio { get; set; }
        public static double LocalEvaporationRatio { get; set; }
        public static double Q0 { get; set; }
        public static double T0 { get; set; }
        public static int AntCount { get; set; }
        public static double[,] ThroughputMatrix { get; set; }
    }
    
    public class OverridenConfig
    {
        public int Beta { get; set; }
        public double GlobalEvaporationRatio { get; set; }
        public double LocalEvaporationRatio { get; set; }
        public double Q0 { get; set; }
        public double T0 { get; set; }
        public int AntCount { get; set; }
    }
}