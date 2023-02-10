using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class CreateOrUpdateCauHinhBaoHiemParameter : BaseParameter
    {
        public CauHinhBaoHiemModel CauHinhBaoHiem { get; set; }
    }
}
