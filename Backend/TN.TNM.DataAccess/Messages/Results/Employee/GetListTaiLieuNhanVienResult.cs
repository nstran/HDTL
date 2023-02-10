using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetListTaiLieuNhanVienResult : BaseResult
    {
        public List<TaiLieuNhanVienEntityModel> ListTaiLieu { get; set; }
        public bool IsShowButtonTuChoi { get; set; }
        public bool IsShowButtonXacNhan { get; set; }
        public bool IsShowButtonYeuCauXacNhan { get; set; }
        public bool IsShowButtonThemMoi { get; set; }
        public bool IsShowButtonSua { get; set; }
        public bool IsShowButtonXoa { get; set; }
    }
}
