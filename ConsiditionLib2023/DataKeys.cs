namespace ConsiditionLib2023;

public static class MapNames
{
    public static string Stockholm { get; } = "stockholm";
    public static string Goteborg { get; } = "goteborg";
    public static string Malmo { get; } = "malmo";
    public static string Uppsala { get; } = "uppsala";
    public static string Vasteras { get; } = "vasteras";
    public static string Orebro { get; } = "orebro";
    public static string London { get; } = "london";
    public static string Berlin { get; } = "berlin";
    public static string Linkoping { get; } = "linkoping";
    public static string GSandbox { get; } = "g-sandbox";
    public static string SSandbox { get; } = "s-sandbox";

}

public static class LocationKeys
{
    public static string Locations { get; } = "locations";
    public static string LocationName { get; } = "locationName";
    public static string LocationType { get; } = "locationType";
    public static string Footfall { get; } = "footfall";
    public static string SalesVolume { get; } = "salesVolume";
    public static string F3100Count { get; } = "freestyle3100Count";
    public static string F9100Count { get; } = "freestyle9100Count";
    public static string SalesCapacity { get; } = "salesCapacity";
    public static string LeasingCost { get; } = "leasingCost";
}

public static class CoordinateKeys
{
    public static string Latitude { get; } = "latitude";
    public static string Longitude { get; } = "longitude";
}

public static class ScoringKeys
{
    public static string GameId { get; } = "id";
    public static string MapName { get; } = "mapName";
    public static string GameScore { get; } = "gameScore";
    public static string TotalRevenue { get; } = "totalRevenue";
    public static string TotalLeasingCost { get; } = "totalLeasingCost";
    public static string TotalF3100Count { get; } = "totalFreestyle3100Count";
    public static string TotalF9100Count { get; } = "totalFreestyle9100Count";
    public static string Co2Savings { get; } = "co2Savings";
    public static string TotalFootfall { get; } = "totalFootfall";
    public static string Earnings { get; } = "earnings";
    public static string Total { get; } = "total";
}

public static class GeneralKeys
{
    public static string ConstantExpDistributionFunction { get; } = "constantExpDistributionFunction";
    public static string WillingnessToTravelInMeters { get; } = "willingnessToTravelInMeters";
    public static string F3100Data { get; } = "freestyle3100Data";
    public static string F9100Data { get; } = "freestyle9100Data";
    public static string RefillCapacityPerWeek { get; } = "refillCapacityPerWeek";
    public static string LeasingCostPerWeek { get; } = "leasingCostPerWeek";
    public static string RefillUnitData { get; } = "refillUnitData";
    public static string ClassicUnitData { get; } = "classicUnitData";
    public static string ProfitPerUnit { get; } = "profitPerUnit";
    public static string Co2PerUnitInGrams { get; } = "co2PerUnitInGrams";
    public static string Co2PricePerKiloInSek { get; } = "co2PricePerKiloInSek";
    public static string LocationTypes { get; } = "locationTypes";
    public static string Type { get; } = "type";
    public static string RefillDistributionRate { get; } = "refillDistributionRate";
    public static string RefillSalesFactor { get; } = "refillSalesFactor";
}