using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetListHopDongNhanSuResult : BaseResult
    {
        public List<HopDongNhanSuModel> ListHopDongNhanSu { get; set; }
        public List<CategoryEntityModel> ListLoaiHopDongNhanSu { get; set; }
        public List<PositionModel> ListChucVu { get; set; }
        public bool IsShowButton { get; set; }
    }
}
