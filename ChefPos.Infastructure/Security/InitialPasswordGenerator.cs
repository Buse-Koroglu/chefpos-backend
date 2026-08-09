using ChefPos.Application.Common.Interfaces;

namespace ChefPos.Infastructure.Security;

public class InitialPasswordGenerator : IInitialPasswordGenerator
{
    public string Generate(string firstName, string personalId)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("Ad boş olamaz.", nameof(firstName));
        if (string.IsNullOrWhiteSpace(personalId) || personalId.Trim().Length < 5)
            throw new ArgumentException("Personel ID en az 5 haneli olmalı.", nameof(personalId));

        var name = firstName.Trim();
        // örnek şifre => Personel İsim = Mehmet, Personel Id ilk 5 rakamı = 67324, Kullanıcı İlk Giriş Şifresi : Mehmet67324
        var formattedName = char.ToUpper(name[0], new System.Globalization.CultureInfo("tr-TR")) + name[1..];

        var idPrefix = personalId.Trim()[..5];

        return $"{formattedName}{idPrefix}";
    }
}