using AirTableApi.Dtos.Ahiva;
using ServiceReference;   // Carga masiva
using ServiceReference1;  // Consultar estados

namespace AirTableApi.Services
{
    public class AhivaService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<AhivaService> _logger;

        private readonly string _user;
        private readonly string _password;
        private readonly string _cuenta;
        private readonly string _subcuenta;

        public AhivaService(IConfiguration config, ILogger<AhivaService> logger)
        {
            _config = config;
            _logger = logger;

            // Leer credenciales desde appsettings.json
            _user = config["Ahiva:User"]!;
            _password = config["Ahiva:Password"]!;
            _cuenta = config["Ahiva:Cuenta"]!;
            _subcuenta = config["Ahiva:Subcuenta"]!;
        }

        private void ValidarCredenciales()
        {
            var user = _config["Ahiva:User"];
            var pass = _config["Ahiva:Password"];
            var cuenta = _config["Ahiva:Cuenta"];
            var subcuenta = _config["Ahiva:Subcuenta"];

            if (string.IsNullOrWhiteSpace(user))
                throw new Exception("Falta configurar Ahiva:User en appsettings.");

            if (string.IsNullOrWhiteSpace(pass))
                throw new Exception("Falta configurar Ahiva:Password en appsettings.");

            if (string.IsNullOrWhiteSpace(cuenta))
                throw new Exception("Falta configurar Ahiva:Cuenta en appsettings.");

            if (string.IsNullOrWhiteSpace(subcuenta))
                throw new Exception("Falta configurar Ahiva:Subcuenta en appsettings.");
        }

        // =======================================================
        //  1) CARGA MASIVA
        // =======================================================
        public async Task<DtoResponse> EnviarCargaMasiva(DtoCargaMasivaRequest request)
        {
            try
            {
                ValidarCredenciales();

                var client = new CargaMasivaServicev4Client(
                    CargaMasivaServicev4Client.EndpointConfiguration.CargaMasivaServicev4Port);

                // MAPEO → SOAP ======================================

                // Parametros (arg4)
                var parametros = request.Parametros?
                    .Select(p => new dataParametro
                    {
                        clave = p.Clave,
                        valor = p.Valor
                    })
                    .ToArray();

                // Envíos (arg5)
                var envios = request.Data?
                    .Select(d => new dataEnvio
                    {
                        soloDestinatario = d.SoloDestinatario,
                        cedulaDestinatario = d.CedulaDestinatario,

                        destinatario = new dataDestinatario
                        {
                            nombre = d.Destinatario.Nombre,
                            mail = d.Destinatario.Mail,
                            celular = d.Destinatario.Celular
                        },

                        lugarEntrega = d.LugarEntrega == null ? null : new dataLugarEntrega
                        {
                            calle = d.LugarEntrega.Calle,
                            nroPuerta = d.LugarEntrega.NroPuerta,
                            localidad = d.LugarEntrega.Localidad,
                            departamento = d.LugarEntrega.Departamento
                        },

                        paquetesSimples = d.Paquetes?
                            .Select(p => new dataPaquete
                            {
                                peso = p.Peso,
                                referencia = p.Referencia,
                                responsableServEntrega = p.ResponsableServEntrega,
                                codigoBarrasCliente = p.CodigoBarrasCliente
                            })
                            .ToArray()
                    })
                    .ToArray();

                // Retiro (arg6)
                dataRetiro? retiro = null;
                if (request.Retiro != null)
                {
                    retiro = new dataRetiro
                    {
                        contacto = request.Retiro.NombreContacto,
                        direccion = $"{request.Retiro.Localidad}, {request.Retiro.Calle} {request.Retiro.NroPuerta}",
                        telefono = request.Retiro.TelefonoContacto,
                        mail = request.Retiro.MailContacto,
                        desde = Convert.ToInt32(request.Retiro.HorarioDesde),
                        hasta = Convert.ToInt32(request.Retiro.HorarioHasta),
                        fecha = request.Retiro.Fecha,
                        fechaSpecified = true
                    };
                }

                // ================================================
                //  LLAMADA SOAP → (user/pass/cuenta/subcuenta desde config)
                // ================================================
                var soapResponse = await client.cargaMasivaAsync(
                    _user,          // arg0
                    _password,      // arg1
                    _cuenta,        // arg2
                    _subcuenta,     // arg3
                    parametros,     // arg4
                    envios,         // arg5
                    retiro          // arg6
                );

                var result = soapResponse.@return;

                // ================================================
                //  MAPEO RESPUESTA → DTO
                // ================================================
                var trackingList = result.envios?
                    .Where(e => e.codigostrazabilidad != null)
                    .SelectMany(e => e.codigostrazabilidad!)
                    .ToList() ?? new List<string>();

                double? costoEstimado = null;
                var envioConCostos = result.envios?.FirstOrDefault(e => e.costos != null);
                if (envioConCostos?.costos != null)
                    costoEstimado = envioConCostos.costos.costoTotal_destinatario;

                return new DtoResponse
                {
                    CodigoRespuesta = result.codigoRespuesta.ToString(),
                    DescripcionRespuesta = result.descripcionRespuesta,
                    EsError = result.esError,
                    Tracking = trackingList,
                    CostoEstimado = costoEstimado
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al llamar a CargaMasivaServicev4");
                throw;
            }
        }

        // =======================================================
        //  2) CONSULTAR ESTADO
        // =======================================================
        public async Task<DtoResponse> ConsultarEstado(string tracking)
        {
            try
            {
                ValidarCredenciales();

                var client = new ConsultarEstadosServiceClient(
                    ConsultarEstadosServiceClient.EndpointConfiguration.ConsultarEstadosServicePort);

                // arg0 = tracking, arg1 = user, arg2 = password
                var soapResponse = await client.consultarEstadoAsync(tracking, _user, _password);

                var result = soapResponse.@return;

                return new DtoResponse
                {
                    CodigoRespuesta = result.codigoRespuesta.ToString(),
                    DescripcionRespuesta = result.descripcionRespuesta,
                    EsError = result.esError,
                    Tracking = new List<string> { tracking },
                    Estado = result.estado?.descripcion
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al llamar a ConsultarEstadosService");
                throw;
            }
        }
    }
}
