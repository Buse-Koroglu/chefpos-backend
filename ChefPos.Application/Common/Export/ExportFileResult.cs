namespace ChefPos.Application.Common.Export;

public record ExportFileResult(byte[] Content, string FileName)
{
    public const string ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
}
