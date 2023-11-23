using System.Diagnostics;

namespace ConsiditionLib2023
{
    public static class Core
    {
        #region apikey
        const string apikey = "bf6975df-b5bf-438b-a39a-99ecd73afba1";
        #endregion

        private static string GetMapName(Maps map)
        {
            return map switch
            {
                Maps.Stockholm => MapNames.Stockholm,
                Maps.Goteborg => MapNames.Goteborg,
                Maps.Malmo => MapNames.Malmo,
                Maps.Uppsala => MapNames.Uppsala,
                Maps.Vasteras => MapNames.Vasteras,
                Maps.Orebro => MapNames.Orebro,
                Maps.London => MapNames.London,
                Maps.Linkoping => MapNames.Linkoping,
                Maps.Berlin => MapNames.Berlin,
                _ => null
            } ?? throw new InvalidOperationException("Invalid map selected");
        }

        public async static Task<MapData?> GetMapDataAsync(Maps map)
        {
            string? mapName = GetMapName(map);
            HttpClient client = new();
            Api api = new(client);
            return await api.GetMapDataAsync(mapName, apikey);
        }

        public async static Task<GameData?> SubmitAsync(string mapName, SubmitSolution solution)
        {
            try
            {
                HttpClient client = new();
                Api api = new(client);
                return await api.SubmitAsync(mapName, solution, apikey);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return null;
        }

        public async static Task<CompleteMapData> GetCompleteMapDataAsync(Maps map)
        {
            string? mapName = GetMapName(map);
            HttpClient client = new();
            Api api = new(client);
            MapData? mapData = await api.GetMapDataAsync(mapName, apikey);
            GeneralData? generalData = await api.GetGeneralDataAsync();

            if (generalData is null)
            {
                throw new NullReferenceException($"{nameof(generalData)} is null");
            }
            else if (mapData is null)
            {
                throw new NullReferenceException($"{mapData} is null");
            }

            return new CompleteMapData()
            {
                MapData = mapData,
                GeneralData = generalData,
                MapName = mapName
            };
        }

        public static GameData Run(SolutionConfiguration config, CompleteMapData completeMapData)
        {
            if (config is null)
            {
                throw new ArgumentNullException($"{nameof(config)} is null");
            }

            if (completeMapData is null)
            {
                throw new ArgumentNullException($"{completeMapData} is null");
            }

            SubmitSolution solution = new()
            {
                Locations = config.Locations
            };

            GameData score = Scoring.CalculateScore(completeMapData.MapName,
                solution,
                completeMapData.MapData,
                completeMapData.GeneralData);

            if (score.GameScore is null)
            {
                throw new NullReferenceException($"{nameof(score.GameScore)} is null");
            }
            
            return score;
        }

        public static async Task<Score> CoreTest()
        {
            return await Task.FromResult(new Score { Total = new Random().Next(0, 500000) });
        }
    }
}
