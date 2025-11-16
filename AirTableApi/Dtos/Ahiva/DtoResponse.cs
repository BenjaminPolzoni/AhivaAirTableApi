namespace AirTableApi.Dtos.Ahiva
{
    public class DtoResponse
    {
        public string CodigoRespuesta { get; set; }
        public string DescripcionRespuesta { get; set; }
        public bool EsError { get; set; }

        // Lista de códigos de seguimiento / tracking (para carga masiva)
        public List<string>? Tracking { get; set; }

        // Costo estimado o total calculado por AHIVA
        public double? CostoEstimado { get; set; }

        // Solo para Consultar Estado
        public string? Estado { get; set; }
    }

    public class DtoEnvioRespuesta
    {
        public List<string> CodigosTrazabilidad { get; set; }
        public List<string> EtiquetasPDFBase64 { get; set; }
        public decimal? CostoEstimado { get; set; }
    }
}
