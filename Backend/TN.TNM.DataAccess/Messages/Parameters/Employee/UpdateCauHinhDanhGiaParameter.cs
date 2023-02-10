using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Parameters.Employee
{
    public class UpdateCauHinhDanhGiaParameter:BaseParameter
    {
        public List<MucDanhGiaDanhGiaMappingEntityModel> ListThangDiemDanhGia { get; set; }
        public int MucDanhGiaId { get; set; }
    }
}
