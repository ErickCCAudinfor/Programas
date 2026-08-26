select  distinct
fi.IdFormulaIndexado
,CUPS.codigocups
,CodigoContrato
,c.IdContratoSituacion
,cs.textosituacion
,tc.textocoeficiente As Mercado
,case when fi.ismezcla=1 then 'SI'
 when fi.ismezcla=0 then 'NO'END AS AplicarPFijo
,replace(fi.valorpreciofijo,'.',',') as 'Precio Fijo (€/MWh)'
,case when fi.aplicarporcentaje=1 then 'SI'
 when fi.aplicarporcentaje=0 then 'NO'END  As AplicarPorcentaje
,replace(fi.Porcentaje,'.',',') as 'Porcentaje (% P.Fijo)'
,case when fi.Iscierrecargabase=1 then 'SI'
 when ISNULL(fi.Iscierrecargabase,0)=0  then 'NO' END As CierreEnCargaBase
,replace(ISNULL(fi.Consumocargabase,0),'.',',') As MWCargaBase
,fi.fechadesde as DesdeFecha
,fi.fechahasta As HastaFecha
from formulaindexado fi 
inner join cups on fi.idcups = cups.idcups
inner join Contrato c on fi.IdCups = c.IdCups
inner join contratosituacion cs  on c.idcontratosituacion = cs.idcontratosituacion
inner join tipocoeficiente tc on tc.idtipocoeficiente=fi.idtipocoeficiente