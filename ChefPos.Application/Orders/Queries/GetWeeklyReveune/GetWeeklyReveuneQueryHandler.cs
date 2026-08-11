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

        var dailyRevenues = await _orderRepository.GetDailyRevenueAsync(request.LocationId, monday, saturdayExclusive, cancellationToken);
        var revenueByDate = dailyRevenues.ToDictionary(x => x.Date, x => x.Revenue);

        var days = new List<DailyRevenueDto>();
        for (var i = 0; i < 5; i++)
        {
            var date = monday.AddDays(i);
            days.Add(new DailyRevenueDto
            {
                Date = date,
                DayName = DayNames[i],
                Revenue = revenueByDate.TryGetValue(date, out var revenue) ? revenue : 0m
            });
        }

        return new WeeklyRevenueResponseDto { Days = days };
    }
}