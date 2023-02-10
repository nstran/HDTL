using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Address;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetThongTinNhanSuResult : BaseResult
    {
        public ThongTinNhanSuModel ThongTinNhanSu { get; set; }
        public List<BaseType> ListDeptCode { get; set; }
        public List<BaseType> ListSubCode1 { get; set; }
        public List<BaseType> ListSubCode2 { get; set; }
        public List<CategoryEntityModel> ListCapBac { get; set; }
        public List<ProvinceEntityModel> ListProvince { get; set; }
        public bool IsShowButtonSua { get; set; }
    }
}
