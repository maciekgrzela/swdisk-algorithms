using System;
using System.Collections.Generic;
using System.Linq;
using SWDISK_ALG.Model;

namespace SWDISK_ALG.Helpers
{
    public static class ListExtension
    {
        public static void VisualizePath(this List<Coordinate> list)
        {
            if (list.Count > 0)
            {
                foreach (var coord in list)
                {
                    Console.Write($"{coord.Index} -> ");
                }
            
                Console.Write($"{list[0].Index}");
                Console.WriteLine();
                return;
            }
            
            Console.WriteLine("Empty coordinates list");
        }
        
        public static string ReturnPath(this List<Coordinate> list)
        {
            var path = string.Empty;
            
            if (list.Count > 0)
            {
                path = list.Aggregate(path, (current, coord) => current + $"{coord.Index} -> ");
                path += $"{list[0].Index}";
            }

            return path == string.Empty ? "Empty coordinates list" : path;
        }
    }
}