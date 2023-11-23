using GAF.Operators;
using GAF;
using System;
using GAF.Threading;
using System.Collections.Generic;

namespace GUIdition.Genetics
{
    public class SwapPairMutationOperator : MutateBase, IGeneticOperator
    {
        public SwapPairMutationOperator(double mutationProbability)
            : base(mutationProbability)
        {
            base.RequiresEvaluatedPopulation = false;
        }

        internal static List<int> GetSwapPoints(Chromosome chromosome)
        {
            List<int> list = new List<int>();

            int pair1StartIndex = RandomProvider.GetThreadRandom().Next(chromosome.Genes.Count - 1);
            int pair2StartIndex = RandomProvider.GetThreadRandom().Next(chromosome.Genes.Count - 1);

            while (pair1StartIndex % 2 != 0)
            {
                pair1StartIndex = RandomProvider.GetThreadRandom().Next(chromosome.Genes.Count - 1);
            }

            while (pair1StartIndex == pair2StartIndex || pair2StartIndex % 2 != 0)
            {
                pair2StartIndex = RandomProvider.GetThreadRandom().Next(chromosome.Genes.Count - 1);
            }

            list.Add(pair1StartIndex);
            list.Add(pair2StartIndex);

            return list;
        }

        private void Mutate(Chromosome chromosome, int indexOfFirstPair, int indexOfSecondPair)
        {
            Gene firstGene = chromosome.Genes[indexOfFirstPair];
            Gene secondGene = chromosome.Genes[indexOfFirstPair + 1];

            chromosome.Genes[indexOfFirstPair] = chromosome.Genes[indexOfSecondPair];
            chromosome.Genes[indexOfFirstPair + 1] = chromosome.Genes[indexOfSecondPair + 1];
            chromosome.Genes[indexOfSecondPair] = firstGene;
            chromosome.Genes[indexOfSecondPair + 1] = secondGene;
        }

        protected override void Mutate(Chromosome child)
        {
            if (child == null || child.Genes == null)
            {
                throw new ArgumentException("The Chromosome is either null or the Chromosomes Genes are null.");
            }

            if (!(RandomProvider.GetThreadRandom().NextDouble() <= MutationProbability))
            {
                return;
            }

            List<int> swapPoints = GetSwapPoints(child);
            Mutate(child, swapPoints[0], swapPoints[1]);
        }

        protected override void MutateGene(Gene gene)
        {
            throw new NotImplementedException("This operator does not mutate individual genes.");
        }
    }
}
