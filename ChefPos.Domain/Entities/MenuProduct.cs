using ChefPos.Domain.Common;

namespace ChefPos.Domain.Entities;

public class MenuProduct : BaseEntity
{
    public Guid MenuId { get; private set; }
    public Menu Menu { get; private set; } = null!;
    public Guid ProductId { get; private set; }
    public Product Product { get; private set; } = null!;
    public int DisplayOrder { get; private set; }

    private MenuProduct() { }

    internal MenuProduct(Guid menuId, Guid productId, int displayOrder)
    {
        MenuId = menuId;
        ProductId = productId;
        DisplayOrder = displayOrder;
    }
}