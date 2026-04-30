Public Class ConsultasSQL
	' Consulta para obtener todos Clicks Luz
	Public Shared ReadOnly GetClickLuz As String = ObtenerConsulta("GetClickLuz")

	' Consulta para obtener todos Clicks Gas
	Public Shared ReadOnly GetClickGas As String = ObtenerConsulta("GetClickGas")

	Public Shared Function GetHunosa(DesdeFecha As Date, hastaFecha As Date) As String
		Dim consulta As String = ObtenerConsulta("GetHunosa")
		consulta = consulta.Replace("DesdeFechaReplace", DesdeFecha.ToString("dd/MM/yyyy"))
		consulta = consulta.Replace("hastaFechaReplace", hastaFecha.ToString("dd/MM/yyyy"))
		Return consulta

	End Function

	Public Shared Function GetCadasa(DesdeFecha As Date, hastaFecha As Date) As String
		Dim consulta As String = ObtenerConsulta("GetCadasa")
		consulta = consulta.Replace("DesdeFechaReplace", DesdeFecha.ToString("dd/MM/yyyy"))
		consulta = consulta.Replace("hastaFechaReplace", hastaFecha.ToString("dd/MM/yyyy"))
		Return consulta
	End Function

	Public Shared Function GetRechazosVeolia() As String
		Dim consulta As String = ObtenerConsulta("GetRechazosVeolia")
		Return consulta
	End Function
	Public Shared Function GetQuantum(DesdeFecha As Date, hastaFecha As Date) As String
		Dim consulta As String = ObtenerConsulta("GetQuantum")
		consulta = consulta.Replace("DesdeFechaReplace", DesdeFecha.ToString("dd/MM/yyyy"))
		consulta = consulta.Replace("hastaFechaReplace", hastaFecha.ToString("dd/MM/yyyy"))

		Return consulta
	End Function
	Public Shared Function GetGAM(DesdeFecha As Date, hastaFecha As Date) As String
		Dim consulta As String = ObtenerConsulta("GAM")
		consulta = consulta.Replace("DesdeFechaReplace", DesdeFecha.ToString("dd/MM/yyyy"))
		consulta = consulta.Replace("hastaFechaReplace", hastaFecha.ToString("dd/MM/yyyy"))

		Return consulta
	End Function
	Public Shared Function GetCurvaHoraria(DesdeFecha As Date, hastaFecha As Date, Optional ListaCups As List(Of String) = Nothing, Optional Cups As String = "") As String
		Dim joinCups = ""
		If Cups.Length > 1 Then
			joinCups = $"'{Cups}'"
		End If
		If Not ListaCups Is Nothing AndAlso ListaCups.Count >= 1 Then
			joinCups = String.Join(",", ListaCups.Select(Function(c) $"'{c.Trim}'"))
		End If

		Dim consulta As String = ObtenerConsulta("CurvaHoraria")
		consulta = consulta.Replace("joinCupsReplace", joinCups)
		consulta = consulta.Replace("DesdeFechaReplace", DesdeFecha.ToString("dd/MM/yyyy"))
		consulta = consulta.Replace("hastaFechaReplace", hastaFecha.ToString("dd/MM/yyyy"))
		Return consulta
	End Function


	Public Shared Function GetCurvaCuartoHoraria(DesdeFecha As Date, hastaFecha As Date, Optional ListaCups As List(Of String) = Nothing, Optional Cups As String = "") As String
		Dim joinCups = ""
		If Cups.Length > 1 Then
			joinCups = $"'{Cups}'"
		End If
		If Not ListaCups Is Nothing AndAlso ListaCups.Count >= 1 Then
			joinCups = String.Join(",", ListaCups.Select(Function(c) $"'{c.Trim}'"))
		End If
		Dim consulta As String = ObtenerConsulta("CurvaCuartoHoraria")
		consulta = consulta.Replace("joinCupsReplace", joinCups)
		consulta = consulta.Replace("DesdeFechaReplace", DesdeFecha.ToString("dd/MM/yyyy"))
		consulta = consulta.Replace("hastaFechaReplace", hastaFecha.ToString("dd/MM/yyyy"))
		Return consulta
	End Function

	Public Shared Function GetCurvaFacturable(DesdeFecha As Date, hastaFecha As Date, Optional ListaCups As List(Of String) = Nothing, Optional Cups As String = "") As String
		Dim joinCups = ""
		If Cups.Length > 1 Then
			joinCups = $"'{Cups}'"
		End If
		If Not ListaCups Is Nothing AndAlso ListaCups.Count >= 1 Then
			joinCups = String.Join(",", ListaCups.Select(Function(c) $"'{c.Trim}'"))
		End If
		Dim consulta As String = ObtenerConsulta("CurvaFacturable")
		consulta = consulta.Replace("joinCupsReplace", joinCups)
		consulta = consulta.Replace("DesdeFechaReplace", DesdeFecha.ToString("dd/MM/yyyy"))
		consulta = consulta.Replace("hastaFechaReplace", hastaFecha.ToString("dd/MM/yyyy"))
		Return consulta
	End Function
	Public Shared Function GetConsultaContrato(codCntrato As List(Of Long)) As String
		Dim codCntratojoin = String.Join(",", codCntrato)
		Dim consulta As String = ObtenerConsulta("ConsultaContrato")
		consulta = consulta.Replace("codCntratojoinReplace", codCntratojoin)
		Return consulta

	End Function

	Public Shared Function GetConsultaNorauto(DesdeFecha As Date, hastaFecha As Date, CIF As String) As String

		Dim consulta As String = ObtenerConsulta("ConsultaNorauto")

		consulta = consulta.Replace("fechaReplace", $"{DesdeFecha:dd-MM-yyyy}")
		consulta = consulta.Replace("fechahastaReplace", $"{hastaFecha:dd-MM-yyyy}")
		consulta = consulta.Replace("CIFReplace", CIF)

		Return consulta

	End Function


	Public Shared Function validacionesScript1() As String
		Return "select c.codigocontrato
,cups.CodigoCUPS
,c.Confirmado
,convert(varchar,c.FechaCreacion, 103) as FechaCreacion
,convert(varchar,c.FechaPrevistaActivacion, 103) as FechaPrevistaActivacion
,stf.TextoFechaEfecto
,c.observaciones 
,cs.textosituacion 
from contrato c 
inner join cups on cups.IdCups = c.idcups
inner join ContratoSituacion cs on c.IdContratoSituacion = cs.IdContratoSituacion
left join SolicitudTipoFechaEfecto stf on stf.IdSolicitudTipoFechaEfecto = c.IdSolicitudTipoFechaEfecto
inner join Solicitud s on s.CodigoContrato = c.CodigoContrato and (s.idsolicitudtipo in (1009
,1010
,1011
,1013
,50103
,50112) or s.IdSolicitudTipo is null)
where c.idcontratosituacion in (4,14) and Confirmado=0
order by c.CodigoContrato"
	End Function
	Public Shared Function validacionesScript2() As String
		Return "select c.codigocontrato
,cups.CodigoCUPS
,c.Confirmado
,convert(varchar,c.FechaCreacion, 103) as FechaCreacion
,convert(varchar,c.FechaPrevistaActivacion, 103) as FechaPrevistaActivacion
,stf.TextoFechaEfecto
,c.observaciones 
,ss.Nombre SituacionSolicitud
,s.FechaApertura FechaAperturaSolicitud
from contrato c 
inner join cups on cups.IdCups = c.idcups
left join SolicitudTipoFechaEfecto stf on stf.IdSolicitudTipoFechaEfecto = c.IdSolicitudTipoFechaEfecto

inner join Solicitud s on s.CodigoContrato = c.CodigoContrato and s.IdUsuario in (1610,
1611,5900,5901,5902,5903,5904,5905,5906,5907,5908,5909,5910,5911,5884,5885,5886,5887,5888,5889,5890,5891,5892,5893,5894,5895,5896,5897,5898,5899) 
left join SolicitudSituacion ss on  s.IdSolicitudSituacion = ss.IdSolicitudSituacion
where c.idcontratosituacion=4 order by s.FechaApertura desc"
	End Function
	Public Shared Function validacionesScript3() As String
		Return ";with 
ContratoTarifaVigenteMaxima as
(    select CodigoContrato,
           max(FechaDesde) as FechaDesde
    from ContratoTarifa
    where GetDate() between FechaDesde and FechaHasta
    group by CodigoContrato)
,ContratoTarifaFechaAjustada as
(    select ct.CodigoContrato,
           ctvm.FechaDesde as fechaCTVM,
           ct.FechaDesde as fechaCT,
           case when ctvm.FechaDesde is not null then ctvm.FechaDesde else ct.FechaDesde end as Fecha
    from ContratoTarifa ct left join ContratoTarifaVigenteMaxima ctvm on ctvm.CodigoContrato = ct.CodigoContrato)
,ContratoTarifaFechaMaxima as
(    select CodigoContrato,
           max(Fecha) as FechaDesde
    from ContratoTarifaFechaAjustada
    group by CodigoContrato)
,ContratoTarifaVigente as
(    select ctfm.Codigocontrato,
           ctfm.FechaDesde,
           Entorno,
           IdTarifaGrupo,
           IdTarifa,
           IdPerfilFacturacion
    from ContratoTarifa ct 
	inner join ContratoTarifaFechaMaxima ctfm on ct.CodigoContrato = ctfm.CodigoContrato and ct.FechaDesde = ctfm.FechaDesde)
select Solicitud.IdSolicitud as Solicitud
,SolicitudTipo.NombreSolicitudTipo as TipoSolicitud
,u.Nombre
,SolicitudSituacion.Nombre as Situacion 
,Cliente.Identidad as Cliente
,Solicitud.CodigoContrato as Contrato
,Contrato.FechaCreacion as 'Fecha creacion contrato'
,CASE
	WHEN (Cliente.Nombre is null) OR (Cliente.Nombre = '')
		THEN Cliente.RazonSocial
		ELSE CONCAT(Cliente.Nombre, ' ' , ISNULL(Cliente.Apellido1, '') , ' ' , ISNULL(Cliente.Apellido2, ''))
		END as Nombre
,CUPS.CodigoCUPS as CUPS
,convert(varchar,Solicitud.FechaApertura,103) as 'Fecha apertura'
,convert(varchar,Solicitud.FechaCierre,103) as 'Fecha cierre'
,convert(varchar,Contrato.FechaAlta,103) as 'Fecha alta'
,convert(varchar,Contrato.FechaPrevistaActivacion,103) as 'F. Prev. Act.'
,SolicitudTipoFechaEfecto.TextoFechaEfecto as 'Texto Fecha Efecto'
,convert(varchar,Contrato.FechaPrevistaBaja,103) as 'F. Prev. Baja'
,convert(varchar,Contrato.FechaBaja,103) as 'Fecha baja'
,Motivobaja.TextoBaja as 'Motivo baja'
,MotivoRechazo.TextoRechazo as 'Motivo Rechazo'
,Solicitud.Observaciones 
,Case
	when Agente.CodigoTipoAgente=2
		then Agente.NombreAgente
    when Agente.CodigoTipoAgente=3
        then Agenteb.NombreAgente
        else null
    End As NombreAgente
,Case
	when Agente.CodigoTipoAgente=3
		then Agente.NombreAgente
        else null
    End As NombreSubAgente
,CASE
	when ClienteContactoTelefono.TipoContacto = 'T'
		then ClienteContactoTelefono.Valor
	when ClienteContactoTelefono.TipoContacto = 'M'
		then ClienteContactoTelefono.Valor
	end as TelefonoAgente
,Tarifa.TextoTarifa As Tarifa
, CONCAT( CallejeroTipoVia.TextoVia,' ',Callejero.NombreCalle, ' ', Cliente.Numero, ' ' ,Cliente.Aclarador) as Direccion
, Ciudad.TextoCiudad as Pobllacion
, ClienteContactoEmail.Valor as EMail
,eq.IdEquipoMedida as 'Nº Equipo Medida'
,ContratoPotenciaMaxima.PotenciaMaxima as 'Potencia Actual'
,Solicitud.ValorTrafo as 'Situación Libre'
,ModoLectura.Descripcion as 'Modo lectura'
from Solicitud with(nolock)
left join Usuario u with(nolock) on u.IdUsuario = Solicitud.IdUsuario
left join SolicitudTipoFechaEfecto on Solicitud.IdSolicitudTipoFechaEfecto=SolicitudTipoFechaEfecto.IdSolicitudTipoFechaEfecto
left join SolicitudTipo with(nolock) on SolicitudTipo.IdSolicitudTipo = Solicitud.IdSolicitudTipo
left join SolicitudSituacion with(nolock) on SolicitudSituacion.IdSolicitudSituacion = Solicitud.IdSolicitudSituacion
left join Contrato with(nolock) on Contrato.CodigoContrato = Solicitud.CodigoContrato
left join Cliente with(nolock) on Cliente.IdCliente = Contrato.IdCliente
left join CUPS with(nolock) on CUPS.IdCups = Contrato.IdCups
left join MotivoBaja with(nolock) on MotivoBaja.IdMotivoBaja = Contrato.IdMotivoBaja
left join MotivoRechazo with(nolock) on MotivoRechazo.IdMotivoRechazo = Solicitud.IdMotivoRechazo
left join Agente with(nolock) on Agente.IdAgente = Contrato.IdAgente
left join (select IdAgente, NombreAgente, IdAgenteNivelAnterior from Agente with(nolock)) as Agenteb on Agenteb.IdAgente = Agente.IdAgenteNivelAnterior
left join ClienteContacto as ClienteContactoTelefono with(nolock) on ClienteContactoTelefono.IdCliente = Cliente.IdCliente AND ClienteContactoTelefono.TipoContacto = 'T' and ClienteContactoTelefono.PorDefecto = 1
left join Tarifa with(nolock) on Tarifa.IdTarifa = Contrato.IdTarifa
left join Callejero with(nolock) on Callejero.IdCallejero = Cliente.IdCallejero
left join CallejeroTipoVia with(nolock) on CallejeroTipoVia.IdCallejeroTipoVia = Callejero.IdCallejeroTipoVia
left join Ciudad with(nolock) on Ciudad.IdCiudad = CUPS.IdCiudad
left join ClienteContacto as ClienteContactoEmail with(nolock) on ClienteContactoEmail.IdCliente = Cliente.IdCliente AND ClienteContactoEmail.TipoContacto = 'E' and ClienteContactoEmail.PorDefecto = 1
LEFT JOIN (Select Top 1 Entorno, CodigoContrato, NumeroSerie, IdEquipoModelo, Min(IdEquipoMedida) as IdEquipoMedida, IsInstalado as IsInstalado from EquipoMedida with(nolock) where isinstalado=1  group by Entorno, CodigoContrato, NumeroSerie, IdEquipoModelo, IsInstalado) as eq ON (Contrato.CodigoContrato = eq.CodigoContrato and eq.Entorno = 'G2')
LEFT JOIN EquipoModelo with(nolock) ON (eq.IdEquipoModelo =EquipoModelo.IdEquipoModelo) 
LEFT JOIN (
			SELECT	IdContrato, MAX(PotenciaContratada)	AS PotenciaMaxima 
				FROM ContratoPotencia with(nolock)
				GROUP BY IdContrato
			  ) AS ContratoPotenciaMaxima 
			  ON ContratoPotenciaMaxima.IdContrato = Contrato.IdContrato
LEFT JOIN ModoLectura with(nolock) ON Solicitud.IdModoLectura = ModoLectura.IdModoLectura
left join ContratoTarifaVigente with (nolock) on ContratoTarifaVigente.CodigoContrato = Contrato.CodigoContrato
left join TarifaGrupo with (nolock) on TarifaGrupo.IdTarifaGrupo = ContratoTarifaVigente.IdTarifaGrupo
where   Solicitud.FechaApertura  >= DATEADD (dd, 0, DATEDIFF (dd, 0, GETDATE() - 1))
order by Solicitud.IdSolicitudTipo, Solicitud.FechaApertura"
	End Function

	Public Shared Function ConsultaTop() As String
		Dim ConsultaTopDirectory = ObtenerConsulta("ConsultaTopParaLidia")
		Return ConsultaTopDirectory
	End Function

	Public Shared Function BuscarFacturaATR(facturasatr As List(Of String)) As String
		Dim consulta As String = ObtenerConsulta("BuscarFacturaATR")
		Dim facturasatrBD = String.Join(",", facturasatr.Select(Function(c) $"'{c.Trim}'"))
		consulta = consulta.Replace("facturasatrBDReplace", facturasatrBD)
		Return consulta
	End Function

	Public Shared Function GetTrebolGas(DesdeFecha As Date, hastaFecha As Date) As String
		Dim consulta As String = ObtenerConsulta("ConsultaFacturas_SantaLucia_TREBOL_GAS")
		consulta = consulta.Replace("DesdeFechaReplace", DesdeFecha.ToString("dd/MM/yyyy"))
		consulta = consulta.Replace("hastaFechaReplace", hastaFecha.ToString("dd/MM/yyyy"))
		Return consulta
	End Function
	Public Shared Function GetTrebolLuz(DesdeFecha As Date, hastaFecha As Date) As String
		Dim consulta As String = ObtenerConsulta("ConsultaFacturasTREBOL_ELEC")
		consulta = consulta.Replace("DesdeFechaReplace", DesdeFecha.ToString("dd/MM/yyyy"))
		consulta = consulta.Replace("hastaFechaReplace", hastaFecha.ToString("dd/MM/yyyy"))
		Return consulta
	End Function
	Public Shared Function GetCAM(DesdeFecha As Date, hastaFecha As Date) As String
		Dim consulta As String = ObtenerConsulta("ConsultaFacturasCAM")
		consulta = consulta.Replace("DesdeFechaReplace", DesdeFecha.ToString("dd/MM/yyyy"))
		consulta = consulta.Replace("hastaFechaReplace", hastaFecha.ToString("dd/MM/yyyy"))
		Return consulta
	End Function

	Public Shared Function GetTrebolLuz_V2(Identidad As String) As String
		Dim consulta As String = ObtenerConsulta("ConsultaFacturasTrebol_ELEC_By_Identidad")
		consulta = consulta.Replace("identidadReplace", Identidad)
		Return consulta

	End Function
End Class
