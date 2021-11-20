using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using SWDISK_ALG.AntColonyOptimizationFiles;
using SWDISK_ALG.Model;

namespace SWDISK_ALG.Helpers
{
    public static class EdgesHelper
    {
        public static string GenerateName(Coordinate coordinate1, Coordinate coordinate2)
        {
            return BitConverter.ToString(new SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes($"{coordinate1.Index}to{coordinate2.Index}")))
                .Replace("-", string.Empty);
        }
        
        public static string GenerateName(int index1, int index2)
        {
            return BitConverter.ToString(new SHA256Managed().ComputeHash(Encoding.UTF8.GetBytes($"{index1}to{index2}")))
                .Replace("-", string.Empty);
        }
        
        public static List<Edge> EdgeCumulativeSum(List<Edge> sequence)
        {
            double sum = 0;
            foreach (var item in sequence)
            {
                sum += item.Weight;
                item.Weight = sum;
            }

            return sequence;
        }
        
        public static Coordinate GetRandomEdge(IEnumerable<Edge> cumSum)
        {
            var random = RandomGenerator.Instance.Random.NextDouble();
            return cumSum.First(j => j.Weight >= random).End;
        }
    }
}