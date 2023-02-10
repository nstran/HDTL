using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.BusinessLogic.Messages.Requests.QuyTrinh;
using TN.TNM.BusinessLogic.Messages.Responses.QuyTrinh;

namespace TN.TNM.BusinessLogic.Interfaces.QuyTrinh
{
    public interface IQuyTrinh
    {
        CreateQuyTrinhResponse CreateQuyTrinh(CreateQuyTrinhRequest request);
        SearchQuyTrinhResponse SearchQuyTrinh(SearchQuyTrinhRequest request);
        GetMasterDataSearchQuyTrinhResponse GetMasterDataSearchQuyTrinh(GetMasterDataSearchQuyTrinhRequest request);
        GetDetailQuyTrinhResponse GetDetailQuyTrinh(GetDetailQuyTrinhRequest request);
        UpdateQuyTrinhResponse UpdateQuyTrinh(UpdateQuyTrinhRequest request);
        DeleteQuyTrinhResponse DeleteQuyTrinh(DeleteQuyTrinhRequest request);
        CheckTrangThaiQuyTrinhResponse CheckTrangThaiQuyTrinh(CheckTrangThaiQuyTrinhRequest request);
        GuiPheDuyetResponse GuiPheDuyet(GuiPheDuyetRequest request);
        PheDuyetResponse PheDuyet(PheDuyetRequest request);
        HuyYeuCauPheDuyetResponse HuyYeuCauPheDuyet(HuyYeuCauPheDuyetRequest request);
        TuChoiResponse TuChoi(TuChoiRequest request);
        GetLichSuPheDuyetResponse GetLichSuPheDuyet(GetLichSuPheDuyetRequest request);
        GetDuLieuQuyTrinhResponse GetDuLieuQuyTrinh(GetDuLieuQuyTrinhRequest request);
    }
}
