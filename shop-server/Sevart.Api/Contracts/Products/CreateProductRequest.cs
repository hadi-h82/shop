namespace Sevart.Api.Contracts.Products;

public class CreateProductRequest
{
    public int CategoryId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Slug { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int DisplayOrder { get; set; }

    public List<CreateProductOptionRequest> Options { get; set; } = [];
}


public class CreateProductOptionRequest
{
    // اشاره به ویژگی عمومی:
    // مثلا "اندازه"، "رنگ"، "مدل لبه"
    public int ProductOptionDefinitionId { get; set; }

    // آیا انتخاب این ویژگی برای این محصول اجباری است؟
    public bool IsRequired { get; set; }

    public int DisplayOrder { get; set; }

    public List<CreateProductOptionValueRequest> Values { get; set; } = [];
}


public class CreateProductOptionValueRequest
{
    // چیزی که کاربر می‌بیند
    // مثلا: "۲۵ سانتی‌متر"
    public string Label { get; set; } = string.Empty;

    // مقدار داخلی
    // مثلا: "25"
    public string Value { get; set; } = string.Empty;

    // اختلاف قیمت نسبت به قیمت پایه محصول
    public decimal PriceAdjustment { get; set; }

    // برای ویژگی‌هایی مثل رنگ
    // مثلا: "#d4af37"
    public string? ColorCode { get; set; }

    public int DisplayOrder { get; set; }
}