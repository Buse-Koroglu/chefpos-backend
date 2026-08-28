namespace ChefPos.Application.Common.Export;

public static class ExportLimits
{
    public const int MaxRows = 10_000;

    public static string ExceededMessage =>
        $"En fazla {MaxRows:N0} kayıt aynı anda dışarı aktarılabilir. Lütfen filtre uygulayıp tekrar deneyin.";
}
