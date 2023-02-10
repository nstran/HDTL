using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Results.Salary
{
    public class ImportLuongChiTietResult : BaseResult
    {
        public bool IsError { get; set; }
        public string MessEmpHeThong { get; set; }
        public string MessEmpBangLuong { get; set; }
    }
}
