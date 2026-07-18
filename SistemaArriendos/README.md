# Sistema de Arriendos

Este modulo gestiona los clientes y los arriendos de vehiculos.

## Instrucciones para ejecutar

1. Asegurate de tener MySQL corriendo localmente.
2. Crea el archivo appsettings.local.json en esta carpeta con tu configuracion:
{
  "ConnectionStrings": {
    "DefaultConnection": "server=127.0.0.1;user=tu_usuario;pwd=tu_contrasenia;database=arriendos"
  }
}
3. Abre una terminal en esta carpeta y ejecuta:
dotnet run
