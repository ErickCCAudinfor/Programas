using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SigeGestor.Core.Modelos;
using SigeGestor.Core.Operaciones;
using SigeGestor.Core.Registro;

namespace SigeGestor.App.Servicios;

/// <summary>
/// Deja en el log de ejecuciones los cambios que hacen las pantallas propias.
///
/// POR QUE HACE FALTA. Las operaciones normales se registran solas: pasan por
/// EjecucionViewModel, que al terminar llama a RegistroEjecuciones. Las pantallas que tienen
/// pagina propia —«Asignar nº de equipo en prefacturas», «Empresas y bases de datos» y
/// «Modelos de impresión»— no pasan por ahí, asi que lo que hacian NO SALIA EN EJECUCIONES.
/// Y son justo las que escriben en Produccion o cambian configuracion que usa todo el equipo:
/// si alguien pregunta quien toco algo, en el log no habia nada.
///
/// Es opcional a proposito: las pantallas lo reciben como nullable y, si no lo tienen —el
/// arnes las construye asi—, simplemente no registran. Que falte el log no puede impedir que
/// se haga el cambio.
/// </summary>
public sealed class AnotadorDeCambios
{
    private readonly RegistroEjecuciones _registro;
    private readonly Usuario _usuario;

    public AnotadorDeCambios(RegistroEjecuciones registro, Usuario usuario)
    {
        _registro = registro;
        _usuario = usuario;
    }

    /// <summary>
    /// Anota un cambio. No lanza nunca: RegistrarAsync ya se traga sus propios fallos, y aquí
    /// se cubre además lo que pueda pasar al montar la línea.
    ///
    /// ConfigureAwait(false) a propósito: esto no toca interfaz después de escribir, y así no
    /// se cuelga de la hebra de la ventana. Quien llame y necesite volver a ella que lo diga
    /// en SU await, que es donde se sabe.
    /// </summary>
    public async Task AnotarAsync(string operacion,
                                  SeccionOperacion seccion,
                                  string detalle,
                                  ClaveEntorno entorno,
                                  EstadoEjecucion estado,
                                  long registros = 0,
                                  int errores = 0,
                                  TimeSpan duracion = default,
                                  IEnumerable<string>? salidas = null)
    {
        try
        {
            await _registro.RegistrarAsync(new Ejecucion
            {
                Id = DateTime.Now.Ticks,
                Momento = DateTime.Now - duracion,
                Operacion = operacion,
                Grupo = seccion.ToString(),
                Detalle = detalle,
                NombreUsuario = _usuario.Nombre,
                Login = _usuario.Login,
                Entorno = entorno,
                Estado = estado,
                Registros = registros,
                Errores = errores,
                Duracion = duracion,
                Salidas = salidas?.ToArray() ?? Array.Empty<string>()
            }).ConfigureAwait(false);
        }
        catch
        {
            // Lo dicho: el log no puede tumbar lo que el usuario acaba de hacer.
        }
    }

    /// <summary>
    /// Qué estado le corresponde a un cambio por lo que haya salido. Se pone aquí para que
    /// las tres pantallas lo cuenten igual: «Completada» con 0 tocados se lee como si hubiera
    /// trabajado, y por eso existe SinCambios.
    /// </summary>
    public static EstadoEjecucion EstadoPor(long tocados, int errores) =>
        errores > 0 ? EstadoEjecucion.ConErrores
        : tocados > 0 ? EstadoEjecucion.Completada
        : EstadoEjecucion.SinCambios;
}
