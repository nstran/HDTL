using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetThongTinGiaDinhResult:BaseResult
    {
        public List<ContactEntityModel> ListThanhVienGiaDinh { get; set; }
        public bool IsManager { get; set; }
        public List<CategoryEntityModel> ListQuanHe { get; set; }
    }
}
