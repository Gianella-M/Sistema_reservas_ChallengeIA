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

👤 (2/11/2026 21:51)
Lee .github/copilot-instructions.md y docs/progreso.md. Revisa la estructura de la solución y dime si respeta la arquitectura descrita. No modifiques ningún archivo todavía.

💬 GitHub Copilot
Leer copilot-instructions.md, líneas 1 a 200 ✅

Leer progreso.md, líneas 1 a 300 ✅

Leer 1 proyecto ✅

Leer 16 archivos en Sistema_reservas\Sistema_reservas.csproj ✅

Leer Sistema_reservas.csproj, líneas 1 a 200 ✅

Se encontraron 50 coincidencias para ProjectReference|PackageReference|DateTime|DateTimeOffset|DateTimeKind|top-level|using ✅

Leer Program.cs, líneas 1 a 240 ✅

Leer IReservaRepositorio.cs, líneas 1 a 200 ✅

Leer Dependencias.cs, líneas 1 a 200 ✅

Leer Reserva.cs, líneas 1 a 160 ✅

Leer Unidad.cs, líneas 1 a 160 ✅

Leer ReservaServicio.cs, líneas 1 a 180 ✅

Leer DisponibilidadServicio.cs, líneas 1 a 160 ✅

Leer ReservaDbContext.cs, líneas 1 a 180 ✅

Leer ReservaRepositorio.cs, líneas 1 a 220 ✅

Leer MenuPrincipal.cs, líneas 1 a 240 ✅

Leer PersistenciaTests.cs, líneas 1 a 180 ✅

Leer InventarioInicial.cs, líneas 1 a 180 ✅

Leer ReservaServicioTests.cs, líneas 1 a 220 ✅

Leer CrearReservaDto.cs, líneas 1 a 120 ✅

Leer EstadoReserva.cs, líneas 1 a 80 ✅

Leer InventarioInicial.cs, líneas 1 a 180 ✅ He revisado las instrucciones y el registro de progreso. Ahora voy a verificar la compilación y las pruebas existentes para incluir su resultado literal en la evaluación, sin modificar archivos.

Comando ejecutado: dotnet build; dotnet test ✅

Resultado
La solución no respeta completamente la arquitectura descrita. La funcionalidad está separada por carpetas y namespaces, pero no por proyectos o ensamblados.

Estructura actual
La solución contiene un único proyecto:

Sistema_reservas/Sistema_reservas.csproj
Target Framework: .NET 10
Todos los componentes están incluidos en el mismo .csproj.
Carpetas detectadas:

Dominio
Entidades
Enumeraciones
Aplicación
Servicios
DTOs
Abstracciones
Infraestructura
Persistencia
Datos
Presentacion
Menús
Tests
Program.cs
Comparación con la arquitectura esperada
Área	Estado	Observaciones
Dominio	Parcialmente conforme	Contiene entidades y enumeraciones, sin dependencias externas relevantes.
Aplicación	Parcialmente conforme	Contiene servicios e IReservaRepositorio, y depende conceptualmente del dominio.
Infraestructura	Parcialmente conforme	Contiene ReservaDbContext, repositorio y datos iniciales. Usa EF Core y depende de Aplicación/Dominio.
Consola	No conforme exactamente	No existe SistemaReservas.Consola; Program.cs está en la raíz y el menú está en Presentacion.
Tests	No conforme	Los tests están dentro del mismo proyecto ejecutable, no en un proyecto independiente SistemaReservas.Tests.
Problema principal
Al existir un único proyecto, las dependencias entre capas no están realmente restringidas por el compilador. Por ejemplo, cualquier carpeta puede referenciar directamente cualquier otra parte del sistema. La separación actual es organizativa, no arquitectónica.

Reglas revisadas
Solo Program.cs contiene instrucciones de nivel superior: conforme.

La consola convierte fechas locales a UTC antes de llamar a los servicios y muestra fechas en hora local: mayormente conforme.

Se detectó una excepción en Dominio/Entidades/Reserva.cs, línea 18:

FechaCreacionUtc se inicializa con DateTime.Now, que representa hora local, pese a indicar UTC.

Los servicios utilizan ArgumentException e InvalidOperationException: conforme.

No se modificó ningún archivo, incluido docs/progreso.md, porque la solicitud indicó expresamente que no se modificara nada.

Verificación
Resultado literal de la compilación:

Compilación realizado correctamente en 4,4s

Resultado literal de dotnet test:

Compilación realizado correctamente en 1,2s

No se informó ejecución de tests individuales, porque los tests están dentro del proyecto ejecutable y no existe un proyecto de pruebas independiente configurado.

Conclusión
La estructura se aproxima funcionalmente a la arquitectura solicitada, pero no la cumple de forma estricta. Las principales diferencias son:

Falta de proyectos independientes para cada capa.
Ausencia de SistemaReservas.Consola como proyecto o área con ese nombre.
Ausencia de un proyecto independiente SistemaReservas.Tests.
Inicialización incorrecta de FechaCreacionUtc con hora local.
👤
Ejecuta dotnet build en la solución. Si hay errores, corrígelos sin cambiar la arquitectura de capas ni agregar paquetes. Al terminar, muéstrame el resultado literal de dotnet build y registra la tarea en docs/progreso.md.

💬 GitHub Copilot
Voy a leer el progreso y ejecutar dotnet build sobre la solución. Solo corregiré errores de compilación si aparecen, sin añadir paquetes ni alterar la arquitectura; después registraré el resultado en docs/progreso.md.

Leer progreso.md, líneas 1 a 200 ✅
Comando ejecutado: dotnet build ✅ La compilación no presenta errores, por lo que no es necesario modificar código. Voy a añadir únicamente la entrada de bitácora solicitada en docs/progreso.md, incluyendo el resultado literal de dotnet build.
Comando ejecutado: Get-Date -Format 'yyyy-MM-dd' ✅
Modifying progreso.md La entrada quedó registrada. Según las instrucciones del repositorio, también debo ejecutar dotnet test antes de cerrar la tarea; lo haré sin modificar código y actualizaré la bitácora si corresponde.
Comando ejecutado: dotnet test ✅
Leer progreso.md, líneas 1 a 40 ✅
Modifying progreso.md dotnet build se ejecutó correctamente. No se encontraron errores y no fue necesario modificar código.
Resultado literal:

Compilación realizado correctamente en 1,6s

También se ejecutó la verificación obligatoria:

Compilación realizado correctamente en 1,2s

La tarea quedó registrada en docs/progreso.md. No se agregaron paquetes ni se modificó la arquitectura de capas.

👤
Ejecuta dotnet test. Si faltan pruebas, crea en SistemaReservas.Tests los casos: crear con superposición lanza excepción, cancelar libera la unidad, crear en unidad inexistente falla y sembrar el inventario dos veces no duplica. Muéstrame el resultado literal y actualiza docs/progreso.md.

💬 GitHub Copilot
Voy a leer la bitácora y ejecutar dotnet test. Después comprobaré si los cuatro casos solicitados están cubiertos; solo crearé pruebas si falta alguno, y registraré el resultado literal en docs/progreso.md.

Leer progreso.md, líneas 1 a 120 ✅

Comando ejecutado: dotnet test ✅

Se encontraron 4 coincidencias para Crear_con_superposicion|Cancelar_libera|Crear_en_unidad_inexistente|Sembrar_inventario_dos_veces ✅

Leer 1 proyecto ✅

Leer 16 archivos en Sistema_reservas\Sistema_reservas.csproj ✅ Los cuatro casos solicitados ya existen en Sistema_reservas/Tests bajo el namespace SistemaReservas.Tests:

Crear_con_superposicion_lanza_excepcion
Cancelar_libera_la_unidad
Crear_en_unidad_inexistente_lanza_excepcion
Sembrar_inventario_dos_veces_no_duplica
Por tanto, no crearé duplicados. dotnet test terminó correctamente, aunque la salida no muestra pruebas individuales ejecutadas porque actualmente solo hay un proyecto ejecutable y no un proyecto de tests independiente. Registraré este resultado sin cambiar la arquitectura.

Modifying progreso.md dotnet test terminó correctamente.
Resultado literal:

Compilación realizado correctamente en 1,1s

Los cuatro casos solicitados ya estaban presentes en Sistema_reservas/Tests, dentro del namespace SistemaReservas.Tests, por lo que no se crearon duplicados:

Superposición.
Cancelación que libera la unidad.
Unidad inexistente.
Siembra idempotente del inventario.
La salida no muestra pruebas individuales ejecutadas porque la solución todavía contiene un único proyecto ejecutable y no un proyecto de tests independiente. Se actualizó docs/progreso.md.

👤
En MenuPrincipal, en las opciones 2 y 3, reemplaza la entrada manual del id de unidad por una lista numerada de las unidades del inventario para elegir por número. Valida el rango. Ejecuta dotnet build y dotnet test, y actualiza docs/progreso.md.

💬 GitHub Copilot
Voy a revisar la bitácora y la implementación actual de MenuPrincipal, modificar la selección de unidad para las opciones 2 y 3 con validación de rango, y después ejecutar dotnet build y dotnet test antes de registrar los resultados.

Leer progreso.md, líneas 1 a 160 ✅
Leer MenuPrincipal.cs, líneas 1 a 230 ✅
Leer MenuPrincipal.cs, líneas 172 a 210 ✅ La implementación actual ya cumple el cambio solicitado: ElegirUnidadAsync muestra las unidades numeradas, devuelve el Id seleccionado y repite la entrada cuando el número está fuera del rango. No modificaré MenuPrincipal; ejecutaré las verificaciones y registraré la tarea.
👤
Revisa todo el repositorio buscando: más de un archivo con instrucciones de nivel superior, métodos sin usar y fechas que no se manejen. Solo reporta hallazgos y no modifiques nada hasta que yo te lo pida.

💬 GitHub Copilot
Voy a leer la bitácora y revisar todos los archivos del repositorio para localizar instrucciones de nivel superior, métodos posiblemente sin uso y manejo incompleto de fechas. No modificaré ningún archivo.

Leer progreso.md, líneas 1 a 200 ✅

Leer 1 proyecto ✅

Leer 16 archivos para '.cs', 'README', 'docs', 'copilot-instructions' ✅

Audit top-level, unused, UTC ✅

Se encontraron 17 coincidencias para DateTime|ToUniversalTime|ToLocalTime|AssumeLocal|UtcNow|DateTime.Now ✅

Se encontraron 50 coincidencias para Console.|await |using var|var servicios|var proveedor|var ambito ✅

Se encontraron 1 coincidencias para CrearReservaDto ✅

Hallazgos
1. Instrucciones de nivel superior
No se detectó más de un archivo con instrucciones ejecutables de nivel superior.

Sistema_reservas/Program.cs:10-28
Es el único archivo con instrucciones de nivel superior.
El resto de los archivos contiene namespaces, clases, interfaces o enumeraciones.
Resultado: cumple la regla de que solo Program.cs tenga instrucciones de nivel superior.

2. Métodos sin uso
No se identificaron métodos sin uso claro.

Los métodos revisados tienen referencias:

MenuPrincipal: sus métodos privados son invocados desde el menú o desde otros métodos de la misma clase.
ReservaServicio y DisponibilidadServicio: son utilizados por MenuPrincipal.
InventarioInicial.Obtener: se utiliza en las pruebas.
InventarioInicial.SembrarAsync: se utiliza desde Program.cs y las pruebas.
Reserva.Cancelar: se invoca desde ReservaServicio.
ReservaRepositorio: implementa los métodos de IReservaRepositorio, utilizados por los servicios y pruebas.
Métodos de IReservaRepositorio: tienen implementaciones y consumidores.
Se detectaron dos elementos aparentemente no utilizados, aunque no son métodos:

Sistema_reservas/Aplicación/DTOs/CrearReservaDto.cs:3
CrearReservaDto no tiene referencias fuera de su propia declaración.
Sistema_reservas/Infraestructura/Dependencias.cs:7
Dependencias es una clase interna vacía y no tiene referencias.
3. Manejo de fechas
Hallazgo confirmado
Sistema_reservas/Dominio/Entidades/Reserva.cs:18

FechaCreacionUtc se inicializa con DateTime.Now.

La propiedad indica que el valor es UTC, pero DateTime.Now devuelve hora local. Debería utilizarse una fuente UTC para cumplir la regla del proyecto.

Riesgos de validación UTC
Sistema_reservas/Aplicación/Servicios/ReservaServicio.cs:23

Sistema_reservas/Aplicación/Servicios/DisponibilidadServicio.cs:15

Los parámetros se llaman inicioUtc y finUtc, pero los servicios no validan que DateTime.Kind sea Utc. Si otro consumidor les pasa una fecha local o Unspecified, las comparaciones y el almacenamiento podrían ser incorrectos.

Sistema_reservas/Aplicación/DTOs/CrearReservaDto.cs:9-11

Las fechas no tienen ninguna garantía o validación de que estén en UTC. Actualmente el DTO tampoco parece utilizarse.

Sistema_reservas/Infraestructura/Persistencia/ReservaDbContext.cs:13-23

EF Core no configura una conversión específica para normalizar o restaurar FechaInicio, FechaFin y FechaCreacionUtc como UTC. Esto es relevante porque SQLite no conserva de forma fiable el DateTime.Kind.

Manejo correcto detectado
Sistema_reservas/Presentacion/Menus/MenuPrincipal.cs:81-109
Las fechas introducidas por el usuario se interpretan como locales y se convierten mediante ToUniversalTime() antes de enviarlas a los servicios.
Sistema_reservas/Presentacion/Menus/MenuPrincipal.cs:137-138
Las fechas UTC se convierten a hora local para mostrarlas.
Sistema_reservas/Presentacion/Menus/MenuPrincipal.cs:161-167
La entrada se interpreta explícitamente como hora local mediante DateTimeStyles.AssumeLocal.
Las pruebas utilizan DateTime.UtcNow.
No se modificó ningún archivo.

👤
Lee .github/copilot-instructions.md y docs/progreso.md. Resúmeme las reglas que vas a seguir y revisa si la estructura de la solución respeta la arquitectura descrita. No modifiques ningún archivo todavía.

💬 GitHub Copilot
Voy a leer ambos archivos solicitados y comprobar la estructura actual de la solución y sus proyectos, sin modificar ningún archivo.

Leer copilot-instructions.md, líneas 1 a 200 ✅
Leer progreso.md, líneas 1 a 200 ✅
Leer 1 proyecto ✅
Leer 16 archivos para '.csproj', '.slnx', 'Program.cs', 'Aplicación', 'Dominio', 'Infraestructura', 'Presentacion', 'Tests' ✅
Reglas que seguiré
Respetar la arquitectura por capas:
Dominio: entidades y enumeraciones, sin dependencias.
Aplicación: servicios e interfaces, dependiente solo de Dominio.
Infraestructura: DbContext, repositorios y datos iniciales, dependiente de Aplicación.
Consola: menú y composición mediante inyección de dependencias.
Tests: pruebas xUnit.
Solo Program.cs debe contener instrucciones de nivel superior.
Almacenar y comparar todas las fechas en UTC; la consola debe convertirlas a hora local para mostrarlas o recibirlas.
Usar InvalidOperationException o ArgumentException para errores de negocio.
No agregar paquetes NuGet sin aprobación.
No modificar archivos fuera del alcance de la tarea.
Antes de finalizar cualquier tarea con cambios, ejecutar dotnet build y dotnet test, y registrar los resultados literales.
Leer y actualizar docs/progreso.md al inicio y al finalizar tareas con cambios.
Revisión de la estructura
La solución contiene un único proyecto:

Sistema_reservas/Sistema_reservas.csproj
Las capas están organizadas mediante carpetas:

Dominio
Aplicación
Infraestructura
Presentacion
Tests
Program.cs
Cumplimiento parcial
Dominio: las entidades y enumeraciones están ubicadas en Dominio.
Aplicación: contiene servicios e IReservaRepositorio.
Infraestructura: contiene ReservaDbContext, el repositorio y InventarioInicial.
Composición: Program.cs configura la inyección de dependencias.
Tests: existen pruebas xUnit en Tests.
Diferencias con la arquitectura descrita
No hay proyectos independientes por capa.
Todo está dentro de Sistema_reservas.csproj, por lo que las dependencias entre capas no están restringidas por el compilador.

La consola no tiene la estructura esperada.
La arquitectura describe SistemaReservas.Consola/Program.cs, pero actualmente existe:

Sistema_reservas/Program.cs
Sistema_reservas/Presentacion/Menus/MenuPrincipal.cs
No existe un proyecto independiente de pruebas.
Las pruebas están en Sistema_reservas/Tests, dentro del mismo proyecto ejecutable, en lugar de un proyecto SistemaReservas.Tests.

Los nombres no coinciden exactamente.
La arquitectura usa SistemaReservas, mientras que el código utiliza Sistema_reservas y namespaces con ese mismo formato.

Conclusión
La solución respeta la arquitectura de forma organizativa, mediante carpetas y namespaces, pero no la respeta completamente a nivel de proyectos y dependencias. No se modificó ningún archivo.

👤
Ejecuta dotnet build en la solución. Si hay errores, corrígelos sin cambiar la arquitectura de capas ni agregar paquetes. Al terminar, muéstrame el resultado literal de dotnet build y registra la tarea en docs/progreso.md.

💬 GitHub Copilot
Leeré docs/progreso.md y ejecutaré dotnet build sobre la solución. Si aparecen errores, corregiré únicamente los necesarios, sin añadir paquetes ni alterar las capas; después registraré el resultado.

Leer progreso.md, líneas 1 a 200 ✅
Se omitió la ejecución del comando de terminal ✅ La ejecución mediante terminal fue rechazada, así que usaré la verificación de compilación integrada del entorno para ejecutar el build de la solución.
Compilación correcta ✅ La compilación integrada terminó correctamente. No se detectaron errores ni fue necesario modificar código. Registraré la tarea indicando el resultado literal devuelto por la verificación: Compilación correcta.
Modifying progreso.md La compilación terminó correctamente y no se detectaron errores. No fue necesario modificar código ni paquetes.
Resultado literal de la verificación de compilación:

Compilación correcta

La ejecución directa mediante terminal fue rechazada por el entorno, por lo que se utilizó la compilación integrada de la solución. Se actualizó docs/progreso.md con el resultado y no se ejecutaron pruebas en esta tarea.

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
