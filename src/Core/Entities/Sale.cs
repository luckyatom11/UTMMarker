using utmMarker.Core.Enums;

namespace utmMarker.Core.Entities;

public sealed class Sale(
    int saleId, 
    string folio, 
    DateTime saleDate, 
    int totalItems, 
    decimal totalSale, 
    SaleStatus status)
{
    // Primary constructor fields are implicitly declared as private readonly fields
    // and are used to initialize properties.

    public int SaleID { get; set; } = saleId; // Changed from init to set
    public string Folio { get; init; } = folio;
    public DateTime SaleDate { get; init; } = saleDate;
    public SaleStatus Status { get; set; } = status;

    private readonly List<SaleDetail> _saleDetails = [];
    public IReadOnlyList<SaleDetail> SaleDetails => _saleDetails.AsReadOnly();

    public void AddSaleDetail(SaleDetail detail)
    {
        ArgumentNullException.ThrowIfNull(detail);
        _saleDetails.Add(detail);
    }
    
    // Changed from calculated properties to settable properties to align with DB schema
    public int TotalItems { get; set; } = totalItems;
    public decimal TotalSale { get; set; } = totalSale;

    // Add a parameterless constructor for ORM/Dapper mapping if needed (can be private)
    private Sale() : this(0, string.Empty, DateTime.MinValue, 0, 0, SaleStatus.Pending) { }
}
