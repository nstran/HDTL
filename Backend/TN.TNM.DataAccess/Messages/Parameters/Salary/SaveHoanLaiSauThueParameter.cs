using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Salary;

namespace TN.TNM.DataAccess.Messages.Parameters.Salary
{
    public class SaveHoanLaiSauThueParameter : BaseParameter
    {
        public List<LuongCtHoanLaiSauThueModel> ListLuongCtHoanLaiSauThue { get; set; }
    }
}
