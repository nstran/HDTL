using System;
using TN.TNM.DataAccess.Messages.Parameters.Asset;
using TN.TNM.DataAccess.Messages.Parameters.Contract;
using TN.TNM.DataAccess.Messages.Parameters.Employee;
using TN.TNM.DataAccess.Messages.Results.Asset;
using TN.TNM.DataAccess.Messages.Results.Employee;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IAssetDataAccess
    {
        GetMasterDataAssetFormResult GetMasterDataAssetForm(GetMasterDataAssetFormParameter parameter);
        CreateOrUpdateAssetResult CreateOrUpdateAsset(CreateOrUpdateAssetParameter request);
        GetAllAssetListResult GetAllAssetList(GetAllAssetListParameter parameter);
        GetMasterDataPhanBoTSFormResult GetMasterDataPhanBoTSForm(GetMasterDataAssetFormParameter request);
        TaoPhanBoTaiSanResult TaoPhanBoTaiSan(TaoPhanBoTaiSanParameter request);
        GetMasterDataPhanBoTSFormResult GetMasterDataThuHoiTSForm(GetMasterDataAssetFormParameter request);
        TaoPhanBoTaiSanResult TaoThuHoiTaiSan(TaoPhanBoTaiSanParameter request);
        CreateOrUpdateBaoDuongResult CreateOrUpdateBaoDuong(CreateOrUpdateBaoDuongParameter request);
        DeleteBaoDuongResult DeleteBaoDuong(DeleteBaoDuongParameter request);
        GetDataAssetDetailResult GetDataAssetDetail(GetDataAssetDetailParameter request);
        UploadFileVacanciesResult UploadFile(UploadFileAssetParameter request);
        DownloadTemplateAssetResult DownloadTemplateAsset(DownloadTemplateAssetParameter request);
        GetMasterDataPhanBoTSFormResult GetMasterDataYeuCauCapPhatForm(GetMasterDataAssetFormParameter request);
        CreateOrYeuCauCapPhatResult CreateOrYeuCauCapPhat(CreateOrYeuCauCapPhatParameter request);
        GetAllYeuCauCapPhatTSListResult GetAllYeuCauCapPhatTSList(GetAllYeuCauCapPhatTSListParameter request);
        XoaYeuCauCapPhatResult XoaYeuCauCapPhat(XoaYeuCauCapPhatParameter request);
        GetDataYeuCauCapPhatDetailResult GetDataYeuCauCapPhatDetail(GetDataYeuCauCapPhatDetailParameter request);
        DeleteChiTietYeuCauCapPhatResult DeleteChiTietYeuCauCapPhat(DeleteChiTietYeuCauCapPhatParameter request);
        CreateOrUpdateChiTietYeuCauCapPhatResult CreateOrUpdateChiTietYeuCauCapPhat(CreateOrUpdateChiTietYeuCauCapPhatParameter request);
        DatVeMoiYeuCauCapPhatTSResult DatVeMoiYeuCauCapPhatTS(DatVeMoiYeuCauCapPhatTSParameter request);
        CapNhapTrangThaiYeuCauCapPhatResult CapNhapTrangThaiYeuCauCapPhat(CapNhapTrangThaiYeuCauCapPhatParameter request);
        BaoCaoPhanBoResult BaoCaoPhanBo(BaoCaoPhanBoParameter parameter);
        BaoCaoKhauHaoResult BaoCaoKhauHao(BaoCaoKhauHaoParameter parameter);
        BaoCaoPhanBoResult GetMasterDataBaoCaoPhanBo(BaoCaoPhanBoParameter parameter);
        ImportAssetResult ImportAsset(ImportAssetParameter parameter);
        DownloadTemplateImportResult DownloadTemplateImportAsset(DownloadTemplateImportParameter parameter);
        DotKiemKeSearchResult DotKiemKeSearch(DotKiemKeSearchParameter parameter);
        TaoDotKiemKeResult TaoDotKiemKe(TaoDotKiemKeParameter parameter);
        DeleteDotKiemKeResult DeleteDotKiemKe(DeleteDotKiemKeParameter parameter);
        DotKiemKeDetailResult DotKiemKeDetail(DotKiemKeDetailParameter parameter);
        UpdateKhauHaoMobileResult UpdateKhauHaoMobile(UpdateKhauHaoMobileParameter parameter);
        AddTaiSanToDotKiemKeResult AddTaiSanToDotKiemKe(AddTaiSanToDotKiemKeParameter parameter);
        GetMasterDataAddTaiSanVaoDotKiemKeResult GetMasterDataAddTaiSanVaoDotKiemKe(GetMasterDataAddTaiSanVaoDotKiemKeParameter parameter);


    }
}
