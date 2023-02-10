using System.Collections.Generic;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Lead;

namespace TN.TNM.DataAccess.Messages.Results.Lead
{
    public class ImportLeadResult:BaseResult
    {
        public List<LeadEntityModel> lstcontactLeadDuplicate { get; set; }
        public List<ContactEntityModel> lstcontactContactDuplicate { get; set; }
        public List<ContactEntityModel> lstcontactContact_CON_Duplicate { get; set; }
        public List<ContactEntityModel> lstcontactCustomerDuplicate { get; set; }
    }
}
