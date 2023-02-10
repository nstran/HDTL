using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Salary;

namespace TN.TNM.DataAccess.Messages.Parameters.Salary
{
    public class ImportTroCapKhacParameter : BaseParameter
    {
        public List<ImportTroCapKhacModel> ListData { get; set; }
        public int KyLuongId { get; set; }
    }
}
