using System.Globalization;
using ChefPos.Application.Common.Export;
using ChefPos.Application.Common.Interfaces;
using ClosedXML.Excel;

namespace ChefPos.Infrastructure.Export;

public class ExcelExportService : IExcelExportService
{
    public byte[] Generate<T>(IEnumerable<T> rows, IReadOnlyList<ExportColumn<T>> columns, string sheetName)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add(sheetName);

        for (var col = 0; col < columns.Count; col++)
        {
            var cell = worksheet.Cell(1, col + 1);
            cell.Value = columns[col].Header;
            cell.Style.Font.Bold = true;
        }

        var rowIndex = 2;
        foreach (var row in rows)
        {
            for (var col = 0; col < columns.Count; col++)
            {
                worksheet.Cell(rowIndex, col + 1).Value = FormatValue(columns[col].Selector(row));
            }
            rowIndex++;
        }

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private static XLCellValue FormatValue(object? value)
    {
        return value switch
        {
            null => Blank.Value,
            bool b => b ? "Evet" : "Hayır",
            DateTime dt => dt.ToString("dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture),
            decimal dec => dec,
            int i => i,
            double d => d,
            _ => value.ToString() ?? string.Empty,
        };
    }
}
