using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class UpdateCandidateStatusParameter : BaseParameter
    {
        public Guid CandidateId { get; set; }
        public int Status { get; set; }
    }

}
