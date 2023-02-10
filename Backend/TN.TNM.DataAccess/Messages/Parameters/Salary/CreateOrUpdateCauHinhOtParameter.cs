using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.CauHinhOtMođel;

namespace TN.TNM.DataAccess.Messages.Parameters.Salary
{
    public class CreateOrUpdateCauHinhOtParameter : BaseParameter
    {
        public CauHinhOtModel CauHinhOt { get; set; }
    }
}
