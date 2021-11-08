using System;
using System.Collections.Generic;
using SWDISK_ALG.Model;

namespace SWDISK_ALG.Helpers
{
    public static class ListExtension
    {
        public static void VisualizePath(this List<Coordinate> list)
        {
            foreach (var coord in list)
            {
                Console.Write($"{coord.Index} -> ");
            }
            
            Console.Write($"{list[0].Index}");
            Console.WriteLine();
        }
    }
}