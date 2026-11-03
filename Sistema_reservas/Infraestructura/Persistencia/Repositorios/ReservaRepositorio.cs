using Microsoft.EntityFrameworkCore;
using Sistema_reservas.Aplicación.Abstracciones.Persistencia;
using Sistema_reservas.Dominio.Entidades;
using Sistema_reservas.Dominio.Enumeraciones;

namespace Sistema_reservas.Infraestructura.Persistencia.Repositorios;

public sealed class ReservaRepositorio : IReservaRepositorio
{
    private readonly ReservaDbContext _contexto;

    public ReservaRepositorio(ReservaDbContext contexto)
    {
        _contexto = contexto;
    }

    public Task<Reserva?> ObtenerPorIdAsync(int id)
        => _contexto.Reservas.FirstOrDefaultAsync(r => r.IdReserva == id);

    public async Task<IReadOnlyList<Reserva>> ListarTodasAsync()
        => await _contexto.Reservas
            .AsNoTracking()
            .OrderByDescending(r => r.FechaInicio)
            .ToListAsync();

    public async Task<IReadOnlyList<Reserva>> ListarActivasPorUnidadAsync(string unidadId)
        => await _contexto.Reservas
            .AsNoTracking()
            .Where(r => r.UnidadId == unidadId && r.Estado == EstadoReserva.Activa)
            .OrderBy(r => r.FechaInicio)
            .ToListAsync();

    public async Task<IReadOnlyList<Unidad>> ListarUnidadesAsync()
        => await _contexto.Unidades
            .AsNoTracking()
            .OrderBy(u => u.Id)
            .ToListAsync();

    public Task<bool> ExisteUnidadAsync(string unidadId)
        => _contexto.Unidades.AnyAsync(u => u.Id == unidadId);

    public async Task AgregarAsync(Reserva reserva)
    {
        _contexto.Reservas.Add(reserva);
        await _contexto.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Reserva reserva)
    {
        _contexto.Reservas.Update(reserva);
        await _contexto.SaveChangesAsync();
    }
}