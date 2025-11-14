using System.Net.Http.Headers;
using System.Text.Json;
using AirTableApi.Dtos;
using Newtonsoft.Json;

public class AirtableService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseId;
    private readonly string _tableName;
    private readonly string _apiKey;

    public AirtableService(IConfiguration config)
    {
        _baseId = config["Airtable:BaseId"];
        _tableName = config["Airtable:TableName"];
        _apiKey = config["Airtable:ApiKey"];

        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _apiKey);
    }

    public async Task<List<AirtableRecord>> GetAllRecordsAsync()
    {
        var allRecords = new List<AirtableRecord>();
        string offset = null;

        var baseUrl = $"https://api.airtable.com/v0/{_baseId}/{_tableName}";

        do
        {
            var url = baseUrl;

            // Armamos los query params: pageSize + offset (si hay)
            var queryParams = new List<string> { "pageSize=100" };

            if (!string.IsNullOrEmpty(offset))
                queryParams.Add($"offset={offset}");

            if (queryParams.Count > 0)
                url += "?" + string.Join("&", queryParams);

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<AirtableResponse>(json);

            if (result?.Records != null)
                allRecords.AddRange(result.Records);

            offset = result.Offset;

        } while (!string.IsNullOrEmpty(offset));

        return allRecords;
    }
}

