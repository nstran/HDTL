using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetCauHinhBaoHiemLoftCareByIdResult : BaseResult
    {
        public bool IsExists { get; set; }
        public List<CauHinhBaoHiemLoftCareModel> ListCauHinhBaoHiemLoftCare { get; set; }
        public List<PositionModel> ListPosition { get; set; }
        public List<TrangThaiGeneral> ListDonVi { get; set; }
        public List<TrangThaiGeneral> ListDoiTuong { get; set; }
    }
}
