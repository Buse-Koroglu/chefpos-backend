using ChefPos.Domain.Common;

namespace ChefPos.Domain.Entities;

public class ProductItem : BaseEntity
{
    
    public Guid ProductId { get; private set; }
    public Product Product { get; private set; } = null!;
    
    public Guid IngredientId { get; private set; }
    public Ingredient Ingredient { get; private set; } = null!;
    public decimal QuantityPerServing { get; private set; }
    
    
    private  ProductItem()
    {}

    internal ProductItem(Guid productId, Guid ingredientId, decimal quantityPerServing)
    {
        
        if (quantityPerServing < 0)
        {
            throw new ArgumentOutOfRangeException( nameof(quantityPerServing),"Kullanılan miktar pozitif olmalı");
        } 
        
        ProductId = productId;
        IngredientId = ingredientId;
        QuantityPerServing = quantityPerServing;
    
    }
    

    internal void UpdateQuantity(decimal newQuantityPerServing)
    {
        if (newQuantityPerServing <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(newQuantityPerServing), "Kullanılan miktar pozitif olmalı.");
        }

        QuantityPerServing = newQuantityPerServing;
        Touch();
    }
}