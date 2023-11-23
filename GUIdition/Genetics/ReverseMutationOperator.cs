using GAF;
using GAF.Operators;
using GAF.Threading;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GUIdition.Genetics
{
    public class ReverseMutationOperator : MutateBase, IGeneticOperator
    {
        public ReverseMutationOperator(double mutationProbability)
            : base(mutationProbability)
        {

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

            List<Gene> reverseGenes = new(((IEnumerable<Gene>)child.Genes).Reverse().ToList());
            child.Genes.Clear();
            child.Genes.AddRange(reverseGenes);
        }

        protected override void MutateGene(Gene gene)
        {
            throw new NotImplementedException("This operator does not mutate individual genes.");
        }
    }
}
