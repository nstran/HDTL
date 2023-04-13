using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class TakeListEvaluateParameter : BaseParameter
    {
        public Guid? EmployeeId { get; set; }
        public Guid? CustomerId { get; set; }
        public Guid? OrderId { get; set; }
    }
}
