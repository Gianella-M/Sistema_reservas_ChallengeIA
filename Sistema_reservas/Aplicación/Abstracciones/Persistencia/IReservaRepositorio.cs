using Sistema_reservas.Dominio.Entidades;

namespace Sistema_reservas.Aplicación.Abstracciones.Persistencia;

public interface IReservaRepositorio
{
    Task<Reserva?> ObtenerPorIdAsync(int id);
    Task<IReadOnlyList<Reserva>> ListarTodasAsync();
    Task<IReadOnlyList<Reserva>> ListarActivasPorUnidadAsync(string unidadId);
    Task<IReadOnlyList<Unidad>> ListarUnidadesAsync();
    Task<bool> ExisteUnidadAsync(string unidadId);
    Task AgregarAsync(Reserva reserva);
    Task ActualizarAsync(Reserva reserva);
}