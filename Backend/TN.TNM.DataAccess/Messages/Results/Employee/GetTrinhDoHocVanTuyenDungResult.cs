using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetTrinhDoHocVanTuyenDungResult : BaseResult
    {
        public TrinhDoHocVanTuyenDungModel TrinhDoHocVanTuyenDung { get; set; }
        public bool IsShowButtonSua { get; set; }
    }
}
