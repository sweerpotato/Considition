using ConsiditionLib2023;
using GAF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GUIdition.Genetics
{
    public class Generations
    {
        public static async Task<List<Gene>> GetFirstGenerationGenes(Maps map)
        {
            CompleteMapData completeMapData = await Core.GetCompleteMapDataAsync(map);
            SolutionConfiguration config = new(map, new Dictionary<string, PlacedLocations>());

            foreach (KeyValuePair<string, StoreLocation> locationKeyValuePair in completeMapData.MapData.Locations)
            {
                config.Locations.Add(locationKeyValuePair.Key, new PlacedLocations()
                {
                    Freestyle3100Count = 0,
                    Freestyle9100Count = 1
                });
            }

            GameData currentGameData = Core.Run(config, completeMapData);
            GameData highScoreGameData = null;
            double highScore = currentGameData.GameScore.Total;

            KeyValuePair<string, PlacedLocations> modifyFreestyle3100(KeyValuePair<string, PlacedLocations> locationKVP, bool add)
            {
                if (add)
                {
                    if (locationKVP.Value.Freestyle3100Count == 2)
                    {
                        return locationKVP;
                    }

                    ++locationKVP.Value.Freestyle3100Count;
                }
                else
                {
                    if (locationKVP.Value.Freestyle3100Count == 0)
                    {
                        return locationKVP;
                    }

                    --locationKVP.Value.Freestyle3100Count;
                }

                return locationKVP;
            }
            KeyValuePair<string, PlacedLocations> modifyFreestyle9100(KeyValuePair<string, PlacedLocations> locationKVP, bool add)
            {
                if (add)
                {
                    if (locationKVP.Value.Freestyle9100Count == 2)
                    {
                        return locationKVP;
                    }

                    ++locationKVP.Value.Freestyle9100Count;
                }
                else
                {
                    if (locationKVP.Value.Freestyle9100Count == 0)
                    {
                        return locationKVP;
                    }

                    --locationKVP.Value.Freestyle9100Count;
                }
                
                return locationKVP;
            }

            foreach (KeyValuePair<string, PlacedLocations> locationKVP in config.Locations)
            {
                modifyFreestyle9100(locationKVP, add: true);

                currentGameData = Core.Run(config, completeMapData);

                if (highScore > currentGameData.GameScore.Total)
                {
                    modifyFreestyle9100(locationKVP, add: false);
                }
                else
                {
                    highScore = currentGameData.GameScore.Total;
                    highScoreGameData = currentGameData;
                }
            }

            foreach (KeyValuePair<string, PlacedLocations> locationKVP in config.Locations)
            {
                modifyFreestyle3100(locationKVP, add: true);

                currentGameData = Core.Run(config, completeMapData);

                if (highScore > currentGameData.GameScore.Total)
                {
                    modifyFreestyle3100(locationKVP, add: false);
                }
                else
                {
                    highScore = currentGameData.GameScore.Total;
                    highScoreGameData = currentGameData;
                }
            }

            foreach (KeyValuePair<string, PlacedLocations> locationKVP in config.Locations)
            {
                modifyFreestyle3100(locationKVP, add: true);

                currentGameData = Core.Run(config, completeMapData);

                if (highScore > currentGameData.GameScore.Total)
                {
                    modifyFreestyle3100(locationKVP, add: false);
                }
                else
                {
                    highScore = currentGameData.GameScore.Total;
                    highScoreGameData = currentGameData;
                }
            }

            foreach (KeyValuePair<string, PlacedLocations> locationKVP in config.Locations.ToList())
            {
                modifyFreestyle9100(locationKVP, add: false);

                if (locationKVP.Value.Freestyle3100Count == 0 &&
                    locationKVP.Value.Freestyle9100Count == 0)
                {
                    config.Locations.Remove(locationKVP.Key);
                }

                currentGameData = Core.Run(config, completeMapData);

                if (highScore > currentGameData.GameScore.Total)
                {
                    config.Locations.Add(locationKVP.Key, new PlacedLocations()
                    {
                        Freestyle3100Count = 0,
                        Freestyle9100Count = 1
                    });
                }
                else
                {
                    highScore = currentGameData.GameScore.Total;
                    highScoreGameData = currentGameData;
                }
            }

            foreach (KeyValuePair<string, PlacedLocations> locationKVP in config.Locations.ToList())
            {
                modifyFreestyle3100(locationKVP, add: false);

                if (locationKVP.Value.Freestyle3100Count == 0 &&
                    locationKVP.Value.Freestyle9100Count == 0)
                {
                    config.Locations.Remove(locationKVP.Key);
                }

                currentGameData = Core.Run(config, completeMapData);

                if (highScore > currentGameData.GameScore.Total)
                {
                    config.Locations.Add(locationKVP.Key, new PlacedLocations()
                    {
                        Freestyle3100Count = 1,
                        Freestyle9100Count = 0
                    });
                }
                else
                {
                    highScore = currentGameData.GameScore.Total;
                    highScoreGameData = currentGameData;
                }
            }

            foreach (KeyValuePair<string, PlacedLocations> locationKVP in config.Locations.ToList())
            {
                modifyFreestyle3100(locationKVP, add: false);

                if (locationKVP.Value.Freestyle3100Count == 0 &&
                    locationKVP.Value.Freestyle9100Count == 0)
                {
                    config.Locations.Remove(locationKVP.Key);
                }

                currentGameData = Core.Run(config, completeMapData);

                if (highScore > currentGameData.GameScore.Total)
                {
                    config.Locations.Add(locationKVP.Key, new PlacedLocations()
                    {
                        Freestyle3100Count = 1,
                        Freestyle9100Count = 0
                    });
                }
                else
                {
                    highScore = currentGameData.GameScore.Total;
                    highScoreGameData = currentGameData;
                }
            }

            List<Gene> result = new();

            foreach (KeyValuePair<string, StoreLocation> locationKeyValuePair in completeMapData.MapData.Locations)
            {
                if (!config.Locations.ContainsKey(locationKeyValuePair.Key))
                {
                    result.Add(new Gene(0));
                    result.Add(new Gene(0));
                }
                else
                {
                    result.Add(new Gene(config.Locations[locationKeyValuePair.Key].Freestyle3100Count));
                    result.Add(new Gene(config.Locations[locationKeyValuePair.Key].Freestyle9100Count));
                }
            }

            return result;
        }
    }
}
