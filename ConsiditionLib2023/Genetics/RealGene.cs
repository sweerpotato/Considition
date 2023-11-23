using System.ComponentModel.DataAnnotations;

namespace ConsiditionLib2023.Genetics
{
    [Serializable]
    public class RealGene
    {
        [Range(0, 5)]
        public required int Freestyle9100Count { get; set; } = 0b0;

        [Range(0, 5)]
        public required int Freestyle3100Count { get; set; } = 0b0;

        public override bool Equals(object? obj)
        {
            return Equals(obj as RealGene);
        }

        protected bool Equals(RealGene? other)
        {
            if (other == null)
            {
                return false;
            }

            return Freestyle3100Count == other.Freestyle3100Count &&
                Freestyle9100Count == other.Freestyle9100Count;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Freestyle9100Count, Freestyle3100Count);
        }
    }
}
