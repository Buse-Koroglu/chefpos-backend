using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Tables.DTOs;
using MediatR;

namespace ChefPos.Application.Tables.Commands.DeactivateTable;

public class DeactivateTableCommandHandler : IRequestHandler<DeactivateTableCommand, TableResponseDto>
{
    private readonly ITableRepository _tableRepository;

    public DeactivateTableCommandHandler(ITableRepository tableRepository)
    {
        _tableRepository = tableRepository;
    }

    public async Task<TableResponseDto> Handle(DeactivateTableCommand request, CancellationToken cancellationToken)
    {
        var table = await _tableRepository.GetByIdAsync(request.TableId, cancellationToken)
            .OrThrowNotFoundAsync($"Masa bulunamadı: {request.TableId}");

        table.Deactivate();

        await _tableRepository.SaveAllChangesAsync(cancellationToken);

        return TableResponseDto.FromEntity(table);
    }
}
