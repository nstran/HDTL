using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Salary;

namespace TN.TNM.DataAccess.Messages.Results.Salary
{
    public class GetPhieuLuongByIdResult : BaseResult
    {
        public PhieuLuongModel PhieuLuong { get; set; }
    }
}
