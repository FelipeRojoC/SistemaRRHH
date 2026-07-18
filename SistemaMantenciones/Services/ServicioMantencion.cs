using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using SistemaMantenciones.Models;
using SistemaMantenciones.Protos;

namespace SistemaMantenciones.Services;

public class ServicioMantencionImpl : ServicioMantencion.ServicioMantencionBase
{
    private readonly ArriendosMantencionesDbContext _contextoDb;

    public ServicioMantencionImpl(ArriendosMantencionesDbContext contextoDb)
    {
        _contextoDb = contextoDb;
    }

    public override async Task<VehiculoRespuesta> obtieneVehiculo(ObtieneVehiculoPeticion peticion, ServerCallContext contexto)
    {
        var v = await _contextoDb.vehiculos.FirstOrDefaultAsync(x => x.codigo == peticion.Id);
        if (v == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, $"Vehiculo con codigo {peticion.Id} no encontrado."));
        }

        return new VehiculoRespuesta
        {
            Codigo = v.codigo,
            Patente = v.patente,
            Marca = v.marca,
            Modelo = v.modelo,
            Tipo = v.tipo,
            Kilometraje = v.kilometraje,
            Estado = v.estado,
            PrecioArriendoDiario = v.precioArriendoDiario
        };
    }

    public override async Task<ListaVehiculosRespuesta> obtieneVehiculos(ObtieneVehiculosPeticion peticion, ServerCallContext contexto)
    {
        var lista = await _contextoDb.vehiculos.ToListAsync();
        var respuesta = new ListaVehiculosRespuesta();
        foreach (var v in lista)
        {
            respuesta.Vehiculos.Add(new VehiculoRespuesta
            {
                Codigo = v.codigo,
                Patente = v.patente,
                Marca = v.marca,
                Modelo = v.modelo,
                Tipo = v.tipo,
                Kilometraje = v.kilometraje,
                Estado = v.estado,
                PrecioArriendoDiario = v.precioArriendoDiario
            });
        }
        return respuesta;
    }

    public override async Task<CambiaEstadoVehiculoRespuesta> cambiaEstadoVehiculo(CambiaEstadoVehiculoPeticion peticion, ServerCallContext contexto)
    {
        var v = await _contextoDb.vehiculos.FirstOrDefaultAsync(x => x.codigo == peticion.Id);
        if (v == null)
        {
            return new CambiaEstadoVehiculoRespuesta
            {
                Exito = false,
                Mensaje = $"Vehiculo con codigo {peticion.Id} no encontrado."
            };
        }

        v.estado = peticion.Estado;
        _contextoDb.Entry(v).State = EntityState.Modified;
        await _contextoDb.SaveChangesAsync();

        return new CambiaEstadoVehiculoRespuesta
        {
            Exito = true,
            Mensaje = "Estado del vehiculo actualizado correctamente."
        };
    }
}
