using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class DeleteThongTinGiaDinhByIdParameter : BaseParameter
    {
        public Guid ContactId { get; set; }
    }
}
