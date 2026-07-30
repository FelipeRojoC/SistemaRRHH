using RabbitMQ.Client;

namespace SistemaArriendos.Messaging;

public class RabbitMqConnection : IDisposable
{
    public const string ColaMantencion = "cola_mantencion";
    public const string ColaArriendo = "cola_arriendo";

    private readonly IConnection _conexion;

    public RabbitMqConnection(IConfiguration configuracion)
    {
        var factory = new ConnectionFactory
        {
            HostName = configuracion["RabbitMQ:HostName"] ?? "localhost",
            UserName = configuracion["RabbitMQ:UserName"] ?? "guest",
            Password = configuracion["RabbitMQ:Password"] ?? "guest"
        };
        _conexion = factory.CreateConnection("SistemaArriendos");
    }

    public IModel CrearCanal()
    {
        var canal = _conexion.CreateModel();
        canal.QueueDeclare(ColaMantencion, durable: true, exclusive: false, autoDelete: false);
        canal.QueueDeclare(ColaArriendo, durable: true, exclusive: false, autoDelete: false);
        return canal;
    }

    public void Dispose() => _conexion.Dispose();
}
