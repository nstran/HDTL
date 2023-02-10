using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.QuyTrinh
{
    public class GetMasterDataSearchQuyTrinhResult : BaseResult
    {
        public List<EmployeeEntityModel> ListEmployee { get; set; }
    }
}
