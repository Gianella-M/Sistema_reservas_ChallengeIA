using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sistema_reservas.Aplicación.Abstracciones.Persistencia;
using Sistema_reservas.Aplicación.Servicios;
using Sistema_reservas.Infraestructura.Datos;
using Sistema_reservas.Infraestructura.Persistencia;
using Sistema_reservas.Infraestructura.Persistencia.Repositorios;
using Sistema_reservas.Presentacion;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var servicios = new ServiceCollection();

servicios.AddDbContext<ReservaDbContext>(o => o.UseSqlite("Data Source=reservas.db"));
servicios.AddScoped<IReservaRepositorio, ReservaRepositorio>();
servicios.AddScoped<DisponibilidadServicio>();
servicios.AddScoped<ReservaServicio>();
servicios.AddScoped<MenuPrincipal>();

using var proveedor = servicios.BuildServiceProvider();
using var ambito = proveedor.CreateScope();

var contexto = ambito.ServiceProvider.GetRequiredService<ReservaDbContext>();
await contexto.Database.EnsureCreatedAsync();
await InventarioInicial.SembrarAsync(contexto);

var menu = ambito.ServiceProvider.GetRequiredService<MenuPrincipal>();
await menu.IniciarAsync();