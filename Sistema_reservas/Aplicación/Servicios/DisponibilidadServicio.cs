using Sistema_reservas.Aplicación.Abstracciones.Persistencia;
using Sistema_reservas.Dominio.Entidades;

namespace Sistema_reservas.Aplicación.Servicios;

public class DisponibilidadServicio
{
    private readonly IReservaRepositorio _repositorio;

    public DisponibilidadServicio(IReservaRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<bool> EstaDisponibleAsync(string unidadId, DateTime inicioUtc, DateTime finUtc)
    {
        if (!await _repositorio.ExisteUnidadAsync(unidadId))
            throw new InvalidOperationException("La unidad no existe.");

        if (inicioUtc >= finUtc)
            throw new ArgumentException("El intervalo de fechas no es válido.");

        var activas = await _repositorio.ListarActivasPorUnidadAsync(unidadId);

        var haySuperposicion = activas.Any(r =>
            r.FechaInicio < finUtc &&
            r.FechaFin > inicioUtc);

        return !haySuperposicion;
    }

    public async Task<IReadOnlyList<Reserva>> ConsultarOcupacionAsync(string unidadId)
    {
        if (!await _repositorio.ExisteUnidadAsync(unidadId))
            throw new InvalidOperationException("La unidad no existe.");

        return await _repositorio.ListarActivasPorUnidadAsync(unidadId);
    }
}
