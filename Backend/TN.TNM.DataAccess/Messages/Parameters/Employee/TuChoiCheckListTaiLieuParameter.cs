using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class TuChoiCheckListTaiLieuParameter : BaseParameter
    {
        public Guid EmployeeId { get; set; }
        public string LyDo { get; set; }
    }
}
