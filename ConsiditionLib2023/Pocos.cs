using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ConsiditionLib2023
{
    public class SubmitSolution
    {
        public required Dictionary<string, PlacedLocations> Locations
        {
            get; set;
        }
        public double Longitude
        {
            get; set;
        }
        public string LocationType { get; set; } = string.Empty;
    }

    public class PlacedLocations
    {
        [Range(0, 2)]
        public required int Freestyle9100Count { get; set; } = -1;

        [Range(0, 2)]
        public required int Freestyle3100Count { get; set; } = -1;
        public double Latitude
        {
            get; set;
        }
        public double Longitude
        {
            get; set;
        }
        public string LocationType { get; set; } = string.Empty;
    }




    public class MapData
    {
        public required string MapName
        {
            get; set;
        }
        public required Border Border
        {
            get; set;
        }
        public required Dictionary<string, StoreLocation> Locations
        {
            get; set;
        }
        public required List<Hotspot> Hotspots
        {
            get; set;
        }
        public required Dictionary<string, int> LocationTypeCount
        {
            get; set;
        }
        public required DateTime AvailableFrom
        {
            get; set;
        }
        public required DateTime AvailableTo
        {
            get; set;
        }
    }

    public class StoreLocation
    {
        public required string LocationName
        {
            get; set;
        }
        public required string LocationType
        {
            get; set;
        }
        public double Latitude
        {
            get; set;
        }
        public double Longitude
        {
            get; set;
        }
        public double Footfall
        {
            get; set;
        }
        public int FootfallScale
        {
            get; set;
        }
        public double SalesVolume
        {
            get; set;
        }
    }

    public class Border
    {
        public double LatitudeMax
        {
            get; set;
        }
        public double LatitudeMin
        {
            get; set;
        }
        public double LongitudeMax
        {
            get; set;
        }
        public double LongitudeMin
        {
            get; set;
        }
    }

    public class Hotspot
    {
        public double Spread
        {
            get; set;
        }
        public string Name { get; set; } = string.Empty;
        public double Latitude
        {
            get; set;
        }
        public double Longitude
        {
            get; set;
        }
        public double Footfall
        {
            get; set;
        }
    }


    public class GeneralData
    {
        public required ContainerData ClassicUnitData
        {
            get; set;
        }
        public required ContainerData RefillUnitData
        {
            get; set;
        }
        public required RefillMachineData Freestyle9100Data
        {
            get; set;
        }
        public required RefillMachineData Freestyle3100Data
        {
            get; set;
        }
        public required Dictionary<string, LocationType> LocationTypes
        {
            get; set;
        }
        //public required LocationType GroceryStoreLarge { get; set; }
        // public required LocationType GroceryStore { get; set; }
        // public required LocationType Convenience { get; set; }
        // public required LocationType GasStation { get; set; }
        // public required LocationType Kiosk { get; set; }

        public required List<string> CompetitionMapNames
        {
            get; set;
        }
        public required List<string> TrainingMapNames
        {
            get; set;
        }

        [Range(0, double.MaxValue)]
        public required double Co2PricePerKiloInSek { get; set; } = -1;
        [Range(0, double.MaxValue)]
        public required double WillingnessToTravelInMeters { get; set; } = -1;
        [Range(0, double.MaxValue)]
        public required double ConstantExpDistributionFunction { get; set; } = -1;
        [Range(0, double.MaxValue)]
        public required double RefillSalesFactor { get; set; } = -1;
        [Range(0, double.MaxValue)]
        public required double RefillDistributionRate { get; set; } = -1;
    }

    public class ContainerData
    {
        public required string Type { get; set; } = string.Empty;
        [Range(0, double.MaxValue)]
        public required double Co2PerUnitInGrams { get; set; } = -1;
        [Range(0, double.MaxValue)]
        public required double ProfitPerUnit { get; set; } = -1;
    }

    public class RefillMachineData
    {
        public string Type { get; set; } = string.Empty;
        [Range(0, double.MaxValue)]
        public required double LeasingCostPerWeek { get; set; } = -1;
        [Range(0, double.MaxValue)]
        public required double RefillCapacityPerWeek { get; set; } = -1;
        [Range(0, double.MaxValue)]
        public required double StaticCo2 { get; set; } = -1;
    }

    public class LocationType
    {
        public required string Type { get; set; } = string.Empty;
        [Range(0, double.MaxValue)]
        public required double SalesVolume { get; set; } = -1;
    }



    public class GameData
    {
        public Guid Id
        {
            get; set;
        }
        public string MapName { get; set; } = string.Empty;
        public Score? GameScore
        {
            get; set;
        }
        public Guid TeamId
        {
            get; set;
        }
        public string TeamName { get; set; } = string.Empty;
        public int TotalFreestyle9100Count { get; set; } = 0;
        public int TotalFreestyle3100Count { get; set; } = 0;
        public double TotalLeasingCost { get; set; } = 0;
        public double TotalRevenue { get; set; } = 0;
        public Dictionary<string, StoreLocationScoring>? Locations
        {
            get; set;
        }

        public override string ToString()
        {
            return $"Score total: {GameScore.Total}\n" +
                $"CO2 Total: {GameScore.KgCo2Savings}\n" +
                $"Unprofitable locations: {Locations.Where(pair => !pair.Value.IsProfitable).Count()}\n" +
                $"Non-saving CO2 locations: {Locations.Where(pair => !pair.Value.IsCo2Saving).Count()}";
        }
    }

    public class StoreLocationScoring
    {
        public string LocationName { get; set; } = string.Empty;
        public string LocationType { get; set; } = string.Empty;
        public double Latitude
        {
            get; set;
        }
        public double Longitude
        {
            get; set;
        }
        public double Footfall
        {
            get; set;
        }
        public int FootfallScale
        {
            get; set;
        }
        public double SalesVolume
        {
            get; set;
        }
        public double SalesCapacity
        {
            get; set;
        }
        public double LeasingCost
        {
            get; set;
        }
        public double Revenue
        {
            get; set;
        }
        public double Earnings
        {
            get; set;
        }
        public int Freestyle9100Count
        {
            get; set;
        }
        public int Freestyle3100Count
        {
            get; set;
        }
        public double GramCo2Savings
        {
            get; set;
        }
        public bool IsProfitable { get; set; } = false;
        public bool IsCo2Saving { get; set; } = false;
    }

    public class Score
    {
        public double KgCo2Savings { get; set; } = 0;
        public double Earnings { get; set; } = 0;
        public double TotalFootfall { get; set; } = 0;
        public double Total { get; set; } = 0;
    }

}