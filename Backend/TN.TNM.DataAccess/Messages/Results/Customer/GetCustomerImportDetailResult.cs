using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.DataAccess.Messages.Results.Customer
{
    public class GetCustomerImportDetailResult:BaseResult
    {
        public List<CategoryEntityModel> ListCustomerGroup { get; set; }
        public List<string> ListCustomerCompanyCode { get; set; }
        public List<string> ListEmail { get; set; }
        public List<string> ListPhone { get; set; }
    } 
}
