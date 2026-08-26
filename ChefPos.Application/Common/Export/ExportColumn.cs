namespace ChefPos.Application.Common.Export;

public record ExportColumn<T>(string Header, Func<T, object?> Selector);
