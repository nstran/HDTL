using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Contract;
using TN.TNM.BusinessLogic.Messages.Requests.Contract;
using TN.TNM.BusinessLogic.Messages.Responses.Contract;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Contract;
using TN.TNM.DataAccess.Messages.Results.Contract;

namespace TN.TNM.Api.Controllers
{
    public class ContractController : Controller
    {
        private IContract _iContract;
        private IContractDataAccess _iContractDataAccess;
        public ContractController(IContract iContract, IContractDataAccess iContractDataAccess)
        {
            this._iContract = iContract;
            this._iContractDataAccess = iContractDataAccess;
        }
        /// <summary>
        /// Get master data contract
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/contract/getMaterDataContract")]
        [Authorize(Policy = "Member")]
        public GetMaterDataContractResult GetMaterDataContract([FromBody]GetMasterDataContractParameter request)
        {
            return this._iContractDataAccess.GetMasterDataContract(request);
        }

        /// <summary>
        /// Get list main contract
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/contract/getListMainContract")]
        [Authorize(Policy = "Member")]
        public GetListMainContractResult GetListMainContract([FromBody]GetListMainContractParameter request)
        {
            return this._iContractDataAccess.GetListMainContract(request);
        }

        /// <summary>
        /// Create or update contract
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/contract/createOrUpdateContract")]
        [Authorize(Policy = "Member")]
        public CreateOrUpdateContractResult CreateOrUpdateContract([FromForm]CreateOrUpdateContractParameter request)
        {
            return this._iContractDataAccess.CreateOrUpdateContract(request);
        }

        /// <summary>
        /// Clone contract
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/contract/createCloneContract")]
        [Authorize(Policy = "Member")]
        public CreateCloneContractResult CreateCloneContract([FromBody]CreateCloneContractParameter request)
        {
            return this._iContractDataAccess.CreateCloneContract(request);
        }

        [HttpPost]
        [Route("api/contract/uploadFile")]
        [Authorize(Policy = "Member")]
        public UploadFileResult UploadFile([FromForm]UploadFileParameter request)
        {
            return this._iContractDataAccess.UploadFile(request);
        }

        [HttpPost]
        [Route("api/contract/deleteFile")]
        [Authorize(Policy = "Member")]
        public DeleteFileResult DeleteFile([FromBody]DeleteFileParameter request)
        {
            return this._iContractDataAccess.DeleteFile(request);
        }

        /// <summary>
        /// Get Master data search update contract
        /// </summary>
        /// <param name="request">Contain parameter</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/contract/getMasterDataSearchContract")]
        [Authorize(Policy = "Member")]
        public GetMasterDataSearchContractResult GetMasterDataSearchContract([FromBody]GetMasterDataSearchContractParameter request)
        {
            return this._iContractDataAccess.GetMasterDataSearchContract(request);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/contract/searchContract")]
        [Authorize(Policy = "Member")]
        public SearchContractResult SearchContract([FromBody]SearchContractParameter request)
        {
            return this._iContractDataAccess.SearchContract(request);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/contract/changeContractStatus")]
        [Authorize(Policy = "Member")]
        public ChangeContractStatusResult ChangeContractStatus([FromBody]ChangeContractStatusParameter request)
        {
            return this._iContractDataAccess.ChangeContractStatus(request);
        }

        /// <summary>
        /// /
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/contract/getMasterDataDashboardContract")]
        [Authorize(Policy = "Member")]
        public GetMasterDataDashBoardResult GetMasterDataDashboardContract([FromBody]GetMasterDataDashBoardParameter request)
        {
            return this._iContractDataAccess.GetMasterDataDashBoard(request);
        }

        [HttpPost]
        [Route("api/contract/deleteContract")]
        [Authorize(Policy = "Member")]
        public DeleteContractResult DeleteContract([FromBody]DeleteContractParamter request)
        {
            return this._iContractDataAccess.DeleteContract(request);
        }

        [HttpPost]
        [Route("api/contract/capNhatOrAddTheoDoiTT")]
        [Authorize(Policy = "Member")]
        public CapNhatOrAddTheoDoiTTResult CapNhatOrAddTheoDoiTT([FromBody] CapNhatOrAddTheoDoiTTParameter request)
        {
            return this._iContractDataAccess.CapNhatOrAddTheoDoiTT(request);
        }

        [HttpPost]
        [Route("api/contract/deleteTheoDoiTT")]
        [Authorize(Policy = "Member")]
        public DeleteTheoDoiTTResult DeleteTheoDoiTT([FromBody] DeleteTheoDoiTTParameter request)
        {
            return this._iContractDataAccess.DeleteTheoDoiTT(request);
        }

    }
}