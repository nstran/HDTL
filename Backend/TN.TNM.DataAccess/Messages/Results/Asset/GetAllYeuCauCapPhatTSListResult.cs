
using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Asset;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Asset
{
    public class GetAllYeuCauCapPhatTSListResult : BaseResult
    {
        public List<EmployeeEntityModel> ListEmployee { get; set; }
        public List<YeuCauCapPhatTaiSanEntityModel> ListYeuCauCapPhatTaiSan { get; set; }
    }
}
