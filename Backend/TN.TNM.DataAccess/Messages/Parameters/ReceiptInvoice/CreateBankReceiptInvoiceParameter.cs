using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.ReceiptInvoice;

namespace TN.TNM.DataAccess.Messages.Parameters.ReceiptInvoice
{
    public class CreateBankReceiptInvoiceParameter: BaseParameter
    {
        public BankReceiptInvoiceEntityModel BankReceiptInvoice { get; set; }
        public BankReceiptInvoiceMappingEntityModel BankReceiptInvoiceMapping { get; set; }
        public List<ReceiptHistoryEntityModel> ReceiptOrderHistory { get; set; }

        public Guid? OrderId { get; set; }
    }
}
