# Sistema de Arriendos

Este modulo gestiona los clientes y los arriendos de vehiculos.

La integracion con Mantenciones ya no es gRPC: se hace por RabbitMQ (colas `cola_arriendo` y `cola_mantencion`). Este servicio publica en `cola_arriendo` al crear/cerrar un arriendo, y consume `cola_mantencion` para mantener una cache local de vehiculos (tabla `vehiculo_cache`).

## Instrucciones para ejecutar

1. Asegurate de tener MySQL y RabbitMQ corriendo localmente (`rabbitmq-server`, puerto 5672).
2. Crea el archivo appsettings.local.json en esta carpeta con tu configuracion:
{
  "ConnectionStrings": {
    "DefaultConnection": "server=127.0.0.1;user=tu_usuario;pwd=tu_contrasenia;database=arriendos"
  },
  "RabbitMQ": {
    "HostName": "localhost",
    "UserName": "guest",
    "Password": "guest"
  }
}
3. Abre una terminal en esta carpeta y ejecuta:
dotnet run
