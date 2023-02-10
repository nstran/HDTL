using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetMasterDataDeNghiFormResult : BaseResult
    {
        public List<HoSoCongTacEntityModel> ListHoSoCongTac { get; set; }
        public List<HoSoCongTacEntityModel> ListHoSoCongTacFull { get; set; }
        public List<CategoryEntityModel> ListHinhThucTT { get; set; }
        public string EmployeeName { get; set; }
    }
}
