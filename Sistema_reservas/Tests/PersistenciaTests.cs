using Sistema_reservas.Dominio.Entidades;
using Sistema_reservas.Infraestructura.Datos;
using Xunit;

namespace SistemaReservas.Tests;

public class PersistenciaTests
{
    [Fact]
    public async Task Agregar_y_obtener_reserva()
    {
        var (_, repo) = EntornoPrueba.Crear();
        var inicio = DateTime.UtcNow.AddDays(1);

        var reserva = new Reserva
        {
            UnidadId = "U1",
            Solicitante = "Ana",
            FechaInicio = inicio,
            FechaFin = inicio.AddHours(1)
        };

        await repo.AgregarAsync(reserva);
        var leida = await repo.ObtenerPorIdAsync(reserva.IdReserva);

        Assert.NotNull(leida);
        Assert.Equal("Ana", leida!.Solicitante);
    }

    [Fact]
    public async Task Sembrar_inventario_dos_veces_no_duplica()
    {
        var (contexto, repo) = EntornoPrueba.Crear(conUnidad: false);

        await InventarioInicial.SembrarAsync(contexto);
        await InventarioInicial.SembrarAsync(contexto);

        var unidades = await repo.ListarUnidadesAsync();
        Assert.Equal(InventarioInicial.Obtener().Count, unidades.Count);
    }
}