using System;
using System.Collections.Generic;
using System.Linq;

namespace SWDISK_ALG.Helpers
{
    public class RandomGenerator
    {
        public static RandomGenerator Instance { get; } = new();
        public Random Random { get; }

        private RandomGenerator() => Random = new Random();

        public static double GetDoubleRangeRandomNumber(double minimum, double maximum)
        {
            return Instance.Random.NextDouble() * (maximum - minimum) + minimum;
        }

        public static int GetRandomInt(int minimum, int maximum)
        {
            return Instance.Random.Next(minimum, maximum);
        }

        public static IEnumerable<int> GenerateRandom(int count, int min, int max)
        {
            return Enumerable.Range(min, max).OrderBy(_ => Instance.Random.Next()).Take(count).ToList();
        }
    }
}