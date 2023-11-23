using Newtonsoft.Json;

namespace ConsiditionLib2023
{
    internal class Api
    {
        private readonly HttpClient _httpClient;

        public Api(HttpClient httpClient)
        {
            _httpClient = httpClient;
            httpClient.BaseAddress = new Uri("https://api.considition.com/");
        }

        public async Task<MapData?> GetMapDataAsync(string mapName, string apiKey)
        {
            HttpRequestMessage request = new()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"/api/game/getmapdata?mapName={Uri.EscapeDataString(mapName)}", UriKind.Relative)
            };
            request.Headers.Add("x-api-key", apiKey);
            HttpResponseMessage response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string responseText = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<MapData>(responseText);
        }

        public async Task<GeneralData?> GetGeneralDataAsync()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("/api/game/getgeneralgamedata");
            response.EnsureSuccessStatusCode();
            string responseText = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GeneralData>(responseText);
        }

        public async Task<GameData?> GetGameAsync(Guid id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"/api/game/getgamedata{id}");
            response.EnsureSuccessStatusCode();
            string responseText = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GameData>(responseText);
        }

        public async Task<GameData?> SubmitAsync(string mapName, SubmitSolution solution, string apiKey)
        {
            HttpRequestMessage request = new()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri($"/api/Game/submitSolution?mapName={Uri.EscapeDataString(mapName)}", UriKind.Relative)
            };
            request.Headers.Add("x-api-key", apiKey);
            request.Content = new StringContent(JsonConvert.SerializeObject(solution), System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage response = _httpClient.Send(request);
            string responseText = await response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
            return JsonConvert.DeserializeObject<GameData>(responseText);
        }
    }
}
