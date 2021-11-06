using System;
using System.Collections.Generic;
using SWDISK_ALG.Model;

namespace SWDISK_ALG.Helpers
{
    public static class ReadData
    {
        public static (List<Coordinate>, double[,]) Read()
        {
            var returnedCoords = new List<Coordinate>();
            var coordsCount = 0;
            Console.WriteLine("Podaj liczbę koordynatów: ");
            coordsCount = Convert.ToInt32(Console.ReadLine());
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
    }
}