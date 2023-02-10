using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.ChamCong;

namespace TN.TNM.DataAccess.Messages.Parameters.Salary
{
    public class CreateOrUpdateCauHinhOtCaNgayParameter : BaseParameter
    {
        public CauHinhOtCaNgayModel CauHinhOtCaNgay { get; set; }
    }
}
