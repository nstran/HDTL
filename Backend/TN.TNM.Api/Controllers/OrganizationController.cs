using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Admin.Organization;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.Organization;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.Organization;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Admin.Organization;
using TN.TNM.DataAccess.Messages.Results.Admin.Organization;

namespace TN.TNM.Api.Controllers
{
    public class OrganizationController : Controller
    {
        private readonly IOrganizationDataAccess iOrganizationDataAccess;
        public OrganizationController(IOrganizationDataAccess _iOrganizationDataAccess)
        {
            iOrganizationDataAccess = _iOrganizationDataAccess;
        }

        [HttpPost]
        [Route("api/organization/getAllOrganization")]
        [Authorize(Policy = "Member")]
        public GetAllOrganizationResult GetAllOrganization(GetAllOrganizationParameter request)
        {
            return this.iOrganizationDataAccess.GetAllOrganization(request);
        }

        [HttpPost]
        [Route("api/organization/createOrganization")]
        [Authorize(Policy = "Member")]
        public CreateOrganizationResult CreateOrganization([FromBody] CreateOrganizationParameter request)
        {
            return this.iOrganizationDataAccess.CreateOrganization(request);
        }

        [HttpPost]
        [Route("api/organization/getOrganizationById")]
        [Authorize(Policy = "Member")]
        public GetOrganizationByIdResult GetOrganizationById([FromBody] GetOrganizationByIdParameter request)
        {
            return this.iOrganizationDataAccess.GetOrganizationById(request);
        }

        [HttpPost]
        [Route("api/organization/editOrganizationById")]
        [Authorize(Policy = "Member")]
        public EditOrganizationByIdResult EditOrganizationById([FromBody] EditOrganizationByIdParameter request)
        {
            return this.iOrganizationDataAccess.EditOrganizationById(request);
        }

        [HttpPost]
        [Route("api/organization/deleteOrganizationById")]
        [Authorize(Policy = "Member")]
        public DeleteOrganizationByIdResult DeleteOrganizationById([FromBody] DeleteOrganizationByIdParameter request)
        {
            return this.iOrganizationDataAccess.DeleteOrganizationById(request);
        }

        [HttpPost]
        [Route("api/organization/getAllOrganizationCode")]
        [Authorize(Policy = "Member")]
        public GetAllOrganizationCodeResult GetAllOrganizationCode([FromBody] GetAllOrganizationCodeParameter request)
        {
            return this.iOrganizationDataAccess.GetAllOrganizationCode(request);
        }

        [HttpPost]
        [Route("api/organization/getFinancialindependenceOrg")]
        [Authorize(Policy = "Member")]
        public GetFinancialindependenceOrgResult GetFinancialindependenceOrg([FromBody] GetFinancialindependenceOrgParameter request)
        {
            return this.iOrganizationDataAccess.GetFinancialindependenceOrg(request);
        }

        [HttpPost]
        [Route("api/organization/getChildrenOrganizationById")]
        [Authorize(Policy = "Member")]
        public GetChildrenOrganizationByIdResult GetChildrenOrganizationById([FromBody] GetChildrenOrganizationByIdParameter request)
        {
            return this.iOrganizationDataAccess.GetChildrenOrganizationById(request);
        }

        [HttpPost]
        [Route("api/organization/getOrganizationByEmployeeId")]
        [Authorize(Policy = "Member")]
        public GetOrganizationByEmployeeIdResult GetOrganizationByEmployeeId([FromBody] GetOrganizationByEmployeeIdParameter request)
        {
            return this.iOrganizationDataAccess.GetOrganizationByEmployeeId(request);
        }

        [HttpPost]
        [Route("api/organization/getChildrenByOrganizationId")]
        [Authorize(Policy = "Member")]
        public GetChildrenByOrganizationIdResult GetChildrenByOrganizationId([FromBody] GetChildrenByOrganizationIdParameter request)
        {
            return this.iOrganizationDataAccess.GetChildrenByOrganizationId(request);
        }

        [HttpPost]
        [Route("api/organization/UpdateOrganizationById")]
        [Authorize(Policy = "Member")]
        public UpdateOrganizationByIdResult UpdateOrganizationById([FromBody] UpdateOrganizationByIdParameter request)
        {
            return this.iOrganizationDataAccess.UpdateOrganizationById(request);
        }

        [HttpPost]
        [Route("api/organization/getOrganizationByUser")]
        [Authorize(Policy = "Member")]
        public GetOrganizationByUserResult GetOrganizationByUser([FromBody] GetOrganizationByUserParameter request)
        {
            return this.iOrganizationDataAccess.GetOrganizationByUser(request);
        }

        [HttpPost]
        [Route("api/organization/deleteNhanVienThuocDonVi")]
        [Authorize(Policy = "Member")]
        public DeleteNhanVienThuocDonViResult DeleteNhanVienThuocDonVi([FromBody] DeleteNhanVienThuocDonViParameter request)
        {
            return this.iOrganizationDataAccess.DeleteNhanVienThuocDonVi(request);
        }
    }
}
