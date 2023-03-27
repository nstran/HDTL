using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class DeleteEmployeeParameter : BaseParameter
    {
        public List<Guid> ListEmployeeId { get; set; }
    }
}
