using GAF.Operators;
using GAF;
using System.Collections.Generic;
using GAF.Extensions;
using System.Linq;
using System;
using System.Diagnostics;
using GAF.Threading;

namespace GUIdition.Genetics
{
    public class CustomCrossover : CrossoverBase, IGeneticOperator
    {
        #region Constructors

        internal CustomCrossover()
            : this(1.0)
        {
        }

        public CustomCrossover(double crossOverProbability)
            : this(crossOverProbability, allowDuplicates: true)
        {
        }

        public CustomCrossover(double crossOverProbability, bool allowDuplicates)
            : this(crossOverProbability, allowDuplicates, CrossoverType.SinglePoint)
        {
        }

        public CustomCrossover(double crossOverProbability, bool allowDuplicates, CrossoverType crossoverType)
            : this(crossOverProbability, allowDuplicates, crossoverType, ReplacementMethod.GenerationalReplacement)
        {
        }

        public CustomCrossover(double crossOverProbability, bool allowDuplicates, CrossoverType crossoverType, ReplacementMethod replacementMethod)
            : base(crossOverProbability, allowDuplicates, crossoverType, replacementMethod)
        {
        }

        #endregion

        protected override void PerformCrossoverDoublePoint(Chromosome p1, Chromosome p2, CrossoverData crossoverData, out List<Gene> cg1, out List<Gene> cg2)
        {
            if (crossoverData.Points == null || crossoverData.Points.Count < 2)
            {
                throw new ArgumentException("The CrossoverData.Points property is either null or is missing the required crossover points.");
            }

            /*int num = crossoverData.Points[0];
            int num2 = crossoverData.Points[1];*/
            int chromosomeLength = crossoverData.ChromosomeLength;

            //num   = 3
            //num2  = 7
            // 0 0 0 0 0 0 0 0
            // 1 1 1 1 1 1 1 1
            // 0 0 0 1 1 1 1 0  cg1
            //                  cg2
            /*
             *  2 1 0 2 2 1 2 2 2 0 0 1 0 1 2 0 0 0 1 0 1 0 2 1 0 0 0 0 2 2 2 0 1 0 2 0 0 0 0 0 0 2 2 2 1 1 1 1 1 1 2 1 0 0 2 1 1 1 1 2 1 0 1 0 2 1 1 0 0 2 2 2 0 2 1 1 1 2
                1 0 0 1 2 1 0 1 2 2 1 1 2 0 1 2 1 1 0 1 1 0 0 0 1 2 1 1 0 1 2 0 1 2 0 1 0 2 0 0 2 2 0 0 0 2 0 1 2 2 1 2 2 1 0 2 0 1 0 1 2 1 2 2 2 1 2 1 0 1 0 2 2 2 1 1 1 2
                
                p1, p2
                2 1 0 2 2 1 2 2 2 0 0 1 0 1 2 0 0 1 0 1 1 0 0 0 1 2 1 1 0 1 2 0 1 2 0 1 0 2 0 0 2 2 0 0 0 2 0 1 2 2 1 2 2 0 2 1 1 1 1 2 1 0 1 0 2 1 1 0 0 2 2 2 0 2 1 1 1 2
                1 0 0 1 2 1 0 1 2 2 1 1 2 0 1 2 1 0 1 0 1 0 2 1 0 0 0 0 2 2 2 0 1 0 2 0 0 0 0 0 0 2 2 2 1 1 1 1 1 1 2 1 0 1 0 2 0 1 0 1 2 1 2 2 2 1 2 1 0 1 0 2 2 2 1 1 1 2
             */
            /*cg1.AddRangeCloned(p1.Genes.Take(num).ToList());
            cg1.AddRangeCloned(p2.Genes.Skip(num).Take(num2 - num));
            cg1.AddRangeCloned(p1.Genes.Skip(num2).Take(chromosomeLength - num2));
            cg2.AddRangeCloned(p2.Genes.Take(num).ToList());
            cg2.AddRangeCloned(p1.Genes.Skip(num).Take(num2 - num));
            cg2.AddRangeCloned(p2.Genes.Skip(num2).Take(chromosomeLength - num2));*/
            //Debug.WriteLine($"{num} {num2}");
            /* 0 0 1 1 1 0 1 0
             * 0 1 1 0 0 0 1 1 
             * 0 0 1 1 0 0 1 1 
             * 0 1 1 0 1 0 1 0
             */
            int randomInt = RandomProvider.GetThreadRandom().Next(0, 10);

            /*if (randomInt == 0)
            {
                cg1 = new List<Gene>(new Gene[chromosomeLength]);
                cg2 = new List<Gene>(new Gene[chromosomeLength]);
                //Skapa barn med jämna index från 1 och udda index från 2
                for (int i = 0; i != chromosomeLength; i += 2)
                {
                    cg1[i] = p1.Genes[i];
                    cg1[i + 1] = p2.Genes[i + 1];
                    cg2[i] = p2.Genes[i];
                    cg2[i + 1] = p1.Genes[i + 1];
                }
            }
            else if (randomInt == 1)
            {
                cg1 = new List<Gene>(new Gene[chromosomeLength]);
                cg2 = new List<Gene>(new Gene[chromosomeLength]);
                //Skapa barn med udda index från 1 och jämna index från 2
                for (int i = 0; i != chromosomeLength; i += 2)
                {
                    cg1[i + 1] = p1.Genes[i + 1];
                    cg1[i] = p2.Genes[i];
                    cg2[i + 1] = p2.Genes[i + 1];
                    cg2[i] = p1.Genes[i];
                }
            }
            else if (randomInt == 2)
            {
                cg1 = new List<Gene>();
                cg2 = new List<Gene>();
                int num = crossoverData.Points[0];
                cg1.AddRangeCloned(p1.Genes.Take(num).ToList());
                cg1.AddRangeCloned(p2.Genes.Skip(num).Take(chromosomeLength - num));
                cg2.AddRangeCloned(p2.Genes.Take(num).ToList());
                cg2.AddRangeCloned(p1.Genes.Skip(num).Take(chromosomeLength - num));
            }
            else
            {*/
                int rnum;
                int rnum2;
                do
                {
                    rnum = RandomProvider.GetThreadRandom().Next(0, chromosomeLength);
                    rnum2 = RandomProvider.GetThreadRandom().Next(0, chromosomeLength);
                }
                while (rnum == rnum2 && rnum % 2 == 0 && rnum2 % 2 == 0);

                int num = System.Math.Min(rnum, rnum2);
                int num2 = System.Math.Max(rnum, rnum2);

                cg1 = new List<Gene>();
                cg2 = new List<Gene>();
                //int num = crossoverData.Points[0];
                //int num2 = crossoverData.Points[1];
                cg1.AddRangeCloned(p1.Genes.Take(num).ToList());
                cg1.AddRangeCloned(p2.Genes.Skip(num).Take(num2 - num));
                cg1.AddRangeCloned(p1.Genes.Skip(num2).Take(chromosomeLength - num2));
                cg2.AddRangeCloned(p2.Genes.Take(num).ToList());
                cg2.AddRangeCloned(p1.Genes.Skip(num).Take(num2 - num));
                cg2.AddRangeCloned(p2.Genes.Skip(num2).Take(chromosomeLength - num2));
            //}

        }

        protected override void PerformCrossoverDoublePointOrdered(Chromosome p1, Chromosome p2, CrossoverData crossoverData, out List<Gene> cg1, out List<Gene> cg2)
        {
            if (crossoverData.Points == null || crossoverData.Points.Count < 2)
            {
                throw new ArgumentException("The CrossoverData.Points property is either null or is missing the required crossover points.");
            }

            int num = crossoverData.Points[0];
            int num2 = crossoverData.Points[1];
            cg1 = new List<Gene>();
            cg2 = new List<Gene>();
            cg1.AddRangeCloned(p1.Genes.Skip(num).Take(num2 - num));
            cg2.AddRangeCloned(p2.Genes.Skip(num).Take(num2 - num));
            HashSet<object> hashSet = new HashSet<object>();
            HashSet<object> hashSet2 = new HashSet<object>();
            foreach (Gene gene in p1.Genes)
            {
                hashSet.Add(gene.ObjectValue);
            }

            foreach (Gene item in cg1)
            {
                hashSet2.Add(item.ObjectValue);
            }

            foreach (Gene gene2 in p2.Genes)
            {
                bool num3 = hashSet.Contains(gene2.ObjectValue);
                bool flag = hashSet2.Contains(gene2.ObjectValue);
                if (num3 && !flag)
                {
                    cg1.AddCloned(gene2);
                }
            }

            HashSet<object> hashSet3 = new HashSet<object>();
            HashSet<object> hashSet4 = new HashSet<object>();
            foreach (Gene gene3 in p2.Genes)
            {
                hashSet3.Add(gene3.ObjectValue);
            }

            foreach (Gene item2 in cg2)
            {
                hashSet4.Add(item2.ObjectValue);
            }

            foreach (Gene gene4 in p1.Genes)
            {
                bool num4 = hashSet3.Contains(gene4.ObjectValue);
                bool flag2 = hashSet4.Contains(gene4.ObjectValue);
                if (num4 && !flag2)
                {
                    cg2.AddCloned(gene4);
                }
            }

            if (cg1.Count != p1.Count || cg2.Count != p2.Count)
            {
                throw new CrossoverTypeIncompatibleException("The parent Chromosomes were not suitable for Ordered Crossover as they do not contain the same set of values. Consider using a different crossover type, or ensure all solutions are build with the same set of values.");
            }
        }

        protected override void PerformCrossoverSinglePoint(Chromosome p1, Chromosome p2, CrossoverData crossoverData, out List<Gene> cg1, out List<Gene> cg2)
        {
            if (crossoverData.Points == null || crossoverData.Points.Count < 1)
            {
                throw new ArgumentException("The CrossoverData.Points property is either null or is missing the required crossover points.");
            }

            cg1 = new List<Gene>();
            cg2 = new List<Gene>();
            int num = crossoverData.Points[0];
            int chromosomeLength = crossoverData.ChromosomeLength;
            cg1.AddRangeCloned(p1.Genes.Take(num).ToList());
            cg1.AddRangeCloned(p2.Genes.Skip(num).Take(chromosomeLength - num));
            cg2.AddRangeCloned(p2.Genes.Take(num).ToList());
            cg2.AddRangeCloned(p1.Genes.Skip(num).Take(chromosomeLength - num));
        }
    }
}
