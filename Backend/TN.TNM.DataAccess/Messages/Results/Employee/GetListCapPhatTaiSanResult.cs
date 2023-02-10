using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetListCapPhatTaiSanResult : BaseResult
    {
        public List<CapPhatTaiSanEntityModel> ListCapPhatTaiSan { get; set; }
        public bool IsShowButton { get; set; }
    }
}
