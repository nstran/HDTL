using System.Collections.Generic;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.ReceiptInvoice
{
    public class GetMasterDataSearchReceiptInvoiceResult : BaseResult
    {
        public List<CategoryEntityModel> ListReason { get; set; }
        public List<CategoryEntityModel> ListStatus { get; set; }
        public List<EmployeeEntityModel> ListEmployee { get; set; }
        
    }
}
