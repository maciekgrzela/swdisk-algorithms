using System;
using System.Collections.Generic;
using System.Linq;
using SWDISK_ALG.Model;

namespace SWDISK_ALG.GeneticAlgorithmFiles
{
    public class Population
    {
        private List<Road> p { get; set; }
        public double MaxFit { get; private set; }
        private static Random RandomGenerator { get; set; }

        private Population(List<Road> l)
        {
            p = l;
            MaxFit = CalcMaxFit();
            RandomGenerator = new Random();
        }

        public static Population Randomized(Road t, int n)
        {
            var tmp = new List<Road>();

            for (var i = 0; i < n; ++i)
                tmp.Add( t.Rearrange() );

            return new Population(tmp);
        }

        private double CalcMaxFit() => p.Max( t => t.FitnessRatio );

        private Road Select()
        {
            while (true)
            {
                var i = RandomGenerator.Next(0, Config.PopulationSize);

                if (RandomGenerator.NextDouble() < p[i].FitnessRatio / MaxFit)
                    return new Road(p[i].Coordinates);
            }
        }

        private Population GenNewPop(int n)
        {
            var p = new List<Road>();

            for (var i = 0; i < n; ++i)
            {
                var t = Select().PerformCrossing( Select() );

                foreach (var unused in t.Coordinates)
                    t = t.PerformMutation();

                p.Add(t);
            }

            return new Population(p);
        }

        private Population Elite(int n)
        {
            var best = new List<Road>();
            var tmp = new Population(p);

            for (var i = 0; i < n; ++i)
            {
                best.Add( tmp.FindBest() );
                tmp = new Population( tmp.p.Except(best).ToList() );
            }

            return new Population(best);
        }

        public Road FindBest()
        {
            return p.FirstOrDefault(t => Math.Abs(t.FitnessRatio - this.MaxFit) < double.Epsilon);
        }

        public Population Evolve()
        {
            var best = this.Elite(Config.NumberOfDominantsInNextGeneration);
            var np = this.GenNewPop(Config.PopulationSize - Config.NumberOfDominantsInNextGeneration);
            return new Population( best.p.Concat(np.p).ToList() );
        }
    }
}