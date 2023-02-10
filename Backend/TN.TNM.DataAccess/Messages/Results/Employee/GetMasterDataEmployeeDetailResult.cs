using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Address;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetMasterDataEmployeeDetailResult : BaseResult
    {
        public List<PositionModel> ListPosition { get; set; }
        public List<OrganizationEntityModel> ListOrganization { get; set; }
        public List<RoleEntityModel> ListRole { get; set; }
        public List<CategoryEntityModel> ListLoaiHopDong { get; set; }
        public List<CountryEntityModel> ListQuocGia { get; set; }
        public List<CategoryEntityModel> ListNganHang { get; set; }
    }
}
