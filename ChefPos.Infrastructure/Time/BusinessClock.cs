using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Common.Settings;
using Microsoft.Extensions.Options;

namespace ChefPos.Infrastructure.Time;

public class BusinessClock : IBusinessClock
{
    private readonly TimeZoneInfo _timeZone;

    public BusinessClock(IOptions<BusinessSettings> settings)
    {
        _timeZone = TimeZoneInfo.FindSystemTimeZoneById(settings.Value.TimeZoneId);
    }

    public DateTime Today => GetBusinessDate(DateTime.UtcNow);

    public DateTime GetBusinessDate(DateTime utcInstant)
    {
        var utc = DateTime.SpecifyKind(utcInstant, DateTimeKind.Utc);
        return TimeZoneInfo.ConvertTimeFromUtc(utc, _timeZone).Date;
    }

    public DateTime ToUtc(DateTime businessLocalDate)
    {
        var local = DateTime.SpecifyKind(businessLocalDate.Date, DateTimeKind.Unspecified);
        return TimeZoneInfo.ConvertTimeToUtc(local, _timeZone);
    }
}
