using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Orders.DTOs;
using ChefPos.Application.Orders.Queries.GetWeeklyReveune;
using MediatR;

public class GetWeeklyRevenueQueryHandler : IRequestHandler<GetWeeklyRevenueQuery, WeeklyRevenueResponseDto>
{
    private static readonly string[] DayNames = { "Pazartesi", "Salı", "Çarşamba", "Perşembe", "Cuma" };

    private readonly IOrderRepository _orderRepository;

    public GetWeeklyRevenueQueryHandler(IOrderRepository orderRepository) => _orderRepository = orderRepository;

    public async Task<WeeklyRevenueResponseDto> Handle(GetWeeklyRevenueQuery request, CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;
        var diff = ((int)today.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
        var monday = today.AddDays(-diff);
        var saturdayExclusive = monday.AddDays(5);

        var dailyProfits = await _orderRepository.GetDailyProfitAsync(request.LocationId, monday, saturdayExclusive, cancellationToken);
        var profitByDate = dailyProfits.ToDictionary(x => x.Date, x => x.Profit);

        var days = new List<DailyRevenueDto>();
        for (var i = 0; i < 5; i++)
        {
            var date = monday.AddDays(i);
            days.Add(new DailyRevenueDto
            {
                Date = date,
                DayName = DayNames[i],
                Profit = profitByDate.TryGetValue(date, out var profit) ? profit : 0m
            });
        }

        return new WeeklyRevenueResponseDto { Days = days };
    }
}