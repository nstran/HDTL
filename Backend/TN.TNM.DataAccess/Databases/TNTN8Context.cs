using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Interfaces;

namespace TN.TNM.DataAccess.Databases
{
    public partial class TNTN8Context : DbContext
    {
        public TNTN8Context()
        {
        }

        private Guid _tenantId;


        public TNTN8Context(DbContextOptions<TNTN8Context> options) : base(options)
        {

        }

        public TNTN8Context(DbContextOptions<TNTN8Context> options, ITenantProvider tenantProvider) : base(options)
        {
            _tenantId = tenantProvider.GetTenantId();
        }
        public virtual DbSet<ManagerPacketService> ManagerPacketService { get; set; }
        public virtual DbSet<ActionResource> ActionResource { get; set; }
        public virtual DbSet<AdditionalInformation> AdditionalInformation { get; set; }
        public virtual DbSet<AdvertisementConfiguration> AdvertisementConfiguration { get; set; }
        public virtual DbSet<AttributeConfiguration> AttributeConfiguration { get; set; }
        public virtual DbSet<AuditTrace> AuditTrace { get; set; }
        public virtual DbSet<BankAccount> BankAccount { get; set; }
        public virtual DbSet<BankBook> BankBook { get; set; }
        public virtual DbSet<BankPayableInvoice> BankPayableInvoice { get; set; }
        public virtual DbSet<BankPayableInvoiceMapping> BankPayableInvoiceMapping { get; set; }
        public virtual DbSet<BankReceiptInvoice> BankReceiptInvoice { get; set; }
        public virtual DbSet<BankReceiptInvoiceMapping> BankReceiptInvoiceMapping { get; set; }
        public virtual DbSet<BaoDuongTaiSan> BaoDuongTaiSan { get; set; }
        public virtual DbSet<BillOfSale> BillOfSale { get; set; }
        public virtual DbSet<BillOfSaleCost> BillOfSaleCost { get; set; }
        public virtual DbSet<BillOfSaleCostProductAttribute> BillOfSaleCostProductAttribute { get; set; }
        public virtual DbSet<BillOfSaleDetail> BillOfSaleDetail { get; set; }
        public virtual DbSet<Branch> Branch { get; set; }
        public virtual DbSet<BusinessGoals> BusinessGoals { get; set; }
        public virtual DbSet<BusinessGoalsDetail> BusinessGoalsDetail { get; set; }
        public virtual DbSet<CacBuocApDung> CacBuocApDung { get; set; }
        public virtual DbSet<CacBuocQuyTrinh> CacBuocQuyTrinh { get; set; }
        public virtual DbSet<CaLamViec> CaLamViec { get; set; }
        public virtual DbSet<CaLamViecChiTiet> CaLamViecChiTiet { get; set; }
        public virtual DbSet<Candidate> Candidate { get; set; }
        public virtual DbSet<CandidateAssessment> CandidateAssessment { get; set; }
        public virtual DbSet<CandidateAssessmentDetail> CandidateAssessmentDetail { get; set; }
        public virtual DbSet<CandidateAssessmentMapping> CandidateAssessmentMapping { get; set; }
        public virtual DbSet<CandidateVacanciesMapping> CandidateVacanciesMapping { get; set; }
        public virtual DbSet<CapPhatTaiSan> CapPhatTaiSan { get; set; }
        public virtual DbSet<Case> Case { get; set; }
        public virtual DbSet<CaseActivities> CaseActivities { get; set; }
        public virtual DbSet<CashBook> CashBook { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<CategoryType> CategoryType { get; set; }
        public virtual DbSet<CauHinhBaoHiem> CauHinhBaoHiem { get; set; }
        public virtual DbSet<CauHinhBaoHiemLoftCare> CauHinhBaoHiemLoftCare { get; set; }
        public virtual DbSet<CauHinhChamCongOt> CauHinhChamCongOt { get; set; }
        public virtual DbSet<CauHinhChecklist> CauHinhChecklist { get; set; }
        public virtual DbSet<CauHinhGiamTru> CauHinhGiamTru { get; set; }
        public virtual DbSet<CauHinhNghiLe> CauHinhNghiLe { get; set; }
        public virtual DbSet<CauHinhNghiLeChiTiet> CauHinhNghiLeChiTiet { get; set; }
        public virtual DbSet<CauHinhOt> CauHinhOt { get; set; }
        public virtual DbSet<CauHinhOtCaNgay> CauHinhOtCaNgay { get; set; }
        public virtual DbSet<CauHinhQuyTrinh> CauHinhQuyTrinh { get; set; }
        public virtual DbSet<CauHinhThueTncn> CauHinhThueTncn { get; set; }
        public virtual DbSet<CauHoiDanhGia> CauHoiDanhGia { get; set; }
        public virtual DbSet<CauHoiMucDanhGiaMapping> CauHoiMucDanhGiaMapping { get; set; }
        public virtual DbSet<CauHoiPhieuDanhGiaMapping> CauHoiPhieuDanhGiaMapping { get; set; }
        public virtual DbSet<CauHoiPhieuDanhGiaMappingDanhMucItem> CauHoiPhieuDanhGiaMappingDanhMucItem { get; set; }
        public virtual DbSet<CauTraLoi> CauTraLoi { get; set; }
        public virtual DbSet<ChamCong> ChamCong { get; set; }
        public virtual DbSet<ChamCongOt> ChamCongOt { get; set; }
        public virtual DbSet<ChiTietDanhGiaNhanVien> ChiTietDanhGiaNhanVien { get; set; }
        public virtual DbSet<ChiTietDeXuatCongTac> ChiTietDeXuatCongTac { get; set; }
        public virtual DbSet<ChiTietHoanUng> ChiTietHoanUng { get; set; }
        public virtual DbSet<ChiTietTamUng> ChiTietTamUng { get; set; }
        public virtual DbSet<ChucVuBaoHiemLoftCare> ChucVuBaoHiemLoftCare { get; set; }
        public virtual DbSet<Company> Company { get; set; }
        public virtual DbSet<CompanyConfiguration> CompanyConfiguration { get; set; }
        public virtual DbSet<ConfigurationRule> ConfigurationRule { get; set; }
        public virtual DbSet<CongThucTinhLuong> CongThucTinhLuong { get; set; }
        public virtual DbSet<Contact> Contact { get; set; }
        public virtual DbSet<Contract> Contract { get; set; }
        public virtual DbSet<ContractCostDetail> ContractCostDetail { get; set; }
        public virtual DbSet<ContractDetail> ContractDetail { get; set; }
        public virtual DbSet<ContractDetailProductAttribute> ContractDetailProductAttribute { get; set; }
        public virtual DbSet<Cost> Cost { get; set; }
        public virtual DbSet<CostsQuote> CostsQuote { get; set; }
        public virtual DbSet<Counter> Counter { get; set; }
        public virtual DbSet<Country> Country { get; set; }
        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<CustomerAdditionalInformation> CustomerAdditionalInformation { get; set; }
        public virtual DbSet<CustomerCare> CustomerCare { get; set; }
        public virtual DbSet<CustomerCareCustomer> CustomerCareCustomer { get; set; }
        public virtual DbSet<CustomerCareFeedBack> CustomerCareFeedBack { get; set; }
        public virtual DbSet<CustomerCareFilter> CustomerCareFilter { get; set; }
        public virtual DbSet<CustomerMeeting> CustomerMeeting { get; set; }
        public virtual DbSet<CustomerOrder> CustomerOrder { get; set; }
        public virtual DbSet<CustomerOrderDetail> CustomerOrderDetail { get; set; }
        public virtual DbSet<CustomerOrderExtension> CustomerOrderExtension { get; set; }
        public virtual DbSet<CustomerOrderLocalPointMapping> CustomerOrderLocalPointMapping { get; set; }
        public virtual DbSet<CustomerOrderTask> CustomerOrderTask { get; set; }
        public virtual DbSet<CustomerServiceLevel> CustomerServiceLevel { get; set; }
        public virtual DbSet<DanhGiaNhanVien> DanhGiaNhanVien { get; set; }
        public virtual DbSet<DanhMucCauTraLoiDanhGiaMapping> DanhMucCauTraLoiDanhGiaMapping { get; set; }
        public virtual DbSet<DeNghiTamHoanUng> DeNghiTamHoanUng { get; set; }
        public virtual DbSet<DeNghiTamHoanUngChiTiet> DeNghiTamHoanUngChiTiet { get; set; }
        public virtual DbSet<DeNghiTamUng> DeNghiTamUng { get; set; }
        public virtual DbSet<DeXuatCongTac> DeXuatCongTac { get; set; }
        public virtual DbSet<DeXuatNghi> DeXuatNghi { get; set; }
        public virtual DbSet<DeXuatTangLuong> DeXuatTangLuong { get; set; }
        public virtual DbSet<DeXuatTangLuongNhanVien> DeXuatTangLuongNhanVien { get; set; }
        public virtual DbSet<DeXuatThayDoiChucVu> DeXuatThayDoiChucVu { get; set; }
        public virtual DbSet<DeXuatXinNghi> DeXuatXinNghi { get; set; }
        public virtual DbSet<DeXuatXinNghiChiTiet> DeXuatXinNghiChiTiet { get; set; }
        public virtual DbSet<District> District { get; set; }
        public virtual DbSet<Document> Document { get; set; }
        public virtual DbSet<DoiTuongPhuThuocMapping> DoiTuongPhuThuocMapping { get; set; }
        public virtual DbSet<DotKiemKe> DotKiemKe { get; set; }
        public virtual DbSet<DotKiemKeChiTiet> DotKiemKeChiTiet { get; set; }
        public virtual DbSet<EmailTemplate> EmailTemplate { get; set; }
        public virtual DbSet<EmailTemplateCcvalue> EmailTemplateCcvalue { get; set; }
        public virtual DbSet<EmailTemplateToken> EmailTemplateToken { get; set; }
        public virtual DbSet<Employee> Employee { get; set; }
        public virtual DbSet<EmployeeAllowance> EmployeeAllowance { get; set; }
        public virtual DbSet<EmployeeAssessment> EmployeeAssessment { get; set; }
        public virtual DbSet<EmployeeInsurance> EmployeeInsurance { get; set; }
        public virtual DbSet<EmployeeMappingNotificationConfiguration> EmployeeMappingNotificationConfiguration { get; set; }
        public virtual DbSet<EmployeeMappingPermissionConfiguration> EmployeeMappingPermissionConfiguration { get; set; }
        public virtual DbSet<EmployeeMonthySalary> EmployeeMonthySalary { get; set; }
        public virtual DbSet<EmployeeRequest> EmployeeRequest { get; set; }
        public virtual DbSet<EmployeeSalary> EmployeeSalary { get; set; }
        public virtual DbSet<EmployeeTimesheet> EmployeeTimesheet { get; set; }
        public virtual DbSet<ExternalUser> ExternalUser { get; set; }
        public virtual DbSet<FeatureNote> FeatureNote { get; set; }
        public virtual DbSet<FeatureWorkFlowProgress> FeatureWorkFlowProgress { get; set; }
        public virtual DbSet<FileInFolder> FileInFolder { get; set; }
        public virtual DbSet<FireBase> FireBase { get; set; }
        public virtual DbSet<Folder> Folder { get; set; }
        public virtual DbSet<GeographicalArea> GeographicalArea { get; set; }
        public virtual DbSet<Groups> Groups { get; set; }
        public virtual DbSet<GroupUser> GroupUser { get; set; }
        public virtual DbSet<Hash> Hash { get; set; }
        public virtual DbSet<HopDongNhanSu> HopDongNhanSu { get; set; }
        public virtual DbSet<HoSoCongTac> HoSoCongTac { get; set; }
        public virtual DbSet<InforScreen> InforScreen { get; set; }
        public virtual DbSet<InterviewSchedule> InterviewSchedule { get; set; }
        public virtual DbSet<InterviewScheduleMapping> InterviewScheduleMapping { get; set; }
        public virtual DbSet<Inventory> Inventory { get; set; }
        public virtual DbSet<InventoryDeliveryVoucher> InventoryDeliveryVoucher { get; set; }
        public virtual DbSet<InventoryDeliveryVoucherMapping> InventoryDeliveryVoucherMapping { get; set; }
        public virtual DbSet<InventoryDeliveryVoucherSerialMapping> InventoryDeliveryVoucherSerialMapping { get; set; }
        public virtual DbSet<InventoryDetail> InventoryDetail { get; set; }
        public virtual DbSet<InventoryReceivingVoucher> InventoryReceivingVoucher { get; set; }
        public virtual DbSet<InventoryReceivingVoucherMapping> InventoryReceivingVoucherMapping { get; set; }
        public virtual DbSet<InventoryReceivingVoucherSerialMapping> InventoryReceivingVoucherSerialMapping { get; set; }
        public virtual DbSet<InventoryReport> InventoryReport { get; set; }
        public virtual DbSet<Invoices> Invoices { get; set; }
        public virtual DbSet<Job> Job { get; set; }
        public virtual DbSet<Jobparameter> Jobparameter { get; set; }
        public virtual DbSet<Jobqueue> Jobqueue { get; set; }
        public virtual DbSet<KeHoachOt> KeHoachOt { get; set; }
        public virtual DbSet<KeHoachOtPhongBan> KeHoachOtPhongBan { get; set; }
        public virtual DbSet<KeHoachOtThanhVien> KeHoachOtThanhVien { get; set; }
        public virtual DbSet<KichHoatTinhHuong> KichHoatTinhHuong { get; set; }
        public virtual DbSet<KichHoatTinhHuongChiTiet> KichHoatTinhHuongChiTiet { get; set; }
        public virtual DbSet<KinhPhiCongDoan> KinhPhiCongDoan { get; set; }
        public virtual DbSet<KyDanhGia> KyDanhGia { get; set; }
        public virtual DbSet<KyLuong> KyLuong { get; set; }
        public virtual DbSet<Lead> Lead { get; set; }
        public virtual DbSet<LeadCare> LeadCare { get; set; }
        public virtual DbSet<LeadCareFeedBack> LeadCareFeedBack { get; set; }
        public virtual DbSet<LeadCareFilter> LeadCareFilter { get; set; }
        public virtual DbSet<LeadCareLead> LeadCareLead { get; set; }
        public virtual DbSet<LeadDetail> LeadDetail { get; set; }
        public virtual DbSet<LeadInterestedGroupMapping> LeadInterestedGroupMapping { get; set; }
        public virtual DbSet<LeadMeeting> LeadMeeting { get; set; }
        public virtual DbSet<LeadProductDetailProductAttributeValue> LeadProductDetailProductAttributeValue { get; set; }
        public virtual DbSet<LichSuPheDuyet> LichSuPheDuyet { get; set; }
        public virtual DbSet<LichSuThanhToanBaoHiem> LichSuThanhToanBaoHiem { get; set; }
        public virtual DbSet<LinkOfDocument> LinkOfDocument { get; set; }
        public virtual DbSet<List> List { get; set; }
        public virtual DbSet<LocalAddress> LocalAddress { get; set; }
        public virtual DbSet<LocalPoint> LocalPoint { get; set; }
        public virtual DbSet<LoginAuditTrace> LoginAuditTrace { get; set; }
        public virtual DbSet<LuongCtBaoHiem> LuongCtBaoHiem { get; set; }
        public virtual DbSet<LuongCtCtyDong> LuongCtCtyDong { get; set; }
        public virtual DbSet<LuongCtDieuKienTroCapCoDinh> LuongCtDieuKienTroCapCoDinh { get; set; }
        public virtual DbSet<LuongCtDieuKienTroCapKhac> LuongCtDieuKienTroCapKhac { get; set; }
        public virtual DbSet<LuongCtEmpDktcKhac> LuongCtEmpDktcKhac { get; set; }
        public virtual DbSet<LuongCtGiamTruSauThue> LuongCtGiamTruSauThue { get; set; }
        public virtual DbSet<LuongCtGiamTruTruocThue> LuongCtGiamTruTruocThue { get; set; }
        public virtual DbSet<LuongCtHoanLaiSauThue> LuongCtHoanLaiSauThue { get; set; }
        public virtual DbSet<LuongCtLoaiTroCapCoDinh> LuongCtLoaiTroCapCoDinh { get; set; }
        public virtual DbSet<LuongCtLoaiTroCapKhac> LuongCtLoaiTroCapKhac { get; set; }
        public virtual DbSet<LuongCtLoaiTroCapOt> LuongCtLoaiTroCapOt { get; set; }
        public virtual DbSet<LuongCtOther> LuongCtOther { get; set; }
        public virtual DbSet<LuongCtThuNhapTinhThue> LuongCtThuNhapTinhThue { get; set; }
        public virtual DbSet<LuongCtTroCapCoDinh> LuongCtTroCapCoDinh { get; set; }
        public virtual DbSet<LuongCtTroCapKhac> LuongCtTroCapKhac { get; set; }
        public virtual DbSet<LuongCtTroCapOt> LuongCtTroCapOt { get; set; }
        public virtual DbSet<LuongTongHop> LuongTongHop { get; set; }
        public virtual DbSet<MenuBuild> MenuBuild { get; set; }
        public virtual DbSet<MilestoneConfiguration> MilestoneConfiguration { get; set; }
        public virtual DbSet<MinusItemMapping> MinusItemMapping { get; set; }
        public virtual DbSet<MobileAppConfiguration> MobileAppConfiguration { get; set; }
        public virtual DbSet<MucDanhGia> MucDanhGia { get; set; }
        public virtual DbSet<MucDanhGiaDanhGiaMapping> MucDanhGiaDanhGiaMapping { get; set; }
        public virtual DbSet<MucHuongBaoHiemLoftCare> MucHuongBaoHiemLoftCare { get; set; }
        public virtual DbSet<MucHuongDmvs> MucHuongDmvs { get; set; }
        public virtual DbSet<MucHuongTheoNgayNghi> MucHuongTheoNgayNghi { get; set; }
        public virtual DbSet<NhanVienDeXuatThayDoiChucVu> NhanVienDeXuatThayDoiChucVu { get; set; }
        public virtual DbSet<NhanVienKyDanhGia> NhanVienKyDanhGia { get; set; }
        public virtual DbSet<NhomBaoHiemLoftCare> NhomBaoHiemLoftCare { get; set; }
        public virtual DbSet<NoiDungKyDanhGia> NoiDungKyDanhGia { get; set; }
        public virtual DbSet<Note> Note { get; set; }
        public virtual DbSet<NoteDocument> NoteDocument { get; set; }
        public virtual DbSet<NotifiAction> NotifiAction { get; set; }
        public virtual DbSet<Notifications> Notifications { get; set; }
        public virtual DbSet<NotifiCondition> NotifiCondition { get; set; }
        public virtual DbSet<NotificationConfiguration> NotificationConfiguration { get; set; }
        public virtual DbSet<NotifiSetting> NotifiSetting { get; set; }
        public virtual DbSet<NotifiSettingCondition> NotifiSettingCondition { get; set; }
        public virtual DbSet<NotifiSettingToken> NotifiSettingToken { get; set; }
        public virtual DbSet<NotifiSpecial> NotifiSpecial { get; set; }
        public virtual DbSet<Options> Options { get; set; }
        public virtual DbSet<OrderCostDetail> OrderCostDetail { get; set; }
        public virtual DbSet<OrderProductDetailProductAttributeValue> OrderProductDetailProductAttributeValue { get; set; }
        public virtual DbSet<OrderStatus> OrderStatus { get; set; }
        public virtual DbSet<OrderTechniqueMapping> OrderTechniqueMapping { get; set; }
        public virtual DbSet<Organization> Organization { get; set; }
        public virtual DbSet<OverviewCandidate> OverviewCandidate { get; set; }
        public virtual DbSet<PartItemMapping> PartItemMapping { get; set; }
        public virtual DbSet<PayableInvoice> PayableInvoice { get; set; }
        public virtual DbSet<PayableInvoiceMapping> PayableInvoiceMapping { get; set; }
        public virtual DbSet<Permission> Permission { get; set; }
        public virtual DbSet<PermissionConfiguration> PermissionConfiguration { get; set; }
        public virtual DbSet<PermissionMapping> PermissionMapping { get; set; }
        public virtual DbSet<PermissionSet> PermissionSet { get; set; }
        public virtual DbSet<PhieuDanhGia> PhieuDanhGia { get; set; }
        public virtual DbSet<PhieuLuong> PhieuLuong { get; set; }
        public virtual DbSet<PhongBanApDung> PhongBanApDung { get; set; }
        public virtual DbSet<PhongBanPheDuyetDoiTuong> PhongBanPheDuyetDoiTuong { get; set; }
        public virtual DbSet<PhongBanTrongCacBuocQuyTrinh> PhongBanTrongCacBuocQuyTrinh { get; set; }
        public virtual DbSet<Position> Position { get; set; }
        public virtual DbSet<PotentialCustomerProduct> PotentialCustomerProduct { get; set; }
        public virtual DbSet<PriceProduct> PriceProduct { get; set; }
        public virtual DbSet<PriceSuggestedSupplierQuotesMapping> PriceSuggestedSupplierQuotesMapping { get; set; }
        public virtual DbSet<ProcurementPlan> ProcurementPlan { get; set; }
        public virtual DbSet<ProcurementRequest> ProcurementRequest { get; set; }
        public virtual DbSet<ProcurementRequestItem> ProcurementRequestItem { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<ProductAttribute> ProductAttribute { get; set; }
        public virtual DbSet<ProductAttributeCategory> ProductAttributeCategory { get; set; }
        public virtual DbSet<ProductAttributeCategoryValue> ProductAttributeCategoryValue { get; set; }
        public virtual DbSet<ProductBillOfMaterials> ProductBillOfMaterials { get; set; }
        public virtual DbSet<ProductCategory> ProductCategory { get; set; }
        public virtual DbSet<ProductImage> ProductImage { get; set; }
        public virtual DbSet<ProductionOrder> ProductionOrder { get; set; }
        public virtual DbSet<ProductionOrderHistory> ProductionOrderHistory { get; set; }
        public virtual DbSet<ProductionOrderMapping> ProductionOrderMapping { get; set; }
        public virtual DbSet<ProductMappingOptions> ProductMappingOptions { get; set; }
        public virtual DbSet<ProductOrderWorkflow> ProductOrderWorkflow { get; set; }
        public virtual DbSet<ProductVendorMapping> ProductVendorMapping { get; set; }
        public virtual DbSet<Project> Project { get; set; }
        public virtual DbSet<ProjectContractMapping> ProjectContractMapping { get; set; }
        public virtual DbSet<ProjectCostReport> ProjectCostReport { get; set; }
        public virtual DbSet<ProjectEmployeeMapping> ProjectEmployeeMapping { get; set; }
        public virtual DbSet<ProjectMilestone> ProjectMilestone { get; set; }
        public virtual DbSet<ProjectObjective> ProjectObjective { get; set; }
        public virtual DbSet<ProjectResource> ProjectResource { get; set; }
        public virtual DbSet<ProjectScope> ProjectScope { get; set; }
        public virtual DbSet<ProjectVendor> ProjectVendor { get; set; }
        public virtual DbSet<Promotion> Promotion { get; set; }
        public virtual DbSet<PromotionMapping> PromotionMapping { get; set; }
        public virtual DbSet<PromotionObjectApply> PromotionObjectApply { get; set; }
        public virtual DbSet<PromotionObjectApplyMapping> PromotionObjectApplyMapping { get; set; }
        public virtual DbSet<PromotionProductMapping> PromotionProductMapping { get; set; }
        public virtual DbSet<Province> Province { get; set; }
        public virtual DbSet<PurchaseOrderStatus> PurchaseOrderStatus { get; set; }
        public virtual DbSet<Queue> Queue { get; set; }
        public virtual DbSet<Quiz> Quiz { get; set; }
        public virtual DbSet<Quote> Quote { get; set; }
        public virtual DbSet<QuoteApproveDetailHistory> QuoteApproveDetailHistory { get; set; }
        public virtual DbSet<QuoteApproveHistory> QuoteApproveHistory { get; set; }
        public virtual DbSet<QuoteCostDetail> QuoteCostDetail { get; set; }
        public virtual DbSet<QuoteDetail> QuoteDetail { get; set; }
        public virtual DbSet<QuoteDocument> QuoteDocument { get; set; }
        public virtual DbSet<QuoteParticipantMapping> QuoteParticipantMapping { get; set; }
        public virtual DbSet<QuotePaymentTerm> QuotePaymentTerm { get; set; }
        public virtual DbSet<QuotePlan> QuotePlan { get; set; }
        public virtual DbSet<QuoteProductDetailProductAttributeValue> QuoteProductDetailProductAttributeValue { get; set; }
        public virtual DbSet<QuoteScope> QuoteScope { get; set; }
        public virtual DbSet<QuyenLoiBaoHiemLoftCare> QuyenLoiBaoHiemLoftCare { get; set; }
        public virtual DbSet<QuyLuong> QuyLuong { get; set; }
        public virtual DbSet<QuyTrinh> QuyTrinh { get; set; }
        public virtual DbSet<ReceiptInvoice> ReceiptInvoice { get; set; }
        public virtual DbSet<ReceiptInvoiceMapping> ReceiptInvoiceMapping { get; set; }
        public virtual DbSet<ReceiptOrderHistory> ReceiptOrderHistory { get; set; }
        public virtual DbSet<RecruitmentCampaign> RecruitmentCampaign { get; set; }
        public virtual DbSet<RelateTaskMapping> RelateTaskMapping { get; set; }
        public virtual DbSet<RememberItem> RememberItem { get; set; }
        public virtual DbSet<RequestPayment> RequestPayment { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<RoleAndMenuBuild> RoleAndMenuBuild { get; set; }
        public virtual DbSet<RoleAndPermission> RoleAndPermission { get; set; }
        public virtual DbSet<SaleBidding> SaleBidding { get; set; }
        public virtual DbSet<SaleBiddingDetail> SaleBiddingDetail { get; set; }
        public virtual DbSet<SaleBiddingDetailProductAttribute> SaleBiddingDetailProductAttribute { get; set; }
        public virtual DbSet<SaleBiddingEmployeeMapping> SaleBiddingEmployeeMapping { get; set; }
        public virtual DbSet<Satellite> Satellite { get; set; }
        public virtual DbSet<Schema> Schema { get; set; }
        public virtual DbSet<Screen> Screen { get; set; }
        public virtual DbSet<Serial> Serial { get; set; }
        public virtual DbSet<Server> Server { get; set; }
        public virtual DbSet<ServicePacket> ServicePacket { get; set; }
        public virtual DbSet<ServicePacketImage> ServicePacketImage { get; set; }
        public virtual DbSet<ServicePacketMappingOptions> ServicePacketMappingOptions { get; set; }
        public virtual DbSet<Set> Set { get; set; }
        public virtual DbSet<SoKho> SoKho { get; set; }
        public virtual DbSet<State> State { get; set; }
        public virtual DbSet<StockTake> StockTake { get; set; }
        public virtual DbSet<StockTakeProductMapping> StockTakeProductMapping { get; set; }
        public virtual DbSet<SuggestedSupplierQuotes> SuggestedSupplierQuotes { get; set; }
        public virtual DbSet<SuggestedSupplierQuotesDetail> SuggestedSupplierQuotesDetail { get; set; }
        public virtual DbSet<SynchonizedHistory> SynchonizedHistory { get; set; }
        public virtual DbSet<SystemFeature> SystemFeature { get; set; }
        public virtual DbSet<SystemParameter> SystemParameter { get; set; }
        public virtual DbSet<TaiLieuNhanVien> TaiLieuNhanVien { get; set; }
        public virtual DbSet<TaiSan> TaiSan { get; set; }
        public virtual DbSet<Task> Task { get; set; }
        public virtual DbSet<TaskConstraint> TaskConstraint { get; set; }
        public virtual DbSet<TaskDocument> TaskDocument { get; set; }
        public virtual DbSet<TaskMilestonesMapping> TaskMilestonesMapping { get; set; }
        public virtual DbSet<TaskResourceMapping> TaskResourceMapping { get; set; }
        public virtual DbSet<TechniqueRequest> TechniqueRequest { get; set; }
        public virtual DbSet<TechniqueRequestMapping> TechniqueRequestMapping { get; set; }
        public virtual DbSet<Template> Template { get; set; }
        public virtual DbSet<Tenants> Tenants { get; set; }
        public virtual DbSet<TermsOfPayment> TermsOfPayment { get; set; }
        public virtual DbSet<ThanhVienPhongBan> ThanhVienPhongBan { get; set; }
        public virtual DbSet<TheoDoiThanhToan> TheoDoiThanhToan { get; set; }
        public virtual DbSet<ThongKeDiMuonVeSom> ThongKeDiMuonVeSom { get; set; }
        public virtual DbSet<TimeSheet> TimeSheet { get; set; }
        public virtual DbSet<TimeSheetDetail> TimeSheetDetail { get; set; }
        public virtual DbSet<TongHopChamCong> TongHopChamCong { get; set; }
        public virtual DbSet<TongHopChamCongOt> TongHopChamCongOt { get; set; }
        public virtual DbSet<TotalProductionOrder> TotalProductionOrder { get; set; }
        public virtual DbSet<TotalProductionOrderMapping> TotalProductionOrderMapping { get; set; }
        public virtual DbSet<TroCap> TroCap { get; set; }
        public virtual DbSet<TroCapChucVuMapping> TroCapChucVuMapping { get; set; }
        public virtual DbSet<TroCapDieuKienHuongMapping> TroCapDieuKienHuongMapping { get; set; }
        public virtual DbSet<TroCapLoaiHopDongMapping> TroCapLoaiHopDongMapping { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserRole> UserRole { get; set; }
        public virtual DbSet<Vacancies> Vacancies { get; set; }
        public virtual DbSet<VacanciesDocument> VacanciesDocument { get; set; }
        public virtual DbSet<Vendor> Vendor { get; set; }
        public virtual DbSet<VendorOrder> VendorOrder { get; set; }
        public virtual DbSet<VendorOrderCostDetail> VendorOrderCostDetail { get; set; }
        public virtual DbSet<VendorOrderDetail> VendorOrderDetail { get; set; }
        public virtual DbSet<VendorOrderProcurementRequestMapping> VendorOrderProcurementRequestMapping { get; set; }
        public virtual DbSet<VendorOrderProductDetailProductAttributeValue> VendorOrderProductDetailProductAttributeValue { get; set; }
        public virtual DbSet<Ward> Ward { get; set; }
        public virtual DbSet<Warehouse> Warehouse { get; set; }
        public virtual DbSet<WorkFlows> WorkFlows { get; set; }
        public virtual DbSet<WorkFlowSteps> WorkFlowSteps { get; set; }
        public virtual DbSet<YeuCauCapPhatTaiSan> YeuCauCapPhatTaiSan { get; set; }
        public virtual DbSet<YeuCauCapPhatTaiSanChiTiet> YeuCauCapPhatTaiSanChiTiet { get; set; }
        public virtual DbSet<CustomerOrderDetailExten> CustomerOrderDetailExten { get; set; }
        public virtual DbSet<ReportPoint> ReportPoint { get; set; }
        public virtual DbSet<OrderProcess> OrderProcess { get; set; }
        public virtual DbSet<OrderProcessDetail> OrderProcessDetail { get; set; }
        public virtual DbSet<OrderProcessMappingEmployee> OrderProcessMappingEmployee { get; set; }
        public virtual DbSet<VendorMappingOption> VendorMappingOption { get; set; }
        public virtual DbSet<OrderTaskMappingEmp> OrderTaskMappingEmp { get; set; }
        public virtual DbSet<PaymentMethodConfigure> PaymentMethodConfigure { get; set; }

        // Unable to generate entity type for table 'hangfire.lock'. Please see the warning messages.

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                //optionsBuilder.UseNpgsql("Server=localhost;Port=5432;User Id=postgres;Password=postgres;Database=HDTL");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ActionResource>(entity =>
            {
                entity.Property(e => e.ActionResourceId).ValueGeneratedNever();

                entity.Property(e => e.ActionResource1)
                    .HasColumnName("ActionResource")
                    .HasColumnType("character varying(255)");

                entity.Property(e => e.Description).HasColumnType("character varying(500)");
            });

            modelBuilder.Entity<AdditionalInformation>(entity =>
            {
                entity.Property(e => e.AdditionalInformationId).ValueGeneratedNever();

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasColumnType("character varying");

                entity.Property(e => e.ObjectType)
                    .IsRequired()
                    .HasColumnType("character varying(10)");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnType("character varying");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<AdvertisementConfiguration>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Content).HasColumnType("character varying");

                entity.Property(e => e.Image).HasColumnType("character varying");

                entity.Property(e => e.Title).HasColumnType("character varying");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<AttributeConfiguration>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<AuditTrace>(entity =>
            {
                entity.HasKey(e => e.TraceId);

                entity.Property(e => e.TraceId).ValueGeneratedNever();

                entity.Property(e => e.ActionName)
                    .IsRequired()
                    .HasColumnType("character varying(20)");

                entity.Property(e => e.Description).HasColumnType("character varying");

                entity.Property(e => e.ObjectName)
                    .IsRequired()
                    .HasColumnType("character varying(50)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<BankAccount>(entity =>
            {
                entity.Property(e => e.BankAccountId).ValueGeneratedNever();

                entity.Property(e => e.AccountName).HasColumnType("character varying(250)");

                entity.Property(e => e.AccountNumber).HasColumnType("character varying(50)");

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("true");

                entity.Property(e => e.BankDetail).HasColumnType("character varying(250)");

                entity.Property(e => e.BankName).HasColumnType("character varying(250)");

                entity.Property(e => e.BranchName).HasColumnType("character varying(250)");

                entity.Property(e => e.ObjectType)
                    .IsRequired()
                    .HasColumnType("character varying(20)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<BankBook>(entity =>
            {
                entity.Property(e => e.BankBookId).ValueGeneratedNever();

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.CreateDate).HasColumnType("date");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<BankPayableInvoice>(entity =>
            {
                entity.Property(e => e.BankPayableInvoiceId).ValueGeneratedNever();

                entity.Property(e => e.BankPayableInvoiceAmount).HasColumnType("money");

                entity.Property(e => e.BankPayableInvoiceAmountText).HasColumnType("character varying(250)");

                entity.Property(e => e.BankPayableInvoiceCode)
                    .IsRequired()
                    .HasColumnType("character varying(50)");

                entity.Property(e => e.BankPayableInvoiceDetail).HasColumnType("character varying(500)");

                entity.Property(e => e.BankPayableInvoiceNote).HasColumnType("character varying");

                entity.Property(e => e.BankPayableInvoicePrice).HasColumnType("money");

                entity.Property(e => e.ReceiveAccountName).HasColumnType("character varying(250)");

                entity.Property(e => e.ReceiveAccountNumber).HasColumnType("character varying(250)");

                entity.Property(e => e.ReceiveBankName).HasColumnType("character varying(250)");

                entity.Property(e => e.ReceiveBranchName).HasColumnType("character varying(250)");

                entity.HasOne(d => d.BankPayableInvoiceBankAccount)
                    .WithMany(p => p.BankPayableInvoice)
                    .HasForeignKey(d => d.BankPayableInvoiceBankAccountId)
                    .HasConstraintName("FK__BankPayab__BankP__17B8652E");

                entity.HasOne(d => d.BankPayableInvoicePriceCurrencyNavigation)
                    .WithMany(p => p.BankPayableInvoiceBankPayableInvoicePriceCurrencyNavigation)
                    .HasForeignKey(d => d.BankPayableInvoicePriceCurrency)
                    .HasConstraintName("FK__BankPayab__BankP__15D01CBC");

                entity.HasOne(d => d.BankPayableInvoiceReasonNavigation)
                    .WithMany(p => p.BankPayableInvoiceBankPayableInvoiceReasonNavigation)
                    .HasForeignKey(d => d.BankPayableInvoiceReason)
                    .HasConstraintName("FK__BankPayab__BankP__16C440F5");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.BankPayableInvoice)
                    .HasForeignKey(d => d.OrganizationId)
                    .HasConstraintName("FK__BankPayab__Organ__19A0ADA0");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.BankPayableInvoiceStatus)
                    .HasForeignKey(d => d.StatusId)
                    .HasConstraintName("FK__BankPayab__Statu__1A94D1D9");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<BankPayableInvoiceMapping>(entity =>
            {
                entity.Property(e => e.BankPayableInvoiceMappingId).ValueGeneratedNever();

                entity.HasOne(d => d.BankPayableInvoice)
                    .WithMany(p => p.BankPayableInvoiceMapping)
                    .HasForeignKey(d => d.BankPayableInvoiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__BankPayab__BankP__241E3C13");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<BankReceiptInvoice>(entity =>
            {
                entity.Property(e => e.BankReceiptInvoiceId).ValueGeneratedNever();

                entity.Property(e => e.BankReceiptInvoiceAmount).HasColumnType("money");

                entity.Property(e => e.BankReceiptInvoiceAmountText).HasColumnType("character varying(250)");

                entity.Property(e => e.BankReceiptInvoiceCode)
                    .IsRequired()
                    .HasColumnType("character varying(50)");

                entity.Property(e => e.BankReceiptInvoiceDetail).HasColumnType("character varying(500)");

                entity.Property(e => e.BankReceiptInvoiceNote).HasColumnType("character varying");

                entity.Property(e => e.BankReceiptInvoicePrice).HasColumnType("money");

                entity.HasOne(d => d.BankReceiptInvoiceBankAccount)
                    .WithMany(p => p.BankReceiptInvoice)
                    .HasForeignKey(d => d.BankReceiptInvoiceBankAccountId)
                    .HasConstraintName("FK__BankRecei__BankR__1F5986F6");

                entity.HasOne(d => d.BankReceiptInvoicePriceCurrencyNavigation)
                    .WithMany(p => p.BankReceiptInvoiceBankReceiptInvoicePriceCurrencyNavigation)
                    .HasForeignKey(d => d.BankReceiptInvoicePriceCurrency)
                    .HasConstraintName("FK__BankRecei__BankR__1D713E84");

                entity.HasOne(d => d.BankReceiptInvoiceReasonNavigation)
                    .WithMany(p => p.BankReceiptInvoiceBankReceiptInvoiceReasonNavigation)
                    .HasForeignKey(d => d.BankReceiptInvoiceReason)
                    .HasConstraintName("FK__BankRecei__BankR__1E6562BD");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.BankReceiptInvoice)
                    .HasForeignKey(d => d.OrganizationId)
                    .HasConstraintName("FK__BankRecei__Organ__204DAB2F");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.BankReceiptInvoiceStatus)
                    .HasForeignKey(d => d.StatusId)
                    .HasConstraintName("FK__BankRecei__Statu__2141CF68");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<BankReceiptInvoiceMapping>(entity =>
            {
                entity.Property(e => e.BankReceiptInvoiceMappingId).ValueGeneratedNever();

                entity.HasOne(d => d.BankReceiptInvoice)
                    .WithMany(p => p.BankReceiptInvoiceMapping)
                    .HasForeignKey(d => d.BankReceiptInvoiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__BankRecei__BankR__26FAA8BE");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<BaoDuongTaiSan>(entity =>
            {
                entity.Property(e => e.MoTa).HasColumnType("character varying(500)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<BillOfSale>(entity =>
            {
                entity.Property(e => e.BillOfSaLeId).ValueGeneratedNever();

                entity.Property(e => e.BillOfSaLeCode)
                    .IsRequired()
                    .HasColumnType("character varying(250)");

                entity.Property(e => e.CustomerAddress).HasColumnType("character varying(250)");

                entity.Property(e => e.CustomerName).HasColumnType("character varying(250)");

                entity.Property(e => e.Description).HasColumnType("character varying(250)");

                entity.Property(e => e.DiscountValue).HasColumnType("money");

                entity.Property(e => e.InvoiceSymbol).HasColumnType("character varying(250)");

                entity.Property(e => e.Mst)
                    .HasColumnName("MST")
                    .HasColumnType("character varying(50)");

                entity.Property(e => e.Note).HasColumnType("character varying");

                entity.Property(e => e.PercentAdvance)
                    .HasColumnType("money")
                    .HasDefaultValueSql("'$0.00'::money");

                entity.Property(e => e.PercentAdvanceType)
                    .IsRequired()
                    .HasDefaultValueSql("true");

                entity.Property(e => e.Vat).HasColumnName("VAT");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<BillOfSaleCost>(entity =>
            {
                entity.Property(e => e.BillOfSaleCostId).ValueGeneratedNever();

                entity.Property(e => e.CostCode).HasColumnType("character varying(50)");

                entity.Property(e => e.CostName).HasColumnType("character varying(500)");

                entity.Property(e => e.UnitPrice).HasColumnType("money");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<BillOfSaleCostProductAttribute>(entity =>
            {
                entity.Property(e => e.BillOfSaleCostProductAttributeId).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<BillOfSaleDetail>(entity =>
            {
                entity.Property(e => e.BillOfSaleDetailId).ValueGeneratedNever();

                entity.Property(e => e.Description).HasColumnType("character varying");

                entity.Property(e => e.DiscountValue).HasColumnType("money");

                entity.Property(e => e.IncurredUnit).HasColumnType("character varying(50)");

                entity.Property(e => e.MoneyForGoods).HasColumnType("money");

                entity.Property(e => e.PriceInitial).HasColumnType("money");

                entity.Property(e => e.ProductName).HasColumnType("character varying(500)");

                entity.Property(e => e.UnitLaborPrice)
                    .HasColumnType("money")
                    .HasDefaultValueSql("'$0.00'::money");

                entity.Property(e => e.UnitPrice).HasColumnType("money");

                entity.Property(e => e.Vat)
                    .HasColumnName("VAT")
                    .HasColumnType("money");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Branch>(entity =>
            {
                entity.Property(e => e.BranchId).ValueGeneratedNever();

                entity.Property(e => e.Address).HasColumnType("character varying(1000)");

                entity.Property(e => e.BranchCode).HasColumnType("character varying(100)");

                entity.Property(e => e.BranchName).HasColumnType("character varying(100)");

                entity.Property(e => e.Description).HasColumnType("character varying(2000)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<BusinessGoals>(entity =>
            {
                entity.Property(e => e.BusinessGoalsId).ValueGeneratedNever();

                entity.Property(e => e.Year).HasColumnType("character varying(10)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<BusinessGoalsDetail>(entity =>
            {
                entity.Property(e => e.BusinessGoalsDetailId).ValueGeneratedNever();

                entity.Property(e => e.April)
                    .HasColumnType("money")
                    .HasDefaultValueSql("'$0.00'::money");

                entity.Property(e => e.August)
                    .HasColumnType("money")
                    .HasDefaultValueSql("'$0.00'::money");

                entity.Property(e => e.BusinessGoalsType).HasColumnType("character varying(10)");

                entity.Property(e => e.December)
                    .HasColumnType("money")
                    .HasDefaultValueSql("'$0.00'::money");

                entity.Property(e => e.February)
                    .HasColumnType("money")
                    .HasDefaultValueSql("'$0.00'::money");

                entity.Property(e => e.January)
                    .HasColumnType("money")
                    .HasDefaultValueSql("'$0.00'::money");

                entity.Property(e => e.July)
                    .HasColumnType("money")
                    .HasDefaultValueSql("'$0.00'::money");

                entity.Property(e => e.June)
                    .HasColumnType("money")
                    .HasDefaultValueSql("'$0.00'::money");

                entity.Property(e => e.March)
                    .HasColumnType("money")
                    .HasDefaultValueSql("'$0.00'::money");

                entity.Property(e => e.May)
                    .HasColumnType("money")
                    .HasDefaultValueSql("'$0.00'::money");

                entity.Property(e => e.November)
                    .HasColumnType("money")
                    .HasDefaultValueSql("'$0.00'::money");

                entity.Property(e => e.October)
                    .HasColumnType("money")
                    .HasDefaultValueSql("'$0.00'::money");

                entity.Property(e => e.September)
                    .HasColumnType("money")
                    .HasDefaultValueSql("'$0.00'::money");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CacBuocApDung>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.ObjectNumber).HasDefaultValueSql("0");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CacBuocQuyTrinh>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CaLamViec>(entity =>
            {
                entity.Property(e => e.Active).HasDefaultValueSql("true");

                entity.Property(e => e.GioRa).HasColumnType("time without time zone");

                entity.Property(e => e.GioVao).HasColumnType("time without time zone");

                entity.Property(e => e.ThoiGianKetThucCa).HasColumnType("time without time zone");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Candidate>(entity =>
            {
                entity.Property(e => e.CandidateId).ValueGeneratedNever();

                entity.Property(e => e.Address).HasColumnType("character varying");

                entity.Property(e => e.Avatar).HasColumnType("character varying");

                entity.Property(e => e.Email).HasColumnType("character varying(100)");

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasColumnType("character varying");

                entity.Property(e => e.Phone).HasColumnType("character varying(50)");

                entity.Property(e => e.TomTatHocVan).HasColumnType("character varying");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CandidateAssessment>(entity =>
            {
                entity.Property(e => e.CandidateAssessmentId).ValueGeneratedNever();

                entity.Property(e => e.OtherReview).HasColumnType("character varying");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CandidateAssessmentDetail>(entity =>
            {
                entity.Property(e => e.CandidateAssessmentDetailId).ValueGeneratedNever();

                entity.Property(e => e.Review).HasColumnType("character varying");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CandidateAssessmentMapping>(entity =>
            {
                entity.Property(e => e.CandidateAssessmentMappingId).ValueGeneratedNever();

                entity.Property(e => e.Review).HasColumnType("character varying");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CandidateVacanciesMapping>(entity =>
            {
                entity.HasKey(e => e.CandidateId);

                entity.Property(e => e.CandidateId).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CapPhatTaiSan>(entity =>
            {
                entity.Property(e => e.LyDo).HasColumnType("character varying(500)");

                entity.Property(e => e.YeuCauCapPhatTaiSanChiTietId).HasDefaultValueSql("0");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Case>(entity =>
            {
                entity.Property(e => e.CaseId).ValueGeneratedNever();

                entity.Property(e => e.CasteTitle).HasColumnType("character varying");

                entity.Property(e => e.ObjectType).HasColumnType("character varying(50)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CaseActivities>(entity =>
            {
                entity.Property(e => e.CaseActivitiesId).ValueGeneratedNever();

                entity.Property(e => e.CaseActivitiesContent).HasColumnType("character varying");

                entity.HasOne(d => d.Case)
                    .WithMany(p => p.CaseActivities)
                    .HasForeignKey(d => d.CaseId)
                    .HasConstraintName("FK_CaseActivities_Case");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CashBook>(entity =>
            {
                entity.Property(e => e.CashBookId).ValueGeneratedNever();

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.PaidDate).HasColumnType("date");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.CategoryId).ValueGeneratedNever();

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("true");

                entity.Property(e => e.CategoryCode)
                    .IsRequired()
                    .HasColumnType("character varying(10)");

                entity.Property(e => e.CategoryName)
                    .IsRequired()
                    .HasColumnType("character varying(600)");

                entity.Property(e => e.UpdatedDate).HasColumnType("date");

                entity.HasOne(d => d.CategoryType)
                    .WithMany(p => p.Category)
                    .HasForeignKey(d => d.CategoryTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Category_CategoryType");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CategoryType>(entity =>
            {
                entity.Property(e => e.CategoryTypeId).ValueGeneratedNever();

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("true");

                entity.Property(e => e.CategoryTypeCode)
                    .IsRequired()
                    .HasColumnType("character varying(20)");

                entity.Property(e => e.CategoryTypeName)
                    .IsRequired()
                    .HasColumnType("character varying(50)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CauHinhBaoHiem>(entity =>
            {
                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("true");

                entity.Property(e => e.MucDong)
                    .HasColumnType("money")
                    .HasDefaultValueSql("'$0.00'::money");

                entity.Property(e => e.MucDongToiDa).HasColumnType("money");

                entity.Property(e => e.TiLePhanBoMucDongBhtncuaNld).HasColumnName("TiLePhanBoMucDongBHTNCuaNLD");

                entity.Property(e => e.TiLePhanBoMucDongBhtncuaNsdld).HasColumnName("TiLePhanBoMucDongBHTNCuaNSDLD");

                entity.Property(e => e.TiLePhanBoMucDongBhtnnncuaNld).HasColumnName("TiLePhanBoMucDongBHTNNNCuaNLD");

                entity.Property(e => e.TiLePhanBoMucDongBhtnnncuaNsdld).HasColumnName("TiLePhanBoMucDongBHTNNNCuaNSDLD");

                entity.Property(e => e.TiLePhanBoMucDongBhxhcuaNld).HasColumnName("TiLePhanBoMucDongBHXHCuaNLD");

                entity.Property(e => e.TiLePhanBoMucDongBhxhcuaNsdld).HasColumnName("TiLePhanBoMucDongBHXHCuaNSDLD");

                entity.Property(e => e.TiLePhanBoMucDongBhytcuaNld).HasColumnName("TiLePhanBoMucDongBHYTCuaNLD");

                entity.Property(e => e.TiLePhanBoMucDongBhytcuaNsdld).HasColumnName("TiLePhanBoMucDongBHYTCuaNSDLD");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CauHinhBaoHiemLoftCare>(entity =>
            {
                entity.Property(e => e.NamCauHinh).HasColumnType("character varying(255)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CauHinhChamCongOt>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("now()");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CauHinhChecklist>(entity =>
            {
                entity.Property(e => e.TenTaiLieu).HasColumnType("character varying(255)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CauHinhGiamTru>(entity =>
            {
                entity.Property(e => e.Active).HasDefaultValueSql("true");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("now()");

                entity.Property(e => e.MucGiamTru).HasColumnType("money");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CauHinhNghiLe>(entity =>
            {
                entity.HasKey(e => e.NghiLeId);

                entity.Property(e => e.Active).HasDefaultValueSql("true");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CauHinhNghiLeChiTiet>(entity =>
            {
                entity.HasKey(e => e.NghiLeChiTietId);

                entity.Property(e => e.Active).HasDefaultValueSql("true");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CauHinhOt>(entity =>
            {
                entity.Property(e => e.Active).HasDefaultValueSql("true");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("now()");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CauHinhOtCaNgay>(entity =>
            {
                entity.Property(e => e.GioKetThucChieu).HasColumnType("time without time zone");

                entity.Property(e => e.GioKetThucSang).HasColumnType("time without time zone");

                entity.Property(e => e.GioRaChieu).HasColumnType("time without time zone");

                entity.Property(e => e.GioRaSang).HasColumnType("time without time zone");

                entity.Property(e => e.GioVaoChieu).HasColumnType("time without time zone");

                entity.Property(e => e.GioVaoSang).HasColumnType("time without time zone");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CauHinhQuyTrinh>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.QuyTrinh).HasColumnType("character varying(1000)");

                entity.Property(e => e.SoTienTu)
                    .HasColumnType("money")
                    .HasDefaultValueSql("'$0.00'::money");

                entity.Property(e => e.TenCauHinh)
                    .IsRequired()
                    .HasColumnType("character varying(255)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CauHoiDanhGia>(entity =>
            {
                entity.Property(e => e.NoiDungCauHoi).HasColumnType("character varying");

                entity.Property(e => e.TrongSo).HasDefaultValueSql("'0'::numeric");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CauHoiPhieuDanhGiaMapping>(entity =>
            {
                entity.Property(e => e.NoiDungCauHoi).HasColumnType("character varying(2000)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CauHoiPhieuDanhGiaMappingDanhMucItem>(entity =>
            {
                entity.Property(e => e.CauHoiPhieuDanhGiaMappingDanhMucItemId).HasDefaultValueSql("nextval('\"CauHoiPhieuDanhGiaMappingDanh_CauHoiPhieuDanhGiaMappingDanh_seq\"'::regclass)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CauTraLoi>(entity =>
            {
                entity.Property(e => e.LuaChonDs).HasColumnName("LuaChonDS");

                entity.Property(e => e.NoiDungTraLoi).HasColumnType("character varying");

                entity.Property(e => e.TrongSo).HasDefaultValueSql("'0'::numeric");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ChamCong>(entity =>
            {
                entity.Property(e => e.Active).HasDefaultValueSql("true");

                entity.Property(e => e.RaChieu).HasColumnType("time without time zone");

                entity.Property(e => e.RaSang).HasColumnType("time without time zone");

                entity.Property(e => e.VaoChieu).HasColumnType("time without time zone");

                entity.Property(e => e.VaoSang).HasColumnType("time without time zone");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ChamCongOt>(entity =>
            {
                entity.Property(e => e.GioRaChieu).HasColumnType("time without time zone");

                entity.Property(e => e.GioRaSang).HasColumnType("time without time zone");

                entity.Property(e => e.GioRaToi).HasColumnType("time without time zone");

                entity.Property(e => e.GioVaoChieu).HasColumnType("time without time zone");

                entity.Property(e => e.GioVaoSang).HasColumnType("time without time zone");

                entity.Property(e => e.GioVaoToi).HasColumnType("time without time zone");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ChiTietDanhGiaNhanVien>(entity =>
            {
                entity.Property(e => e.CauTraLoiText).HasColumnType("character varying");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ChiTietDeXuatCongTac>(entity =>
            {
                entity.Property(e => e.Active).HasDefaultValueSql("false");

                entity.Property(e => e.LyDo).HasColumnType("character varying(100)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ChiTietHoanUng>(entity =>
            {
                entity.Property(e => e.ChiPhiKhac).HasColumnType("character varying(500)");

                entity.Property(e => e.GhiChu).HasColumnType("character varying(500)");

                entity.Property(e => e.HinhThucThanhToan).HasColumnType("character varying(500)");

                entity.Property(e => e.KhachSan).HasColumnType("character varying(500)");

                entity.Property(e => e.SoHoaDon).HasColumnType("character varying(500)");

                entity.Property(e => e.SoTienTamUng).HasDefaultValueSql("'0'::numeric");

                entity.Property(e => e.TienDonDn)
                    .HasColumnName("TienDonDN")
                    .HasColumnType("character varying(500)");

                entity.Property(e => e.TienDonHnNd)
                    .HasColumnName("TienDonHN_ND")
                    .HasColumnType("character varying(500)");

                entity.Property(e => e.TomTatMucDichChi).HasColumnType("character varying(500)");

                entity.Property(e => e.VanChuyenXeMay).HasColumnType("character varying(500)");

                entity.Property(e => e.Vat)
                    .HasColumnName("VAT")
                    .HasDefaultValueSql("'0'::numeric");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ChiTietTamUng>(entity =>
            {
                entity.Property(e => e.NoiDung).HasColumnType("character varying(500)");

                entity.Property(e => e.SoTienTamUng).HasDefaultValueSql("'0'::numeric");

                entity.Property(e => e.Vat)
                    .HasColumnName("VAT")
                    .HasDefaultValueSql("'0'::numeric");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ChucVuBaoHiemLoftCare>(entity =>
            {
                entity.Property(e => e.SoNamKinhNghiem).HasDefaultValueSql("'0'::numeric");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Company>(entity =>
            {
                entity.Property(e => e.CompanyId).ValueGeneratedNever();

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("true");

                entity.Property(e => e.CompanyName)
                    .IsRequired()
                    .HasColumnType("character varying(500)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CompanyConfiguration>(entity =>
            {
                entity.HasKey(e => e.CompanyId);

                entity.Property(e => e.CompanyId).ValueGeneratedNever();

                entity.Property(e => e.CompanyAddress).HasColumnType("character varying");

                entity.Property(e => e.CompanyName)
                    .IsRequired()
                    .HasColumnType("character varying(250)");

                entity.Property(e => e.ContactName).HasColumnType("character varying(250)");

                entity.Property(e => e.ContactRole).HasColumnType("character varying(100)");

                entity.Property(e => e.Email).HasColumnType("character varying(250)");

                entity.Property(e => e.Phone).HasColumnType("character varying(20)");

                entity.Property(e => e.TaxCode).HasColumnType("character varying(50)");

                entity.Property(e => e.Website).HasColumnType("character varying");

                entity.HasOne(d => d.BankAccount)
                    .WithMany(p => p.CompanyConfiguration)
                    .HasForeignKey(d => d.BankAccountId)
                    .HasConstraintName("FK__CompanyCo__BankA__12F3B011");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ConfigurationRule>(entity =>
            {
                entity.Property(e => e.ConfigurationRuleId).ValueGeneratedNever();

                entity.Property(e => e.Description).HasColumnType("character varying(500)");

                entity.Property(e => e.Money).HasColumnType("money");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CongThucTinhLuong>(entity =>
            {
                entity.Property(e => e.CongThuc)
                    .IsRequired()
                    .HasColumnType("character varying(3000)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Contact>(entity =>
            {
                entity.Property(e => e.ContactId).ValueGeneratedNever();

                entity.Property(e => e.AccountNumber).HasColumnType("character varying(255)");

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("true");

                entity.Property(e => e.Address).HasColumnType("character varying");

                entity.Property(e => e.AddressTiengAnh).HasColumnType("character varying(255)");

                entity.Property(e => e.Agency).HasColumnType("character varying(100)");

                entity.Property(e => e.AvatarUrl).HasColumnType("character varying");

                entity.Property(e => e.BankAccount).HasColumnType("character varying(255)");

                entity.Property(e => e.BankAddress).HasColumnType("character varying(255)");

                entity.Property(e => e.BankCode).HasColumnType("character varying(50)");

                entity.Property(e => e.BankName).HasColumnType("character varying(500)");

                entity.Property(e => e.BankOwnerName).HasColumnType("character varying(255)");

                entity.Property(e => e.Birthplace).HasColumnType("character varying(100)");

                entity.Property(e => e.BranchName).HasColumnType("character varying(255)");

                entity.Property(e => e.CompanyAddress).HasColumnType("character varying(500)");

                entity.Property(e => e.CompanyName).HasColumnType("character varying(500)");

                entity.Property(e => e.Email).HasColumnType("character varying(100)");

                entity.Property(e => e.EvaluateContactPeople).HasColumnType("character varying(2000)");

                entity.Property(e => e.FirstName).HasColumnType("character varying(250)");

                entity.Property(e => e.Gender).HasColumnType("character varying(10)");

                entity.Property(e => e.HealthInsuranceDateOfIssue).HasColumnType("date");

                entity.Property(e => e.HealthInsuranceDateOfParticipation).HasColumnType("date");

                entity.Property(e => e.HealthInsuranceNumber).HasColumnType("character varying(50)");

                entity.Property(e => e.HoKhauThuongTruTa)
                    .HasColumnName("HoKhauThuongTruTA")
                    .HasColumnType("character varying(255)");

                entity.Property(e => e.HoKhauThuongTruTv)
                    .HasColumnName("HoKhauThuongTruTV")
                    .HasColumnType("character varying(255)");

                entity.Property(e => e.IdentityId)
                    .HasColumnName("IdentityID")
                    .HasColumnType("character varying(20)");

                entity.Property(e => e.IdentityIddateOfIssue)
                    .HasColumnName("IdentityIDDateOfIssue")
                    .HasColumnType("date");

                entity.Property(e => e.IdentityIddateOfParticipation)
                    .HasColumnName("IdentityIDDateOfParticipation")
                    .HasColumnType("date");

                entity.Property(e => e.IdentityIdplaceOfIssue)
                    .HasColumnName("IdentityIDPlaceOfIssue")
                    .HasColumnType("character varying(100)");

                entity.Property(e => e.Job).HasColumnType("character varying(100)");

                entity.Property(e => e.LastName).HasColumnType("character varying(250)");

                entity.Property(e => e.LinkFace).HasColumnType("character varying(2000)");

                entity.Property(e => e.MaTheBhLoftCare).HasColumnType("character varying(255)");

                entity.Property(e => e.MoneyLimit).HasColumnType("money");

                entity.Property(e => e.NguyenQuan).HasColumnType("character varying(255)");

                entity.Property(e => e.NoiCapCmndtiengAnh)
                    .HasColumnName("NoiCapCMNDTiengAnh")
                    .HasColumnType("character varying(255)");

                entity.Property(e => e.NoiSinh).HasColumnType("character varying(255)");

                entity.Property(e => e.Note).HasColumnType("character varying");

                entity.Property(e => e.ObjectType)
                    .IsRequired()
                    .HasColumnType("character varying(20)");

                entity.Property(e => e.OptionPosition).HasColumnType("character varying(500)");

                entity.Property(e => e.Other).HasColumnType("character varying(500)");

                entity.Property(e => e.OtherEmail).HasColumnType("character varying(100)");

                entity.Property(e => e.OtherPhone).HasColumnType("character varying(50)");

                entity.Property(e => e.Phone).HasColumnType("character varying(50)");

                entity.Property(e => e.PhuThuoc).HasDefaultValueSql("false");

                entity.Property(e => e.PostCode).HasColumnType("character varying(10)");

                entity.Property(e => e.PotentialCustomerPosition).HasColumnType("character varying(100)");

                entity.Property(e => e.RelationShip).HasColumnType("character varying");

                entity.Property(e => e.Role).HasColumnType("character varying(100)");

                entity.Property(e => e.SocialInsuranceDateOfIssue).HasColumnType("date");

                entity.Property(e => e.SocialInsuranceDateOfParticipation).HasColumnType("date");

                entity.Property(e => e.SocialInsuranceNumber).HasColumnType("character varying(50)");

                entity.Property(e => e.SocialUrl).HasColumnType("character varying");

                entity.Property(e => e.TaxCode).HasColumnType("character varying(50)");

                entity.Property(e => e.VisaNumber).HasColumnType("character varying(50)");

                entity.Property(e => e.WebsiteUrl).HasColumnType("character varying");

                entity.Property(e => e.WorkEmail).HasColumnType("character varying(100)");

                entity.Property(e => e.WorkHourOfEnd).HasColumnType("time without time zone");

                entity.Property(e => e.WorkHourOfStart).HasColumnType("time without time zone");

                entity.Property(e => e.WorkPermitNumber).HasColumnType("character varying(50)");

                entity.Property(e => e.WorkPhone).HasColumnType("character varying(50)");

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.Contact)
                    .HasForeignKey(d => d.CountryId)
                    .HasConstraintName("FK__Contact__Country");

                entity.HasOne(d => d.District)
                    .WithMany(p => p.Contact)
                    .HasForeignKey(d => d.DistrictId)
                    .HasConstraintName("FK_Contact_District");

                entity.HasOne(d => d.MaritalStatus)
                    .WithMany(p => p.Contact)
                    .HasForeignKey(d => d.MaritalStatusId)
                    .HasConstraintName("FK__Contact__Category");

                entity.HasOne(d => d.Province)
                    .WithMany(p => p.Contact)
                    .HasForeignKey(d => d.ProvinceId)
                    .HasConstraintName("FK_Contact_Province");

                entity.HasOne(d => d.Ward)
                    .WithMany(p => p.Contact)
                    .HasForeignKey(d => d.WardId)
                    .HasConstraintName("FK_Contact_Ward");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Contract>(entity =>
            {
                entity.Property(e => e.ContractId).ValueGeneratedNever();

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.ChucVuNguoiKyHdkh).HasColumnName("ChucVuNguoiKyHDKH");

                entity.Property(e => e.ContractCode)
                    .IsRequired()
                    .HasColumnType("character varying(500)");

                entity.Property(e => e.ContractDescription).HasColumnType("character varying(500)");

                entity.Property(e => e.ContractName).HasColumnType("character varying(500)");

                entity.Property(e => e.ContractNote).HasColumnType("character varying(500)");

                entity.Property(e => e.ContractTimeUnit).HasColumnType("character varying(50)");

                entity.Property(e => e.DiaChiXuatHoaDon).HasColumnType("character varying(500)");

                entity.Property(e => e.DiscountValue).HasColumnType("money");

                entity.Property(e => e.GiaTriXuatHdgomVat).HasColumnName("GiaTriXuatHDGomVat");

                entity.Property(e => e.NguoiKyHdkh)
                    .HasColumnName("NguoiKyHDKH")
                    .HasColumnType("character varying(250)");

                entity.Property(e => e.ValueContract).HasColumnType("money");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ContractCostDetail>(entity =>
            {
                entity.Property(e => e.ContractCostDetailId).ValueGeneratedNever();

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("true");

                entity.Property(e => e.CostName).HasColumnType("character varying(500)");

                entity.Property(e => e.UnitPrice).HasColumnType("money");

                entity.HasOne(d => d.Contract)
                    .WithMany(p => p.ContractCostDetail)
                    .HasForeignKey(d => d.ContractId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ContractCostDetail_Contract");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ContractDetail>(entity =>
            {
                entity.Property(e => e.ContractDetailId).ValueGeneratedNever();

                entity.Property(e => e.Description).HasColumnType("character varying");

                entity.Property(e => e.DiscountValue).HasColumnType("money");

                entity.Property(e => e.IncurredUnit).HasColumnType("character varying(50)");

                entity.Property(e => e.PriceInitial).HasColumnType("money");

                entity.Property(e => e.ProductName).HasColumnType("character varying");

                entity.Property(e => e.Quantity).HasColumnType("money");

                entity.Property(e => e.QuantityOdered).HasColumnType("money");

                entity.Property(e => e.Tax).HasColumnType("money");

                entity.Property(e => e.UnitLaborPrice)
                    .HasColumnType("money")
                    .HasDefaultValueSql("'$0.00'::money");

                entity.Property(e => e.UnitPrice).HasColumnType("money");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ContractDetailProductAttribute>(entity =>
            {
                entity.Property(e => e.ContractDetailProductAttributeId).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Cost>(entity =>
            {
                entity.Property(e => e.CostId).ValueGeneratedNever();

                entity.Property(e => e.CostCode).HasColumnType("character varying");

                entity.Property(e => e.CostName).HasColumnType("character varying");

                entity.Property(e => e.DonGia)
                    .HasColumnType("money")
                    .HasDefaultValueSql("'$0.00'::money");

                entity.Property(e => e.NgayHieuLuc).HasDefaultValueSql("now()");

                entity.Property(e => e.SoLuongToiThieu).HasDefaultValueSql("'1'::numeric");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CostsQuote>(entity =>
            {
                entity.Property(e => e.CostsQuoteId).ValueGeneratedNever();

                entity.Property(e => e.Description).HasColumnType("character varying");

                entity.Property(e => e.DiscountValue).HasColumnType("money");

                entity.Property(e => e.IncurredUnit).HasColumnType("character varying(50)");

                entity.Property(e => e.ProductName).HasColumnType("character varying(250)");

                entity.Property(e => e.UnitLaborPrice)
                    .HasColumnType("money")
                    .HasDefaultValueSql("'$0.00'::money");

                entity.Property(e => e.UnitPrice).HasColumnType("money");

                entity.Property(e => e.Vat)
                    .HasColumnName("VAT")
                    .HasColumnType("money");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Counter>(entity =>
            {
                entity.ToTable("counter", "hangfire");

                entity.HasIndex(e => e.Expireat)
                    .HasName("ix_hangfire_counter_expireat");

                entity.HasIndex(e => e.Key)
                    .HasName("ix_hangfire_counter_key");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('hangfire.counter_id_seq'::regclass)");

                entity.Property(e => e.Expireat).HasColumnName("expireat");

                entity.Property(e => e.Key)
                    .IsRequired()
                    .HasColumnName("key");

                entity.Property(e => e.Value).HasColumnName("value");
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.Property(e => e.CountryId).ValueGeneratedNever();

                entity.Property(e => e.CountryCode)
                    .IsRequired()
                    .HasColumnType("character varying(50)");

                entity.Property(e => e.CountryName)
                    .IsRequired()
                    .HasColumnType("character varying(250)");
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(e => e.CustomerId).ValueGeneratedNever();

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("true");
                entity.Property(e => e.BusinessRegistrationDate).HasColumnType("date");

                entity.Property(e => e.BussinessName).HasColumnType("character varying(255)");

                entity.Property(e => e.ContactAddress).HasColumnType("character varying(255)");

                entity.Property(e => e.ContactEmail).HasColumnType("character varying(255)");

                entity.Property(e => e.ContactName).HasColumnType("character varying(255)");

                entity.Property(e => e.CustomerCode)
                    .IsRequired()
                    .HasColumnType("character varying(20)");

                entity.Property(e => e.CustomerName).HasColumnType("character varying(500)");

                entity.Property(e => e.EvaluateCompany).HasColumnType("character varying(2000)");

                entity.Property(e => e.Mst)
                    .HasColumnName("MST")
                    .HasColumnType("character varying(255)");

                entity.Property(e => e.SalesUpdate).HasColumnType("character varying(2000)");

                entity.Property(e => e.SalesUpdateAfterMeeting).HasColumnType("character varying(2000)");

                entity.Property(e => e.Website).HasColumnType("character varying(255)");

                entity.Property(e => e.SubjectsApplication)
                    .IsRequired()
                    .HasDefaultValueSql("false");

                entity.HasOne(d => d.CustomerServiceLevel)
                    .WithMany(p => p.Customer)
                    .HasForeignKey(d => d.CustomerServiceLevelId)
                    .HasConstraintName("FK__Customer__CustomerServiceLevel");

                entity.HasOne(d => d.Field)
                    .WithMany(p => p.CustomerField)
                    .HasForeignKey(d => d.FieldId)
                    .HasConstraintName("FK__Customer__FieldId");

                entity.HasOne(d => d.Scale)
                    .WithMany(p => p.CustomerScale)
                    .HasForeignKey(d => d.ScaleId)
                    .HasConstraintName("FK__Customer__ScaleId");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.CustomerStatus)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Customer_Status");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CustomerAdditionalInformation>(entity =>
            {
                entity.Property(e => e.CustomerAdditionalInformationId).ValueGeneratedNever();

                entity.Property(e => e.Answer).HasColumnType("character varying(4000)");

                entity.Property(e => e.Question).HasColumnType("character varying(4000)");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.CustomerAdditionalInformation)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CustomerAdditionalInformation_Customer");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CustomerCare>(entity =>
            {
                entity.Property(e => e.CustomerCareId).ValueGeneratedNever();

                entity.Property(e => e.CustomerCareCode).HasColumnType("character varying(50)");

                entity.Property(e => e.CustomerCareContent).HasColumnType("character varying");

                entity.Property(e => e.CustomerCareContentEmail).HasColumnType("character varying");

                entity.Property(e => e.CustomerCareContentSms)
                    .HasColumnName("CustomerCareContentSMS")
                    .HasColumnType("character varying");

                entity.Property(e => e.CustomerCareEventHour).HasColumnType("time without time zone");

                entity.Property(e => e.CustomerCareTitle).HasColumnType("character varying");

                entity.Property(e => e.CustomerCareVoucher).HasColumnType("character varying(100)");

                entity.Property(e => e.DiscountAmount).HasColumnType("money");

                entity.Property(e => e.ExpectedAmount).HasColumnType("money");

                entity.Property(e => e.SendDate).HasColumnType("date");

                entity.Property(e => e.SendEmailDate).HasColumnType("date");

                entity.Property(e => e.SendEmailHour).HasColumnType("time without time zone");

                entity.Property(e => e.SendHour).HasColumnType("time without time zone");

                entity.Property(e => e.TypeCustomer).HasDefaultValueSql("1");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CustomerCareCustomer>(entity =>
            {
                entity.Property(e => e.CustomerCareCustomerId).ValueGeneratedNever();

                entity.HasOne(d => d.CustomerCare)
                    .WithMany(p => p.CustomerCareCustomer)
                    .HasForeignKey(d => d.CustomerCareId)
                    .HasConstraintName("FK_CustomerCareCustomer_CustomerCare");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.CustomerCareCustomer)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_CustomerCareCustomer_Customer");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CustomerCareFeedBack>(entity =>
            {
                entity.Property(e => e.CustomerCareFeedBackId).ValueGeneratedNever();

                entity.Property(e => e.FeedBackContent).HasColumnType("character varying");

                entity.HasOne(d => d.CustomerCare)
                    .WithMany(p => p.CustomerCareFeedBack)
                    .HasForeignKey(d => d.CustomerCareId)
                    .HasConstraintName("FK_CustomerCareFeedBack_CustomerCare");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.CustomerCareFeedBack)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_CustomerCareFeedBack_Customer");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CustomerCareFilter>(entity =>
            {
                entity.Property(e => e.CustomerCareFilterId).ValueGeneratedNever();

                entity.Property(e => e.QueryContent).HasColumnType("character varying");

                entity.HasOne(d => d.CustomerCare)
                    .WithMany(p => p.CustomerCareFilter)
                    .HasForeignKey(d => d.CustomerCareId)
                    .HasConstraintName("FK_CustomerCareFilter_CustomerCare");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CustomerMeeting>(entity =>
            {
                entity.Property(e => e.CustomerMeetingId).ValueGeneratedNever();

                entity.Property(e => e.Content).HasColumnType("character varying");

                entity.Property(e => e.CustomerEmail).HasColumnType("character varying(500)");

                entity.Property(e => e.EndHours).HasColumnType("time without time zone");

                entity.Property(e => e.LocationMeeting).HasColumnType("character varying");

                entity.Property(e => e.Participants).HasColumnType("character varying");

                entity.Property(e => e.StartHours).HasColumnType("time without time zone");

                entity.Property(e => e.Title).HasColumnType("character varying(500)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CustomerOrder>(entity =>
            {
                entity.HasKey(e => e.OrderId);

                entity.Property(e => e.OrderId).ValueGeneratedNever();

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("true");

                entity.Property(e => e.Amount).HasColumnType("numeric(19,4)");

                entity.Property(e => e.CustomerAddress).HasColumnType("character varying(500)");

                entity.Property(e => e.CustomerCode).HasColumnType("character varying(250)");

                entity.Property(e => e.CustomerName).HasColumnType("character varying(500)");

                entity.Property(e => e.CustomerNumber).HasColumnType("character varying(250)");

                entity.Property(e => e.CustomerPhone).HasColumnType("character varying(50)");

                entity.Property(e => e.Description).HasColumnType("character varying");

                entity.Property(e => e.DiscountValue).HasColumnType("numeric(19,4)");

                entity.Property(e => e.LocationOfShipment).HasColumnType("character varying");

                entity.Property(e => e.MaxDebt).HasColumnType("numeric(18,2)");

                entity.Property(e => e.Note).HasColumnType("character varying");

                entity.Property(e => e.NoteTechnique).HasColumnType("character varying");

                entity.Property(e => e.OrderCode).HasColumnType("character varying(20)");

                entity.Property(e => e.PaymentMethodNote).HasColumnType("character varying(255)");

                entity.Property(e => e.PercentAdvance).HasColumnType("numeric(18,2)");

                entity.Property(e => e.PercentAdvanceType)
                    .IsRequired()
                    .HasDefaultValueSql("true");

                entity.Property(e => e.PlaceOfDelivery).HasColumnType("character varying");

                entity.Property(e => e.ReasonCancel).HasColumnType("character varying(50)");

                entity.Property(e => e.ReceiptInvoiceAmount).HasColumnType("numeric(18,2)");

                entity.Property(e => e.ReceivedDate).HasColumnType("date");

                entity.Property(e => e.ReceivedHour).HasColumnType("time without time zone");

                entity.Property(e => e.RecipientEmail).HasColumnType("character varying(50)");

                entity.Property(e => e.RecipientName).HasColumnType("character varying(250)");

                entity.Property(e => e.RecipientPhone).HasColumnType("character varying(50)");

                entity.Property(e => e.ShippingNote).HasColumnType("character varying");

                entity.Property(e => e.Vat)
                    .HasColumnName("VAT")
                    .HasColumnType("numeric(12,6)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CustomerOrderDetail>(entity =>
            {
                entity.HasKey(e => e.OrderDetailId);

                entity.Property(e => e.OrderDetailId).ValueGeneratedNever();

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("true");

                entity.Property(e => e.Description).HasColumnType("character varying");

                entity.Property(e => e.DiscountValue).HasColumnType("money");

                entity.Property(e => e.EmployeePhoneNumber).HasColumnType("character varying(255)");

                entity.Property(e => e.IncurredUnit).HasColumnType("character varying(50)");

                entity.Property(e => e.PriceInitial).HasColumnType("money");

                entity.Property(e => e.ProductCode).HasColumnType("character varying(250)");

                entity.Property(e => e.ProductColor).HasColumnType("character varying(300)");

                entity.Property(e => e.ProductColorCode).HasColumnType("character varying(300)");

                entity.Property(e => e.ProductGroupCode).HasColumnType("character varying(255)");

                entity.Property(e => e.ProductName).HasColumnType("character varying(500)");

                entity.Property(e => e.ReviewDetail).HasColumnType("character varying(255)");

                entity.Property(e => e.ReviewName).HasColumnType("character varying(255)");

                entity.Property(e => e.TechniqueDescription).HasColumnType("character varying");

                entity.Property(e => e.UnitLaborPrice)
                    .HasColumnType("money")
                    .HasDefaultValueSql("'$0.00'::money");

                entity.Property(e => e.UnitName).HasColumnType("character varying(250)");

                entity.Property(e => e.UnitPrice).HasColumnType("money");

                entity.Property(e => e.UserReview).HasColumnType("character varying(255)");

                entity.Property(e => e.Vat).HasColumnName("VAT");

                entity.Property(e => e.VendorId).HasColumnName("VendorID");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CustomerOrderExtension>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.ObjectId).HasColumnName("ObjectID");

                entity.Property(e => e.ObjectType).HasColumnType("character varying(255)");

                entity.Property(e => e.Value).HasColumnType("character varying(500)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CustomerOrderLocalPointMapping>(entity =>
            {
                entity.Property(e => e.CustomerOrderLocalPointMappingId).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CustomerOrderTask>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Note).HasColumnType("character varying");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CustomerServiceLevel>(entity =>
            {
                entity.Property(e => e.CustomerServiceLevelId).ValueGeneratedNever();

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("true");

                entity.Property(e => e.CustomerServiceLevelCode).HasColumnType("character varying(20)");

                entity.Property(e => e.CustomerServiceLevelName)
                    .IsRequired()
                    .HasColumnType("character varying(250)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<DanhGiaNhanVien>(entity =>
            {
                entity.Property(e => e.MucLuongDeXuatQuanLy).HasColumnName("MucLuongDeXuat_QuanLy");

                entity.Property(e => e.MucLuongDeXuatTruongPhong).HasColumnName("MucLuongDeXuat_TruongPhong");

                entity.Property(e => e.NhanXetTruongPhong)
                    .HasColumnName("NhanXet_TruongPhong")
                    .HasColumnType("character varying");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<DanhMucCauTraLoiDanhGiaMapping>(entity =>
            {
                entity.Property(e => e.DanhMucCauTraLoiDanhGiaMappingId).HasDefaultValueSql("nextval('\"DanhMucCauTraLoiDanhGiaMappin_DanhMucCauTraLoiDanhGiaMappin_seq\"'::regclass)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<DeNghiTamHoanUng>(entity =>
            {
                entity.Property(e => e.Active).HasDefaultValueSql("false");

                entity.Property(e => e.LoaiDeNghi).HasDefaultValueSql("0");

                entity.Property(e => e.LyDo).HasColumnType("character varying");

                entity.Property(e => e.MaDeNghi).HasColumnType("character varying(50)");

                entity.Property(e => e.TienTamUng).HasDefaultValueSql("'0'::numeric");

                entity.Property(e => e.TongTienThanhToan).HasDefaultValueSql("'0'::numeric");

                entity.Property(e => e.TrangThai).HasDefaultValueSql("0");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<DeNghiTamHoanUngChiTiet>(entity =>
            {
                entity.Property(e => e.ChiPhiKhac).HasDefaultValueSql("'0'::numeric");

                entity.Property(e => e.DinhKemCt)
                    .HasColumnName("DinhKemCT")
                    .HasColumnType("character varying");

                entity.Property(e => e.GhiChu).HasColumnType("character varying");

                entity.Property(e => e.KhachSan).HasDefaultValueSql("'0'::numeric");

                entity.Property(e => e.NoiDung).HasColumnType("character varying");

                entity.Property(e => e.SoHoaDon).HasColumnType("character varying(100)");

                entity.Property(e => e.TienDonDn)
                    .HasColumnName("TienDonDN")
                    .HasDefaultValueSql("'0'::numeric");

                entity.Property(e => e.TienDonHnnb)
                    .HasColumnName("TienDonHNNB")
                    .HasDefaultValueSql("'0'::numeric");

                entity.Property(e => e.TienSauVat)
                    .HasColumnName("TienSauVAT")
                    .HasDefaultValueSql("'0'::numeric");

                entity.Property(e => e.TongTienTruocVat)
                    .HasColumnName("TongTienTruocVAT")
                    .HasDefaultValueSql("'0'::numeric");

                entity.Property(e => e.VanChuyenXm)
                    .HasColumnName("VanChuyenXM")
                    .HasDefaultValueSql("'0'::numeric");

                entity.Property(e => e.Vat)
                    .HasColumnName("VAT")
                    .HasDefaultValueSql("'0'::numeric");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<DeNghiTamUng>(entity =>
            {
                entity.Property(e => e.GhiChu).HasColumnType("character varying");

                entity.Property(e => e.IsTamUng).HasDefaultValueSql("false");

                entity.Property(e => e.LyDoThanhToan).HasColumnType("character varying(500)");

                entity.Property(e => e.NoiDungThanhToan).HasColumnType("character varying(200)");

                entity.Property(e => e.SoTienCanThanhToan).HasDefaultValueSql("'0'::numeric");

                entity.Property(e => e.SoTienTamUng).HasDefaultValueSql("'0'::numeric");

                entity.Property(e => e.TrangThaiTamUng).HasDefaultValueSql("0");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<DeXuatCongTac>(entity =>
            {
                entity.Property(e => e.Active).HasDefaultValueSql("false");

                entity.Property(e => e.DiaDiem).HasColumnType("character varying(100)");

                entity.Property(e => e.DonVi).HasColumnType("character varying(100)");

                entity.Property(e => e.MaDeXuat).HasColumnType("character varying(50)");

                entity.Property(e => e.NhiemVu).HasColumnType("character varying(200)");

                entity.Property(e => e.PhuongTien).HasColumnType("character varying(100)");

                entity.Property(e => e.TenDeXuat)
                    .IsRequired()
                    .HasColumnType("character varying(500)");

                entity.Property(e => e.TrangThai).HasDefaultValueSql("0");

                entity.Property(e => e.TrangThaiDeXuat).HasDefaultValueSql("0");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<DeXuatNghi>(entity =>
            {
                entity.Property(e => e.Active).HasDefaultValueSql("true");

                entity.Property(e => e.DenCaChieu).HasColumnType("time without time zone");

                entity.Property(e => e.DenCaSang).HasColumnType("time without time zone");

                entity.Property(e => e.DenNgay).HasColumnType("date");

                entity.Property(e => e.LyDo).HasColumnType("character varying(200)");

                entity.Property(e => e.MaDeXuat).HasColumnType("character(20)");

                entity.Property(e => e.TuCaChieu).HasColumnType("time without time zone");

                entity.Property(e => e.TuCaSang).HasColumnType("time without time zone");

                entity.Property(e => e.TuNgay).HasColumnType("date");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<DeXuatTangLuong>(entity =>
            {
                entity.Property(e => e.Active).HasDefaultValueSql("true");

                entity.Property(e => e.GhiChu).HasColumnType("character varying(200)");

                entity.Property(e => e.NgayDeXuat).HasColumnType("date");

                entity.Property(e => e.TenDeXuat).HasColumnType("character varying(200)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<DeXuatTangLuongNhanVien>(entity =>
            {
                entity.Property(e => e.Active).HasDefaultValueSql("true");

                entity.Property(e => e.LuongDeXuat)
                    .HasColumnType("money")
                    .HasDefaultValueSql("'$0.00'::money");

                entity.Property(e => e.LuongHienTai)
                    .HasColumnType("money")
                    .HasDefaultValueSql("'$0.00'::money");

                entity.Property(e => e.LyDo).HasColumnType("character varying(500)");

                entity.Property(e => e.LyDoDeXuat).HasColumnType("character varying(200)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<DeXuatThayDoiChucVu>(entity =>
            {
                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("true");

                entity.Property(e => e.TenDeXuat)
                    .IsRequired()
                    .HasColumnType("character varying");

                entity.Property(e => e.TrangThai).HasDefaultValueSql("1");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<DeXuatXinNghi>(entity =>
            {
                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasColumnType("character varying(255)");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("now()");

                entity.Property(e => e.LyDo).HasColumnType("character varying");

                entity.Property(e => e.LyDoTuChoi).HasColumnType("character varying(3000)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<District>(entity =>
            {
                entity.Property(e => e.DistrictId).ValueGeneratedNever();

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("true");

                entity.Property(e => e.DistrictCode).HasColumnType("character varying(5)");

                entity.Property(e => e.DistrictName)
                    .IsRequired()
                    .HasColumnType("character varying(50)");

                entity.Property(e => e.DistrictType).HasColumnType("character varying(20)");

                entity.HasOne(d => d.Province)
                    .WithMany(p => p.District)
                    .HasForeignKey(d => d.ProvinceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_District_Province");
            });

            modelBuilder.Entity<Document>(entity =>
            {
                entity.Property(e => e.DocumentId).ValueGeneratedNever();

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("true");

                entity.Property(e => e.ContentType).HasColumnType("character varying");

                entity.Property(e => e.DocumentUrl).HasColumnType("character varying");

                entity.Property(e => e.Extension).HasColumnType("character varying(10)");

                entity.Property(e => e.Header).HasColumnType("character varying");

                entity.Property(e => e.Name).HasColumnType("character varying(200)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<DotKiemKe>(entity =>
            {
                entity.Property(e => e.TenDoiKiemKe)
                    .IsRequired()
                    .HasColumnType("character varying(500)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<EmailTemplate>(entity =>
            {
                entity.Property(e => e.EmailTemplateId).ValueGeneratedNever();

                entity.Property(e => e.EmailTemplateCode).HasColumnType("character varying(50)");

                entity.Property(e => e.EmailTemplateContent).HasColumnType("character varying");

                entity.Property(e => e.EmailTemplateName).HasColumnType("character varying(100)");

                entity.Property(e => e.EmailTemplateTitle).HasColumnType("character varying(100)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<EmailTemplateCcvalue>(entity =>
            {
                entity.ToTable("EmailTemplateCCValue");

                entity.Property(e => e.EmailTemplateCcvalueId)
                    .HasColumnName("EmailTemplateCCValueId")
                    .ValueGeneratedNever();

                entity.Property(e => e.Ccto)
                    .HasColumnName("CCTo")
                    .HasColumnType("character varying(20)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<EmailTemplateToken>(entity =>
            {
                entity.Property(e => e.EmailTemplateTokenId).ValueGeneratedNever();

                entity.Property(e => e.TokenCode).HasColumnType("character varying(30)");

                entity.Property(e => e.TokenLabel).HasColumnType("character varying(50)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.Property(e => e.EmployeeId).ValueGeneratedNever();

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("true");

                entity.Property(e => e.BienSo).HasColumnType("character varying(255)");

                entity.Property(e => e.ChiPhiTheoGio)
                    .HasColumnType("money")
                    .HasDefaultValueSql("'$0.00'::money");

                entity.Property(e => e.ChuyenNganhHoc).HasColumnType("character varying(255)");

                entity.Property(e => e.CodeMayChamCong).HasColumnType("character varying(255)");

                entity.Property(e => e.ContractEndDate).HasColumnType("date");

                entity.Property(e => e.DanToc).HasColumnType("character varying(255)");

                entity.Property(e => e.DiaDiemLamviec).HasColumnType("character varying(255)");

                entity.Property(e => e.DiemTest).HasColumnType("character varying(255)");

                entity.Property(e => e.EmployeeCode).HasColumnType("character varying(50)");

                entity.Property(e => e.EmployeeName)
                    .IsRequired()
                    .HasColumnType("character varying(200)");

                entity.Property(e => e.GradeTesting).HasColumnType("character varying(255)");

                entity.Property(e => e.HoTenTiengAnh).HasColumnType("character varying(255)");

                entity.Property(e => e.IsXacNhanTaiLieu)
                    .IsRequired()
                    .HasDefaultValueSql("true");

                entity.Property(e => e.KinhNghiemLamViec).HasColumnType("character varying");

                entity.Property(e => e.LoaiChuyenNganhHoc).HasColumnType("character varying(2)");

                entity.Property(e => e.LoaiXe).HasColumnType("character varying(255)");

                entity.Property(e => e.MaSoThueCaNhan).HasColumnType("character varying(255)");

                entity.Property(e => e.MaTest).HasColumnType("character varying(255)");

                entity.Property(e => e.QuocTich).HasColumnType("character varying(255)");

                entity.Property(e => e.TenMayChamCong).HasColumnType("character varying(255)");

                entity.Property(e => e.TenTruongHocCaoNhat).HasColumnType("character varying(255)");

                entity.Property(e => e.TomTatHocVan).HasColumnType("character varying(4000)");

                entity.Property(e => e.TonGiao).HasColumnType("character varying(255)");

                entity.Property(e => e.ViTriLamViec).HasColumnType("character varying(255)");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.Employee)
                    .HasForeignKey(d => d.OrganizationId)
                    .HasConstraintName("FK_Orgranization_Employee");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<EmployeeAllowance>(entity =>
            {
                entity.Property(e => e.EmployeeAllowanceId).ValueGeneratedNever();

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeAllowance)
                    .HasForeignKey(d => d.EmployeeId)
                    .HasConstraintName("FK_AllowanceSalary_Employee");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<EmployeeAssessment>(entity =>
            {
                entity.Property(e => e.EmployeeAssessmentId).ValueGeneratedNever();

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeAssessment)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeAssessment_Employee");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<EmployeeInsurance>(entity =>
            {
                entity.Property(e => e.EmployeeInsuranceId).ValueGeneratedNever();

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeInsurance)
                    .HasForeignKey(d => d.EmployeeId)
                    .HasConstraintName("FK_EmployeeInsurance_Employee");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<EmployeeMappingNotificationConfiguration>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<EmployeeMappingPermissionConfiguration>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<EmployeeMonthySalary>(entity =>
            {
                entity.Property(e => e.EmployeeMonthySalaryId).ValueGeneratedNever();

                entity.Property(e => e.BankAccountCode).HasColumnType("character varying(500)");

                entity.Property(e => e.BankAccountName).HasColumnType("character varying(500)");

                entity.Property(e => e.BranchOfBank).HasColumnType("character varying(500)");

                entity.Property(e => e.Description).HasColumnType("character varying");

                entity.Property(e => e.Email).HasColumnType("character varying(100)");

                entity.Property(e => e.EmployeeBranch).HasColumnType("character varying(500)");

                entity.Property(e => e.EmployeeCode).HasColumnType("character varying(50)");

                entity.Property(e => e.EmployeeName).HasColumnType("character varying(500)");

                entity.Property(e => e.EmployeeUnit).HasColumnType("character varying(500)");

                entity.Property(e => e.PostionName).HasColumnType("character varying(500)");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeMonthySalary)
                    .HasForeignKey(d => d.EmployeeId)
                    .HasConstraintName("FK_EmployeeMonthSalary_Employee");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<EmployeeRequest>(entity =>
            {
                entity.Property(e => e.EmployeeRequestId).ValueGeneratedNever();

                entity.Property(e => e.CreateEmployeeCode).HasColumnType("character varying(50)");

                entity.Property(e => e.Detail).HasColumnType("character varying");

                entity.Property(e => e.EmployeeRequestCode).HasColumnType("character varying(50)");

                entity.Property(e => e.NotifyList).HasColumnType("character varying");

                entity.Property(e => e.OfferEmployeeCode).HasColumnType("character varying(50)");

                entity.Property(e => e.RequestDate).HasColumnType("date");

                entity.HasOne(d => d.OfferEmployee)
                    .WithMany(p => p.EmployeeRequest)
                    .HasForeignKey(d => d.OfferEmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_EmployeeRequest_Employee");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<EmployeeSalary>(entity =>
            {
                entity.Property(e => e.EmployeeSalaryId).ValueGeneratedNever();

                entity.Property(e => e.EmployeeSalaryBase).HasColumnType("money");

                entity.Property(e => e.ResponsibilitySalary).HasColumnType("money");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeSalary)
                    .HasForeignKey(d => d.EmployeeId)
                    .HasConstraintName("FK_BasedSalary_Employee");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<EmployeeTimesheet>(entity =>
            {
                entity.Property(e => e.EmployeeTimesheetId).ValueGeneratedNever();

                entity.Property(e => e.Center).HasColumnType("character varying(500)");

                entity.Property(e => e.CheckIn).HasColumnType("time without time zone");

                entity.Property(e => e.CheckOut).HasColumnType("time without time zone");

                entity.Property(e => e.TimesheetDate).HasColumnType("date");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeTimesheet)
                    .HasForeignKey(d => d.EmployeeId)
                    .HasConstraintName("FK_EmployeeTimesheet_Employee");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ExternalUser>(entity =>
            {
                entity.Property(e => e.ExternalUserId).ValueGeneratedNever();

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnType("character varying");

                entity.Property(e => e.ResetCode).HasColumnType("character varying(50)");

                entity.Property(e => e.ResetCodeDate).HasColumnType("date");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasColumnType("character varying(50)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<FeatureNote>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Note)
                    .IsRequired()
                    .HasColumnType("character varying");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<FeatureWorkFlowProgress>(entity =>
            {
                entity.Property(e => e.FeatureWorkflowProgressId).ValueGeneratedNever();

                entity.Property(e => e.Comment).HasColumnType("character varying");

                entity.Property(e => e.Name).HasColumnType("character varying(250)");

                entity.Property(e => e.ProgressStatus).HasColumnType("character varying(250)");

                entity.Property(e => e.RecordStatus).HasColumnType("character varying(50)");

                entity.Property(e => e.SystemFeatureCode).HasColumnType("character varying(50)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<FileInFolder>(entity =>
            {
                entity.Property(e => e.FileInFolderId).ValueGeneratedNever();

                entity.Property(e => e.FileExtension).HasColumnType("character varying(50)");

                entity.Property(e => e.FileName)
                    .IsRequired()
                    .HasColumnType("character varying(200)");

                entity.Property(e => e.ObjectNumber).HasDefaultValueSql("0");

                entity.Property(e => e.ObjectType).HasColumnType("character(50)");

                entity.Property(e => e.Size).HasColumnType("character varying(200)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<FireBase>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.RoomName)
                    .IsRequired()
                    .HasColumnType("character varying(500)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Folder>(entity =>
            {
                entity.Property(e => e.FolderId).ValueGeneratedNever();

                entity.Property(e => e.FolderType).HasColumnType("character varying");

                entity.Property(e => e.IsDelete).HasColumnName("isDelete");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("character varying(200)");

                entity.Property(e => e.ObjectNumber).HasDefaultValueSql("0");

                entity.Property(e => e.Url)
                    .HasColumnName("URL")
                    .HasColumnType("character varying");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<GeographicalArea>(entity =>
            {
                entity.Property(e => e.GeographicalAreaId).ValueGeneratedNever();

                entity.Property(e => e.GeographicalAreaCode).HasColumnType("character varying(500)");

                entity.Property(e => e.GeographicalAreaName).HasColumnType("character varying(500)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Groups>(entity =>
            {
                entity.HasKey(e => e.GroupId);

                entity.Property(e => e.GroupId).ValueGeneratedNever();

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("true");

                entity.Property(e => e.Description).HasColumnType("character varying");

                entity.Property(e => e.GroupName)
                    .IsRequired()
                    .HasColumnType("character varying(50)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<GroupUser>(entity =>
            {
                entity.Property(e => e.GroupUserId).ValueGeneratedNever();

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("true");

                entity.Property(e => e.UpdatedDate).HasColumnType("date");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.GroupUser)
                    .HasForeignKey(d => d.GroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GroupUser_Group");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.GroupUser)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GroupUser_User");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Hash>(entity =>
            {
                entity.ToTable("hash", "hangfire");

                entity.HasIndex(e => new { e.Key, e.Field })
                    .HasName("hash_key_field_key")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('hangfire.hash_id_seq'::regclass)");

                entity.Property(e => e.Expireat).HasColumnName("expireat");

                entity.Property(e => e.Field)
                    .IsRequired()
                    .HasColumnName("field");

                entity.Property(e => e.Key)
                    .IsRequired()
                    .HasColumnName("key");

                entity.Property(e => e.Updatecount).HasColumnName("updatecount");

                entity.Property(e => e.Value).HasColumnName("value");
            });

            modelBuilder.Entity<HopDongNhanSu>(entity =>
            {
                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("true");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("now()");

                entity.Property(e => e.SoHopDong)
                    .IsRequired()
                    .HasColumnType("character varying(255)");

                entity.Property(e => e.SoPhuLuc)
                    .IsRequired()
                    .HasColumnType("character varying(255)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<HoSoCongTac>(entity =>
            {
                entity.Property(e => e.Active).HasDefaultValueSql("false");

                entity.Property(e => e.KetQua).HasColumnType("character varying");

                entity.Property(e => e.MaHoSoCongTac).HasColumnType("character varying(50)");

                entity.Property(e => e.TrangThai).HasDefaultValueSql("0");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<InforScreen>(entity =>
            {
                entity.Property(e => e.InforScreenId).ValueGeneratedNever();

                entity.Property(e => e.InforScreenCode)
                    .IsRequired()
                    .HasColumnType("character varying(500)");

                entity.Property(e => e.InforScreenName)
                    .IsRequired()
                    .HasColumnType("character varying(1000)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<InterviewSchedule>(entity =>
            {
                entity.Property(e => e.InterviewScheduleId).ValueGeneratedNever();

                entity.Property(e => e.Address).HasColumnType("character varying");

                entity.Property(e => e.InterviewTitle)
                    .IsRequired()
                    .HasColumnType("character varying");

                entity.Property(e => e.TypeOfInterview).HasColumnType("character varying(10)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<InterviewScheduleMapping>(entity =>
            {
                entity.Property(e => e.InterviewScheduleMappingId).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Inventory>(entity =>
            {
                entity.Property(e => e.InventoryId).ValueGeneratedNever();

                entity.Property(e => e.InventoryDetail).HasColumnType("character varying(500)");

                entity.Property(e => e.ProductService)
                    .IsRequired()
                    .HasColumnType("character varying(100)");

                entity.HasOne(d => d.InventoryStatusNavigation)
                    .WithMany(p => p.Inventory)
                    .HasForeignKey(d => d.InventoryStatus)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductSe__Produ__11558062");

                entity.HasOne(d => d.Vendor)
                    .WithMany(p => p.Inventory)
                    .HasForeignKey(d => d.VendorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductSe__Vendo__7F36D027");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<InventoryDeliveryVoucher>(entity =>
            {
                entity.Property(e => e.InventoryDeliveryVoucherId).ValueGeneratedNever();

                entity.Property(e => e.Description).HasColumnType("character varying");

                entity.Property(e => e.InventoryDeliveryVoucherCode)
                    .IsRequired()
                    .HasColumnType("character varying(50)");

                entity.Property(e => e.InventoryDeliveryVoucherTime).HasColumnType("time without time zone");

                entity.Property(e => e.Note).HasColumnType("character varying");

                entity.Property(e => e.Reason).HasColumnType("character varying(500)");

                entity.Property(e => e.Receiver).HasColumnType("character varying(200)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<InventoryDeliveryVoucherMapping>(entity =>
            {
                entity.Property(e => e.InventoryDeliveryVoucherMappingId).ValueGeneratedNever();

                entity.Property(e => e.PriceProduct).HasColumnType("money");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<InventoryDeliveryVoucherSerialMapping>(entity =>
            {
                entity.Property(e => e.InventoryDeliveryVoucherSerialMappingId).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<InventoryDetail>(entity =>
            {
                entity.Property(e => e.InventoryDetailId).ValueGeneratedNever();

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("true");

                entity.Property(e => e.InventoryDetailProductPrice).HasColumnType("money");

                entity.HasOne(d => d.InventoryDetailProduct)
                    .WithMany(p => p.InventoryDetail)
                    .HasForeignKey(d => d.InventoryDetailProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__InventoryDetail_Product");

                entity.HasOne(d => d.Inventory)
                    .WithMany(p => p.InventoryDetailNavigation)
                    .HasForeignKey(d => d.InventoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__InventoryDetail_Inventory");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<InventoryReceivingVoucher>(entity =>
            {
                entity.Property(e => e.InventoryReceivingVoucherId).ValueGeneratedNever();

                entity.Property(e => e.Description).HasColumnType("character varying(1000)");

                entity.Property(e => e.InventoryReceivingVoucherCode)
                    .IsRequired()
                    .HasColumnType("character varying(50)");

                entity.Property(e => e.InventoryReceivingVoucherTime).HasColumnType("time without time zone");

                entity.Property(e => e.Note).HasColumnType("character varying(1000)");

                entity.Property(e => e.ShiperName).HasColumnType("character varying(200)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<InventoryReceivingVoucherMapping>(entity =>
            {
                entity.Property(e => e.InventoryReceivingVoucherMappingId).ValueGeneratedNever();

                entity.Property(e => e.PriceProduct).HasColumnType("money");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<InventoryReceivingVoucherSerialMapping>(entity =>
            {
                entity.Property(e => e.InventoryReceivingVoucherSerialMappingId).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<InventoryReport>(entity =>
            {
                entity.Property(e => e.InventoryReportId).ValueGeneratedNever();

                entity.Property(e => e.Note).HasColumnType("character varying");

                entity.Property(e => e.OpeningBalance).HasColumnType("money");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Invoices>(entity =>
            {
                entity.HasKey(e => e.InvoiceId);

                entity.Property(e => e.InvoiceId).ValueGeneratedNever();

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("true");

                entity.Property(e => e.ObjectType)
                    .IsRequired()
                    .HasColumnType("character varying(20)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Job>(entity =>
            {
                entity.ToTable("job", "hangfire");

                entity.HasIndex(e => e.Statename)
                    .HasName("ix_hangfire_job_statename");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('hangfire.job_id_seq'::regclass)");

                entity.Property(e => e.Arguments)
                    .IsRequired()
                    .HasColumnName("arguments");

                entity.Property(e => e.Createdat).HasColumnName("createdat");

                entity.Property(e => e.Expireat).HasColumnName("expireat");

                entity.Property(e => e.Invocationdata)
                    .IsRequired()
                    .HasColumnName("invocationdata");

                entity.Property(e => e.Stateid).HasColumnName("stateid");

                entity.Property(e => e.Statename).HasColumnName("statename");

                entity.Property(e => e.Updatecount).HasColumnName("updatecount");
            });

            modelBuilder.Entity<Jobparameter>(entity =>
            {
                entity.ToTable("jobparameter", "hangfire");

                entity.HasIndex(e => new { e.Jobid, e.Name })
                    .HasName("ix_hangfire_jobparameter_jobidandname");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('hangfire.jobparameter_id_seq'::regclass)");

                entity.Property(e => e.Jobid).HasColumnName("jobid");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");

                entity.Property(e => e.Updatecount).HasColumnName("updatecount");

                entity.Property(e => e.Value).HasColumnName("value");

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.Jobparameter)
                    .HasForeignKey(d => d.Jobid)
                    .HasConstraintName("jobparameter_jobid_fkey");
            });

            modelBuilder.Entity<Jobqueue>(entity =>
            {
                entity.ToTable("jobqueue", "hangfire");

                entity.HasIndex(e => new { e.Jobid, e.Queue })
                    .HasName("ix_hangfire_jobqueue_jobidandqueue");

                entity.HasIndex(e => new { e.Queue, e.Fetchedat })
                    .HasName("ix_hangfire_jobqueue_queueandfetchedat");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('hangfire.jobqueue_id_seq'::regclass)");

                entity.Property(e => e.Fetchedat).HasColumnName("fetchedat");

                entity.Property(e => e.Jobid).HasColumnName("jobid");

                entity.Property(e => e.Queue)
                    .IsRequired()
                    .HasColumnName("queue");

                entity.Property(e => e.Updatecount).HasColumnName("updatecount");
            });

            modelBuilder.Entity<KeHoachOt>(entity =>
            {
                entity.Property(e => e.Active).HasDefaultValueSql("true");

                entity.Property(e => e.DiaDiem).HasColumnType("character varying(500)");

                entity.Property(e => e.GhiChu).HasColumnType("character varying(3000)");

                entity.Property(e => e.GioBatDau).HasColumnType("time without time zone");

                entity.Property(e => e.GioKetThuc).HasColumnType("time without time zone");

                entity.Property(e => e.LyDo).HasColumnType("character varying(200)");

                entity.Property(e => e.TenKeHoach).HasColumnType("character varying(50)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<KeHoachOtThanhVien>(entity =>
            {
                entity.HasKey(e => e.ThanVienOtId);

                entity.Property(e => e.Active).HasDefaultValueSql("true");

                entity.Property(e => e.GhiChu).HasColumnType("character varying(500)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<KichHoatTinhHuong>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.NoiDung)
                    .IsRequired()
                    .HasColumnType("character varying(255)");

                entity.Property(e => e.Session).HasColumnType("character varying(255)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<KichHoatTinhHuongChiTiet>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.HoVaTen).HasColumnType("character varying(50)");

                entity.Property(e => e.NoiDung)
                    .IsRequired()
                    .HasColumnType("character varying(255)");

                entity.Property(e => e.PhanHoi).HasColumnType("character varying(255)");

                entity.Property(e => e.Sdt)
                    .IsRequired()
                    .HasColumnType("character varying(20)");

                entity.Property(e => e.Session).HasColumnType("character varying(255)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<KyDanhGia>(entity =>
            {
                entity.Property(e => e.LyDo).HasColumnType("character varying");

                entity.Property(e => e.MaKyDanhGia).HasColumnType("character varying(100)");

                entity.Property(e => e.TenKyDanhGia)
                    .IsRequired()
                    .HasColumnType("character varying(500)");

                entity.Property(e => e.TrangThaiDanhGia).HasDefaultValueSql("0");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<KyLuong>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("now()");

                entity.Property(e => e.DenNgay).HasColumnType("date");

                entity.Property(e => e.LyDoTuChoi).HasColumnType("character varying(3000)");

                entity.Property(e => e.TenKyLuong)
                    .IsRequired()
                    .HasColumnType("character varying(500)");

                entity.Property(e => e.TuNgay).HasColumnType("date");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Lead>(entity =>
            {
                entity.Property(e => e.LeadId).ValueGeneratedNever();

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("true");

                entity.Property(e => e.CreatedById)
                    .IsRequired()
                    .HasColumnType("character varying(50)");

                entity.Property(e => e.ExpectedSale).HasColumnType("money");

                entity.Property(e => e.ForecastSales).HasColumnType("money");

                entity.Property(e => e.LeadCode).HasColumnType("character varying(30)");

                entity.Property(e => e.RequirementDetail).HasColumnType("character varying");

                entity.Property(e => e.Role).HasColumnType("character varying(50)");

                entity.Property(e => e.UpdatedById).HasColumnType("character varying(50)");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Lead)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("FK_Lead_Company");

                entity.HasOne(d => d.InterestedGroup)
                    .WithMany(p => p.LeadInterestedGroup)
                    .HasForeignKey(d => d.InterestedGroupId)
                    .HasConstraintName("FK_Lead_GroupProduct");

                entity.HasOne(d => d.PaymentMethod)
                    .WithMany(p => p.LeadPaymentMethod)
                    .HasForeignKey(d => d.PaymentMethodId)
                    .HasConstraintName("FK_Lead_PaymentMethod");

                entity.HasOne(d => d.Potential)
                    .WithMany(p => p.LeadPotential)
                    .HasForeignKey(d => d.PotentialId)
                    .HasConstraintName("FK_Lead_Potential");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.LeadStatus)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Lead_Status");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<LeadCare>(entity =>
            {
                entity.Property(e => e.LeadCareId).ValueGeneratedNever();

                entity.Property(e => e.DiscountAmount).HasColumnType("money");

                entity.Property(e => e.LeadCareCode).HasColumnType("character varying(50)");

                entity.Property(e => e.LeadCareContent).HasColumnType("character varying");

                entity.Property(e => e.LeadCareContentEmail).HasColumnType("character varying");

                entity.Property(e => e.LeadCareContentSms)
                    .HasColumnName("LeadCareContentSMS")
                    .HasColumnType("character varying");

                entity.Property(e => e.LeadCareEventHour).HasColumnType("time without time zone");

                entity.Property(e => e.LeadCareTitle).HasColumnType("character varying");

                entity.Property(e => e.LeadCareVoucher).HasColumnType("character varying(100)");

                entity.Property(e => e.SendDate).HasColumnType("date");

                entity.Property(e => e.SendEmailDate).HasColumnType("date");

                entity.Property(e => e.SendEmailHour).HasColumnType("time without time zone");

                entity.Property(e => e.SendHour).HasColumnType("time without time zone");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<LeadCareFeedBack>(entity =>
            {
                entity.Property(e => e.LeadCareFeedBackId).ValueGeneratedNever();

                entity.Property(e => e.FeedBackContent).HasColumnType("character varying");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<LeadCareFilter>(entity =>
            {
                entity.Property(e => e.LeadCareFilterId).ValueGeneratedNever();

                entity.Property(e => e.QueryContent).HasColumnType("character varying");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<LeadCareLead>(entity =>
            {
                entity.Property(e => e.LeadCareLeadId).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<LeadDetail>(entity =>
            {
                entity.Property(e => e.LeadDetailId).ValueGeneratedNever();

                entity.Property(e => e.Description).HasColumnType("character varying");

                entity.Property(e => e.DiscountValue).HasColumnType("money");

                entity.Property(e => e.IncurredUnit).HasColumnType("character varying(50)");

                entity.Property(e => e.ProductName).HasColumnType("character varying(200)");

                entity.Property(e => e.UnitLaborPrice)
                    .HasColumnType("money")
                    .HasDefaultValueSql("'$0.00'::money");

                entity.Property(e => e.UnitPrice).HasColumnType("money");

                entity.Property(e => e.Vat).HasColumnName("VAT");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<LeadInterestedGroupMapping>(entity =>
            {
                entity.Property(e => e.LeadInterestedGroupMappingId).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<LeadMeeting>(entity =>
            {
                entity.Property(e => e.LeadMeetingId).ValueGeneratedNever();

                entity.Property(e => e.Content).HasColumnType("character varying");

                entity.Property(e => e.EndHours).HasColumnType("time without time zone");

                entity.Property(e => e.LocationMeeting).HasColumnType("character varying");

                entity.Property(e => e.Participant).HasColumnType("character varying");

                entity.Property(e => e.StartHours).HasColumnType("time without time zone");

                entity.Property(e => e.Title).HasColumnType("character varying(500)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<LeadProductDetailProductAttributeValue>(entity =>
            {
                entity.HasKey(e => e.LeadProductDetailProductAttributeValue1);

                entity.Property(e => e.LeadProductDetailProductAttributeValue1)
                    .HasColumnName("LeadProductDetailProductAttributeValue")
                    .ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<LichSuPheDuyet>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.LyDo).HasColumnType("character varying(3000)");

                entity.Property(e => e.ObjectNumber).HasDefaultValueSql("0");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<LichSuThanhToanBaoHiem>(entity =>
            {
                entity.Property(e => e.GhiChu).HasColumnType("character varying");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<LinkOfDocument>(entity =>
            {
                entity.Property(e => e.LinkOfDocumentId).ValueGeneratedNever();

                entity.Property(e => e.LinkName).HasColumnType("character varying");

                entity.Property(e => e.LinkValue).HasColumnType("character varying");

                entity.Property(e => e.ObjectType).HasColumnType("character varying");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<List>(entity =>
            {
                entity.ToTable("list", "hangfire");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('hangfire.list_id_seq'::regclass)");

                entity.Property(e => e.Expireat).HasColumnName("expireat");

                entity.Property(e => e.Key)
                    .IsRequired()
                    .HasColumnName("key");

                entity.Property(e => e.Updatecount).HasColumnName("updatecount");

                entity.Property(e => e.Value).HasColumnName("value");
            });

            modelBuilder.Entity<LocalAddress>(entity =>
            {
                entity.Property(e => e.LocalAddressId).ValueGeneratedNever();

                entity.Property(e => e.Address).HasColumnType("character varying(1000)");

                entity.Property(e => e.Description).HasColumnType("character varying(2000)");

                entity.Property(e => e.LocalAddressCode).HasColumnType("character varying(100)");

                entity.Property(e => e.LocalAddressName).HasColumnType("character varying(100)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<LocalPoint>(entity =>
            {
                entity.Property(e => e.LocalPointId).ValueGeneratedNever();

                entity.Property(e => e.Address).HasColumnType("character varying(1000)");

                entity.Property(e => e.Description).HasColumnType("character varying(2000)");

                entity.Property(e => e.LocalPointCode).HasColumnType("character varying(100)");

                entity.Property(e => e.LocalPointName).HasColumnType("character varying(100)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<LoginAuditTrace>(entity =>
            {
                entity.Property(e => e.LoginAuditTraceId).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<LuongCtCtyDong>(entity =>
            {
                entity.Property(e => e.FundOct).HasColumnName("Fund_Oct");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<LuongTongHop>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("now()");

                entity.Property(e => e.SoNgayMucLuongCu).HasColumnName("SoNgay_MucLuongCu");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<MenuBuild>(entity =>
            {
                entity.Property(e => e.MenuBuildId).ValueGeneratedNever();

                entity.Property(e => e.Code).HasColumnType("character varying(100)");

                entity.Property(e => e.CodeParent).HasColumnType("character varying(100)");

                entity.Property(e => e.IsShow)
                    .IsRequired()
                    .HasDefaultValueSql("true");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("character varying(500)");

                entity.Property(e => e.NameIcon).HasColumnType("character varying(100)");

                entity.Property(e => e.Path).HasColumnType("character varying(2000)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<MilestoneConfiguration>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Content).HasColumnType("character varying");

                entity.Property(e => e.Name).HasColumnType("character varying");

                entity.Property(e => e.Note).HasColumnType("character varying");

                entity.Property(e => e.ScopeReport).HasColumnType("character varying(255)");

                entity.Property(e => e.Status).HasColumnType("character varying(255)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<MinusItemMapping>(entity =>
            {
                entity.Property(e => e.MinusItemMappingId).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<MobileAppConfiguration>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.IntroduceColor).HasColumnType("character varying(50)");

                entity.Property(e => e.IntroduceImageOrVideo).HasColumnType("character varying");

                entity.Property(e => e.IntroduceSologan).HasColumnType("character varying(255)");

                entity.Property(e => e.IsPaymentScreenIconVnpay).HasColumnName("IsPaymentScreenIconVNPAY");

                entity.Property(e => e.LoginAndRegisterScreenImage).HasColumnType("character varying");

                entity.Property(e => e.LoginScreenColor).HasColumnType("character varying(50)");

                entity.Property(e => e.LoginScreenIcon).HasColumnType("character varying");

                entity.Property(e => e.OrderNotificationImage).HasColumnType("character varying");

                entity.Property(e => e.PaymentScreenContentTransfer).HasColumnType("character varying(255)");

                entity.Property(e => e.PaymentScreenContentVnpay)
                    .HasColumnName("PaymentScreenContentVNPAY")
                    .HasColumnType("character varying(255)");

                entity.Property(e => e.PaymentScreenIconTransfer).HasColumnType("character varying");

                entity.Property(e => e.PaymentScreenIconVnpay)
                    .HasColumnName("PaymentScreenIconVNPAY")
                    .HasColumnType("character varying");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<MucDanhGia>(entity =>
            {
                entity.Property(e => e.DiemDanhGia).HasDefaultValueSql("'0'::numeric");

                entity.Property(e => e.TenMucDanhGia)
                    .IsRequired()
                    .HasColumnType("character varying(100)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<MucDanhGiaDanhGiaMapping>(entity =>
            {
                entity.Property(e => e.DiemDen).HasDefaultValueSql("'0'::numeric");

                entity.Property(e => e.DiemTu).HasDefaultValueSql("'0'::numeric");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<MucHuongBaoHiemLoftCare>(entity =>
            {
                entity.Property(e => e.DoiTuongHuong).HasDefaultValueSql("1");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<NhanVienDeXuatThayDoiChucVu>(entity =>
            {
                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("true");

                entity.Property(e => e.LyDo).HasColumnType("character varying(500)");

                entity.Property(e => e.LyDoDeXuat).HasColumnType("character varying");

                entity.Property(e => e.NghiaVu).HasColumnType("character varying");

                entity.Property(e => e.TrangThai).HasDefaultValueSql("1");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<NhanVienKyDanhGia>(entity =>
            {
                entity.Property(e => e.TrangThai).HasDefaultValueSql("1");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<NhomBaoHiemLoftCare>(entity =>
            {
                entity.Property(e => e.TenNhom).HasColumnType("character varying");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Note>(entity =>
            {
                entity.Property(e => e.NoteId).ValueGeneratedNever();

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("true");

                entity.Property(e => e.Description).HasColumnType("character varying");

                entity.Property(e => e.NoteTitle)
                    .IsRequired()
                    .HasColumnType("character varying(200)");

                entity.Property(e => e.ObjectNumber).HasDefaultValueSql("0");

                entity.Property(e => e.ObjectType)
                    .HasColumnType("character varying(15)")
                    .HasDefaultValueSql("'LD'::character varying");

                entity.Property(e => e.Type)
                    .HasColumnType("character(3)")
                    .HasDefaultValueSql("'N'::bpchar");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<NoteDocument>(entity =>
            {
                entity.Property(e => e.NoteDocumentId).ValueGeneratedNever();

                entity.Property(e => e.DocumentName).HasColumnType("character varying(100)");

                entity.Property(e => e.DocumentSize).HasColumnType("character varying(20)");

                entity.Property(e => e.DocumentUrl).HasColumnType("character varying(300)");

                entity.Property(e => e.ObjectNumber).HasDefaultValueSql("0");

                entity.HasOne(d => d.Note)
                    .WithMany(p => p.NoteDocument)
                    .HasForeignKey(d => d.NoteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__NoteDocum__NoteI__4FBCC72F");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<NotifiAction>(entity =>
            {
                entity.Property(e => e.NotifiActionId).ValueGeneratedNever();

                entity.Property(e => e.NotifiActionCode)
                    .IsRequired()
                    .HasColumnType("character varying(500)");

                entity.Property(e => e.NotifiActionName)
                    .IsRequired()
                    .HasColumnType("character varying(1000)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Notifications>(entity =>
            {
                entity.HasKey(e => e.NotificationId);

                entity.Property(e => e.NotificationId).ValueGeneratedNever();

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasColumnType("character varying(250)");

                entity.Property(e => e.NotificationType)
                    .IsRequired()
                    .HasColumnType("character varying(10)");

                entity.Property(e => e.ObjectType)
                    .IsRequired()
                    .HasColumnType("character varying(50)");

                entity.HasOne(d => d.Receiver)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.ReceiverId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Notificat__Recei__29E1370A");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<NotifiCondition>(entity =>
            {
                entity.Property(e => e.NotifiConditionId).ValueGeneratedNever();

                entity.Property(e => e.NotifiConditionName)
                    .IsRequired()
                    .HasColumnType("character varying(1000)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<NotificationConfiguration>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);

            });

            modelBuilder.Entity<NotifiSetting>(entity =>
            {
                entity.Property(e => e.NotifiSettingId).ValueGeneratedNever();

                entity.Property(e => e.CustomerEmailContent).HasColumnType("character varying");

                entity.Property(e => e.CustomerEmailTitle).HasColumnType("character varying(1000)");

                entity.Property(e => e.CustomerSmsContent).HasColumnType("character varying");

                entity.Property(e => e.CustomerSmsTitle).HasColumnType("character varying(1000)");

                entity.Property(e => e.EmailContent).HasColumnType("character varying");

                entity.Property(e => e.EmailTitle).HasColumnType("character varying(1000)");

                entity.Property(e => e.NotifiSettingName)
                    .IsRequired()
                    .HasColumnType("character varying(1000)");

                entity.Property(e => e.SmsContent).HasColumnType("character varying");

                entity.Property(e => e.SmsTitle).HasColumnType("character varying(1000)");

                entity.Property(e => e.SystemContent).HasColumnType("character varying");

                entity.Property(e => e.SystemTitle).HasColumnType("character varying(1000)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<NotifiSettingCondition>(entity =>
            {
                entity.Property(e => e.NotifiSettingConditionId).ValueGeneratedNever();

                entity.Property(e => e.StringValue).HasColumnType("character varying");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<NotifiSettingToken>(entity =>
            {
                entity.Property(e => e.NotifiSettingTokenId).ValueGeneratedNever();

                entity.Property(e => e.TokenCode).HasColumnType("character varying(100)");

                entity.Property(e => e.TokenLabel).HasColumnType("character varying(100)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<NotifiSpecial>(entity =>
            {
                entity.Property(e => e.NotifiSpecialId).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Options>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description).HasColumnType("character varying");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("character varying(255)");

                entity.Property(e => e.Note).HasColumnType("character varying");

                entity.Property(e => e.Price).HasColumnType("numeric(18,2)");

                entity.Property(e => e.Vat).HasColumnName("VAT").HasColumnType("numeric(18,2)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<OrderCostDetail>(entity =>
            {
                entity.Property(e => e.OrderCostDetailId).ValueGeneratedNever();

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("true");

                entity.Property(e => e.CostName).HasColumnType("character varying(500)");

                entity.Property(e => e.UnitPrice).HasColumnType("money");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<OrderProductDetailProductAttributeValue>(entity =>
            {
                entity.Property(e => e.OrderProductDetailProductAttributeValueId).ValueGeneratedNever();

                entity.HasOne(d => d.OrderDetail)
                    .WithMany(p => p.OrderProductDetailProductAttributeValue)
                    .HasForeignKey(d => d.OrderDetailId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderProductDetailProductAttributeValue_CustomerOrderDetail");

                entity.HasOne(d => d.ProductAttributeCategory)
                    .WithMany(p => p.OrderProductDetailProductAttributeValue)
                    .HasForeignKey(d => d.ProductAttributeCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderProductDetailProductAttributeValue_ProductAttributeCate");

                entity.HasOne(d => d.ProductAttributeCategoryValue)
                    .WithMany(p => p.OrderProductDetailProductAttributeValue)
                    .HasForeignKey(d => d.ProductAttributeCategoryValueId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderProductDetailProductAttribute_ProductAttributeCategoryV");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderProductDetailProductAttributeValue)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderProductDetailProductAttributeValue_Product");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<OrderStatus>(entity =>
            {
                entity.Property(e => e.OrderStatusId).ValueGeneratedNever();

                entity.Property(e => e.Description).HasColumnType("character varying");

                entity.Property(e => e.OrderStatusCode)
                    .IsRequired()
                    .HasColumnType("character varying(20)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<OrderTechniqueMapping>(entity =>
            {
                entity.Property(e => e.OrderTechniqueMappingId).ValueGeneratedNever();

                entity.Property(e => e.Active).HasDefaultValueSql("true");

                entity.Property(e => e.Rate).HasDefaultValueSql("'1'::smallint");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Organization>(entity =>
            {
                entity.Property(e => e.OrganizationId).ValueGeneratedNever();

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("true");

                entity.Property(e => e.IsAccess).HasDefaultValueSql("false");

                entity.Property(e => e.IsHr)
                    .HasColumnName("IsHR")
                    .HasDefaultValueSql("false");

                entity.Property(e => e.OrganizationCode)
                    .IsRequired()
                    .HasColumnType("character varying(20)");

                entity.Property(e => e.OrganizationName)
                    .IsRequired()
                    .HasColumnType("character varying(100)");

                entity.Property(e => e.OrganizationOtherCode).HasColumnType("character varying(500)");

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId)
                    .HasConstraintName("FK_Department_Department");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<OverviewCandidate>(entity =>
            {
                entity.Property(e => e.OverviewCandidateId).ValueGeneratedNever();

                entity.Property(e => e.CertificatePlace).HasColumnType("character varying");

                entity.Property(e => e.EducationAndWorkExpName)
                    .IsRequired()
                    .HasColumnType("character varying");

                entity.Property(e => e.JobDescription).HasColumnType("character varying");

                entity.Property(e => e.Phone).HasColumnType("character varying(50)");

                entity.Property(e => e.ProficiencyLevel).HasColumnType("character varying");

                entity.Property(e => e.SpecializedTraining).HasColumnType("character varying");

                entity.Property(e => e.Type).HasColumnType("character varying(20)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<PartItemMapping>(entity =>
            {
                entity.Property(e => e.PartItemMappingId).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<PayableInvoice>(entity =>
            {
                entity.Property(e => e.PayableInvoiceId).ValueGeneratedNever();

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.AmountText).HasColumnType("character varying(250)");

                entity.Property(e => e.PayableInvoiceCode).HasColumnType("character varying(50)");

                entity.Property(e => e.PayableInvoiceDetail).HasColumnType("character varying(500)");

                entity.Property(e => e.PayableInvoiceNote).HasColumnType("character varying");

                entity.Property(e => e.PayableInvoicePrice).HasColumnType("money");

                entity.Property(e => e.RecipientAddress).HasColumnType("character varying");

                entity.Property(e => e.RecipientName).HasColumnType("character varying(250)");

                entity.Property(e => e.UnitPrice).HasColumnType("money");

                entity.HasOne(d => d.CurrencyUnitNavigation)
                    .WithMany(p => p.PayableInvoiceCurrencyUnitNavigation)
                    .HasForeignKey(d => d.CurrencyUnit)
                    .HasConstraintName("FK__PayableIn__Curre__22A007F5");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.PayableInvoice)
                    .HasForeignKey(d => d.OrganizationId)
                    .HasConstraintName("FK__PayableIn__Organ__3489AE06");

                entity.HasOne(d => d.PayableInvoicePriceCurrencyNavigation)
                    .WithMany(p => p.PayableInvoicePayableInvoicePriceCurrencyNavigation)
                    .HasForeignKey(d => d.PayableInvoicePriceCurrency)
                    .HasConstraintName("FK__PayableInvoice_Category");

                entity.HasOne(d => d.PayableInvoiceReasonNavigation)
                    .WithMany(p => p.PayableInvoicePayableInvoiceReasonNavigation)
                    .HasForeignKey(d => d.PayableInvoiceReason)
                    .HasConstraintName("FK__PayableIn__Payab__1ECF7711");

                entity.HasOne(d => d.RegisterTypeNavigation)
                    .WithMany(p => p.PayableInvoiceRegisterTypeNavigation)
                    .HasForeignKey(d => d.RegisterType)
                    .HasConstraintName("FK__PayableIn__Regis__1FC39B4A");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.PayableInvoiceStatus)
                    .HasForeignKey(d => d.StatusId)
                    .HasConstraintName("FK__PayableIn__Statu__21ABE3BC");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<PayableInvoiceMapping>(entity =>
            {
                entity.Property(e => e.PayableInvoiceMappingId).ValueGeneratedNever();

                entity.HasOne(d => d.PayableInvoice)
                    .WithMany(p => p.PayableInvoiceMapping)
                    .HasForeignKey(d => d.PayableInvoiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__PayableIn__Payab__257C74A0");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Permission>(entity =>
            {
                entity.Property(e => e.PermissionId).ValueGeneratedNever();

                entity.Property(e => e.IconClass).HasColumnType("character varying(200)");

                entity.Property(e => e.PermissionCode)
                    .IsRequired()
                    .HasColumnType("character varying(50)");

                entity.Property(e => e.PermissionDescription).HasColumnType("character varying");

                entity.Property(e => e.PermissionName)
                    .IsRequired()
                    .HasColumnType("character varying");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasColumnType("character varying(1)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<PermissionConfiguration>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<PermissionMapping>(entity =>
            {
                entity.Property(e => e.PermissionMappingId).ValueGeneratedNever();

                entity.Property(e => e.UseFor)
                    .IsRequired()
                    .HasColumnType("character varying(1)");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.PermissionMapping)
                    .HasForeignKey(d => d.GroupId)
                    .HasConstraintName("FK_PermissionMapping_Group");

                entity.HasOne(d => d.PermissionSet)
                    .WithMany(p => p.PermissionMapping)
                    .HasForeignKey(d => d.PermissionSetId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PermisisonSetMapping");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.PermissionMapping)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_PermissionMapping_User");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<PermissionSet>(entity =>
            {
                entity.Property(e => e.PermissionSetId).ValueGeneratedNever();

                entity.Property(e => e.PermissionId).HasColumnType("character varying");

                entity.Property(e => e.PermissionSetCode)
                    .IsRequired()
                    .HasColumnType("character varying(20)");

                entity.Property(e => e.PermissionSetDescription)
                    .IsRequired()
                    .HasColumnType("character varying(100)");

                entity.Property(e => e.PermissionSetName)
                    .IsRequired()
                    .HasColumnType("character varying(50)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<PhieuDanhGia>(entity =>
            {
                entity.Property(e => e.MaPhieuDanhGia).HasColumnType("character varying(30)");

                entity.Property(e => e.TenPhieuDanhGia)
                    .IsRequired()
                    .HasColumnType("character varying");

                entity.Property(e => e.TrangThaiPhieuDanhGia).HasDefaultValueSql("0");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<PhieuLuong>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("now()");

                entity.Property(e => e.DuocHuongTroCapChuyenCan)
                    .IsRequired()
                    .HasColumnType("character varying(255)");

                entity.Property(e => e.DuocHuongTroCapKpi)
                    .IsRequired()
                    .HasColumnType("character varying(255)");

                entity.Property(e => e.NamBatDauKyLuong)
                    .IsRequired()
                    .HasColumnType("character varying(255)");

                entity.Property(e => e.NamKetThucKyLuong)
                    .IsRequired()
                    .HasColumnType("character varying(255)");

                entity.Property(e => e.NamTheoThangTruoc)
                    .IsRequired()
                    .HasColumnType("character varying(255)");

                entity.Property(e => e.NgayBatDauKyLuong)
                    .IsRequired()
                    .HasColumnType("character varying(255)");

                entity.Property(e => e.NgayKetThucKyLuong)
                    .IsRequired()
                    .HasColumnType("character varying(255)");

                entity.Property(e => e.PhieuLuongCode)
                    .IsRequired()
                    .HasColumnType("character varying(255)");

                entity.Property(e => e.ThangBatDauKyLuong)
                    .IsRequired()
                    .HasColumnType("character varying(255)");

                entity.Property(e => e.ThangKetThucKyLuong)
                    .IsRequired()
                    .HasColumnType("character varying(255)");

                entity.Property(e => e.ThangTruoc)
                    .IsRequired()
                    .HasColumnType("character varying(255)");

                entity.Property(e => e.ThangTruocTiengAnh)
                    .IsRequired()
                    .HasColumnType("character varying(255)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<PhongBanApDung>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<PhongBanTrongCacBuocQuyTrinh>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Position>(entity =>
            {
                entity.Property(e => e.PositionId).ValueGeneratedNever();

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("true");

                entity.Property(e => e.PositionCode).HasColumnType("character varying(50)");

                entity.Property(e => e.PositionName)
                    .IsRequired()
                    .HasColumnType("character varying(50)");

                entity.Property(e => e.TenTiengAnh).HasColumnType("character varying(255)");

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId)
                    .HasConstraintName("FK_Position_Position");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<PotentialCustomerProduct>(entity =>
            {
                entity.Property(e => e.PotentialCustomerProductId).ValueGeneratedNever();

                entity.Property(e => e.ProductFixedPrice).HasColumnType("money");

                entity.Property(e => e.ProductName).HasColumnType("character varying(100)");

                entity.Property(e => e.ProductNote).HasColumnType("character varying");

                entity.Property(e => e.ProductUnit).HasColumnType("character varying(20)");

                entity.Property(e => e.ProductUnitPrice).HasColumnType("money");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<PriceProduct>(entity =>
            {
                entity.Property(e => e.PriceProductId).ValueGeneratedNever();

                entity.Property(e => e.MinQuantity).HasDefaultValueSql("'1'::numeric");

                entity.Property(e => e.PriceForeignMoney).HasColumnType("money");

                entity.Property(e => e.PriceVnd)
                    .HasColumnName("PriceVND")
                    .HasColumnType("money");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<PriceSuggestedSupplierQuotesMapping>(entity =>
            {
                entity.Property(e => e.PriceSuggestedSupplierQuotesMappingId).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ProcurementPlan>(entity =>
            {
                entity.Property(e => e.ProcurementPlanId).ValueGeneratedNever();

                entity.Property(e => e.ProcurementAmount).HasColumnType("money");

                entity.Property(e => e.ProcurementContent).HasColumnType("character varying");

                entity.Property(e => e.ProcurementPlanCode).HasColumnType("character varying(100)");

                entity.HasOne(d => d.ProductCategory)
                    .WithMany(p => p.ProcurementPlan)
                    .HasForeignKey(d => d.ProductCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Procureme__Produ__3B0BC30C");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ProcurementRequest>(entity =>
            {
                entity.Property(e => e.ProcurementRequestId).ValueGeneratedNever();

                entity.Property(e => e.EmployeePhone).HasColumnType("character varying(50)");

                entity.Property(e => e.Explain).HasColumnType("character varying");

                entity.Property(e => e.ProcurementCode).HasColumnType("character varying(100)");

                entity.Property(e => e.ProcurementContent).HasColumnType("character varying");

                entity.HasOne(d => d.Approver)
                    .WithMany(p => p.ProcurementRequestApprover)
                    .HasForeignKey(d => d.ApproverId)
                    .HasConstraintName("FK_ProcurementRequest_EmployeeApprove");

                entity.HasOne(d => d.RequestEmployee)
                    .WithMany(p => p.ProcurementRequestRequestEmployee)
                    .HasForeignKey(d => d.RequestEmployeeId)
                    .HasConstraintName("FK_ProcurementRequest_Employee");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ProcurementRequestItem>(entity =>
            {
                entity.Property(e => e.ProcurementRequestItemId).ValueGeneratedNever();

                entity.Property(e => e.Description).HasColumnType("character varying");

                entity.Property(e => e.DiscountValue).HasColumnType("money");

                entity.Property(e => e.IncurredUnit).HasColumnType("character varying(200)");

                entity.Property(e => e.UnitPrice).HasColumnType("money");

                entity.Property(e => e.Vat).HasColumnName("VAT");

                entity.HasOne(d => d.ProcurementPlan)
                    .WithMany(p => p.ProcurementRequestItem)
                    .HasForeignKey(d => d.ProcurementPlanId)
                    .HasConstraintName("FK_ProcurementRequestItem_ProcurementPlan");

                entity.HasOne(d => d.ProcurementRequest)
                    .WithMany(p => p.ProcurementRequestItem)
                    .HasForeignKey(d => d.ProcurementRequestId)
                    .HasConstraintName("FK_ProcurementRequestItem_ProcurementRequest");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProcurementRequestItem)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_ProcurementRequestItem_Product");

                entity.HasOne(d => d.Vendor)
                    .WithMany(p => p.ProcurementRequestItem)
                    .HasForeignKey(d => d.VendorId)
                    .HasConstraintName("FK_ProcurementRequestItem_Vendor");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.ProductId).ValueGeneratedNever();

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("true");

                entity.Property(e => e.ExWarehousePrice).HasColumnType("money");

                entity.Property(e => e.Price1).HasColumnType("money");

                entity.Property(e => e.Price2).HasColumnType("money");

                entity.Property(e => e.ProductCode).HasColumnType("character varying(25)");

                entity.Property(e => e.ProductDescription).HasColumnType("character varying");

                entity.Property(e => e.ProductName)
                    .IsRequired()
                    .HasColumnType("character varying(250)");

                entity.Property(e => e.Vat).HasColumnName("VAT");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ProductAttribute>(entity =>
            {
                entity.Property(e => e.ProductAttributeId).ValueGeneratedNever();

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("true");

                entity.HasOne(d => d.ProductAttributeCategory)
                    .WithMany(p => p.ProductAttribute)
                    .HasForeignKey(d => d.ProductAttributeCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductAttribute__ProductAttributeCategory");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductAttribute)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductAttribute__Product");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ProductAttributeCategory>(entity =>
            {
                entity.Property(e => e.ProductAttributeCategoryId).ValueGeneratedNever();

                entity.Property(e => e.ProductAttributeCategoryName)
                    .IsRequired()
                    .HasColumnType("character varying(250)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ProductAttributeCategoryValue>(entity =>
            {
                entity.Property(e => e.ProductAttributeCategoryValueId).ValueGeneratedNever();

                entity.Property(e => e.ProductAttributeCategoryValue1)
                    .IsRequired()
                    .HasColumnName("ProductAttributeCategoryValue")
                    .HasColumnType("character varying(250)");

                entity.HasOne(d => d.ProductAttributeCategory)
                    .WithMany(p => p.ProductAttributeCategoryValue)
                    .HasForeignKey(d => d.ProductAttributeCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductAt__Produ__467D75B8");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ProductBillOfMaterials>(entity =>
            {
                entity.HasKey(e => e.ProductBillOfMaterialId);

                entity.Property(e => e.ProductBillOfMaterialId).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.Property(e => e.ProductCategoryId).ValueGeneratedNever();

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("true");

                entity.Property(e => e.Description).HasColumnType("character varying");

                entity.Property(e => e.ProductCategoryCode).HasColumnType("character varying(500)");

                entity.Property(e => e.ProductCategoryName)
                    .IsRequired()
                    .HasColumnType("character varying(500)");

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId)
                    .HasConstraintName("FK_ProductCategory_ProductCategory");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.Property(e => e.ProductImageId).ValueGeneratedNever();

                entity.Property(e => e.ImageName).HasColumnType("character varying(100)");

                entity.Property(e => e.ImageSize).HasColumnType("character varying(20)");

                entity.Property(e => e.ImageUrl).HasColumnType("character varying(300)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ProductionOrder>(entity =>
            {
                entity.Property(e => e.ProductionOrderId).ValueGeneratedNever();

                entity.Property(e => e.CustomerName).HasColumnType("character varying(100)");

                entity.Property(e => e.CustomerNumber).HasColumnType("character varying(250)");

                entity.Property(e => e.Especially).HasDefaultValueSql("false");

                entity.Property(e => e.Note).HasColumnType("character varying");

                entity.Property(e => e.NoteTechnique).HasColumnType("character varying");

                entity.Property(e => e.PlaceOfDelivery).HasColumnType("character varying");

                entity.Property(e => e.ProductionOrderCode).HasColumnType("character varying(100)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ProductionOrderHistory>(entity =>
            {
                entity.Property(e => e.ProductionOrderHistoryId).ValueGeneratedNever();

                entity.Property(e => e.Description).HasColumnType("character varying");

                entity.Property(e => e.IsError).HasDefaultValueSql("false");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ProductionOrderMapping>(entity =>
            {
                entity.Property(e => e.ProductionOrderMappingId).ValueGeneratedNever();

                entity.Property(e => e.ProductCode).HasColumnType("character varying(250)");

                entity.Property(e => e.ProductColor).HasColumnType("character varying(300)");

                entity.Property(e => e.ProductColorCode).HasColumnType("character varying(300)");

                entity.Property(e => e.ProductGroupCode).HasColumnType("character varying(255)");

                entity.Property(e => e.ProductName).HasColumnType("character varying(300)");

                entity.Property(e => e.TechniqueDescription).HasColumnType("character varying");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ProductMappingOptions>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Amount).HasColumnType("numeric(18,2)");

                entity.Property(e => e.Price).HasColumnType("numeric(18,2)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ProductOrderWorkflow>(entity =>
            {
                entity.Property(e => e.ProductOrderWorkflowId).ValueGeneratedNever();

                entity.Property(e => e.Active).HasDefaultValueSql("true");

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasColumnType("character varying(50)");

                entity.Property(e => e.Description).HasColumnType("character varying");

                entity.Property(e => e.IsDefault).HasDefaultValueSql("false");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("character varying(100)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ProductVendorMapping>(entity =>
            {
                entity.Property(e => e.ProductVendorMappingId).ValueGeneratedNever();

                entity.Property(e => e.Price).HasColumnType("money");

                entity.Property(e => e.VendorProductCode).HasColumnType("character varying(250)");

                entity.Property(e => e.VendorProductName).HasColumnType("character varying(250)");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductVendorMapping)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductVendorMapping__Product");

                entity.HasOne(d => d.Vendor)
                    .WithMany(p => p.ProductVendorMapping)
                    .HasForeignKey(d => d.VendorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductVendorMapping__Vendor");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.Property(e => e.ProjectId).ValueGeneratedNever();

                entity.Property(e => e.BudgetUsd).HasColumnName("BudgetUSD");

                entity.Property(e => e.BudgetVnd).HasColumnName("BudgetVND");

                entity.Property(e => e.Description).HasColumnType("character varying");

                entity.Property(e => e.GiaBanTheoGio)
                    .HasColumnType("money")
                    .HasDefaultValueSql("'$0.00'::money");

                entity.Property(e => e.ProjectCode).HasColumnType("character varying(500)");

                entity.Property(e => e.ProjectName)
                    .IsRequired()
                    .HasColumnType("character varying(500)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ProjectContractMapping>(entity =>
            {
                entity.HasKey(e => e.ProjectContractId);

                entity.Property(e => e.ProjectContractId).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ProjectCostReport>(entity =>
            {
                entity.Property(e => e.ProjectCostReportId).ValueGeneratedNever();

                entity.Property(e => e.Ac)
                    .HasColumnName("AC")
                    .HasColumnType("money")
                    .HasDefaultValueSql("'$0.00'::money");

                entity.Property(e => e.Ev)
                    .HasColumnName("EV")
                    .HasColumnType("money")
                    .HasDefaultValueSql("'$0.00'::money");

                entity.Property(e => e.Pv)
                    .HasColumnName("PV")
                    .HasColumnType("money")
                    .HasDefaultValueSql("'$0.00'::money");
            });

            modelBuilder.Entity<ProjectEmployeeMapping>(entity =>
            {
                entity.HasKey(e => e.ProjectResourceMappingId);

                entity.Property(e => e.ProjectResourceMappingId).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ProjectMilestone>(entity =>
            {
                entity.HasKey(e => e.ProjectMilestonesId);

                entity.Property(e => e.ProjectMilestonesId).ValueGeneratedNever();

                entity.Property(e => e.Description).HasColumnType("character varying");

                entity.Property(e => e.Name).HasColumnType("character varying(500)");

                entity.Property(e => e.Status).HasColumnType("character varying(500)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ProjectObjective>(entity =>
            {
                entity.HasKey(e => e.ProjectObjectId);

                entity.Property(e => e.ProjectObjectId).ValueGeneratedNever();

                entity.Property(e => e.ProjectObjectDescription).HasColumnType("character varying");

                entity.Property(e => e.ProjectObjectName).HasColumnType("character varying(500)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ProjectResource>(entity =>
            {
                entity.Property(e => e.ProjectResourceId).ValueGeneratedNever();

                entity.Property(e => e.ChiPhiTheoGio)
                    .HasColumnType("money")
                    .HasDefaultValueSql("'$0.00'::money");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ProjectScope>(entity =>
            {
                entity.Property(e => e.ProjectScopeId).ValueGeneratedNever();

                entity.Property(e => e.Description).HasColumnType("character varying");

                entity.Property(e => e.ProjectScopeCode).HasColumnType("character varying(50)");

                entity.Property(e => e.ProjectScopeName).HasColumnType("character varying(500)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ProjectVendor>(entity =>
            {
                entity.Property(e => e.ProjectVendorId).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Promotion>(entity =>
            {
                entity.Property(e => e.PromotionId).ValueGeneratedNever();

                entity.Property(e => e.Description).HasColumnType("character varying");

                entity.Property(e => e.FilterContent).HasColumnType("character varying");

                entity.Property(e => e.FilterQuery).HasColumnType("character varying");

                entity.Property(e => e.Note).HasColumnType("character varying");

                entity.Property(e => e.PromotionCode)
                    .IsRequired()
                    .HasColumnType("character varying(50)");

                entity.Property(e => e.PromotionName)
                    .IsRequired()
                    .HasColumnType("character varying(1000)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<PromotionMapping>(entity =>
            {
                entity.Property(e => e.PromotionMappingId).ValueGeneratedNever();

                entity.Property(e => e.GiaTri)
                    .HasColumnType("money")
                    .HasDefaultValueSql("'$0.00'::money");

                entity.Property(e => e.SoTienTu)
                    .HasColumnType("money")
                    .HasDefaultValueSql("'$0.00'::money");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<PromotionObjectApply>(entity =>
            {
                entity.Property(e => e.PromotionObjectApplyId).ValueGeneratedNever();

                entity.Property(e => e.Amount)
                    .HasColumnType("money")
                    .HasDefaultValueSql("'$0.00'::money");

                entity.Property(e => e.GiaTri)
                    .HasColumnType("money")
                    .HasDefaultValueSql("'$0.00'::money");

                entity.Property(e => e.ObjectType)
                    .IsRequired()
                    .HasColumnType("character varying(50)");

                entity.Property(e => e.SoTienTu)
                    .HasColumnType("money")
                    .HasDefaultValueSql("'$0.00'::money");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<PromotionObjectApplyMapping>(entity =>
            {
                entity.Property(e => e.PromotionObjectApplyMappingId).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<PromotionProductMapping>(entity =>
            {
                entity.Property(e => e.PromotionProductMappingId).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Province>(entity =>
            {
                entity.Property(e => e.ProvinceId).ValueGeneratedNever();

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("true");

                entity.Property(e => e.IsShowAsset).HasDefaultValueSql("false");

                entity.Property(e => e.ProvinceCode).HasColumnType("character varying(5)");

                entity.Property(e => e.ProvinceName)
                    .IsRequired()
                    .HasColumnType("character varying(50)");

                entity.Property(e => e.ProvinceType).HasColumnType("character varying(20)");
            });

            modelBuilder.Entity<PurchaseOrderStatus>(entity =>
            {
                entity.Property(e => e.PurchaseOrderStatusId).ValueGeneratedNever();

                entity.Property(e => e.Description).HasColumnType("character varying(500)");

                entity.Property(e => e.PurchaseOrderStatusCode).HasColumnType("character varying(500)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Queue>(entity =>
            {
                entity.Property(e => e.QueueId).ValueGeneratedNever();

                entity.Property(e => e.Bcc)
                    .HasColumnName("BCC")
                    .HasColumnType("character varying(500)");

                entity.Property(e => e.Cc)
                    .HasColumnName("CC")
                    .HasColumnType("character varying(500)");

                entity.Property(e => e.FromTo).HasColumnType("character varying(200)");

                entity.Property(e => e.Method).HasColumnType("character varying(5)");

                entity.Property(e => e.SendContent).HasColumnType("character varying");

                entity.Property(e => e.SendTo)
                    .IsRequired()
                    .HasColumnType("character varying(100)");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasColumnType("character varying(300)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Quiz>(entity =>
            {
                entity.Property(e => e.QuizId).ValueGeneratedNever();

                entity.Property(e => e.QuizName)
                    .IsRequired()
                    .HasColumnType("character varying");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Quote>(entity =>
            {
                entity.Property(e => e.QuoteId).ValueGeneratedNever();

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("true");

                entity.Property(e => e.Amount)
                    .HasColumnType("money")
                    .HasDefaultValueSql("'$0.00'::money");

                entity.Property(e => e.ConstructionTime).HasColumnType("character varying(50)");

                entity.Property(e => e.Description).HasColumnType("character varying");

                entity.Property(e => e.DiscountValue).HasColumnType("money");

                entity.Property(e => e.ExpirationDate).HasColumnType("date");

                entity.Property(e => e.LocationOfShipment).HasColumnType("character varying");

                entity.Property(e => e.MaxDebt).HasColumnType("money");

                entity.Property(e => e.Note).HasColumnType("character varying");

                entity.Property(e => e.ObjectType).HasColumnType("character varying(50)");

                entity.Property(e => e.PercentAdvanceType)
                    .IsRequired()
                    .HasDefaultValueSql("true");

                entity.Property(e => e.PlaceOfDelivery).HasColumnType("character varying");

                entity.Property(e => e.QuoteCode).HasColumnType("character varying(20)");

                entity.Property(e => e.QuoteName).HasColumnType("character varying");

                entity.Property(e => e.ReceivedDate).HasColumnType("date");

                entity.Property(e => e.ReceivedHour).HasColumnType("time without time zone");

                entity.Property(e => e.RecipientEmail).HasColumnType("character varying(50)");

                entity.Property(e => e.RecipientName).HasColumnType("character varying(250)");

                entity.Property(e => e.RecipientPhone).HasColumnType("character varying(50)");

                entity.Property(e => e.ShippingNote).HasColumnType("character varying");

                entity.Property(e => e.Vat).HasColumnName("VAT");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<QuoteApproveDetailHistory>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Description).HasColumnType("character varying");

                entity.Property(e => e.IncurredUnit).HasColumnType("character varying(50)");

                entity.Property(e => e.UnitPrice).HasColumnType("money");

                entity.Property(e => e.Vat).HasColumnName("VAT");

                entity.Property(e => e.VendorId).HasColumnName("VendorID");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<QuoteApproveHistory>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.AmountPriceInitial).HasColumnType("money");

                entity.Property(e => e.AmountPriceProfit).HasColumnType("money");

                entity.Property(e => e.QuoteCode).HasColumnType("character varying(20)");

                entity.Property(e => e.QuoteName).HasColumnType("character varying");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<QuoteCostDetail>(entity =>
            {
                entity.Property(e => e.QuoteCostDetailId).ValueGeneratedNever();

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("true");

                entity.Property(e => e.CostName).HasColumnType("character varying(500)");

                entity.Property(e => e.UnitPrice).HasColumnType("money");

                entity.HasOne(d => d.Quote)
                    .WithMany(p => p.QuoteCostDetail)
                    .HasForeignKey(d => d.QuoteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QuoteCostDetail_Quote");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<QuoteDetail>(entity =>
            {
                entity.Property(e => e.QuoteDetailId).ValueGeneratedNever();

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("true");

                entity.Property(e => e.Description).HasColumnType("character varying");

                entity.Property(e => e.DiscountValue).HasColumnType("money");

                entity.Property(e => e.IncurredUnit).HasColumnType("character varying(50)");

                entity.Property(e => e.PriceInitial).HasColumnType("money");

                entity.Property(e => e.ProductName).HasColumnType("character varying(200)");

                entity.Property(e => e.UnitLaborPrice)
                    .HasColumnType("money")
                    .HasDefaultValueSql("'$0.00'::money");

                entity.Property(e => e.UnitPrice).HasColumnType("money");

                entity.Property(e => e.Vat).HasColumnName("VAT");

                entity.Property(e => e.VendorId).HasColumnName("VendorID");

                entity.HasOne(d => d.Quote)
                    .WithMany(p => p.QuoteDetail)
                    .HasForeignKey(d => d.QuoteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QuoteDetail_Quote");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<QuoteDocument>(entity =>
            {
                entity.Property(e => e.QuoteDocumentId).ValueGeneratedNever();

                entity.Property(e => e.DocumentName).HasColumnType("character varying(100)");

                entity.Property(e => e.DocumentSize).HasColumnType("character varying(20)");

                entity.Property(e => e.DocumentUrl).HasColumnType("character varying(300)");

                entity.HasOne(d => d.Quote)
                    .WithMany(p => p.QuoteDocument)
                    .HasForeignKey(d => d.QuoteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__QuoteDocum__QuoteI__4FBCC72F");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<QuoteParticipantMapping>(entity =>
            {
                entity.Property(e => e.QuoteParticipantMappingId).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<QuotePaymentTerm>(entity =>
            {
                entity.HasKey(e => e.PaymentTermId);

                entity.Property(e => e.PaymentTermId).ValueGeneratedNever();

                entity.Property(e => e.Milestone).HasColumnType("character varying");

                entity.Property(e => e.OrderNumber)
                    .IsRequired()
                    .HasColumnType("character varying(10)");

                entity.Property(e => e.PaymentPercentage).HasColumnType("character varying");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<QuotePlan>(entity =>
            {
                entity.HasKey(e => e.PlanId);

                entity.Property(e => e.PlanId).ValueGeneratedNever();

                entity.Property(e => e.ExecTime).HasColumnType("character varying");

                entity.Property(e => e.Finished).HasColumnType("character varying");

                entity.Property(e => e.SumExecTime).HasColumnType("character varying");

                entity.Property(e => e.Tt).HasColumnName("TT");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<QuoteProductDetailProductAttributeValue>(entity =>
            {
                entity.Property(e => e.QuoteProductDetailProductAttributeValueId).ValueGeneratedNever();

                entity.HasOne(d => d.ProductAttributeCategory)
                    .WithMany(p => p.QuoteProductDetailProductAttributeValue)
                    .HasForeignKey(d => d.ProductAttributeCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QuoteProductDetailProductAttributeValue_ProductAttributeCate");

                entity.HasOne(d => d.ProductAttributeCategoryValue)
                    .WithMany(p => p.QuoteProductDetailProductAttributeValue)
                    .HasForeignKey(d => d.ProductAttributeCategoryValueId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QuoteProductDetailProductAttribute_ProductAttributeCategoryV");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.QuoteProductDetailProductAttributeValue)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QuoteProductDetailProductAttributeValue_Product");

                entity.HasOne(d => d.QuoteDetail)
                    .WithMany(p => p.QuoteProductDetailProductAttributeValue)
                    .HasForeignKey(d => d.QuoteDetailId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_QuoteProductDetailProductAttributeValue_QuoteDetail");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<QuoteScope>(entity =>
            {
                entity.HasKey(e => e.ScopeId);

                entity.Property(e => e.ScopeId)
                    .HasColumnName("scopeId")
                    .ValueGeneratedNever();

                entity.Property(e => e.Category)
                    .HasColumnName("category")
                    .HasColumnType("character varying");

                entity.Property(e => e.CreatedDate).HasColumnName("createdDate");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasColumnType("character varying");

                entity.Property(e => e.Level).HasColumnName("level");

                entity.Property(e => e.ParentId).HasColumnName("parentId");

                entity.Property(e => e.QuoteId).HasColumnName("quoteId");

                entity.Property(e => e.Tt)
                    .IsRequired()
                    .HasColumnName("TT")
                    .HasColumnType("character varying(10)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<QuyenLoiBaoHiemLoftCare>(entity =>
            {
                entity.Property(e => e.TenQuyenLoi).HasColumnType("character varying");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<QuyLuong>(entity =>
            {
                entity.Property(e => e.Active).HasDefaultValueSql("true");

                entity.Property(e => e.QuyLuong1).HasColumnName("QuyLuong");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<QuyTrinh>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("now()");

                entity.Property(e => e.MaQuyTrinh)
                    .IsRequired()
                    .HasColumnType("character varying(50)");

                entity.Property(e => e.MoTa).HasColumnType("character varying");

                entity.Property(e => e.TenQuyTrinh)
                    .IsRequired()
                    .HasColumnType("character varying(255)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ReceiptInvoice>(entity =>
            {
                entity.Property(e => e.ReceiptInvoiceId).ValueGeneratedNever();

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.AmountText).HasColumnType("character varying(250)");

                entity.Property(e => e.ReceiptInvoiceCode).HasColumnType("character varying(50)");

                entity.Property(e => e.ReceiptInvoiceDetail).HasColumnType("character varying(500)");

                entity.Property(e => e.ReceiptInvoiceNote).HasColumnType("character varying");

                entity.Property(e => e.RecipientAddress).HasColumnType("character varying");

                entity.Property(e => e.RecipientName).HasColumnType("character varying(250)");

                entity.Property(e => e.UnitPrice).HasColumnType("money");

                entity.HasOne(d => d.CurrencyUnitNavigation)
                    .WithMany(p => p.ReceiptInvoiceCurrencyUnitNavigation)
                    .HasForeignKey(d => d.CurrencyUnit)
                    .HasConstraintName("FK__ReceiptIn__Curre__47D18CA4");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.ReceiptInvoice)
                    .HasForeignKey(d => d.OrganizationId)
                    .HasConstraintName("FK__ReceiptIn__Organ__3B36AB95");

                entity.HasOne(d => d.ReceiptInvoiceReasonNavigation)
                    .WithMany(p => p.ReceiptInvoiceReceiptInvoiceReasonNavigation)
                    .HasForeignKey(d => d.ReceiptInvoiceReason)
                    .HasConstraintName("FK__ReceiptIn__Recei__4400FBC0");

                entity.HasOne(d => d.RegisterTypeNavigation)
                    .WithMany(p => p.ReceiptInvoiceRegisterTypeNavigation)
                    .HasForeignKey(d => d.RegisterType)
                    .HasConstraintName("FK__ReceiptIn__Regis__44F51FF9");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.ReceiptInvoiceStatus)
                    .HasForeignKey(d => d.StatusId)
                    .HasConstraintName("FK__ReceiptIn__Statu__46DD686B");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ReceiptInvoiceMapping>(entity =>
            {
                entity.Property(e => e.ReceiptInvoiceMappingId).ValueGeneratedNever();

                entity.HasOne(d => d.ReceiptInvoice)
                    .WithMany(p => p.ReceiptInvoiceMapping)
                    .HasForeignKey(d => d.ReceiptInvoiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ReceiptIn__Recei__4AADF94F");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ReceiptOrderHistory>(entity =>
            {
                entity.Property(e => e.ReceiptOrderHistoryId).ValueGeneratedNever();

                entity.Property(e => e.AmountCollected).HasColumnType("money");

                entity.Property(e => e.ObjectType)
                    .IsRequired()
                    .HasColumnType("character varying(10)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<RecruitmentCampaign>(entity =>
            {
                entity.Property(e => e.RecruitmentCampaignId).ValueGeneratedNever();

                entity.Property(e => e.RecruitmentCampaignDes).HasColumnType("character varying");

                entity.Property(e => e.RecruitmentCampaignName)
                    .IsRequired()
                    .HasColumnType("character varying(500)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<RelateTaskMapping>(entity =>
            {
                entity.Property(e => e.RelateTaskMappingId).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<RememberItem>(entity =>
            {
                entity.Property(e => e.RememberItemId).ValueGeneratedNever();

                entity.Property(e => e.Description).HasColumnType("character varying");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<RequestPayment>(entity =>
            {
                entity.Property(e => e.RequestPaymentId).ValueGeneratedNever();

                entity.Property(e => e.Description).HasColumnType("character varying");

                entity.Property(e => e.RequestEmployeePhone).HasColumnType("character varying(50)");

                entity.Property(e => e.RequestPaymentCode).HasColumnType("character varying(100)");

                entity.Property(e => e.RequestPaymentCreateDate).HasColumnType("date");

                entity.Property(e => e.RequestPaymentNote).HasColumnType("character varying(500)");

                entity.Property(e => e.StatusId).HasColumnName("StatusID");

                entity.Property(e => e.TotalAmount).HasColumnType("money");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.RoleId).ValueGeneratedNever();

                entity.Property(e => e.Description).HasColumnType("character varying(500)");

                entity.Property(e => e.RoleValue).HasColumnType("character varying(500)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<RoleAndMenuBuild>(entity =>
            {
                entity.Property(e => e.RoleAndMenuBuildId).ValueGeneratedNever();

                entity.Property(e => e.Code).HasColumnType("character varying(100)");

                entity.Property(e => e.Path).HasColumnType("character varying(2000)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<RoleAndPermission>(entity =>
            {
                entity.Property(e => e.RoleAndPermissionId).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<SaleBidding>(entity =>
            {
                entity.Property(e => e.SaleBiddingId).ValueGeneratedNever();

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("true");

                entity.Property(e => e.Address).HasColumnType("character varying(500)");

                entity.Property(e => e.FormOfBid)
                    .IsRequired()
                    .HasColumnType("character varying(100)");

                entity.Property(e => e.Note).HasColumnType("character varying");

                entity.Property(e => e.SaleBiddingCode)
                    .IsRequired()
                    .HasColumnType("character varying(50)");

                entity.Property(e => e.SaleBiddingName)
                    .IsRequired()
                    .HasColumnType("character varying(250)");

                entity.Property(e => e.ValueBid).HasColumnType("money");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<SaleBiddingDetail>(entity =>
            {
                entity.Property(e => e.SaleBiddingDetailId).ValueGeneratedNever();

                entity.Property(e => e.Category)
                    .IsRequired()
                    .HasColumnType("character varying(250)");

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasColumnType("character varying");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<SaleBiddingDetailProductAttribute>(entity =>
            {
                entity.Property(e => e.SaleBiddingDetailProductAttributeId).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<SaleBiddingEmployeeMapping>(entity =>
            {
                entity.Property(e => e.SaleBiddingEmployeeMappingId).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Satellite>(entity =>
            {
                entity.Property(e => e.SatelliteId).ValueGeneratedNever();

                entity.Property(e => e.SatelliteCode).HasColumnType("character varying(500)");

                entity.Property(e => e.SatelliteName).HasColumnType("character varying(500)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Schema>(entity =>
            {
                entity.HasKey(e => e.Version);

                entity.ToTable("schema", "hangfire");

                entity.Property(e => e.Version)
                    .HasColumnName("version")
                    .ValueGeneratedNever();
            });

            modelBuilder.Entity<Screen>(entity =>
            {
                entity.Property(e => e.ScreenId).ValueGeneratedNever();

                entity.Property(e => e.ScreenCode)
                    .IsRequired()
                    .HasColumnType("character varying(500)");

                entity.Property(e => e.ScreenName)
                    .IsRequired()
                    .HasColumnType("character varying(1000)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Serial>(entity =>
            {
                entity.Property(e => e.SerialId).ValueGeneratedNever();

                entity.Property(e => e.SerialCode)
                    .IsRequired()
                    .HasColumnType("character varying(50)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Server>(entity =>
            {
                entity.ToTable("server", "hangfire");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Data).HasColumnName("data");

                entity.Property(e => e.Lastheartbeat).HasColumnName("lastheartbeat");

                entity.Property(e => e.Updatecount).HasColumnName("updatecount");
            });

            modelBuilder.Entity<ServicePacket>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.AttributeName).HasColumnType("character varying(255)");

                entity.Property(e => e.Description).HasColumnType("character varying(255)");

                entity.Property(e => e.Message).HasColumnType("character varying");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("character varying(255)");

                entity.Property(e => e.Status).HasColumnType("character varying(255)");

                entity.Property(e => e.Code).HasColumnType("character varying(50)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ServicePacketImage>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.BackgroundImage).HasColumnType("character varying");

                entity.Property(e => e.Icon).HasColumnType("character varying");

                entity.Property(e => e.MainImage).HasColumnType("character varying");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ServicePacketMappingOptions>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).HasColumnType("character varying(255)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Set>(entity =>
            {
                entity.ToTable("set", "hangfire");

                entity.HasIndex(e => new { e.Key, e.Value })
                    .HasName("set_key_value_key")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('hangfire.set_id_seq'::regclass)");

                entity.Property(e => e.Expireat).HasColumnName("expireat");

                entity.Property(e => e.Key)
                    .IsRequired()
                    .HasColumnName("key");

                entity.Property(e => e.Score).HasColumnName("score");

                entity.Property(e => e.Updatecount).HasColumnName("updatecount");

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasColumnName("value");
            });

            modelBuilder.Entity<SoKho>(entity =>
            {
                entity.Property(e => e.SoKhoId).ValueGeneratedNever();

                entity.Property(e => e.Gia).HasColumnType("money");

                entity.Property(e => e.SoChungTu)
                    .IsRequired()
                    .HasColumnType("character varying(50)");

                entity.Property(e => e.ThanhTien).HasColumnType("money");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<State>(entity =>
            {
                entity.ToTable("state", "hangfire");

                entity.HasIndex(e => e.Jobid)
                    .HasName("ix_hangfire_state_jobid");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasDefaultValueSql("nextval('hangfire.state_id_seq'::regclass)");

                entity.Property(e => e.Createdat).HasColumnName("createdat");

                entity.Property(e => e.Data).HasColumnName("data");

                entity.Property(e => e.Jobid).HasColumnName("jobid");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");

                entity.Property(e => e.Reason).HasColumnName("reason");

                entity.Property(e => e.Updatecount).HasColumnName("updatecount");

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.State)
                    .HasForeignKey(d => d.Jobid)
                    .HasConstraintName("state_jobid_fkey");
            });

            modelBuilder.Entity<StockTake>(entity =>
            {
                entity.Property(e => e.StockTakeId).ValueGeneratedNever();

                entity.Property(e => e.StockTakeCode)
                    .IsRequired()
                    .HasColumnType("character varying(50)");

                entity.Property(e => e.StockTakeTime).HasColumnType("time without time zone");

                entity.Property(e => e.TotalMoneyActual).HasColumnType("money");

                entity.Property(e => e.TotalMoneyDeflectionIncreases).HasColumnType("money");

                entity.Property(e => e.TotalMoneyDeviationDecreases).HasColumnType("money");

                entity.Property(e => e.TotalMoneyDeviationDeviation).HasColumnType("money");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<StockTakeProductMapping>(entity =>
            {
                entity.Property(e => e.StockTakeProductMappingId).ValueGeneratedNever();

                entity.Property(e => e.PriceDeviation).HasColumnType("money");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<SuggestedSupplierQuotes>(entity =>
            {
                entity.HasKey(e => e.SuggestedSupplierQuoteId);

                entity.Property(e => e.SuggestedSupplierQuoteId).ValueGeneratedNever();

                entity.Property(e => e.Note).HasColumnType("character varying");

                entity.Property(e => e.ObjectType).HasColumnType("character varying(50)");

                entity.Property(e => e.SuggestedSupplierQuote)
                    .IsRequired()
                    .HasColumnType("character varying(50)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<SuggestedSupplierQuotesDetail>(entity =>
            {
                entity.HasKey(e => e.SuggestedSupplierQuoteDetailId);

                entity.Property(e => e.SuggestedSupplierQuoteDetailId).ValueGeneratedNever();

                entity.Property(e => e.Note).HasColumnType("character varying");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<SynchonizedHistory>(entity =>
            {
                entity.Property(e => e.Note).HasColumnType("character varying");
            });

            modelBuilder.Entity<SystemFeature>(entity =>
            {
                entity.Property(e => e.SystemFeatureId).ValueGeneratedNever();

                entity.Property(e => e.SystemFeatureCode)
                    .IsRequired()
                    .HasColumnType("character varying(50)");

                entity.Property(e => e.SystemFeatureName)
                    .IsRequired()
                    .HasColumnType("character varying(250)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<SystemParameter>(entity =>
            {
                entity.Property(e => e.SystemParameterId).ValueGeneratedNever();

                entity.Property(e => e.Description).HasColumnType("character varying");

                entity.Property(e => e.SystemDescription).HasColumnType("character varying(500)");

                entity.Property(e => e.SystemGroupCode).HasColumnType("character varying(20)");

                entity.Property(e => e.SystemGroupDesc).HasColumnType("character varying(100)");

                entity.Property(e => e.SystemKey)
                    .IsRequired()
                    .HasColumnType("character varying(100)");

                entity.Property(e => e.SystemValueString).HasColumnType("character varying");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<TaiLieuNhanVien>(entity =>
            {
                entity.Property(e => e.Active).HasDefaultValueSql("true");

                entity.Property(e => e.CreatedDate).HasDefaultValueSql("now()");

                entity.Property(e => e.TenTaiLieu).HasColumnType("character varying(255)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<TaiSan>(entity =>
            {
                entity.Property(e => e.HangSxid).HasColumnName("HangSXId");

                entity.Property(e => e.MaCode).HasColumnType("character varying(50)");

                entity.Property(e => e.MaTaiSan)
                    .IsRequired()
                    .HasColumnType("character varying(255)");

                entity.Property(e => e.MoTa).HasColumnType("character varying");

                entity.Property(e => e.Model).HasColumnType("character varying(255)");

                entity.Property(e => e.NamSx).HasColumnName("NamSX");

                entity.Property(e => e.NuocSxid).HasColumnName("NuocSXId");

                entity.Property(e => e.SoHieu).HasColumnType("character varying(255)");

                entity.Property(e => e.SoSerial).HasColumnType("character varying(255)");

                entity.Property(e => e.TenTaiSan).HasColumnType("character varying(255)");

                entity.Property(e => e.ThoiDiemBdtinhKhauHao).HasColumnName("ThoiDiemBDTinhKhauHao");

                entity.Property(e => e.ThongTinNoiBaoHanh).HasColumnType("character varying");

                entity.Property(e => e.ThongTinNoiMua).HasColumnType("character varying");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Task>(entity =>
            {
                entity.Property(e => e.TaskId).ValueGeneratedNever();

                entity.Property(e => e.Description).HasColumnType("character varying");

                entity.Property(e => e.TaskCode).HasColumnType("character varying(24)");

                entity.Property(e => e.TaskName)
                    .IsRequired()
                    .HasColumnType("character varying(500)");

                entity.Property(e => e.TimeType).HasColumnType("character varying(20)");

                entity.Property(e => e.UpdateDate).HasDefaultValueSql("now()");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<TaskConstraint>(entity =>
            {
                entity.Property(e => e.TaskConstraintId).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<TaskDocument>(entity =>
            {
                entity.Property(e => e.TaskDocumentId).ValueGeneratedNever();

                entity.Property(e => e.DocumentName).HasColumnType("character varying(100)");

                entity.Property(e => e.DocumentSize).HasColumnType("character varying(20)");

                entity.Property(e => e.DocumentUrl).HasColumnType("character varying(300)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<TaskMilestonesMapping>(entity =>
            {
                entity.HasKey(e => e.ProjectMilestonesMappingId);

                entity.Property(e => e.ProjectMilestonesMappingId).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<TaskResourceMapping>(entity =>
            {
                entity.Property(e => e.TaskResourceMappingId).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<TechniqueRequest>(entity =>
            {
                entity.Property(e => e.TechniqueRequestId).ValueGeneratedNever();

                entity.Property(e => e.Active).HasDefaultValueSql("true");

                entity.Property(e => e.Description).HasColumnType("character varying");

                entity.Property(e => e.TechniqueName)
                    .IsRequired()
                    .HasColumnType("character varying(100)");

                entity.Property(e => e.TechniqueRequestCode).HasColumnType("character varying(255)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<TechniqueRequestMapping>(entity =>
            {
                entity.Property(e => e.TechniqueRequestMappingId).ValueGeneratedNever();

                entity.Property(e => e.TechniqueName).HasColumnType("character varying(300)");

                entity.Property(e => e.TechniqueValue).HasDefaultValueSql("'0'::double precision");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Template>(entity =>
            {
                entity.Property(e => e.TemplateId).ValueGeneratedNever();

                entity.Property(e => e.TemplateContent)
                    .IsRequired()
                    .HasColumnType("character varying");

                entity.Property(e => e.TemplateTitle)
                    .IsRequired()
                    .HasColumnType("character varying(300)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Tenants>(entity =>
            {
                entity.HasKey(e => e.TenantId);

                entity.Property(e => e.TenantId).ValueGeneratedNever();

                entity.Property(e => e.Status).HasColumnType("character varying(50)");

                entity.Property(e => e.TenantHost)
                    .IsRequired()
                    .HasColumnType("character varying(500)");

                entity.Property(e => e.TenantMode).HasColumnType("character varying(50)");

                entity.Property(e => e.TenantName)
                    .IsRequired()
                    .HasColumnType("character varying(500)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<TermsOfPayment>(entity =>
            {
                entity.Property(e => e.TermsOfPaymentId).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ThanhVienPhongBan>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<TheoDoiThanhToan>(entity =>
            {
                entity.Property(e => e.TheoDoiThanhToanId).ValueGeneratedNever();

                entity.Property(e => e.DieuKienThanhToan).HasColumnType("character varying(500)");

                entity.Property(e => e.LanThanhToan).HasColumnType("character varying(500)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<TimeSheet>(entity =>
            {
                entity.Property(e => e.TimeSheetId).ValueGeneratedNever();

                entity.Property(e => e.Note).HasColumnType("character varying");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<TimeSheetDetail>(entity =>
            {
                entity.Property(e => e.TimeSheetDetailId).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<TongHopChamCong>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("now()");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<TotalProductionOrder>(entity =>
            {
                entity.Property(e => e.TotalProductionOrderId).ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasColumnType("character varying(50)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<TotalProductionOrderMapping>(entity =>
            {
                entity.Property(e => e.TotalProductionOrderMappingId).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<TroCap>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("now()");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.UserId).ValueGeneratedNever();

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("true");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnType("character varying");


                entity.Property(e => e.ResetCode).HasColumnType("character varying(50)");

                entity.Property(e => e.ResetCodeDate).HasColumnType("date");

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasColumnType("character varying(50)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.Property(e => e.UserRoleId).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Vacancies>(entity =>
            {
                entity.Property(e => e.VacanciesId).ValueGeneratedNever();

                entity.Property(e => e.CandidateBenefits).HasColumnType("character varying");

                entity.Property(e => e.Currency).HasColumnType("character varying(20)");

                entity.Property(e => e.PlaceOfWork).HasColumnType("character varying");

                entity.Property(e => e.ProfessionalRequirements).HasColumnType("character varying");

                entity.Property(e => e.VacanciesDes).HasColumnType("character varying");

                entity.Property(e => e.VacanciesName)
                    .IsRequired()
                    .HasColumnType("character varying");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<VacanciesDocument>(entity =>
            {
                entity.Property(e => e.VacanciesDocumentId).ValueGeneratedNever();

                entity.Property(e => e.DocumentName).HasColumnType("character varying");

                entity.Property(e => e.DocumentSize).HasColumnType("character varying");

                entity.Property(e => e.DocumentUrl).HasColumnType("character varying");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Vendor>(entity =>
            {
                entity.Property(e => e.VendorId).ValueGeneratedNever();

                entity.Property(e => e.AccountNumberInfor).HasColumnType("character varying(255)");

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("true");

                entity.Property(e => e.Address).HasColumnType("character varying(500)");

                entity.Property(e => e.ContactEmail).HasColumnType("character varying(255)");

                entity.Property(e => e.ContactName).HasColumnType("character varying(255)");

                entity.Property(e => e.ContactPhoneNumber).HasColumnType("character varying(50)");

                entity.Property(e => e.Description).HasColumnType("character varying");

                entity.Property(e => e.Email).HasColumnType("character varying(255)");

                entity.Property(e => e.Mst)
                    .HasColumnName("MST")
                    .HasColumnType("character varying(255)");

                entity.Property(e => e.PhoneNumber).HasColumnType("character varying(50)");

                entity.Property(e => e.VendorCode)
                    .IsRequired()
                    .HasColumnType("character varying(20)");

                entity.Property(e => e.VendorName)
                    .IsRequired()
                    .HasColumnType("character varying(250)");

                entity.Property(e => e.Website).HasColumnType("character varying(255)");

                entity.HasOne(d => d.Payment)
                    .WithMany(p => p.VendorPayment)
                    .HasForeignKey(d => d.PaymentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Vendor__Payment__76A18A26");

                entity.HasOne(d => d.VendorGroup)
                    .WithMany(p => p.VendorVendorGroup)
                    .HasForeignKey(d => d.VendorGroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Vendor__VendorGr__75AD65ED");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<VendorOrder>(entity =>
            {
                entity.Property(e => e.VendorOrderId).ValueGeneratedNever();

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("true");

                entity.Property(e => e.Amount).HasColumnType("money");

                entity.Property(e => e.Description).HasColumnType("character varying");

                entity.Property(e => e.DiscountValue).HasColumnType("money");

                entity.Property(e => e.LocationOfShipment).HasColumnType("character varying");

                entity.Property(e => e.Note).HasColumnType("character varying");

                entity.Property(e => e.PlaceOfDelivery).HasColumnType("character varying");

                entity.Property(e => e.ReceivedDate).HasColumnType("date");

                entity.Property(e => e.ReceivedHour).HasColumnType("time without time zone");

                entity.Property(e => e.RecipientEmail).HasColumnType("character varying(50)");

                entity.Property(e => e.RecipientName).HasColumnType("character varying(250)");

                entity.Property(e => e.RecipientPhone).HasColumnType("character varying(50)");

                entity.Property(e => e.ShipperName).HasColumnType("character varying(300)");

                entity.Property(e => e.ShippingNote).HasColumnType("character varying");

                entity.Property(e => e.TypeCost).HasColumnType("character varying(50)");

                entity.Property(e => e.VendorOrderCode).HasColumnType("character varying(20)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<VendorOrderCostDetail>(entity =>
            {
                entity.Property(e => e.VendorOrderCostDetailId).ValueGeneratedNever();

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("true");

                entity.Property(e => e.CostName).HasColumnType("character varying(500)");

                entity.Property(e => e.UnitPrice).HasColumnType("money");

                entity.HasOne(d => d.VendorOrder)
                    .WithMany(p => p.VendorOrderCostDetail)
                    .HasForeignKey(d => d.VendorOrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VendorOrderCostDetail_VendorOrder");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<VendorOrderDetail>(entity =>
            {
                entity.Property(e => e.VendorOrderDetailId).ValueGeneratedNever();

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("true");

                entity.Property(e => e.Cost).HasColumnType("money");

                entity.Property(e => e.Description).HasColumnType("character varying");

                entity.Property(e => e.DiscountValue).HasColumnType("money");

                entity.Property(e => e.IncurredUnit).HasColumnType("character varying(50)");

                entity.Property(e => e.PriceValueWarehouse).HasColumnType("money");

                entity.Property(e => e.PriceWarehouse).HasColumnType("money");

                entity.Property(e => e.UnitPrice).HasColumnType("money");

                entity.Property(e => e.Vat).HasColumnName("VAT");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.VendorOrderDetail)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_VendorOrderDetail_Product");

                entity.HasOne(d => d.Unit)
                    .WithMany(p => p.VendorOrderDetail)
                    .HasForeignKey(d => d.UnitId)
                    .HasConstraintName("FK__VendorOrd__UnitI__71C7C670");

                entity.HasOne(d => d.Vendor)
                    .WithMany(p => p.VendorOrderDetail)
                    .HasForeignKey(d => d.VendorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VendorOrderDetail_Vendor");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<VendorOrderProcurementRequestMapping>(entity =>
            {
                entity.Property(e => e.VendorOrderProcurementRequestMappingId).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<VendorOrderProductDetailProductAttributeValue>(entity =>
            {
                entity.HasKey(e => e.OrderProductDetailProductAttributeValueId);

                entity.Property(e => e.OrderProductDetailProductAttributeValueId).ValueGeneratedNever();

                entity.HasOne(d => d.ProductAttributeCategory)
                    .WithMany(p => p.VendorOrderProductDetailProductAttributeValue)
                    .HasForeignKey(d => d.ProductAttributeCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VendorOrderProductDetailProductAttributeValue_ProductAttribu");

                entity.HasOne(d => d.ProductAttributeCategoryValue)
                    .WithMany(p => p.VendorOrderProductDetailProductAttributeValue)
                    .HasForeignKey(d => d.ProductAttributeCategoryValueId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VendorOrderProductDetailProductAttributeValue_ProductAttribC");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.VendorOrderProductDetailProductAttributeValue)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VendorOrderProductDetailProductAttributeValue_Product");

                entity.HasOne(d => d.VendorOrderDetail)
                    .WithMany(p => p.VendorOrderProductDetailProductAttributeValue)
                    .HasForeignKey(d => d.VendorOrderDetailId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_VendorOrderProductDetailProductAttributeValue_VendorOrderDet");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<Ward>(entity =>
            {
                entity.Property(e => e.WardId).ValueGeneratedNever();

                entity.Property(e => e.Active)
                    .IsRequired()
                    .HasDefaultValueSql("true");

                entity.Property(e => e.WardCode).HasColumnType("character(10)");

                entity.Property(e => e.WardName)
                    .IsRequired()
                    .HasColumnType("character varying(50)");

                entity.Property(e => e.WardType).HasColumnType("character varying(20)");

                entity.HasOne(d => d.District)
                    .WithMany(p => p.Ward)
                    .HasForeignKey(d => d.DistrictId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Ward_District");
            });

            modelBuilder.Entity<Warehouse>(entity =>
            {
                entity.Property(e => e.WarehouseId).ValueGeneratedNever();

                entity.Property(e => e.WarehouseAddress).HasColumnType("character varying(500)");

                entity.Property(e => e.WarehouseCode)
                    .IsRequired()
                    .HasColumnType("character varying(50)");

                entity.Property(e => e.WarehouseName)
                    .IsRequired()
                    .HasColumnType("character varying(500)");

                entity.Property(e => e.WarehousePhone).HasColumnType("character varying(50)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<WorkFlows>(entity =>
            {
                entity.HasKey(e => e.WorkFlowId);

                entity.Property(e => e.WorkFlowId).ValueGeneratedNever();

                entity.Property(e => e.Description).HasColumnType("character varying(100)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("character varying(250)");

                entity.Property(e => e.WorkflowCode).HasColumnType("character varying(50)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<WorkFlowSteps>(entity =>
            {
                entity.HasKey(e => e.WorkflowStepId);

                entity.Property(e => e.WorkflowStepId).ValueGeneratedNever();

                entity.Property(e => e.ApprovedText)
                    .IsRequired()
                    .HasColumnType("character varying(100)");

                entity.Property(e => e.NotApprovedText).HasColumnType("character varying(100)");

                entity.Property(e => e.RecordStatus).HasColumnType("character varying(50)");

                entity.HasOne(d => d.Workflow)
                    .WithMany(p => p.WorkFlowSteps)
                    .HasForeignKey(d => d.WorkflowId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WorkFlowSteps_WorkFlows");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<YeuCauCapPhatTaiSan>(entity =>
            {
                entity.Property(e => e.Active).HasDefaultValueSql("true");

                entity.Property(e => e.MaYeuCau)
                    .IsRequired()
                    .HasColumnType("character varying(50)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<YeuCauCapPhatTaiSanChiTiet>(entity =>
            {
                entity.Property(e => e.Active).HasDefaultValueSql("true");

                entity.Property(e => e.LyDo).HasColumnType("character varying(500)");

                entity.Property(e => e.MoTa).HasColumnType("character varying");

                entity.Property(e => e.SoLuong).HasDefaultValueSql("'0'::numeric");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<CustomerOrderDetailExten>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).HasColumnType("character varying(5000)");

                entity.Property(e => e.Price).HasColumnType("numeric(18,2)");

                entity.Property(e => e.Quantity).HasColumnType("numeric(18,2)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<ReportPoint>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Content).HasColumnType("character varying(5000)");

                entity.Property(e => e.IsCusView).HasDefaultValueSql("false");

                entity.Property(e => e.Name).HasColumnType("character varying(5000)");

                entity.HasQueryFilter(e => e.TenantId == _tenantId);

            });

            modelBuilder.Entity<OrderProcess>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.OrderProcessCode).HasColumnType("character varying(500)");

                entity.Property(e => e.Status).HasDefaultValueSql("1");
                entity.HasQueryFilter(e => e.TenantId == _tenantId);

            });

            modelBuilder.Entity<OrderProcessDetail>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Status).HasDefaultValueSql("1");
                entity.HasQueryFilter(e => e.TenantId == _tenantId);

            });

            modelBuilder.Entity<OrderProcessMappingEmployee>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.RateContent).HasColumnType("character varying(5000)");
            });

            modelBuilder.Entity<VendorMappingOption>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });
     
            modelBuilder.Entity<ManagerPacketService>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.HasQueryFilter(e => e.TenantId == _tenantId);

            });

            modelBuilder.Entity<OrderTaskMappingEmp>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.Entity<PaymentMethodConfigure>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
                entity.Property(e => e.Content).HasColumnType("character varying(50000)");
                entity.HasQueryFilter(e => e.TenantId == _tenantId);
            });

            modelBuilder.HasSequence<int>("counter_id_seq");

            modelBuilder.HasSequence<int>("hash_id_seq");

            modelBuilder.HasSequence<int>("job_id_seq");

            modelBuilder.HasSequence<int>("jobparameter_id_seq");

            modelBuilder.HasSequence<int>("jobqueue_id_seq");

            modelBuilder.HasSequence<int>("list_id_seq");

            modelBuilder.HasSequence<int>("set_id_seq");

            modelBuilder.HasSequence<int>("state_id_seq");

            modelBuilder.HasSequence<int>("CauHoiPhieuDanhGiaMappingDanh_CauHoiPhieuDanhGiaMappingDanh_seq");

            modelBuilder.HasSequence<int>("DanhMucCauTraLoiDanhGiaMappin_DanhMucCauTraLoiDanhGiaMappin_seq");
        }

       
        public override int SaveChanges()
        {
            // Get entities that are added
            var changedEntities = ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified).ToList();

            // Run through entities and look for a property named TentantId
            changedEntities.ForEach(e =>
            {
                // Tro to get property "TenantId"
                var tentantId = e.Entity.GetType().GetProperty("TenantId");
                // If the entity has this property then set it
                if (tentantId != null)
                {
                    // Set to current _tentantId
                    tentantId.SetValue(e.Entity, _tenantId);
                }
            });

            return base.SaveChanges();
        }
    } 
}