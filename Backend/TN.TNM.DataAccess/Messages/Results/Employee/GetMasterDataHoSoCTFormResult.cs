using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetMasterDataHoSoCTFormResult : BaseResult
    {
        public List<DeXuatCongTacEntityModel> ListQuyetDinhCT { get; set; }
        public List<EmployeeEntityModel> ListNhanVienCT { get; set; }
    }
}
