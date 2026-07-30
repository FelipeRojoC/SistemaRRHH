namespace SistemaArriendos.Messaging;

// Recibido desde cola_mantencion cuando un vehiculo se crea o cambia de estado.
public record VehiculoMensaje(
    string Codigo,
    string Patente,
    string Marca,
    string Modelo,
    string Tipo,
    int Kilometraje,
    string Estado,
    int PrecioArriendoDiario);

// Publicado en cola_arriendo cuando se crea o se cierra un arriendo.
public record ArriendoMensaje(string Tipo, string CodigoVehiculo);
