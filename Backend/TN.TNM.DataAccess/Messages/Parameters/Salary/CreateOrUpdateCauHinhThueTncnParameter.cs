using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.CauHinhThue;

namespace TN.TNM.DataAccess.Messages.Parameters.Salary
{
    public class CreateOrUpdateCauHinhThueTncnParameter : BaseParameter
    {
        public CauHinhThueTncnModel CauHinhThueTncn { get; set; }
    }
}
