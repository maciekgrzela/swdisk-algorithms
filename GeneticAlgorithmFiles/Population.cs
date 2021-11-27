using System;
using System.Collections.Generic;
using System.Linq;
using SWDISK_ALG.Helpers;

namespace SWDISK_ALG.GeneticAlgorithmFiles
{
    public class Population
    {
        private List<Road> Roads { get; set; }
        public double MaximumFitness { get; private set; }

        private Population(List<Road> roads)
        {
            Roads = roads;
            MaximumFitness = ComputeMaximumFitness();
        }

        public static Population Randomized(Road road, int times)
        {
            var newRoad = new List<Road>();

            for (var i = 0; i < times; ++i)
                newRoad.Add( road.Rearrange() );

            return new Population(newRoad);
        }

        private double ComputeMaximumFitness()
        {
            return Roads.Max(t => t.FitnessRatio);   
        }

        private Road Select()
        {
            while (true)
            {
                var index = RandomGenerator.GetRandomInt(0, Config.PopulationSize);

                if (RandomGenerator.Instance.Random.NextDouble() < Roads[index].FitnessRatio / MaximumFitness)
                    return new Road(Roads[index].Coordinates);
            }
        }

        private Population GenerateNewPopulation(int individuals)
        {
            var population = new List<Road>();

            for (var i = 0; i < individuals; ++i)
            {
                var road = Select().PerformCrossing(Select());

                foreach (var unused in road.Coordinates)
                    road = road.PerformMutation();

                population.Add(road);
            }

            return new Population(population);
        }

        private Population Elite(int dominants)
        {
            var best = new List<Road>();
            var population = new Population(Roads);

            for (var i = 0; i < dominants; ++i)
            {
                best.Add(population.FindBest());
                population = new Population(population.Roads.Except(best).ToList());
            }

            return new Population(best);
        }

        public Road FindBest()
        {
            return Roads.FirstOrDefault(t => Math.Abs(t.FitnessRatio - MaximumFitness) < double.Epsilon);
        }

        public Population Evolve()
        {
            var bestPopulation = Elite(Config.NumberOfDominantsInNextGeneration);
            var newPopulationGenerated = GenerateNewPopulation(Config.PopulationSize - Config.NumberOfDominantsInNextGeneration);
            return new Population(bestPopulation.Roads.Concat(newPopulationGenerated.Roads).ToList());
        }
    }
}