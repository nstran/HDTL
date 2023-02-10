using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Salary;

namespace TN.TNM.DataAccess.Messages.Results.Salary
{
    public class GetListDieuKienTroCapKhacResult : BaseResult
    {
        public string LoaiTroCap { get; set; }
        public decimal MucTroCap { get; set; }
        public List<LuongCtDieuKienTroCapKhacModel> ListLuongCtDieuKienTroCapKhac { get; set; }
        public bool IsCoCauHinhLoaiTroCap { get; set; }
    }
}
