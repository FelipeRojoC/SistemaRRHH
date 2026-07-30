namespace SistemaMantenciones.Messaging;

// Publicado en cola_mantencion cuando un vehiculo se crea o cambia de estado.
public record VehiculoMensaje(
    string Codigo,
    string Patente,
    string Marca,
    string Modelo,
    string Tipo,
    int Kilometraje,
    string Estado,
    int PrecioArriendoDiario);

// Recibido desde cola_arriendo cuando Arriendos crea o cierra un arriendo.
public record ArriendoMensaje(string Tipo, string CodigoVehiculo);
