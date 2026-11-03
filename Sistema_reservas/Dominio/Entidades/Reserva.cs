using Sistema_reservas.Dominio.Enumeraciones;

namespace Sistema_reservas.Dominio.Entidades;

public class Reserva
{
    public int IdReserva { get; set; } 

    public string UnidadId { get; set; } = string.Empty;

    public string Solicitante { get; set; } = string.Empty;

    public DateTime FechaInicio { get; set; }

    public DateTime FechaFin { get; set; }

    public EstadoReserva Estado { get; set; }= EstadoReserva.Activa;
    public DateTime FechaCreacionUtc { get; set; } = DateTime.Now;
    public void Cancelar()
    {
        if (Estado == EstadoReserva.Cancelada)
            throw new InvalidOperationException("La reserva ya está cancelada.");

        Estado = EstadoReserva.Cancelada;
    }
}
