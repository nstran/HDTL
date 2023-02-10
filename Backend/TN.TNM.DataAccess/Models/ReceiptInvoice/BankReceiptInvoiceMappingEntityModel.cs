using System;

namespace TN.TNM.DataAccess.Models.ReceiptInvoice
{
    public class BankReceiptInvoiceMappingEntityModel : BaseModel<Databases.Entities.BankReceiptInvoiceMapping>
    {
        public Guid BankReceiptInvoiceMappingId { get; set; }
        public Guid BankReceiptInvoiceId { get; set; }
        public Guid? ObjectId { get; set; }
        public short? ReferenceType { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public BankReceiptInvoiceMappingEntityModel()
        {
        }

        public BankReceiptInvoiceMappingEntityModel(Databases.Entities.BankReceiptInvoiceMapping entity)
        {
            Mapper(entity, this);
            //Xu ly sau khi lay tu DB len
        }

        public override Databases.Entities.BankReceiptInvoiceMapping ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new Databases.Entities.BankReceiptInvoiceMapping();
            Mapper(this, entity);
            return entity;
        }
    }
}
