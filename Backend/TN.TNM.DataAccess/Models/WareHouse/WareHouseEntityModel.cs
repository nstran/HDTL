using System;

namespace TN.TNM.DataAccess.Models.WareHouse
{
    public class WareHouseEntityModel : BaseModel<Databases.Entities.Warehouse>
    {
        public Guid WarehouseId { get; set; }
        public string WarehouseCode { get; set; }
        public string WarehouseName { get; set; }
        public Guid? WarehouseParent { get; set; }
        public string WarehouseParentName { get; set; }
        public bool HasChild { get; set; }
        public bool CanRemove { get; set; }
        public bool CanAddChild { get; set; }
        public string WarehouseAddress { get; set; }
        public string WarehousePhone { get; set; }
        public Guid? Storagekeeper { get; set; }
        public string StoragekeeperName { get; set; }
        public string WarehouseDescription { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public Guid? TenantId { get; set; }
        public string WarehouseCodeName { get; set; }
        public WareHouseEntityModel() { }

        public WareHouseEntityModel(Databases.Entities.Warehouse entity) : base(entity)
        {
            //Xu ly sau khi lay tu DB len
            Mapper(entity, this);
        }

        public override Databases.Entities.Warehouse ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new Databases.Entities.Warehouse();
            Mapper(this, entity);
            return entity;
        }
    }
}
