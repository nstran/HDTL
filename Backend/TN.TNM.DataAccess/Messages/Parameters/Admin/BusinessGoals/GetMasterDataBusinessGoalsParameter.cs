using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Admin.BusinessGoals
{
    public class GetMasterDataBusinessGoalsParameter : BaseParameter
    {
        public Guid? OrganizationId { get; set; }
        public string Year { get; set; }
        public bool Type { get; set; }
    }
}
