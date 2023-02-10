using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.BankAccount;
using TN.TNM.DataAccess.Models.PayableInvoice;
using TN.TNM.DataAccess.Models.Vendor;

namespace TN.TNM.DataAccess.Messages.Results.PayableInvoice
{
    public class GetMasterDataBankPayableInvoiceResult : BaseResult
    {
        public List<CategoryEntityModel> ReasonOfPaymentList { get; set; }
        public List<BankAccountEntityModel> TypesOfPaymentList { get; set; }
        public List<CategoryEntityModel> StatusOfPaymentList { get; set; }
        public List<CategoryEntityModel> UnitMoneyList { get; set; }
        public List<OrganizationEntityModel> OrganizationList { get; set; }
        public List<VendorEntityModel> VendorList { get; set; }
        public BankPayableInvoiceEntityModel BankPayableInvoice { get; set; }
    }
}
