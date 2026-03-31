using System.Collections.Generic;

public class CrystalRequest
{
    public byte[] ReportModel { get; set; }
    public byte[] DataSetBinary { get; set; }
    public string DataSetXml { get; set; } // Cambiado de byte[] a string
    public Dictionary<string, object> Parameters { get; set; }
}