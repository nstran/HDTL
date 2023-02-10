using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.GiamTru;

namespace TN.TNM.DataAccess.Messages.Parameters.Salary
{
    public class CreateOrUpdateCauHinhGiamTruParameter : BaseParameter
    {
        public CauHinhGiamTruModel CauHinhGiamTru { get; set; }
    }
}
