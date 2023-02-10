using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.CauHinhNghiLe;

namespace TN.TNM.DataAccess.Messages.Parameters.Salary
{
    public class CreateOrUpdateCauHinhNghiLeParameter : BaseParameter
    {
        public CauHinhNghiLeModel CauHinhNghiLe { get; set; }
    }
}
