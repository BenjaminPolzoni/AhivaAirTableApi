namespace AirTableApi.Dtos
{
    public class DtoPedido
    {
        public string? Identificador { get; set; }
        public string? Estado { get; set; }
        public string? Producto { get; set; }
        public string? Nombre { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }

        public DateTime? UltimoCambioEstado { get; set; }  // 👈 ESTE ES EL CAMPO DE FECHA

        public string? Pago { get; set; } // 👈 ESTE ES EL CAMPO QUE DELIMITA QUE SE ENCUENTRE PAGO EL PEDIDO

        public DateTime? FechaHora { get; set; }


        public static DtoPedido MapAirtable(Dictionary<string, object> fields)
        {
            return new DtoPedido
            {
                Identificador = fields.GetValueOrDefault("Identificador")?.ToString(),
                Estado = fields.GetValueOrDefault("Estado")?.ToString(),
                Producto = fields.GetValueOrDefault("Producto")?.ToString(),
                Nombre = fields.GetValueOrDefault("Nombre")?.ToString(),
                Telefono = fields.GetValueOrDefault("Telefono")?.ToString(),
                Email = fields.GetValueOrDefault("Email")?.ToString(),

                UltimoCambioEstado = DateTime.TryParse(fields.GetValueOrDefault("Últ. cambio de estado")?.ToString(), out var fecha) ? fecha : null,

                Pago = fields.GetValueOrDefault("Pago")?.ToString(),

                FechaHora = DateTime.TryParse(fields.GetValueOrDefault("Fecha y hora")?.ToString(),out var fh) ? fh : null

            };
        }
    }
}
