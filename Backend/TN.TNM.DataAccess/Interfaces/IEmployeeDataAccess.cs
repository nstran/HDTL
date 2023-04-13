using System.Threading.Tasks;
using TN.TNM.DataAccess.Messages.Parameters.Contract;
using TN.TNM.DataAccess.Messages.Parameters.Employee;
using TN.TNM.DataAccess.Messages.Results.Asset;
using TN.TNM.DataAccess.Messages.Results.CustomerCare;
using TN.TNM.DataAccess.Messages.Results.DataType;
using TN.TNM.DataAccess.Messages.Results.Employee;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface IEmployeeDataAccess
    {
        CreateEmployeeResult CreateEmployee(CreateEmployeeParameter parameter);
        SearchEmployeeResult SearchEmployee(SearchEmployeeParameter parameter);
        GetAllEmployeeResult GetAllEmployee(GetAllEmployeeParameter parameter);
        GetEmployeeByIdResult GetEmployeeById(GetEmployeeByIdParameter parameter);
        EditEmployeeByIdResult EditEmployeeById(EditEmployeeByIdParameter parameter);
        GetAllEmpAccountResult GetAllEmpAccount(GetAllEmpAccountParameter parameter);
        GetAllEmployeeAccountResult GetAllEmployeeAccount();
        GetAllEmpIdentityResult GetAllEmpIdentity(GetAllEmpIdentityParameter parameter);
        EditEmployeeDataPermissionResult EditEmployeeDataPermission(EditEmployeeDataPermissionParameter parameter);
        EmployeePermissionMappingResult EmployeePermissionMapping(EmployeePermissionMappingParameter parameter);
        GetEmployeeByPositionCodeResult GetEmployeeByPositionCode(GetEmployeeByPositionCodeParameter parameter);
        GetEmployeeHighLevelByEmpIdResult GetEmployeeHighLevelByEmpId(GetEmployeeHighLevelByEmpIdParameter parameter);
        GetEmployeeByOrganizationIdResult GetEmployeeByOrganizationId(GetEmployeeByOrganizationIdParameter parameter);
        GetEmployeeByTopRevenueResult GetEmployeeByTopRevenue(GetEmployeeByTopRevenueParameter parameter);
        ExportEmployeeRevenueResult ExportEmployeeRevenue(ExportEmployeeRevenueParameter parameter);
        GetStatisticForEmpDashBoardResult GetStatisticForEmpDashBoard(GetStatisticForEmpDashBoardParameter parameter);
        GetEmployeeCareStaffResult GetEmployeeCareStaff(GetEmployeeCareStaffParameter parameter);
        SearchEmployeeFromListResult SearchEmployeeFromList(SearchEmployeeFromListParameter parameter);
        GetAllEmpAccIdentityResult GetAllEmpAccIdentity(GetAllEmpAccIdentityParameter parameter);
        DisableEmployeeResult DisableEmployee(DisableEmployeeParameter parameter);
        CheckAdminLoginResult CheckAdminLogin(CheckAdminLoginParameter parameter);
        GetMasterDataEmployeeDetailResult GetMasterDataEmployeeDetail(GetMasterDataEmployeeDetailParameter parameter);
        GetThongTinCaNhanThanhVienResult GetThongTinCaNhanThanhVien(GetThongTinCaNhanThanhVienParameter parameter);
        GetThongTinChungThanhVienResult GetThongTinChungThanhVien(GetThongTinChungThanhVienParameter parameter);
        SaveThongTinChungThanhVienResult SaveThongTinChungThanhVien(SaveThongTinChungThanhVienParameter parameter);
        SaveThongTinCaNhanThanhVienResult SaveThongTinCaNhanThanhVien(SaveThongTinCaNhanThanhVienParameter parameter);
        GetCauHinhPhanQuyenResult GetCauHinhPhanQuyen(GetCauHinhPhanQuyenParameter parameter);
        SaveCauHinhPhanQuyenResult SaveCauHinhPhanQuyen(SaveCauHinhPhanQuyenParameter parameter);
        GetThongTinNhanSuResult GetThongTinNhanSu(GetThongTinNhanSuParameter parameter);
        SaveThongTinNhanSuResult SaveThongTinNhanSu(SaveThongTinNhanSuParameter parameter);
        GetThongTinLuongVaTroCapResult GetThongTinLuongVaTroCap(GetThongTinLuongVaTroCapParameter parameter);
        SaveThongTinLuongVaTroCapResult SaveThongTinLuongVaTroCap(SaveThongTinLuongVaTroCapParameter parameter);
        GetThongTinGhiChuResult GetThongTinGhiChu(GetThongTinGhiChuParameter parameter);
        ResetPasswordResult ResetPassword(ResetPasswordParameter parameter);
        GetMasterDataDashboardResult GetMasterDataDashboard(GetMasterDataDashboardParameter parameter);
        CreateRecruitmentCampaignResult CreateRecruitmentCampaign(CreateRecruitmentCampaignParameter parameter);
        GetMasterSearchRecruitmentCampaignResult GetMasterSearchRecruitmentCampaign(
            GetMasterSearchRecruitmentCampaignParameter parameter);
        DeleteRecruitmentCampaignResult DeleteRecruitmentCampaign(DeleteRecruitmentCampaignParameter parameter);
        SearchRecruitmentCampaignResult SearchRecruitmentCampaign(SearchRecruitmentCampaignParameter parameter);
        GetMasterRecruitmentCampaignDetailResult GetMasterRecruitmentCampaignDetail(
     GetMasterRecruitmentCampaignDetailParameter parameter);

        UploadFileVacanciesResult UploadFile(UploadFileParameter request);
        CreateOrUpdateVacanciesResult CreateOrUpdateVacancies(CreateOrUpdateVacanciesParameter parameter);
        CreateCandidateResult CreateCandidate(CreateCandidateParameter parameter);
        GetAllVacanciesResult GetAllVacancies(GetAllVacanciesParameter parameter);
        DeleteVacanciesByIdResult DeleteVacanciesById(DeleteVacanciesByIdParameter parameter);
        GetMasterDataRecruitmentCampaignInformationResult GetMasterDataRecruitmentCampaignInformation(GetMasterDataRecruitmentCampaignInformationParameter parameter);
        GetMasterDataBaoCaoTuyenDungResult GetMasterDataBaoCaoTuyenDung(GetMasterDataBaoCaoTuyenDungParameter parameter);
        GetBaoCaoBaoCaoTuyenDungResult GetBaoCaoBaoCaoTuyenDung(GetBaoCaoBaoCaoTuyenDungParameter parameter);
        GetMasterDataVacanciesCreateResult GetMasterDataVacanciesCreate(GetMasterDataVacanciesCreateParameter parameter);
        GetMasterDataVacanciesDetailResult GetMasterDataVacanciesDetail(GetMasterDataVacanciesDetailParameter request);
        CreateOrUpdateVacanciesResult UpdateVacancies(CreateOrUpdateVacanciesParameter parameter);
        UpdateCandidateStatusFromVacanciesResult UpdateStatusCandidateFromVacancies(UpdateCandidateStatusFromVacanciesParameter parameter);
        CreateInterviewScheduleResult CreateInterviewSchedule(CreateInterviewScheduleParameter parameter);
        UpdateCandidateResult UpdateCandidate(UpdateCandidateParameter parameter);
        DeleteCandidatesResult DeleteCandidates(DeleteCandidatesParameter parameter);
        DownloadTemplateImportResult DownloadTemplateImportCandidate(DownloadTemplateImportParameter parameter);
        GetAllVacanciesResult FilterVacancies(FilterVacanciesParameter parameter);
        GetMasterCreateCandidateResult GetMasterCreateCandidate(GetMasterCreateCandidateParameter parameter);
        GetMasterCandidateDetailResult GetMasterCandidateDetail(GetMasterCandidateDetailParameter parameter);
        DeleteCandidateResult DeleteCandidate(DeleteCandidateParameter parameter);
        UpdateCandidateStatusResult UpdateCandidateStatus(UpdateCandidateStatusParameter parameter);
        DeleteCandidateAssessmentResult DeleteCandidateAssessment(DeleteCandidateAssessmentParameter parameter);
        CreateOrUpdateCandidateAssessmentResult CreateOrUpdateCandidateAssessment(CreateOrUpdateCandidateAssessmentParameter parameter);
        CreateOrUpdateCandidateDetailInforResult CreateOrUpdateCandidateDetailInfor(CreateOrUpdateCandidateDetailInforParameter parameter);
        UpdateInterviewScheduleResult UpdateInterviewSchedule(UpdateInterviewScheduleParameter parameter);
        CreateOrUpdateQuizResult CreateOrUpdateQuiz(CreateOrUpdateQuizParameter parameter);
        GetMasterCauHinhBaoHiemResult GetMasterCauHinhBaoHiem(GetMasterCauHinhBaoHiemParameter parameter);
        CreateOrUpdateCauHinhBaoHiemResult CreateOrUpdateCauHinhBaoHiem(CreateOrUpdateCauHinhBaoHiemParameter parameter);
        CreateOrUpdateCauHinhBaoHiemLoftCareResult CreateOrUpdateCauHinhBaoHiemLoftCare(CreateOrUpdateCauHinhBaoHiemLoftCareParameter parameter);
        DeleteCauHinhBaoHiemLoftCareResult DeleteCauHinhBaoHiemLoftCare(DeleteCauHinhBaoHiemLoftCareParameter request);

        DeleteCandidateDetailInforResult DeleteCandidateDetailInfor(DeleteCandidateDetailInforParameter parameter);
        DeleteInterviewScheduleResult DeleteInterviewSchedule(DeleteInterviewScheduleParameter parameter);
        DeleteQuizResult DeleteQuiz(DeleteQuizParameter parameter);
        GetMasterSearchCandidateResult GetMasterSearchCandidate(GetMasterSearchCandidateParameter parameter);
        SearchCandidateResult SearchCandidate(SearchCandidateParameter parameter);
        SendEmailInterviewResult SendEmailInterview(SendEmailInterviewParameter parameter);
        GetCandidateImportDetaiResult GetCandidateImportDetai(GetCandidateImportDetaiParameter parameter);
        ImportListCandidateResult ImportListCandidate(ImportListCandidateParameter parameter);




        GetMasterDataCreateEmployeeResult GetMasterDataCreateEmployee(GetMasterDataCreateEmployeeParameter request);
        SaveThongTinKhacResult SaveThongTinKhac(SaveThongTinKhacParameter request);
        GetThongTinKhacResult GetThongTinKhac(GetThongTinKhacParameter request);
        GetThongTinGiaDinhResult GetThongTinGiaDinh(GetThongTinGiaDinhParameter request);
        CreateOrUpdateThongTinGiaDinhResult CreateOrUpdateThongTinGiaDinh(CreateOrUpdateThongTinGiaDinhParameter request);
        GetCheckListTaiLieuResult GetCheckListTaiLieu(GetCheckListTaiLieuParameter request);
        GetMasterDataCreateDeXuatTangLuongResult GetMasterDataCreateDeXuatTangLuong(GetMasterDataCreateDeXuatTangLuongParameter request);
        TaoDeXuatTangLuongResult TaoDeXuatTangLuong(TaoDeXuatTangLuongParameter request);
        CreateHopDongNhanSuResult CreateHopDongNhanSu(CreateHopDongNhanSuParameter parameter);
        
        GetHopDongNhanSuByIdResult GetHopDongNhanSuById(GetHopDongNhanSuByIdParameter parameter);
        GetListHopDongNhanSuResult GetListHopDongNhanSu(GetListHopDongNhanSuParameter parameter);
        UpdateHopDongNhanSuResult UpdateHopDongNhanSu(UpdateHopDongNhanSuParameter parameter);
        DeleteHopDongNhanSuByIdResult DeleteHopDongNhanSuById(DeleteHopDongNhanSuByIdParameter parameter);
        GetLichSuHopDongNhanSuResult GetLichSuHopDongNhanSu(GetLichSuHopDongNhanSuParameter parameter);
        DeXuatTangLuongDetailResult DeXuatTangLuongDetail(DeXuatTangLuongDetailParameter parameter);
        ListDeXuatTangLuongResult ListDeXuatTangLuong(ListDeXuatTangLuongParameter parameter);
        GetLichSuThayDoiChucVuResult GetLichSuThayDoiChucVu(GetLichSuThayDoiChucVuParameter parameter);
        DeleteDeXuatTangLuongResult DeleteDeXuatTangLuong(DeleteDeXuatTangLuongParameter parameter);
        GetThongTinBaoHiemVaThueResult GetThongTinBaoHiemVaThue(GetThongTinBaoHiemVaThueParameter parameter);
        GetCauHinhBaoHiemLoftCareByIdResult GetCauHinhBaoHiemLoftCareById(GetCauHinhBaoHiemLoftCareByIdParameter parameter);
        GetCauHinhBaoHiemByIdResult GetCauHinhBaoHiemById(GetCauHinhBaoHiemByIdParameter parameter);
        SaveThongTinBaoHiemVaThueResult SaveThongTinBaoHiemVaThue(SaveThongTinBaoHiemVaThueParameter parameter);
        GetListTaiLieuNhanVienResult GetListTaiLieuNhanVien(GetListTaiLieuNhanVienParameter parameter);
        CreateTaiLieuNhanVienResult CreateTaiLieuNhanVien(CreateTaiLieuNhanVienParameter parameter);
        TuChoiCheckListTaiLieuResult TuChoiCheckListTaiLieu(TuChoiCheckListTaiLieuParameter parameter);
        XacNhanCheckListTaiLieuResult XacNhanCheckListTaiLieu(XacNhanCheckListTaiLieuParameter parameter);

        YeuCauXacNhanCheckListTaiLieuResult YeuCauXacNhanCheckListTaiLieu(
            YeuCauXacNhanCheckListTaiLieuParameter parameter);

        DeleteTaiLieuNhanVienByIdResult DeleteTaiLieuNhanVienById(DeleteTaiLieuNhanVienByIdParameter parameter);
        UpdateTaiLieuNhanVienResult UpdateTaiLieuNhanVien(UpdateTaiLieuNhanVienParameter parameter);
        GetMasterCreateKeHoachOtResult GetMasterCreateKeHoachOt(GetMasterCreateKeHoachOtParameter parameter);
        CreateOrUpdateKeHoachOtResult CreateOrUpdateKeHoachOt(CreateOrUpdateKeHoachOtParameter parameter);
        GetMasterSearchKeHoachOtResult GetMasterSearchKeHoachOt(GetMasterSearchKeHoachOtParameter parameter);
        GetTrinhDoHocVanTuyenDungResult GetTrinhDoHocVanTuyenDung(GetTrinhDoHocVanTuyenDungParameter parameter);
        GetMasterDataKeHoachOtDetailResult GetMasterDataKeHoachOtDetail(GetMasterDataKeHoachOtDetailParameter parameter);
        GetMasterDataKeHoachOtPheDuyetResult GetMasterDataKeHoachOtPheDuyet(GetMasterDataKeHoachOtPheDuyetParameter parameter);

        UpdateTrinhDoHocVanTuyenDungResult
            UpdateTrinhDoHocVanTuyenDung(UpdateTrinhDoHocVanTuyenDungParameter parameter);

        DeleteThongTinGiaDinhByIdResult DeleteThongTinGiaDinhById(DeleteThongTinGiaDinhByIdParameter parameter);
        UpdateDeXuatTangLuongResult UpdateDeXuatTangLuong(UpdateDeXuatTangLuongParameter parameter);
        DatVeMoiDeXuatTangLuongResult DatVeMoiDeXuatTangLuong(DatVeMoiDeXuatTangLuongParameter parameter);
        TuChoiOrPheDuyetNhanVienDeXuatTLResult TuChoiOrPheDuyetNhanVienDeXuatTL(TuChoiOrPheDuyetNhanVienDeXuatTLParameter parameter);
        DownloadTemplateImportDXTLResult DownloadTemplateImportDXTL(DownloadTemplateImportDXTLParameter parameter);
        GetMasterDataCreateDeXuatChucVuResult GetMasterDataCreateDeXuatChucVu(GetMasterDataCreateDeXuatChucVuParameter parameter);
        TaoDeXuatChucVuResult TaoDeXuatChucVu(TaoDeXuatChucVuParameter parameter);
        DownloadTemplateImportDXCVResult DownloadTemplateImportDXCV(DownloadTemplateImportDXCVParameter parameter);
        ListDeXuatChucVuResult ListDeXuatChucVu(ListDeXuatChucVuParameter parameter);
        DeleteDeXuatChucVuResult DeleteDeXuatChucVu(DeleteDeXuatChucVuParameter parameter);
        DeXuatChucVuDetailResult DeXuatChucVuDetail(DeXuatChucVuDetailParameter parameter);
        DatVeMoiDeXuatChucVuResult DatVeMoiDeXuatChucVu(DatVeMoiDeXuatChucVuParameter parameter);
        TuChoiOrPheDuyetNhanVienDeXuatCVResult TuChoiOrPheDuyetNhanVienDeXuatCV(TuChoiOrPheDuyetNhanVienDeXuatCVParameter parameter);
        UpdateDeXuatChucVuResult UpdateDeXuatChucVu(UpdateDeXuatChucVuParameter parameter);

        //GetMasterCreateOrUpdateDeXuatCongTacResult GetMasterCreateOrUpdateDeXuatCongTac(GetMasterCreateOrUpdateDeXuatCongTacParameter parameter);
        //CreateOrUpdateDeXuatCongTacResult CreateOrUpdateDeXuatCongTac(CreateOrUpdateDeXuatCongTacParameter parameter);

        //GetListDeXuatCongTacResult GetListDeXuatCongTac(GetListDeXuatCongTacParameter parameter);
        //DeleteDeXuatCongTacResult DeleteDeXuatCongTac(DeleteDeXuatCongTacParameter request);

        #region QUẢN LÝ CÔNG TÁC
        GetMasterDataHoSoCTFormResult GetMasterDataHoSoCTForm(GetMasterDataHoSoCTFormParameter parameter);
        CreateOrUpdateHoSoCTResult CreateOrUpdateHoSoCT(CreateOrUpdateHoSoCTParameter parameter);
        GetAllHoSoCongTacListResult GetAllHoSoCongTacList(GetAllHoSoCongTacListParameter parameter);
        XoaHoSoCongTacResult XoaHoSoCongTac(XoaHoSoCongTacParameter parameter);
        GetDataHoSoCTDetailResult GetDataHoSoCTDetail( GetDataDetailParameter parameter);
        GetMasterDataDeNghiFormResult GetMasterDataDeNghiForm();
        GetDataDeNghiDetailFormResult GetDataDeNghiDetailForm(GetDataDetailParameter parameter);
        CreateOrUpdateDeNghiTamHoanUngResult CreateOrUpdateDeNghiTamHoanUng(CreateOrUpdateDeNghiTamHoanUngParameter parameter);
        UpdateDeNghiTamHoanUngChiTietResult UpdateDeNghiTamHoanUngChiTiet(UpdateDeNghiTamHoanUngChiTietParameter parameter);
        DeleteDeNghiTamHoanUngChiTietResult DeleteDeNghiTamHoanUngChiTiet(DeleteDeNghiTamHoanUngChiTietParameter parameter);
        DatVeMoiDeNghiTamHoanUngResult DatVeMoiDeNghiTamHoanUng(DatVeMoiDeNghiTamHoanUngParameter parameter);
        XoaDeNghiTamHoanUngResult XoaDeNghiTamHoanUng(XoaDeNghiTamHoanUngParameter parameter);
        GetAllDenghiTamUngListResult GetAllDenghiTamUngList(GetAllDenghiTamUngListParameter parameter);
        GetMasterDataDeXuatCongTacResult GetMasterDataDeXuatCongTac(GetMasterDataHoSoCTFormParameter parameter);
        GetMasterDeXuatCongTacDetailResult GetMasterDeXuatCongTacDetail(GetMasterDeXuatCongTacDetailParameter parameter);
        DeleteDeXuatCongTacResult DatVeMoiDeXuatCongTac(DeleteDeXuatCongTacParameter parameter);
        #endregion

        GetMasterKeHoachOTDetailResult GetMasterKeHoachOTDetail(GetMasterKeHoachOTDetailParameter parameter);
        DatVeMoiKeHoachOtResult DatVeMoiKeHoachOt(DatVeMoiKeHoachOtParameter parameter);
        DangKyOTOrHuyDangKyOTResult DangKyOTOrHuyDangKyOT(DangKyOTOrHuyDangKyOTParameter parameter);
        PheDuyetNhanSuDangKyOTResult PheDuyetNhanSuDangKyOT(PheDuyetNhanSuDangKyOTParameter parameter);
        TuChoiNhanVienOTTongPheDuyetResult TuChoiNhanVienOTTongPheDuyet(TuChoiNhanVienOTTongPheDuyetParameter parameter);
        GiaHanThemKeHoachOTResult GiaHanThemKeHoachOT(GiaHanThemKeHoachOTParameter parameter);
        DeleteKehoachOTResult DeleteKehoachOT(DeleteKehoachOTParameter parameter);
        GetMasterDataCreateCauHinhDanhGiaResult GetMasterDataCreateCauHinhDanhGia(GetMasterDataCreateCauHinhDanhGiaParameter parameter);
        UpdateCauHinhDanhGiaResult UpdateCauHinhDanhGia(UpdateCauHinhDanhGiaParameter parameter);
        DeleteCauHinhDanhGiaResult DeleteCauHinhDanhGia(DeleteCauHinhDanhGiaParameter parameter);
        CreateCauHinhDanhGiaResult CreateCauHinhDanhGia(CreateCauHinhDanhGiaParameter parameter);
        TaoQuyLuongResult TaoQuyLuong(TaoQuyLuongParameter parameter);
        TaoQuyLuongResult UpdateQuyLuong(TaoQuyLuongParameter parameter);
        TaoQuyLuongResult DeleteQuyLuong(TaoQuyLuongParameter parameter);
        GetListCauHinhChecklistResult GetListCauHinhChecklist(GetListCauHinhChecklistParameter parameter);
        CreateOrUpdateCauHinhChecklistResult CreateOrUpdateCauHinhChecklist(CreateOrUpdateCauHinhChecklistParameter parameter);
        DeleteCauHinhChecklistByIdResult DeleteCauHinhChecklistById(DeleteCauHinhChecklistByIdParameter parameter);
        GetMasterDataCreatePhieuDanhGiaResult GetMasterDataCreatePhieuDanhGia(GetMasterDataCreatePhieuDanhGiaParameter parameter);
        TaoPhieuDanhGiaResult TaoPhieuDanhGia(TaoPhieuDanhGiaParameter parameter);
        DanhSachPhieuDanhGiaResult DanhSachPhieuDanhGia(DanhSachPhieuDanhGiaParameter parameter);
        DeletePhieuDanhGiaResult DeletePhieuDanhGia(DeletePhieuDanhGiaParameter parameter);
        CreateOrUpdateDeXuatCTResult CreateOrUpdateDeXuatCT(CreateOrUpdateDeXuatCTParameter parameter);
        XoaYeuCauCapPhatResult XoaDeXuatCongTac(XoaDeXuatCongTacParameter parameter);
        GetAllDeXuatCongTacResult GetAllDeXuatCongTac(GetAllDeXuatCongTacParameter request);
        DanhSachKyDanhGiaResult DanhSachKyDanhGia(DanhSachKyDanhGiaParameter request);
        PhieuDanhGiaDetailResult PhieuDanhGiaDetail(PhieuDanhGiaDetailParameter request);
        CapNhatPhieuDanhGiaResult CapNhatPhieuDanhGia(CapNhatPhieuDanhGiaParameter request);
        HoanThanhOrUpdateStatusPhieuDanhGiaResult HoanThanhOrUpdateStatusPhieuDanhGia(HoanThanhOrUpdateStatusPhieuDanhGiaParameter request);
        GetMasterDataTaoKyDanhGiaResult GetMasterDataTaoKyDanhGia(GetMasterDataTaoKyDanhGiaParameter request);
        CreateOrUpdateChiTietDeXuatCongTacResult CreateOrUpdateChiTietDeXuatCongTac(CreateOrUpdateChiTietDeXuatCongTacParameter parameter);
        XoaYeuCauCapPhatResult XoaDeXuatCongTacChiTiet(XoaDeXuatCongTacChiTietParameter parameter);
        GetListCapPhatTaiSanResult GetListCapPhatTaiSan(GetListCapPhatTaiSanParameter request);
        CapNhatLyDoPheDuyetOrTuChoiDeXuatNVResult CapNhatLyDoPheDuyetOrTuChoiDeXuatNV(CapNhatLyDoPheDuyetOrTuChoiDeXuatNVParameter parameter);
        CapNhatNgayApDungDeXuatResult CapNhatNgayApDungDeXuat(CapNhatNgayApDungDeXuatParameter parameter);
        TaoKyDanhGiaResult TaoKyDanhGia(TaoKyDanhGiaParameter parameter);
        DeleteKyDanhGiaResult DeleteKyDanhGia(DeleteKyDanhGiaParameter parameter);
        KyDanhGiaDetailResult KyDanhGiaDetail(KyDanhGiaDetailParameter parameter);
        XoaPhieuDanhGiaCuaKyResult XoaPhieuDanhGiaCuaKy(XoaPhieuDanhGiaCuaKyParameter parameter);
        LuuPhieuDanhGiaCuaKyResult LuuPhieuDanhGiaCuaKy(LuuPhieuDanhGiaCuaKyParameter parameter);
        CapNhatKyDanhGiaResult CapNhatKyDanhGia(CapNhatKyDanhGiaParameter parameter);
        XoaPhongBanKyDanhGiaResult XoaPhongBanKyDanhGia(XoaPhongBanKyDanhGiaParameter parameter);
        CreateOrAddPhongBanKyDanhGiaResult CreateOrAddPhongBanKyDanhGia(CreateOrAddPhongBanKyDanhGiaParameter parameter);
        HoanThanhkyDanhGiaResult HoanThanhkyDanhGia(HoanThanhkyDanhGiaParameter parameter);
        UpdateNguoiDanhGiaNhanVienKyResult UpdateNguoiDanhGiaNhanVienKy(UpdateNguoiDanhGiaNhanVienKyParameter parameter);
        TaoPhieuTuDanhGiaNhanVienResult TaoPhieuTuDanhGiaNhanVien(TaoPhieuTuDanhGiaNhanVienParameter parameter);
        ThucHienDanhGiaDetailResult ThucHienDanhGiaDetail(ThucHienDanhGiaDetailParameter parameter);
        LuuOrHoanThanhDanhGiaResult LuuOrHoanThanhDanhGia(LuuOrHoanThanhDanhGiaParameter parameter);
        GanMucDanhGiaChungResult GanMucDanhGiaChung(GanMucDanhGiaChungParameter parameter);
        CapNhatDanhGiaNhanVienRowResult CapNhatDanhGiaNhanVienRow(CapNhatDanhGiaNhanVienRowParameter parameter);
        TaoDeXuatTangLuongKyDanhGiaResult TaoDeXuatTangLuongKyDanhGia(TaoDeXuatTangLuongKyDanhGiaParameter parameter);
        CreateOrUpdateLichSuThanhToanBaoHiemResult CreateOrUpdateLichSuThanhToanBaoHiem(CreateOrUpdateLichSuThanhToanBaoHiemParameter parameter);
        DeleteLichSuThanhToanBaoHiemByIdResult DeleteLichSuThanhToanBaoHiemById(DeleteLichSuThanhToanBaoHiemByIdParameter parameter);
        GetAllVacanciesForOtherResult GetAllVacanciesForOther();

        GetMasterDateImportEmployeeResult GetMasterDateImportEmployee(GetMasterDateImportEmployeeParameter parameter);
        ImportEmployeeResult ImportEmployee(ImportEmployeeParameter parameter);
        DownloadTemplateImportResult DownloadTemplateImportEmployee(DownloadTemplateImportParameter parameter);
        GetDataDashboardHomeResult GetDataDashboardHome(GetDataDashboardHomeParameter parameter);
        SynchronizeCandidateDataFromCMSResult SynchronizeCandidateDataFromCMS(SynchronizeCandidateDataFromCMSParameter parameter);
        DashboardHomeViewDetailResult DashboardHomeViewDetail(DashboardHomeViewDetailParameter parameter);

        GetListBaoCaoNhanSuResult GetListBaoCaoNhanSu(GetListBaoCaoNhanSuParameter parameter);
        GetBieuDoThongKeNhanSuResult GetBieuDoThongKeNhanSu(GetBieuDoThongKeNhanSuParameter parameter);
        GiaHanPheDuyetKeHoachOTResult GiaHanPheDuyetKeHoachOT(GiaHanPheDuyetKeHoachOTParameter parameter);
        HoanThanhHoSoCongTacResult HoanThanhHoSoCongTac(HoanThanhHoSoCongTacParameter parameter);
        SaveGhiChuNhanVienKeHoachOTResult SaveGhiChuNhanVienKeHoachOT(SaveGhiChuNhanVienKeHoachOTParameter parameter);
        LayNhanVienCungCapVaCapDuoiOrgResult LayNhanVienCungCapVaCapDuoiOrg(LayNhanVienCungCapVaCapDuoiOrgParameter parameter);
        HoanThanhDanhGiaPhongBanResult HoanThanhDanhGiaPhongBan(HoanThanhDanhGiaPhongBanParameter parameter);
        DownloadTemplateImportHDNSResult DownloadTemplateImportHDNS(DownloadTemplateImportHDNSParameter parameter);
        GetMasterDateImportHDNSResult GetMasterDateImportHDNS(GetMasterDateImportHDNSParameter parameter);
        ImportHDNSResult ImportHDNS(ImportHDNSParameter parameter);
        Task<GetBenefitType> GetBenefitType();
        TakeListEmployeeByUserNameResult TakeListEmployeeByUserName(TakeListEmployeeByUserNameParameter parameter);
        TakeListOrganizationToFilterEmployeeResult TakeListOrganizationToFilterEmployee(TakeListOrganizationToFilterEmployeeParameter parameter);
        DeleteEmployeeResult DeleteEmployee(DeleteEmployeeParameter parameter);
        Task<TakeListEvaluateResult> TakeListEvaluateForObjectId(TakeListEvaluateParameter parameter);
    }
}
