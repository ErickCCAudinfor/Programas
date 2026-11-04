using System.Runtime.InteropServices;

namespace Convertidor.Conversion;

/// <summary>
/// Word, Excel y PowerPoint por COM, con enlace tardío (dynamic): no hace falta referenciar
/// los ensamblados de interop y vale para cualquier versión de Office instalada.
///
/// Cada aplicación se abre la primera vez que hace falta y se cierra al acabar la tanda.
///
/// CUIDADO AL CERRAR: PowerPoint es de instancia única. Si ya lo tienes abierto, pedirlo por
/// COM devuelve ese mismo PowerPoint, y un Quit() te cerraría tus presentaciones. Por eso se
/// anota qué había abierto al empezar, y solo se cierra la aplicación si al empezar no había
/// nada y al acabar tampoco.
///
/// Todo esto tiene que llamarse desde un hilo STA (ver MainWindow.EjecutarEnSta).
/// </summary>
public sealed class SesionOffice : IDisposable
{
    // msoAutomationSecurityForceDisable: abrir sin ejecutar macros
    private const int SinMacros = 3;

    private dynamic? _word, _excel, _powerPoint;
    private bool _cerrarWord, _cerrarExcel, _cerrarPowerPoint;

    public dynamic Word
    {
        get
        {
            if (_word != null) return _word;
            _word = Crear("Word.Application", "Microsoft Word");
            _word.Visible = false;
            _word.DisplayAlerts = 0; // wdAlertsNone
            _word.AutomationSecurity = SinMacros;
            _cerrarWord = _word.Documents.Count == 0;
            return _word;
        }
    }

    public dynamic Excel
    {
        get
        {
            if (_excel != null) return _excel;
            _excel = Crear("Excel.Application", "Microsoft Excel");
            _excel.Visible = false;
            _excel.DisplayAlerts = false;
            _excel.AutomationSecurity = SinMacros;
            _cerrarExcel = _excel.Workbooks.Count == 0;
            return _excel;
        }
    }

    public dynamic PowerPoint
    {
        get
        {
            if (_powerPoint != null) return _powerPoint;
            _powerPoint = Crear("PowerPoint.Application", "Microsoft PowerPoint");
            // A PowerPoint no se le puede poner Visible = false: da error. Se abren las
            // presentaciones sin ventana (WithWindow = false) y queda igual de oculto.
            _powerPoint.DisplayAlerts = 1; // ppAlertsNone
            _powerPoint.AutomationSecurity = SinMacros;
            _cerrarPowerPoint = _powerPoint.Presentations.Count == 0;
            return _powerPoint;
        }
    }

    private static dynamic Crear(string progId, string nombre)
    {
        var tipo = Type.GetTypeFromProgID(progId)
                   ?? throw new InvalidOperationException($"hace falta {nombre} instalado en este equipo");
        return Activator.CreateInstance(tipo)
               ?? throw new InvalidOperationException($"no se ha podido abrir {nombre}");
    }

    public static void Liberar(object? com)
    {
        if (com != null && Marshal.IsComObject(com))
            Marshal.FinalReleaseComObject(com);
    }

    public void Dispose()
    {
        try { if (_word != null && _cerrarWord && _word!.Documents.Count == 0) _word!.Quit(0); } catch { }
        try { if (_excel != null && _cerrarExcel && _excel!.Workbooks.Count == 0) _excel!.Quit(); } catch { }
        try { if (_powerPoint != null && _cerrarPowerPoint && _powerPoint!.Presentations.Count == 0) _powerPoint!.Quit(); } catch { }

        Liberar(_word); Liberar(_excel); Liberar(_powerPoint);
        _word = _excel = _powerPoint = null;

        // Sin esto el proceso de Office puede quedarse colgado en el Administrador de tareas
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }
}
