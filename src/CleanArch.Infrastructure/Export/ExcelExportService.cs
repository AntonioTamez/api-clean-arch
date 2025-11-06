using System.ComponentModel;
using CleanArch.Application.Common.Interfaces;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace CleanArch.Infrastructure.Export;

public class ExcelExportService : IExcelExportService
{
    public ExcelExportService()
    {
        // Configurar licencia de EPPlus (NonCommercial para desarrollo)
        ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
    }

    public byte[] ExportToExcel<T>(IEnumerable<T> data, string sheetName) where T : class
    {
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add(sheetName);

        // Obtener propiedades del tipo T
        var properties = typeof(T).GetProperties()
            .Where(p => p.CanRead && IsSimpleType(p.PropertyType))
            .ToList();

        if (!properties.Any())
            throw new InvalidOperationException("El tipo no tiene propiedades exportables");

        // Agregar headers
        for (int i = 0; i < properties.Count; i++)
        {
            var prop = properties[i];
            var displayName = GetDisplayName(prop);
            worksheet.Cells[1, i + 1].Value = displayName;
        }

        // Estilizar headers
        using (var range = worksheet.Cells[1, 1, 1, properties.Count])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }

        // Agregar datos
        var dataList = data.ToList();
        for (int row = 0; row < dataList.Count; row++)
        {
            var item = dataList[row];
            for (int col = 0; col < properties.Count; col++)
            {
                var value = properties[col].GetValue(item);
                worksheet.Cells[row + 2, col + 1].Value = value;
            }
        }

        // Auto-fit columnas
        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

        // Agregar filtros
        worksheet.Cells[1, 1, 1, properties.Count].AutoFilter = true;

        return package.GetAsByteArray();
    }

    public byte[] ExportMultipleSheetsToExcel(Dictionary<string, object> sheets)
    {
        using var package = new ExcelPackage();

        foreach (var sheet in sheets)
        {
            var sheetName = sheet.Key;
            var data = sheet.Value;

            var worksheet = package.Workbook.Worksheets.Add(sheetName);

            // Obtener el tipo de los datos
            var dataType = data.GetType();
            
            if (dataType.IsGenericType && 
                dataType.GetGenericTypeDefinition() == typeof(List<>) ||
                dataType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                var itemType = dataType.GetGenericArguments()[0];
                var properties = itemType.GetProperties()
                    .Where(p => p.CanRead && IsSimpleType(p.PropertyType))
                    .ToList();

                if (properties.Any())
                {
                    // Headers
                    for (int i = 0; i < properties.Count; i++)
                    {
                        worksheet.Cells[1, i + 1].Value = GetDisplayName(properties[i]);
                    }

                    // Estilizar headers
                    using (var range = worksheet.Cells[1, 1, 1, properties.Count])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);
                    }

                    // Datos
                    var items = ((System.Collections.IEnumerable)data).Cast<object>().ToList();
                    for (int row = 0; row < items.Count; row++)
                    {
                        for (int col = 0; col < properties.Count; col++)
                        {
                            var value = properties[col].GetValue(items[row]);
                            worksheet.Cells[row + 2, col + 1].Value = value;
                        }
                    }

                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                    worksheet.Cells[1, 1, 1, properties.Count].AutoFilter = true;
                }
            }
        }

        return package.GetAsByteArray();
    }

    private static bool IsSimpleType(Type type)
    {
        return type.IsPrimitive ||
               type.IsEnum ||
               type == typeof(string) ||
               type == typeof(decimal) ||
               type == typeof(DateTime) ||
               type == typeof(DateTimeOffset) ||
               type == typeof(TimeSpan) ||
               type == typeof(Guid) ||
               Nullable.GetUnderlyingType(type) != null;
    }

    private static string GetDisplayName(System.Reflection.PropertyInfo property)
    {
        var displayNameAttr = property.GetCustomAttributes(typeof(DisplayNameAttribute), false)
            .FirstOrDefault() as DisplayNameAttribute;

        return displayNameAttr?.DisplayName ?? property.Name;
    }
}
