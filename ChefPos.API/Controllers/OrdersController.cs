using ChefPos.Application.Orders.Commands;
using ChefPos.Application.Orders.Commands.CompleteOrder;
using ChefPos.Application.Orders.Commands.CreateKioskOrder;
using ChefPos.Application.Orders.Commands.MakePaidOrder;
using ChefPos.Application.Orders.DTOs;
using ChefPos.Application.Orders.Queries.GetOrderById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ChefPos.API.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult> CreateOrder(CreateOrderCommand command,CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
    
    [HttpPost("kiosk")]
    public async Task<ActionResult> CreateKioskOrder(CreateKioskOrderCommand command,CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetOrderById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetOrderByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id}/complete")]
    public async Task<ActionResult> CompleteOrder(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CompleteOrderCommand(id), cancellationToken);
        return Ok(result);
    }
    
    [HttpPost("{id}/paid")]
    public async Task<ActionResult> MakePaidOrder(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new MakePaidOrderCommand(id), cancellationToken);
        return Ok(result);
    }
        
    

}