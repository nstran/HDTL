using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Salary;
using TN.TNM.DataAccess.Messages.Results.Salary;

namespace TN.TNM.Api.Controllers
{
    public class SalaryController : Controller
    {
        private readonly ISalaryDataAccess _iSalaryDataAccess;

        public SalaryController(ISalaryDataAccess iSalaryDataAccess)
        {
            _iSalaryDataAccess = iSalaryDataAccess;
        }

        [HttpPost]
        [Route("api/salary/getMasterCauHinhLuong")]
        [Authorize(Policy = "Member")]
        public GetMasterCauHinhLuongResult GetMasterCauHinhLuong([FromBody] GetMasterCauHinhLuongParameter request)
        {
            return this._iSalaryDataAccess.GetMasterCauHinhLuong(request);
        }

        [HttpPost]
        [Route("api/salary/createOrUpdateCaLamViec")]
        [Authorize(Policy = "Member")]
        public CreateOrUpdateCaLamViecResult CreateOrUpdateCaLamViec([FromBody] CreateOrUpdateCaLamViecParameter request)
        {
            return this._iSalaryDataAccess.CreateOrUpdateCaLamViec(request);
        }

        [HttpPost]
        [Route("api/salary/deleteCauHinhCaLamviec")]
        [Authorize(Policy = "Member")]
        public DeleteCauHinhCaLamViecResult DeleteCauHinhCaLamViec([FromBody] DeleteCauHinhCaLamViecParameter request)
        {
            return this._iSalaryDataAccess.DeleteCauHinhCaLamViec(request);
        }

        [HttpPost]
        [Route("api/salary/createOrUpdateCauHinhNghiLe")]
        [Authorize(Policy = "Member")]
        public CreateOrUpdateCauHinhNghiLeResult CreateOrUpdateCauHinhNghiLe([FromBody] CreateOrUpdateCauHinhNghiLeParameter request)
        {
            return this._iSalaryDataAccess.CreateOrUpdateCauHinhNghiLe(request);
        }

        [HttpPost]
        [Route("api/salary/deleteCauHinhNghiLe")]
        [Authorize(Policy = "Member")]
        public DeleteCauHinhNghiLeResult DeleteCauHinhNghiLe([FromBody] DeleteCauHinhNghiLeParameter request)
        {
            return this._iSalaryDataAccess.DeleteCauHinhNghiLe(request);
        }

        [HttpPost]
        [Route("api/salary/createOrUpdateCauHinhOt")]
        [Authorize(Policy = "Member")]
        public CreateOrUpdateCauHinhOtResult CreateOrUpdateCauHinhOt([FromBody] CreateOrUpdateCauHinhOtParameter request)
        {
            return this._iSalaryDataAccess.CreateOrUpdateCauHinhOt(request);
        }

        [HttpPost]
        [Route("api/salary/deleteCauHinhOt")]
        [Authorize(Policy = "Member")]
        public DeleteCauHinhOtResult DeleteCauHinhOt([FromBody] DeleteCauHinhOtParameter request)
        {
            return this._iSalaryDataAccess.DeleteCauHinhOt(request);
        }

        [HttpPost]
        [Route("api/salary/createOrUpdateCauHinhGiamTru")]
        [Authorize(Policy = "Member")]
        public CreateOrUpdateCauHinhGiamTruResult CreateOrUpdateCauHinhGiamTru([FromBody] CreateOrUpdateCauHinhGiamTruParameter request)
        {
            return this._iSalaryDataAccess.CreateOrUpdateCauHinhGiamTru(request);
        }

        [HttpPost]
        [Route("api/salary/deleteCauHinhGiamTru")]
        [Authorize(Policy = "Member")]
        public DeleteCauHinhGiamTruResult DeleteCauHinhGiamTru([FromBody] DeleteCauHinhGiamTruParameter request)
        {
            return this._iSalaryDataAccess.DeleteCauHinhGiamTru(request);
        }

        [HttpPost]
        [Route("api/salary/createOrUpdateKinhPhi")]
        [Authorize(Policy = "Member")]
        public CreateOrUpdateKinhPhiResult CreateOrUpdateKinhPhi([FromBody] CreateOrUpdateKinhPhiParameter request)
        {
            return this._iSalaryDataAccess.CreateOrUpdateKinhPhi(request);
        }

        [HttpPost]
        [Route("api/salary/createOrUpdateCongThucTinhLuong")]
        [Authorize(Policy = "Member")]
        public CreateOrUpdateCongThucTinhLuongResult CreateOrUpdateCongThucTinhLuong([FromBody] CreateOrUpdateCongThucTinhLuongParameter request)
        {
            return this._iSalaryDataAccess.CreateOrUpdateCongThucTinhLuong(request);
        }

        [HttpPost]
        [Route("api/salary/getTkDiMuonVeSom")]
        [Authorize(Policy = "Member")]
        public GetTkDiMuonVeSomResult GetTkDiMuonVeSom([FromBody] GetTkDiMuonVeSomParameter request)
        {
            return this._iSalaryDataAccess.GetTkDiMuonVeSom(request);
        }

        [HttpPost]
        [Route("api/salary/importChamCong")]
        [Authorize(Policy = "Member")]
        public ImportChamCongResult ImportChamCong([FromBody] ImportChamCongParameter request)
        {
            return this._iSalaryDataAccess.ImportChamCong(request);
        }
        
        [HttpPost]
        [Route("api/salary/getMasterThongKeChamCong")]
        [Authorize(Policy = "Member")]
        public GetMasterThongKeChamCongResult GetMasterThongKeChamCong([FromBody] GetMasterThongKeChamCongParameter request)
        {
            return this._iSalaryDataAccess.GetMasterThongKeChamCong(request);
        }

        [HttpPost]
        [Route("api/salary/createOrUpdateChamCong")]
        [Authorize(Policy = "Member")]
        public CreateOrUpdateChamCongResult CreateOrUpdateChamCong([FromBody] CreateOrUpdateChamCongParameter request)
        {
            return this._iSalaryDataAccess.CreateOrUpdateChamCong(request);
        }

        [HttpPost]
        [Route("api/salary/createOrUpdateCauHinhChamCongOt")]
        [Authorize(Policy = "Member")]
        public CreateOrUpdateCauHinhChamCongOtResult CreateOrUpdateCauHinhChamCongOt([FromBody] CreateOrUpdateCauHinhChamCongOtParameter request)
        {
            return this._iSalaryDataAccess.CreateOrUpdateCauHinhChamCongOt(request);
        }

        [HttpPost]
        [Route("api/salary/createOrUpdateCauHinhOtCaNgay")]
        [Authorize(Policy = "Member")]
        public CreateOrUpdateCauHinhOtCaNgayResult CreateOrUpdateCauHinhOtCaNgay([FromBody] CreateOrUpdateCauHinhOtCaNgayParameter request)
        {
            return this._iSalaryDataAccess.CreateOrUpdateCauHinhOtCaNgay(request);
        }

        [HttpPost]
        [Route("api/salary/createOrUpdateCauHinhThueTncn")]
        [Authorize(Policy = "Member")]
        public CreateOrUpdateCauHinhThueTncnResult CreateOrUpdateCauHinhThueTncn([FromBody] CreateOrUpdateCauHinhThueTncnParameter request)
        {
            return this._iSalaryDataAccess.CreateOrUpdateCauHinhThueTncn(request);
        }

        [HttpPost]
        [Route("api/salary/deleteCauHinhThueTncn")]
        [Authorize(Policy = "Member")]
        public DeleteCauHinhThueTncnResult DeleteCauHinhThueTncn([FromBody] DeleteCauHinhThueTncnParameter request)
        {
            return this._iSalaryDataAccess.DeleteCauHinhThueTncn(request);
        }

        [HttpPost]
        [Route("api/salary/getTkThoiGianOt")]
        [Authorize(Policy = "Member")]
        public GetTkThoiGianOtResult GetTkThoiGianOt([FromBody] GetTkThoiGianOtParameter request)
        {
            return this._iSalaryDataAccess.GetTkThoiGianOt(request);
        }

        [HttpPost]
        [Route("api/salary/getChamCongOtByEmpId")]
        [Authorize(Policy = "Member")]
        public GetChamCongOtByEmpIdResult GetChamCongOtByEmpId([FromBody] GetChamCongOtByEmpIdParameter request)
        {
            return this._iSalaryDataAccess.GetChamCongOtByEmpId(request);
        }

        [HttpPost]
        [Route("api/salary/createOrUpdateChamCongOt")]
        [Authorize(Policy = "Member")]
        public CreateOrUpdateChamCongOtResult CreateOrUpdateChamCongOt([FromBody] CreateOrUpdateChamCongOtParameter request)
        {
            return this._iSalaryDataAccess.CreateOrUpdateChamCongOt(request);
        }

        [HttpPost]
        [Route("api/salary/getMasterDataTroCap")]
        [Authorize(Policy = "Member")]
        public GetMasterDataTroCapResult GetMasterDataTroCap([FromBody] GetMasterDataTroCapParameter request)
        {
            return this._iSalaryDataAccess.GetMasterDataTroCap(request);
        }

        [HttpPost]
        [Route("api/salary/createOrUpdateTroCap")]
        [Authorize(Policy = "Member")]
        public CreateOrUpdateTroCapResult CreateOrUpdateTroCap([FromBody] CreateOrUpdateTroCapParameter request)
        {
            return this._iSalaryDataAccess.CreateOrUpdateTroCap(request);
        }
        
        [HttpPost]
        [Route("api/salary/deleteTroCap")]
        [Authorize(Policy = "Member")]
        public DeleteTroCapResult DeleteTroCap([FromBody] DeleteTroCapParameter request)
        {
            return this._iSalaryDataAccess.DeleteTroCap(request);
        }

        [HttpPost]
        [Route("api/salary/getListKyLuong")]
        [Authorize(Policy = "Member")]
        public GetListKyLuongResult GetListKyLuong([FromBody] GetListKyLuongParameter request)
        {
            return this._iSalaryDataAccess.GetListKyLuong(request);
        }

        [HttpPost]
        [Route("api/salary/createOrUpdateKyLuong")]
        [Authorize(Policy = "Member")]
        public CreateOrUpdateKyLuongResult CreateOrUpdateKyLuong([FromBody] CreateOrUpdateKyLuongParameter request)
        {
            return this._iSalaryDataAccess.CreateOrUpdateKyLuong(request);
        }

        [HttpPost]
        [Route("api/salary/deleteKyLuong")]
        [Authorize(Policy = "Member")]
        public DeleteKyLuongResult DeleteKyLuong([FromBody] DeleteKyLuongParameter request)
        {
            return this._iSalaryDataAccess.DeleteKyLuong(request);
        }

        [HttpPost]
        [Route("api/salary/datVeMoiKyLuong")]
        [Authorize(Policy = "Member")]
        public DatVeMoiKyLuongResult DatVeMoiKyLuong([FromBody] DatVeMoiKyLuongParameter request)
        {
            return this._iSalaryDataAccess.DatVeMoiKyLuong(request);
        }

        [HttpPost]
        [Route("api/salary/getKyLuongById")]
        [Authorize(Policy = "Member")]
        public GetKyLuongByIdResult GetKyLuongById([FromBody] GetKyLuongByIdParameter request)
        {
            return this._iSalaryDataAccess.GetKyLuongById(request);
        }

        [HttpPost]
        [Route("api/salary/saveThuNhapTinhThue")]
        [Authorize(Policy = "Member")]
        public SaveThuNhapTinhThueResult SaveThuNhapTinhThue([FromBody] SaveThuNhapTinhThueParameter request)
        {
            return this._iSalaryDataAccess.SaveThuNhapTinhThue(request);
        }

        [HttpPost]
        [Route("api/salary/saveBaoHiem")]
        [Authorize(Policy = "Member")]
        public SaveBaoHiemResult SaveBaoHiem([FromBody] SaveBaoHiemParameter request)
        {
            return this._iSalaryDataAccess.SaveBaoHiem(request);
        }

        [HttpPost]
        [Route("api/salary/saveGiamTruTruocThue")]
        [Authorize(Policy = "Member")]
        public SaveGiamTruTruocThueResult SaveGiamTruTruocThue([FromBody] SaveGiamTruTruocThueParameter request)
        {
            return this._iSalaryDataAccess.SaveGiamTruTruocThue(request);
        }

        [HttpPost]
        [Route("api/salary/saveGiamTruSauThue")]
        [Authorize(Policy = "Member")]
        public SaveGiamTruSauThueResult SaveGiamTruSauThue([FromBody] SaveGiamTruSauThueParameter request)
        {
            return this._iSalaryDataAccess.SaveGiamTruSauThue(request);
        }

        [HttpPost]
        [Route("api/salary/saveHoanLaiSauThue")]
        [Authorize(Policy = "Member")]
        public SaveHoanLaiSauThueResult SaveHoanLaiSauThue([FromBody] SaveHoanLaiSauThueParameter request)
        {
            return this._iSalaryDataAccess.SaveHoanLaiSauThue(request);
        }

        [HttpPost]
        [Route("api/salary/saveCtyDong")]
        [Authorize(Policy = "Member")]
        public SaveCtyDongResult SaveCtyDong([FromBody] SaveCtyDongParameter request)
        {
            return this._iSalaryDataAccess.SaveCtyDong(request);
        }

        [HttpPost]
        [Route("api/salary/saveOther")]
        [Authorize(Policy = "Member")]
        public SaveOtherResult SaveOther([FromBody] SaveOtherParameter request)
        {
            return this._iSalaryDataAccess.SaveOther(request);
        }

        [HttpPost]
        [Route("api/salary/getListDieuKienTroCapCoDinh")]
        [Authorize(Policy = "Member")]
        public GetListDieuKienTroCapCoDinhResult GetListDieuKienTroCapCoDinh([FromBody] GetListDieuKienTroCapCoDinhParameter request)
        {
            return this._iSalaryDataAccess.GetListDieuKienTroCapCoDinh(request);
        }

        [HttpPost]
        [Route("api/salary/updateDieuKienTroCapCoDinh")]
        [Authorize(Policy = "Member")]
        public UpdateDieuKienTroCapCoDinhResult UpdateDieuKienTroCapCoDinh([FromBody] UpdateDieuKienTroCapCoDinhParameter request)
        {
            return this._iSalaryDataAccess.UpdateDieuKienTroCapCoDinh(request);
        }

        [HttpPost]
        [Route("api/salary/saveLuongCtTroCapKhac")]
        [Authorize(Policy = "Member")]
        public SaveLuongCtTroCapKhacResult SaveLuongCtTroCapKhac([FromBody] SaveLuongCtTroCapKhacParameter request)
        {
            return this._iSalaryDataAccess.SaveLuongCtTroCapKhac(request);
        }

        [HttpPost]
        [Route("api/salary/getListDieuKienTroCapKhac")]
        [Authorize(Policy = "Member")]
        public GetListDieuKienTroCapKhacResult GetListDieuKienTroCapKhac([FromBody] GetListDieuKienTroCapKhacParameter request)
        {
            return this._iSalaryDataAccess.GetListDieuKienTroCapKhac(request);
        }

        [HttpPost]
        [Route("api/salary/updateDieuKienTroCapKhac")]
        [Authorize(Policy = "Member")]
        public UpdateDieuKienTroCapKhacResult UpdateDieuKienTroCapKhac([FromBody] UpdateDieuKienTroCapKhacParameter request)
        {
            return this._iSalaryDataAccess.UpdateDieuKienTroCapKhac(request);
        }

        [HttpPost]
        [Route("api/salary/updateMucTroCapKhac")]
        [Authorize(Policy = "Member")]
        public UpdateMucTroCapKhacResult UpdateMucTroCapKhac([FromBody] UpdateMucTroCapKhacParameter request)
        {
            return this._iSalaryDataAccess.UpdateMucTroCapKhac(request);
        }

        [HttpPost]
        [Route("api/salary/importTroCapKhac")]
        [Authorize(Policy = "Member")]
        public ImportTroCapKhacResult ImportTroCapKhac([FromBody] ImportTroCapKhacParameter request)
        {
            return this._iSalaryDataAccess.ImportTroCapKhac(request);
        }

        [HttpPost]
        [Route("api/salary/importLuongChiTiet")]
        [Authorize(Policy = "Member")]
        public ImportLuongChiTietResult ImportLuongChiTiet([FromBody] ImportLuongChiTietParameter request)
        {
            return this._iSalaryDataAccess.ImportLuongChiTiet(request);
        }

        [HttpPost]
        [Route("api/salary/hoanThanhKyLuong")]
        [Authorize(Policy = "Member")]
        public HoanThanhKyLuongResult HoanThanhKyLuong([FromBody] HoanThanhKyLuongParameter request)
        {
            return this._iSalaryDataAccess.HoanThanhKyLuong(request);
        }

        [HttpPost]
        [Route("api/salary/getListPhieuLuong")]
        [Authorize(Policy = "Member")]
        public GetListPhieuLuongResult GetListPhieuLuong([FromBody] GetListPhieuLuongParameter request)
        {
            return this._iSalaryDataAccess.GetListPhieuLuong(request);
        }

        [HttpPost]
        [Route("api/salary/getPhieuLuongById")]
        [Authorize(Policy = "Member")]
        public GetPhieuLuongByIdResult GetPhieuLuongById([FromBody] GetPhieuLuongByIdParameter request)
        {
            return this._iSalaryDataAccess.GetPhieuLuongById(request);
        }

        [HttpPost]
        [Route("api/salary/exportBaoCaoKyLuong")]
        [Authorize(Policy = "Member")]
        public ExportBaoCaoKyLuongResult ExportBaoCaoKyLuong([FromBody] ExportBaoCaoKyLuongParameter request)
        {
            return this._iSalaryDataAccess.ExportBaoCaoKyLuong(request);
        }

        [HttpPost]
        [Route("api/salary/capNhatBangLuong")]
        [Authorize(Policy = "Member")]
        public CapNhatBangLuongResult CapNhatBangLuong([FromBody] CapNhatBangLuongParameter request)
        {
            return this._iSalaryDataAccess.CapNhatBangLuong(request);
        }

        [HttpPost]
        [Route("api/salary/exportExcelKyLuongMealAllowance")]
        [Authorize(Policy = "Member")]
        public ExportExcelKyLuongMealAllowanceResult ExportExcelKyLuongMealAllowance([FromBody] ExportExcelKyLuongMealAllowanceParameter request)
        {
            return this._iSalaryDataAccess.ExportExcelKyLuongMealAllowance(request);

        }
    }
}
