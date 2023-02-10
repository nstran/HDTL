using System;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.ReceiptInvoice
{
    public class BankReceiptInvoiceEntityModel : BaseModel<BankReceiptInvoice>
    {
        public Guid BankReceiptInvoiceId { get; set; }
        public string BankReceiptInvoiceCode { get; set; }
        public string BankReceiptInvoiceDetail { get; set; }
        public decimal? BankReceiptInvoicePrice { get; set; }
        public Guid? BankReceiptInvoicePriceCurrency { get; set; }
        public decimal? BankReceiptInvoiceExchangeRate { get; set; }
        public Guid? BankReceiptInvoiceReason { get; set; }
        public string BankReceiptInvoiceNote { get; set; }
        public Guid? BankReceiptInvoiceBankAccountId { get; set; }
        public decimal? BankReceiptInvoiceAmount { get; set; }
        public string BankReceiptInvoiceAmountText { get; set; }
        public DateTime BankReceiptInvoicePaidDate { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? StatusId { get; set; }
        public bool? Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string BankReceiptInvoiceReasonText { get; set; }
        public string CreatedByName { get; set; }
        public string AvatarUrl { get; set; }
        public Guid? ObjectId { get; set; }
        public string ObjectName { get; set; }
        public string StatusName { get; set; }
        public string BackgroundColorForStatus { get; set; }
        public DateTime? VouchersDate { get; set; }
        public bool? IsSendMail { get; set; }

        public BankReceiptInvoiceEntityModel()
        {
        }

        public BankReceiptInvoiceEntityModel(DataAccess.Databases.Entities.BankReceiptInvoice entity)
        {
            Mapper(entity, this);
            //Xu ly sau khi lay tu DB len
        }

        public override DataAccess.Databases.Entities.BankReceiptInvoice ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new DataAccess.Databases.Entities.BankReceiptInvoice();
            Mapper(this, entity);
            return entity;
        }
    }
}
