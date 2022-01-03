using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SWDISK_ALG.Model;

namespace SWDISK_ALG.Helpers
{
    public static class ReadData
    {
        public static (List<Coordinate>, double[,]) Read()
        {
            var returnedCoords = new List<Coordinate>();
            Console.WriteLine("Podaj liczbę koordynatów: ");
            var coordsCount = Convert.ToInt32(Console.ReadLine());
            var returnedThroughputMatrix = new double[coordsCount, coordsCount]; 

            for (var i = 0; i < coordsCount; i++)
            {
                var coordsStr = Console.ReadLine() ?? "0,0";
                var coords = coordsStr.Split(',');
                returnedCoords.Add(new Coordinate
                {
                    Index = i,
                    Latitude = int.Parse(coords[0]),
                    Longitude = int.Parse(coords[1])
                });
            }
            
            
            Console.WriteLine("Zdefiniuj macierz przepustowości dla koordynatów o następującej strukturze");

            for (var i = 0; i < coordsCount; i++)
            {
                for (var j = 0; j < coordsCount; j++)
                {
                    Console.Write($"p{i}{j}\t");
                }
                Console.WriteLine();
            }
            
            Console.WriteLine();

            for (var i = 0; i < coordsCount; i++)
            {
                var row = Console.ReadLine() ?? "";
                var throughput = Array.ConvertAll(row.Split("\t"), double.Parse);

                for (var j = 0; j < throughput.Length; j++)
                {
                    returnedThroughputMatrix[i, j] = throughput[j];
                }
            }

            return (returnedCoords, returnedThroughputMatrix);
        }
        
        public static (List<Coordinate>, double[,]) ReadFromFile(string fileName)
        {
            var returnedCoords = new List<Coordinate>();
            var coordsCount = 0;
            
            foreach (var line in File.ReadLines($"../../../scripts/SWDISK_TXT/{fileName}.tsp_transformed.txt"))
            {
                if (line == "")
                {
                    break;
                }
                
                var coords = line.Split(' ');

                returnedCoords.Add(new Coordinate
                {
                    Index = coordsCount,
                    Latitude = double.Parse(coords[0].Replace(".", ",")),
                    Longitude = double.Parse(coords[1].Replace(".", ","))
                });

                coordsCount++;
            }
            
            var returnedThroughputMatrix = new double[coordsCount, coordsCount];

            var i = 0;
            
            foreach (var line in File.ReadLines($"../../../scripts/SWDISK_TXT/{fileName}.tsp_transformed.txt").Skip(coordsCount + 3))
            {
                var throughput = Array.ConvertAll(line.Replace(".", ",").Split("\t"), double.Parse);

                for (var j = 0; j < throughput.Length; j++)
                {
                    returnedThroughputMatrix[i, j] = throughput[j];
                }
                
                i++;
            }

            return (returnedCoords, returnedThroughputMatrix);
        }
    }
}