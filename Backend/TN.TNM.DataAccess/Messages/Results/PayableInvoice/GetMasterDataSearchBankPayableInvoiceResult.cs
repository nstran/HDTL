using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;


namespace TN.TNM.DataAccess.Messages.Results.PayableInvoice
{
    public class GetMasterDataSearchBankPayableInvoiceResult : BaseResult
    {
        public List<CategoryEntityModel> ReasonOfPaymentList { get; set; }
        public List<CategoryEntityModel> StatusOfPaymentList { get; set; }  
        public List<EmployeeEntityModel> EmployeeList { get; set; }      
    }
}
