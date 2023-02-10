using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class CustomerOrderDetail
    {
        public CustomerOrderDetail()
        {
            OrderProductDetailProductAttributeValue = new HashSet<OrderProductDetailProductAttributeValue>();
        }

        public Guid OrderDetailId { get; set; }
        public Guid? VendorId { get; set; }
        public Guid OrderId { get; set; }
        public Guid? ProductId { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public Guid? CurrencyUnit { get; set; }
        public decimal? ExchangeRate { get; set; }
        public decimal? Vat { get; set; }
        public bool? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
        public string Description { get; set; }
        public short? OrderDetailType { get; set; }
        public Guid? UnitId { get; set; }
        public string IncurredUnit { get; set; }
        public bool? Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? Guarantee { get; set; }
        public int? GuaranteeTime { get; set; }
        public DateTime? GuaranteeDatetime { get; set; }
        public Guid? TenantId { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string ProductName { get; set; }
        public Guid? WarehouseId { get; set; }
        public decimal? PriceInitial { get; set; }
        public bool? IsPriceInitial { get; set; }
        public int? WarrantyPeriod { get; set; }
        public int? ActualInventory { get; set; }
        public int? BusinessInventory { get; set; }
        public int? OrderNumber { get; set; }
        public string ProductColor { get; set; }
        public string ProductColorCode { get; set; }
        public double? ProductThickness { get; set; }
        public double? ProductLength { get; set; }
        public double? ProductWidth { get; set; }
        public string TechniqueDescription { get; set; }
        public string ProductCode { get; set; }
        public string UnitName { get; set; }
        public double? TotalArea { get; set; }
        public int? Borehole { get; set; }
        public int? Hole { get; set; }
        public string ProductGroupCode { get; set; }
        public double? Grind { get; set; }
        public double? Stt { get; set; }
        public decimal UnitLaborPrice { get; set; }
        public int UnitLaborNumber { get; set; }
        public Guid? ProductCategoryId { get; set; }
        public string EmployeePhoneNumber { get; set; }
        public string UserReview { get; set; }
        public string ReviewName { get; set; }
        public string ReviewDetail { get; set; }
        public Guid? OptionId { get; set; }
        public Guid? PacketServiceId { get; set; }

        public CustomerOrder Order { get; set; }
        public ICollection<OrderProductDetailProductAttributeValue> OrderProductDetailProductAttributeValue { get; set; }
    }
}
