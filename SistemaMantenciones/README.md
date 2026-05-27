# Sistema de Registro de Mantenciones

Este subsistema se encarga de la gestión de la flota de vehículos y del registro de mantenciones técnicas preventivas y correctivas del taller mecánico.

## Propósito del Sistema
*   **Gestión de Vehículos (CRUD)**: Administrar las propiedades de los autos (Código, Patente, Marca, Modelo, Tipo, Kilometraje, Estado, Precio Diario).
*   **Registro de Mantenciones**: Formulario interactivo para registrar los trabajos de mantención especificando el vehículo y las horas que duró. Al ingresar un servicio, el estado del vehículo transiciona de forma automática a `'En Mantencion'` (para alertar al subsistema de Arriendos).
*   **Exportar Horas de Trabajo**: Consolidar y agrupar en MySQL las horas totales trabajadas por cada mecánico (RUT) y exportar en tiempo real el archivo `horas.json` directo en la carpeta de Recursos Humanos.

---

## Guía de Configuración Inicial (Para Nuevos Programadores)

Si acabas de clonar este repositorio de GitHub por primera vez, sigue estos pasos rápidos para correr el sistema en tu PC local:

### 1. Inicializar la Base de Datos Compartida en MySQL
Este sistema comparte base de datos física con el futuro sistema de Arriendos. Ejecuta el script SQL en tu servidor de MySQL local para crear la base de datos `arriendos_mantenciones_db` y sus tablas correspondientes:
*   El script se encuentra en la carpeta del proyecto: **`schema.sql`**

### 2. Configurar tus Credenciales Locales (Seguro contra Git)
Para evitar subir contraseñas a GitHub y respetar las claves personales de cada desarrollador de tu equipo, este proyecto utiliza un sistema de configuración descentralizado:
1.  En esta misma carpeta (`SistemaMantenciones/`), crea un archivo llamado **`appsettings.local.json`**.
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
*   La aplicación se levantará por defecto en: **`http://localhost:5206`**
