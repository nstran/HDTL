using TN.TNM.DataAccess.Messages.Parameters.Note;
using TN.TNM.DataAccess.Messages.Results.Note;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface INoteDataAccess
    {
        /// <summary>
        /// CreateNote
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        CreateNoteResult CreateNote(CreateNoteParameter parameter);

        /// <summary>
        /// DisableNote
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        DisableNoteResult DisableNote(DisableNoteParameter parameter);

        CreateNoteAndNoteDocumentResult CreateNoteAndNoteDocument(CreateNoteAndNoteDocumentParameter parameter);
        EditNoteByIdResult EditNoteById(EditNoteByIdParameter parameter);
        SearchNoteResult SearchNote(SearchNoteParameter parameter);
        CreateNoteForCustomerDetailResult CreateNoteForCustomerDetail(CreateNoteForCustomerDetailParameter parameter);
        CreateNoteForLeadDetailResult CreateNoteForLeadDetail(CreateNoteForLeadDetailParameter parameter);
        CreateNoteForOrderDetailResult CreateNoteForOrderDetail(CreateNoteForOrderDetailParameter parameter);
        CreateNoteForQuoteDetailResult CreateNoteForQuoteDetail(CreateNoteForQuoteDetailParameter parameter);
        CreateNoteForSaleBiddingDetailResult CreateNoteForSaleBiddingDetail(CreateNoteForSaleBiddingDetailParameter parameter);
        CreateNoteForContractResult CreateNoteForContract(CreateNoteForContractParameter parameter);
        CreateNoteForBillSaleDetailResult CreateNoteForBillSaleDetail(CreateNoteForBillSaleDetailParameter parameter);
        CreateNoteForProjectDetailResult CreateNoteForProjectDetail(CreateNoteForProjectDetailParameter parameter);
        
        DeleteNoteDocumentResult DeleteNoteDocument(DeleteNoteDocumentParameter parameter);
        
        CreateNoteForObjectResult CreateNoteForObject(CreateNoteForObjectParameter parameter);
        CreateNoteForProductionOrderDetailResult CreateNoteForProductionOrderDetail(CreateNoteForProductionOrderDetailParameter parameter);
        CreateNoteForProjectResourceResult CreateNoteForProjectResource(CreateNoteForProjectResourceParameter parameter);
        CreateNoteForProjectScopeResult CreateNoteForProjectScope(CreateNoteForProjectScopeParameter parameter);
        CreateNoteTaskResult CreateNoteTask(CreateNoteTaskParameter parameter);
        CreateNoteMilestoneResult CreateNoteMilestone(CreateNoteMilestoneParameter parameter);
        CreateNoteForAllRecruitmentCampaignResult CreateDocumentRecruitmentCampaign(CreateNoteForAllRecruitmentCampaignParameter parameter);
        GetListNoteResult GetListNote(GetListNoteParameter parameter);
        ThemMoiGhiChuResult ThemMoiGhiChu(ThemMoiGhiChuParameter parameter);
        XoaGhiChuResult XoaGhiChu(XoaGhiChuParameter parameter);
        XoaFileGhiChuResult XoaFileGhiChu(XoaFileGhiChuParameter parameter);
    }
}
