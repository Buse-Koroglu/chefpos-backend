using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Tables.DTOs;
using MediatR;

namespace ChefPos.Application.Tables.Commands.ActivateTable;

public class ActivateTableCommandHandler : IRequestHandler<ActivateTableCommand, TableResponseDto>
{
    private readonly ITableRepository _tableRepository;

    public ActivateTableCommandHandler(ITableRepository tableRepository)
    {
        _tableRepository = tableRepository;
    }

    public async Task<TableResponseDto> Handle(ActivateTableCommand request, CancellationToken cancellationToken)
    {
        var table = await _tableRepository.GetByIdAsync(request.TableId, cancellationToken)
            .OrThrowNotFoundAsync($"Masa bulunamadı: {request.TableId}");

        table.Activate();

        await _tableRepository.SaveAllChangesAsync(cancellationToken);

        return TableResponseDto.FromEntity(table);
    }
}
