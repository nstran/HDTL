using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Customer;
using TN.TNM.DataAccess.Models.PayableInvoice;

namespace TN.TNM.DataAccess.Messages.Results.PayableInvoice
{
    public class GetMasterDataPayableInvoiceResult:BaseResult
    {
        public List<CategoryEntityModel> ReasonOfPaymentList { get; set; }
        public List<CategoryEntityModel> TypesOfPaymentList { get; set; }
        public List<CategoryEntityModel> StatusOfPaymentList { get; set; }
        public List<CategoryEntityModel> UnitMoneyList { get; set; }
        public List<OrganizationEntityModel> OrganizationList { get; set; }
        public List<CustomerEntityModel> CustomerList { get; set; }
        public PayableInvoiceEntityModel PayableInvoice { get; set; }
    }
}
