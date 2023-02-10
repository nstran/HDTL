using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class TakeListEmployeeByUserNameParameter : BaseParameter
    {
        public string FilterText { get; set; }
    }
}
