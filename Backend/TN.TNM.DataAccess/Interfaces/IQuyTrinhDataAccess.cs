

using TN.TNM.DataAccess.Messages.Parameters.QuyTrinh;
using TN.TNM.DataAccess.Messages.Results.QuyTrinh;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IQuyTrinhDataAccess
    {
        CreateQuyTrinhResult CreateQuyTrinh(CreateQuyTrinhParameter parameter);
        SearchQuyTrinhResult SearchQuyTrinh(SearchQuyTrinhParameter parameter);
        GetMasterDataSearchQuyTrinhResult GetMasterDataSearchQuyTrinh(GetMasterDataSearchQuyTrinhParameter parameter);
        GetDetailQuyTrinhResult GetDetailQuyTrinh(GetDetailQuyTrinhParameter parameter);
        UpdateQuyTrinhResult UpdateQuyTrinh(UpdateQuyTrinhParameter parameter);
        DeleteQuyTrinhResult DeleteQuyTrinh(DeleteQuyTrinhParameter parameter);
        CheckTrangThaiQuyTrinhResult CheckTrangThaiQuyTrinh(CheckTrangThaiQuyTrinhParameter parameter);
        GuiPheDuyetResult GuiPheDuyet(GuiPheDuyetParameter parameter);
        PheDuyetResult PheDuyet(PheDuyetParameter parameter);
        HuyYeuCauPheDuyetResult HuyYeuCauPheDuyet(HuyYeuCauPheDuyetParameter parameter);
        TuChoiResult TuChoi(TuChoiParameter parameter);
        GetLichSuPheDuyetResult GetLichSuPheDuyet(GetLichSuPheDuyetParameter parameter);
        GetDuLieuQuyTrinhResult GetDuLieuQuyTrinh(GetDuLieuQuyTrinhParameter parameter);
        GetMasterDataCreateQuyTrinhResult GetMasterDataCreateQuyTrinh(GetMasterDataCreateQuyTrinhParameter parameter);
        CheckUpdateQuyTrinhResult CheckUpdateQuyTrinh(CheckUpdateQuyTrinhParameter parameter);
    }
}
