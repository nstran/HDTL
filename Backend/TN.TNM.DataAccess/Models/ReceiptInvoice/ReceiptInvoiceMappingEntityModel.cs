using System;

namespace TN.TNM.DataAccess.Models.ReceiptInvoice
{
    public class ReceiptInvoiceMappingEntityModel : BaseModel<DataAccess.Databases.Entities.ReceiptInvoiceMapping>
    {
        public Guid? ReceiptInvoiceMappingId { get; set; }
        public Guid? ReceiptInvoiceId { get; set; }
        public Guid? ObjectId { get; set; }
        public short? ReferenceType { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public ReceiptInvoiceMappingEntityModel()
        {

        }

        public ReceiptInvoiceMappingEntityModel(Databases.Entities.ReceiptInvoiceMapping model)
        {
            Mapper(model, this);
        }

        public override DataAccess.Databases.Entities.ReceiptInvoiceMapping ToEntity()
        {
            var entity = new DataAccess.Databases.Entities.ReceiptInvoiceMapping();
            Mapper(this, entity);
            return entity;
        }
    }
}
