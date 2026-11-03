# Decisiones de delegación

Este documento registra **qué se delegó al agente, qué se hizo a mano y por qué**, además de las decisiones de diseño tomadas durante el proyecto.

---

## 1. Criterio de delegación

**Se delega al agente** tareas acotadas, con resultado comprobable de forma automática:

- Compilar la solución y corregir errores de compilación.
- Escribir o completar pruebas unitarias a partir de reglas ya definidas.
- Implementar cambios pequeños y bien descritos (por ejemplo, un ajuste del menú).
- Revisar el repositorio y reportar hallazgos sin modificar.

**No se delega** (decide la persona):

- El alcance y las reglas de negocio (`docs/especificacion.md`).
- La arquitectura por capas y sus dependencias.
- La aprobación o el rechazo de cada cambio propuesto.
- La incorporación de paquetes y la publicación del repositorio.
- La redacción final de la documentación y las conclusiones.

**Cómo se controla:** cada tarea termina con `dotnet build` y `dotnet test`, revisión del `git diff` y un commit propio. Ver `docs/preparacion-agente.md`, apartado 3.

---

## 2. Decisiones de diseño del proyecto

| Decisión | Alternativas consideradas | Motivo | Quién la tomó |
|---|---|---|---|
| Arquitectura por capas (Dominio, Aplicación, Infraestructura, Consola) | Proyecto único | Separar reglas de negocio de la persistencia y la interfaz, y facilitar las pruebas | [COMPLETAR] |
| SQLite con EF Core | Archivos de texto o JSON, base en memoria | Persistencia real sin instalar un servidor | [COMPLETAR] |
| Un único repositorio (`IReservaRepositorio`) que incluye las unidades | Un repositorio por entidad | Simplicidad: el sistema tiene solo dos entidades | [COMPLETAR] |
| Fechas en UTC, conversión a hora local solo en la consola | Guardar hora local | Evitar ambigüedades y errores por zona horaria | [COMPLETAR] |
| Errores de negocio con `InvalidOperationException` y `ArgumentException` | Códigos de retorno o un tipo `Resultado` | Menos código y mensajes claros en el menú | [COMPLETAR] |
| Inyección de dependencias con `ServiceCollection` | Crear objetos manualmente en `Program.cs` | Desacoplar las capas | [COMPLETAR] |
| `EnsureCreated` en lugar de migraciones | Migraciones de EF Core | Alcance pequeño, sin cambios de esquema previstos | [COMPLETAR] |
| Elegir la unidad por número en el menú | Escribir el id a mano | Evitar errores de tipeo | [COMPLETAR] |

---

## 3. Registro de delegación por tarea

| # | Tarea | Delegada al agente | Hecha por mí | Resultado | Commit |
|---|---|---|---|---|---|
| 1 | Verificar compilación y corregir errores | [COMPLETAR] | [COMPLETAR] | [COMPLETAR] | [COMPLETAR] |
| 2 | Pruebas unitarias y de persistencia | [COMPLETAR] | [COMPLETAR] | [COMPLETAR] | [COMPLETAR] |
| 3 | Selección de unidad por número en el menú | [COMPLETAR] | [COMPLETAR] | [COMPLETAR] | [COMPLETAR] |
| 4 | Revisión final del repositorio (solo reporte) | [COMPLETAR] | [COMPLETAR] | [COMPLETAR] | [COMPLETAR] |
| 5 | [COMPLETAR] | [COMPLETAR] | [COMPLETAR] | [COMPLETAR] | [COMPLETAR] |

---

## 4. Propuestas del agente que rechacé o modifiqué

| Fecha | Qué propuso el agente | Decisión | Motivo |
|---|---|---|---|
| [COMPLETAR] | [COMPLETAR] | [Rechazada / Modificada] | [COMPLETAR] |

Si no hubo ninguna, escribirlo explícitamente: **"No se rechazó ninguna propuesta"**, y explicar brevemente cómo se verificó que lo aceptado era correcto.

---

## 5. Origen del código inicial

Parte del código inicial se obtuvo mediante consultas a un asistente de chat, copiado luego al repositorio (ver `docs/preparacion-agente.md`, apartado 1.3). Todo el trabajo posterior de verificación, corrección y ampliación se realizó con el agente sobre el repositorio.

- Partes que provienen del chat: [COMPLETAR]
- Partes modificadas por el agente: [COMPLETAR]
- Partes escritas por mí: [COMPLETAR]

---

## 6. Riesgos de delegar y cómo se mitigaron

| Riesgo | Mitigación aplicada |
|---|---|
| Aceptar código que compila pero no cumple las reglas | Pruebas automáticas basadas en la especificación y revisión del `git diff` |
| El agente cambia la arquitectura sin avisar | Regla explícita en el archivo de instrucciones y revisión de cada diff |
| Instalación de paquetes innecesarios | Regla de pedir aprobación y revisión de los archivos `.csproj` |
| Pérdida de contexto entre sesiones | Archivo `docs/progreso.md` actualizado al terminar cada tarea |
| Afirmar que algo funciona sin comprobarlo | Obligación de reportar la salida literal de `dotnet build` y `dotnet test` |

---

## 7. Lecciones aprendidas

- Qué funcionó bien al delegar: [COMPLETAR]
- Qué tuve que corregir o volver a pedir: [COMPLETAR]
- Qué haría distinto: [COMPLETAR]
