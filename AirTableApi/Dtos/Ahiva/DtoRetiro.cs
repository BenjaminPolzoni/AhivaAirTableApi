namespace AirTableApi.Dtos.Ahiva
{
    public class DtoRetiro
    {
        public DateTime Fecha { get; set; }
        public string HorarioDesde { get; set; }
        public string HorarioHasta { get; set; }
        public string NombreContacto { get; set; }
        public string TelefonoContacto { get; set; }
        public string MailContacto { get; set; }

        public string Departamento { get; set; }
        public string Localidad { get; set; }
        public string Calle { get; set; }
        public string NroPuerta { get; set; }
        public string Apartamento { get; set; }
    }
}
