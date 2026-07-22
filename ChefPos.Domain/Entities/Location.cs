using ChefPos.Domain.Common;

namespace ChefPos.Domain.Entities;

public class Location : BaseEntity
{
    public string Name { get; private set; } 
    public ICollection<Order> Orders { get; private set; } = new List<Order>();
    public ICollection<Product> Products { get; private set; } = new List<Product>();
    public ICollection<User> Users { get; private set; } = new List<User>();
    public ICollection<Category> Categories { get; private set; } = new List<Category>();
}