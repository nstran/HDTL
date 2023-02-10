using System;

namespace TN.TNM.DataAccess.Models.PayableInvoice
{
    public class PayableInvoiceMappingEntityModel : BaseModel<DataAccess.Databases.Entities.PayableInvoiceMapping>
    {
        public Guid? PayableInvoiceMappingId { get; set; }
        public Guid? PayableInvoiceId { get; set; }
        public Guid? ObjectId { get; set; }
        public short? ReferenceType { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public PayableInvoiceMappingEntityModel()
        {

        }

        public PayableInvoiceMappingEntityModel(Databases.Entities.PayableInvoiceMapping model)
        {
            Mapper(model, this);
        }

        public override DataAccess.Databases.Entities.PayableInvoiceMapping ToEntity()
        {
            var entity = new DataAccess.Databases.Entities.PayableInvoiceMapping();
            Mapper(this, entity);
            return entity;
        }
    }
}
