# Sistema de Arriendos

Este subsistema se encarga de la operación comercial de la empresa: gestión de clientes y registro de arriendos de vehículos, con control automático del estado de la flota.

## Propósito del Sistema
*   **Gestión de Clientes (CRUD)**: Administrar la cartera de clientes (RUT, Nombre, Dirección).
*   **Registro de Arriendos**: Crear arriendos validando en tiempo real la disponibilidad del vehículo. Solo se pueden arrendar vehículos en estado `'Activo'` — los `'Arrendado'`, `'En Mantencion'` y `'De Baja'` quedan fuera del listado de selección y son rechazados por la validación de servidor.
*   **Cálculo automático**: El `PrecioDiario` y el `PrecioTotal` (días × valor diario) se calculan automáticamente desde el servidor usando el precio configurado en el vehículo.
*   **Control de estado**: Al crear un arriendo, el vehículo pasa a estado `'Arrendado'`. Al usar la acción **Devolver** desde el listado de arriendos, vuelve a `'Activo'`.
*   **Consulta de Flota**: Visor de solo lectura del estado actual de la flota (la administración de vehículos se realiza en el Sistema de Mantenciones).

## Integración con otros sistemas
*   **BD compartida con Mantenciones**: ambos sistemas usan la base `arriendos_mantenciones_db` (tablas `Vehiculo`, `Cliente`, `Mantencion`, `Arriendo`).
*   **No tiene contacto con RRHH**: la comunicación con Recursos Humanos pasa exclusivamente por el archivo `horas.json` que genera Mantenciones, no por este subsistema.

---

## Guía de Configuración Inicial (Para Nuevos Programadores)

Si acabas de clonar este repositorio de GitHub por primera vez, sigue estos pasos rápidos para correr el sistema en tu PC local:

### 1. Inicializar la Base de Datos Compartida en MySQL
La base de datos `arriendos_mantenciones_db` es la **misma** que usa el Sistema de Mantenciones. Si ya la creaste para Mantenciones, **no es necesario volver a ejecutar el script**.

Si es la primera vez, ejecuta el script SQL en tu servidor de MySQL local:
*   El script se encuentra en: **`../SistemaMantenciones/schema.sql`**

### 2. Configurar tus Credenciales Locales (Seguro contra Git)
Para evitar subir contraseñas a GitHub y respetar las claves personales de cada desarrollador de tu equipo, este proyecto utiliza un sistema de configuración descentralizado:
1.  En esta misma carpeta (`SistemaArriendos/`), crea un archivo llamado **`appsettings.local.json`**.
2.  Agrega el siguiente contenido, reemplazando `YOUR_USER` y `YOUR_PASSWORD` por las credenciales de tu MySQL local:
    ```json
    {
      "ConnectionStrings": {
        "DefaultConnection": "server=127.0.0.1;user=YOUR_USER;pwd=YOUR_PASSWORD;database=arriendos_mantenciones_db"
      }
    }
    ```
*   *(Nota: Este archivo ya se encuentra en el `.gitignore` local, por lo que nunca se subirá a GitHub ni alterará la clave de tus compañeros).*

### 3. Compilar y Correr la Aplicación
Abre una terminal en esta carpeta y ejecuta los siguientes comandos de .NET:
```bash
# Restaurar y compilar
dotnet build

# Correr el servidor local
dotnet run
```
*   La aplicación se levantará por defecto en: **`http://localhost:5266`**

### 4. Datos de prueba sugeridos
Para probar el sistema completo:
1.  Crea uno o más vehículos desde el **Sistema de Mantenciones** con estado `'Activo'` y un `PrecioArriendoDiario`.
2.  Crea un cliente en **Sistema de Arriendos → Clientes → Nuevo Cliente**.
3.  Registra un arriendo en **Nuevo Arriendo**. El vehículo pasará a `'Arrendado'` automáticamente.
4.  En el listado de arriendos, usa **Devolver** para volver el vehículo a `'Activo'`.
