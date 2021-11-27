namespace SWDISK_ALG.SimulatedAnnealingFiles
{
    public static class Config
    {
        public static double StartingTemperature { get; set; }
        public static double CoolingRate { get; set; }
        public static double[,] ThroughputMatrix { get; set; }
    }
}