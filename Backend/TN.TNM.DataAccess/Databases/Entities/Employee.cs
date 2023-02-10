using System;
using System.Collections.Generic;

namespace TN.TNM.DataAccess.Databases.Entities
{
    public partial class Employee
    {
        public Employee()
        {
            EmployeeAllowance = new HashSet<EmployeeAllowance>();
            EmployeeAssessment = new HashSet<EmployeeAssessment>();
            EmployeeInsurance = new HashSet<EmployeeInsurance>();
            EmployeeMonthySalary = new HashSet<EmployeeMonthySalary>();
            EmployeeRequest = new HashSet<EmployeeRequest>();
            EmployeeSalary = new HashSet<EmployeeSalary>();
            EmployeeTimesheet = new HashSet<EmployeeTimesheet>();
            Notifications = new HashSet<Notifications>();
            ProcurementRequestApprover = new HashSet<ProcurementRequest>();
            ProcurementRequestRequestEmployee = new HashSet<ProcurementRequest>();
        }

        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public DateTime? StartedDate { get; set; }
        public Guid? OrganizationId { get; set; }
        public Guid? PositionId { get; set; }
        public bool IsManager { get; set; }
        public DateTime? ProbationEndDate { get; set; }
        public DateTime? ProbationStartDate { get; set; }
        public DateTime? TrainingStartDate { get; set; }
        public bool? Active { get; set; }
        public Guid CreatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? UpdatedById { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid? ContractType { get; set; }
        public DateTime? ContractEndDate { get; set; }
        public Guid? TenantId { get; set; }
        public Guid? BranchId { get; set; }
        public bool? IsOrder { get; set; }
        public bool? IsCashier { get; set; }
        public bool IsTakeCare { get; set; }
        public decimal ChiPhiTheoGio { get; set; }
        public bool? IsCustomer { get; set; }
        public bool IsOverviewer { get; set; }
        public bool IsXuLyPhanHoi { get; set; }
        public string HoTenTiengAnh { get; set; }
        public string DanToc { get; set; }
        public int? EmployeeMayChamCongId { get; set; }
        public string TonGiao { get; set; }
        public string CodeMayChamCong { get; set; }
        public string TenMayChamCong { get; set; }
        public DateTime? StartDateMayChamCong { get; set; }
        public string MaTest { get; set; }
        public string DiemTest { get; set; }
        public string GradeTesting { get; set; }
        public string DiaDiemLamviec { get; set; }
        public string BienSo { get; set; }
        public string LoaiXe { get; set; }
        public DateTime? ThangNopDangKyGiamTru { get; set; }
        public string TomTatHocVan { get; set; }
        public string ChuyenNganhHoc { get; set; }
        public string TenTruongHocCaoNhat { get; set; }
        public Guid? BangCapCaoNhatDatDuocId { get; set; }
        public string QuocTich { get; set; }
        public Guid? CapBacId { get; set; }
        public Guid? Mission { get; set; }
        public string MaSoThueCaNhan { get; set; }
        public decimal SoNguoiDangKyPhuThuoc { get; set; }
        public bool? IsXacNhanTaiLieu { get; set; }
        public Guid? NguoiYeuCauXacNhanId { get; set; }
        public Guid? PhuongThucTuyenDungId { get; set; }
        public string LoaiChuyenNganhHoc { get; set; }
        public decimal MucPhi { get; set; }
        public Guid? NguonTuyenDungId { get; set; }
        public string ViTriLamViec { get; set; }
        public int? DeptCodeValue { get; set; }
        public int? SubCode1Value { get; set; }
        public int? SubCode2Value { get; set; }
        public bool IsNhanSu { get; set; }
        public DateTime? NgayNghiViec { get; set; }
        public int? KyNangTayNghes { get; set; }
        public string KinhNghiemLamViec { get; set; }
        public decimal? SoNgayDaNghiPhep { get; set; }
        public decimal? SoNgayPhepConLai { get; set; }
        public Guid? CustomerOrderId { get; set; }
        public int? EmployeeType { get; set; }
        public int? Benefit { get; set; }
        public decimal? PercenBenefit { get; set; }

        public Organization Organization { get; set; }
        public ICollection<EmployeeAllowance> EmployeeAllowance { get; set; }
        public ICollection<EmployeeAssessment> EmployeeAssessment { get; set; }
        public ICollection<EmployeeInsurance> EmployeeInsurance { get; set; }
        public ICollection<EmployeeMonthySalary> EmployeeMonthySalary { get; set; }
        public ICollection<EmployeeRequest> EmployeeRequest { get; set; }
        public ICollection<EmployeeSalary> EmployeeSalary { get; set; }
        public ICollection<EmployeeTimesheet> EmployeeTimesheet { get; set; }
        public ICollection<Notifications> Notifications { get; set; }
        public ICollection<ProcurementRequest> ProcurementRequestApprover { get; set; }
        public ICollection<ProcurementRequest> ProcurementRequestRequestEmployee { get; set; }
    }
}
