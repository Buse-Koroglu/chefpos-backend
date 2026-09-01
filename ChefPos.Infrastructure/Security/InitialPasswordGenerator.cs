using ChefPos.Application.Common.Interfaces;

namespace ChefPos.Infastructure.Security;

public class InitialPasswordGenerator : IInitialPasswordGenerator
{
    private static readonly System.Globalization.CultureInfo TurkishCulture = System.Globalization.CultureInfo.GetCultureInfo("tr-TR");
    public string Generate(string firstName, string personalId)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("Ad boş olamaz.", nameof(firstName));
        if (string.IsNullOrWhiteSpace(personalId) || personalId.Trim().Length < 5)
            throw new ArgumentException("Personel ID ilk 5 hanesi bulunmalı.", nameof(personalId));

        var formattedName = FormatName(firstName.Trim());
        var idPrefix = personalId.Trim()[..5];

        return $"{formattedName}{idPrefix}";
    }

    private static string FormatName(string name)
    {
        var words = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var sb = new System.Text.StringBuilder();

        foreach (var word in words)
        {
            var capitalized = char.ToUpper(word[0], TurkishCulture) + word[1..].ToLower(TurkishCulture);
            sb.Append(capitalized);
        }
        return sb.ToString();
    }
}