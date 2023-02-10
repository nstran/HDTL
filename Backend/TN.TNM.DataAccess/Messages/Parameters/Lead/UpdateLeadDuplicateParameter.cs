using System.Collections.Generic;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Lead;

namespace TN.TNM.DataAccess.Messages.Parameters.Lead
{
    public class UpdateLeadDuplicateParameter : BaseParameter
    {
        public List<LeadEntityModel> lstcontactLeadDuplicate { get; set; }
        public List<ContactEntityModel> lstcontactContactDuplicate { get; set; }
    }

}
