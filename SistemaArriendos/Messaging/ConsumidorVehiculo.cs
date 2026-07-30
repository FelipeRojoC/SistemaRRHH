using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client.Events;
using SistemaArriendos.Models;

namespace SistemaArriendos.Messaging;

// Escucha cola_mantencion y mantiene actualizada la cache local de vehiculos.
public class ConsumidorVehiculo : BackgroundService
{
    private readonly RabbitMqConnection _conexion;
    private readonly IServiceScopeFactory _scopeFactory;

    public ConsumidorVehiculo(RabbitMqConnection conexion, IServiceScopeFactory scopeFactory)
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
            var mensaje = JsonSerializer.Deserialize<VehiculoMensaje>(Encoding.UTF8.GetString(evento.Body.ToArray()));
            if (mensaje != null)
            {
                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ArriendosMantencionesDbContext>();

                var vehiculo = await db.vehiculosCache.FindAsync(mensaje.Codigo);
                if (vehiculo == null)
                {
                    db.vehiculosCache.Add(new VehiculoCache { codigo = mensaje.Codigo });
                    vehiculo = await db.vehiculosCache.FindAsync(mensaje.Codigo);
                }

                vehiculo!.patente = mensaje.Patente;
                vehiculo.marca = mensaje.Marca;
                vehiculo.modelo = mensaje.Modelo;
                vehiculo.tipo = mensaje.Tipo;
                vehiculo.kilometraje = mensaje.Kilometraje;
                vehiculo.estado = mensaje.Estado;
                vehiculo.precioArriendoDiario = mensaje.PrecioArriendoDiario;

                await db.SaveChangesAsync();
            }

            canal.BasicAck(evento.DeliveryTag, multiple: false);
        };

        canal.BasicConsume(
            queue: RabbitMqConnection.ColaMantencion,
            autoAck: false,
            consumerTag: string.Empty,
            noLocal: false,
            exclusive: false,
            arguments: null,
            consumer: consumidor);

        stoppingToken.Register(() => canal.Dispose());
        return Task.CompletedTask;
    }
}
