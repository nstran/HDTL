using System;

namespace TN.TNM.DataAccess.Models.PayableInvoice
{
    public class BankPayableInvoiceMappingEntityModel : BaseModel<DataAccess.Databases.Entities.BankPayableInvoiceMapping>
    {
        public Guid BankPayableInvoiceMappingId { get; set; }
        public Guid BankPayableInvoiceId { get; set; }
        public Guid? ObjectId { get; set; }
        public short? ReferenceType { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public BankPayableInvoiceMappingEntityModel() { }

        public BankPayableInvoiceMappingEntityModel(Databases.Entities.BankPayableInvoiceMapping model)
        {
            Mapper(model, this);
        }

        public override DataAccess.Databases.Entities.BankPayableInvoiceMapping ToEntity()
        {
            var entity = new DataAccess.Databases.Entities.BankPayableInvoiceMapping();
            Mapper(this, entity);
            return entity;
        }
    }
}
