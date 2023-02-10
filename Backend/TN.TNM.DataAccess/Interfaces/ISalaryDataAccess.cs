using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Messages.Parameters.Salary;
using TN.TNM.DataAccess.Messages.Results.Salary;

namespace TN.TNM.DataAccess.Interfaces
{
    public interface ISalaryDataAccess
    {
        GetMasterCauHinhLuongResult GetMasterCauHinhLuong(GetMasterCauHinhLuongParameter parameter);
        CreateOrUpdateCaLamViecResult CreateOrUpdateCaLamViec(CreateOrUpdateCaLamViecParameter parameter);
        DeleteCauHinhCaLamViecResult DeleteCauHinhCaLamViec(DeleteCauHinhCaLamViecParameter parameter);
        CreateOrUpdateCauHinhNghiLeResult CreateOrUpdateCauHinhNghiLe(CreateOrUpdateCauHinhNghiLeParameter parameter);
        DeleteCauHinhNghiLeResult DeleteCauHinhNghiLe(DeleteCauHinhNghiLeParameter parameter);
        CreateOrUpdateCauHinhOtResult CreateOrUpdateCauHinhOt(CreateOrUpdateCauHinhOtParameter parameter);
        DeleteCauHinhOtResult DeleteCauHinhOt(DeleteCauHinhOtParameter parameter);
        CreateOrUpdateCauHinhGiamTruResult CreateOrUpdateCauHinhGiamTru(CreateOrUpdateCauHinhGiamTruParameter parameter);
        DeleteCauHinhGiamTruResult DeleteCauHinhGiamTru(DeleteCauHinhGiamTruParameter parameter);
        CreateOrUpdateKinhPhiResult CreateOrUpdateKinhPhi(CreateOrUpdateKinhPhiParameter parameter);
        CreateOrUpdateCongThucTinhLuongResult CreateOrUpdateCongThucTinhLuong(CreateOrUpdateCongThucTinhLuongParameter parameter);
        GetTkDiMuonVeSomResult GetTkDiMuonVeSom(GetTkDiMuonVeSomParameter parameter);
        ImportChamCongResult ImportChamCong(ImportChamCongParameter parameter);
        GetMasterThongKeChamCongResult GetMasterThongKeChamCong(GetMasterThongKeChamCongParameter parameter);
        CreateOrUpdateChamCongResult CreateOrUpdateChamCong(CreateOrUpdateChamCongParameter parameter);
        CreateOrUpdateCauHinhChamCongOtResult CreateOrUpdateCauHinhChamCongOt(
            CreateOrUpdateCauHinhChamCongOtParameter parameter);
        GetTkThoiGianOtResult GetTkThoiGianOt(GetTkThoiGianOtParameter parameter);
        CreateOrUpdateCauHinhOtCaNgayResult CreateOrUpdateCauHinhOtCaNgay(
            CreateOrUpdateCauHinhOtCaNgayParameter parameter);
        CreateOrUpdateCauHinhThueTncnResult CreateOrUpdateCauHinhThueTncn(
            CreateOrUpdateCauHinhThueTncnParameter parameter);
        DeleteCauHinhThueTncnResult DeleteCauHinhThueTncn(DeleteCauHinhThueTncnParameter parameter);
        GetChamCongOtByEmpIdResult GetChamCongOtByEmpId(GetChamCongOtByEmpIdParameter parameter);
        CreateOrUpdateChamCongOtResult CreateOrUpdateChamCongOt(CreateOrUpdateChamCongOtParameter parameter);
        GetMasterDataTroCapResult GetMasterDataTroCap(GetMasterDataTroCapParameter parameter);
        CreateOrUpdateTroCapResult CreateOrUpdateTroCap(CreateOrUpdateTroCapParameter parameter);
        DeleteTroCapResult DeleteTroCap(DeleteTroCapParameter parameter);
        GetListKyLuongResult GetListKyLuong(GetListKyLuongParameter parameter);
        CreateOrUpdateKyLuongResult CreateOrUpdateKyLuong(CreateOrUpdateKyLuongParameter parameter);
        DeleteKyLuongResult DeleteKyLuong(DeleteKyLuongParameter parameter);
        DatVeMoiKyLuongResult DatVeMoiKyLuong(DatVeMoiKyLuongParameter parameter);
        GetKyLuongByIdResult GetKyLuongById(GetKyLuongByIdParameter parameter);
        SaveThuNhapTinhThueResult SaveThuNhapTinhThue(SaveThuNhapTinhThueParameter parameter);
        SaveBaoHiemResult SaveBaoHiem(SaveBaoHiemParameter parameter);
        SaveGiamTruTruocThueResult SaveGiamTruTruocThue(SaveGiamTruTruocThueParameter parameter);
        SaveGiamTruSauThueResult SaveGiamTruSauThue(SaveGiamTruSauThueParameter parameter);
        SaveHoanLaiSauThueResult SaveHoanLaiSauThue(SaveHoanLaiSauThueParameter parameter);
        SaveCtyDongResult SaveCtyDong(SaveCtyDongParameter parameter);
        SaveOtherResult SaveOther(SaveOtherParameter parameter);
        GetListDieuKienTroCapCoDinhResult GetListDieuKienTroCapCoDinh(GetListDieuKienTroCapCoDinhParameter parameter);
        UpdateDieuKienTroCapCoDinhResult UpdateDieuKienTroCapCoDinh(UpdateDieuKienTroCapCoDinhParameter parameter);
        SaveLuongCtTroCapKhacResult SaveLuongCtTroCapKhac(SaveLuongCtTroCapKhacParameter parameter);
        GetListDieuKienTroCapKhacResult GetListDieuKienTroCapKhac(GetListDieuKienTroCapKhacParameter parameter);
        UpdateDieuKienTroCapKhacResult UpdateDieuKienTroCapKhac(UpdateDieuKienTroCapKhacParameter parameter);
        UpdateMucTroCapKhacResult UpdateMucTroCapKhac(UpdateMucTroCapKhacParameter parameter);
        ImportTroCapKhacResult ImportTroCapKhac(ImportTroCapKhacParameter parameter);
        ImportLuongChiTietResult ImportLuongChiTiet(ImportLuongChiTietParameter parameter);
        HoanThanhKyLuongResult HoanThanhKyLuong(HoanThanhKyLuongParameter parameter);
        GetListPhieuLuongResult GetListPhieuLuong(GetListPhieuLuongParameter parameter);
        GetPhieuLuongByIdResult GetPhieuLuongById(GetPhieuLuongByIdParameter parameter);
        ExportBaoCaoKyLuongResult ExportBaoCaoKyLuong(ExportBaoCaoKyLuongParameter parameter);
        CapNhatBangLuongResult CapNhatBangLuong(CapNhatBangLuongParameter parameter);
        ExportExcelKyLuongMealAllowanceResult ExportExcelKyLuongMealAllowance(ExportExcelKyLuongMealAllowanceParameter parameter);
    }
}
