namespace AirTableApi.Dtos.Ahiva
{
    public class DtoPaquete
    {
        public double Peso { get; set; }
        public string ResponsableServEntrega { get; set; }
        public string Empaque { get; set; }
        public string Almacenamiento { get; set; }
        public string CodigoBarrasCliente { get; set; }
        public string Referencia { get; set; }
        public decimal? ValorDeclarado { get; set; }
        public bool GarantiaPlus { get; set; }
    }

}
