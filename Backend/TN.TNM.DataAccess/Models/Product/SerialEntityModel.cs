using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Models.Product
{
    public class SerialEntityModel: BaseModel<Databases.Entities.Serial>
    {
        public Guid? SerialId { get; set; }
        public string SerialCode { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? StatusId { get; set; }
        public Guid? WarehouseId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public SerialEntityModel() { }

        public SerialEntityModel(Databases.Entities.Serial entity) : base(entity)
        {
            //Xu ly sau khi lay tu DB len
        }

        public override Databases.Entities.Serial ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new Databases.Entities.Serial();
            Mapper(this, entity);
            return entity;
        }
    }
}
