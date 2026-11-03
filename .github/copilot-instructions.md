# Instrucciones del agente — Sistema de Reservas

## Proyecto
Aplicación de consola en C# (.NET) para reservar equipos de laboratorio.
Persistencia con EF Core y SQLite.

## Arquitectura (respetar siempre)
- SistemaReservas.Dominio: entidades y enumeraciones. Sin dependencias.
- SistemaReservas.Aplicacion: servicios e interfaces (IReservaRepositorio). Depende solo de Dominio.
- SistemaReservas.Infraestructura: DbContext, repositorio y datos iniciales. Depende de Aplicacion.
- SistemaReservas.Consola: menú y composición (inyección de dependencias).
- SistemaReservas.Tests: pruebas xUnit.

## Reglas
- Solo SistemaReservas.Consola/Program.cs puede tener instrucciones de nivel superior.
- Todas las fechas se almacenan y comparan en UTC; la consola convierte a hora local.
- Los servicios lanzan InvalidOperationException o ArgumentException para errores de negocio.
- No agregar paquetes NuGet sin pedir aprobación.
- No modificar archivos fuera de lo que pide la tarea.

## Verificación obligatoria
Antes de dar por terminada una tarea: ejecutar dotnet build y dotnet test
y reportar el resultado literal. Si algo falla, no declarar la tarea como completada.

## Persistencia de progreso
- Al empezar: leer docs/progreso.md.
- Al terminar cada tarea: añadir una entrada con fecha, cambios, decisiones,
  resultado de build/tests y pendientes.

## Turnos consecutivos
Si el usuario envía varias instrucciones seguidas, resumir primero qué tareas
entendiste y en qué orden las harás, y esperar confirmación si hay ambigüedad.
