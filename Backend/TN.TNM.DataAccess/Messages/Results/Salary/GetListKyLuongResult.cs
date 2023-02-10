using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Models.Salary;

namespace TN.TNM.DataAccess.Messages.Results.Salary
{
    public class GetListKyLuongResult : BaseResult
    {
        public List<TrangThaiGeneral> ListStatus { get; set; }
        public List<KyLuongModel> ListData { get; set; }
    }
}
