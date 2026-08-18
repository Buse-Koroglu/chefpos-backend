using ChefPos.Application.Common.Behaviors;
using ChefPos.Application.Common.Exceptions;
using ChefPos.Application.Common.Interfaces;
using ChefPos.Application.Tables.DTOs;
using ChefPos.Domain.Entities;
using MediatR;

namespace ChefPos.Application.Tables.Commands.CreateTable;

public class CreateTableCommandHandler : IRequestHandler<CreateTableCommand, TableResponseDto>
{
    private readonly ITableRepository _tableRepository;
    private readonly ILocationRepository _locationRepository;

    public CreateTableCommandHandler(ITableRepository tableRepository, ILocationRepository locationRepository)
    {
        _tableRepository = tableRepository;
        _locationRepository = locationRepository;
    }

    public async Task<TableResponseDto> Handle(CreateTableCommand request, CancellationToken cancellationToken)
    {
        await _locationRepository.GetByIdAsync(request.LocationId, cancellationToken)
            .OrThrowNotFoundAsync($"Yerleşke bulunamadı: {request.LocationId}");

        if (await _tableRepository.ExistsByNumberAsync(request.LocationId, request.TableNumber, null, cancellationToken))
        {
            throw new ValidationException($"Bu yerleşkede {request.TableNumber} numaralı masa zaten mevcut.");
        }

        var table = new Table(request.LocationId, request.TableNumber);

        await _tableRepository.AddAsync(table, cancellationToken);
        await _tableRepository.SaveAllChangesAsync(cancellationToken);

        return TableResponseDto.FromEntity(table);
    }
}
