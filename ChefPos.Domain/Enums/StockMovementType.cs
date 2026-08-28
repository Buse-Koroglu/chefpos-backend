namespace ChefPos.Domain.Enums;

public enum StockMovementType
{
    PURCHASE, // stock manager tarafından yapılan alımlarda değişen stok hareketi
    ORDER_SALE, // ürün satışı sonrası değişen stok hareketi
    MANUAL_DEDUCTION, // inventory staff tarafından elle girişi yapılan stok değişim hareketi
    PRODUCTION_DEDUCTION // üretim sonucu değişen stok hareketi
}
