using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Product
{
    public class ProductVendorMappingEntityModel : BaseModel<DataAccess.Databases.Entities.ProductVendorMapping>
    {
        public Guid VendorId { get; set; }
        public Guid? MoneyUnitId { get; set; }
        public Guid ProductVendorMappingId { get; set; }
        public Guid ProductId { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool Active { get; set; }
        public Guid? TenantId { get; set; }
        public string VendorProductName { get; set; }
        public string VendorProductCode { get; set; }
        public decimal? MiniumQuantity { get; set; }
        public decimal? Price { get; set; }
        public Guid? UnitPriceId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string ProductName { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public string MoneyUnitName { get; set; }
        public string ProductUnitName { get; set; }
        public string ProductCode { get; set; }
        public int? OrderNumber { get; set; }
        public decimal? ExchangeRate { get; set; }
        public List<Guid?> ListSuggestedSupplierQuoteId { get; set; }

        public ProductVendorMappingEntityModel()
        {
        }

        public ProductVendorMappingEntityModel(DataAccess.Databases.Entities.ProductVendorMapping entity)
        {
            Mapper(entity, this);
            //Xu ly sau khi lay tu DB len
        }

        public override DataAccess.Databases.Entities.ProductVendorMapping ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new DataAccess.Databases.Entities.ProductVendorMapping();
            Mapper(this, entity);
            return entity;
        }
    }
}
