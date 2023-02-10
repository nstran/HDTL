using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class GetMasterDateImportEmployeeParameter : BaseParameter
    {
        public int Type { get; set; }
        public List<Guid> ListEmpId { get; set; }
    } 
}
