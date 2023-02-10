using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.WareHouse
{
    public class InventoryReceivingVoucherMappingEntityModel : BaseModel<InventoryReceivingVoucherMapping>
    {
        public Guid? InventoryReceivingVoucherMappingId { get; set; }
        public Guid? InventoryReceivingVoucherId { get; set; }
        public Guid? ObjectId { get; set; }
        public Guid ProductId { get; set; }
        public decimal? QuantityRequest { get; set; }
        public decimal? QuantityActual { get; set; }
        public decimal? QuantitySerial { get; set; }
        public decimal? PriceProduct { get; set; }
        public Guid? WarehouseId { get; set; }
        public string Description { get; set; }
        public bool? Active { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public Guid? UnitId { get; set; }
        public decimal? ExchangeRate { get; set; }
        public decimal? Vat { get; set; }
        public bool? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
        public Guid? CurrencyUnit { get; set; }
        public decimal? QuantityReservation { get; set; }
        public bool? PriceAverage { get; set; }
        public Guid? ObjectDetailId { get; set; }

        public InventoryReceivingVoucherMappingEntityModel()
        {
        }

        public InventoryReceivingVoucherMappingEntityModel(DataAccess.Databases.Entities.InventoryReceivingVoucherMapping entity)
        {
            Mapper(entity, this);
            //Xu ly sau khi lay tu DB len
        }

        public override DataAccess.Databases.Entities.InventoryReceivingVoucherMapping ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new DataAccess.Databases.Entities.InventoryReceivingVoucherMapping();
            Mapper(this, entity);
            return entity;
        }
    }
}
