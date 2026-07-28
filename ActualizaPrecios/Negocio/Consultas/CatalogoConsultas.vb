''' <summary>
''' Catálogo de consultas que se ofrecen en el desplegable del formulario principal.
'''
''' PARA AÑADIR UNA CONSULTA NUEVA: añade aquí una DefinicionConsulta y ya aparece en el
''' combo, con sus campos habilitados y validados. No hay que tocar el diseñador ni Form1.
'''
''' Grupo, Nombre    -> lo que se lee en el desplegable.
''' Requiere         -> qué campos habilita y exige el formulario antes de lanzar.
''' UsaConexionTM    -> True si va contra SigeTotalTM en vez de contra SigeTotal.
''' PermiteDividir   -> True si tiene sentido "un fichero por CUPS".
'''
''' OJO al editar: no dejes líneas en blanco dentro de la lista de abajo. VB corta ahí la
''' continuación implícita de línea y deja de compilar.
''' </summary>
Public Module CatalogoConsultas

    Public Const GrupoGenerales As String = "Generales"
    Public Const GrupoPool As String = "Pool (CUPS)"

    Public ReadOnly Todas As New List(Of DefinicionConsulta) From {
        New DefinicionConsulta With {
            .Nombre = "Clicks TODO (Luz y Gas)",
            .Grupo = GrupoGenerales,
            .Requiere = EntradasConsulta.Ninguna,
            .Descripcion = "Genera un único Excel con dos hojas: Luz y Gas.",
            .Ejecutar = Function(ctx) EjecutarHojas(ctx, "Consulta_ClicksTODO_LuzGas", {
                New HojaConsulta With {.Nombre = "Luz", .Sql = Function() ConsultasSQL.GetClickLuz},
                New HojaConsulta With {.Nombre = "Gas", .Sql = Function() ConsultasSQL.GetClickGas}
            })
        },
        New DefinicionConsulta With {
            .Nombre = "Hunosa",
            .Grupo = GrupoGenerales,
            .Requiere = EntradasConsulta.Fechas,
            .Ejecutar = Function(ctx) EjecutarSimple(ctx, "Hunosa", "Hunosa",
                                                     Function() ConsultasSQL.GetHunosa(ctx.Desde, ctx.Hasta))
        },
        New DefinicionConsulta With {
            .Nombre = "Cadasa",
            .Grupo = GrupoGenerales,
            .Requiere = EntradasConsulta.Fechas,
            .Ejecutar = Function(ctx) EjecutarSimple(ctx, "Cadasa", "Cadasa",
                                                     Function() ConsultasSQL.GetCadasa(ctx.Desde, ctx.Hasta))
        },
        New DefinicionConsulta With {
            .Nombre = "Quantum",
            .Grupo = GrupoGenerales,
            .Requiere = EntradasConsulta.Fechas,
            .Ejecutar = Function(ctx) EjecutarSimple(ctx, "Quantum", "Quantum",
                                                     Function() ConsultasSQL.GetQuantum(ctx.Desde, ctx.Hasta))
        },
        New DefinicionConsulta With {
            .Nombre = "Rechazos Veolia",
            .Grupo = GrupoGenerales,
            .Requiere = EntradasConsulta.Ninguna,
            .Ejecutar = Function(ctx) EjecutarSimple(ctx, "RechazosVeolia", "Veolia",
                                                     Function() ConsultasSQL.GetRechazosVeolia())
        },
        New DefinicionConsulta With {
            .Nombre = "GAM",
            .Grupo = GrupoGenerales,
            .Requiere = EntradasConsulta.Fechas,
            .Ejecutar = Function(ctx) EjecutarSimple(ctx, "GAM", "GAM",
                                                     Function() ConsultasSQL.GetGAM(ctx.Desde, ctx.Hasta))
        },
        New DefinicionConsulta With {
            .Nombre = "CAM",
            .Grupo = GrupoGenerales,
            .Requiere = EntradasConsulta.Fechas,
            .Ejecutar = Function(ctx) EjecutarSimple(ctx, "CAM", "CAM",
                                                     Function() ConsultasSQL.GetCAM(ctx.Desde, ctx.Hasta))
        },
        New DefinicionConsulta With {
            .Nombre = "Trébol (Luz y Gas)",
            .Grupo = GrupoGenerales,
            .Requiere = EntradasConsulta.Cifs,
            .Descripcion = "Genera un Excel por cada CIF de la lista.",
            .Ejecutar = Function(ctx) EjecutarPorEntrada(ctx, "TREBOL_LUZ",
                                                         Function(cif) ConsultasSQL.GetTrebolLuz_V2(cif))
        },
        New DefinicionConsulta With {
            .Nombre = "Grupo SRS",
            .Grupo = GrupoGenerales,
            .Requiere = EntradasConsulta.Cifs Or EntradasConsulta.Fechas,
            .Descripcion = "Genera un Excel por cada CIF de la lista.",
            .Ejecutar = Function(ctx) EjecutarPorEntrada(ctx, "",
                                                         Function(cif) ConsultasSQL.GetConsultaNorauto(ctx.Desde, ctx.Hasta, cif))
        },
        New DefinicionConsulta With {
            .Nombre = "Energía Activa y Reactiva",
            .Grupo = GrupoGenerales,
            .Requiere = EntradasConsulta.Facturas,
            .Ejecutar = Function(ctx) EjecutarSimple(ctx, "EnergiaActivaReactiva", "ActivaReactiva",
                                                     Function() ConsultasSQL.ConsultaLecturaActivaReactivayVarios(ctx.Entradas))
        },
        New DefinicionConsulta With {
            .Nombre = "Curva Horaria",
            .Grupo = GrupoPool,
            .Requiere = EntradasConsulta.Cups Or EntradasConsulta.Fechas,
            .PermiteDividir = True,
            .UsaConexionTM = True,
            .Ejecutar = Function(ctx) EjecutarCurva(ctx, "CH",
                                                    Function(c) ConsultasSQL.GetCurvaHoraria(ctx.Desde, ctx.Hasta, Nothing, c))
        },
        New DefinicionConsulta With {
            .Nombre = "Curva Cuarto Horaria",
            .Grupo = GrupoPool,
            .Requiere = EntradasConsulta.Cups Or EntradasConsulta.Fechas,
            .PermiteDividir = True,
            .UsaConexionTM = True,
            .Ejecutar = Function(ctx) EjecutarCurva(ctx, "QH",
                                                    Function(c) ConsultasSQL.GetCurvaCuartoHoraria(ctx.Desde, ctx.Hasta, Nothing, c))
        },
        New DefinicionConsulta With {
            .Nombre = "Curva Facturable",
            .Grupo = GrupoPool,
            .Requiere = EntradasConsulta.Cups Or EntradasConsulta.Fechas,
            .PermiteDividir = True,
            .UsaConexionTM = True,
            .Ejecutar = Function(ctx) EjecutarCurva(ctx, "CF",
                                                    Function(c) ConsultasSQL.GetCurvaFacturable(ctx.Desde, ctx.Hasta, Nothing, c))
        }
    }

End Module
