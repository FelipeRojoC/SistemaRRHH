# Sistema de Manteniciones

Este modulo gestiona los vehiculos y sus manteniciones.

La integracion con Arriendos ya no es gRPC: se hace por RabbitMQ (colas `cola_arriendo` y `cola_mantencion`). Este servicio publica en `cola_mantencion` cuando se crea/edita un vehiculo o cambia su estado, y consume `cola_arriendo` para actualizar el estado del vehiculo cuando Arriendos crea o cierra un arriendo.

## Instrucciones para ejecutar

1. Asegurate de tener MySQL y RabbitMQ corriendo localmente (`rabbitmq-server`, puerto 5672).
2. Crea el archivo appsettings.local.json en esta carpeta con tu configuracion:
{
  "ConnectionStrings": {
    "DefaultConnection": "server=127.0.0.1;user=tu_usuario;pwd=tu_contrasenia;database=mantenciones"
  },
  "RabbitMQ": {
    "HostName": "localhost",
    "UserName": "guest",
    "Password": "guest"
  }
}
3. Abre una terminal en esta carpeta y ejecuta:
dotnet run
