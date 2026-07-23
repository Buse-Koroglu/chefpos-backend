using ChefPos.Domain.Entities;


public class OrderTest
{
    private const int OrderNumber = 1;
    private const string CustomerName = "Mock Customer";
    private const string ProductName = "Mock Product";
    [Fact]
    public void ShouldThrowExceptionWhenNameIsEmpty()
    {
        Assert.Throws<ArgumentException>(() =>
            Order.CreateByKiosk(OrderNumber, Guid.NewGuid(), ""));
    }
    
    [Fact]
    public void ShouldIncreaseQuantityWhenIteIsInTheOrder()
    {
        var order = Order.CreateByCashier(OrderNumber, Guid.NewGuid(), Guid.NewGuid(), CustomerName);
        var productId = Guid.NewGuid();
 
        order.AddItem(productId, 2, 10, ProductName);
        order.AddItem(productId, 3, 10, ProductName);
 
        Assert.Single(order.Items);
        Assert.Equal(5, order.Items.First().Quantity);
    }
    
    [Fact]
    public void ShouldCalculateCorrectPriceAfterAddingItem()
    {
        var order = Order.CreateByCashier(OrderNumber, Guid.NewGuid(), Guid.NewGuid(), CustomerName);
 
        order.AddItem(Guid.NewGuid(), 2, 15, ProductName);   
        order.AddItem(Guid.NewGuid(), 1, 40, ProductName);    
 
        Assert.Equal(70, order.TotalPrice);
    }
    
    [Fact]
    public void ShouldCalculateCorrectPriceAfterRemovingItem()
    {
        var order = Order.CreateByCashier(OrderNumber, Guid.NewGuid(), Guid.NewGuid(), CustomerName);
 
        order.AddItem(Guid.NewGuid(), 1, 90, ProductName);   
        order.AddItem(Guid.NewGuid(), 2, 10, ProductName);
        var itemId = order.Items.First().Id;
        order.RemoveItem(itemId);    
 
        Assert.Equal(20, order.TotalPrice);
    }
    
    [Fact]
    public void ShouldThrowAnExceptionWithEmptyOrder()
    {
        var order = Order.CreateByCashier(OrderNumber, Guid.NewGuid(), Guid.NewGuid(), CustomerName);
 
        Assert.Throws<InvalidOperationException>(() => order.Complete());
    }
 
    [Fact]
    public void ShouldThrowAnExceptionWithCompletedOrder()
    {
        var order = Order.CreateByCashier(1, Guid.NewGuid(), Guid.NewGuid(), CustomerName);
        order.AddItem(Guid.NewGuid(), 1, 10, ProductName);
        order.Complete();
 
        Assert.Throws<InvalidOperationException>(() => order.Complete());
    }
    
    
    [Fact]
    public void ShouldThrowAnExceptionWhenCompletedOrderIsCancelled()
    {
        var order = Order.CreateByCashier(1, Guid.NewGuid(), Guid.NewGuid(), CustomerName);
        order.AddItem(Guid.NewGuid(), 1, 10, ProductName);
        order.Complete();
 
        Assert.Throws<InvalidOperationException>(() => order.Cancel());
    }

    [Fact]
    public void ShouldThrowAnExceptionAddItemWithNegativeQuantity()
    {
        var order = Order.CreateByCashier(OrderNumber, Guid.NewGuid(), Guid.NewGuid(), CustomerName);
        Assert.Throws<ArgumentOutOfRangeException>(() => order.AddItem(Guid.NewGuid(), -1, 10, ProductName));
    }

    [Fact]
    public void ShouldThrowAnExceptionAddItemToCompletedOrder()
    {
        var order = Order.CreateByCashier(OrderNumber, Guid.NewGuid(), Guid.NewGuid(), CustomerName);
        order.AddItem(Guid.NewGuid(), 1, 30, ProductName);
        order.Complete();
        Assert.Throws<InvalidOperationException>(() => order.AddItem(Guid.NewGuid(), 1, 10, ProductName));
    }

    [Fact]
    public void ShouldThrowAnExceptionMarkAsPaidCalledMoreThanOnce()
    {
        var order = Order.CreateByCashier(OrderNumber, Guid.NewGuid(), Guid.NewGuid(), CustomerName);
        order.AddItem(Guid.NewGuid(), 1, 10, ProductName);
        order.MarkAsPaid();

        Assert.Throws<InvalidOperationException>(() => order.MarkAsPaid());
    }


}