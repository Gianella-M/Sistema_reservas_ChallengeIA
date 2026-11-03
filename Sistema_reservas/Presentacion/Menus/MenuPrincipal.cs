using System.Globalization;
using Sistema_reservas.Aplicación.Servicios;

namespace Sistema_reservas.Presentacion;

public sealed class MenuPrincipal
{
    private const string Formato = "dd/MM/yyyy HH:mm";

    private readonly ReservaServicio _reservas;
    private readonly DisponibilidadServicio _disponibilidad;

    public MenuPrincipal(ReservaServicio reservas, DisponibilidadServicio disponibilidad)
    {
        _reservas = reservas;
        _disponibilidad = disponibilidad;
    }

    public async Task IniciarAsync()
    {
        var salir = false;

        while (!salir)
        {
            MostrarMenu();
            var opcion = Console.ReadLine()?.Trim();

            try
            {
                switch (opcion)
                {
                    case "1": await ConsultarInventarioAsync(); break;
                    case "2": await ConsultarDisponibilidadAsync(); break;
                    case "3": await CrearReservaAsync(); break;
                    case "4": await CancelarReservaAsync(); break;
                    case "5": await ConsultarHistorialAsync(); break;
                    case "0": salir = true; break;
                    default: Console.WriteLine("Opción no válida."); break;
                }
            }
            catch (Exception ex) when (ex is InvalidOperationException or ArgumentException)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            if (!salir)
            {
                Console.Write("\nPresione una tecla para continuar...");
                Console.ReadKey(true);
            }
        }

        Console.WriteLine("Hasta luego.");
    }

    private static void MostrarMenu()
    {
        Console.Clear();
        Console.WriteLine("=================================");
        Console.WriteLine("   SISTEMA DE RESERVAS");
        Console.WriteLine("=================================");
        Console.WriteLine();
        Console.WriteLine("1. Consultar inventario");
        Console.WriteLine("2. Consultar disponibilidad");
        Console.WriteLine("3. Crear reserva");
        Console.WriteLine("4. Cancelar reserva");
        Console.WriteLine("5. Consultar historial");
        Console.WriteLine("0. Salir");
        Console.WriteLine();
        Console.Write("Seleccione una opción: ");
    }

    private async Task ConsultarInventarioAsync()
    {
        var unidades = await _reservas.ObtenerInventarioAsync();
        Console.WriteLine();
        foreach (var u in unidades)
            Console.WriteLine($"{u.Id,-8} {u.Nombre} ({u.Tipo})");
    }

    private async Task ConsultarDisponibilidadAsync()
    {
        var unidadId = await ElegirUnidadAsync();
        var desde = LeerFecha("Desde (dd/MM/yyyy HH:mm): ");
        var hasta = LeerFecha("Hasta (dd/MM/yyyy HH:mm): ");

        var libre = await _disponibilidad.EstaDisponibleAsync(
            unidadId, desde.ToUniversalTime(), hasta.ToUniversalTime());

        Console.WriteLine(libre ? "\nLa unidad está DISPONIBLE." : "\nLa unidad está OCUPADA en ese horario.");

        var ocupacion = await _disponibilidad.ConsultarOcupacionAsync(unidadId);
        if (ocupacion.Count > 0)
        {
            Console.WriteLine("\nReservas activas de la unidad:");
            foreach (var r in ocupacion)
                Console.WriteLine($"  {Mostrar(r.FechaInicio)} -> {Mostrar(r.FechaFin)} ({r.Solicitante})");
        }
    }

    private async Task CrearReservaAsync()
    {
        var unidadId = await ElegirUnidadAsync();
        var solicitante = LeerTexto("Solicitante: ");
        var inicio = LeerFecha("Inicio (dd/MM/yyyy HH:mm): ");
        var fin = LeerFecha("Fin (dd/MM/yyyy HH:mm): ");

        var reserva = await _reservas.CrearAsync(
            unidadId, solicitante, inicio.ToUniversalTime(), fin.ToUniversalTime());

        Console.WriteLine($"\nReserva creada. Id: {reserva.IdReserva}");
    }

    private async Task CancelarReservaAsync()
    {
        var id = LeerEntero("Id de la reserva a cancelar: ");
        await _reservas.CancelarAsync(id);
        Console.WriteLine("\nReserva cancelada.");
    }

    private async Task ConsultarHistorialAsync()
    {
        var historial = await _reservas.ListarHistorialAsync();
        Console.WriteLine();

        if (historial.Count == 0)
        {
            Console.WriteLine("No hay reservas registradas.");
            return;
        }

        foreach (var r in historial)
            Console.WriteLine(
                $"#{r.IdReserva} | {r.UnidadId} | {r.Solicitante} | {Mostrar(r.FechaInicio)} -> {Mostrar(r.FechaFin)} | {r.Estado}");
    }

    private static string Mostrar(DateTime utc)
        => utc.ToLocalTime().ToString(Formato, CultureInfo.InvariantCulture);

    private static string LeerTexto(string mensaje)
    {
        while (true)
        {
            Console.Write(mensaje);
            var texto = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(texto)) return texto;
            Console.WriteLine("El valor no puede estar vacío.");
        }
    }

    private static int LeerEntero(string mensaje)
    {
        while (true)
        {
            Console.Write(mensaje);
            if (int.TryParse(Console.ReadLine(), out var valor)) return valor;
            Console.WriteLine("Ingrese un número válido.");
        }
    }

    private static DateTime LeerFecha(string mensaje)
    {
        while (true)
        {
            Console.Write(mensaje);
            if (DateTime.TryParseExact(Console.ReadLine(), Formato, CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeLocal, out var fecha))
                return fecha;
            Console.WriteLine("Formato inválido. Ejemplo: 05/11/2026 14:30");
        }
    }
    private async Task<string> ElegirUnidadAsync()
    {
        var unidades = await _reservas.ObtenerInventarioAsync();

        Console.WriteLine();
        for (var i = 0; i < unidades.Count; i++)
            Console.WriteLine($"{i + 1}. {unidades[i].Id} - {unidades[i].Nombre}");

        while (true)
        {
            var numero = LeerEntero("Seleccione la unidad (número): ");
            if (numero >= 1 && numero <= unidades.Count)
                return unidades[numero - 1].Id;

            Console.WriteLine("Número fuera de rango.");
        }
    }
}