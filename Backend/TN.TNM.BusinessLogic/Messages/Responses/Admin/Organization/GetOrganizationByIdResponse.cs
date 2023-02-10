using System.Collections.Generic;
using TN.TNM.BusinessLogic.Models.Admin;
using TN.TNM.DataAccess.Models;

namespace TN.TNM.BusinessLogic.Messages.Responses.Admin.Organization
{
    public class GetOrganizationByIdResponse : BaseResponse
    {
        public OrganizationModel Organization { get; set; }
        public List<ThanhVienPhongBanModel> ListThanhVienPhongBan { get; set; }
    }
}
