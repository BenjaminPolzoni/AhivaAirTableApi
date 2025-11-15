using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AirTableApi.Dtos;
using Newtonsoft.Json;

public class AirtableService
{
    private readonly HttpClient _httpClient;

    // Info que traemos del Appjson para los valores de la tabla que llamaremos

    private readonly string _baseId;
    private readonly string _tableName;
    private readonly string _apiKey;


    // nombre de la tabla que vamos a actualizar con el nro de pedido
    private const string FIELD_TRACKING = "Número de rastreo";

    public AirtableService(IConfiguration config)
    {
        _baseId = config["Airtable:BaseId"];
        _tableName = config["Airtable:TableName"];
        _apiKey = config["Airtable:ApiKey"];

        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _apiKey);
    }


    // Esta funcion o metodo se encarga de gacer un get de la airtable a mi api, trayendo todos los valores correspondientes
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

    // Este metodo se encarga de realizar un Put a la base de datos (Lo vamos a usar cuando tengamos que actualizar algun valor especifico)
    public async Task<bool> UpdateTrackingNumber(string recordId, string trackingNumber)
    {

        var url = $"https://api.airtable.com/v0/{_baseId}/{_tableName}/{recordId}";



        var body = new
        {
            fields = new Dictionary<string, object>
        {
            { "Numero de rastreo", trackingNumber }
        }
        };

        var json = JsonConvert.SerializeObject(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PatchAsync(url, content);

        return response.IsSuccessStatusCode;
    }

}

