namespace AirTableApi.Dtos.Ahiva
{
    public class DtoEnvio
    {
        public DtoDestinatario Destinatario { get; set; }
        public DtoLugarEntrega LugarEntrega { get; set; }
        public DtoDevolucion Devolucion { get; set; }
        public List<DtoPaquete> Paquetes { get; set; } = new();

        public DtoContraReembolso ContraReembolso { get; set; }
        public DtoFacturaConformada FacturaConformada { get; set; }

        public bool SoloDestinatario { get; set; }
        public string CedulaDestinatario { get; set; }
    }
}
