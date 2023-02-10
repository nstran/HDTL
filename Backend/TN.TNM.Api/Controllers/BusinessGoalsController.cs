using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.TNM.BusinessLogic.Interfaces.Admin.BusinessGoals;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.BusinessGoals;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.BusinessGoals;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Admin.BusinessGoals;
using TN.TNM.DataAccess.Messages.Results.Admin.BusinessGoals;

namespace TN.TNM.Api.Controllers
{
    public class BusinessGoalsController : ControllerBase
    {
        private readonly IBusinessGoals _iBusinessGoal;
        private readonly IBusinessGoalsDataAccess _iBusinessGoalDataAccess;
        public BusinessGoalsController(IBusinessGoals iBusinessGoal, IBusinessGoalsDataAccess iBusinessGoalsDataAccess)
        {
            this._iBusinessGoal = iBusinessGoal;
            this._iBusinessGoalDataAccess = iBusinessGoalsDataAccess;
        }

        [HttpPost]
        [Route("api/billSale/getMasterDataBusinessGoals")]
        [Authorize(Policy = "Member")]
        public GetMasterDataBusinessGoalsResult GetMasterDataBusinessGoals([FromBody] GetMasterDataBusinessGoalsParameter request)
        {
            return _iBusinessGoalDataAccess.GetMasterDataBusinessGoals(request);
        }

        [HttpPost]
        [Route("api/billSale/createOrUpdateBusinessGoals")]
        [Authorize(Policy = "Member")]
        public CreateOrUpdateBusinessGoalsResult CreateOrUpdateBusinessGoals([FromBody] CreateOrUpdateBusinessGoalsParameter request)
        {
            return _iBusinessGoalDataAccess.CreateOrUpdateBusinessGoals(request);
        }
    }
}
