
using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Asset
{
    public class GetMasterDataDeXuatCongTacResult : BaseResult
    {
        public List<EmployeeEntityModel> ListEmp { get; set; }
        public Guid EmployeeId { get; set; }
    }
}
