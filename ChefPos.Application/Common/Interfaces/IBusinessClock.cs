namespace ChefPos.Application.Common.Interfaces;

public interface IBusinessClock
{
    DateTime Today { get; }

    DateTime GetBusinessDate(DateTime utcInstant);

    DateTime ToUtc(DateTime businessLocalDate);
}