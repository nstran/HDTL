using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Salary;

namespace TN.TNM.DataAccess.Messages.Parameters.Salary
{
    public class UpdateDieuKienTroCapKhacParameter : BaseParameter
    {
        public Guid EmployeeId { get; set; }
        public LuongCtDieuKienTroCapKhacModel LuongCtDieuKienTroCapKhac { get; set; }
    }
}
