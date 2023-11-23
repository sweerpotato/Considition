using System.Diagnostics;

namespace ConsiditionLib2023
{
    public static class SolutionOptimizer
    {
        public static async Task<GameData?> OptimizeAndSubmitv2Async(SubmitSolution solution, GameData gameData, GeneralData generalData)
        {
            if (gameData.Locations == null)
            {
                return null;
            }

            foreach (KeyValuePair<string, StoreLocationScoring> scoredLocation in gameData.Locations)
            {
                int numberOf9100 = 0;
                int numberOf3100 = 0;
                double salesCapacity = scoredLocation.Value.SalesCapacity; /// generalData.RefillSalesFactor; //* generalData.RefillSalesFactor?

                while (salesCapacity >= generalData.Freestyle9100Data.RefillCapacityPerWeek)
                {
                    ++numberOf9100;
                    salesCapacity -= generalData.Freestyle9100Data.RefillCapacityPerWeek;
                }

                while (salesCapacity >= generalData.Freestyle3100Data.RefillCapacityPerWeek)
                {
                    ++numberOf3100;
                    salesCapacity -= generalData.Freestyle3100Data.RefillCapacityPerWeek;
                }

                if (numberOf9100 == 0 && numberOf3100 == 0)
                {
                    solution.Locations.Remove(scoredLocation.Key);
                }
                else
                {
                    solution.Locations[scoredLocation.Key].Freestyle3100Count = numberOf3100;
                    solution.Locations[scoredLocation.Key].Freestyle9100Count = numberOf9100;
                }
            }

            GameData? result = await Core.SubmitAsync(gameData.MapName, solution);

            return result;
        }

        public static async Task<GameData?> OptimizeAndSubmitAsync(SubmitSolution highScoreSolution, GameData gameData)
        {
            if (gameData.Locations == null)
            {
                return null;
            }

            GameData? result = gameData;

            if (result == null || result.Locations == null)
            {
                throw new Exception("zzz");
            }

            while (result.Locations.Where(locationPair => !locationPair.Value.IsProfitable).Any())
            {
                IEnumerable<KeyValuePair<string, StoreLocationScoring>> unprofitableLocations = result.Locations.Where(locationPair => !locationPair.Value.IsProfitable);
                bool wasModified = false;

                foreach (KeyValuePair<string, StoreLocationScoring> item in unprofitableLocations)
                {
                    if (!highScoreSolution.Locations.ContainsKey(item.Key))
                    {
                        continue;
                    }

                    if (highScoreSolution.Locations[item.Key].Freestyle9100Count > 0)
                    {
                        highScoreSolution.Locations[item.Key].Freestyle9100Count--;
                        wasModified = true;

                        if (highScoreSolution.Locations[item.Key].Freestyle3100Count > 0)
                        {
                            continue;
                        }
                    }

                    if (highScoreSolution.Locations[item.Key].Freestyle9100Count == 0 &&
                        highScoreSolution.Locations[item.Key].Freestyle3100Count == 0)
                    {
                        highScoreSolution.Locations[item.Key].Freestyle3100Count = 2;
                        wasModified = true;
                        continue;
                    }

                    if (highScoreSolution.Locations[item.Key].Freestyle3100Count > 0)
                    {
                        highScoreSolution.Locations[item.Key].Freestyle3100Count--;
                        wasModified = true;
                    }

                    if (highScoreSolution.Locations[item.Key].Freestyle9100Count == 0 &&
                        highScoreSolution.Locations[item.Key].Freestyle3100Count == 0)
                    {
                        highScoreSolution.Locations.Remove(item.Key);
                    }
                }

                Debug.WriteLine("Optimerar mer..");

                if (wasModified)
                {
                    result = await Core.SubmitAsync(gameData.MapName, highScoreSolution);
                    Debug.WriteLine("Väntar..");
                    await Task.Delay(3000);
                }
                else
                {
                    Debug.WriteLine("Klar!");
                    break;
                }
            }

            return result;
        }
    }
}
