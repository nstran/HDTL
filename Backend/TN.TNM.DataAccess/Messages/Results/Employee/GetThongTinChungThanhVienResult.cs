using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetThongTinChungThanhVienResult : BaseResult
    {
        public ThongTinChungThanhVienModel ThongTinChung { get; set; }
        public List<CategoryEntityModel> ListMission { get; set; }
        
        public bool IsShowButtonSua { get; set; }
    }
}
