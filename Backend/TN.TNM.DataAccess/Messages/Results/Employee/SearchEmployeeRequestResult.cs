using System.Collections.Generic;
using TN.TNM.DataAccess.Models.DeXuatXinNghiModel;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class SearchEmployeeRequestResult : BaseResult
    {
        public List<DeXuatXinNghiModel> ListDeXuatXinNghi { get; set; }
    }
}
