using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Promotion;
using TN.TNM.BusinessLogic.Messages.Requests.Promotion;
using TN.TNM.BusinessLogic.Messages.Responses.Promotion;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Promotion;
using TN.TNM.DataAccess.Messages.Results.Promotion;

namespace TN.TNM.Api.Controllers
{
    public class PromotionController : Controller
    {
        private readonly IPromotion iPromotion;
        private readonly IPromotionDataAccess iPromotionDataAccess;
        public PromotionController(IPromotion _iPromotion, IPromotionDataAccess _iPromotionDataAccess)
        {
            this.iPromotion = _iPromotion;
            this.iPromotionDataAccess = _iPromotionDataAccess;
        }

        [HttpPost]
        [Route("api/promotion/getMasterDataCreatePromotion")]
        [Authorize(Policy = "Member")]
        public GetMasterDataCreatePromotionResult GetMasterDataCreatePromotion([FromBody]GetMasterDataCreatePromotionParameter request)
        {
            return iPromotionDataAccess.GetMasterDataCreatePromotion(request);
        }

        //
        [HttpPost]
        [Route("api/promotion/createPromotion")]
        [Authorize(Policy = "Member")]
        public CreatePromotionResult CreatePromotion([FromBody]CreatePromotionParameter request)
        {
            return iPromotionDataAccess.CreatePromotion(request);
        }

        //
        [HttpPost]
        [Route("api/promotion/getMasterDataListPromotion")]
        [Authorize(Policy = "Member")]
        public GetMasterDataListPromotionResult GetMasterDataListPromotion([FromBody]GetMasterDataListPromotionParameter request)
        {
            return iPromotionDataAccess.GetMasterDataListPromotion(request);
        }

        //
        [HttpPost]
        [Route("api/promotion/searchListPromotion")]
        [Authorize(Policy = "Member")]
        public SearchListPromotionResult SearchListPromotion([FromBody]SearchListPromotionParameter request)
        {
            return iPromotionDataAccess.SearchListPromotion(request);
        }

        //
        [HttpPost]
        [Route("api/promotion/getMasterDataDetailPromotion")]
        [Authorize(Policy = "Member")]
        public GetMasterDataDetailPromotionResult GetMasterDataDetailPromotion([FromBody]GetMasterDataDetailPromotionParameter request)
        {
            return iPromotionDataAccess.GetMasterDataDetailPromotion(request);
        }

        //
        [HttpPost]
        [Route("api/promotion/getDetailPromotion")]
        [Authorize(Policy = "Member")]
        public GetDetailPromotionResult GetDetailPromotion([FromBody]GetDetailPromotionParameter request)
        {
            return iPromotionDataAccess.GetDetailPromotion(request);
        }

        //
        [HttpPost]
        [Route("api/promotion/deletePromotion")]
        [Authorize(Policy = "Member")]
        public DeletePromotionResult DeletePromotion([FromBody]DeletePromotionParameter request)
        {
            return iPromotionDataAccess.DeletePromotion(request);
        }

        //
        [HttpPost]
        [Route("api/promotion/updatePromotion")]
        [Authorize(Policy = "Member")]
        public UpdatePromotionResult UpdatePromotion([FromBody]UpdatePromotionParameter request)
        {
            return iPromotionDataAccess.UpdatePromotion(request);
        }

        //
        [HttpPost]
        [Route("api/promotion/createLinkForPromotion")]
        [Authorize(Policy = "Member")]
        public CreateLinkForPromotionResult CreateLinkForPromotion([FromBody]CreateLinkForPromotionParameter request)
        {
            return iPromotionDataAccess.CreateLinkForPromotion(request);
        }

        //
        [HttpPost]
        [Route("api/promotion/deleteLinkFromPromotion")]
        [Authorize(Policy = "Member")]
        public DeleteLinkFromPromotionResult DeleteLinkFromPromotion([FromBody]DeleteLinkFromPromotionParameter request)
        {
            return iPromotionDataAccess.DeleteLinkFromPromotion(request);
        }

        //
        [HttpPost]
        [Route("api/promotion/createFileForPromotion")]
        [Authorize(Policy = "Member")]
        public CreateFileForPromotionResult CreateFileForPromotion([FromForm]CreateFileForPromotionParameter request)
        {
            return iPromotionDataAccess.CreateFileForPromotion(request);
        }

        //
        [HttpPost]
        [Route("api/promotion/deleteFileFromPromotion")]
        [Authorize(Policy = "Member")]
        public DeleteFileFromPromotionResult DeleteFileFromPromotion([FromBody]DeleteFileFromPromotionParameter request)
        {
            return iPromotionDataAccess.DeleteFileFromPromotion(request);
        }

        //
        [HttpPost]
        [Route("api/promotion/createNoteForPromotionDetail")]
        [Authorize(Policy = "Member")]
        public CreateNoteForPromotionDetailResult CreateNoteForPromotionDetail([FromBody]CreateNoteForPromotionDetailParameter request)
        {
            return iPromotionDataAccess.CreateNoteForPromotionDetail(request);
        }

        //
        [HttpPost]
        [Route("api/promotion/checkPromotionByCustomer")]
        [Authorize(Policy = "Member")]
        public CheckPromotionByCustomerResult CheckPromotionByCustomer([FromBody]CheckPromotionByCustomerParameter request)
        {
            return iPromotionDataAccess.CheckPromotionByCustomer(request);
        }

        //
        [HttpPost]
        [Route("api/promotion/getApplyPromotion")]
        [Authorize(Policy = "Member")]
        public GetApplyPromotionResult GetApplyPromotion([FromBody]GetApplyPromotionParameter request)
        {
            return iPromotionDataAccess.GetApplyPromotion(request);
        }

        //
        [HttpPost]
        [Route("api/promotion/checkPromotionByAmount")]
        [Authorize(Policy = "Member")]
        public CheckPromotionByAmountResult CheckPromotionByAmount([FromBody]CheckPromotionByAmountParameter request)
        {
            return iPromotionDataAccess.CheckPromotionByAmount(request);
        }

        //
        [HttpPost]
        [Route("api/promotion/checkPromotionByProduct")]
        [Authorize(Policy = "Member")]
        public CheckPromotionByProductResult CheckPromotionByProduct([FromBody]CheckPromotionByProductParameter request)
        {
            return iPromotionDataAccess.CheckPromotionByProduct(request);
        }
    }
}
