Namespace Operaciones

    ''' <summary>
    ''' Todo lo que hace la aplicación, en una lista.
    '''
    ''' PARA AÑADIR UNA OPERACIÓN: se añade aquí una DefinicionOperacion y ya aparece en su
    ''' sección con su formulario y su validación. No hay que tocar ninguna vista.
    '''
    ''' Las consultas están portadas una a una desde el CatalogoConsultas.vb de
    ''' ActualizaPrecios, con sus mismas entradas requeridas. El resto de secciones sale de
    ''' los botones que hoy viven en Form1.
    ''' </summary>
    Public Module CatalogoOperaciones

        Public Const SubgrupoGenerales As String = "Generales"
        Public Const SubgrupoPool As String = "Pool (CUPS)"

        Private _todas As IReadOnlyList(Of DefinicionOperacion)

        ''' <summary>
        ''' El catálogo completo. Perezoso por la misma razón que Sql: construirlo en un
        ''' inicializador de campo lo ataba al orden de declaración del fichero.
        ''' </summary>
        Public ReadOnly Property Todas As IReadOnlyList(Of DefinicionOperacion)
            Get
                If _todas Is Nothing Then _todas = Construir()
                Return _todas
            End Get
        End Property

        Private Function Construir() As IReadOnlyList(Of DefinicionOperacion)
            Dim lista As New List(Of DefinicionOperacion)

            ' ============================================================
            ' PRECIOS — escritura
            ' ============================================================
            lista.Add(New DefinicionOperacion With {
                .Nombre = "Actualizar precios de tarifa",
                .Seccion = SeccionOperacion.Precios,
                .Descripcion = "Sustituye los precios de los contratos indicados por los del grupo de tarifa destino. La fecha la aporta cada contrato: su fecha de aplicación de precios, si no la del contrato, y si no hoy.",
                .Requiere = EntradasOperacion.Contratos Or EntradasOperacion.Cups Or EntradasOperacion.Cifs Or
                            EntradasOperacion.GrupoTarifa Or EntradasOperacion.FiltroTarifaActual,
                .EsEscritura = True,
                .EtiquetaAccion = "Actualizar precios",
                .Ejecutable = New Implementadas.ActualizarPreciosTarifa()
            })
            lista.Add(New DefinicionOperacion With {
                .Nombre = "Revisar tarifas personalizadas",
                .Seccion = SeccionOperacion.Precios,
                .Descripcion = "Entrega dos Excel, uno de luz y otro de gas, con los contratos que tienen precios personalizados. No cambia nada y no necesita ningún dato.",
                .Requiere = EntradasOperacion.Ninguna,
                .EsEscritura = False,
                .EtiquetaAccion = "Revisar",
                .Ejecutable = New Implementadas.RevisarTarifasPersonalizadas()
            })
            ' Pedía fechas y no las usa: la fecha de cada contrato viene en la columna 1 del
            ' Excel, y la línea del original que leía el DateTimePicker está comentada.
            lista.Add(New DefinicionOperacion With {
                .Nombre = "Aplicar precios desde Excel",
                .Seccion = SeccionOperacion.Precios,
                .Descripcion = "Aplica los precios de un Excel con cuatro columnas en la hoja «Hoja1»: " &
                               "FechaContrato, IdContratoTarifa, IdTarifaGrupo y CodigoContrato.",
                .Requiere = EntradasOperacion.Excel,
                .EsEscritura = True,
                .EtiquetaAccion = "Aplicar precios",
                .Ejecutable = New Implementadas.AplicarPreciosDesdeExcel()
            })
            lista.Add(New DefinicionOperacion With {
                .Nombre = "Cambiar grupo de tarifa",
                .Seccion = SeccionOperacion.Precios,
                .Descripcion = "Reasigna el grupo de tarifa sin recalcular precios: los contratos se quedan con el grupo nuevo y los precios del viejo. Si quieres cambiar grupo Y precios, usa «Actualizar precios de tarifa».",
                .Requiere = EntradasOperacion.Contratos Or EntradasOperacion.Cups Or EntradasOperacion.Cifs Or
                            EntradasOperacion.GrupoTarifa Or EntradasOperacion.FiltroTarifaActual,
                .EsEscritura = True,
                .EtiquetaAccion = "Cambiar grupo",
                .Ejecutable = New Implementadas.CambiarGrupoTarifa()
            })

            ' ============================================================
            ' CONTRATOS — escritura
            ' ============================================================
            lista.Add(New DefinicionOperacion With {
                .Nombre = "Volver a renovar",
                .Seccion = SeccionOperacion.Contratos,
                .Descripcion = "Deja pendientes de renovación los contratos indicados. Solo toca los que están activos; el resto se informa y se deja igual.",
                .Requiere = EntradasOperacion.Contratos Or EntradasOperacion.Cups Or EntradasOperacion.Cifs,
                .EsEscritura = True,
                .EtiquetaAccion = "Volver a renovar",
                .Ejecutable = New Implementadas.VolverARenovar()
            })
            lista.Add(New DefinicionOperacion With {
                .Nombre = "Masivo contrato",
                .Seccion = SeccionOperacion.Contratos,
                .Descripcion = "Cambia en bloque los campos del contrato. Se aplica solo lo que rellenes: " &
                               "lo que dejes en «no tocar» o en blanco se queda como está. Antes y después " &
                               "se genera un Excel con el estado de los contratos para poder comparar.",
                .Requiere = EntradasOperacion.Contratos Or EntradasOperacion.Cups Or EntradasOperacion.Cifs Or
                            EntradasOperacion.Carpeta,
                .EsEscritura = True,
                .EtiquetaAccion = "Aplicar cambios",
                .Ejecutable = New Implementadas.MasivoContrato(),
                .Campos = New List(Of CampoOperacion) From {
                    New CampoOperacion With {
                        .Clave = Implementadas.MasivoContrato.ClaveSituacion,
                        .Etiqueta = "Situación del contrato",
                        .Tipo = TipoCampo.Seleccion,
                        .Origen = OrigenLista.SituacionesContrato,
                        .EtiquetaVacio = "— no tocar —",
                        .Requerido = False
                    },
                    New CampoOperacion With {
                        .Clave = Implementadas.MasivoContrato.ClaveFechaAlta,
                        .Etiqueta = "Fecha de alta",
                        .Tipo = TipoCampo.Fecha,
                        .Requerido = False
                    },
                    New CampoOperacion With {
                        .Clave = Implementadas.MasivoContrato.ClaveFechaVto,
                        .Etiqueta = "Fecha de vencimiento",
                        .Ayuda = "Se ignora si marcas «sumar un año».",
                        .Tipo = TipoCampo.Fecha,
                        .Requerido = False
                    },
                    New CampoOperacion With {
                        .Clave = Implementadas.MasivoContrato.ClaveSumarAnoVto,
                        .Etiqueta = "Sumar un año al vencimiento actual",
                        .Ayuda = "Suma un año al vencimiento que ya tenga cada contrato, sin fijar una fecha concreta.",
                        .Tipo = TipoCampo.Seleccion,
                        .OpcionesFijas = SiNoSinTocar.Opciones(),
                        .Requerido = False
                    },
                    New CampoOperacion With {
                        .Clave = Implementadas.MasivoContrato.ClaveObservaciones,
                        .Etiqueta = "Observaciones",
                        .Tipo = TipoCampo.Texto,
                        .Requerido = False
                    },
                    New CampoOperacion With {
                        .Clave = Implementadas.MasivoContrato.ClaveModoObservaciones,
                        .Etiqueta = "Qué hacer con las observaciones",
                        .Tipo = TipoCampo.Seleccion,
                        .OpcionesFijas = Implementadas.MasivoContrato.OpcionesModoTexto(),
                        .Requerido = False
                    },
                    New CampoOperacion With {
                        .Clave = Implementadas.MasivoContrato.ClaveTextoRevision,
                        .Etiqueta = "Texto de revisión de facturas",
                        .Tipo = TipoCampo.Texto,
                        .Requerido = False
                    },
                    New CampoOperacion With {
                        .Clave = Implementadas.MasivoContrato.ClaveModoTextoRevision,
                        .Etiqueta = "Qué hacer con el texto de revisión",
                        .Tipo = TipoCampo.Seleccion,
                        .OpcionesFijas = Implementadas.MasivoContrato.OpcionesModoTexto(),
                        .Requerido = False
                    },
                    New CampoOperacion With {
                        .Clave = Implementadas.MasivoContrato.ClaveTipoImpuesto,
                        .Etiqueta = "Tipo de impuesto",
                        .Tipo = TipoCampo.Seleccion,
                        .Origen = OrigenLista.TiposImpuesto,
                        .EtiquetaVacio = "— no tocar —",
                        .Requerido = False
                    },
                    New CampoOperacion With {
                        .Clave = Implementadas.MasivoContrato.ClaveAgruparFacturas,
                        .Etiqueta = "Unificar facturas",
                        .Tipo = TipoCampo.Seleccion,
                        .OpcionesFijas = SiNoSinTocar.Opciones(),
                        .Requerido = False
                    },
                    New CampoOperacion With {
                        .Clave = Implementadas.MasivoContrato.ClaveRevisionFra,
                        .Etiqueta = "Revisión de factura",
                        .Tipo = TipoCampo.Seleccion,
                        .OpcionesFijas = SiNoSinTocar.Opciones(),
                        .Requerido = False
                    },
                    New CampoOperacion With {
                        .Clave = Implementadas.MasivoContrato.ClaveModeloFactura,
                        .Etiqueta = "Modelo de factura de energía",
                        .Tipo = TipoCampo.Seleccion,
                        .Origen = OrigenLista.ModelosFactura,
                        .EtiquetaVacio = "— no tocar —",
                        .Requerido = False
                    },
                    New CampoOperacion With {
                        .Clave = Implementadas.MasivoContrato.ClaveModeloFacturaVarios,
                        .Etiqueta = "Modelo de factura de varios",
                        .Tipo = TipoCampo.Seleccion,
                        .Origen = OrigenLista.ModelosFacturaVarios,
                        .EtiquetaVacio = "— no tocar —",
                        .Requerido = False
                    },
                    New CampoOperacion With {
                        .Clave = Implementadas.MasivoContrato.ClaveModeloContrato,
                        .Etiqueta = "Modelo de contrato",
                        .Tipo = TipoCampo.Seleccion,
                        .Origen = OrigenLista.ModelosContrato,
                        .EtiquetaVacio = "— no tocar —",
                        .Requerido = False
                    },
                    New CampoOperacion With {
                        .Clave = Implementadas.MasivoContrato.ClaveCnae,
                        .Etiqueta = "CNAE",
                        .Tipo = TipoCampo.Seleccion,
                        .Origen = OrigenLista.Cnaes,
                        .EtiquetaVacio = "— no tocar —",
                        .Requerido = False
                    },
                    New CampoOperacion With {
                        .Clave = Implementadas.MasivoContrato.ClaveTipoImprimir,
                        .Etiqueta = "Tipo de impresión",
                        .Tipo = TipoCampo.Seleccion,
                        .OpcionesFijas = Implementadas.MasivoContrato.OpcionesTipoImprimir(),
                        .Requerido = False
                    },
                    New CampoOperacion With {
                        .Clave = Implementadas.MasivoContrato.ClaveRenovacionProcesada,
                        .Etiqueta = "Renovación procesada",
                        .Tipo = TipoCampo.Seleccion,
                        .OpcionesFijas = SiNoSinTocar.Opciones(),
                        .Requerido = False
                    },
                    New CampoOperacion With {
                        .Clave = Implementadas.MasivoContrato.ClaveNoRenovar,
                        .Etiqueta = "No renovar",
                        .Tipo = TipoCampo.Seleccion,
                        .OpcionesFijas = SiNoSinTocar.Opciones(),
                        .Requerido = False
                    },
                    New CampoOperacion With {
                        .Clave = Implementadas.MasivoContrato.ClaveRepresentante,
                        .Etiqueta = "Representante",
                        .Tipo = TipoCampo.Texto,
                        .Requerido = False
                    },
                    New CampoOperacion With {
                        .Clave = Implementadas.MasivoContrato.ClaveColectivoRep,
                        .Etiqueta = "Colectivo del representante",
                        .Tipo = TipoCampo.Seleccion,
                        .Origen = OrigenLista.Colectivos,
                        .EtiquetaVacio = "— no tocar —",
                        .Requerido = False
                    },
                    New CampoOperacion With {
                        .Clave = Implementadas.MasivoContrato.ClaveIdentificadorRep,
                        .Etiqueta = "Identificador del representante",
                        .Tipo = TipoCampo.Texto,
                        .Requerido = False
                    },
                    New CampoOperacion With {
                        .Clave = Implementadas.MasivoContrato.ClaveEmailRep,
                        .Etiqueta = "Email del representante",
                        .Tipo = TipoCampo.Texto,
                        .Requerido = False
                    },
                    New CampoOperacion With {
                        .Clave = Implementadas.MasivoContrato.ClaveMovilRep,
                        .Etiqueta = "Móvil del representante",
                        .Tipo = TipoCampo.Texto,
                        .Requerido = False
                    },
                    New CampoOperacion With {
                        .Clave = Implementadas.MasivoContrato.ClaveScoring,
                        .Etiqueta = "Situación de scoring",
                        .Tipo = TipoCampo.Seleccion,
                        .Origen = OrigenLista.SituacionesScoring,
                        .EtiquetaVacio = "— no tocar —",
                        .Requerido = False
                    },
                    New CampoOperacion With {
                        .Clave = Implementadas.MasivoContrato.ClaveCifClientePago,
                        .Etiqueta = "CIF para buscar el cliente de pago",
                        .Ayuda = "Escribe el CIF y el desplegable de abajo se llena con sus clientes de pago.",
                        .Tipo = TipoCampo.Texto,
                        .LongitudMaxima = 20,
                        .Requerido = False
                    },
                    New CampoOperacion With {
                        .Clave = Implementadas.MasivoContrato.ClaveClientePago,
                        .Etiqueta = "Cliente de pago",
                        .Ayuda = "Sale vacío hasta que escribas el CIF de arriba.",
                        .Tipo = TipoCampo.Seleccion,
                        .Origen = OrigenLista.ClientesPago,
                        .DependeDe = Implementadas.MasivoContrato.ClaveCifClientePago,
                        .EtiquetaVacio = "— no tocar —",
                        .Requerido = False
                    },
                    New CampoOperacion With {
                        .Clave = Implementadas.MasivoContrato.ClaveAutoconsumo,
                        .Etiqueta = "Autoconsumo",
                        .Tipo = TipoCampo.Seleccion,
                        .OpcionesFijas = SiNoSinTocar.Opciones(),
                        .Requerido = False
                    },
                    New CampoOperacion With {
                        .Clave = Implementadas.MasivoContrato.ClaveAutoconsumoNoCompensable,
                        .Etiqueta = "Autoconsumo no compensable",
                        .Tipo = TipoCampo.Seleccion,
                        .OpcionesFijas = SiNoSinTocar.Opciones(),
                        .Requerido = False
                    },
                    New CampoOperacion With {
                        .Clave = Implementadas.MasivoContrato.ClaveTipoAutoconsumo,
                        .Etiqueta = "Tipo de autoconsumo",
                        .Tipo = TipoCampo.Seleccion,
                        .Origen = OrigenLista.TiposAutoconsumo,
                        .EtiquetaVacio = "— no tocar —",
                        .Requerido = False
                    },
                    New CampoOperacion With {
                        .Clave = Implementadas.MasivoContrato.ClaveLicitacion,
                        .Etiqueta = "Es licitación",
                        .Tipo = TipoCampo.Seleccion,
                        .OpcionesFijas = SiNoSinTocar.Opciones(),
                        .Requerido = False
                    },
                    New CampoOperacion With {
                        .Clave = Implementadas.MasivoContrato.ClaveExencionIe,
                        .Etiqueta = "Exención del impuesto eléctrico",
                        .Tipo = TipoCampo.Seleccion,
                        .OpcionesFijas = SiNoSinTocar.Opciones(),
                        .Requerido = False
                    }
                }
            })
            lista.Add(New DefinicionOperacion With {
                .Nombre = "Añadir códigos DIR",
                .Seccion = SeccionOperacion.Contratos,
                .Descripcion = "Fija unidad tramitadora, oficina contable y órgano gestor en los contratos indicados. Los tres se escriben juntos.",
                .Requiere = EntradasOperacion.Contratos Or EntradasOperacion.Cups Or EntradasOperacion.Cifs,
                .EsEscritura = True,
                .EtiquetaAccion = "Fijar códigos",
                .Campos = {
                    New CampoOperacion With {
                        .Clave = Implementadas.ActualizarCodigosDir.CampoUnidad,
                        .Etiqueta = "Código de unidad tramitadora", .LongitudMaxima = 50},
                    New CampoOperacion With {
                        .Clave = Implementadas.ActualizarCodigosDir.CampoOficina,
                        .Etiqueta = "Código de oficina contable", .LongitudMaxima = 50},
                    New CampoOperacion With {
                        .Clave = Implementadas.ActualizarCodigosDir.CampoOrgano,
                        .Etiqueta = "Código de órgano gestor", .LongitudMaxima = 50}
                },
                .Ejecutable = New Implementadas.ActualizarCodigosDir()
            })
            lista.Add(New DefinicionOperacion With {
                .Nombre = "Cambiar agente",
                .Seccion = SeccionOperacion.Contratos,
                .Descripcion = "Reasigna el agente comercial de una lista de contratos. Solo se tocan los activos.",
                .Requiere = EntradasOperacion.Contratos Or EntradasOperacion.Cups Or EntradasOperacion.Cifs,
                .EsEscritura = True,
                .EtiquetaAccion = "Cambiar agente",
                .Campos = {
                    New CampoOperacion With {
                        .Clave = Implementadas.ReasignarContrato.CampoDestino,
                        .Etiqueta = "Agente destino",
                        .Tipo = TipoCampo.Seleccion,
                        .Origen = OrigenLista.Agentes}
                },
                .Ejecutable = New Implementadas.ReasignarContrato(
                    Contratos.RepositorioContratos.ColumnaAgente, "agente",
                    origenLista:=OrigenLista.Agentes)
            })
            lista.Add(New DefinicionOperacion With {
                .Nombre = "Cambiar administrador",
                .Seccion = SeccionOperacion.Contratos,
                .Descripcion = "Reasigna el administrador de una lista de contratos, o lo deja sin administrador. Solo se tocan los activos.",
                .Requiere = EntradasOperacion.Contratos Or EntradasOperacion.Cups Or EntradasOperacion.Cifs,
                .EsEscritura = True,
                .EtiquetaAccion = "Cambiar administrador",
                .Campos = {
                    New CampoOperacion With {
                        .Clave = Implementadas.ReasignarContrato.CampoDestino,
                        .Etiqueta = "Administrador destino",
                        .Tipo = TipoCampo.Seleccion,
                        .Origen = OrigenLista.Administradores,
                        .EtiquetaVacio = "Sin administrador",
                        .Requerido = False}
                },
                .Ejecutable = New Implementadas.ReasignarContrato(
                    Contratos.RepositorioContratos.ColumnaAdministrador, "administrador",
                    admiteVacio:=True, origenLista:=OrigenLista.Administradores)
            })
            ' Al catalogar esta operación se dio por hecho que escribía —«revisa y aplica»— y
            ' que necesitaba fechas. Al portarla se ha comprobado que ni una cosa ni la otra:
            ' PenalizacionesLuz y PenalizacionesGas solo hacen SELECT y vuelcan a Excel, sin un
            ' solo UPDATE, y el original tampoco pedía fechas. Corregido: es de lectura y no
            ' pide fechas, así puede ejecutarse contra Réplica, que es donde debe ir.
            lista.Add(New DefinicionOperacion With {
                .Nombre = "Penalizaciones",
                .Seccion = SeccionOperacion.Contratos,
                .Descripcion = "Saca a Excel los precios y consumos de la última factura de cada contrato, " &
                               "separando luz y gas. Solo consulta: no cambia nada.",
                .Requiere = EntradasOperacion.Contratos Or EntradasOperacion.Cups Or EntradasOperacion.Cifs Or EntradasOperacion.Carpeta,
                .EsEscritura = False,
                .Ejecutable = New Implementadas.Penalizaciones()
            })
            ' Primera operación portada de verdad. Solo lee, así que es la más segura para
            ' estrenar el ejecutor.
            lista.Add(New DefinicionOperacion With {
                .Nombre = "Comprobar que los contratos existen",
                .Seccion = SeccionOperacion.Contratos,
                .Descripcion = "Recorre la lista y dice, uno a uno, cuáles existen y cuáles no. No cambia nada.",
                .Requiere = EntradasOperacion.Contratos Or EntradasOperacion.Cups Or EntradasOperacion.Cifs,
                .EsEscritura = False,
                .EtiquetaAccion = "Comprobar",
                .Ejecutable = New Implementadas.ComprobarContratos()
            })
            lista.Add(New DefinicionOperacion With {
                .Nombre = "Verificar licitación",
                .Seccion = SeccionOperacion.Contratos,
                .Descripcion = "Dice, uno a uno, si el contrato tiene marcado el campo IsLicitacion. No cambia nada.",
                .Requiere = EntradasOperacion.Contratos Or EntradasOperacion.Cups Or EntradasOperacion.Cifs,
                .EsEscritura = False,
                .EtiquetaAccion = "Verificar",
                .Ejecutable = New Implementadas.VerificarLicitacion()
            })
            lista.Add(New DefinicionOperacion With {
                .Nombre = "Actualizar email, móvil y teléfono",
                .Seccion = SeccionOperacion.Contratos,
                .Descripcion = "Actualiza los contactos de los contratos de un Excel de tres columnas: " &
                               "CodContrato, Emails (varios separados por punto y coma) y TlfnoMovil.",
                .Requiere = EntradasOperacion.Excel,
                .EsEscritura = True,
                .EtiquetaAccion = "Actualizar contactos",
                .Ejecutable = New Implementadas.ActualizarContactos()
            })

            ' ============================================================
            ' PRODUCTOS — escritura
            ' ============================================================
            lista.Add(New DefinicionOperacion With {
                .Nombre = "Añadir productos a contratos",
                .Seccion = SeccionOperacion.Productos,
                .Descripcion = "Asigna un producto a una lista de contratos. Al elegir el producto se " &
                               "rellenan solos su importe, su impuesto y sus casillas; se pueden cambiar.",
                .Requiere = EntradasOperacion.Contratos Or EntradasOperacion.Cups Or EntradasOperacion.Cifs,
                .EsEscritura = True,
                .EtiquetaAccion = "Asignar producto",
                .Ejecutable = New Implementadas.AnadirProductos(),
                .Campos = New List(Of CampoOperacion) From {
                    New CampoOperacion With {
                        .Clave = Implementadas.AnadirProductos.ClaveProducto,
                        .Etiqueta = "Producto",
                        .Ayuda = "Salen los de luz y los de gas. Cada contrato se comprueba: si el producto " &
                                 "no es de su entorno, ese contrato se deja igual y se dice por qué.",
                        .Tipo = TipoCampo.Seleccion,
                        .Origen = OrigenLista.Productos
                    },
                    New CampoOperacion With {
                        .Clave = Implementadas.AnadirProductos.ClaveImporte,
                        .Etiqueta = "Importe",
                        .Ayuda = "Si se deja vacío se usa el importe que tenga el producto.",
                        .Tipo = TipoCampo.Importe,
                        .Requerido = False
                    },
                    New CampoOperacion With {
                        .Clave = Implementadas.AnadirProductos.ClaveImpuesto,
                        .Etiqueta = "Tipo de impuesto",
                        .Ayuda = "Si el contrato tiene su propio tipo de impuesto, manda el del contrato.",
                        .Tipo = TipoCampo.Seleccion,
                        .Origen = OrigenLista.TiposImpuesto,
                        .EtiquetaVacio = "— el del producto —",
                        .Requerido = False
                    },
                    New CampoOperacion With {
                        .Clave = Implementadas.AnadirProductos.ClaveDesde,
                        .Etiqueta = "Fecha inicial",
                        .Ayuda = "Si se deja vacía se usa hoy.",
                        .Tipo = TipoCampo.Fecha,
                        .Requerido = False
                    },
                    New CampoOperacion With {
                        .Clave = Implementadas.AnadirProductos.ClaveHasta,
                        .Etiqueta = "Fecha final",
                        .Ayuda = "Opcional. Vacía significa sin fecha de fin.",
                        .Tipo = TipoCampo.Fecha,
                        .Requerido = False
                    },
                    New CampoOperacion With {
                        .Clave = Implementadas.AnadirProductos.ClaveAntesIe,
                        .Etiqueta = "Aplicar antes del impuesto eléctrico",
                        .Tipo = TipoCampo.Booleano, .Requerido = False
                    },
                    New CampoOperacion With {
                        .Clave = Implementadas.AnadirProductos.ClaveSobreConsumo,
                        .Etiqueta = "Aplicar sobre el consumo",
                        .Tipo = TipoCampo.Booleano, .Requerido = False
                    },
                    New CampoOperacion With {
                        .Clave = Implementadas.AnadirProductos.ClavePrecioSobreConsumo,
                        .Etiqueta = "El precio va sobre el consumo",
                        .Tipo = TipoCampo.Booleano, .Requerido = False
                    },
                    New CampoOperacion With {
                        .Clave = Implementadas.AnadirProductos.ClavePrecioDia,
                        .Etiqueta = "El precio va por día",
                        .Tipo = TipoCampo.Booleano, .Requerido = False
                    }
                }
            })
            ' El «redondear» es la casilla CheckRedondear de ProductosAsig: afecta a Importe y a
            ' ImporteTotalPlazo, y a nada más.
            lista.Add(New DefinicionOperacion With {
                .Nombre = "Importar productos desde Excel",
                .Seccion = SeccionOperacion.Productos,
                .Descripcion = "Carga asignaciones de producto en bloque desde la plantilla. " &
                               "Cada fila es un contrato con su producto; el producto se busca por su texto " &
                               "en el entorno del contrato.",
                .Requiere = EntradasOperacion.Excel,
                .EsEscritura = True,
                .EtiquetaAccion = "Importar productos",
                .Ejecutable = New Implementadas.ImportarProductos(),
                .Campos = New List(Of CampoOperacion) From {
                    New CampoOperacion With {
                        .Clave = Implementadas.ImportarProductos.ClaveRedondear,
                        .Etiqueta = "Redondear los importes a dos decimales",
                        .Tipo = TipoCampo.Booleano,
                        .Requerido = False
                    }
                }
            })
            lista.Add(New DefinicionOperacion With {
                .Nombre = "Generar plantilla de productos",
                .Seccion = SeccionOperacion.Productos,
                .Descripcion = "Crea el Excel vacío con las doce columnas que espera la importación.",
                .Requiere = EntradasOperacion.Carpeta,
                .EsEscritura = False,
                .EtiquetaAccion = "Generar plantilla",
                .Ejecutable = New Implementadas.GenerarPlantillaProductos()
            })

            ' ============================================================
            ' CONSULTAS — lectura. Portadas desde CatalogoConsultas.vb
            ' ============================================================
            lista.AddRange(Consultas())

            ' ============================================================
            ' FACTURAS Y PDF — lectura
            ' ============================================================
            lista.Add(New DefinicionOperacion With {
                .Nombre = "Extraer PDF de facturas",
                .Seccion = SeccionOperacion.Facturas,
                .Descripcion = "Descarga el PDF de cada factura a la carpeta elegida y, al terminar, " &
                               "los une todos en Facturas_Unificadas.pdf.",
                .Requiere = EntradasOperacion.Facturas Or EntradasOperacion.Carpeta,
                .EtiquetaAccion = "Extraer PDF",
                .Ejecutable = New Implementadas.ExtraerPdfFacturas()
            })
            lista.Add(New DefinicionOperacion With {
                .Nombre = "Extraer PDF por cliente",
                .Seccion = SeccionOperacion.Facturas,
                .Descripcion = "Baja el PDF de cada factura a una subcarpeta por cliente.",
                .Requiere = EntradasOperacion.Facturas Or EntradasOperacion.Carpeta,
                .EtiquetaAccion = "Extraer por cliente",
                .Ejecutable = New Implementadas.ExtraerPdfPorCliente()
            })
            ' El nombre del catálogo decía «une en un solo PDF». No: el original solo agrupa
            ' en carpetas, no une nada. Corregida la descripción, no el comportamiento. La que
            ' une es «Extraer PDF de facturas».
            lista.Add(New DefinicionOperacion With {
                .Nombre = "PDF agrupado por nº de pedido",
                .Seccion = SeccionOperacion.Facturas,
                .Descripcion = "Baja el PDF de cada factura a una subcarpeta por número de pedido de facturación.",
                .Requiere = EntradasOperacion.Facturas Or EntradasOperacion.Carpeta,
                .EtiquetaAccion = "Agrupar PDF",
                .Ejecutable = New Implementadas.ExtraerPdfPorPedido()
            })
            ' OJO: esto ESCRIBE. Estaba catalogada como lectura y descrita como «comprueba»,
            ' pero el botón original hace un UPDATE de lectura.Perfilar = 1. Catalogada así se
            ' podía lanzar contra Réplica creyendo que no tocaba nada.
            lista.Add(New DefinicionOperacion With {
                .Nombre = "Marcar perfilar en lecturas",
                .Seccion = SeccionOperacion.Facturas,
                .Descripcion = "Pone Perfilar = 1 en las lecturas de las facturas ATR indicadas.",
                .Requiere = EntradasOperacion.Facturas,
                .EsEscritura = True,
                .EtiquetaAccion = "Marcar perfilar",
                .Ejecutable = New Implementadas.MarcarPerfilar()
            })
            lista.Add(New DefinicionOperacion With {
                .Nombre = "Trocear XML",
                .Seccion = SeccionOperacion.Facturas,
                .Descripcion = "Parte un XML grande en ficheros más pequeños, cortando por nodo completo.",
                .Requiere = EntradasOperacion.Carpeta,
                .EtiquetaAccion = "Trocear",
                .Ejecutable = New Implementadas.TrocearXml(),
                .Campos = New List(Of CampoOperacion) From {
                    New CampoOperacion With {
                        .Clave = Implementadas.TrocearXml.ClaveEntrada,
                        .Etiqueta = "Fichero XML de entrada",
                        .Tipo = TipoCampo.Fichero,
                        .FiltroFichero = "Ficheros XML (*.xml)|*.xml|Todos los ficheros (*.*)|*.*"
                    },
                    New CampoOperacion With {
                        .Clave = Implementadas.TrocearXml.ClaveTipo,
                        .Etiqueta = "Tipo de XML",
                        .Ayuda = "Sale de Config\TipoXml.json. El nodo es por donde se corta; la raíz, el elemento que envuelve cada trozo.",
                        .Tipo = TipoCampo.Seleccion,
                        .Origen = OrigenLista.TiposXml
                    },
                    New CampoOperacion With {
                        .Clave = Implementadas.TrocearXml.ClaveTamano,
                        .Etiqueta = "Tamaño máximo por fichero (KB)",
                        .Ayuda = "Se corta al pasarse, siempre entre nodo y nodo: ningún trozo queda partido por la mitad.",
                        .Tipo = TipoCampo.Numero,
                        .ValorInicial = "5000",
                        .Minimo = 1,
                        .Maximo = 2000000
                    }
                }
            })

            ' ============================================================
            ' AJUSTES
            '
            ' NOTA: aquí había una entrada «Administradores» y otra «Agentes». Se han quitado:
            ' los formularios AdministradoresWF y Agentes de ActualizaPrecios no son mantenimiento
            ' de esas tablas, son las pantallas de «Cambiar agente» y «Cambiar administrador» del
            ' contrato, que ya están en la sección de Contratos. Tenerlas dos veces solo confundía.
            '
            ' Y «Validación de plantillas Excel» tampoco existía: se dedujo del nombre de la clase
            ' ValidacionExcel, que en realidad es un cajón con conversores a CSV y penalizaciones.
            ' El botón «Validaciones» del original es lo que hay abajo: tres consultas a Excel.
            ' ============================================================
            lista.Add(New DefinicionOperacion With {
                .Nombre = "Empresas y bases de datos",
                .Seccion = SeccionOperacion.Ajustes,
                .Descripcion = "Las bases de clientes contra las que se suben los modelos de impresión: servidor, base, " &
                               "credenciales y si hace falta VPN. No son los entornos de las operaciones.",
                .EsEscritura = True,
                .PaginaPropia = PaginasPropias.Empresas
            })
            lista.Add(New DefinicionOperacion With {
                .Nombre = "Modelos de impresión",
                .Seccion = SeccionOperacion.Ajustes,
                .Descripcion = "Los modelos con los que SIGE imprime facturas y contratos, por empresa. Permite subir " &
                               "un .rpt a varias empresas de golpe y comprobar cuáles lo tienen.",
                .EsEscritura = True,
                .PaginaPropia = PaginasPropias.ModelosImpresion
            })
            lista.Add(New DefinicionOperacion With {
                .Nombre = "Validaciones",
                .Seccion = SeccionOperacion.Ajustes,
                .Descripcion = "Los tres scripts de validación de contratos, cada uno en su hoja del mismo " &
                               "Excel. No pide nada y no cambia nada.",
                .Requiere = EntradasOperacion.Ninguna,
                .EsEscritura = False,
                .EtiquetaAccion = "Ejecutar validaciones",
                .Ejecutable = New Implementadas.ConsultaMultiHoja(
                    Sql, "Validaciones",
                    {New Implementadas.HojaConsulta With {.Nombre = "Validacion1", .Plantilla = "Validaciones_1"},
                     New Implementadas.HojaConsulta With {.Nombre = "Validacion2", .Plantilla = "Validaciones_2"},
                     New Implementadas.HojaConsulta With {.Nombre = "Validacion3", .Plantilla = "Validaciones_3"}})
            })

            Return lista
        End Function

        ''' <summary>
        ''' Las 20 consultas de ActualizaPrecios, con las mismas entradas requeridas que
        ''' declara su catálogo original.
        ''' </summary>
        ''' <summary>
        ''' Consulta del subgrupo Generales. Es un método y no una lambda porque los
        ''' parámetros Optional de una lambda en VB se compilan como obligatorios.
        ''' </summary>
        Private Function General(nombre As String,
                                 requiere As EntradasOperacion,
                                 descripcion As String,
                                 Optional dividir As Boolean = False,
                                 Optional ejecutable As IOperacionEjecutable = Nothing) As DefinicionOperacion
            Return New DefinicionOperacion With {
                .Nombre = nombre,
                .Seccion = SeccionOperacion.Consultas,
                .Subgrupo = SubgrupoGenerales,
                .Descripcion = descripcion,
                .Requiere = requiere Or EntradasOperacion.Carpeta,
                .PermiteDividir = dividir,
                .EsEscritura = False,
                .EtiquetaAccion = "Consultar",
                .Ejecutable = ejecutable
            }
        End Function

        Private _sqlCompartido As Consultas.RepositorioSql

        ''' <summary>
        ''' Lector de las plantillas .sql. Uno para toda la aplicación: cachea lo que lee y no
        ''' tiene estado que dependa de la ejecución.
        '''
        ''' SE CREA AL PEDIRLO, no en un campo con inicializador. Los campos de un Module se
        ''' inicializan en orden de declaración, y este se declara después de Todas: siendo un
        ''' campo, cuando Construir() corría todavía valía Nothing y las 21 consultas se
        ''' quedaban sin lector. Así el orden dentro del fichero deja de importar.
        ''' </summary>
        Private ReadOnly Property Sql As Consultas.RepositorioSql
            Get
                If _sqlCompartido Is Nothing Then _sqlCompartido = New Consultas.RepositorioSql()
                Return _sqlCompartido
            End Get
        End Property

        ''' <summary>
        ''' Atajo para las consultas de «fechas y un Excel», que son diez y solo se diferencian
        ''' en el .sql y en el nombre del fichero.
        ''' </summary>
        ''' <summary>Atajo para las consultas que se lanzan una vez por entrada de la lista.</summary>
        Private Function PorLista(plantilla As String,
                                  prefijoFichero As String,
                                  marcadorEntrada As String,
                                  Optional formatoFecha As String = Nothing) As IOperacionEjecutable
            Return New Implementadas.ConsultaPorLista(
                Sql, plantilla, prefijoFichero, marcadorEntrada,
                If(formatoFecha, Implementadas.ConsultaPorFechas.FormatoBarras))
        End Function

        Private Function PorFechas(plantilla As String,
                                   nombreFichero As String,
                                   Optional nombreHoja As String = "",
                                   Optional pideFechas As Boolean = True) As IOperacionEjecutable
            Return New Implementadas.ConsultaPorFechas(
                Sql, plantilla, nombreFichero,
                If(String.IsNullOrEmpty(nombreHoja), nombreFichero, nombreHoja),
                Implementadas.ConsultaPorFechas.FormatoBarras,
                pideFechas)
        End Function

        ''' <summary>
        ''' Curva del subgrupo Pool. Todas piden CUPS, fechas y carpeta, y todas van contra la
        ''' base SigeTotalTM del servidor del entorno elegido, no contra SigeTotal.
        ''' </summary>
        Private Function Pool(nombre As String, plantilla As String, prefijo As String) As DefinicionOperacion
            Return New DefinicionOperacion With {
                .Nombre = nombre,
                .Seccion = SeccionOperacion.Consultas,
                .Subgrupo = SubgrupoPool,
                .Descripcion = "Curva de consumo por CUPS y rango de fechas. Va contra la base SigeTotalTM.",
                .Requiere = EntradasOperacion.Cups Or EntradasOperacion.Fechas Or EntradasOperacion.Carpeta,
                .PermiteDividir = True,
                .EsEscritura = False,
                .EtiquetaAccion = "Consultar",
                .BaseDatosAlternativa = "SigeTotalTM",
                .Ejecutable = New Implementadas.ConsultaPorLista(
                    Sql, plantilla, prefijo, "joinCupsReplace",
                    Implementadas.ConsultaPorFechas.FormatoCompacto,
                    transformar:=AddressOf Implementadas.ConsultaPorLista.ComoListaIn)
            }
        End Function

        Private Function Consultas() As IEnumerable(Of DefinicionOperacion)
            Return New List(Of DefinicionOperacion) From {
                General("Clicks TODO (Luz y Gas)", EntradasOperacion.Ninguna,
                        "Un único Excel con dos hojas: Luz y Gas. No necesita ningún dato.",
                        ejecutable:=New Implementadas.ConsultaMultiHoja(
                            Sql, "Consulta_ClicksTODO_LuzGas",
                            {New Implementadas.HojaConsulta With {.Nombre = "Luz", .Plantilla = "GetClickLuz"},
                             New Implementadas.HojaConsulta With {.Nombre = "Gas", .Plantilla = "GetClickGas"}})),
                General("Hunosa", EntradasOperacion.Fechas, "Facturación de Hunosa en el rango indicado.",
                        ejecutable:=PorFechas("GetHunosa", "Hunosa")),
                General("Cadasa", EntradasOperacion.Fechas, "Facturación de Cadasa en el rango indicado.",
                        ejecutable:=PorFechas("GetCadasa", "Cadasa")),
                General("Quantum", EntradasOperacion.Fechas, "Facturación de Quantum en el rango indicado.",
                        ejecutable:=PorFechas("GetQuantum", "Quantum")),
                General("Rechazos Veolia", EntradasOperacion.Ninguna, "Rechazos pendientes de Veolia.",
                        ejecutable:=PorFechas("GetRechazosVeolia", "RechazosVeolia", "Veolia", pideFechas:=False)),
                General("GAM", EntradasOperacion.Fechas, "Facturación de GAM en el rango indicado.",
                        ejecutable:=PorFechas("GAM", "GAM")),
                General("CAM", EntradasOperacion.Fechas, "Facturación de CAM en el rango indicado.",
                        ejecutable:=PorFechas("ConsultaFacturasCAM", "CAM")),
                General("Desglosado Trébol Luz by CIF", EntradasOperacion.Cifs Or EntradasOperacion.Fechas,
                        "Facturas de luz (entorno E1) por CIF y rango de fechas.", dividir:=True,
                        ejecutable:=PorLista("ConsultaFacturasTREBOL_ELEC__ByIdentidadFechas", "TREBOL_LUZ", "identidadReplace")),
                General("Desglosado Trébol Gas by CIF", EntradasOperacion.Cifs Or EntradasOperacion.Fechas,
                        "Facturas de gas (entorno E2) por CIF y rango de fechas.", dividir:=True,
                        ejecutable:=PorLista("ConsultaFacturasTREBOL_GAS_ByIdentidadFechas", "TREBOL_GAS", "identidadReplace")),
                General("Grupo SRS", EntradasOperacion.Cifs Or EntradasOperacion.Fechas,
                        "Se entrega partida por CIF. Desmarca la casilla para juntarlo todo.", dividir:=True,
                        ejecutable:=PorLista("ConsultaNorauto", "", "CIFReplace",
                                             Implementadas.ConsultaPorFechas.FormatoGuiones)),
                General("Trébol Luz", EntradasOperacion.Fechas, "Facturas de luz del entorno E1 en el rango indicado.",
                        ejecutable:=PorFechas("ConsultaFacturasTREBOL_ELEC_V5", "TREBOL_LUZ", "TrebolLuz")),
                General("Trébol Gas", EntradasOperacion.Fechas, "Facturas de gas del entorno E2 en el rango indicado.",
                        ejecutable:=PorFechas("ConsultaFacturasTREBOL_GAS", "TREBOL_GAS", "TrebolGas")),
                General("JC Castilla-La Mancha", EntradasOperacion.Fechas, "Facturación de la Junta de Castilla-La Mancha.",
                        ejecutable:=PorFechas("Consulta_JC CASTILLA LA MANCHA", "JC_CastillaLaMancha", "JCCLM")),
                General("Santa Lucía Trébol Gas", EntradasOperacion.Fechas,
                        "Facturas de gas (E2) del grupo de tarifa Santa Lucía.",
                        ejecutable:=PorFechas("ConsultaFacturas_SantaLucia_TREBOL_GAS_v2", "SantaLucia_TREBOL_GAS", "SantaLuciaGas")),
                General("Cogeneración", EntradasOperacion.Fechas,
                        "Primer paso. Al terminar, abre el Excel y usa «Cogeneración por factura» con los IDs de la columna A.",
                        ejecutable:=PorFechas("ConsultaCogeneracionLidia_V7_2025", "ConsultaCogeneracion", "Cogeneracion")),
                General("Cogeneración por factura", EntradasOperacion.Facturas,
                        "Segundo paso: detalle por equipo de los IDs de factura que devolvió Cogeneración.",
                        ejecutable:=New Implementadas.ConsultaPorFechas(
                            Sql, "ConsultaCogeneracionLidia__V7_PorEquipo_2025", "ConsultaCogeneracionPorFactura",
                            "PorFactura", Implementadas.ConsultaPorFechas.FormatoBarras, pideFechas:=False,
                            extra:=Sub(pl, c) pl.Poner("IdsFacturasReplace", String.Join(",", c.Entradas)))),
                General("Cuentas LB2B", EntradasOperacion.Texto Or EntradasOperacion.Fechas,
                        "Busca por parte del nombre del agente; no hace falta escribirlo completo.",
                        ejecutable:=New Implementadas.ConsultaPorFechas(
                            Sql, "Consulta_Cuentas_LB2B", "Cuentas_LB2B", "CuentasLB2B",
                            Implementadas.ConsultaPorFechas.FormatoBarras, pideFechas:=True,
                            extra:=Sub(pl, c) pl.Poner("NombreAgenteReplace", c.Texto))),
                General("Energía Activa y Reactiva", EntradasOperacion.Facturas,
                        "Lecturas de activa, reactiva y varios de las facturas indicadas.",
                        ejecutable:=New Implementadas.ConsultaPorFechas(
                            Sql, "ConsultaLecturaActivaReactivayVarios", "EnergiaActivaReactiva",
                            "ActivaReactiva", Implementadas.ConsultaPorFechas.FormatoBarras, pideFechas:=False,
                            extra:=Sub(pl, c) pl.Poner("ListaFacturasParam", "'" & String.Join("','", c.Entradas) & "'"))),
                Pool("Curva Horaria", "CurvaHoraria", "CH"),
                Pool("Curva Cuarto Horaria", "CurvaCuartoHoraria", "QH"),
                Pool("Curva Facturable", "CurvaFacturable", "CF")
            }
        End Function

        Public Function DeSeccion(seccion As SeccionOperacion) As IReadOnlyList(Of DefinicionOperacion)
            Return Todas.Where(Function(o) o.Seccion = seccion).ToList()
        End Function

        Public Function CuantasEn(seccion As SeccionOperacion) As Integer
            Return Todas.Where(Function(o) o.Seccion = seccion).Count()
        End Function

    End Module

End Namespace
