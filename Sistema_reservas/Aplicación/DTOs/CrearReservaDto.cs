namespace Sistema_reservas.Aplicación.DTOs;

public sealed class CrearReservaDto
{
    public string UnidadId { get; set; } = string.Empty;

    public string Solicitante { get; set; } = string.Empty;

    public DateTime FechaInicio { get; set; }

    public DateTime FechaFin { get; set; }
}
