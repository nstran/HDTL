using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Customer;

namespace TN.TNM.DataAccess.Messages.Results.ReceiptInvoice
{
    public class GetMasterDataReceiptInvoiceResult : BaseResult
    {
        public List<CategoryEntityModel> ListReason { get; set; }
        public List<CategoryEntityModel> TypesOfReceiptList { get; set; }
        public List<CategoryEntityModel> ListStatus { get; set; }
        public List<CategoryEntityModel> UnitMoneyList { get; set; }
        public List<OrganizationEntityModel> OrganizationList { get; set; }
        public List<CustomerEntityModel> CustomerList { get; set; }
    }
}
