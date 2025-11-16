namespace AirTableApi.Dtos.Ahiva
{
    public class DtoCargaMasivaRequest
    {
        public string User { get; set; }
        public string Password { get; set; }
        public string Cuenta { get; set; }
        public string Subcuenta { get; set; }

        public List<DtoParametro> Parametros { get; set; } = new();
        public List<DtoEnvio> Data { get; set; } = new();
        public DtoRetiro Retiro { get; set; }
    }

}
