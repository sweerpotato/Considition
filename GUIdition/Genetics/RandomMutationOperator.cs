using GAF;
using GAF.Operators;
using GAF.Threading;
using System;

namespace GUIdition.Genetics
{
    public class RandomMutationOperator : MutateBase, IGeneticOperator
    {
        private Random _Random = new();

        public RandomMutationOperator(double mutationProbability)
            : base(mutationProbability)
        {

        }

        protected override void MutateGene(Gene gene)
        {
            if (gene.GeneType == GeneType.Object)
            {
                throw new OperatorException("Genes with a GeneType of Object cannot be mutated by the BinaryMutate operator.");
            }

            gene.ObjectValue = _Random.Next(0, 3);

            /*switch (gene.GeneType)
            {
                case GeneType.Binary:
                    gene.ObjectValue = Ran
                    break;
                case GeneType.Real:
                    gene.ObjectValue = (double)gene.ObjectValue * -1.0;
                    break;
                case GeneType.Integer:
                    gene.ObjectValue = (int)gene.ObjectValue * -1;
                    break;
            }*/
        }
    }
}
