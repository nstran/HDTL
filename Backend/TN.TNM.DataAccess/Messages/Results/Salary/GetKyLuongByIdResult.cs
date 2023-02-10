using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.DynamicColumnTable;
using TN.TNM.DataAccess.Models.Salary;

namespace TN.TNM.DataAccess.Messages.Results.Salary
{
    public class GetKyLuongByIdResult : BaseResult
    {
        public KyLuongModel KyLuong { get; set; }
        public List<TongHopChamCongModel> ListTongHopChamCong { get; set; }
        public List<LuongTongHopModel> ListLuongTongHop { get; set; }
        public List<LuongCtThuNhapTinhThueModel> ListLuongCtThuNhapTinhThue { get; set; }
        public List<LuongCtBaoHiemModel> ListLuongCtBaoHiem { get; set; }
        public List<LuongCtGiamTruTruocThueModel> ListLuongCtGiamTruTruocThue { get; set; }
        public List<LuongCtGiamTruSauThueModel> ListLuongCtGiamTruSauThue { get; set; }
        public List<LuongCtHoanLaiSauThueModel> ListLuongCtHoanLaiSauThue { get; set; }
        public List<LuongCtCtyDongModel> ListLuongCtCtyDong { get; set; }
        public List<LuongCtOtherModel> ListLuongCtOther { get; set; }
        public List<List<DataRowModel>> ListDataTroCapOt { get; set; }
        public List<List<DataHeaderModel>> ListDataHeaderTroCapOt { get; set; }
        public List<List<DataRowModel>> ListDataTroCapCoDinh { get; set; }
        public List<List<DataHeaderModel>> ListDataHeaderTroCapCoDinh { get; set; }
        public List<CategoryEntityModel> ListLoaiTroCapKhac { get; set; }
        public List<CategoryEntityModel> ListSelectedLoaiTroCapKhac { get; set; }
        public List<List<DataRowModel>> ListDataTroCapKhac { get; set; }
        public List<List<DataHeaderModel>> ListDataHeaderTroCapKhac { get; set; }
        public bool IsShowGuiPheDuyet { get; set; }
        public bool IsShowPheDuyet { get; set; }
        public bool IsShowTuChoi { get; set; }
        public bool IsShowDatVeMoi { get; set; }
        public bool IsShowXoa { get; set; }
        public bool IsShowSua { get; set; }
    }
}
