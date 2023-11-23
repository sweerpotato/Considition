using ConsiditionLib2023;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsiditionLib2023
{
    internal static class Scoring
    {
        public static List<string> SandBoxMaps { get; } = new List<string> { "s-sandbox", "g-sandbox" };

        public static GameData CalculateScore(string mapName, SubmitSolution solution, MapData mapEntity, GeneralData generalData)
        {
            GameData scoredSolution = new()
            {
                MapName = mapName,
                TeamId = Guid.Empty,
                TeamName = string.Empty,
                Locations = new(),
                GameScore = new()
            };

            List<string> sandboxMaps = SandBoxMaps;
            if (sandboxMaps.Contains(mapName) == false)
            {
                //Separate locations on the map into dict for those that have a refill station and those who have not.
                Dictionary<string, StoreLocationScoring> locationListNoRefillStation = new();
                //Dictionary<string, StoreLocationScoring> locationListWithRefillStation = new();
                foreach (KeyValuePair<string, StoreLocation> kvp in mapEntity.Locations)
                {
                    if (solution.Locations.ContainsKey(kvp.Key) == true)
                    {
                        scoredSolution.Locations[kvp.Key] = new()
                        {
                            LocationName = kvp.Value.LocationName,
                            LocationType = kvp.Value.LocationType,
                            Latitude = kvp.Value.Latitude,
                            Longitude = kvp.Value.Longitude,
                            Footfall = kvp.Value.Footfall,
                            FootfallScale = kvp.Value.FootfallScale,
                            Freestyle3100Count = solution.Locations[kvp.Key].Freestyle3100Count,
                            Freestyle9100Count = solution.Locations[kvp.Key].Freestyle9100Count,

                            SalesVolume = kvp.Value.SalesVolume * generalData.RefillSalesFactor,

                            SalesCapacity = solution.Locations[kvp.Key].Freestyle3100Count * generalData.Freestyle3100Data.RefillCapacityPerWeek +
                                solution.Locations[kvp.Key].Freestyle9100Count * generalData.Freestyle9100Data.RefillCapacityPerWeek,

                            LeasingCost = solution.Locations[kvp.Key].Freestyle3100Count * generalData.Freestyle3100Data.LeasingCostPerWeek +
                                solution.Locations[kvp.Key].Freestyle9100Count * generalData.Freestyle9100Data.LeasingCostPerWeek
                        };

                        if (scoredSolution.Locations[kvp.Key].SalesCapacity > 0 == false)
                        {
                            return null;

                        }
                    }
                    else
                        locationListNoRefillStation[kvp.Key] = new()
                        {
                            LocationName = kvp.Value.LocationName,
                            LocationType = kvp.Value.LocationType,
                            Latitude = kvp.Value.Latitude,
                            Longitude = kvp.Value.Longitude,
                            SalesVolume = kvp.Value.SalesVolume * generalData.RefillSalesFactor,
                        };
                }


                //Throw an error if no valid locations with a refill station was found
                if (scoredSolution.Locations.Count == 0)
                    return null;

                //Distribute sales from locations without a refill station to those with.
                scoredSolution.Locations = DistributeSales(scoredSolution.Locations, locationListNoRefillStation, generalData);
            }
            else
            {
                scoredSolution.Locations = InitiateSandboxLocations(scoredSolution.Locations, generalData, solution);
                scoredSolution.Locations = CalcualteFootfall(scoredSolution.Locations, mapEntity);
            }

            scoredSolution.Locations = DivideFootfall(scoredSolution.Locations, generalData);

            foreach (KeyValuePair<string, StoreLocationScoring> kvp in scoredSolution.Locations)
            {
                kvp.Value.SalesVolume = Math.Round(kvp.Value.SalesVolume, 0);
                if (kvp.Value.Footfall <= 0 && sandboxMaps.Contains(mapName) == true)
                {
                    kvp.Value.SalesVolume = 0;
                }
                double sales = kvp.Value.SalesVolume;
                if (kvp.Value.SalesCapacity < kvp.Value.SalesVolume)
                {
                    sales = kvp.Value.SalesCapacity;
                }

                kvp.Value.GramCo2Savings = sales * (generalData.ClassicUnitData.Co2PerUnitInGrams - generalData.RefillUnitData.Co2PerUnitInGrams)
                    - kvp.Value.Freestyle3100Count * generalData.Freestyle3100Data.StaticCo2
                    - kvp.Value.Freestyle9100Count * generalData.Freestyle9100Data.StaticCo2;

                scoredSolution.GameScore.KgCo2Savings += kvp.Value.GramCo2Savings / 1000;
                if (kvp.Value.GramCo2Savings > 0)
                {
                    kvp.Value.IsCo2Saving = true;
                }

                kvp.Value.Revenue = sales * generalData.RefillUnitData.ProfitPerUnit;
                scoredSolution.TotalRevenue += kvp.Value.Revenue;

                kvp.Value.Earnings = kvp.Value.Revenue - kvp.Value.LeasingCost;
                if (kvp.Value.Earnings > 0)
                {
                    kvp.Value.IsProfitable = true;
                }

                scoredSolution.TotalLeasingCost += kvp.Value.LeasingCost;

                scoredSolution.TotalFreestyle3100Count += kvp.Value.Freestyle3100Count;
                scoredSolution.TotalFreestyle9100Count += kvp.Value.Freestyle9100Count;

                scoredSolution.GameScore.TotalFootfall += kvp.Value.Footfall / 1000;
            }

            //Just some rounding for nice whole numbers
            scoredSolution.TotalRevenue = Math.Round(scoredSolution.TotalRevenue, 2);
            scoredSolution.GameScore.KgCo2Savings = Math.Round(scoredSolution.GameScore.KgCo2Savings, 2);
            scoredSolution.GameScore.TotalFootfall = Math.Round(scoredSolution.GameScore.TotalFootfall, 4);

            //Calculate Earnings
            scoredSolution.GameScore.Earnings = (scoredSolution.TotalRevenue - scoredSolution.TotalLeasingCost) / 1000;

            //Calculate total score
            scoredSolution.GameScore.Total = Math.Round(
                (scoredSolution.GameScore.KgCo2Savings * generalData.Co2PricePerKiloInSek + scoredSolution.GameScore.Earnings) *
                (1 + scoredSolution.GameScore.TotalFootfall),
                2
            );
            return scoredSolution;
        }

        private static Dictionary<string, StoreLocationScoring> DistributeSales(Dictionary<string, StoreLocationScoring> with, Dictionary<string, StoreLocationScoring> without, GeneralData generalData)
        {
            foreach (KeyValuePair<string, StoreLocationScoring> kvpWithout in without)
            {
                Dictionary<string, double> distributeSalesTo = new();
                //double locationSalesFrom = await GetSalesVolume(kvpWithout.Value.LocationType) ?? throw new Exception(string.Format("Location: {0}, have an invalid location type: {1}", kvpWithout.Key, kvpWithout.Value.LocationType));

                foreach (KeyValuePair<string, StoreLocationScoring> kvpWith in with)
                {
                    int distance = DistanceBetweenPoint(
                        kvpWithout.Value.Latitude, kvpWithout.Value.Longitude, kvpWith.Value.Latitude, kvpWith.Value.Longitude
                    );
                    if (distance < generalData.WillingnessToTravelInMeters)
                    {
                        distributeSalesTo[kvpWith.Value.LocationName] = distance;
                    }
                }

                double total = 0;
                if (distributeSalesTo.Count > 0)
                {
                    foreach (KeyValuePair<string, double> kvp in distributeSalesTo)
                    {
                        distributeSalesTo[kvp.Key] = Math.Pow(generalData.ConstantExpDistributionFunction, generalData.WillingnessToTravelInMeters - kvp.Value) - 1;
                        total += distributeSalesTo[kvp.Key];
                    }

                    //Add boosted sales to original sales volume
                    foreach (KeyValuePair<string, double> kvp in distributeSalesTo)
                    {
                        with[kvp.Key].SalesVolume += distributeSalesTo[kvp.Key] / total *
                        generalData.RefillDistributionRate * kvpWithout.Value.SalesVolume;//locationSalesFrom;
                    }
                }
            }

            return with;
        }

        public static Dictionary<string, StoreLocationScoring> CalcualteFootfall(Dictionary<string, StoreLocationScoring> locations, MapData mapEntity)
        {
            double maxFootfall = 0;
            foreach (KeyValuePair<string, StoreLocationScoring> kvpLoc in locations)
            {
                foreach (Hotspot hotspot in mapEntity.Hotspots)
                {
                    double distanceInMeters = DistanceBetweenPoint(
                        hotspot.Latitude, hotspot.Longitude, kvpLoc.Value.Latitude, kvpLoc.Value.Longitude
                    );
                    double maxSpread = hotspot.Spread;
                    if (distanceInMeters <= maxSpread)
                    {
                        double val = hotspot.Footfall * (1 - (distanceInMeters / maxSpread));
                        kvpLoc.Value.Footfall += val / 10;
                    }
                }
                if (maxFootfall < kvpLoc.Value.Footfall)
                {
                    maxFootfall = kvpLoc.Value.Footfall;
                }
            }
            if (maxFootfall > 0)
            {
                foreach (KeyValuePair<string, StoreLocationScoring> kvpLoc in locations)
                {
                    if (kvpLoc.Value.Footfall > 0)
                    {
                        kvpLoc.Value.FootfallScale = Convert.ToInt32(kvpLoc.Value.Footfall / maxFootfall * 10);
                        if (kvpLoc.Value.FootfallScale == 0)
                        {
                            kvpLoc.Value.FootfallScale = 1;
                        }
                    }
                }
            }
            return locations;
        }
        private static double GetSalesVolume(string locationType, GeneralData generalData)
        {
            foreach (KeyValuePair<string, LocationType> kvpLoc in generalData.LocationTypes)
            {
                if (locationType == kvpLoc.Value.Type)
                {
                    return kvpLoc.Value.SalesVolume;
                }
            }
            return 0;
        }
        public static Dictionary<string, StoreLocationScoring> InitiateSandboxLocations(Dictionary<string, StoreLocationScoring> locations, GeneralData generalData, SubmitSolution request)
        {
            foreach (KeyValuePair<string, PlacedLocations> kvpLoc in request.Locations)
            {
                double sv = GetSalesVolume(kvpLoc.Value.LocationType, generalData);
                StoreLocationScoring scoredSolution = new()
                {
                    Longitude = kvpLoc.Value.Longitude,
                    Latitude = kvpLoc.Value.Latitude,
                    Freestyle3100Count = kvpLoc.Value.Freestyle3100Count,
                    Freestyle9100Count = kvpLoc.Value.Freestyle9100Count,
                    LocationType = kvpLoc.Value.LocationType,
                    LocationName = kvpLoc.Key,
                    SalesVolume = sv,
                    SalesCapacity = request.Locations[kvpLoc.Key].Freestyle3100Count * generalData.Freestyle3100Data.RefillCapacityPerWeek +
                                request.Locations[kvpLoc.Key].Freestyle9100Count * generalData.Freestyle9100Data.RefillCapacityPerWeek,
                    LeasingCost = request.Locations[kvpLoc.Key].Freestyle3100Count * generalData.Freestyle3100Data.LeasingCostPerWeek +
                                request.Locations[kvpLoc.Key].Freestyle9100Count * generalData.Freestyle9100Data.LeasingCostPerWeek
                };
                locations.Add(kvpLoc.Key, scoredSolution);
            }
            foreach (KeyValuePair<string, StoreLocationScoring> kvpScope in locations)
            {
                int count = 1;
                //Dictionary<string, double> distributeSalesTo = new();
                foreach (KeyValuePair<string, StoreLocationScoring> kvpSurrounding in locations)
                {
                    if (kvpScope.Key != kvpSurrounding.Key)
                    {
                        int distance = DistanceBetweenPoint(
                            kvpScope.Value.Latitude, kvpScope.Value.Longitude, kvpSurrounding.Value.Latitude, kvpSurrounding.Value.Longitude
                        );
                        if (distance < generalData.WillingnessToTravelInMeters)
                        {
                            count++;
                        }
                    }
                }

                kvpScope.Value.SalesVolume = kvpScope.Value.SalesVolume / count;

            }
            return locations;
        }

        public static Dictionary<string, StoreLocationScoring> DivideFootfall(Dictionary<string, StoreLocationScoring> locations, GeneralData generalData)
        {
            foreach (KeyValuePair<string, StoreLocationScoring> kvpScope in locations)
            {
                int count = 1;
                foreach (KeyValuePair<string, StoreLocationScoring> kvpSurrounding in locations)
                {
                    if (kvpScope.Key != kvpSurrounding.Key)
                    {
                        int distance = DistanceBetweenPoint(
                            kvpScope.Value.Latitude, kvpScope.Value.Longitude, kvpSurrounding.Value.Latitude, kvpSurrounding.Value.Longitude
                        );
                        if (distance < generalData.WillingnessToTravelInMeters)
                        {
                            count++;
                        }
                    }
                }

                kvpScope.Value.Footfall = kvpScope.Value.Footfall / count;

            }
            return locations;
        }

        public static string SandboxValidation(string inMapName, SubmitSolution request, MapData mapData)
        {
            int countGroceryStoreLarge = 0;
            int countGroceryStore = 0;
            int countConvenience = 0;
            int countGasStation = 0;
            int countKiosk = 0;
            const int maxGroceryStoreLarge = 5;
            const int maxGroceryStore = 20;
            const int maxConvenience = 20;
            const int maxGasStation = 8;
            const int maxKiosk = 3;
            const int totalStores = maxGroceryStoreLarge + maxGroceryStore + maxConvenience + maxGasStation + maxKiosk;
            string numberErrorMsg = string.Format("locationName needs to start with 'location' and followed with a number larger than 0 and less than {0}.", totalStores + 1);
            string mapName = inMapName.ToLower();
            foreach (KeyValuePair<string, PlacedLocations> kvp in request.Locations)
            {
                //Validate location name
                if (kvp.Key.StartsWith("location") == false)
                {
                    return string.Format("{0} {1} is not a valid name", numberErrorMsg, kvp.Key);
                }
                string loc_num = kvp.Key.Substring(8);
                if (string.IsNullOrWhiteSpace(loc_num))
                {

                    return string.Format("{0} Nothing followed location in the locationName", numberErrorMsg);
                }
                var isNumeric = int.TryParse(loc_num, out int n);
                if (isNumeric == false)
                {
                    return string.Format("{0} {1} is not a number", numberErrorMsg, loc_num);
                }
                if (n <= 0 || n > totalStores)
                {
                    return string.Format("{0} {1} is not within the constraints", numberErrorMsg, n);
                }
                //Validate long and lat
                if (mapData.Border.LatitudeMin > kvp.Value.Latitude || mapData.Border.LatitudeMax < kvp.Value.Latitude)
                {
                    return string.Format("Latitude is missing or out of bounds for location : {0}", kvp.Key);
                }
                if (mapData.Border.LongitudeMin > kvp.Value.Longitude || mapData.Border.LongitudeMax < kvp.Value.Longitude)
                {
                    return string.Format("Longitude is missing or out of bounds for location : {0}", kvp.Key);
                }
                //Validate locationType
                if (kvp.Value.LocationType.Equals(string.Empty))
                {
                    return string.Format("locationType is missing for location) : {0}", kvp.Key);
                }
                else if (kvp.Value.LocationType.Equals("Grocery-store-large"))
                {
                    countGroceryStoreLarge += 1;
                }
                else if (kvp.Value.LocationType.Equals("Grocery-store"))
                {
                    countGroceryStore += 1;
                }
                else if (kvp.Value.LocationType.Equals("Convenience"))
                {
                    countConvenience += 1;
                }
                else if (kvp.Value.LocationType.Equals("Gas-station"))
                {
                    countGasStation += 1;
                }
                else if (kvp.Value.LocationType.Equals("Kiosk"))
                {
                    countKiosk += 1;
                }
                else
                {
                    return string.Format("locationType --> {0} not valid (check GetGeneralGameData for correct values) for location : {1}", kvp.Value.LocationType, kvp.Key);
                }
                //Validate that max number of location is not exceeded
                if (countGroceryStoreLarge > maxGroceryStoreLarge || countGroceryStore > maxGroceryStore ||
                    countConvenience > maxConvenience || countGasStation > maxGasStation ||
                    countKiosk > maxKiosk)
                {
                    return string.Format("Number of allowed locations exceeded for locationType: {0}", kvp.Value.LocationType);
                }
            }
            return null;
        }

        private static int DistanceBetweenPoint(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            double r = 6371e3;
            double latRadian1 = latitude1 * Math.PI / 180;
            double latRadian2 = latitude2 * Math.PI / 180;

            double latDelta = (latitude2 - latitude1) * Math.PI / 180;
            double longDelta = (longitude2 - longitude1) * Math.PI / 180;

            double a = Math.Sin(latDelta / 2) * Math.Sin(latDelta / 2) +
                Math.Cos(latRadian1) * Math.Cos(latRadian2) *
                Math.Sin(longDelta / 2) * Math.Sin(longDelta / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            int distance = (int)Math.Round(r * c, 0);

            return distance;
        }
    }
}