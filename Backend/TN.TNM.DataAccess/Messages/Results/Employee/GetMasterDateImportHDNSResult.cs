using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetMasterDateImportHDNSResult: BaseResult
    {
        public List<HopDongNhanSu> ListHopDong { get; set; }
        public List<CategoryEntityModel> ListLoaiHopDong { get; set; }
        public List<Position> ListChucVu { get; set; }
        public List<string> ListEmployeeCode { get; set; }
    }
}
