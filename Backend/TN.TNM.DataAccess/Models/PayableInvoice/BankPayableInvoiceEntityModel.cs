using System;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Models.PayableInvoice
{
    public class BankPayableInvoiceEntityModel : BaseModel<DataAccess.Databases.Entities.BankPayableInvoice>
    {
        public Guid BankPayableInvoiceId { get; set; }
        public string BankPayableInvoiceCode { get; set; }
        public string BankPayableInvoiceDetail { get; set; }
        public decimal? BankPayableInvoicePrice { get; set; }
        public Guid? BankPayableInvoicePriceCurrency { get; set; }
        public decimal? BankPayableInvoiceExchangeRate { get; set; }
        public Guid? BankPayableInvoiceReason { get; set; }
        public string BankPayableInvoiceNote { get; set; }
        public Guid? BankPayableInvoiceBankAccountId { get; set; }
        public decimal? BankPayableInvoiceAmount { get; set; }
        public string BankPayableInvoiceAmountText { get; set; }
        public DateTime BankPayableInvoicePaidDate { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? StatusId { get; set; }
        public string ReceiveAccountNumber { get; set; }
        public string ReceiveAccountName { get; set; }
        public string ReceiveBankName { get; set; }
        public string ReceiveBranchName { get; set; }
        public bool? Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string BankPayableInvoiceReasonText { get; set; }
        public string CreatedByName { get; set; }
        public string AvatarUrl { get; set; }
        public string ObjectName { get; set; }
        public string StatusName { get; set; }
        public Guid? ObjectId { get; set; }
        public string BackgroundColorForStatus { get; set; }

        public BankPayableInvoiceEntityModel() { }

        public BankPayableInvoiceEntityModel(Databases.Entities.BankPayableInvoice model)
        {
            Mapper(model, this);
        }

        public override DataAccess.Databases.Entities.BankPayableInvoice ToEntity()
        {
            var entity = new DataAccess.Databases.Entities.BankPayableInvoice();
            Mapper(this, entity);
            return entity;
        }
    }
}
