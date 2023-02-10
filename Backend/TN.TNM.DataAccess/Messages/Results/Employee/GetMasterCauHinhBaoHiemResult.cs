using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Note;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetMasterCauHinhBaoHiemResult : BaseResult
    {
        public bool IsExists { get; set; }
        public CauHinhBaoHiemModel CauHinhBaoHiemModel { get; set; }
        public List<TrangThaiGeneral> ListLoaiDong { get; set; }
    }
}
