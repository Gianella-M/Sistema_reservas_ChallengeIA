# Sistema de Reservas de equipos de laboratorio

Aplicación de consola en C# (.NET) para consultar el inventario, verificar disponibilidad, crear y cancelar reservas y revisar el historial. Persistencia con Entity Framework Core y SQLite.

Proyecto del **Challenge IA**: desarrollo con un agente de programación que actúa sobre el repositorio.

Autor: [COMPLETAR nombre y legajo]
Materia / cátedra: [COMPLETAR]

## Requisitos

- .NET SDK [COMPLETAR versión, visible con `dotnet --version`]
- Git

No hace falta instalar ninguna base de datos: SQLite crea el archivo `reservas.db` al primer arranque.

## Cómo ejecutar

```bash
dotnet build
dotnet run --project SistemaReservas.Consola
```

## Cómo ejecutar las pruebas

```bash
dotnet test
```

## Menú

Las fechas se ingresan con el formato `dd/MM/yyyy HH:mm` (hora local). Ejemplo: `05/11/2026 14:30`.

## Estructura

Las dependencias van hacia adentro: Consola, Infraestructura y Tests dependen de Aplicación, y Aplicación depende solo de Dominio.

## Documentación

| Documento | Contenido |
|---|---|
| [docs/especificacion.md](docs/especificacion.md) | Alcance, requisitos y reglas de negocio |
| [docs/preparacion-agente.md](docs/preparacion-agente.md) | Agente, modelo, instrucciones, permisos, aprobaciones y verificación |
| [docs/decisiones-delegacion.md](docs/decisiones-delegacion.md) | Qué se delegó al agente y qué se decidió a mano |
| [docs/progreso.md](docs/progreso.md) | Bitácora de tareas realizadas con el agente |

## Uso del agente

El trabajo se realizó con [COMPLETAR agente y modelo] sobre este repositorio. Las instrucciones permanentes están en [COMPLETAR ruta, p. ej. `.github/copilot-instructions.md`] y la evidencia en `docs/evidencias/`.
