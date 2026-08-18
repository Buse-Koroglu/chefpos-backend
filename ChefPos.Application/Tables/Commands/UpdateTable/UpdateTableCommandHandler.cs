using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Tables.DTOs;
using MediatR;

namespace ChefPos.Application.Tables.Commands.UpdateTable;

public class UpdateTableCommandHandler : IRequestHandler<UpdateTableCommand, TableResponseDto>
{
    private readonly ITableRepository _tableRepository;

    public UpdateTableCommandHandler(ITableRepository tableRepository)
    {
        _tableRepository = tableRepository;
    }

    public async Task<TableResponseDto> Handle(UpdateTableCommand request, CancellationToken cancellationToken)
    {
        var table = await _tableRepository.GetByIdAsync(request.TableId, cancellationToken)
            .OrThrowNotFoundAsync($"Masa bulunamadı: {request.TableId}");

        if (await _tableRepository.ExistsByNumberAsync(table.LocationId, request.TableNumber, table.Id, cancellationToken))
        {
            throw new ValidationException($"Bu yerleşkede {request.TableNumber} numaralı masa zaten mevcut.");
        }

        table.UpdateTableNumber(request.TableNumber);

        await _tableRepository.SaveAllChangesAsync(cancellationToken);

        return TableResponseDto.FromEntity(table);
    }
}
