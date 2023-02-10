using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class SynchronizeCandidateDataFromCMSParameter : BaseParameter
    {
        public SynchronizeCandidateDataModel Candidate { get; set; }
    }
}
