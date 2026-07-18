# Sistema de Manteniciones

Este modulo gestiona los vehiculos y sus manteniciones.

## Instrucciones para ejecutar

1. Asegurate de tener MySQL corriendo localmente.
2. Crea el archivo appsettings.local.json en esta carpeta con tu configuracion:
{
  "ConnectionStrings": {
    "DefaultConnection": "server=127.0.0.1;user=tu_usuario;pwd=tu_contrasenia;database=mantenciones"
  }
}
3. Abre una terminal en esta carpeta y ejecuta:
dotnet run
