using System.Net.Http.Headers;
using System.Text.Json;
using AirTableApi.Dtos;

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

    public async Task<List<DtoPedido>> GetPedidosAsync()
    {
        // 1️⃣ Construimos la URL base de la API de Airtable.
        //    Ejemplo: https://api.airtable.com/v0/app3mwobjU214HZqW/TuespacioUy%20PEDIDOS
        var url = $"https://api.airtable.com/v0/{_baseId}/{_tableName}";

        // 2️⃣ Enviamos una solicitud GET al endpoint.
        //    _httpClient ya tiene configurado el header de autorización con el token Bearer.
        var response = await _httpClient.GetAsync(url);

        // 3️⃣ Si la respuesta no es 200 OK, lanza una excepción automática.
        //    Esto evita que el código continúe si hubo error (por ejemplo, token inválido o base incorrecta).
        response.EnsureSuccessStatusCode();

        // 4️⃣ Leemos el contenido del cuerpo de la respuesta como texto JSON.
        var json = await response.Content.ReadAsStringAsync();

        // 5️⃣ Deserializamos el JSON a un objeto de tipo AirtableResponse.
        //    Este modelo contiene una lista de registros (Records), cada uno con sus "fields".
        var result = JsonSerializer.Deserialize<AirtableResponse>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true // Permite ignorar mayúsculas/minúsculas en nombres de propiedades.
        });

        // 6️⃣ Verificamos que existan registros y mapeamos cada uno al modelo "Pedido".
        //    Airtable devuelve los campos en un diccionario, por eso usamos un mapper auxiliar.

        var fechaLimite = DateTime.Now.AddDays(-20); // Le digo que me traiga fechas de hoy hasta hace 14 dias

        var pedidos = result?.Records
            .Where(r => r.Fields != null) // Evita registros vacíos

            // Filtro por estado = entregar y pago = pago
            .Where(r =>
                r.Fields.ContainsKey("Estado") &&
                r.Fields["Estado"]?.ToString()?.ToLower() == "entregar" &&

                r.Fields.ContainsKey("Estado de pago") &&
                r.Fields["Estado de pago"]?.ToString()?.ToLower() == "pago"
            )

            // Mapear después de filtrar
            .Select(r => DtoPedido.MapAirtable(r.Fields))

            // Solo fechas dentro de las últimas 2 semanas
            .Where(p => p.FechaHora != null && p.FechaHora >= fechaLimite)

            // ORDEN IGUAL AL DE AIRTABLE
            .OrderBy(p => p.Pago == "Pago" ? 1 : 2)      // Pago primero
            .ThenBy(p => p.FechaHora)                            // más antiguo → más reciente

            .ToList();

        // 7️⃣ Retornamos la lista final de pedidos, o una lista vacía si no hubo resultados.
        return pedidos ?? new List<DtoPedido>();
    }

}

public class AirtableResponse
{
    public List<AirtableRecord> Records { get; set; }
}

public class AirtableRecord
{
    public string Id { get; set; }
    public Dictionary<string, object> Fields { get; set; }
}
