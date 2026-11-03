using Sistema_reservas.Aplicación.Abstracciones.Persistencia;
using Sistema_reservas.Dominio.Entidades;

namespace Sistema_reservas.Aplicación.Servicios;

public class ReservaServicio 
{
    private readonly IReservaRepositorio _repositorio;
    private readonly DisponibilidadServicio _disponibilidad;

    public ReservaServicio(IReservaRepositorio repositorio, DisponibilidadServicio disponibilidad)
    {
        _repositorio = repositorio;
        _disponibilidad = disponibilidad;
    }

    public Task<IReadOnlyList<Unidad>> ObtenerInventarioAsync()
        => _repositorio.ListarUnidadesAsync();

    public Task<IReadOnlyList<Reserva>> ListarHistorialAsync()
        => _repositorio.ListarTodasAsync();

    public async Task<Reserva> CrearAsync(string unidadId, string solicitante, DateTime inicioUtc, DateTime finUtc)
    {
        if (string.IsNullOrWhiteSpace(solicitante))
            throw new ArgumentException("El solicitante es obligatorio.");

        if (inicioUtc < DateTime.UtcNow)
            throw new ArgumentException("No se puede reservar en el pasado.");

        if (!await _disponibilidad.EstaDisponibleAsync(unidadId, inicioUtc, finUtc))
            throw new InvalidOperationException("La unidad no está disponible en ese horario.");

        var reserva = new Reserva
        {
            UnidadId = unidadId,
            Solicitante = solicitante.Trim(),
            FechaInicio = inicioUtc,
            FechaFin = finUtc
        };

        await _repositorio.AgregarAsync(reserva);
        return reserva;
    }

    public async Task CancelarAsync(int reservaId)
    {
        var reserva = await _repositorio.ObtenerPorIdAsync(reservaId)
            ?? throw new InvalidOperationException("La reserva no existe.");

        reserva.Cancelar();
        await _repositorio.ActualizarAsync(reserva);
    }
}