
using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class UpdateDeNghiTamHoanUngChiTietResult : BaseResult
    {
        public List<DeNghiTamHoanUngChiTiet> ListNoiDungTT { get; set; }
    }
}
