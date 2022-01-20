using System;
using System.Collections.Generic;
using SWDISK_ALG.GeneticAlgorithmFiles;
using SWDISK_ALG.Helpers;
using SWDISK_ALG.Model;

namespace SWDISK_ALG
{
    class Program
    {
        private static List<Coordinate> _coordinates;
        private static double[,] _throughputMatrix;

        static void Main(string[] args)
        {
            ReadUserInput();
            
            Console.WriteLine("Wprowadź liczbę powtórzeń wykonania każdego z algorytmów:");
            var times = int.Parse(Console.ReadLine() ?? string.Empty);

            var bruteForceResults = new List<AlgorithmResult>();
            var nearestNeighborResults = new List<AlgorithmResult>();
            var geneticResults = new List<AlgorithmResult>();
            var simulatedAnnealingResults = new List<AlgorithmResult>();
            var acoResults = new List<AlgorithmResult>();

            var bruteForceSelected = _coordinates.Count <= 10;
            
            Console.WriteLine("Hiperparametry dla GeneticAlgorithm:");
            Console.WriteLine("1.   Domyślne");
            Console.WriteLine("2.   Wprowadź ręcznie");
            var paramsForGeneticAlgRes = int.Parse(Console.ReadLine() ?? string.Empty);

            OverridenConfig paramsForGeneticAlg = null;
            SimulatedAnnealingFiles.OverridenConfig paramsForSA = null;
            AntColonyOptimizationFiles.OverridenConfig paramsForAco = null;
            

            if (paramsForGeneticAlgRes == 2)
            {
                paramsForGeneticAlg = new OverridenConfig();
                
                Console.WriteLine(nameof(Config.MutationProbability));
                paramsForGeneticAlg.MutationProbability = double.Parse(Console.ReadLine() ?? string.Empty);
                
                Console.WriteLine(nameof(Config.PopulationSize));
                paramsForGeneticAlg.PopulationSize = int.Parse(Console.ReadLine() ?? string.Empty);
                
                Console.WriteLine(nameof(Config.NumberOfDominantsInNextGeneration));
                paramsForGeneticAlg.NumberOfDominantsInNextGeneration = int.Parse(Console.ReadLine() ?? string.Empty);
            }
            
            Console.WriteLine("Hiperparametry dla SimulatedAnnealing:");
            Console.WriteLine("1.   Domyślne");
            Console.WriteLine("2.   Wprowadź ręcznie");
            var paramsForSARes = int.Parse(Console.ReadLine() ?? string.Empty);

            if (paramsForSARes == 2)
            {
                paramsForSA = new SimulatedAnnealingFiles.OverridenConfig();
                Console.WriteLine(nameof(paramsForSA.CoolingRate));
                paramsForSA.CoolingRate = double.Parse(Console.ReadLine() ?? string.Empty);
                
                Console.WriteLine(nameof(paramsForSA.StartingTemperature));
                paramsForSA.StartingTemperature = double.Parse(Console.ReadLine() ?? string.Empty);
            }
            
            Console.WriteLine("Hiperparametry dla GeneticAlgorithm:");
            Console.WriteLine("1.   Domyślne");
            Console.WriteLine("2.   Wprowadź ręcznie");
            var paramsForAcoRes = int.Parse(Console.ReadLine() ?? string.Empty);

            if (paramsForAcoRes == 2)
            {
                paramsForAco = new AntColonyOptimizationFiles.OverridenConfig();
                
                Console.WriteLine(nameof(paramsForAco.Beta));
                paramsForAco.Beta = int.Parse(Console.ReadLine() ?? string.Empty);
                
                Console.WriteLine(nameof(paramsForAco.Q0));
                paramsForAco.Q0 = double.Parse(Console.ReadLine() ?? string.Empty);
                
                Console.WriteLine(nameof(paramsForAco.T0));
                paramsForAco.T0 = double.Parse(Console.ReadLine() ?? string.Empty);
                
                Console.WriteLine(nameof(paramsForAco.AntCount));
                paramsForAco.AntCount = int.Parse(Console.ReadLine() ?? string.Empty);
                
                Console.WriteLine(nameof(paramsForAco.GlobalEvaporationRatio));
                paramsForAco.GlobalEvaporationRatio = double.Parse(Console.ReadLine() ?? string.Empty);
                
                Console.WriteLine(nameof(paramsForAco.LocalEvaporationRatio));
                paramsForAco.LocalEvaporationRatio = double.Parse(Console.ReadLine() ?? string.Empty);
            }

            for (var i = 0; i < times; i++)
            {
                if (bruteForceSelected)
                {
                    var bruteForce = new BruteForce(_coordinates, _throughputMatrix);

                    bruteForceResults.Add(new AlgorithmResult
                    {
                        Score = bruteForce.Result,
                        Path = bruteForce.ResultPath.ReturnPath()
                    });
                }

                GeneticAlgorithm geneticAlgorithm = null;
                SimulatedAnnealing simulatedAnnealingAlgorithm = null;
                AntColonyOptimization antColonyOptimizationAlgorithm = null;

                var nearestNeighbor = new NearestNeighbour(_coordinates, _throughputMatrix);
                
                geneticAlgorithm = paramsForGeneticAlg != null ? new GeneticAlgorithm(_coordinates, _throughputMatrix, paramsForGeneticAlg.MutationProbability, paramsForGeneticAlg.PopulationSize, paramsForGeneticAlg.NumberOfDominantsInNextGeneration) : new GeneticAlgorithm(_coordinates, _throughputMatrix);
                simulatedAnnealingAlgorithm = paramsForSA != null ? new SimulatedAnnealing(_coordinates, _throughputMatrix, paramsForSA.StartingTemperature, paramsForSA.CoolingRate) : new SimulatedAnnealing(_coordinates, _throughputMatrix);
                antColonyOptimizationAlgorithm = paramsForAco != null ? new AntColonyOptimization(_coordinates, _throughputMatrix, nearestNeighbor.Result, paramsForAco.Beta, paramsForAco.GlobalEvaporationRatio, paramsForAco.LocalEvaporationRatio, paramsForAco.Q0, paramsForAco.AntCount, paramsForAco.T0) : new AntColonyOptimization(_coordinates, _throughputMatrix, nearestNeighbor.Result);

                nearestNeighborResults.Add(new AlgorithmResult
                {
                    Score = nearestNeighbor.Result,
                    Path = nearestNeighbor.ResultPath.ReturnPath()
                });

                geneticResults.Add(new AlgorithmResult
                {
                    Score = geneticAlgorithm.Result,
                    Path = geneticAlgorithm.ResultPath.ReturnPath()
                });

                simulatedAnnealingResults.Add(new AlgorithmResult
                {
                    Score = simulatedAnnealingAlgorithm.Result,
                    Path = simulatedAnnealingAlgorithm.ResultPath.ReturnPath()
                });

                acoResults.Add(new AlgorithmResult
                {
                    Score = antColonyOptimizationAlgorithm.Result,
                    Path = antColonyOptimizationAlgorithm.ResultPath.ReturnPath()
                });
            }

            if (bruteForceSelected)
            {
                Console.WriteLine($"Scores for: {nameof(BruteForce)}");
                foreach (var result in bruteForceResults)
                {
                    Console.Write(result.Score + " ");
                }
            }
            else
            {
                Console.WriteLine("Pomijam wyniki dla algorytmu BruteForce");
            }

            Console.WriteLine();
            Console.WriteLine($"Scores for: {nameof(NearestNeighbour)}");
            foreach (var result in nearestNeighborResults)
            {
                Console.Write(result.Score + " ");
            }

            Console.WriteLine();
            Console.WriteLine($"Scores for: {nameof(GeneticAlgorithm)}");
            foreach (var result in geneticResults)
            {
                Console.Write(result.Score + " ");
            }

            Console.WriteLine();
            Console.WriteLine($"Scores for: {nameof(SimulatedAnnealing)}");
            foreach (var result in simulatedAnnealingResults)
            {
                Console.Write(result.Score + " ");
            }
            
            Console.WriteLine();
            Console.WriteLine($"Scores for: {nameof(AntColonyOptimization)}");
            foreach (var result in acoResults)
            {
                Console.Write(result.Score + " ");
            }
        }

        static void ReadUserInput()
        {
            int res;

            do
            {
                Console.WriteLine("SWDISK Symulator");
                Console.WriteLine("Wybierz tryb wprowadzania danych");
                Console.WriteLine("1.   Z przetworzonego pliku tsp");
                Console.WriteLine("2.   Wprowadzanie ręczne");
                res = int.Parse(Console.ReadLine() ?? string.Empty);
            } while (res != 1 && res != 2);

            if (res == 1)
            {
                Console.WriteLine("Wprowadź nazwę pliku: ");
                (_coordinates, _throughputMatrix) = ReadData.ReadFromFile(Console.ReadLine());
            }

            if (res == 2)
            {
                (_coordinates, _throughputMatrix) = ReadData.Read();
            }
        }
    }
}