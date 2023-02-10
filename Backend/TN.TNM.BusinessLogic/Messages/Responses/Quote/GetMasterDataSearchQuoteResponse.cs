using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.BusinessLogic.Messages.Responses.Quote
{
    public class GetMasterDataSearchQuoteResponse : BaseResponse
    {
        public List<EmployeeEntityModel> ListEmp { get; set; }
    }
}
