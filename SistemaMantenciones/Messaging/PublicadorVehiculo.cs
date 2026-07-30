using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace SistemaMantenciones.Messaging;

public class PublicadorVehiculo
{
    private readonly RabbitMqConnection _conexion;

    public PublicadorVehiculo(RabbitMqConnection conexion)
    {
        _conexion = conexion;
    }

    public void Publicar(VehiculoMensaje mensaje)
    {
        using var canal = _conexion.CrearCanal();
        var cuerpo = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(mensaje));
        var propiedades = canal.CreateBasicProperties();
        propiedades.Persistent = true;
        canal.BasicPublish(exchange: "", routingKey: RabbitMqConnection.ColaMantencion, basicProperties: propiedades, body: cuerpo);
    }
}
