using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Lead;

namespace TN.TNM.DataAccess.Messages.Parameters.Lead
{
    public class EditLeadByIdParameter : BaseParameter
    {
        public LeadEntityModel Lead { get; set; }
        public ContactEntityModel Contact { get; set; }
        public List<Guid?> ListInterestedId { get; set; }
        public List<ContactEntityModel> ListContact { get; set; }
        public List<DataAccess.Models.Lead.LeadDetailModel> ListLeadDetail { get; set; }
        public List<DataAccess.Models.Document.LinkOfDocumentEntityModel> ListLinkOfDocument { get; set; }
        public List<Guid?> ListDocumentIdNeedRemove { get; set; }
        public bool? IsGetNoti { get; set; }
    }
}
