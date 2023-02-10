using System;

namespace TN.TNM.DataAccess.Messages.Parameters.Lead
{
    public class EditLeadStatusByIdParameter : BaseParameter
    {
        public Guid LeadId { get; set; }
        public string LeadStatusCode { get; set; }
    }
}
