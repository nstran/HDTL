using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Xml;
using System.Threading.Tasks;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.MilestoneConfiguration;
using TN.TNM.DataAccess.Messages.Results.MilestoneConfiguration;
using TN.TNM.DataAccess.Messages.Results.Options;

namespace TN.TNM.Api.Controllers
{
    public class MilestoneConfigurationController : Controller
    {
        private readonly IMilestoneConfigurationDataAccess _milestoneConfigurationDataAccess;
        public MilestoneConfigurationController(IMilestoneConfigurationDataAccess milestoneConfigurationDataAccess)
        {
            _milestoneConfigurationDataAccess = milestoneConfigurationDataAccess;
        }

        [HttpGet]
        [Route("api/milestone/getListMilestoneConfiguration")]
        [Authorize(Policy = "Member")]
        public async Task<GetListMilestoneConfigurationResult> GetListMilestoneConfigurationResult()
        {
            return await this._milestoneConfigurationDataAccess.GetListMilestoneConfigurationResult();
        }
        [HttpPost]
        [Route("api/milestone/createMilestoneConfiguration")]
        [Authorize(Policy = "Member")]
        public async Task<CreateOrUpdateMilestoneConfigurationResult> CreateOrUpdateMilestoneConfigurationResult([FromBody] CreateOrUpdateMilestoneConfigurationParameter request)
        {
            return await this._milestoneConfigurationDataAccess.CreateOrUpdateMilestoneConfiguration(request);
        }
        [HttpPost]
        [Route("api/milestone/deleteMilestoneConfiguration")]
        [Authorize(Policy = "Member")]
        public async Task<MilestoneConfigurationResult> DeleteMilestoneConfiguration([FromBody] DeleteMilestoneConfigurationParameter request)
        {
            return await this._milestoneConfigurationDataAccess.DeleteMilestoneConfiguration(request);
        }
        [HttpPost]
        [Route("api/milestone/getListMilestoneConfigurationByOptionId")]
        [Authorize(Policy = "Member")]
        public async Task<GetListMilestoneConfigurationResult> GetListMilestoneConfigurationByOptionIdResult([FromBody] GetListMilestoneConfigurationByOptionIdParameter request)
        {
            return await this._milestoneConfigurationDataAccess.GetListMilestoneConfigurationByOptionIdResult(request);
        }
        [HttpPost]
        [Route("api/milestone/updateListMilestoneConfigure")]
        [Authorize(Policy = "Member")]
        public async Task<UpdateListMilestoneConfigurationResult> UpdateListMilestoneConfigure([FromBody] UpdateListMilestoneConfigurationParameter request)
        {
            return await this._milestoneConfigurationDataAccess.UpdateListMilestoneConfigure(request);
        }
    }
}
