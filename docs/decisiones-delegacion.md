# Decisiones de delegación

Este documento registra **qué se delegó al agente, qué se hizo a mano y por qué**, además de las decisiones de diseño tomadas durante el proyecto.

---

## 1. Criterio de delegación

**Se delega al agente** tareas acotadas, con resultado comprobable de forma automática:

- Compilar la solución y corregir errores de compilación.
- Escribir o completar pruebas unitarias a partir de reglas ya definidas.
- Implementar cambios pequeños y bien descritos (por ejemplo, un ajuste del menú).
- Revisar el repositorio y reportar hallazgos sin modificar.

**No se delega** :

- El alcance y las reglas de negocio (`docs/especificacion.md`).
- La arquitectura por capas y sus dependencias.
- La aprobación o el rechazo de cada cambio propuesto.
- La incorporación de paquetes y la publicación del repositorio.
- La redacción final de la documentación y las conclusiones.

**Cómo se controla:** cada tarea termina con `dotnet build` y `dotnet test`, revisión del `git diff` y un commit propio. Ver `docs/preparacion-agente.md`, apartado 3.

---

## 2. Decisiones de diseño del proyecto

| Decisión | Alternativas consideradas | Motivo |
|---|---|---|
| Arquitectura por capas (Dominio, Aplicación, Infraestructura, Consola) | Proyecto único | Separar reglas de negocio de la persistencia y la interfaz, y facilitar las pruebas | 
| SQLite con EF Core | Archivos de texto o JSON, base en memoria | Persistencia real sin instalar un servidor | 
| Un único repositorio (`IReservaRepositorio`) que incluye las unidades | Un repositorio por entidad | Simplicidad: el sistema tiene solo dos entidades | 
| Fechas en UTC, conversión a hora local solo en la consola | Guardar hora local | Evitar ambigüedades y errores por zona horaria | 
| Errores de negocio con `InvalidOperationException` y `ArgumentException` | Códigos de retorno o un tipo `Resultado` | Menos código y mensajes claros en el menú | 
| Inyección de dependencias con `ServiceCollection` | Crear objetos manualmente en `Program.cs` | Desacoplar las capas |
| `EnsureCreated` en lugar de migraciones | Migraciones de EF Core | Alcance pequeño, sin cambios de esquema previstos | 
| Elegir la unidad por número en el menú | Escribir el id a mano | Evitar errores de tipeo | 

---

## 3. Registro de delegación por tarea y propuestas del agente que rechacé o modifiqué

Parte del código inicial se obtuvo mediante consultas a un asistente de chat, copiado luego al repositorio. Todo el trabajo posterior de verificación, corrección y ampliación se realizó con el agente sobre el repositorio.

Registro de la sesión con GitHub Copilot (modo Agent)

Herramienta: GitHub Copilot en modo Agent, dentro de Visual Studio.
Repositorio: Sistema_reservas, con las instrucciones en .github/copilot-instructions.md y la bitácora en docs/progreso.md.
Primer mensaje: 2/11/2026, 21:51.

Primera ronda de tareas
1. Orientación, sin modificar nada
Mensaje que envié:
Lee .github/copilot-instructions.md y docs/progreso.md. Revisa la estructura de la solución y dime si respeta la arquitectura descrita. No modifiques ningún archivo todavía.

Qué hizo el agente: leyó las instrucciones y la bitácora, abrió el proyecto y unos 16 archivos de código, buscó referencias y ejecutó dotnet build y dotnet test.

Qué me devolvió:
La solución tiene un único proyecto (Sistema_reservas.csproj, .NET 10). Las capas están separadas por carpetas y namespaces (Dominio, Aplicación, Infraestructura, Presentacion, Tests), no por proyectos.
Concluyó que la arquitectura se respeta de forma organizativa pero no estricta. Faltan proyectos independientes por capa, un proyecto SistemaReservas.Consola y un proyecto de pruebas separado.
Detectó un error: en Reserva.cs, línea 18, FechaCreacionUtc se inicializa con DateTime.Now (hora local) aunque el nombre indica UTC.
No modificó ningún archivo.

2. Compilar y registrar
Mensaje que envié:
Ejecuta dotnet build en la solución. Si hay errores, corrígelos sin cambiar la arquitectura de capas ni agregar paquetes. Al terminar, muéstrame el resultado literal de dotnet build y registra la tarea en docs/progreso.md.

Qué hizo el agente: ejecutó dotnet build y no encontró errores, así que no cambió código. Añadió una entrada a docs/progreso.md. Luego, por iniciativa propia y siguiendo la regla de verificación obligatoria de mis instrucciones, ejecutó también dotnet test y actualizó la bitácora.

Resultados literales: “Compilación realizado correctamente en 1,6s” (build) y “Compilación realizado correctamente en 1,2s” (test). Confirmó que no agregó paquetes ni alteró la arquitectura.

3. Pruebas unitarias
Mensaje que envié:
Ejecuta dotnet test. Si faltan pruebas, crea en SistemaReservas.Tests los casos: crear con superposición lanza excepción, cancelar libera la unidad, crear en unidad inexistente falla y sembrar el inventario dos veces no duplica. Muéstrame el resultado literal y actualiza docs/progreso.md.

Qué hizo el agente: ejecutó dotnet test y buscó los cuatro casos por nombre. Los encontró todos en Sistema_reservas/Tests (namespace SistemaReservas.Tests):

Crear_con_superposicion_lanza_excepcion
Cancelar_libera_la_unidad
Crear_en_unidad_inexistente_lanza_excepcion
Sembrar_inventario_dos_veces_no_duplica

Decisión del agente: no crear pruebas duplicadas. Aclaró que la salida no muestra pruebas individuales porque no existe un proyecto de tests independiente. Actualizó docs/progreso.md.

4. Selección de unidad por número en el menú
Mensaje que envié:
En MenuPrincipal, en las opciones 2 y 3, reemplaza la entrada manual del id de unidad por una lista numerada de las unidades del inventario para elegir por número. Valida el rango. Ejecuta dotnet build y dotnet test, y actualiza docs/progreso.md.

Qué hizo el agente: leyó la bitácora y MenuPrincipal.cs. Concluyó que ElegirUnidadAsync ya cumplía lo pedido: muestra las unidades numeradas, devuelve el id elegido y repite la entrada si el número está fuera de rango. Por eso no modificó el archivo y anunció que ejecutaría las verificaciones y registraría la tarea.

Estado en el registro: la respuesta termina ahí. No aparecen los resultados de dotnet build y dotnet test de esta tarea ni su entrada en la bitácora.

5. Revisión final, solo reporte
Mensaje que envié:
Revisa todo el repositorio buscando: más de un archivo con instrucciones de nivel superior, métodos sin usar y fechas que no se manejen. Solo reporta hallazgos y no modifiques nada hasta que yo te lo pida.

Qué hizo el agente: leyó la bitácora y todos los archivos .cs, y ejecutó búsquedas de fechas, de uso de Console y await, y de referencias a CrearReservaDto.

Hallazgos que me devolvió:
Instrucciones de nivel superior: solo Program.cs las tiene, así que cumple la regla.
Métodos sin uso: ninguno. Detectó dos elementos que no son métodos y no se usan: la clase CrearReservaDto y la clase interna vacía Dependencias.
Fechas:
Confirmó DateTime.Now en Reserva.cs.
Señaló que ReservaServicio y DisponibilidadServicio no validan que las fechas recibidas sean UTC.
Indicó que ReservaDbContext no configura una conversión de fechas a UTC, y SQLite no conserva el tipo de fecha de forma fiable.
Como manejo correcto, destacó que MenuPrincipal interpreta la entrada como hora local, la convierte a UTC antes de llamar a los servicios y muestra las fechas en hora local. Las pruebas usan DateTime.UtcNow.

No modificó ningún archivo.

Segunda ronda

6. Orientación, repetida
Mensaje que envié:
Lee .github/copilot-instructions.md y docs/progreso.md. Resúmeme las reglas que vas a seguir y revisa si la estructura de la solución respeta la arquitectura descrita. No modifiques ningún archivo todavía.

Qué me devolvió:
Un resumen de las reglas: arquitectura por capas, un solo Program.cs con instrucciones de nivel superior, fechas en UTC, excepciones de negocio, sin paquetes NuGet sin aprobación, no tocar archivos fuera de la tarea, dotnet build y dotnet test antes de cerrar, y actualizar docs/progreso.md.
La misma conclusión de antes: respeta la arquitectura de forma organizativa, con un solo proyecto, la consola sin su proyecto propio, sin proyecto de pruebas independiente y con la diferencia de nombres Sistema_reservas frente a SistemaReservas.

No modificó ningún archivo.

7. Compilar y registrar, con rechazo de un comando
Mensaje que envié: el mismo del punto 2.
Qué hizo el agente: leyó la bitácora e intentó ejecutar dotnet build en la terminal, pero la ejecución del comando fue omitida o rechazada. Entonces usó la compilación integrada del entorno, que terminó sin errores. No cambió código ni paquetes, y actualizó docs/progreso.md.

Resultado literal: “Compilación correcta”. En esta tarea no ejecutó pruebas.
---

## 4. Riesgos de delegar y cómo se mitigaron

| Riesgo | Mitigación aplicada |
|---|---|
| Aceptar código que compila pero no cumple las reglas | Pruebas automáticas basadas en la especificación y revisión del `git diff` |
| El agente cambia la arquitectura sin avisar | Regla explícita en el archivo de instrucciones y revisión de cada diff |
| Instalación de paquetes innecesarios | Regla de pedir aprobación y revisión de los archivos `.csproj` |
| Pérdida de contexto entre sesiones | Archivo `docs/progreso.md` actualizado al terminar cada tarea |
| Afirmar que algo funciona sin comprobarlo | Obligación de reportar la salida literal de `dotnet build` y `dotnet test` |

---
