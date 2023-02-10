using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetMasterDataCreatePhieuDanhGiaResult: BaseResult
    {
        public List<EmployeeEntityModel> ListEmp { get; set; }
        public Guid LoginEmployeeID { get; set; }
        public List<TrangThaiGeneral> ListDangCauTraLoi { get; set; }
        public List<CategoryEntityModel> ListItemCauTraLoi { get; set; }
        public List<MucDanhGia> ListThangDiemDanhGia { get; set; }

    }
}
