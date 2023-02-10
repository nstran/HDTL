using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TN.TNM.BusinessLogic.Interfaces.Admin;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Results.Asset;
using TN.TNM.DataAccess.Messages.Parameters.Asset;
using TN.TNM.DataAccess.Messages.Results.Employee;
using TN.TNM.DataAccess.Messages.Parameters.Contract;
using TN.TNM.DataAccess.Messages.Parameters.Employee;
using TN.TNM.DataAccess.Messages.Results.FireBase;
using TN.TNM.DataAccess.Messages.Parameters.FireBase;

namespace TN.TNM.Api.Controllers
{
    public class FireBaseController : Controller
    {
        private readonly IFireBaseDataAccess _iFireBaseDataAccess;

        public FireBaseController(IFireBaseDataAccess iFireBaseDataAccess)
        {
            _iFireBaseDataAccess = iFireBaseDataAccess;
        }

        [HttpPost]
        [Route("api/FireBase/takeDataFireBaseByUserId")]
        [Authorize(Policy = "Member")]
        public TakeDataFireBaseByUserIdResult TakeDataFireBaseByUserId([FromBody] TakeDataFireBaseByUserIdParameter request)
        {
            return this._iFireBaseDataAccess.TakeDataFireBaseByUserId(request);
        }

        [HttpPost]
        [Route("api/FireBase/createDataFireBase")]
        [Authorize(Policy = "Member")]
        public CreateDataFireBaseResult CreateDataFireBase([FromBody] CreateDataFireBaseParameter request)
        {
            return this._iFireBaseDataAccess.CreateDataFireBase(request);
        }
    }
}
