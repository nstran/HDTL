using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Salary;

namespace TN.TNM.DataAccess.Messages.Results.Salary
{
    public class GetListDieuKienTroCapCoDinhResult : BaseResult
    {
        public string LoaiTroCap { get; set; }
        public List<LuongCtDieuKienTroCapCoDinhModel> ListLuongCtDieuKienTroCapCoDinh { get; set; }
    }
}
