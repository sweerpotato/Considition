using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace ConsiditionLib2023.Genetics
{
    public class LocationGene
    {
        public required string LocationName
        {
            get; set;
        }

        public required double Latitude
        {
            get; set;
        }

        public required double Longitude
        {
            get; set;
        }

        public required RealGene RealGene
        {
            get; set;
        }

        public KeyValuePair<string, PlacedLocations> ToSolutionFriendly()
        {
            return new KeyValuePair<string, PlacedLocations>(
                LocationName,
                new PlacedLocations()
                {
                    Freestyle3100Count = RealGene.Freestyle3100Count,
                    Freestyle9100Count = RealGene.Freestyle9100Count
                });
        }
    }
}
