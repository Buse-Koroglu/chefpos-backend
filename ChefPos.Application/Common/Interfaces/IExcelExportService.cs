using ChefPos.Application.Common.Export;

namespace ChefPos.Application.Common.Interfaces;

public interface IExcelExportService
{ 
    byte[] Generate<T>(IEnumerable<T> rows, IReadOnlyList<ExportColumn<T>> columns, string sheetName);
}
