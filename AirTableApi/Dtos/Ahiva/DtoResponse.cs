namespace AirTableApi.Dtos.Ahiva
{
    public class DtoResponse
    {
        public string CodigoRespuesta { get; set; }
        public string DescripcionRespuesta { get; set; }
        public bool EsError { get; set; }

        public List<DtoEnvioRespuesta> Envios { get; set; } = new();
    }

    public class DtoEnvioRespuesta
    {
        public List<string> CodigosTrazabilidad { get; set; }
        public List<string> EtiquetasPDFBase64 { get; set; }
        public decimal? CostoEstimado { get; set; }
    }
}
