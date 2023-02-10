using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Admin.NotifiSetting;
using TN.TNM.BusinessLogic.Messages.Requests.Admin.NotifiSetting;
using TN.TNM.BusinessLogic.Messages.Requests.Customer;
using TN.TNM.BusinessLogic.Messages.Responses.Admin.NotifiSetting;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Admin.NotifiSetting;
using TN.TNM.DataAccess.Messages.Results.Admin.NotifiSetting;

namespace TN.TNM.Api.Controllers
{
    public class NotifiSettingController : Controller
    {
        private readonly INotifiSetting _iNotifiSetting;
        private readonly INotifiSettingDataAccess _iNotifiSettingDataAccess;
        public NotifiSettingController(INotifiSetting iNotifiSetting, INotifiSettingDataAccess iNotifiSettingDataAccess)
        {
            this._iNotifiSetting = iNotifiSetting;
            _iNotifiSettingDataAccess = iNotifiSettingDataAccess;
        }

        [HttpPost]
        [Route("api/notifisetting/getMasterDataNotifiSettingCreate")]
        [Authorize(Policy = "Member")]
        public GetMasterDataNotifiSettingCreateResult GetMasterDataNotifiSettingCreate(
            [FromBody]GetMasterDataNotifiSettingCreateParameter request)
        {
            return this._iNotifiSettingDataAccess.GetMasterDataNotifiSettingCreate(request);
        }

        //
        [HttpPost]
        [Route("api/notifisetting/createNotifiSetting")]
        [Authorize(Policy = "Member")]
        public CreateNotifiSettingResult CreateNotifiSetting(
            [FromBody]CreateNotifiSettingParameter request)
        {
            return this._iNotifiSettingDataAccess.CreateNotifiSetting(request);
        }

        //
        [HttpPost]
        [Route("api/notifisetting/getMasterDataNotifiSettingDetail")]
        [Authorize(Policy = "Member")]
        public GetMasterDataNotifiSettingDetailResult GetMasterDataNotifiSettingDetail(
            [FromBody]GetMasterDataNotifiSettingDetailParameter request)
        {
            return this._iNotifiSettingDataAccess.GetMasterDataNotifiSettingDetail(request);
        }

        //
        [HttpPost]
        [Route("api/notifisetting/updateNotifiSetting")]
        [Authorize(Policy = "Member")]
        public UpdateNotifiSettingResult UpdateNotifiSetting(
            [FromBody]UpdateNotifiSettingParameter request)
        {
            return this._iNotifiSettingDataAccess.UpdateNotifiSetting(request);
        }

        //
        [HttpPost]
        [Route("api/notifisetting/getMasterDataSearchNotifiSetting")]
        [Authorize(Policy = "Member")]
        public GetMasterDataSearchNotifiSettingResult GetMasterDataSearchNotifiSetting(
            [FromBody]GetMasterDataSearchNotifiSettingParameter request)
        {
            return this._iNotifiSettingDataAccess.GetMasterDataSearchNotifiSetting(request);
        }

        //
        [HttpPost]
        [Route("api/notifisetting/searchNotifiSetting")]
        [Authorize(Policy = "Member")]
        public SearchNotifiSettingResult SearchNotifiSetting([FromBody] SearchNotifiSettingParameter request)
        {
            return this._iNotifiSettingDataAccess.SearchNotifiSetting(request);
        }

        //
        [HttpPost]
        [Route("api/notifisetting/changeBackHourInternal")]
        [Authorize(Policy = "Member")]
        public ChangeBackHourInternalResult ChangeBackHourInternal([FromBody] ChangeBackHourInternalParameter request)
        {
            return this._iNotifiSettingDataAccess.ChangeBackHourInternal(request);
        }

        //
        [HttpPost]
        [Route("api/notifisetting/changeActive")]
        [Authorize(Policy = "Member")]
        public ChangeActiveResult ChangeActive([FromBody] ChangeActiveParameter request)
        {
            return this._iNotifiSettingDataAccess.ChangeActive(request);
        }

        //
        [HttpPost]
        [Route("api/notifisetting/changeSendInternal")]
        [Authorize(Policy = "Member")]
        public ChangeSendInternalResult ChangeSendInternal([FromBody] ChangeSendInternalParameter request)
        {
            return this._iNotifiSettingDataAccess.ChangeSendInternal(request);
        }

        //
        [HttpPost]
        [Route("api/notifisetting/changeIsSystem")]
        [Authorize(Policy = "Member")]
        public ChangeIsSystemResult ChangeIsSystem([FromBody] ChangeIsSystemParameter request)
        {
            return this._iNotifiSettingDataAccess.ChangeIsSystem(request);
        }

        //
        [HttpPost]
        [Route("api/notifisetting/changeIsEmail")]
        [Authorize(Policy = "Member")]
        public ChangeIsEmailResult ChangeIsEmail([FromBody] ChangeIsEmailParameter request)
        {
            return this._iNotifiSettingDataAccess.ChangeIsEmail(request);
        }

        //
        [HttpPost]
        [Route("api/notifisetting/changeIsSms")]
        [Authorize(Policy = "Member")]
        public ChangeIsSmsResult ChangeIsSms([FromBody] ChangeIsSmsParameter request)
        {
            return this._iNotifiSettingDataAccess.ChangeIsSms(request);
        }
    }
}
