using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TN.TNM.BusinessLogic.Interfaces.Note;
using TN.TNM.BusinessLogic.Messages.Requests.Note;
using TN.TNM.BusinessLogic.Messages.Responses.Note;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Note;
using TN.TNM.DataAccess.Messages.Results.Note;

namespace TN.TNM.Api.Controllers
{
    public class NoteController : Controller
    {
        private readonly INote iNote;
        private readonly INoteDataAccess iNoteDataAccess;
        public NoteController(INote _iNote, INoteDataAccess _iNoteDataAccess)
        {
            this.iNote = _iNote;
            this.iNoteDataAccess = _iNoteDataAccess;
        }

        /// <summary>
        /// CreateNote
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/note/createNote")]
        [Authorize(Policy = "Member")]
        public CreateNoteResponse CreateNote(CreateNoteRequest request)
        {
            return this.iNote.CreateNote(request);
        }

        /// <summary>
        /// Disable/delete note
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/note/disableNote")]
        [Authorize(Policy = "Member")]
        public DisableNoteResult DisableNote([FromBody]DisableNoteParameter request)
        {
            return this.iNoteDataAccess.DisableNote(request);
        }

        /// <summary>
        /// Create a note contains document
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/note/createNoteAndNoteDocument")]
        [Authorize(Policy = "Member")]
        public CreateNoteAndNoteDocumentResponse CreateNoteAndNoteDocument(
             CreateNoteAndNoteDocumentRequest request)
        {
            return this.iNote.CreateNoteAndNoteDocument(request);
        }

        /// <summary>
        /// Edit a note
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/note/editNoteById")]
        [Authorize(Policy = "Member")]
        public EditNoteByIdResponse EditNoteById(EditNoteByIdRequest request)
        {
            return this.iNote.EditNoteById(request);
        }

        /// <summary>
        /// Search note
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/note/searchNote")]
        [Authorize(Policy = "Member")]
        public SearchNoteResponse SearchNote([FromBody]SearchNoteRequest request)
        {
            return this.iNote.SearchNote(request);
        }

        //
        /// <summary>
        /// Search note
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/note/createNoteForCustomerDetail")]
        [Authorize(Policy = "Member")]
        public CreateNoteForCustomerDetailResult CreateNoteForCustomerDetail([FromBody]CreateNoteForCustomerDetailParameter request)
        {
            return this.iNoteDataAccess.CreateNoteForCustomerDetail(request);
        }

        [HttpPost]
        [Route("api/note/createNoteForLeadDetail")]
        [Authorize(Policy = "Member")]
        public CreateNoteForLeadDetailResponse CreateNoteForLeadDetail([FromBody]CreateNoteForLeadDetailRequest request)
        {
            return this.iNote.CreateNoteForLeadDetail(request);
        }

        //
        [HttpPost]
        [Route("api/note/createNoteForOrderDetail")]
        [Authorize(Policy = "Member")]
        public CreateNoteForOrderDetailResult CreateNoteForOrderDetail([FromBody]CreateNoteForOrderDetailParameter request)
        {
            return this.iNoteDataAccess.CreateNoteForOrderDetail(request);
        }

        [HttpPost]
        [Route("api/note/createNoteForQuoteDetail")]
        [Authorize(Policy = "Member")]
        public CreateNoteForQuoteDetailResponse CreateNoteForQuoteDetail([FromBody]CreateNoteForQuoteDetailRequest request)
        {
            return this.iNote.CreateNoteForQuoteDetail(request);
        }

        [HttpPost]
        [Route("api/note/createNoteForSaleBiddingDetail")]
        [Authorize(Policy = "Member")]
        public CreateNoteForSaleBiddingDetailResponse CreateNoteForSaleBiddingDetail([FromBody]CreateNoteForSaleBiddingDetailRequest request)
        {
            return this.iNote.CreateNoteForSaleBiddingDetail(request);
        }

        [HttpPost]
        [Route("api/note/createNoteForBillSaleDetail")]
        [Authorize(Policy = "Member")]
        public CreateNoteForBillSaleDetailResponse CreateNoteForBillSaleDetail([FromBody]CreateNoteForBillSaleDetailRequest request)
        {
            return this.iNote.CreateNoteForBillSaleDetail(request);
        }

        [HttpPost]
        [Route("api/note/createNoteForContract")]
        [Authorize(Policy = "Member")]
        public CreateNoteForContractResponse CreateNoteForContract([FromBody]CreateNoteForContractRequest request)
        {
            return this.iNote.CreateNoteForContract(request);
        }
        
        [HttpPost]
        [Route("api/note/createNoteForObject")]
        [Authorize(Policy = "Member")]
        public CreateNoteForObjectResponse CreateNoteForObject([FromForm]CreateNoteForObjectRequest request)
        {
            return this.iNote.CreateNoteForObject(request);
        }
        
        // [HttpPost]
        // [Route("api/note/createNoteForProjectDetail")]
        // [Authorize(Policy = "Member")]
        // public CreateNoteForProjectDetailResponse CreateNoteForProjectDetail([FromBody] CreateNoteForProjectDetailRequest request)
        // {
        //     return this.iNote.CreateNoteForProjectDetail(request);
        // }
        
        [HttpPost]
        [Route("api/note/createNoteForProjectDetail")]
        [Authorize(Policy = "Member")]
        public CreateNoteForProjectDetailResponse CreateNoteForProjectDetail([FromForm] CreateNoteForProjectDetailRequest request)
        {
            return this.iNote.CreateNoteForProjectDetail(request);
        }

        [HttpPost]
        [Route("api/note/deleteNoteDocument")]
        [Authorize(Policy = "Member")]
        public DeleteNoteDocumentResponse DeleteNoteDocument([FromBody] DeleteNoteDocumentRequest request)
        {
            return this.iNote.DeleteNoteDocument(request);
        }

        [HttpPost]
        [Route("api/note/createNoteForProjectResource")]
        [Authorize(Policy = "Member")]
        public CreateNoteForProjectResourceResponse CreateNoteForProjectResource([FromBody] CreateNoteForProjectResourceRequest request)
        {
            return this.iNote.CreateNoteForProjectResource(request);
        }

        [HttpPost]
        [Route("api/note/createNoteForProjectScope")]
        [Authorize(Policy = "Member")]
        public CreateNoteForProjectScopeResponse CreateNoteForProjectScope([FromBody] CreateNoteForProjectScopeRequest request)
        {
            return this.iNote.CreateNoteForProjectScope(request);
        }

        [HttpPost]
        [Route("api/note/createNoteForProductionOrderDetail")]
        [Authorize(Policy = "Member")]
        public CreateNoteForProductionOrderDetailResponse CreateNoteForProductionOrderDetail([FromBody] CreateNoteForProductionOrderDetailRequest request)
        {
            return this.iNote.CreateNoteForProductionOrderDetail(request);
        }

        [HttpPost]
        [Route("api/note/createNoteTask")]
        [Authorize(Policy = "Member")]
        public CreateNoteTaskResponse CreateNoteTask([FromBody] CreateNoteTaskRequest request)
        {
            return this.iNote.CreateNoteTask(request);
        }

        [HttpPost]
        [Route("api/note/createNoteMilestone")]
        [Authorize(Policy = "Member")]
        public CreateNoteMilestoneResponse CreateNoteMilestone([FromBody] CreateNoteMilestoneRequest request)
        {
            return this.iNote.CreateNoteMilestone(request);
        }

        // Tạo note chung cho tuyển dụng
        [HttpPost]
        [Route("api/note/createNoteForAllRecruitmentCampaign")]
        [Authorize(Policy = "Member")]
        public CreateNoteForAllRecruitmentCampaignResponse CreateNoteForAllRecruitmentCampaign([FromForm] CreateNoteForAllRecruitmentCampaignRequest request)
        {
            return this.iNote.CreateNoteForAllRecruitmentCampaign(request);
        }
        [HttpPost]
        [Route("api/note/getListNote")]
        [Authorize(Policy = "Member")]
        public GetListNoteResult GetListNote([FromBody] GetListNoteParameter request)
        {
            return this.iNoteDataAccess.GetListNote(request);
        }

        [HttpPost]
        [Route("api/note/themMoiGhiChu")]
        [Authorize(Policy = "Member")]
        public ThemMoiGhiChuResult ThemMoiGhiChu([FromForm] ThemMoiGhiChuParameter parameter)
        {
            return this.iNoteDataAccess.ThemMoiGhiChu(parameter);
        }

        [HttpPost]
        [Route("api/note/xoaGhiChu")]
        [Authorize(Policy = "Member")]
        public XoaGhiChuResult XoaGhiChu([FromBody] XoaGhiChuParameter parameter)
        {
            return this.iNoteDataAccess.XoaGhiChu(parameter);
        }

        [HttpPost]
        [Route("api/note/xoaFileGhiChu")]
        [Authorize(Policy = "Member")]
        public XoaFileGhiChuResult XoaFileGhiChu([FromBody] XoaFileGhiChuParameter parameter)
        {
            return this.iNoteDataAccess.XoaFileGhiChu(parameter);
        }
    }
}
