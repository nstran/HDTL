using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Messages.Parameters.Salary
{
    public class GetListKyLuongParameter : BaseParameter
    {
        public string TenKyLuong { get; set; }
        public List<int> ListTrangThai { get; set; }
    }
}
