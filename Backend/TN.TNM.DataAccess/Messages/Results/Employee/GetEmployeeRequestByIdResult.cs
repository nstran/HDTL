using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Models.DeXuatXinNghiModel;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetEmployeeRequestByIdResult : BaseResult
    {
        public DeXuatXinNghiModel DeXuatXinNghi { get; set; }
        public List<TrangThaiGeneral> ListKyHieuChamCong { get; set; }
        public List<TrangThaiGeneral> ListLoaiCaLamViec { get; set; }
        public bool IsShowGuiPheDuyet { get; set; }
        public bool IsShowPheDuyet { get; set; }
        public bool IsShowTuChoi { get; set; }
        public bool IsShowDatVeMoi { get; set; }
        public bool IsShowXoa { get; set; }
        public bool IsShowSua { get; set; }
    }
}
