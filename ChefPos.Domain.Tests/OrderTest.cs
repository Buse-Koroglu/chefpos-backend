using ChefPos.Domain.Entities;
using ChefPos.Domain.Enums;

public class OrderTest
{
    private const string CustomerName = "Mock Customer";
    private const string ProductName = "Mock Product";
    [Fact]
    public void ShouldThrowExceptionWhenNameIsEmpty()
    {
        Assert.Throws<ArgumentException>(() =>
            Order.CreateByKiosk(Guid.NewGuid(), ""));
    }

    [Fact]
    public void ShouldSetOrderTypeForCashier()
    {
        var order = Order.CreateByCashier(Guid.NewGuid(), Guid.NewGuid(), CustomerName);

        Assert.Equal(OrderType.CASHIER, order.OrderType);
    }

    [Fact]
    public void ShouldSetOrderTypeForWaiter()
    {
        var locationId = Guid.NewGuid();
        var table = new Table(locationId, 1);

        var order = Order.CreateByWaiter(locationId, Guid.NewGuid(), CustomerName, table);

        Assert.Equal(OrderType.WAITER, order.OrderType);
        Assert.Equal(table.Id, order.TableId);
    }

    [Fact]
    public void ShouldThrowExceptionWhenWaiterTableBelongsToDifferentLocation()
    {
        var table = new Table(Guid.NewGuid(), 1);

        Assert.Throws<ArgumentException>(() =>
            Order.CreateByWaiter(Guid.NewGuid(), Guid.NewGuid(), CustomerName, table));
    }

    [Fact]
    public void ShouldThrowExceptionWhenWaiterTableIsInactive()
    {
        var locationId = Guid.NewGuid();
        var table = new Table(locationId, 1);
        table.Deactivate();

        Assert.Throws<ArgumentException>(() =>
            Order.CreateByWaiter(locationId, Guid.NewGuid(), CustomerName, table));
    }


    [Fact]
    public void ShouldIncreaseQuantityWhenIteIsInTheOrder()
    {
        var order = Order.CreateByCashier(Guid.NewGuid(), Guid.NewGuid(), CustomerName);
        var productId = Guid.NewGuid();
 
        order.AddItem(productId, 2, 10, ProductName);
        order.AddItem(productId, 3, 10, ProductName);
 
        Assert.Single(order.Items);
        Assert.Equal(5, order.Items.First().Quantity);
    }
    
    [Fact]
    public void ShouldCalculateCorrectPriceAfterAddingItem()
    {
        var order = Order.CreateByCashier(Guid.NewGuid(), Guid.NewGuid(), CustomerName);
 
        order.AddItem(Guid.NewGuid(), 2, 15, ProductName);   
        order.AddItem(Guid.NewGuid(), 1, 40, ProductName);    
 
        Assert.Equal(70, order.TotalPrice);
    }
    
    [Fact]
    public void ShouldCalculateCorrectPriceAfterRemovingItem()
    {
        var order = Order.CreateByCashier(Guid.NewGuid(), Guid.NewGuid(), CustomerName);
 
        order.AddItem(Guid.NewGuid(), 1, 90, ProductName);   
        order.AddItem(Guid.NewGuid(), 2, 10, ProductName);
        var itemId = order.Items.First().Id;
        order.RemoveItem(itemId);    
 
        Assert.Equal(20, order.TotalPrice);
    }
    
    [Fact]
    public void ShouldThrowAnExceptionWithEmptyOrder()
    {
        var order = Order.CreateByCashier(Guid.NewGuid(), Guid.NewGuid(), CustomerName);
 
        Assert.Throws<InvalidOperationException>(() => order.Complete());
    }
 
    [Fact]
    public void ShouldThrowAnExceptionWithCompletedOrder()
    {
        var order = Order.CreateByCashier(Guid.NewGuid(), Guid.NewGuid(), CustomerName);
        order.AddItem(Guid.NewGuid(), 1, 10, ProductName);
        order.Complete();
 
        Assert.Throws<InvalidOperationException>(() => order.Complete());
    }
    
    
    [Fact]
    public void ShouldThrowAnExceptionWhenCompletedOrderIsCancelled()
    {
        var order = Order.CreateByCashier(Guid.NewGuid(), Guid.NewGuid(), CustomerName);
        order.AddItem(Guid.NewGuid(), 1, 10, ProductName);
        order.Complete();
 
        Assert.Throws<InvalidOperationException>(() => order.Cancel());
    }

    [Fact]
    public void ShouldThrowAnExceptionAddItemWithNegativeQuantity()
    {
        var order = Order.CreateByCashier(Guid.NewGuid(), Guid.NewGuid(), CustomerName);
        Assert.Throws<ArgumentOutOfRangeException>(() => order.AddItem(Guid.NewGuid(), -1, 10, ProductName));
    }

    [Fact]
    public void ShouldThrowAnExceptionAddItemToCompletedOrder()
    {
        var order = Order.CreateByCashier(Guid.NewGuid(), Guid.NewGuid(), CustomerName);
        order.AddItem(Guid.NewGuid(), 1, 30, ProductName);
        order.Complete();
        Assert.Throws<InvalidOperationException>(() => order.AddItem(Guid.NewGuid(), 1, 10, ProductName));
    }

    [Fact]
    public void ShouldThrowAnExceptionMarkAsPaidCalledMoreThanOnce()
    {
        var order = Order.CreateByCashier(Guid.NewGuid(), Guid.NewGuid(), CustomerName);
        order.AddItem(Guid.NewGuid(), 1, 10, ProductName);
        order.MarkAsPaid();

        Assert.Throws<InvalidOperationException>(() => order.MarkAsPaid());
    }

    [Fact]
    public void ShouldThrowAnExceptionAddItemToPaidOrder()
    {
        var order = Order.CreateByCashier(Guid.NewGuid(), Guid.NewGuid(), CustomerName);
        order.AddItem(Guid.NewGuid(), 1, 10, ProductName);
        order.MarkAsPaid();

        Assert.Throws<InvalidOperationException>(() => order.AddItem(Guid.NewGuid(), 1, 10, ProductName));
    }

    [Fact]
    public void ShouldThrowAnExceptionRemoveItemFromPaidOrder()
    {
        var order = Order.CreateByCashier(Guid.NewGuid(), Guid.NewGuid(), CustomerName);
        order.AddItem(Guid.NewGuid(), 1, 10, ProductName);
        var itemId = order.Items.First().Id;
        order.MarkAsPaid();

        Assert.Throws<InvalidOperationException>(() => order.RemoveItem(itemId));
    }

    [Fact]
    public void ShouldThrowAnExceptionDecreaseQuantityOnPaidOrder()
    {
        var order = Order.CreateByCashier(Guid.NewGuid(), Guid.NewGuid(), CustomerName);
        order.AddItem(Guid.NewGuid(), 2, 10, ProductName);
        var itemId = order.Items.First().Id;
        order.MarkAsPaid();

        Assert.Throws<InvalidOperationException>(() => order.DecreaseQuantity(itemId, 1));
    }
}