using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.Task;

namespace TN.TNM.DataAccess.Models.Employee
{
    public class EmployeeEntityModel : BaseModel<DataAccess.Databases.Entities.Employee>
    {
        public Guid? EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public DateTime? StartedDate { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? PositionId { get; set; }
        public Guid? CreatedById { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool? Active { get; set; }
        public bool? IsManager { get; set; }
        public Guid? PermissionSetId { get; set; }
        public Guid? ContactId { get; set; }
        public string Identity { get; set; }
        public decimal? MucLuongHienTai { get; set; }
        public string Username { get; set; }
        public string OrganizationName { get; set; }
        public string AvatarUrl { get; set; }
        public string PositionName { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string DanToc { get; set; }
        public string TonGiao { get; set; }
        public string CodeMayChamCong { get; set; }
        public string TenMayChamCong { get; set; }
        public string HoTenTiengAnh { get; set; }
        public string EmployeeMayChamCongId { get; set; }
        public string ChuyenNganhHoc { get; set; }
        public string TenTruongHocCaoNhat { get; set; }
        public Guid? BangCapCaoNhatDatDuocId { get; set; }
        public string BangCapCaoNhatDatDuocName { get; set; }
        public int? KyNangTayNghes { get; set; }
        public string KyNangTayNghesName { get; set; }
        public string TomTatHocVan { get; set; }
        public Guid? PhuongThucTuyenDungId { get; set; }
        public string PhuongThucTuyenDungName { get; set; }
        public decimal? MucPhi { get; set; }
        public bool? CoPhi { get; set; }
        public Guid? NguonTuyenDung { get; set; }
        public string BienSo { get; set; }
        public string LoaiXe { get; set; }
        public string MaTest { get; set; }
        public string DiemTest { get; set; }
        public string SoSoBHXH { get; set; }
        public string MaTheBHYT { get; set; }
        public string BankAddress { get; set; }
        public string Address { get; set; }
        public string AddressTiengAnh { get; set; }
        public int? DeptCodeValue { get; set; }
        public Guid? ProvinceId { get; set; }
        public string ProvinceName { get; set; }

        public string BankOwnerName { get; set; }
        public string IdentityId { get; set; }
        public string BankAccount { get; set; }
        public string MaTheBHLoftCare { get; set; }
        public DateTime? ThangNopDangKyGiamTru { get; set; }
        public string ContractName { get; set; }
        public Guid? ContractType { get; set; }
        public DateTime? ContractEndDate { get; set; }
        public DateTime? ProbationEndDate { get; set; }
        public DateTime? ProbationStartDate { get; set; }
        public DateTime? TrainingStartDate { get; set; }
        public bool? ActiveUser { get; set; }
        public Guid? RoleId { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string EmployeeCodeName { get; set; }
        public bool? IsTakeCare { get; set; }
        public int? OrganizationLevel { get; set; }
        public decimal? ChiPhiTheoGio { get; set; }
        public bool? IsCashier { get; set; }
        public bool? IsOrder { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string QuocTich { get; set; }
        public int? SubCode1 { get; set; }
        public int? SubCode2 { get; set; }
        public string SubCode1Name { get; set; }
        public string SubCode2Name { get; set; }
        public string DeptCode { get; set; }
        public string GradeTesting { get; set; }
        public DateTime? StartDateMayChamCong { get; set; }
        public Guid? CapBacId { get; set; }
        public string CapBacName { get; set; }
        public int? DeXuatCongTacId { get; set; }
        public List<Guid> ListTaskId { get; set; }
        public string BankName { get; set; }
        public string BankCode { get; set; }
        public string ViTriLamViec { get; set; }
        public int? NumberChildren { get; set; }
        public string Gender { get; set; }
        public string HealthInsuranceNumber { get; set; }
        public string HoKhauThuongTruTa { get; set; }
        public string HoKhauThuongTruTv { get; set; }
        public DateTime? IdentityIddateOfIssue { get; set; } //ngày cấp
        public string IdentityIdplaceOfIssue { get; set; } //nơi cấp

        public string MaSoThueCaNhan { get; set; }
        public Guid? NguonTuyenDungId { get; set; }
        public string NguonTuyenDungName { get; set; }
        public string NoiCapCmndtiengAnh { get; set; }
        public string OtherPhone { get; set; }
        public string WorkEmail { get; set; }
        public string LoaiHopDongName { get; set; }
        public DateTime? NgayKyHopDong { get; set; }
        public DateTime? NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public DateTime? NgayNghiViec { get; set; }

        public string SocialInsuranceNumber { get; set; }

        public int? TrangThaiId { get; set; }
        public int? Age { get; set; }
        public int? SoNamLamViec { get; set; }//thâm niên
        public int? SubCode1Value { get; set; }
        public int? SubCode2Value { get; set; }
        public bool? IsTruongBoPhan { get; set; }

        public decimal? SoPhepConLai { get; set; }
        public decimal? SoPhepDaSuDung { get; set; }

        //Cho đnáh giá
        public bool? ThamGiaDanhGia { get; set; }
        public Guid? NguoiDanhGiaId { get; set; }
        public string NguoiDanhGiaName { get; set; }
        public bool? XemLuong { get; set; }
        //Thêm loại nhân viên
        public int? EmployeeType { get; set; }
        //Phân loại cộng tác viên
        public int? Benefit { get; set; }
        public decimal? PercenBenefit { get; set; }
        public string Mission { get; set; }
        public Organization Organization { get; set; }
        public Guid UserId { get; set; }
        public bool? IsDisable { get; set; }

        public EmployeeEntityModel() { }

        public EmployeeEntityModel(Databases.Entities.Employee entity)
        {
            //Xu ly sau khi lay tu DB len
            Mapper(entity, this);
        }

        public override DataAccess.Databases.Entities.Employee ToEntity()
        {
            //Code tien xu ly model truoc khi day vao DB
            var entity = new DataAccess.Databases.Entities.Employee();
            Mapper(this, entity);
            return entity;
        }
    }
}
