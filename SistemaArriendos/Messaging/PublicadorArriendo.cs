using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace SistemaArriendos.Messaging;

public class PublicadorArriendo
{
    private readonly RabbitMqConnection _conexion;

    public PublicadorArriendo(RabbitMqConnection conexion)
    {
        _conexion = conexion;
    }

    public void Publicar(ArriendoMensaje mensaje)
    {
        using var canal = _conexion.CrearCanal();
        var cuerpo = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(mensaje));
        var propiedades = canal.CreateBasicProperties();
        propiedades.Persistent = true;
        canal.BasicPublish(exchange: "", routingKey: RabbitMqConnection.ColaArriendo, basicProperties: propiedades, body: cuerpo);
    }
}
