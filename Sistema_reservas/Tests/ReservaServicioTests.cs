using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Sistema_reservas.Aplicación.Servicios;
using Sistema_reservas.Dominio.Entidades;
using Sistema_reservas.Dominio.Enumeraciones;
using Sistema_reservas.Infraestructura.Persistencia;
using Sistema_reservas.Infraestructura.Persistencia.Repositorios;
using Xunit;

namespace SistemaReservas.Tests;

internal static class EntornoPrueba
{
    public static (ReservaDbContext Contexto, ReservaRepositorio Repo) Crear(bool conUnidad = true)
    {
        var conexion = new SqliteConnection("DataSource=:memory:");
        conexion.Open();

        var opciones = new DbContextOptionsBuilder<ReservaDbContext>().UseSqlite(conexion).Options;
        var contexto = new ReservaDbContext(opciones);
        contexto.Database.EnsureCreated();

        if (conUnidad)
        {
            contexto.Unidades.Add(new Unidad { Id = "U1", Nombre = "Equipo 1", Tipo = "Prueba" });
            contexto.SaveChanges();
        }

        return (contexto, new ReservaRepositorio(contexto));
    }
}

public class ReservaServicioTests
{
    private static readonly DateTime Base = DateTime.UtcNow.Date.AddDays(1).AddHours(10);

    private static ReservaServicio CrearServicio()
    {
        var (_, repo) = EntornoPrueba.Crear();
        return new ReservaServicio(repo, new DisponibilidadServicio(repo));
    }

    [Fact]
    public async Task Crear_con_superposicion_lanza_excepcion()
    {
        var servicio = CrearServicio();
        await servicio.CrearAsync("U1", "Ana", Base, Base.AddHours(2));

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            servicio.CrearAsync("U1", "Luis", Base.AddHours(1), Base.AddHours(3)));
    }

    [Fact]
    public async Task Cancelar_libera_la_unidad()
    {
        var servicio = CrearServicio();
        var primera = await servicio.CrearAsync("U1", "Ana", Base, Base.AddHours(2));

        await servicio.CancelarAsync(primera.IdReserva);
        var segunda = await servicio.CrearAsync("U1", "Luis", Base, Base.AddHours(2));

        Assert.Equal(EstadoReserva.Activa, segunda.Estado);
    }

    [Fact]
    public async Task Crear_en_unidad_inexistente_lanza_excepcion()
    {
        var servicio = CrearServicio();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            servicio.CrearAsync("NO-EXISTE", "Ana", Base, Base.AddHours(2)));
    }
}