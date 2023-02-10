using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Project;

namespace TN.TNM.DataAccess.Messages.Results.Project
{
    public class GetPhanBoTheoNguonLucResult : BaseResult
    {
        public string EmployeeCodeName { get; set; }
        public List<PhanBoNguonLucModel> ListPhanBoNguonLuc { get; set; }
    }
}
