using Sistema_reservas.Dominio.Entidades;
using Sistema_reservas.Infraestructura.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace Sistema_reservas.Infraestructura.Datos;

public static class InventarioInicial
{
    public static IReadOnlyList<Unidad> Obtener() => new List<Unidad>
    {
        new() { Id = "MIC-01", Nombre = "Microscopio óptico 1", Tipo = "Microscopio" },
        new() { Id = "MIC-02", Nombre = "Microscopio óptico 2", Tipo = "Microscopio" },
        new() { Id = "OSC-01", Nombre = "Osciloscopio", Tipo = "Osciloscopio" },
        new() { Id = "MUL-01", Nombre = "Multímetro", Tipo = "Multímetro" },
        new() { Id = "FUE-01", Nombre = "Fuente de alimentación", Tipo = "Fuente de alimentación" }
    };

    public static async Task SembrarAsync(ReservaDbContext contexto)
    {
        if (await contexto.Unidades.AnyAsync())
            return;

        contexto.Unidades.AddRange(Obtener());
        await contexto.SaveChangesAsync();
    }
}
