# Especificación del Sistema de Reservas

## 1. Propósito

Aplicación de consola que permite gestionar la reserva de equipos de laboratorio: consultar el inventario, verificar disponibilidad, crear y cancelar reservas y revisar el historial. Los datos persisten en una base SQLite local.

## 2. Alcance

**Incluye**

- Inventario de unidades (equipos) cargado automáticamente en el primer arranque.
- Consulta de disponibilidad por unidad y rango horario.
- Creación y cancelación de reservas.
- Historial de todas las reservas.
- Persistencia local y pruebas automáticas.

**No incluye**

- Autenticación ni perfiles de usuario (el solicitante se escribe como texto).
- Edición de una reserva existente (solo se crea o se cancela).
- Interfaz gráfica o web.
- Acceso concurrente de varios usuarios.
- Alta, baja o modificación de unidades desde el menú.

## 3. Actor

**Usuario de laboratorio:** persona que opera la consola para consultar y reservar equipos. No hay roles diferenciados.

## 4. Requisitos funcionales

| Id | Opción del menú | Descripción |
|---|---|---|
| RF1 | 1. Consultar inventario | Muestra todas las unidades con su id, nombre y tipo. |
| RF2 | 2. Consultar disponibilidad | Dada una unidad y un rango de fechas, indica si está disponible y lista las reservas activas de esa unidad. |
| RF3 | 3. Crear reserva | Registra una reserva para una unidad, un solicitante y un rango de fechas. |
| RF4 | 4. Cancelar reserva | Cambia el estado de una reserva existente a Cancelada. |
| RF5 | 5. Consultar historial | Lista todas las reservas, de la más reciente a la más antigua, con su estado. |
| RF6 | 0. Salir | Cierra la aplicación. |

## 5. Reglas de negocio

| Id | Regla |
|---|---|
| RN1 | La unidad indicada debe existir en el inventario. |
| RN2 | La fecha de inicio debe ser anterior a la fecha de fin. |
| RN3 | No se puede crear una reserva con inicio en el pasado. |
| RN4 | El solicitante es obligatorio. |
| RN5 | Dos reservas activas de la misma unidad no pueden solaparse. Hay solapamiento cuando `inicioNueva < finExistente` **y** `finNueva > inicioExistente`. |
| RN6 | Una reserva cancelada no bloquea la unidad: su horario vuelve a estar disponible. |
| RN7 | Una reserva ya cancelada no puede cancelarse de nuevo. |
| RN8 | Las fechas se almacenan y comparan en UTC. La consola las pide y las muestra en hora local. |
| RN9 | El historial se ordena por fecha de inicio descendente. |

## 6. Modelo de datos

**Unidad**

| Campo | Tipo | Descripción |
|---|---|---|
| Id | texto | Identificador único (p. ej. `MIC-01`). |
| Nombre | texto | Nombre del equipo. |
| Tipo | texto | Categoría del equipo. |

**Reserva**

| Campo | Tipo | Descripción |
|---|---|---|
| Id | entero | Generado automáticamente. |
| UnidadId | texto | Unidad reservada. |
| Solicitante | texto | Persona que reserva. |
| FechaInicioUtc | fecha y hora | Inicio de la reserva. |
| FechaFinUtc | fecha y hora | Fin de la reserva. |
| Estado | Activa / Cancelada | Estado actual. |
| FechaCreacionUtc | fecha y hora | Momento del registro. |

## 7. Inventario inicial

| Id | Nombre | Tipo |
|---|---|---|
| MIC-01 | Microscopio óptico 1 | Microscopio |
| MIC-02 | Microscopio óptico 2 | Microscopio |
| CEN-01 | Centrífuga | Centrífuga |
| ESP-01 | Espectrofotómetro | Espectrofotómetro |
| BAL-01 | Balanza analítica | Balanza |

Se carga solo si la tabla de unidades está vacía, por lo que repetir el arranque no duplica datos.

## 8. Mensajes de error esperados

| Situación | Mensaje |
|---|---|
| Unidad inexistente | `Error: La unidad no existe.` |
| Rango de fechas inválido | `Error: El intervalo de fechas no es válido.` |
| Reserva en el pasado | `Error: No se puede reservar en el pasado.` |
| Horario ocupado | `Error: La unidad no está disponible en ese horario.` |
| Reserva inexistente | `Error: La reserva no existe.` |
| Reserva ya cancelada | `Error: La reserva ya está cancelada.` |
| Opción de menú inválida | `Opción no válida.` |
| Fecha con formato incorrecto | `Formato inválido. Ejemplo: 05/11/2026 14:30` |

## 9. Requisitos no funcionales

- Lenguaje y plataforma: C# sobre .NET.
- Persistencia: Entity Framework Core con SQLite (`reservas.db`).
- Arquitectura por capas: Dominio, Aplicación, Infraestructura y Consola. Las dependencias van hacia adentro.
- Inyección de dependencias en el arranque.
- Pruebas automáticas con xUnit sobre SQLite en memoria.
- Los errores de negocio no cierran la aplicación; se informan en pantalla.

## 10. Criterios de aceptación

1. La solución compila sin errores con `dotnet build`.
2. `dotnet test` termina con todas las pruebas en verde.
3. En un primer arranque, la opción 1 muestra las 5 unidades del inventario.
4. Crear una reserva y luego otra solapada sobre la misma unidad devuelve el error de horario ocupado.
5. Cancelar la primera reserva permite crear nuevamente la segunda.
6. El historial muestra ambas reservas con su estado correcto.
7. Cerrar y volver a abrir la aplicación conserva las reservas.
