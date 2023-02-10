using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class DeleteCandidateDetailInforParameter : BaseParameter
    {
        public Guid OverviewCandidateId { get; set; }
    }

}
