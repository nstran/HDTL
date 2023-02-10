using System.Collections.Generic;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Customer;

namespace TN.TNM.DataAccess.Messages.Results.Customer
{
    public class ImportCustomerResult : BaseResult
    {
        public List<CustomerEntityModel> lstcontactCustomerDuplicate { get; set; }
        public List<ContactEntityModel> lstcontactContactDuplicate { get; set; }
        public List<ContactEntityModel> lstcontactContact_CON_Duplicate { get; set; }
        public bool isDupblicateInFile { get; set; }
    }
}
