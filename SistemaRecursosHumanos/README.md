# Sistema de Recursos Humanos

Este subsistema se encarga de la administración de personal y del cálculo consolidado de liquidaciones de sueldo de la empresa.

## Propósito del Sistema
*   **Gestión de Mecánicos (CRUD)**: Registrar, modificar y dar de baja al personal técnico, asociándoles un Sueldo Base y un Valor por Hora de Trabajo.
*   **Cálculo de Remuneraciones**: Procesar de forma automática y en tiempo real el sueldo neto del mes sumando el `Sueldo Base` más el componente variable (`Horas Trabajadas` * `Valor de su Hora`).
*   **Integración**: Lee en tiempo real el archivo consolidado `horas.json` exportado por el subsistema de Mantenciones.

---

## Guía de Configuración Inicial (Para Nuevos Programadores)

Si acabas de clonar este repositorio de GitHub por primera vez, sigue estos pasos rápidos para correr el sistema en tu PC local:

### 1. Inicializar la Base de Datos en MySQL
Ejecuta el script SQL de inicialización en tu servidor de MySQL local (ej: a través de MySQL Workbench o línea de comandos) para crear la base de datos `rrhh_db` y la tabla necesaria:
*   El script se encuentra en la carpeta del proyecto: **`rrhh_schema.sql`**

### 2. Configurar tus Credenciales Locales (Seguro contra Git)
Para evitar subir contraseñas a GitHub y respetar las claves personales de cada desarrollador de tu equipo, este proyecto utiliza un sistema de configuración descentralizado:
1.  En esta misma carpeta (`SistemaRecursosHumanos/`), crea un archivo llamado **`appsettings.local.json`**.
2.  Agrega el siguiente contenido, reemplazando `YOUR_USER` y `YOUR_PASSWORD` por las credenciales de tu MySQL local:
    ```json
    {
      "ConnectionStrings": {
        "DefaultConnection": "server=127.0.0.1;user=YOUR_USER;pwd=YOUR_PASSWORD;database=rrhh_db"
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
*   La aplicación se levantará por defecto en: **`http://localhost:5146`**
