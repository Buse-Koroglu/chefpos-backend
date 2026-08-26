using ChefPos.Application.Orders.Commands;
using ChefPos.Application.Orders.Commands.AddOrderItem;
using ChefPos.Application.Orders.Commands.CancelOrder;
using ChefPos.Application.Orders.Commands.CompleteOrder;
using ChefPos.Application.Orders.Commands.CreateKioskOrder;
using ChefPos.Application.Orders.Commands.DecreaseOrderItem;
using ChefPos.Application.Orders.Commands.MakeKioskOrderPaid;
using ChefPos.Application.Orders.Commands.MakePaidOrder;
using ChefPos.Application.Orders.Commands.RemoveOrderItem;
using ChefPos.Application.Orders.DTOs;
using ChefPos.Application.Orders.Queries.GetKitchenOrders;
using ChefPos.Application.Orders.Queries.GetOrderById;
using ChefPos.Application.Orders.Queries.GetOrders;
using ChefPos.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChefPos.API.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize(Roles = "CASHIER,WAITER")]
    [HttpPost]
    public async Task<ActionResult> CreateOrder(CreateOrderCommand command,CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
    
    [AllowAnonymous]
    [HttpPost("kiosk")]
    public async Task<ActionResult> CreateKioskOrder(CreateKioskOrderCommand command,CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("kiosk/{id}/paid")]
    public async Task<ActionResult> MakeKioskOrderPaid(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new MakeKioskOrderPaidCommand(id), cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "CASHIER,WAITER,ADMIN")]
    [HttpGet("{id}")]
    public async Task<ActionResult> GetOrderById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetOrderByIdQuery(id), cancellationToken);
        return Ok(result);
    }
    
    [Authorize(Roles = "KITCHEN,ADMIN")]
    [HttpGet("kitchen")]
    public async Task<ActionResult> GetKitchenOrders([FromQuery] Guid locationId, [FromQuery] OrderStatus? status, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetKitchenOrdersQuery(locationId, status), cancellationToken);
        return Ok(result);
    }
    
    [Authorize(Roles = "CASHIER,WAITER,ADMIN,KITCHEN")]
    [HttpGet]
    public async Task<ActionResult> GetOrders(
        [FromQuery] Guid locationId,
        [FromQuery] OrderStatus? status,
        [FromQuery] OrderType? type,
        [FromQuery] PaymentStatus? paymentStatus,
        [FromQuery] string? searchTerm,
        [FromQuery] Guid? waiterId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetOrdersQuery(locationId, status, type, paymentStatus, searchTerm, waiterId, fromDate, toDate, pageNumber, pageSize),
            cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "CASHIER,KITCHEN")]
    [HttpPost("{id}/complete")]
    public async Task<ActionResult> CompleteOrder(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CompleteOrderCommand(id), cancellationToken);
        return Ok(result);
    }
    
    [Authorize(Roles = "CASHIER")]
    [HttpPost("{id}/paid")]
    public async Task<ActionResult> MakePaidOrder(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new MakePaidOrderCommand(id), cancellationToken);
        return Ok(result);
    }
    
    [Authorize(Roles = "CASHIER,WAITER")]
    [HttpPost("{id}/cancel")]
    public async Task<ActionResult> CancelOrder(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CancelOrderCommand(id), cancellationToken);
        return Ok(result);
    }
        
    [Authorize(Roles = "CASHIER,WAITER")]
    [HttpPost("{id}/items")]
    public async Task<ActionResult> AddOrderItem(Guid id, AddOrderItemRequest body, CancellationToken cancellationToken)
    {
        var command = new AddOrderItemCommand(id, body.Quantity, body.ProductId);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "CASHIER,WAITER")]
    [HttpDelete("{orderId}/items/{orderItemId}")]
    public async Task<ActionResult> RemoveOrderItem(Guid orderId, Guid orderItemId, CancellationToken cancellationToken)
    {
        var command = new RemoveOrderItemCommand(orderId, orderItemId);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
    
    
    [Authorize(Roles = "CASHIER,WAITER")]
    [HttpPatch("{id}/items/{itemId}/decrease")]
    public async Task<ActionResult> DecreaseItemQuantity(Guid id, Guid itemId,DecreaseOrderItemRequest request, CancellationToken cancellationToken)
    {
        var command = new DecreaseOrderItemCommand(id, itemId, request.Quantity);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

}