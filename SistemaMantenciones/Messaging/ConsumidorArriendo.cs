using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SistemaMantenciones.Models;

namespace SistemaMantenciones.Messaging;

// Escucha cola_arriendo y actualiza el estado del vehiculo segun lo que hizo Arriendos.
public class ConsumidorArriendo : BackgroundService
{
    private readonly RabbitMqConnection _conexion;
    private readonly IServiceScopeFactory _scopeFactory;

    public ConsumidorArriendo(RabbitMqConnection conexion, IServiceScopeFactory scopeFactory)
    {
        _conexion = conexion;
        _scopeFactory = scopeFactory;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var canal = _conexion.CrearCanal();
        var consumidor = new EventingBasicConsumer(canal);

        consumidor.Received += async (modelo, evento) =>
        {
            var mensaje = JsonSerializer.Deserialize<ArriendoMensaje>(Encoding.UTF8.GetString(evento.Body.ToArray()));
            if (mensaje != null)
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ArriendosMantencionesDbContext>();

                var vehiculo = await db.vehiculos.FindAsync(mensaje.CodigoVehiculo);
                if (vehiculo != null)
                {
                    vehiculo.estado = mensaje.Tipo == "ArriendoCreado" ? "Arrendado" : "Activo";
                    await db.SaveChangesAsync();

                    // Devuelve el estado actualizado a Arriendos via cola_mantencion para refrescar su cache.
                    var publicador = scope.ServiceProvider.GetRequiredService<PublicadorVehiculo>();
                    publicador.Publicar(new VehiculoMensaje(
                        vehiculo.codigo, vehiculo.patente, vehiculo.marca, vehiculo.modelo,
                        vehiculo.tipo, vehiculo.kilometraje, vehiculo.estado, vehiculo.precioArriendoDiario));
                }
            }

            canal.BasicAck(evento.DeliveryTag, multiple: false);
        };

        canal.BasicConsume(queue: RabbitMqConnection.ColaArriendo, autoAck: false, consumer: consumidor);

        stoppingToken.Register(() => canal.Dispose());
        return Task.CompletedTask;
    }
}
