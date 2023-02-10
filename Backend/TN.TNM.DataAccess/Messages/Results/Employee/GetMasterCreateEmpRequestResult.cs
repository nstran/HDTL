using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Helper;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetMasterCreateEmpRequestResult : BaseResult
    {
        public List<TrangThaiGeneral> ListKyHieuChamCong { get; set; }
        public List<TrangThaiGeneral> ListLoaiCaLamViec { get; set; }
        public string EmployeeName { get; set; }
        public string OrganizationName { get; set; }
        public string PositionName { get; set; }
        public decimal SoNgayPhepConLai { get; set; }
    }
}
