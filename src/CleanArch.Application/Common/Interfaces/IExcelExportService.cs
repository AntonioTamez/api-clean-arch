namespace CleanArch.Application.Common.Interfaces;

/// <summary>
/// Servicio para exportar datos a Excel
/// </summary>
public interface IExcelExportService
{
    /// <summary>
    /// Exporta datos genéricos a Excel
    /// </summary>
    byte[] ExportToExcel<T>(IEnumerable<T> data, string sheetName) where T : class;
    
    /// <summary>
    /// Exporta múltiples hojas a un archivo Excel
    /// </summary>
    byte[] ExportMultipleSheetsToExcel(Dictionary<string, object> sheets);
}
