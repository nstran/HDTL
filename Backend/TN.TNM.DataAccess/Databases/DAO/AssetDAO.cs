using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using TN.TNM.Common;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Admin.AuditTrace;
using TN.TNM.DataAccess.Messages.Parameters.Asset;
using TN.TNM.DataAccess.Messages.Parameters.Employee;
using TN.TNM.DataAccess.Messages.Results.Admin.AuditTrace;
using TN.TNM.DataAccess.Messages.Results.Asset;
using TN.TNM.DataAccess.Messages.Results.Employee;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Address;
using TN.TNM.DataAccess.Models.Asset;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Folder;
using TN.TNM.DataAccess.Models.Note;

/// <summary>
/// Authentication Data Access Object
/// </summary>
namespace TN.TNM.DataAccess.Databases.DAO
{
    public class AssetDAO : BaseDAO, IAssetDataAccess
    {
        private readonly IHostingEnvironment hostingEnvironment;
        public AssetDAO(TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace, IHostingEnvironment _hostingEnvironment)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
            this.hostingEnvironment = _hostingEnvironment;
        }

        public CreateOrUpdateAssetResult CreateOrUpdateAsset(CreateOrUpdateAssetParameter parameter)
        {
            try
            {
                var listAssetCode = context.TaiSan.Where(a => a.TaiSanId != parameter.TaiSan.TaiSanId).Select(x => x.MaTaiSan).ToList();
                var duplicateCode = false;

                if (listAssetCode.Count > 0)
                {
                    listAssetCode.ForEach(x =>
                    {
                        if (x.Equals(parameter.TaiSan.MaTaiSan))
                        {
                            duplicateCode = true;
                        }
                    });
                }

                if (duplicateCode)
                {
                    return new CreateOrUpdateAssetResult()
                    {
                        Status = false,
                        Message = "Mã tài sản đã tồn tại",
                    };
                }

                // Thêm mới tài sản
                if (parameter.TaiSan.TaiSanId == 0)
                {
                    parameter.TaiSan.CreatedById = parameter.UserId;
                    parameter.TaiSan.CreatedDate = DateTime.Now;
                    parameter.TaiSan.UpdatedById = parameter.UserId;
                    parameter.TaiSan.UpdatedDate = DateTime.Now;

                    context.TaiSan.Add(parameter.TaiSan);
                }
                else
                {
                    var taiSanEntity = context.TaiSan.FirstOrDefault(p => p.TaiSanId == parameter.TaiSan.TaiSanId);
                    if (taiSanEntity != null)
                    {
                        taiSanEntity.MaTaiSan = parameter.TaiSan.MaTaiSan;
                        taiSanEntity.TenTaiSan = parameter.TaiSan.TenTaiSan;
                        taiSanEntity.MaCode = parameter.TaiSan.MaTaiSan;
                        taiSanEntity.PhanLoaiTaiSanId = parameter.TaiSan.PhanLoaiTaiSanId;
                        taiSanEntity.NgayVaoSo = parameter.TaiSan.NgayVaoSo;
                        taiSanEntity.DonViTinhId = parameter.TaiSan.DonViTinhId;
                        taiSanEntity.SoLuong = parameter.TaiSan.SoLuong;
                        taiSanEntity.MoTa = parameter.TaiSan.MoTa;
                        taiSanEntity.KhuVucTaiSanId = parameter.TaiSan.KhuVucTaiSanId;

                        taiSanEntity.SoSerial = parameter.TaiSan.SoSerial;
                        taiSanEntity.SoHieu = parameter.TaiSan.SoHieu;
                        taiSanEntity.ThongTinNoiMua = parameter.TaiSan.ThongTinNoiMua;
                        taiSanEntity.ThongTinNoiBaoHanh = parameter.TaiSan.ThongTinNoiBaoHanh;
                        taiSanEntity.NamSx = parameter.TaiSan.NamSx;
                        taiSanEntity.NuocSxid = parameter.TaiSan.NuocSxid;
                        taiSanEntity.HangSxid = parameter.TaiSan.HangSxid;
                        taiSanEntity.NgayMua = parameter.TaiSan.NgayMua;
                        taiSanEntity.ThoiHanBaoHanh = parameter.TaiSan.ThoiHanBaoHanh;
                        taiSanEntity.BaoDuongDinhKy = parameter.TaiSan.BaoDuongDinhKy;
                        taiSanEntity.Model = parameter.TaiSan.Model;

                        taiSanEntity.GiaTriNguyenGia = parameter.TaiSan.GiaTriNguyenGia;
                        taiSanEntity.GiaTriTinhKhauHao = parameter.TaiSan.GiaTriTinhKhauHao;
                        taiSanEntity.ThoiGianKhauHao = parameter.TaiSan.ThoiGianKhauHao;
                        taiSanEntity.ThoiDiemBdtinhKhauHao = parameter.TaiSan.ThoiDiemBdtinhKhauHao;
                        taiSanEntity.PhuongPhapTinhKhauHao = parameter.TaiSan.PhuongPhapTinhKhauHao;

                        parameter.TaiSan.UpdatedById = parameter.UserId;
                        parameter.TaiSan.UpdatedDate = DateTime.Now;
                        context.TaiSan.Update(taiSanEntity);
                    }
                }
                context.SaveChanges();
                var listTaiSanChuaPhanBo = new List<AssetEntityModel>();
                if (parameter.IsQuick)
                {
                    #region Lấy danh Tài sản chưa phân bổ
                    listTaiSanChuaPhanBo = context.TaiSan.Where(x => x.HienTrangTaiSan == 0).Select(y => new AssetEntityModel
                    {
                        TaiSanId = y.TaiSanId,
                        MaTaiSan = y.MaTaiSan,
                        TenTaiSan = y.TenTaiSan,
                        TenTaiSanCode = y.MaTaiSan + " - " + y.TenTaiSan,
                        MaCode = y.MaCode,
                        PhanLoaiTaiSanId = y.PhanLoaiTaiSanId,
                    }).OrderByDescending(z => z.CreatedDate).ThenByDescending(z => z.NgayVaoSo).ToList();

                    #endregion
                }
                return new CreateOrUpdateAssetResult()
                {
                    Status = true,
                    StatusCode = HttpStatusCode.OK,
                    Message = parameter.TaiSan.TaiSanId == 0 ? "Thêm mới tài sản thành công." : "Cập nhập tài sản thành công",
                    AssetId = parameter.TaiSan.TaiSanId,
                    ListTaiSanChuaPhanBo = listTaiSanChuaPhanBo
                };
            }
            catch (Exception e)
            {
                return new CreateOrUpdateAssetResult()
                {
                    Message = e.Message,
                    StatusCode = HttpStatusCode.Forbidden,
                    Status = false
                };
            }

        }

        public GetMasterDataAssetFormResult GetMasterDataAssetForm(GetMasterDataAssetFormParameter parameter)
        {
            try
            {
                var ListMaTS = new List<string>();
                var ListPhanLoaiTS = new List<CategoryEntityModel>();
                var ListDonVi = new List<CategoryEntityModel>();
                var ListHienTrangTS = new List<CategoryEntityModel>();
                var ListNuocSX = new List<CategoryEntityModel>();
                var ListHangSX = new List<CategoryEntityModel>();
                var listEmployee = new List<EmployeeEntityModel>();
                var ListProvinceEntityModel = new List<ProvinceEntityModel>();
                #region Common data

                var listAllCategoryType = context.CategoryType.ToList();
                var listAllCategory = context.Category.ToList();

                #endregion

                #region List mã tài sản

                ListMaTS = context.TaiSan.Select(x => x.MaTaiSan).ToList();

                #endregion

                #region Lấy danh sách Phân loại tài sản

                var phanLoaiTSId = listAllCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "PLTS")?.CategoryTypeId;
                ListPhanLoaiTS = listAllCategory
                    .Where(x => x.Active == true && x.CategoryTypeId == phanLoaiTSId)
                    .Select(y => new CategoryEntityModel()
                    {
                        CategoryId = y.CategoryId,
                        CategoryCode = y.CategoryCode,
                        CategoryName = y.CategoryName
                    }).ToList();

                #endregion

                #region Lấy khu vực tài sản
                ListProvinceEntityModel = context.Province.Where(x =>x.IsShowAsset == true).Select(p => new ProvinceEntityModel()
                {
                    ProvinceId = p.ProvinceId,
                    ProvinceName = p.ProvinceName,
                    ProvinceCode = p.ProvinceCode,
                }).OrderBy(p => p.ProvinceName).ToList();
                #endregion

                #region Lấy danh sách Đơn vị

                var donViId = listAllCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "DVTS")?.CategoryTypeId;
                ListDonVi = listAllCategory
                    .Where(x => x.Active == true && x.CategoryTypeId == donViId)
                    .Select(y => new CategoryEntityModel()
                    {
                        CategoryId = y.CategoryId,
                        CategoryCode = y.CategoryCode,
                        CategoryName = y.CategoryName
                    }).ToList();

                #endregion

                #region Lấy danh sách Hiện trạng tài sản

                var hienTrangId = listAllCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "HTTS")?.CategoryTypeId;
                ListHienTrangTS = listAllCategory
                    .Where(x => x.Active == true && x.CategoryTypeId == hienTrangId)
                    .Select(y => new CategoryEntityModel()
                    {
                        CategoryId = y.CategoryId,
                        CategoryCode = y.CategoryCode,
                        CategoryName = y.CategoryName
                    }).ToList();

                #endregion

                #region Lấy danh sách Nước sản xuất

                var sanXuatId = listAllCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "NSX")?.CategoryTypeId;
                ListNuocSX = listAllCategory
                    .Where(x => x.Active == true && x.CategoryTypeId == sanXuatId)
                    .Select(y => new CategoryEntityModel()
                    {
                        CategoryId = y.CategoryId,
                        CategoryCode = y.CategoryCode,
                        CategoryName = y.CategoryName
                    }).ToList();

                #endregion

                #region Lấy danh sách Hãng sản xuất

                var hangSXId = listAllCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "HSX")?.CategoryTypeId;
                ListHangSX = listAllCategory
                    .Where(x => x.Active == true && x.CategoryTypeId == hangSXId)
                    .Select(y => new CategoryEntityModel()
                    {
                        CategoryId = y.CategoryId,
                        CategoryCode = y.CategoryCode,
                        CategoryName = y.CategoryName
                    }).ToList();

                #endregion

                #region Lấy danh sách nhân viên đang hoạt động
                var listAllUser = context.User.ToList();

                var listAllEmployee = context.Employee.Select(y =>
                           new EmployeeEntityModel
                           {
                               EmployeeId = y.EmployeeId,
                               Active = y.Active,
                               EmployeeCodeName = y.EmployeeCode + " - " + y.EmployeeName,
                           }).ToList();

                listAllEmployee.ForEach(emp =>
                {
                    var trangThaiId = 0;
                    var user = listAllUser.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId);

                    if (emp.Active == true && user.Active == true)
                    {
                        trangThaiId = 1; //Đang hoạt động - Được phê duyệt
                        listEmployee.Add(emp);
                    }
                    else if (emp.Active == true && user.Active == false)
                    {
                        trangThaiId = 2; //Đang hoạt động - Không được truy cập
                        listEmployee.Add(emp);
                    }
                    else
                    {
                        trangThaiId = 3; //Ngừng hoạt động
                        emp.SoNamLamViec = 0;
                    }
                    emp.TrangThaiId = trangThaiId;
                });

                #endregion

                #region Lấy danh sách Mục đích sử dụng
                var listMucDichSuDung = new List<CategoryEntityModel>();
                var mucDichId = listAllCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "MDSD")?.CategoryTypeId;
                listMucDichSuDung = listAllCategory
                    .Where(x => x.Active == true && x.CategoryTypeId == mucDichId)
                    .Select(y => new CategoryEntityModel()
                    {
                        CategoryId = y.CategoryId,
                        CategoryCode = y.CategoryCode,
                        CategoryName = y.CategoryName
                    }).ToList();

                #endregion

                return new GetMasterDataAssetFormResult()
                {
                    Status = true,
                    StatusCode = HttpStatusCode.OK,
                    ListPhanLoaiTS = ListPhanLoaiTS,
                    ListDonVi = ListDonVi,
                    ListHienTrangTS = ListHienTrangTS,
                    ListNuocSX = ListNuocSX,
                    ListHangSX = ListHangSX,
                    ListEmployee = listEmployee,
                    ListMucDichSuDung = listMucDichSuDung,
                    ListProvinceModel = ListProvinceEntityModel,
                    ListMaTS = ListMaTS
                };

            }
            catch (Exception e)
            {
                return new GetMasterDataAssetFormResult()
                {
                    Message = e.Message,
                    StatusCode = HttpStatusCode.Forbidden,
                    Status = false
                };
            }
        }

        public GetAllAssetListResult GetAllAssetList(GetAllAssetListParameter parameter)
        {
            var user = context.User.FirstOrDefault(c => c.UserId == parameter.UserId);
            if (user == null)
            {
                return new GetAllAssetListResult
                {
                    Message = "Nhân viên không tồn tại trong hệ thống",
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
            var listAllEmp = context.Employee.ToList();
            var employee = listAllEmp.FirstOrDefault(c => c.EmployeeId == user.EmployeeId);
            if (employee == null)
            {
                return new GetAllAssetListResult
                {
                    Message = "Nhân viên không tồn tại trong hệ thống",
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }

            #region Lấy danh sách Phân loại tài sản
            var listAllCategoryType = context.CategoryType.ToList();
            var listAllCategory = context.Category.ToList();

            var phanLoaiTSId = listAllCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "PLTS")?.CategoryTypeId;
            var listPhanLoai = listAllCategory
                .Where(x => x.Active == true && x.CategoryTypeId == phanLoaiTSId)
                .Select(y => new CategoryEntityModel()
                {
                    CategoryId = y.CategoryId,
                    CategoryCode = y.CategoryCode,
                    CategoryName = y.CategoryName
                }).ToList();

            #endregion
            
            var listEmployee = new List<EmployeeEntityModel>();

            #region Lấy danh sách nhân viên
            listEmployee = listAllEmp.Select(y =>
                       new EmployeeEntityModel
                       {
                           EmployeeId = y.EmployeeId,
                           EmployeeCodeName = y.EmployeeCode + " - " + y.EmployeeName,
                       }).ToList();
            #endregion

            var listAsset = new List<AssetEntityModel>();
            var listAllAsset = context.TaiSan.Select(y => new AssetEntityModel
            {
                TaiSanId = y.TaiSanId,
                MaTaiSan = y.MaTaiSan,
                TenTaiSan = y.TenTaiSan,
                NgayVaoSo = y.NgayVaoSo,
                HienTrangTaiSan = y.HienTrangTaiSan.Value,
                MaCode = y.MaCode,
                SoSerial = y.SoSerial,
                Model = y.Model,
                PhanLoaiTaiSanId = y.PhanLoaiTaiSanId,
                MoTa = y.MoTa,
                GiaTriNguyenGia = y.GiaTriNguyenGia,
                GiaTriTinhKhauHao = y.GiaTriTinhKhauHao,
                ThoiGianKhauHao = y.ThoiGianKhauHao,
                ThoiDiemBDTinhKhauHao = y.ThoiDiemBdtinhKhauHao,
                KhuVucTaiSanId = y.KhuVucTaiSanId 
            }).OrderByDescending(z => z.CreatedDate).ThenByDescending(z => z.NgayVaoSo).ToList();

            listAsset = listAllAsset.Where(x =>
                    (parameter.TenTaiSan == null || parameter.TenTaiSan == "" || x.TenTaiSan.Contains(parameter.TenTaiSan)) 
                    && (parameter.MaTaiSan == null || parameter.MaTaiSan == "" || x.MaTaiSan.Contains(parameter.MaTaiSan))
                    && (parameter.ListTrangThai == null || parameter.ListTrangThai.Count == 0 || parameter.ListTrangThai.Contains(x.HienTrangTaiSan))
                    && (parameter.ListLoaiTS == null || parameter.ListLoaiTS.Count == 0 || parameter.ListLoaiTS.Contains(x.PhanLoaiTaiSanId.Value))
                    && (parameter.ListProvinceId == null || parameter.ListProvinceId.Count == 0 || (x.KhuVucTaiSanId != null && parameter.ListProvinceId.Contains(x.KhuVucTaiSanId.Value)))
                    ).ToList();

            if(parameter.ListEmployee != null && parameter.ListEmployee.Count() > 0)
            {
                var lstTaiSanId = listAllAsset?.Where(x => x.HienTrangTaiSan == 1).Select(x => x.TaiSanId).ToList();
                var lstCapPhat = context.CapPhatTaiSan.Where(x =>lstTaiSanId.Contains(x.TaiSanId) && parameter.ListEmployee.Contains(x.NguoiSuDungId)).ToList();
                var lstTaiSanCapPhatId = lstCapPhat.Select(x => x.TaiSanId).ToList();

                if(lstTaiSanCapPhatId.Count() > 0)
                {
                    listAsset = listAsset.Where(x => lstTaiSanCapPhatId.Contains(x.TaiSanId)).ToList();
                }
                else
                    listAsset = new List<AssetEntityModel>(); 
            }
            var listSubCode1 = GeneralList.GetSubCode1().ToList();
            if (listAsset.Count() > 0)
            {
                var lstTaiSanId = listAsset.Select(x => x.TaiSanId).ToList();
                var lstPhanBo = context.CapPhatTaiSan.Where(x => lstTaiSanId.Contains(x.TaiSanId)).OrderByDescending(a => a.CreatedDate).ToList();
                var lstPosition = context.Position.Where(x => x.Active == true).ToList();
                var lstOrganization = context.Organization.ToList();

                listAsset.ForEach(p =>
                {
                    switch (p.HienTrangTaiSan)
                    {
                        case 1:
                            p.HienTrangTaiSanString = "Đang sử dụng";
                            p.BackgroundColorForStatus = "#0F62FE";
                            break;
                        case 0:
                            p.HienTrangTaiSanString = "Không sử dụng";
                            p.BackgroundColorForStatus = "#FFC000";
                            break;
                    }
                    p.PhanLoaiTaiSan = listPhanLoai.FirstOrDefault(x => x.CategoryId == p.PhanLoaiTaiSanId)?.CategoryName;

                    // Phân bổ tài sản
                    var phanbo = lstPhanBo.Where(x => x.TaiSanId == p.TaiSanId && p.HienTrangTaiSan == 1).OrderByDescending(x => x.NgayBatDau).ToList().FirstOrDefault();
                    if (phanbo != null)
                    {
                        var emp = listAllEmp.FirstOrDefault(x => x.EmployeeId == phanbo.NguoiSuDungId);
                        if (emp != null)
                        {
                            p.Account = emp?.EmployeeCode + " - " + emp?.EmployeeName;
                            var subCode = listSubCode1.FirstOrDefault(x => x.Value == emp?.DeptCodeValue)?.Name;
                            p.Dept = subCode + " - " + emp?.DiaDiemLamviec;
                            p.MaNV = emp.EmployeeCode;
                            p.HoVaTen = emp?.EmployeeName;
                            p.PhongBan = lstOrganization.FirstOrDefault(x => x.OrganizationId == emp.OrganizationId)?.OrganizationName;
                            p.ViTriLamViec = lstPosition.FirstOrDefault(x => x.PositionId == emp.PositionId)?.PositionName;
                        }    
                    }
                });
            }

            var companyConfigEntity = context.CompanyConfiguration.FirstOrDefault();
            var companyConfig = new CompanyConfigEntityModel();
            companyConfig.CompanyId = companyConfigEntity.CompanyId;
            companyConfig.CompanyName = companyConfigEntity.CompanyName;
            companyConfig.Email = companyConfigEntity.Email;
            companyConfig.Phone = companyConfigEntity.Phone;
            companyConfig.TaxCode = companyConfigEntity.TaxCode;
            companyConfig.CompanyAddress = companyConfigEntity.CompanyAddress;
            companyConfig.CompanyAddress = companyConfigEntity.CompanyAddress;

            var listProvince = context.Province.Where(x => x.IsShowAsset == true).Select(x => new ProvinceEntityModel
            {
                ProvinceId = x.ProvinceId,
                ProvinceName = x.ProvinceName
            }).ToList();

            // lấy danh sách khu vực
            return new GetAllAssetListResult()
            {
                StatusCode = HttpStatusCode.OK,
                Status = true,
                ListAsset = listAsset,
                CompanyConfig = companyConfig,
                ListEmployee = listEmployee,
                ListLoaiTaiSan = listPhanLoai,
                ListKhuVuc = listProvince
            };
        }

        public GetMasterDataPhanBoTSFormResult GetMasterDataPhanBoTSForm(GetMasterDataAssetFormParameter parameter)
        {
            try
            {
                var listLoaiTSPB = new List<CategoryEntityModel>();
                var listMucDichSuDung = new List<CategoryEntityModel>();
                var listEmployee = new List<EmployeeEntityModel>();
                var listTaiSan = new List<AssetEntityModel>();

                #region Common data
                var listAllCategoryType = context.CategoryType.ToList();
                var listAllCategory = context.Category.ToList();
                #endregion

                #region Lấy danh sách Phân loại tài sản
                var phanLoaiTSId = listAllCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "PLTS")?.CategoryTypeId;
                listLoaiTSPB = listAllCategory
                    .Where(x => x.Active == true && x.CategoryTypeId == phanLoaiTSId)
                    .Select(y => new CategoryEntityModel()
                    {
                        CategoryId = y.CategoryId,
                        CategoryCode = y.CategoryCode,
                        CategoryName = y.CategoryName
                    }).ToList();

                #endregion

                #region Lấy danh sách Mục đích sử dụng

                var donViId = listAllCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "MDSD")?.CategoryTypeId;
                listMucDichSuDung = listAllCategory
                    .Where(x => x.Active == true && x.CategoryTypeId == donViId)
                    .Select(y => new CategoryEntityModel()
                    {
                        CategoryId = y.CategoryId,
                        CategoryCode = y.CategoryCode,
                        CategoryName = y.CategoryName
                    }).ToList();

                #endregion

                #region Lấy danh sách nhân viên
                listEmployee = context.Employee.Select(y =>
                           new EmployeeEntityModel
                           {
                               EmployeeId = y.EmployeeId,
                               EmployeeCode = y.EmployeeCode,
                               EmployeeName = y.EmployeeName,
                               EmployeeCodeName = y.EmployeeCode + " - " + y.EmployeeName,
                               OrganizationId = y.OrganizationId,
                               PositionId = y.PositionId
                           }).ToList();

                var listPosition = context.Position.ToList();
                var listOrganization = context.Organization.ToList();

                listEmployee?.ForEach(item =>
                {
                    var phongBan = listOrganization.FirstOrDefault(x => x.OrganizationId == item.OrganizationId);
                    item.OrganizationName = phongBan?.OrganizationName;

                    var chucVu = listPosition.FirstOrDefault(x => x.PositionId == item.PositionId);
                    item.PositionName = chucVu?.PositionName;
                });
                #endregion

                #region Lấy danh Tài sản chưa phân bổ
                listTaiSan = context.TaiSan.Where(x => x.HienTrangTaiSan == 0).Select(y => new AssetEntityModel
                {
                    TaiSanId = y.TaiSanId,
                    MaTaiSan = y.MaTaiSan,
                    TenTaiSan = y.TenTaiSan,
                    TenTaiSanCode = y.MaTaiSan + " - " + y.TenTaiSan,
                    MaCode = y.MaCode,
                    PhanLoaiTaiSanId = y.PhanLoaiTaiSanId,
                }).OrderByDescending(z => z.CreatedDate).ThenByDescending(z => z.NgayVaoSo).ToList();

                #endregion

                return new GetMasterDataPhanBoTSFormResult()
                {
                    Status = true,
                    StatusCode = HttpStatusCode.OK,
                    ListLoaiTSPB = listLoaiTSPB,
                    ListMucDichSuDung = listMucDichSuDung,
                    ListEmployee = listEmployee,
                    ListTaiSan = listTaiSan
                };

            }
            catch (Exception e)
            {
                return new GetMasterDataPhanBoTSFormResult()
                {
                    Message = e.Message,
                    StatusCode = HttpStatusCode.Forbidden,
                    Status = false
                };
            }
        }
        public TaoPhanBoTaiSanResult TaoPhanBoTaiSan(TaoPhanBoTaiSanParameter parameter)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var listPhanBo = new List<CapPhatTaiSan>();
                    // Kiểm tra xem tài sản đã được phân bổ hay chưa
                    var lstTaiSanId = parameter.ListPhanBo.Select(x => x.TaiSanId).ToList();
                    var lstCapPhatTonTai = new List<int>();

                    var lstEmpId = parameter.ListPhanBo.Select(x => x.NguoiSuDungId).ToList();

                    var lstCapPhat = context.CapPhatTaiSan.Where(x => x.LoaiCapPhat == 1 && lstEmpId.Contains(x.NguoiSuDungId)).ToList();

                    var listTaiSan = context.TaiSan.Where(x => lstTaiSanId.Contains(x.TaiSanId)).ToList();

                    // Danh sách tài sản đã được cấp phát
                    lstTaiSanId.ForEach(taiSanId =>
                    {
                        if (listTaiSan.FirstOrDefault(x => x.HienTrangTaiSan == 1) != null)
                        {
                            lstCapPhatTonTai.Add(taiSanId);
                        }
                    });

                    if (lstCapPhatTonTai.Count() == 0)
                    {
                        var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                        var emp = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                        parameter.ListPhanBo.ForEach(item =>
                        {
                            CapPhatTaiSan capPhat = new CapPhatTaiSan();
                            capPhat.TaiSanId = item.TaiSanId;
                            capPhat.NguoiSuDungId = item.NguoiSuDungId;
                            capPhat.NguoiCapPhatId = emp.EmployeeId;

                            capPhat.MucDichSuDungId = item.MucDichSuDungId;
                            capPhat.NgayBatDau = item.NgayBatDau;
                            capPhat.NgayKetThuc = item.NgayKetThuc;
                            capPhat.LyDo = item.LyDo;
                            capPhat.LoaiCapPhat = 1; // cấp phát - 0 là thu hồi
                            capPhat.TrangThai = true;

                            capPhat.YeuCauCapPhatTaiSanChiTietId = item.YeuCauCapPhatTaiSanChiTietId;

                            capPhat.CreatedById = parameter.UserId;
                            capPhat.CreatedDate = DateTime.Now;
                            capPhat.UpdatedById = parameter.UserId;
                            capPhat.UpdatedDate = DateTime.Now;

                            listPhanBo.Add(capPhat);  
                        });

                        listTaiSan.ForEach(taisan =>
                        {
                            taisan.HienTrangTaiSan = 1;
                        });

                        context.TaiSan.UpdateRange(listTaiSan);
                        context.CapPhatTaiSan.AddRange(listPhanBo);
                        context.SaveChanges();

                        transaction.Commit();
                        return new TaoPhanBoTaiSanResult()
                        {
                            Status = true,
                            StatusCode = HttpStatusCode.OK,
                            Message = "Cấp phát tài sản thành công.",
                        };
                    }
                    else
                    {
                        return new TaoPhanBoTaiSanResult()
                        {
                            ListAssetId = lstCapPhatTonTai,
                            StatusCode = HttpStatusCode.Forbidden,
                            Status = false
                        };
                    }

                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    return new TaoPhanBoTaiSanResult()
                    {
                        Message = e.Message,
                        StatusCode = HttpStatusCode.Forbidden,
                        Status = false
                    };
                }
            }
        }

        public TaoPhanBoTaiSanResult TaoThuHoiTaiSan(TaoPhanBoTaiSanParameter parameter)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var listThuHoi = new List<CapPhatTaiSan>();
                    // Kiểm tra xem tài sản đã được thu hồi hay chưa
                    var lstTaiSanId = parameter.ListPhanBo.Select(x => x.TaiSanId).ToList();

                    var lstEmpId = parameter.ListPhanBo.Select(x => x.NguoiSuDungId).ToList();

                    // Danh sách tài sản đang được cấp phát
                    var listTaiSan = context.TaiSan.Where(x => lstTaiSanId.Contains(x.TaiSanId)).ToList();

                    var lstThuHoiTonTai = new List<int>();
                    // Lấy danh sách thu hổi
                    var lstThuHoi = context.CapPhatTaiSan.Where(x => x.LoaiCapPhat == 0 && lstEmpId.Contains(x.NguoiSuDungId)).ToList();

                    // Danh sách tài sản đã được thu hồi
                    lstTaiSanId.ForEach(taiSanId =>
                    {
                        if (listTaiSan.FirstOrDefault(x => x.HienTrangTaiSan == 0) != null)
                        {
                            lstThuHoiTonTai.Add(taiSanId);
                        }
                    });

                    if (lstThuHoiTonTai.Count() == 0)
                    {
                        var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                        var emp = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                        parameter.ListPhanBo.ForEach(item =>
                        {
                            CapPhatTaiSan thuHoi = new CapPhatTaiSan();
                            thuHoi.TaiSanId = item.TaiSanId;
                            thuHoi.NguoiSuDungId = item.NguoiSuDungId;
                            thuHoi.NguoiCapPhatId = emp.EmployeeId;

                            thuHoi.MucDichSuDungId = item.MucDichSuDungId;
                            thuHoi.NgayBatDau = item.NgayBatDau;
                            thuHoi.NgayKetThuc = null;
                            thuHoi.LyDo = item.LyDo;
                            thuHoi.LoaiCapPhat = 0; // 0 là thu hồi - 1 là cấp phát
                            thuHoi.TrangThai = false;

                            thuHoi.CreatedById = parameter.UserId;
                            thuHoi.CreatedDate = DateTime.Now;
                            thuHoi.UpdatedById = parameter.UserId;
                            thuHoi.UpdatedDate = DateTime.Now;

                            listThuHoi.Add(thuHoi);
                        });

                        listTaiSan.ForEach(taisan =>
                        {
                            taisan.HienTrangTaiSan = 0;
                        });
                        context.TaiSan.UpdateRange(listTaiSan);
                        context.CapPhatTaiSan.AddRange(listThuHoi);

                        context.SaveChanges();
                        transaction.Commit();

                        return new TaoPhanBoTaiSanResult()
                        {
                            Status = true,
                            StatusCode = HttpStatusCode.OK,
                            Message = "Thu hồi tài sản thành công.",
                        };
                    }
                    else
                    {
                        return new TaoPhanBoTaiSanResult()
                        {
                            ListAssetId = lstThuHoiTonTai,
                            StatusCode = HttpStatusCode.Forbidden,
                            Status = false
                        };
                    }

                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    return new TaoPhanBoTaiSanResult()
                    {
                        Message = e.Message,
                        StatusCode = HttpStatusCode.Forbidden,
                        Status = false
                    };
                }
            }
        }
        public GetMasterDataPhanBoTSFormResult GetMasterDataThuHoiTSForm(GetMasterDataAssetFormParameter parameter)
        {
            try
            {
                var listLoaiTS = new List<CategoryEntityModel>();
                var listEmployee = new List<EmployeeEntityModel>();
                var listTaiSan = new List<AssetEntityModel>();

                #region Common data
                var listAllCategoryType = context.CategoryType.ToList();
                var listAllCategory = context.Category.ToList();
                #endregion

                #region Lấy danh sách Phân loại tài sản
                var phanLoaiTSId = listAllCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "PLTS")?.CategoryTypeId;
                listLoaiTS = listAllCategory
                    .Where(x => x.Active == true && x.CategoryTypeId == phanLoaiTSId)
                    .Select(y => new CategoryEntityModel()
                    {
                        CategoryId = y.CategoryId,
                        CategoryCode = y.CategoryCode,
                        CategoryName = y.CategoryName
                    }).ToList();

                #endregion

                #region Lấy danh sách nhân viên
                listEmployee = context.Employee.Select(y =>
                           new EmployeeEntityModel
                           {
                               EmployeeId = y.EmployeeId,
                               EmployeeCode = y.EmployeeCode,
                               EmployeeName = y.EmployeeName,
                               EmployeeCodeName = y.EmployeeCode + " - " + y.EmployeeName,
                               OrganizationId = y.OrganizationId,
                               PositionId = y.PositionId
                           }).ToList();

                var listPosition = context.Position.ToList();
                var listOrganization = context.Organization.ToList();

                listEmployee?.ForEach(item =>
                {
                    var phongBan = listOrganization.FirstOrDefault(x => x.OrganizationId == item.OrganizationId);
                    item.OrganizationName = phongBan?.OrganizationName;

                    var chucVu = listPosition.FirstOrDefault(x => x.PositionId == item.PositionId);
                    item.PositionName = chucVu?.PositionName;
                });
                #endregion

                #region Lấy danh Tài sản đã phẩn bổ
                var lstAllTaiSanCapPhat = context.CapPhatTaiSan.Where(x => x.LoaiCapPhat == 1).ToList();
                var lstTaiSanDaCapPhatId = lstAllTaiSanCapPhat.Select(x => x.TaiSanId).ToList();

                var lstAllTaiSanPhanBo = context.TaiSan.Where(x => x.HienTrangTaiSan == 1).ToList();

                listTaiSan = (from taisan in lstAllTaiSanPhanBo
                                select new AssetEntityModel
                              {
                                  TaiSanId = taisan.TaiSanId,
                                  MaTaiSan = taisan.MaTaiSan,
                                  TenTaiSan = taisan.TenTaiSan,
                                  TenTaiSanCode = taisan.MaTaiSan + " - " + taisan.TenTaiSan,
                                  MaCode = taisan.MaCode,
                                  PhanLoaiTaiSanId = taisan.PhanLoaiTaiSanId,
                              }).OrderBy(x => x.TenTaiSan).ToList();

                listTaiSan.ForEach(ts =>
                {
                    ts.NguoiSuDungId = lstAllTaiSanCapPhat.OrderByDescending(x => x.NgayBatDau).FirstOrDefault(x => x.TaiSanId == ts.TaiSanId)?.NguoiSuDungId;
                    if(ts.NguoiSuDungId != null)
                    {
                        var nhanvien = listEmployee.FirstOrDefault(x => x.EmployeeId == ts.NguoiSuDungId);
                        ts.MaNV = nhanvien.EmployeeCode;
                        ts.HoVaTen = nhanvien.EmployeeName;
                        ts.ViTriLamViec = nhanvien?.PositionName;
                        ts.PhongBan = nhanvien?.OrganizationName;
                    }          
                });

                #endregion
                return new GetMasterDataPhanBoTSFormResult()
                {
                    Status = true,
                    StatusCode = HttpStatusCode.OK,
                    ListLoaiTSPB = listLoaiTS,
                    ListEmployee = listEmployee,
                    ListTaiSan = listTaiSan
                };

            }
            catch (Exception e)
            {
                return new GetMasterDataPhanBoTSFormResult()
                {
                    Message = e.Message,
                    StatusCode = HttpStatusCode.Forbidden,
                    Status = false
                };
            }
        }

        public CreateOrUpdateBaoDuongResult CreateOrUpdateBaoDuong(CreateOrUpdateBaoDuongParameter parameter)
        {
            try
            {
                if (parameter.BaoDuong.BaoDuongTaiSanId == 0)
                {
                    parameter.BaoDuong.CreatedById = parameter.UserId;
                    parameter.BaoDuong.CreatedDate = DateTime.Now;
                    context.BaoDuongTaiSan.Add(parameter.BaoDuong);
                }
                else
                {
                    var baoDuong = context.BaoDuongTaiSan.FirstOrDefault(x => x.BaoDuongTaiSanId == parameter.BaoDuong.BaoDuongTaiSanId);
                    if (baoDuong != null)
                    {
                        baoDuong.MoTa = parameter.BaoDuong.MoTa;
                        baoDuong.TuNgay = parameter.BaoDuong.TuNgay;
                        baoDuong.DenNgay = parameter.BaoDuong.DenNgay;
                        baoDuong.NguoiPhuTrachId = parameter.BaoDuong.NguoiPhuTrachId;
                        baoDuong.UpdatedById = parameter.UserId;
                        baoDuong.UpdatedDate = DateTime.Now;

                        context.BaoDuongTaiSan.Update(baoDuong);
                    }
                }

                context.SaveChanges();

                #region Lấy danh sách nhân viên
                var listEmployee = new List<EmployeeEntityModel>();
                listEmployee = context.Employee.Select(y =>
                           new EmployeeEntityModel
                           {
                               EmployeeId = y.EmployeeId,
                               EmployeeName = y.EmployeeName,
                           }).ToList();
                #endregion
                #region Bảo dưỡng, bảo trì
                var listBaoDuong = new List<BaoDuongEntityModel>();
                listBaoDuong = context.BaoDuongTaiSan.Where(x => x.TaiSanId == parameter.BaoDuong.TaiSanId).Select(y =>
                           new BaoDuongEntityModel
                           {
                               TaiSanId = y.TaiSanId,
                               BaoDuongTaiSanId = y.BaoDuongTaiSanId,
                               TuNgay = y.TuNgay,
                               DenNgay = y.DenNgay,
                               NguoiPhuTrachId = y.NguoiPhuTrachId,
                               MoTa = y.MoTa
                           }).ToList();

                listBaoDuong.ForEach(baoduong =>
                {
                    baoduong.NguoiPhuTrach = listEmployee.FirstOrDefault(x => x.EmployeeId == baoduong.NguoiPhuTrachId)?.EmployeeName;
                });
                #endregion
                return new CreateOrUpdateBaoDuongResult()
                {
                    Status = true,
                    StatusCode = HttpStatusCode.OK,
                    Message = parameter.BaoDuong.BaoDuongTaiSanId == 0 ? "Thêm mới bảo dưỡng thành công." : "Cập nhập bảo dưỡng thành công.",
                    BaoDuongId = parameter.BaoDuong.TaiSanId,
                    ListBaoDuong = listBaoDuong
                };
            }
            catch (Exception e)
            {
                return new CreateOrUpdateBaoDuongResult()
                {
                    Message = e.Message,
                    StatusCode = HttpStatusCode.Forbidden,
                    Status = false
                };
            }
        }

        public DeleteBaoDuongResult DeleteBaoDuong(DeleteBaoDuongParameter parameter)
        {
            try
            {
                var baoduong = context.BaoDuongTaiSan.FirstOrDefault(x => x.BaoDuongTaiSanId == parameter.BaoDuongId);

                if (baoduong != null)
                {
                    context.BaoDuongTaiSan.Remove(baoduong);
                    context.SaveChanges();
                }
                else
                {
                    return new DeleteBaoDuongResult
                    {
                        StatusCode = HttpStatusCode.FailedDependency,
                        MessageCode = "Không tồn tại bảo dưỡng!"
                    };
                }

                return new DeleteBaoDuongResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Xóa bảo dưỡng thành công"
                };
            }
            catch (Exception e)
            {
                return new DeleteBaoDuongResult
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                };
            }
        }

        public GetDataAssetDetailResult GetDataAssetDetail(GetDataAssetDetailParameter parameter)
        {
            try
            {
                int pageSize = 10;
                int pageIndex = 1;
                var taiSanDetail = context.TaiSan.Where(x => x.TaiSanId == parameter.TaiSanId).Select(taiSan => new AssetEntityModel
                {
                    TaiSanId = taiSan.TaiSanId,
                    MaTaiSan = taiSan.MaTaiSan,
                    TenTaiSan = taiSan.TenTaiSan,
                    MaCode = taiSan.MaCode,
                    PhanLoaiTaiSanId = taiSan.PhanLoaiTaiSanId,
                    NgayVaoSo = taiSan.NgayVaoSo,
                    HienTrangTaiSan = taiSan.HienTrangTaiSan.Value,
                    DonViTinhId = taiSan.DonViTinhId,
                    SoLuong = taiSan.SoLuong,
                    MoTa = taiSan.MoTa,

                    SoSerial = taiSan.SoSerial,
                    Model = taiSan.Model,
                    SoHieu = taiSan.SoHieu,
                    ThongTinNoiMua = taiSan.ThongTinNoiMua,
                    NamSX = taiSan.NamSx,
                    HangSXId = taiSan.HangSxid,
                    NuocSXId = taiSan.NuocSxid,
                    NgayMua = taiSan.NgayMua,
                    ThoiHanBaoHanh = taiSan.ThoiHanBaoHanh,
                    BaoDuongDinhKy = taiSan.BaoDuongDinhKy,
                    ThongTinNoiBaoHanh = taiSan.ThongTinNoiBaoHanh,
                    KhuVucTaiSanId = taiSan.KhuVucTaiSanId,
                    GiaTriNguyenGia = taiSan.GiaTriNguyenGia,
                    GiaTriTinhKhauHao = taiSan.GiaTriTinhKhauHao,
                    TiLeKhauHao = taiSan.TiLeKhauHao.Value,
                    ThoiGianKhauHao = taiSan.ThoiGianKhauHao,
                    ThoiDiemBDTinhKhauHao = taiSan.ThoiDiemBdtinhKhauHao,
                    PhuongPhapTinhKhauHao = taiSan.PhuongPhapTinhKhauHao
                }).ToList();

                if (taiSanDetail.Count() == 0)
                {
                    return new GetDataAssetDetailResult()
                    {
                        Message = "Không tồn tại tài sản!",
                        StatusCode = HttpStatusCode.FailedDependency,
                        Status = false
                    };
                }
                var user = context.User.FirstOrDefault(c => c.UserId == parameter.UserId);
                var listTaiSanPhanBo = new List<AssetEntityModel>();
                var listEmployee = new List<EmployeeEntityModel>();
                var listBaoDuong = new List<BaoDuongEntityModel>();

                #region Lấy danh sách nhân viên
                listEmployee = context.Employee.Select(y =>
                           new EmployeeEntityModel
                           {
                               EmployeeId = y.EmployeeId,
                               OrganizationId = y.OrganizationId,
                               PositionId = y.PositionId,
                               EmployeeCode = y.EmployeeCode,
                               EmployeeName = y.EmployeeName,
                               EmployeeCodeName = y.EmployeeCode + " - " + y.EmployeeName,
                           }).ToList();

                var listPosition = context.Position.ToList();
                var listOrganization = context.Organization.ToList();

                listEmployee?.ForEach(item =>
                {
                    var phongBan = listOrganization.FirstOrDefault(x => x.OrganizationId == item.OrganizationId);
                    item.OrganizationName = phongBan?.OrganizationName;

                    var chucVu = listPosition.FirstOrDefault(x => x.PositionId == item.PositionId);
                    item.PositionName = chucVu?.PositionName;
                });
                #endregion

                #region Bảo dưỡng, bảo trì
                listBaoDuong = context.BaoDuongTaiSan.Where(x => x.TaiSanId == parameter.TaiSanId).Select(y =>
                           new BaoDuongEntityModel
                           {
                               TaiSanId = y.TaiSanId,
                               BaoDuongTaiSanId = y.BaoDuongTaiSanId,
                               TuNgay = y.TuNgay,
                               DenNgay = y.DenNgay,
                               NguoiPhuTrachId = y.NguoiPhuTrachId,
                               MoTa = y.MoTa
                           }).ToList();

                listBaoDuong.ForEach(baoduong =>
                {
                    baoduong.NguoiPhuTrach = listEmployee.FirstOrDefault(x => x.EmployeeId == baoduong.NguoiPhuTrachId)?.EmployeeName;
                });
                #endregion

                #region Lấy danh lịch sử phẩn bổ và thu hồi

                var lstAllTaiSanCapPhat = context.CapPhatTaiSan.ToList();
                var lstTaiSanDaCapPhatId = lstAllTaiSanCapPhat.Select(x => x.TaiSanId).ToList();

                listTaiSanPhanBo = (from taisan in taiSanDetail
                                    join capphat in lstAllTaiSanCapPhat on taisan.TaiSanId equals capphat.TaiSanId
                                    //into cu
                                    //from x in cu.DefaultIfEmpty()
                                    select new AssetEntityModel
                                    {
                                        TaiSanId = taisan.TaiSanId,
                                        MaTaiSan = taisan.MaTaiSan,
                                        TenTaiSan = taisan.TenTaiSan,
                                        TenTaiSanCode = taisan.MaTaiSan + " - " + taisan.TenTaiSan,
                                        MaCode = taisan.MaCode,
                                        PhanLoaiTaiSanId = taisan.PhanLoaiTaiSanId,
                                        NguoiSuDungId = capphat.NguoiSuDungId,
                                        NgayBatDau = capphat.NgayBatDau,
                                        NgayKetThuc = capphat.NgayKetThuc,
                                        LoaiCapPhat = capphat.LoaiCapPhat == 0 ? "Thu hồi" : "Cấp phát",
                                        LyDo = capphat.LyDo
                                    }).OrderBy(x => x.CreatedDate).ToList();

                listTaiSanPhanBo.ForEach(ts =>
                {
                    if (ts.NguoiSuDungId != Guid.Empty)
                    {
                        var nhanvien = listEmployee.FirstOrDefault(x => x.EmployeeId == ts.NguoiSuDungId);
                        ts.MaNV = nhanvien.EmployeeCode;
                        ts.HoVaTen = nhanvien.EmployeeName;
                        ts.ViTriLamViec = nhanvien?.PositionName;
                        ts.PhongBan = nhanvien?.OrganizationName;
                        ts.NguoiCapPhat = listEmployee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId).EmployeeName;
                    }
                });

                taiSanDetail.FirstOrDefault().NguoiSuDungId = listTaiSanPhanBo.Count() == 0 ? Guid.Empty : listTaiSanPhanBo?.OrderByDescending(x => x.CreatedDate).FirstOrDefault().NguoiSuDungId;

                #endregion

                #region Lấy dách file đinh kèm 
                var objectType = "ASSET";
                var folderCommon = context.Folder.ToList();
                var folder = folderCommon.FirstOrDefault(x => x.FolderType == objectType);

                var listFileResult = context.FileInFolder
                                .Where(x => x.ObjectNumber == parameter.TaiSanId && x.FolderId == folder.FolderId).Select(y =>
                                    new FileInFolderEntityModel
                                    {
                                        Size = y.Size,
                                        ObjectId = y.ObjectId,
                                        Active = y.Active,
                                        FileExtension = y.FileExtension,
                                        FileInFolderId = y.FileInFolderId,
                                        FileName = y.FileName,
                                        FolderId = y.FolderId,
                                        ObjectType = y.ObjectType,
                                        ObjectNumber = y.ObjectNumber,
                                        CreatedById = y.CreatedById,
                                        CreatedDate = y.CreatedDate,
                                        UpdatedById = y.UpdatedById,
                                        UpdatedDate = y.UpdatedDate
                                    }).OrderBy(z => z.CreatedDate).ToList();

                listFileResult.ForEach(x =>
                {
                    x.UploadByName = context.User.FirstOrDefault(u => u.UserId == x.CreatedById)?.UserName;
                });
                #endregion

                #region Lấy thông tin ghi chú
                var listNote = new List<NoteEntityModel>();

                var folderUrl = folderCommon.FirstOrDefault(x => x.FolderType == objectType)?.Url;
                var webRootPath = hostingEnvironment.WebRootPath + "\\";
                // list ghi chú 
                listNote = context.Note.Where(x =>
                        x.ObjectNumber == parameter.TaiSanId && x.ObjectType == objectType && x.Active == true)
                    .Select(y => new NoteEntityModel
                    {
                        NoteId = y.NoteId,
                        Description = y.Description,
                        Type = y.Type,
                        ObjectNumber = y.ObjectNumber,
                        ObjectType = y.ObjectType,
                        NoteTitle = y.NoteTitle,
                        Active = y.Active,
                        CreatedById = y.CreatedById,
                        CreatedDate = y.CreatedDate,
                        UpdatedById = y.UpdatedById,
                        UpdatedDate = y.UpdatedDate,
                        ResponsibleName = "",
                        ResponsibleAvatar = "",
                        NoteDocList = new List<NoteDocumentEntityModel>()
                    }).ToList();


                if (listNote.Count > 0)
                {
                    var listNoteId = listNote.Select(x => x.NoteId).ToList();
                    var listUser = context.User.ToList();
                    var _listAllEmployee = context.Employee.ToList();
                    var listNoteDocument = context.NoteDocument.Where(x => listNoteId.Contains(x.NoteId)).Select(
                        y => new NoteDocumentEntityModel
                        {
                            DocumentName = y.DocumentName,
                            DocumentSize = y.DocumentSize,
                            DocumentUrl = y.DocumentUrl,
                            CreatedById = y.CreatedById,
                            CreatedDate = y.CreatedDate,
                            UpdatedById = y.UpdatedById,
                            UpdatedDate = y.UpdatedDate,
                            NoteDocumentId = y.NoteDocumentId,
                            NoteId = y.NoteId
                        }
                    ).ToList();

                    var listFileInFolder = context.FileInFolder.Where(x => listNoteId.Contains((Guid)x.ObjectId))
                        .ToList();

                    listFileInFolder.ForEach(item =>
                    {
                        var file = new NoteDocumentEntityModel
                        {
                            DocumentName = item.FileName.Substring(0, item.FileName.LastIndexOf("_")),
                            DocumentSize = item.Size,
                            CreatedById = item.CreatedById,
                            CreatedDate = item.CreatedDate,
                            UpdatedById = item.UpdatedById,
                            UpdatedDate = item.UpdatedDate,
                            NoteDocumentId = item.FileInFolderId,
                            NoteId = (Guid)item.ObjectId
                        };

                        var fileName = $"{item.FileName}.{item.FileExtension}";
                        var folderName = ConvertFolderUrl(folderUrl);

                        file.DocumentUrl = Path.Combine(webRootPath, folderName, fileName);

                        listNoteDocument.Add(file);
                    });

                    listNote.ForEach(item =>
                    {
                        var _user = listUser.FirstOrDefault(x => x.UserId == item.CreatedById);
                        if (_user != null)
                        {
                            var _employee = _listAllEmployee.FirstOrDefault(x => x.EmployeeId == _user.EmployeeId);
                            item.ResponsibleName = _employee.EmployeeName;
                            item.NoteDocList = listNoteDocument.Where(x => x.NoteId == item.NoteId)
                                .OrderBy(z => z.UpdatedDate).ToList();
                        }
                    });

                    // Sắp xếp lại listnote
                    listNote = listNote.OrderByDescending(x => x.CreatedDate).ToList();

                    listNote = listNote
                        .Skip(pageSize * (pageIndex - 1))
                        .Take(pageSize).ToList();
                }

                #endregion



                return new GetDataAssetDetailResult()
                {
                    Status = true,
                    StatusCode = HttpStatusCode.OK,
                    ListTaiSanPhanBo = listTaiSanPhanBo,
                    ListBaoDuong = listBaoDuong,
                    AssetDetail = taiSanDetail.FirstOrDefault(),
                    ListFileInFolder = listFileResult,
                    ListNote = listNote,
                };

            }
            catch (Exception e)
            {
                return new GetDataAssetDetailResult()
                {
                    Message = e.Message,
                    StatusCode = HttpStatusCode.Forbidden,
                    Status = false
                };
            }
        }

        public UploadFileVacanciesResult UploadFile(UploadFileAssetParameter parameter)
        {
            var folder = context.Folder.FirstOrDefault(x => x.FolderType == parameter.FolderType);

            if (folder == null)
            {
                return new UploadFileVacanciesResult()
                {
                    Status = false,
                    Message = "Thư mục upload không tồn tại"
                };
            }

            var listFileDelete = new List<string>();
            try
            {
                var listFileResult = new List<FileInFolderEntityModel>();
                if (parameter.ListFile != null && parameter.ListFile.Count > 0)
                {
                    bool isSave = true;
                    parameter.ListFile.ForEach(item =>
                    {
                        if (folder == null)
                        {
                            isSave = false;
                        }
                        string folderName = ConvertFolderUrl(folder.Url);
                        string webRootPath = hostingEnvironment.WebRootPath;
                        string newPath = Path.Combine(webRootPath, folderName);

                        if (!Directory.Exists(newPath))
                        {
                            isSave = false;
                        }

                        if (isSave)
                        {
                            var file = new FileInFolder()
                            {
                                Active = true,
                                CreatedById = parameter.UserId,
                                CreatedDate = DateTime.Now,
                                FileInFolderId = Guid.NewGuid(),
                                FileName = item.FileInFolder.FileName + "_" + Guid.NewGuid().ToString(),
                                FolderId = folder.FolderId,
                                ObjectId = item.FileInFolder.ObjectId,
                                ObjectType = item.FileInFolder.ObjectType,
                                ObjectNumber = parameter.ObjectNumber,
                                Size = item.FileInFolder.Size,
                                FileExtension =
                                    item.FileSave.FileName.Substring(item.FileSave.FileName.LastIndexOf(".") + 1)
                            };
                            context.Add(file);

                            string fileName = file.FileName + "." + file.FileExtension;

                            if (isSave)
                            {
                                string fullPath = Path.Combine(newPath, fileName);
                                using (var stream = new FileStream(fullPath, FileMode.Create))
                                {
                                    item.FileSave.CopyTo(stream);
                                    listFileDelete.Add(fullPath);
                                }
                            }
                        }
                    });
                    if (!isSave)
                    {
                        listFileDelete.ForEach(item =>
                        {
                            File.Delete(item);
                        });

                        return new UploadFileVacanciesResult()
                        {
                            Status = false,
                            Message = "Bạn phải cấu hình thư mục để lưu"
                        };
                    }
                }

                context.SaveChanges();

                #region Lấy danh sách file
                var listCommonFolders = context.Folder.Where(x => x.ObjectNumber == parameter.ObjectNumber && x.FolderType == parameter.FolderType)
               .Select(y => new FolderEntityModel
               {
                   FolderId = y.FolderId,
                   ParentId = y.ParentId,
                   Name = y.Name,
               }).ToList();

                listCommonFolders.ForEach(item =>
                {
                    item.HasChild = context.Folder.FirstOrDefault(x => x.ParentId == item.FolderId) != null;
                });

                var listCommonFile = context.FileInFolder.Where(x => x.ObjectNumber == parameter.ObjectNumber).ToList();

                var webRootPathR = hostingEnvironment.WebRootPath + "\\";

                var folderObject = context.Folder.FirstOrDefault(x => x.FolderType == parameter.FolderType);
                listFileResult = GetAllFile(folderObject.FolderId, listCommonFolders, listCommonFile);


                listFileResult.ForEach(x =>
                {
                    x.UploadByName = context.User.FirstOrDefault(u => u.UserId == x.CreatedById)?.UserName;
                    x.FileFullName = $"{x.FileName}.{x.FileExtension}";
                    var folderUrl = context.Folder.FirstOrDefault(item => item.FolderId == x.FolderId)?.Url;
                    x.FileUrl = Path.Combine(webRootPathR, folderUrl, x.FileFullName);
                });

                listFileResult = listFileResult.OrderBy(x => x.CreatedDate).ToList();
                #endregion

                return new UploadFileVacanciesResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    ListFileInFolder = listFileResult
                };
            }
            catch (Exception ex)
            {
                listFileDelete.ForEach(item =>
                {
                    Directory.Delete(item);
                });

                return new UploadFileVacanciesResult()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    Message = ex.Message
                };
            }
        }
        private List<FileInFolderEntityModel> GetAllFile(Guid folderId, List<FolderEntityModel> listCommonFolders, List<FileInFolder> listCommonFile)
        {
            var listResult = new List<FileInFolderEntityModel>();

            var listFile = listCommonFile.Where(x => x.FolderId == folderId).ToList();

            listFile.ForEach(item =>
            {
                var fileInFolder = new FileInFolderEntityModel
                {
                    FileInFolderId = item.FileInFolderId,
                    FolderId = item.FolderId,
                    FileName = item.FileName,
                    ObjectId = item.ObjectId,
                    ObjectType = item.ObjectType,
                    FileExtension = item.FileExtension,
                    Size = item.Size,
                    Active = item.Active,
                    CreatedById = item.CreatedById,
                    CreatedDate = item.CreatedDate,
                    UpdatedById = item.UpdatedById,
                    UpdatedDate = item.UpdatedDate
                };

                listResult.Add(fileInFolder);
            });

            var folder = listCommonFolders.FirstOrDefault(x => x.FolderId == folderId);

            if (folder != null && folder.HasChild)
            {
                var listFolderChild = listCommonFolders.Where(x => x.ParentId == folderId).ToList();

                listFolderChild.ForEach(item =>
                {
                    listResult.AddRange(GetAllFile(item.FolderId, listCommonFolders, listCommonFile));
                });
            }

            return listResult;
        }
        private string ConvertFolderUrl(string url)
        {
            var stringResult = url.Split(@"\");
            string result = "";
            for (int i = 0; i < stringResult.Length; i++)
            {
                result = result + stringResult[i] + "\\";
            }

            result = result.Substring(0, result.Length - 1);

            return result;
        }

        public DownloadTemplateAssetResult DownloadTemplateAsset(DownloadTemplateAssetParameter parameter)
        {
            try
            {
                string rootFolder = hostingEnvironment.WebRootPath + "\\ExcelTemplate";
                string fileName = @"";
                var mess = "";
                if (parameter.PhanLoai == 1)
                {
                    fileName = @"Template_PhanBo_TaiSan.xlsx";
                    mess = "Template_PhanBo_TaiSan";
                }
                else if (parameter.PhanLoai == 0)
                {
                    fileName = @"Template_ThuHoi_TaiSan.xlsx";
                    mess = "Template_ThuHoi_TaiSan";
                }
                else if (parameter.PhanLoai == 2)
                {
                    fileName = @"Template_YeuCauTaiSan.xlsx";
                    mess = "Template_YeuCauTaiSan";
                }
                //FileInfo file = new FileInfo(Path.Combine(rootFolder, fileName));
                string newFilePath = Path.Combine(rootFolder, fileName);
                byte[] data = File.ReadAllBytes(newFilePath);

                return new DownloadTemplateAssetResult
                {
                    ExcelFile = data,
                    MessageCode = string.Format("Đã dowload file {0}", mess),
                    NameFile = mess,
                    StatusCode = System.Net.HttpStatusCode.OK
                };

            }
            catch (Exception)
            {
                return new DownloadTemplateAssetResult
                {
                    MessageCode = "Đã có lỗi xảy ra trong quá trình download",
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                };
            }
        }

        #region YÊU CẦU CẤP PHÁT
        public GetMasterDataPhanBoTSFormResult GetMasterDataYeuCauCapPhatForm(GetMasterDataAssetFormParameter parameter)
        {
            try
            {
                #region Check permision: manager
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId && x.Active == true);
                if (user == null)
                {
                    return new GetMasterDataPhanBoTSFormResult
                    {
                        Status = false,
                        Message = "User không có quyền truy xuất dữ liệu trong hệ thống"
                    };
                }
                if (user.EmployeeId == null || user.EmployeeId == Guid.Empty)
                {
                    return new GetMasterDataPhanBoTSFormResult
                    {
                        Status = false,
                        Message = "Lỗi dữ liệu"
                    };
                }

                #endregion

                var listLoaiTS = new List<CategoryEntityModel>();
                var listMucDichSuDung = new List<CategoryEntityModel>();
                var listEmployee = new List<EmployeeEntityModel>();
                var ListDonVi = new List<CategoryEntityModel>();
                #region Common data
                var listAllCategoryType = context.CategoryType.ToList();
                var listAllCategory = context.Category.ToList();
                #endregion

                #region Lấy danh sách Phân loại tài sản
                var phanLoaiTSId = listAllCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "PLTS")?.CategoryTypeId;
                listLoaiTS = listAllCategory
                    .Where(x => x.Active == true && x.CategoryTypeId == phanLoaiTSId)
                    .Select(y => new CategoryEntityModel()
                    {
                        CategoryId = y.CategoryId,
                        CategoryCode = y.CategoryCode,
                        CategoryName = y.CategoryName
                    }).ToList();

                #endregion

                #region Lấy danh sách Mục đích sử dụng

                var mucDichId = listAllCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "MDSD")?.CategoryTypeId;
                listMucDichSuDung = listAllCategory
                    .Where(x => x.Active == true && x.CategoryTypeId == mucDichId)
                    .Select(y => new CategoryEntityModel()
                    {
                        CategoryId = y.CategoryId,
                        CategoryCode = y.CategoryCode,
                        CategoryName = y.CategoryName
                    }).ToList();

                #endregion

                #region Lấy danh sách nhân viên
                var listAllUser = context.User.ToList();
                var listAllEmployee = context.Employee.Select(y =>
                         new EmployeeEntityModel
                         {
                             EmployeeId = y.EmployeeId,
                             EmployeeCode = y.EmployeeCode,
                             EmployeeName = y.EmployeeName,
                             EmployeeCodeName = y.EmployeeCode + " - " + y.EmployeeName,
                             OrganizationId = y.OrganizationId,
                             PositionId = y.PositionId,
                             IsManager = y.IsManager,
                             Active = y.Active
                         }).OrderBy(x => x.EmployeeCode).ToList();

                var listPosition = context.Position.ToList();
                var listOrganization = context.Organization.ToList();

                listAllEmployee?.ForEach(item =>
                {
                    var phongBan = listOrganization.FirstOrDefault(x => x.OrganizationId == item.OrganizationId);
                    item.OrganizationName = phongBan?.OrganizationName;

                    var chucVu = listPosition.FirstOrDefault(x => x.PositionId == item.PositionId);
                    item.PositionName = chucVu?.PositionName;

                    var trangThaiId = 0;
                    var userInfor = listAllUser.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);

                    if (item.Active == true && userInfor.Active == true)
                    {
                        trangThaiId = 1; //Đang hoạt động - Được phê duyệt
                        listEmployee.Add(item);
                    }
                    else if (item.Active == true && userInfor.Active == false)
                    {
                        trangThaiId = 2; //Đang hoạt động - Không được truy cập
                        listEmployee.Add(item);
                    }
                    else
                    {
                        trangThaiId = 3; //Ngừng hoạt động
                        item.SoNamLamViec = 0;
                    }
                    item.TrangThaiId = trangThaiId;

                });

                var employeeId = user.EmployeeId;
                var employeeLogin = listEmployee.FirstOrDefault(x => x.EmployeeId == employeeId);
                var isManager = employeeLogin.IsManager;

                if (isManager == true)
                {
                    //Lấy list phòng ban con của user
                    List<Guid?> listGetAllChild = new List<Guid?>();    //List phòng ban: chính nó và các phòng ban cấp dưới của nó
                    if (employeeLogin.OrganizationId != null)
                    {
                        listGetAllChild.Add(employeeLogin.OrganizationId.Value);
                        listGetAllChild = getOrganizationChildrenId(employeeLogin.OrganizationId.Value, listGetAllChild);
                    }
                    //Lấy danh sách nhân viên EmployyeeId mà user phụ trách
                    var listEmployeeInChargeByManager = listEmployee.Where(x => (listGetAllChild == null || listGetAllChild.Count == 0 || listGetAllChild.Contains(x.OrganizationId))).ToList();
                    List<Guid> listEmployeeInChargeByManagerId = new List<Guid>();

                    listEmployeeInChargeByManager.ForEach(item =>
                    {
                        if (item.EmployeeId != null && item.EmployeeId != Guid.Empty)
                            listEmployeeInChargeByManagerId.Add(item.EmployeeId.Value);
                    });

                    listEmployee = listEmployee.Where(x => listEmployeeInChargeByManagerId.Contains(x.EmployeeId.Value)).ToList();
                }
                else
                {
                    //Nếu không phải quản lý
                    listEmployee = listEmployee.Where(x => x.EmployeeId == employeeId).ToList();
                }

              
                #endregion

                #region Lấy danh sách Đơn vị

                var donViId = listAllCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "DVTS")?.CategoryTypeId;
                ListDonVi = listAllCategory
                    .Where(x => x.Active == true && x.CategoryTypeId == donViId)
                    .Select(y => new CategoryEntityModel()
                    {
                        CategoryId = y.CategoryId,
                        CategoryCode = y.CategoryCode,
                        CategoryName = y.CategoryName
                    }).ToList();

                #endregion

                #region Lấy danh Tài sản chưa phân bổ
                var listTaiSanChuaPhanBo = new List<AssetEntityModel>();
                listTaiSanChuaPhanBo = context.TaiSan.Where(x => x.HienTrangTaiSan == 0).Select(y => new AssetEntityModel
                {
                    TaiSanId = y.TaiSanId,
                    MaTaiSan = y.MaTaiSan,
                    TenTaiSan = y.TenTaiSan,
                    TenTaiSanCode = y.MaTaiSan + " - " + y.TenTaiSan,
                    MaCode = y.MaCode,
                    PhanLoaiTaiSanId = y.PhanLoaiTaiSanId,
                }).OrderByDescending(z => z.CreatedDate).ThenByDescending(z => z.NgayVaoSo).ToList();

                #endregion

                return new GetMasterDataPhanBoTSFormResult()
                {
                    Status = true,
                    StatusCode = HttpStatusCode.OK,
                    ListLoaiTSPB = listLoaiTS,
                    ListMucDichSuDung = listMucDichSuDung,
                    ListEmployee = listEmployee,
                    ListDonVi = ListDonVi,
                    ListTaiSanChuaPhanBo = listTaiSanChuaPhanBo
                };
            }
            catch (Exception e)
            {
                return new GetMasterDataPhanBoTSFormResult()
                {
                    Message = e.Message,
                    StatusCode = HttpStatusCode.Forbidden,
                    Status = false
                };
            }
        }

        public CreateOrYeuCauCapPhatResult CreateOrYeuCauCapPhat(CreateOrYeuCauCapPhatParameter parameter)
        {

            var folder = context.Folder.FirstOrDefault(x => x.FolderType == parameter.FolderType);

            if (folder == null)
            {
                return new CreateOrYeuCauCapPhatResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = "Thư mục upload không tồn tại"
                };
            }

            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var user = context.User.FirstOrDefault(c => c.UserId == parameter.UserId);
                    if (user == null)
                    {
                        return new CreateOrYeuCauCapPhatResult
                        {
                            MessageCode = "Nhân viên không tồn tại trong hệ thống",
                            StatusCode = HttpStatusCode.ExpectationFailed,
                        };
                    }
                    var employee = context.Employee.FirstOrDefault(c => c.EmployeeId == user.EmployeeId);
                    if (employee == null)
                    {
                        return new CreateOrYeuCauCapPhatResult
                        {
                            MessageCode = "Nhân viên không tồn tại tong hệ thống",
                            StatusCode = HttpStatusCode.ExpectationFailed
                        };
                    }

                    // Tạo mới
                    if (parameter.YeuCauCapPhatTaiSan.YeuCauCapPhatTaiSanId == 0)
                    {
                        var yeuCauCapPhatTaiSan = new YeuCauCapPhatTaiSan
                        {
                            MaYeuCau = GenerateYeuCauCapPhatCode(),
                            NgayDeXuat = parameter.YeuCauCapPhatTaiSan.NgayDeXuat,
                            NguoiDeXuatId = parameter.YeuCauCapPhatTaiSan.NguoiDeXuatId,
                            TrangThai = 1,// 1 tạo mới
                            CreatedById = parameter.UserId,
                            CreatedDate = DateTime.Now,
                            UpdatedById = parameter.UserId,
                            UpdatedDate = DateTime.Now,
                            Active = true
                        };
                        context.YeuCauCapPhatTaiSan.Add(yeuCauCapPhatTaiSan);
                        context.SaveChanges();

                        if (parameter.ListFile?.Count > 0)
                        {
                            var isSave = true;
                            parameter.ListFile?.ForEach(item =>
                            {
                                if (folder == null)
                                {
                                    isSave = false;
                                }

                                var folderName = ConvertFolderUrl(folder.Url);
                                var webRootPath = hostingEnvironment.WebRootPath;
                                var newPath = Path.Combine(webRootPath, folderName);

                                if (!Directory.Exists(newPath))
                                {
                                    isSave = false;
                                }

                                if (isSave)
                                {
                                    var file = new FileInFolder()
                                    {
                                        Active = true,
                                        CreatedById = parameter.UserId,
                                        CreatedDate = DateTime.Now,
                                        UpdatedById = parameter.UserId,
                                        UpdatedDate = DateTime.Now,
                                        FileInFolderId = Guid.NewGuid(),
                                        FileName = $"{item.FileInFolder.FileName}_{Guid.NewGuid()}",
                                        FolderId = folder.FolderId,
                                        ObjectNumber = yeuCauCapPhatTaiSan.YeuCauCapPhatTaiSanId,
                                        ObjectType = item.FileInFolder.ObjectType,
                                        Size = item.FileInFolder.Size,
                                        FileExtension = item.FileSave.FileName.Substring(
                                            item.FileSave.FileName.LastIndexOf(".", StringComparison.Ordinal) + 1),
                                    };

                                    context.FileInFolder.Add(file);
                                }
                            });
                           
                            if (!isSave)
                            {

                                return new CreateOrYeuCauCapPhatResult()
                                {
                                    StatusCode = HttpStatusCode.ExpectationFailed,
                                    MessageCode = "Bạn phải cấu hình thư mục để lưu"
                                };
                            }
                            context.SaveChanges();
                        }

                        #region Chi tiết tài sản yêu cầu
                        if (parameter.ListYeuCauCapPhatTaiSanChiTiet?.Count != 0)
                        {
                            var listYeuCauChiTiet = new List<YeuCauCapPhatTaiSanChiTiet>();
                            parameter.ListYeuCauCapPhatTaiSanChiTiet?.ForEach(item =>
                            {
                                var yeuCauChiTiet = new YeuCauCapPhatTaiSanChiTiet
                                {
                                    TaiSanId = item.TaiSanId,
                                    YeuCauCapPhatTaiSanId = yeuCauCapPhatTaiSan.YeuCauCapPhatTaiSanId,
                                    LoaiTaiSanId = item.LoaiTaiSanId,
                                    MoTa = item.MoTa == null ? "" : item.MoTa,
                                    SoLuong = item.SoLuong == null ? 0 : item.SoLuong.Value,
                                    SoLuongPheDuyet = item.SoLuongPheDuyet == null ? 0 : item.SoLuongPheDuyet.Value,
                                    NhanVienYeuCauId = item.NhanVienYeuCauId,
                                    MucDichSuDungId = item.MucDichSuDungId,
                                    NgayBatDau = item.NgayBatDau,
                                    NgayKetThuc = item.NgayKetThuc,
                                    LyDo = item.LyDo == null ? "" : item.LyDo,
                                    TrangThai = 1,
                                    CreatedById = parameter.UserId,
                                    CreatedDate = DateTime.Now,
                                    UpdatedById = parameter.UserId,
                                    UpdatedDate = DateTime.Now
                                };
                                listYeuCauChiTiet.Add(yeuCauChiTiet);
                            });

                            context.YeuCauCapPhatTaiSanChiTiet.AddRange(listYeuCauChiTiet);
                            context.SaveChanges();
                        };
                        #endregion

                        transaction.Commit();
                        return new CreateOrYeuCauCapPhatResult
                        {
                            MessageCode = "Success",
                            StatusCode = HttpStatusCode.OK,
                            YeuCauCapPhatTaiSanId = yeuCauCapPhatTaiSan.YeuCauCapPhatTaiSanId
                        };
                    }
                    // Cập nhật
                    else
                    {
                        var oldYeuCau = context.YeuCauCapPhatTaiSan.FirstOrDefault(c => c.YeuCauCapPhatTaiSanId == parameter.YeuCauCapPhatTaiSan.YeuCauCapPhatTaiSanId);
                        if (oldYeuCau == null)
                        {
                            return new CreateOrYeuCauCapPhatResult
                            {
                                MessageCode = "Yêu cầu cấp phát không tồn tại trong hệ thống",
                                StatusCode = HttpStatusCode.ExpectationFailed,
                            };
                        }

                        oldYeuCau.NgayDeXuat = parameter.YeuCauCapPhatTaiSan.NgayDeXuat;
                        oldYeuCau.UpdatedById = parameter.UserId;
                        oldYeuCau.UpdatedDate = DateTime.Now;

                        context.YeuCauCapPhatTaiSan.Update(oldYeuCau);
                        context.SaveChanges();

                        if (parameter.ListFile?.Count > 0)
                        {
                            var isSave = true;
                            parameter.ListFile?.ForEach(item =>
                            {
                                if (folder == null)
                                {
                                    isSave = false;
                                }

                                var folderName = ConvertFolderUrl(folder.Url);
                                var webRootPath = hostingEnvironment.WebRootPath;
                                var newPath = Path.Combine(webRootPath, folderName);

                                if (!Directory.Exists(newPath))
                                {
                                    isSave = false;
                                }

                                if (isSave)
                                {
                                    var file = new FileInFolder()
                                    {
                                        Active = true,
                                        CreatedById = parameter.UserId,
                                        CreatedDate = DateTime.Now,
                                        UpdatedById = parameter.UserId,
                                        UpdatedDate = DateTime.Now,
                                        FileInFolderId = Guid.NewGuid(),
                                        FileName = $"{item.FileInFolder.FileName}_{Guid.NewGuid()}",
                                        FolderId = folder.FolderId,
                                        ObjectNumber = oldYeuCau.YeuCauCapPhatTaiSanId,
                                        ObjectType = item.FileInFolder.ObjectType,
                                        Size = item.FileInFolder.Size,
                                        FileExtension = item.FileSave.FileName.Substring(
                                            item.FileSave.FileName.LastIndexOf(".", StringComparison.Ordinal) + 1),
                                    };

                                    context.FileInFolder.Add(file);
                                }
                            });
                           
                            if (!isSave)
                            {
                                return new CreateOrYeuCauCapPhatResult()
                                {
                                    StatusCode = HttpStatusCode.ExpectationFailed,
                                    MessageCode = "Bạn phải cấu hình thư mục để lưu"
                                };
                            }
                            context.SaveChanges();
                        }

                        #region Chi tiết tài sản yêu cầu
                        if (parameter.ListYeuCauCapPhatTaiSanChiTiet?.Count != 0)
                        {
                            var lstIDChiTiet = parameter.ListYeuCauCapPhatTaiSanChiTiet.Select(x => x.YeuCauCapPhatTaiSanChiTietId).ToList();
                            var lstOldChiTiet = context.YeuCauCapPhatTaiSanChiTiet.Where(x => lstIDChiTiet.Contains(x.YeuCauCapPhatTaiSanChiTietId)).ToList();
                            lstOldChiTiet?.ForEach(chitiet =>
                            {
                                var soLuongPD = parameter.ListYeuCauCapPhatTaiSanChiTiet.FirstOrDefault(x => x.YeuCauCapPhatTaiSanChiTietId == chitiet.YeuCauCapPhatTaiSanChiTietId)?.SoLuongPheDuyet.Value;
                                chitiet.SoLuongPheDuyet = soLuongPD;
                            });

                            context.YeuCauCapPhatTaiSanChiTiet.UpdateRange(lstOldChiTiet);
                            context.SaveChanges();
                        };
                        #endregion
                    }
                    transaction.Commit();
                    return new CreateOrYeuCauCapPhatResult
                    {
                        MessageCode = "Success",
                        StatusCode = HttpStatusCode.OK
                    };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new CreateOrYeuCauCapPhatResult
                    {
                        StatusCode = HttpStatusCode.Forbidden
                    };
                }
            }
        }

        private string GenerateYeuCauCapPhatCode()
        {
            var strCode = string.Empty;
            var date = DateTime.Now.ToString("yyyy");
            var countYeuCauCapPhat = context.YeuCauCapPhatTaiSan.Count() == 0 ? 0 : context.YeuCauCapPhatTaiSan.Max(x => x.YeuCauCapPhatTaiSanId);
            strCode = $"YC" + date + "-" + (countYeuCauCapPhat + 1);
            return strCode;
        }

        public GetAllYeuCauCapPhatTSListResult GetAllYeuCauCapPhatTSList(GetAllYeuCauCapPhatTSListParameter parameter)
        {
            var user = context.User.FirstOrDefault(c => c.UserId == parameter.UserId);
            if (user == null)
            {
                return new GetAllYeuCauCapPhatTSListResult
                {
                    Message = "Nhân viên không tồn tại trong hệ thống",
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }
            var employee = context.Employee.FirstOrDefault(c => c.EmployeeId == user.EmployeeId);
            if (employee == null)
            {
                return new GetAllYeuCauCapPhatTSListResult
                {
                    Message = "Nhân viên không tồn tại trong hệ thống",
                    StatusCode = HttpStatusCode.ExpectationFailed
                };
            }

            var listEmployee = new List<EmployeeEntityModel>();
            #region Lấy danh sách nhân viên
            listEmployee = context.Employee.Where(x => x.Active == true).Select(y =>
                       new EmployeeEntityModel
                       {
                           EmployeeId = y.EmployeeId,
                           EmployeeCodeName = y.EmployeeCode + " - " + y.EmployeeName,
                       }).ToList();
            #endregion

            var listYeuCauCP = new List<YeuCauCapPhatTaiSanEntityModel>();
            var listAllYeuCauCP = context.YeuCauCapPhatTaiSan.Where(x => x.Active == true).Select(y => new YeuCauCapPhatTaiSanEntityModel
            {
                YeuCauCapPhatTaiSanId = y.YeuCauCapPhatTaiSanId,
                MaYeuCau = y.MaYeuCau,
                NgayDeXuat = y.NgayDeXuat,
                TrangThai = y.TrangThai,
                NguoiDeXuatId = y.NguoiDeXuatId,
                CreatedDate = y.CreatedDate,
            }).OrderByDescending(z => z.CreatedDate).ToList();

            #region Phân quyền dữ liệu theo quy trình phê duyệt

            var thanhVienPhongBan =
                  context.ThanhVienPhongBan.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId);

            var isAccess = context.Organization.FirstOrDefault(x => x.OrganizationId == employee.OrganizationId)?.IsAccess;
            //Nếu k được xem dữ liệu phòng ban khác
            if (isAccess != true)
            {
                //Nếu là trưởng bộ phận (IsManager = 1)
                if (thanhVienPhongBan.IsManager == 1)
            {
                //Lấy ra list đối tượng id mà người dùng phụ trách phê duyệt
                var listId = context.PhongBanPheDuyetDoiTuong
                    .Where(x => x.DoiTuongApDung == 20 &&
                                x.OrganizationId == thanhVienPhongBan.OrganizationId).Select(y => y.ObjectNumber)
                    .ToList();

                var listEmpIdCungOrg = listEmployee.Where(x => x.OrganizationId == employee.OrganizationId).Select(x => x.EmployeeId).ToList();
                listYeuCauCP = listAllYeuCauCP.Where(x =>
                     (parameter.MaYeuCau == null || parameter.MaYeuCau == "" || x.MaYeuCau.ToLower().Contains(parameter.MaYeuCau.ToLower())) &&
                     (parameter.ListEmployee == null || parameter.ListEmployee.Count() == 0 || parameter.ListEmployee.Contains(x.NguoiDeXuatId)) &&
                     (parameter.TrangThai == null || x.TrangThai == parameter.TrangThai) &&
                     (listId.Contains(x.YeuCauCapPhatTaiSanId) || //Cần phê duyệt
                        x.NguoiDeXuatId == employee.EmployeeId) || //Người đề xuất
                        (listEmpIdCungOrg.Contains(x.NguoiDeXuatId) && x.TrangThai != 1) //Cùng phòng ban và trạng thái kahsc mơis
                     ).ToList();
            }
            //Nếu là nhân viên thường (IsManager = 0)
            else
            {
                listYeuCauCP = listAllYeuCauCP.Where(x =>
                    (parameter.MaYeuCau == null || parameter.MaYeuCau == "" || x.MaYeuCau.ToLower().Contains(parameter.MaYeuCau.ToLower())) &&
                    (parameter.ListEmployee == null || parameter.ListEmployee.Count() == 0 || parameter.ListEmployee.Contains(x.NguoiDeXuatId)) &&
                    (parameter.TrangThai == null || x.TrangThai == parameter.TrangThai) &&
                    x.NguoiDeXuatId == employee.EmployeeId).ToList();
            }
            }
            else
            {
                listYeuCauCP = listAllYeuCauCP.Where(x =>
                      (parameter.MaYeuCau == null || parameter.MaYeuCau == "" || x.MaYeuCau.ToLower().Contains(parameter.MaYeuCau.ToLower())) &&
                      (parameter.ListEmployee == null || parameter.ListEmployee.Count() == 0 || parameter.ListEmployee.Contains(x.NguoiDeXuatId)) &&
                      (parameter.TrangThai == null || x.TrangThai == parameter.TrangThai) &&
                       (x.NguoiDeXuatId == user.EmployeeId || // Theo người tạo
                     (x.NguoiDeXuatId != user.EmployeeId && x.TrangThai != 1))  // cùng phòng ban khác trạng thái mới
                     ).ToList();
            }
            #endregion


            if (listYeuCauCP.Count() > 0)
            {
                var lstYeuCauCPId = listYeuCauCP.Select(x => x.YeuCauCapPhatTaiSanId).ToList();
                var lstPhongBan = context.Organization.Where(x => x.Active == true).ToList();

                var lstYeuCauCapPhatChiTiet = context.YeuCauCapPhatTaiSanChiTiet.Where(x => lstYeuCauCPId.Contains(x.YeuCauCapPhatTaiSanId)).ToList();
                listYeuCauCP.ForEach(p =>
                {
                    switch (p.TrangThai)
                    {
                        case 1:
                            p.TrangThaiString = "Mới";
                            p.BackgroundColorForStatus = "#8ec3f4";
                            break;
                        case 2:
                            p.TrangThaiString = "Chờ phê duyệt";
                            p.BackgroundColorForStatus = "#f29505";
                            break;
                        case 3:
                            p.TrangThaiString = "Đã duyệt";
                            p.BackgroundColorForStatus = "#05f235";
                            break;
                        case 4:
                            p.TrangThaiString = "Từ chối";
                            p.BackgroundColorForStatus = "#797979";
                            break;
                        case 5:
                            p.TrangThaiString = "Hoàn thành";
                            p.BackgroundColorForStatus = "#50f296";
                            break;
                    }
                    var emp = context.Employee.FirstOrDefault(x => x.EmployeeId == p.NguoiDeXuatId);
                    p.NguoiDeXuat = emp.EmployeeName;
                    p.PhongBan = lstPhongBan.FirstOrDefault(x => x.OrganizationId == emp.OrganizationId)?.OrganizationName;

                    p.SoLuong = lstYeuCauCapPhatChiTiet.Where(x => x.YeuCauCapPhatTaiSanId == p.YeuCauCapPhatTaiSanId).Sum(x => x.SoLuong);
                });
            }


            return new GetAllYeuCauCapPhatTSListResult()
            {
                StatusCode = HttpStatusCode.OK,
                Status = true,
                ListEmployee = listEmployee,
                ListYeuCauCapPhatTaiSan = listYeuCauCP
            };
        }
        public XoaYeuCauCapPhatResult XoaYeuCauCapPhat(XoaYeuCauCapPhatParameter parameter)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var yeuCau = context.YeuCauCapPhatTaiSan.FirstOrDefault(x => x.YeuCauCapPhatTaiSanId == parameter.YeuCauCapPhatTaiSanId);

                    if (yeuCau != null)
                    {
                        // Xóa chi tiết
                        var lstChiTiet = context.YeuCauCapPhatTaiSanChiTiet.Where(x => x.YeuCauCapPhatTaiSanId == parameter.YeuCauCapPhatTaiSanId).ToList();
                        if (lstChiTiet != null)
                        {
                            lstChiTiet.ForEach(chitiet =>
                            {
                                chitiet.Active = false;
                            });
                            context.YeuCauCapPhatTaiSanChiTiet.UpdateRange(lstChiTiet);
                        }

                        yeuCau.Active = false;
                        context.YeuCauCapPhatTaiSan.Update(yeuCau);
                        context.SaveChanges();
                        transaction.Commit();
                    }
                    else
                    {
                        return new XoaYeuCauCapPhatResult
                        {
                            StatusCode = HttpStatusCode.FailedDependency,
                            MessageCode = "Không tồn tại yêu cầu cấp phát!"
                        };
                    }

                    return new XoaYeuCauCapPhatResult
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = "Xóa yêu cầu cấp phát thành công"
                    };
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    return new XoaYeuCauCapPhatResult
                    {
                        MessageCode = e.Message,
                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    };
                }
            }
        }

        public GetDataYeuCauCapPhatDetailResult GetDataYeuCauCapPhatDetail(GetDataYeuCauCapPhatDetailParameter parameter)
        {
            try
            {
                int pageSize = 10;
                int pageIndex = 1;
                var listAllEmp = context.Employee.ToList();
                var yeuCauCapPhat = context.YeuCauCapPhatTaiSan.Where(x => x.YeuCauCapPhatTaiSanId == parameter.YeuCauCapPhatTaiSanId).Select(yeuCau => new YeuCauCapPhatTaiSanEntityModel
                {
                    YeuCauCapPhatTaiSanId = yeuCau.YeuCauCapPhatTaiSanId,
                    MaYeuCau = yeuCau.MaYeuCau,
                    NgayDeXuat = yeuCau.NgayDeXuat,
                    NguoiDeXuatId = yeuCau.NguoiDeXuatId,
                    TrangThai = yeuCau.TrangThai,
                    CreatedById = yeuCau.CreatedById
                }).FirstOrDefault();
                yeuCauCapPhat.NguoiDeXuat = listAllEmp.FirstOrDefault(x => x.EmployeeId == yeuCauCapPhat.NguoiDeXuatId)?.EmployeeName;

                if (yeuCauCapPhat == null)
                {
                    return new GetDataYeuCauCapPhatDetailResult()
                    {
                        Message = "Không tồn tại yêu cầu!",
                        StatusCode = HttpStatusCode.FailedDependency,
                        Status = false
                    };
                }
                var user = context.User.FirstOrDefault(c => c.UserId == parameter.UserId);
                var employee = listAllEmp.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

                var listEmployee = new List<EmployeeEntityModel>();

                #region Common data
                var listAllCategoryType = context.CategoryType.ToList();
                var listAllCategory = context.Category.ToList();
                #endregion

                #region Lấy danh sách nhân viên
                listEmployee = listAllEmp.Select(y =>
                           new EmployeeEntityModel
                           {
                               EmployeeId = y.EmployeeId,
                               OrganizationId = y.OrganizationId,
                               PositionId = y.PositionId,
                               EmployeeCode = y.EmployeeCode,
                               EmployeeName = y.EmployeeName,
                               EmployeeCodeName = y.EmployeeCode + " - " + y.EmployeeName,
                           }).ToList();

                var listPosition = context.Position.ToList();
                var listOrganization = context.Organization.ToList();

                listEmployee?.ForEach(item =>
                {
                    var phongBan = listOrganization.FirstOrDefault(x => x.OrganizationId == item.OrganizationId);
                    item.OrganizationName = phongBan?.OrganizationName;

                    var chucVu = listPosition.FirstOrDefault(x => x.PositionId == item.PositionId);
                    item.PositionName = chucVu?.PositionName;
                });
                #endregion

                #region Lấy dách file đinh kèm 
                var webRootPath = hostingEnvironment.WebRootPath + "\\";

                var objectType = "YCCAPPHAT";
                var folderCommon = context.Folder.ToList();
                var folder = folderCommon.FirstOrDefault(x => x.FolderType == objectType);

                var listFileResult = context.FileInFolder
                                .Where(x => x.ObjectNumber == parameter.YeuCauCapPhatTaiSanId && x.FolderId == folder.FolderId).Select(y =>
                                    new FileInFolderEntityModel
                                    {
                                        Size = y.Size,
                                        ObjectId = y.ObjectId,
                                        Active = y.Active,
                                        FileExtension = y.FileExtension,
                                        FileInFolderId = y.FileInFolderId,
                                        FileName = y.FileName,
                                        FolderId = y.FolderId,
                                        ObjectType = y.ObjectType,
                                        ObjectNumber = y.ObjectNumber,
                                        CreatedById = y.CreatedById,
                                        CreatedDate = y.CreatedDate,
                                        UpdatedById = y.UpdatedById,
                                        UpdatedDate = y.UpdatedDate
                                    }).OrderBy(z => z.CreatedDate).ToList();

                listFileResult.ForEach(x =>
                {
                    x.UploadByName = context.User.FirstOrDefault(u => u.UserId == x.CreatedById)?.UserName;
                    x.FileFullName = $"{x.FileName}.{x.FileExtension}";
                    var folderUrlTL = context.Folder.FirstOrDefault(item => item.FolderId == x.FolderId)?.Url;
                    x.FileUrl = Path.Combine(webRootPath, folderUrlTL, x.FileFullName);
                });
                #endregion

                #region Lấy danh sách Mục đích sử dụng
                var listMucDichSuDung = new List<CategoryEntityModel>();
                var donViId = listAllCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "MDSD")?.CategoryTypeId;
                listMucDichSuDung = listAllCategory
                    .Where(x => x.Active == true && x.CategoryTypeId == donViId)
                    .Select(y => new CategoryEntityModel()
                    {
                        CategoryId = y.CategoryId,
                        CategoryCode = y.CategoryCode,
                        CategoryName = y.CategoryName
                    }).ToList();

                #endregion

                #region Lấy danh sách Phân loại tài sản
                var listLoaiTS = new List<CategoryEntityModel>();
                var phanLoaiTSId = listAllCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "PLTS")?.CategoryTypeId;
                listLoaiTS = listAllCategory
                    .Where(x => x.Active == true && x.CategoryTypeId == phanLoaiTSId)
                    .Select(y => new CategoryEntityModel()
                    {
                        CategoryId = y.CategoryId,
                        CategoryCode = y.CategoryCode,
                        CategoryName = y.CategoryName
                    }).ToList();

                #endregion

                #region Chi tiết Yêu cầu cấp phát
                var lstChiTiepCapPhat = context.YeuCauCapPhatTaiSanChiTiet.Where(x => x.YeuCauCapPhatTaiSanId == parameter.YeuCauCapPhatTaiSanId)
                    .Select(chitiet => new YeuCauCapPhatTaiSanChiTietEntityModel
                    {
                        YeuCauCapPhatTaiSanChiTietId = chitiet.YeuCauCapPhatTaiSanChiTietId,
                        YeuCauCapPhatTaiSanId = chitiet.YeuCauCapPhatTaiSanId,
                        LoaiTaiSanId = chitiet.LoaiTaiSanId,
                        MoTa = chitiet.MoTa,
                        NhanVienYeuCauId = chitiet.NhanVienYeuCauId,
                        MucDichSuDungId = chitiet.MucDichSuDungId,
                        NgayBatDau = chitiet.NgayBatDau,
                        NgayKetThuc = chitiet.NgayKetThuc,
                        LyDo = chitiet.LyDo,
                        SoLuong = chitiet.SoLuong,
                        SoLuongPheDuyet = chitiet.SoLuongPheDuyet,
                        CreatedDate = chitiet.CreatedDate,
                        CreatedById = chitiet.CreatedById
                    }).ToList();
                List<YeuCauCapPhatTaiSanChiTietEntityModel> lstChild = new List<YeuCauCapPhatTaiSanChiTietEntityModel>();
                lstChiTiepCapPhat.ForEach(ct =>
                {
                    var nhanvien = listEmployee.FirstOrDefault(x => x.EmployeeId == ct.NhanVienYeuCauId);
                    if (nhanvien != null)
                    {
                        ct.MaNV = nhanvien.EmployeeCode;
                        ct.TenNhanVien = nhanvien.EmployeeName;
                        ct.PhongBan = nhanvien?.OrganizationName;
                        ct.ViTriLamViec = nhanvien?.PositionName;
                        ct.LoaiTaiSan = listLoaiTS.FirstOrDefault(x => x.CategoryId == ct.LoaiTaiSanId)?.CategoryName;
                        ct.MucDichSuDung = listMucDichSuDung.FirstOrDefault(x => x.CategoryId == ct.MucDichSuDungId).CategoryName;
                    }
                    // Lấy các tài sản đã cấp phát thuộc chi tiết yêu cầu cấp phát
                    var lstTaiSan = context.CapPhatTaiSan.Where(x => x.YeuCauCapPhatTaiSanChiTietId == ct.YeuCauCapPhatTaiSanChiTietId).ToList();
                    if (lstTaiSan.Count() > 0)
                    {
                        var lstTaiSanDaCapPhat = (from capphat in lstTaiSan
                                                  join taisan in context.TaiSan.Where(x => lstTaiSan.Select(a => a.TaiSanId).Contains(x.TaiSanId)).ToList() on capphat.TaiSanId equals taisan.TaiSanId
                                                  into cu
                                                  from ts in cu.DefaultIfEmpty()
                                                  select new YeuCauCapPhatTaiSanChiTietEntityModel
                                                  {
                                                      YeuCauCapPhatTaiSanChiTietId = ct.YeuCauCapPhatTaiSanChiTietId,
                                                      CapPhatTaiSanId = capphat.CapPhatTaiSanId,
                                                      LoaiTaiSanId = ts.PhanLoaiTaiSanId == null ? Guid.Empty : ts.PhanLoaiTaiSanId.Value,
                                                      NhanVienYeuCauId = capphat.NguoiSuDungId,
                                                      MucDichSuDungId = capphat.MucDichSuDungId,
                                                      NgayBatDau = capphat.NgayBatDau,
                                                      NgayKetThuc = capphat.NgayKetThuc,
                                                      SoLuong = 0,
                                                      SoLuongPheDuyet = 0,
                                                      CreatedDate = capphat.CreatedDate,
                                                      CreatedById = capphat.CreatedById,
                                                      ParentPartId = ct.YeuCauCapPhatTaiSanChiTietId,
                                                      MaTaiSan = ts.MaTaiSan,
                                                      TaiSanId = ts.TaiSanId
                                                  }).OrderBy(x => x.TaiSanId).ToList();
                        ct.TotalChild = lstTaiSanDaCapPhat.Count();
                        lstChild.AddRange(lstTaiSanDaCapPhat);
                    }
                });

                lstChiTiepCapPhat.AddRange(lstChild);
                #endregion

                yeuCauCapPhat.NguoiDeXuat = listEmployee.FirstOrDefault(a => a.EmployeeId == yeuCauCapPhat.NguoiDeXuatId).EmployeeName;
                switch (yeuCauCapPhat.TrangThai)
                {
                    case 1:
                        yeuCauCapPhat.TrangThaiString = "Mới";
                        break;
                    case 2:
                        yeuCauCapPhat.TrangThaiString = "Chờ phê duyệt";
                        break;
                    case 3:
                        yeuCauCapPhat.TrangThaiString = "Đã duyệt";
                        break;
                    case 4:
                        yeuCauCapPhat.TrangThaiString = "Từ chối";
                        break;
                    case 5:
                        yeuCauCapPhat.TrangThaiString = "Hoàn thành";
                        break;
                }
                yeuCauCapPhat.ListYeuCauCapPhatTaiSanChiTiet = new List<YeuCauCapPhatTaiSanChiTietEntityModel>();
                yeuCauCapPhat.ListYeuCauCapPhatTaiSanChiTiet = lstChiTiepCapPhat;

                #region Điều kiện hiển thị các button

                bool isShowGuiPheDuyet = false;
                bool isShowPheDuyet = false;
                bool isShowTuChoi = false;
                bool isShowHuyYeuCauPheDuyet = false;
                bool isShowLuu = false;
                bool isShowXoa = false;
                bool isShowHuy = false;
                bool isShowPhanBo = false;
                bool isShowDatVeMoi = false;
                bool isShowHoanThanh = false;

                if (yeuCauCapPhat.YeuCauCapPhatTaiSanId != 0)
                {
                    #region Điều kiện hiển thị các button theo quy trình 

                    //Trạng thái Mới
                    if (yeuCauCapPhat.TrangThai == 1)
                    {
                        if (yeuCauCapPhat.CreatedById == user.UserId)
                        {
                            isShowGuiPheDuyet = true;
                            isShowXoa = true;
                        }
                        else if (yeuCauCapPhat.NguoiDeXuatId == employee.EmployeeId)
                        {
                            isShowGuiPheDuyet = true;
                            isShowXoa = true;
                        }
                        else
                        {
                            var phongBanNguoiDangNhap =
                                context.ThanhVienPhongBan.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId);

                            var empCreate = context.User.FirstOrDefault(x => x.UserId == yeuCauCapPhat.CreatedById);
                            var phongBanNguoiTao =
                                context.ThanhVienPhongBan.FirstOrDefault(x => x.EmployeeId == empCreate.EmployeeId);

                            var phongBanNhanVienBanHang =
                                context.ThanhVienPhongBan.FirstOrDefault(x => x.EmployeeId == yeuCauCapPhat.NguoiDeXuatId);

                            //Trưởng bộ phận
                            if (phongBanNguoiDangNhap?.IsManager == 1)
                            {
                                if (phongBanNguoiDangNhap?.OrganizationId == phongBanNguoiTao?.OrganizationId ||
                                    phongBanNguoiDangNhap?.OrganizationId == phongBanNhanVienBanHang?.OrganizationId)
                                {
                                    isShowGuiPheDuyet = true;
                                    isShowXoa = true;
                                }
                            }
                        }
                    }

                    // Trạng thái Chờ phê duyệt
                    if (yeuCauCapPhat.TrangThai == 2)
                    {
                        var buocHienTai = context.CacBuocApDung.Where(x => x.ObjectNumber == yeuCauCapPhat.YeuCauCapPhatTaiSanId &&
                                                                           x.DoiTuongApDung == 20 &&
                                                                           x.TrangThai == 0)
                            .OrderByDescending(z => z.Stt)
                            .FirstOrDefault();

                        //Nếu là phê duyệt trưởng bộ phận
                        if (buocHienTai?.LoaiPheDuyet == 1)
                        {
                            var listDonViId_NguoiPhuTrach = context.ThanhVienPhongBan
                                .Where(x => x.EmployeeId == yeuCauCapPhat.NguoiDeXuatId)
                                .Select(y => y.OrganizationId).ToList();

                            var countPheDuyet = context.ThanhVienPhongBan.Count(x =>
                                x.EmployeeId == employee.EmployeeId &&
                                x.IsManager == 1 &&
                                listDonViId_NguoiPhuTrach.Contains(
                                    x.OrganizationId));

                            if (countPheDuyet > 0)
                            {
                                isShowPheDuyet = true;
                                isShowTuChoi = true;
                            }
                        }
                        //Nếu là phòng ban phê duyệt
                        else if (buocHienTai?.LoaiPheDuyet == 2)
                        {
                            //Lấy list Phòng ban đã phê duyệt ở bước hiện tại
                            var listDonViIdDaPheDuyet = context.PhongBanApDung
                                .Where(x => x.CacBuocApDungId == buocHienTai.Id &&
                                            x.CacBuocQuyTrinhId == buocHienTai.CacBuocQuyTrinhId)
                                .Select(y => y.OrganizationId).ToList();

                            //Lấy list Phòng ban chưa phê duyệt ở bước hiện tại
                            var listDonViId = context.PhongBanTrongCacBuocQuyTrinh
                                .Where(x => x.CacBuocQuyTrinhId == buocHienTai.CacBuocQuyTrinhId &&
                                            !listDonViIdDaPheDuyet.Contains(x.OrganizationId))
                                .Select(y => y.OrganizationId).ToList();

                            var countPheDuyet = context.ThanhVienPhongBan.Count(x =>
                                x.EmployeeId == employee.EmployeeId &&
                                x.IsManager == 1 &&
                                listDonViId.Contains(
                                    x.OrganizationId));

                            if (countPheDuyet > 0)
                            {
                                isShowPheDuyet = true;
                                isShowTuChoi = true;
                            }
                        }
                    }

                    // Trạng thái Chờ phê duyệt
                    if (yeuCauCapPhat.TrangThai == 2 && user.UserId == yeuCauCapPhat.NguoiGuiXacNhanId)
                    {
                        var count =
                            context.CacBuocApDung.Count(x => x.ObjectNumber == yeuCauCapPhat.YeuCauCapPhatTaiSanId &&
                                                             x.DoiTuongApDung == 20 && x.TrangThai == 1);

                        if (count == 0)
                        {
                            isShowHuyYeuCauPheDuyet = true;
                        }
                    }

                    // Khác Từ chối phê duyệt
                    if (yeuCauCapPhat.TrangThai != 4)
                    {
                        if (yeuCauCapPhat.CreatedById == user.UserId)
                        {
                            isShowLuu = true;
                        }
                        else if (yeuCauCapPhat.NguoiDeXuatId == employee.EmployeeId)
                        {
                            isShowLuu = true;
                        }
                        else
                        {
                            var phongBanNguoiDangNhap =
                                context.ThanhVienPhongBan.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId);

                            var empCreate = context.User.FirstOrDefault(x => x.UserId == yeuCauCapPhat.CreatedById);
                            var phongBanNguoiTao =
                                context.ThanhVienPhongBan.FirstOrDefault(x => x.EmployeeId == empCreate.EmployeeId);

                            var phongBanNhanVienBanHang =
                                context.ThanhVienPhongBan.FirstOrDefault(x => x.EmployeeId == yeuCauCapPhat.NguoiDeXuatId);

                            //Trưởng bộ phận
                            if (phongBanNguoiDangNhap?.IsManager == 1)
                            {
                                if (phongBanNguoiDangNhap?.OrganizationId == phongBanNguoiTao?.OrganizationId ||
                                    phongBanNguoiDangNhap?.OrganizationId == phongBanNhanVienBanHang?.OrganizationId)
                                {
                                    isShowLuu = true;
                                }
                            }
                        }
                    }

                    // Đã duyệt
                    if (yeuCauCapPhat.TrangThai == 3)
                    {
                        var phongBanNguoiDangNhap =
                            context.ThanhVienPhongBan.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId);

                        var empCreate = context.User.FirstOrDefault(x => x.UserId == yeuCauCapPhat.CreatedById);
                        var phongBanNguoiTao =
                            context.ThanhVienPhongBan.FirstOrDefault(x => x.EmployeeId == empCreate.EmployeeId);

                        var phongBanNhanVienBanHang =
                            context.ThanhVienPhongBan.FirstOrDefault(x => x.EmployeeId == yeuCauCapPhat.NguoiDeXuatId);

                        //Trưởng bộ phận
                        if (phongBanNguoiDangNhap?.IsManager == 1)
                        {
                            if (phongBanNguoiDangNhap?.OrganizationId == phongBanNguoiTao?.OrganizationId ||
                                phongBanNguoiDangNhap?.OrganizationId == phongBanNhanVienBanHang?.OrganizationId)
                            {
                                isShowHuy = true;
                            }
                            var quyTrinh = context.QuyTrinh.FirstOrDefault(x => x.DoiTuongApDung == 20 && x.HoatDong);

                            var cauHinhQuyTrinh = context.CauHinhQuyTrinh.FirstOrDefault(x => x.QuyTrinhId == quyTrinh.Id);
                            var listIdCacBuocQuyTrinh = context.CacBuocQuyTrinh
                                .Where(x => x.CauHinhQuyTrinhId == cauHinhQuyTrinh.Id).Select(y => y.Id).ToList();
                            var listIdPhongBanTrongCacBuocQuyTrinh = context.PhongBanTrongCacBuocQuyTrinh
                                .Where(x => listIdCacBuocQuyTrinh.Contains(x.CacBuocQuyTrinhId)).Select(y => y.OrganizationId)
                                .ToList();

                            if (listIdPhongBanTrongCacBuocQuyTrinh.Contains(phongBanNguoiDangNhap.OrganizationId))
                            {
                                isShowHuy = true;
                            }
                        }
                    }

                    // Đã duyệt => Khi Yêu cầu cấp phát hoàn thành quy trình phê duyệt => Check xem ai có quyền phân bổ tài sản
                    if (yeuCauCapPhat.TrangThai == 3)
                    {
                        var phongBanNguoiDangNhap =
                            context.ThanhVienPhongBan.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId);

                        var empCreate = context.User.FirstOrDefault(x => x.UserId == yeuCauCapPhat.CreatedById);
                        var phongBanNguoiTao =
                            context.ThanhVienPhongBan.FirstOrDefault(x => x.EmployeeId == empCreate.EmployeeId);

                        var phongBanNhanVienBanHang =
                            context.ThanhVienPhongBan.FirstOrDefault(x => x.EmployeeId == yeuCauCapPhat.NguoiDeXuatId);

                        //Trưởng bộ phận phòng ban người tạo hoặc Trưởng bộ phận phòng ban của nhân viên bán hàng
                        if (phongBanNguoiDangNhap?.IsManager == 1)
                        {
                            if (phongBanNguoiDangNhap?.OrganizationId == phongBanNguoiTao?.OrganizationId ||
                                phongBanNguoiDangNhap?.OrganizationId == phongBanNhanVienBanHang?.OrganizationId)
                            {
                                isShowPhanBo = true;
                            }
                        }
                    }

                    // Hoàn thành
                    if (yeuCauCapPhat.TrangThai == 5)
                    {
                        isShowHoanThanh = true;
                    }

                        #endregion
                    }

                #endregion

                if (yeuCauCapPhat.TrangThai == 4) // Từ chối
                {
                    isShowDatVeMoi = true;
                }

                return new GetDataYeuCauCapPhatDetailResult()
                {
                    Status = true,
                    StatusCode = HttpStatusCode.OK,
                    YeuCauCapPhat = yeuCauCapPhat,
                    ListFileInFolder = listFileResult,
                    IsShowGuiPheDuyet = isShowGuiPheDuyet,
                    IsShowPheDuyet = isShowPheDuyet,
                    IsShowTuChoi = isShowTuChoi,
                    IsShowLuu = isShowLuu,
                    IsShowXoa = isShowXoa,
                    IsShowHuy = isShowHuy,
                    IsShowDatVeMoi = isShowDatVeMoi,
                    IsShowHuyYeuCauPheDuyet = isShowHuyYeuCauPheDuyet,
                    IsShowPhanBo = isShowPhanBo,
                    IsShowHoanThanh = isShowHoanThanh
                };

            }
            catch (Exception e)
            {
                return new GetDataYeuCauCapPhatDetailResult()
                {
                    Message = e.Message,
                    StatusCode = HttpStatusCode.Forbidden,
                    Status = false
                };
            }
        }
        public DeleteChiTietYeuCauCapPhatResult DeleteChiTietYeuCauCapPhat(DeleteChiTietYeuCauCapPhatParameter parameter)
        {
            try
            {
                var chiTiet = context.YeuCauCapPhatTaiSanChiTiet.FirstOrDefault(x => x.YeuCauCapPhatTaiSanChiTietId == parameter.YeuCauCapPhatTaiSanChiTietId);

                if (chiTiet != null)
                {
                    context.YeuCauCapPhatTaiSanChiTiet.Remove(chiTiet);
                    context.SaveChanges();
                }
                else
                {
                    return new DeleteChiTietYeuCauCapPhatResult
                    {
                        StatusCode = HttpStatusCode.FailedDependency,
                        MessageCode = "Không tồn tại dữ liệu!"
                    };
                }

                return new DeleteChiTietYeuCauCapPhatResult
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Xóa dữ liệu thành công"
                };
            }
            catch (Exception e)
            {
                return new DeleteChiTietYeuCauCapPhatResult
                {
                    MessageCode = e.Message,
                    StatusCode = HttpStatusCode.ExpectationFailed,
                };
            }
        }

        public CreateOrUpdateChiTietYeuCauCapPhatResult CreateOrUpdateChiTietYeuCauCapPhat(CreateOrUpdateChiTietYeuCauCapPhatParameter parameter)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    // Tạo mới
                    if (parameter.YeuCauCapPhatTaiSanChiTiet.YeuCauCapPhatTaiSanChiTietId == 0)
                    {
                        var chiTiet = new YeuCauCapPhatTaiSanChiTiet
                        {
                            YeuCauCapPhatTaiSanId = parameter.YeuCauCapPhatTaiSanChiTiet.YeuCauCapPhatTaiSanId,
                            LoaiTaiSanId = parameter.YeuCauCapPhatTaiSanChiTiet.LoaiTaiSanId,
                            SoLuong = parameter.YeuCauCapPhatTaiSanChiTiet.SoLuong,
                            MoTa = parameter.YeuCauCapPhatTaiSanChiTiet.MoTa,
                            MucDichSuDungId = parameter.YeuCauCapPhatTaiSanChiTiet.MucDichSuDungId,
                            NhanVienYeuCauId = parameter.YeuCauCapPhatTaiSanChiTiet.NhanVienYeuCauId,
                            NgayBatDau = parameter.YeuCauCapPhatTaiSanChiTiet.NgayBatDau,
                            NgayKetThuc = parameter.YeuCauCapPhatTaiSanChiTiet.NgayKetThuc,
                            LyDo = parameter.YeuCauCapPhatTaiSanChiTiet.LyDo,
                            TrangThai = 1,//Mới
                            SoLuongPheDuyet = 0,
                            UpdatedById = parameter.UserId,
                            UpdatedDate = DateTime.Now,
                            CreatedById = parameter.UserId,
                            CreatedDate = DateTime.Now,
                        };
                        context.YeuCauCapPhatTaiSanChiTiet.Add(chiTiet);
                        context.SaveChanges();
                    }
                    else
                    {
                        var chiTiet = context.YeuCauCapPhatTaiSanChiTiet.FirstOrDefault(x => x.YeuCauCapPhatTaiSanChiTietId == parameter.YeuCauCapPhatTaiSanChiTiet.YeuCauCapPhatTaiSanChiTietId);

                        if (chiTiet == null)
                        {
                            return new CreateOrUpdateChiTietYeuCauCapPhatResult()
                            {
                                StatusCode = HttpStatusCode.ExpectationFailed,
                                MessageCode = "Không tồn tại dữ liệu"
                            };
                        }
                        chiTiet.LoaiTaiSanId = parameter.YeuCauCapPhatTaiSanChiTiet.LoaiTaiSanId;
                        chiTiet.SoLuong = parameter.YeuCauCapPhatTaiSanChiTiet.SoLuong;
                        chiTiet.MoTa = parameter.YeuCauCapPhatTaiSanChiTiet.MoTa;
                        chiTiet.MucDichSuDungId = parameter.YeuCauCapPhatTaiSanChiTiet.MucDichSuDungId;
                        chiTiet.NhanVienYeuCauId = parameter.YeuCauCapPhatTaiSanChiTiet.NhanVienYeuCauId;
                        chiTiet.NgayBatDau = parameter.YeuCauCapPhatTaiSanChiTiet.NgayBatDau;
                        chiTiet.NgayKetThuc = parameter.YeuCauCapPhatTaiSanChiTiet.NgayKetThuc;
                        chiTiet.LyDo = parameter.YeuCauCapPhatTaiSanChiTiet.LyDo;
                        chiTiet.TrangThai = 1;//Mới
                        chiTiet.SoLuongPheDuyet = 0;
                        chiTiet.UpdatedById = parameter.UserId;
                        chiTiet.UpdatedDate = DateTime.Now;

                        context.YeuCauCapPhatTaiSanChiTiet.Update(chiTiet);
                        context.SaveChanges();
                    }
                    transaction.Commit();

                    #region Common data
                    var listAllCategoryType = context.CategoryType.ToList();
                    var listAllCategory = context.Category.ToList();
                    #endregion

                    var listEmployee = new List<EmployeeEntityModel>();
                    #region Lấy danh sách nhân viên
                    listEmployee = context.Employee.Select(y =>
                               new EmployeeEntityModel
                               {
                                   EmployeeId = y.EmployeeId,
                                   OrganizationId = y.OrganizationId,
                                   PositionId = y.PositionId,
                                   EmployeeCode = y.EmployeeCode,
                                   EmployeeName = y.EmployeeName,
                                   EmployeeCodeName = y.EmployeeCode + " - " + y.EmployeeName,
                               }).ToList();

                    var listPosition = context.Position.ToList();
                    var listOrganization = context.Organization.ToList();

                    listEmployee?.ForEach(item =>
                    {
                        var phongBan = listOrganization.FirstOrDefault(x => x.OrganizationId == item.OrganizationId);
                        item.OrganizationName = phongBan?.OrganizationName;

                        var chucVu = listPosition.FirstOrDefault(x => x.PositionId == item.PositionId);
                        item.PositionName = chucVu?.PositionName;
                    });
                    #endregion

                    #region Lấy danh sách Mục đích sử dụng
                    var listMucDichSuDung = new List<CategoryEntityModel>();
                    var donViId = listAllCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "MDSD")?.CategoryTypeId;
                    listMucDichSuDung = listAllCategory
                        .Where(x => x.Active == true && x.CategoryTypeId == donViId)
                        .Select(y => new CategoryEntityModel()
                        {
                            CategoryId = y.CategoryId,
                            CategoryCode = y.CategoryCode,
                            CategoryName = y.CategoryName
                        }).ToList();

                    #endregion
                    #region Lấy danh sách Phân loại tài sản
                    var listLoaiTS = new List<CategoryEntityModel>();
                    var phanLoaiTSId = listAllCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "PLTS")?.CategoryTypeId;
                    listLoaiTS = listAllCategory
                        .Where(x => x.Active == true && x.CategoryTypeId == phanLoaiTSId)
                        .Select(y => new CategoryEntityModel()
                        {
                            CategoryId = y.CategoryId,
                            CategoryCode = y.CategoryCode,
                            CategoryName = y.CategoryName
                        }).ToList();

                    #endregion

                    #region Chi tiết Yêu cầu cấp phát
                    var lstChiTiepCapPhat = context.YeuCauCapPhatTaiSanChiTiet.Where(x => x.YeuCauCapPhatTaiSanId == parameter.YeuCauCapPhatTaiSanChiTiet.YeuCauCapPhatTaiSanId)
                        .Select(chitiet => new YeuCauCapPhatTaiSanChiTietEntityModel
                        {
                            YeuCauCapPhatTaiSanChiTietId = chitiet.YeuCauCapPhatTaiSanChiTietId,
                            YeuCauCapPhatTaiSanId = chitiet.YeuCauCapPhatTaiSanId,
                            LoaiTaiSanId = chitiet.LoaiTaiSanId,
                            MoTa = chitiet.MoTa,
                            NhanVienYeuCauId = chitiet.NhanVienYeuCauId,
                            MucDichSuDungId = chitiet.MucDichSuDungId,
                            NgayBatDau = chitiet.NgayBatDau,
                            NgayKetThuc = chitiet.NgayKetThuc,
                            LyDo = chitiet.LyDo,
                            SoLuong = chitiet.SoLuong,
                            CreatedDate = chitiet.CreatedDate,
                            CreatedById = chitiet.CreatedById
                        }).ToList();

                    lstChiTiepCapPhat.ForEach(ct =>
                    {
                        var nhanvien = listEmployee.FirstOrDefault(x => x.EmployeeId == ct.NhanVienYeuCauId);
                        if (nhanvien != null)
                        {
                            ct.MaNV = nhanvien.EmployeeCode;
                            ct.TenNhanVien = nhanvien.EmployeeName;
                            ct.PhongBan = nhanvien?.OrganizationName;
                            ct.ViTriLamViec = nhanvien?.PositionName;
                            ct.LoaiTaiSan = listLoaiTS.FirstOrDefault(x => x.CategoryId == ct.LoaiTaiSanId)?.CategoryName;
                            ct.MucDichSuDung = listMucDichSuDung.FirstOrDefault(x => x.CategoryId == ct.MucDichSuDungId).CategoryName;
                        }
                    });
                    #endregion
                    return new CreateOrUpdateChiTietYeuCauCapPhatResult
                    {
                        MessageCode = "Success",
                        StatusCode = HttpStatusCode.OK,
                        ListTaiSanYeuCau = lstChiTiepCapPhat
                    };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new CreateOrUpdateChiTietYeuCauCapPhatResult
                    {
                        StatusCode = HttpStatusCode.Forbidden
                    };
                }
            }
        }

        public DatVeMoiYeuCauCapPhatTSResult DatVeMoiYeuCauCapPhatTS(DatVeMoiYeuCauCapPhatTSParameter parameter)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var yeuCau = context.YeuCauCapPhatTaiSan.FirstOrDefault(x => x.YeuCauCapPhatTaiSanId == parameter.YeuCauCapPhatTaiSanId && x.Active == true);
                    if (yeuCau == null)
                    {
                        return new DatVeMoiYeuCauCapPhatTSResult
                        {
                            StatusCode = HttpStatusCode.BadRequest,
                            Message = "Yêu cầu cấp phát không tồn tại trong hệ thống",
                        };
                    }
                    yeuCau.TrangThai = 1; // tt mới
                    context.YeuCauCapPhatTaiSan.Update(yeuCau);
                    context.SaveChanges();

                    var lstYeuCauCapPhat = context.YeuCauCapPhatTaiSanChiTiet.Where(x => x.YeuCauCapPhatTaiSanId == yeuCau.YeuCauCapPhatTaiSanId && x.Active == true).ToList();
                    lstYeuCauCapPhat.ForEach(item =>
                    {
                        item.TrangThai = 1; // tt mới
                        item.Active = true;
                    });
                    context.YeuCauCapPhatTaiSanChiTiet.UpdateRange(lstYeuCauCapPhat);
                    context.SaveChanges();

                    //Xóa các bước áp dụng của phê duyệt để làm mới
                    var listBuocPheDuyet = context.CacBuocApDung.Where(x => x.ObjectNumber == yeuCau.YeuCauCapPhatTaiSanId).ToList();
                    context.CacBuocApDung.RemoveRange(listBuocPheDuyet);
                    context.SaveChanges();
                    transaction.Commit();
                    return new DatVeMoiYeuCauCapPhatTSResult
                    {
                        StatusCode = HttpStatusCode.OK,
                        Message = "Đặt về mới yêu cầu cấp phát thành công",
                    };
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    return new DatVeMoiYeuCauCapPhatTSResult()
                    {
                        StatusCode = HttpStatusCode.Forbidden,
                        Message = e.Message
                    };
                }
            }
        }

        public CapNhapTrangThaiYeuCauCapPhatResult CapNhapTrangThaiYeuCauCapPhat( CapNhapTrangThaiYeuCauCapPhatParameter parameter)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    var yeuCau = context.YeuCauCapPhatTaiSan.FirstOrDefault(x => x.YeuCauCapPhatTaiSanId == parameter.YeuCauCapPhatTaiSanId && x.Active == true);
                    if (yeuCau == null)
                    {
                        return new CapNhapTrangThaiYeuCauCapPhatResult
                        {
                            StatusCode = HttpStatusCode.BadRequest,
                            Message = "Yêu cầu cấp phát không tồn tại trong hệ thống",
                        };
                    }
                    yeuCau.TrangThai = parameter.Type; // 5: Trạng thái hoàn thành phê duyệt
                    context.YeuCauCapPhatTaiSan.Update(yeuCau);
                    context.SaveChanges();

                    transaction.Commit();
                    return new CapNhapTrangThaiYeuCauCapPhatResult
                    {
                        StatusCode = HttpStatusCode.OK,
                        Message = "Cập nhập thành công cấp phát tài sản",
                    };
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    return new CapNhapTrangThaiYeuCauCapPhatResult()
                    {
                        StatusCode = HttpStatusCode.Forbidden,
                        Message = e.Message
                    };
                }
            }
        }

        #endregion
     
        #region Báo cáo      
        public BaoCaoPhanBoResult BaoCaoPhanBo(BaoCaoPhanBoParameter parameter)
        {         
            try
            {
                var listLoaiTSPB = new List<CategoryEntityModel>();

                #region Common data
                var listAllCategoryType = context.CategoryType.ToList();
                var listAllCategory = context.Category.ToList();
                var listAllEmployee = context.Employee.Where(x => x.Active == true).ToList();
                var lstPosition = context.Position.ToList();
                var lstOrganization = context.Organization.ToList();
                #endregion

                #region Lấy danh sách Phân loại tài sản
                var phanLoaiTSId = listAllCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "PLTS")?.CategoryTypeId;
                listLoaiTSPB = listAllCategory
                    .Where(x => x.Active == true && x.CategoryTypeId == phanLoaiTSId)
                    .Select(y => new CategoryEntityModel()
                    {
                        CategoryId = y.CategoryId,
                        CategoryCode = y.CategoryCode,
                        CategoryName = y.CategoryName
                    }).ToList();

                #endregion
                // Danh sách tài sản đang được sử dụng
                var lstTaiSan = context.TaiSan.Where(x => x.HienTrangTaiSan == 1).Select(x => new BaoCaoPhanBoEntityModel
                {
                    TaiSanId = x.TaiSanId,
                    MaTaiSan = x.MaTaiSan,
                    TenTaiSan = x.TenTaiSan,
                    PhanLoaiTaiSanId = x.PhanLoaiTaiSanId,
                    HienTrangTaiSan = x.HienTrangTaiSan.Value,
                    NgayVaoSo = x.NgayVaoSo        ,
                    MoTa = x.MoTa
                }).ToList();

                if(lstTaiSan.Count > 0)
                {
                    var lstTaiSanId = lstTaiSan.Select(a => a.TaiSanId).ToList();
                    var lstAllCapPhat = context.CapPhatTaiSan.Where(x => lstTaiSanId.Contains(x.TaiSanId)).ToList();
                    lstTaiSan?.ForEach(item =>
                    {
                        switch (item.HienTrangTaiSan)
                        {
                            case 1:
                                item.HienTrangTaiSanString = "Đang sử dụng";
                                item.BackgroundColorForStatus = "#0F62FE";
                                break;
                            case 0:
                                item.HienTrangTaiSanString = "Không sử dụng";
                                item.BackgroundColorForStatus = "#FFC000";
                                break;
                        }
                        item.PhanLoaiTaiSan = listLoaiTSPB.FirstOrDefault(x => x.CategoryId == item.PhanLoaiTaiSanId)?.CategoryName;
                        
                        var capPhat = lstAllCapPhat.Where(x => x.TaiSanId == item.TaiSanId).OrderByDescending(x => x.CreatedDate).FirstOrDefault();               

                        var emp =  listAllEmployee.FirstOrDefault(x => x.EmployeeId == capPhat?.NguoiSuDungId);
                        if(emp != null)
                        {
                            item.MaNhanVien = listAllEmployee.FirstOrDefault(x => x.EmployeeId == capPhat.NguoiSuDungId).EmployeeCode;
                            item.TenNhanVien = listAllEmployee.FirstOrDefault(x => x.EmployeeId == capPhat.NguoiSuDungId).EmployeeName;
                            item.ViTriLamViec = lstPosition.FirstOrDefault(x => x.PositionId == emp.PositionId)?.PositionName;
                            item.PhongBan = lstOrganization.FirstOrDefault(x => x.OrganizationId == emp.OrganizationId)?.OrganizationName;
                            item.NguoiSuDungId = capPhat.NguoiSuDungId;
                            item.OrganizationId = emp.OrganizationId;
                        }
                      
                    });
                }

                 lstTaiSan = lstTaiSan.Where(x =>
                        (parameter.ListEmployeeId == null || parameter.ListEmployeeId.Count() == 0 || parameter.ListEmployeeId.Contains(x.NguoiSuDungId)) &&
                        (parameter.ListOrganizationId == null || parameter.ListOrganizationId.Count() == 0 || parameter.ListOrganizationId.Contains(x.OrganizationId)) &&
                        (parameter.ListPhanLoaiTaiSanId == null || parameter.ListPhanLoaiTaiSanId.Count() == 0 || parameter.ListPhanLoaiTaiSanId.Contains(x.PhanLoaiTaiSanId)) &&
                        (parameter.ListHienTrangTaiSan == null || parameter.ListHienTrangTaiSan.Count() == 0 || parameter.ListHienTrangTaiSan.Contains(x.HienTrangTaiSan))
                        ).OrderByDescending(x => x.TenTaiSan)
                        .ToList();

                return new BaoCaoPhanBoResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    Status = true,
                    ListTaiSanPhanBo = lstTaiSan,
                };
            }
            catch (Exception e)
            {
                return new BaoCaoPhanBoResult()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    Message = e.Message
                };
            }
        }
        public BaoCaoKhauHaoResult BaoCaoKhauHao(BaoCaoKhauHaoParameter parameter)
        {
            try
            {
                #region Lấy danh sách khấu hao tài sản
                var listAllCategoryType = context.CategoryType.ToList();
                var listAllCategory = context.Category.ToList();
                var phanLoaiTSId = listAllCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "PLTS")?.CategoryTypeId;
                var listPhanLoai = listAllCategory
                    .Where(x => x.Active == true && x.CategoryTypeId == phanLoaiTSId)
                    .Select(y => new CategoryEntityModel()
                    {
                        CategoryId = y.CategoryId,
                        CategoryCode = y.CategoryCode,
                        CategoryName = y.CategoryName
                    }).ToList();               
                #endregion
                var listAssetAll = (from ts in context.TaiSan
                                   join PhanLoai in listPhanLoai on ts.PhanLoaiTaiSanId equals PhanLoai.CategoryId
                                    into cu
                                    from x in cu.DefaultIfEmpty()
                                        //where
                                        //(parameter.ListEmployeeId == null || parameter.ListEmployeeId.Contains((Guid)e.EmployeeId)) &&
                                        // (parameter.ListOrganizationId == null || parameter.ListOrganizationId.Contains((Guid)e.OrganizationId)) &&
                                        //  (parameter.ListPhanLoaiTaiSanId == null || parameter.ListPhanLoaiTaiSanId.Contains((Guid)ts.PhanLoaiTaiSanId)) &&
                                        //  (parameter.ListHienTrangTaiSan == null || parameter.ListHienTrangTaiSan.Contains(ts.HienTrangTaiSan.Value))
                                    select new BaoCaoKhauHaoEntityModel
                                    {
                                        MaTaiSan = ts.MaTaiSan,
                                        TenTaiSan = ts.TenTaiSan,
                                        LoaiTaiSanStr = x.CategoryName,
                                        HienTrangTaiSanStr = ts.HienTrangTaiSan == 1 ? "Đang sử dụng" : "Không sử dụng",
                                        HienTrangTaiSan = (int)ts.HienTrangTaiSan,
                                        PhanLoaiTaiSanId = ts.PhanLoaiTaiSanId,
                                        PhuongPhapTinhKhauHao= (int) ts.PhuongPhapTinhKhauHao,
                                        NgayVaoSo = ts.NgayVaoSo,
                                        ThoiGianKhauHao = (int)ts.ThoiGianKhauHao,
                                        GiaTriNguyenGia = (decimal)ts.GiaTriNguyenGia,
                                        GiaTriTinhKhauHao = (decimal)ts.GiaTriTinhKhauHao,
                                        ThoiDiemBdtinhKhauHao=(DateTime) ts.ThoiDiemBdtinhKhauHao,
                                        //TiLeKhauHaoTheoThang =ts. ,
                                        //TiLeKhauHaoTheoNam = ,
                                        //GiaTriKhauHaoTheoThang = ,
                                        //GiaTriKhauHaoTheoNam = ,
                                        //ThoiGianKhauHaoDen = ,
                                        //GiaTriKhauHaoLuyKe = ,
                                        //GiaTriConLai = ,
                                 }).OrderBy(x => x.MaTaiSan).ToList();
                listAssetAll.ForEach(x =>
                     {
                        if (x.ThoiGianKhauHao >0 && x.ThoiDiemBdtinhKhauHao!= null)
                         {
                             DateTime dt = (DateTime) x.ThoiDiemBdtinhKhauHao; //                            
                             dt.AddMonths(x.ThoiGianKhauHao);
                             x.ThoiDiemKTKhauHao = dt;
                         }    
                       
                         x.TiLeKhauHaoTheoThang = (100 / x.ThoiGianKhauHao);
                         //Giá trị khấu hao theo tháng = (Giá trị tính khấu hao* tỉ lệ khấu hao theo tháng)/100
                         x.GiaTriKhauHaoTheoThang =  x.GiaTriTinhKhauHao * x.TiLeKhauHaoTheoThang / 100;

                         //Tỉ lệ khấu hao theo năm = (100 / (Thời gian khấu hao / 12))
                         x.TiLeKhauHaoTheoNam = 100 / x.ThoiGianKhauHao / 12;

                         //Giá trị khấu hao theo năm = (Giá trị tính khấu hao* tỉ lệ khấu hao theo năm)/100
                         x.GiaTriKhauHaoTheoNam =x.GiaTriTinhKhauHao * x.TiLeKhauHaoTheoNam / 100;
                          //Giá trị khấu hao lũy kế = Gía trị tính khấu hao  - [ giá trị khấu hao theo tháng * (tháng hiện tại - tháng bắt đầu tính khấu hao)]
                         x.GiaTriKhauHaoLuyKe = x.GiaTriTinhKhauHao - x.GiaTriKhauHaoTheoThang * (((DateTime)x.ThoiDiemBdtinhKhauHao - DateTime.Now).Days / 31);

                         //Giá trị còn lại = Giá trị tính khấu hao - Giá trị khấu hao lũy kế
                         x.GiaTriConLai =x.GiaTriTinhKhauHao - x.GiaTriKhauHaoLuyKe;                        
                     });
                var listAsset = listAssetAll.Where(x =>
                  (parameter.ListPhanLoaiTaiSanId == null || parameter.ListPhanLoaiTaiSanId.Count() == 0 || parameter.ListPhanLoaiTaiSanId.Contains((Guid)x.PhanLoaiTaiSanId)) &&
                  (parameter.ListHienTrangTaiSan == null || parameter.ListHienTrangTaiSan.Count() == 0 || parameter.ListHienTrangTaiSan.Contains(x.HienTrangTaiSan))&&
                  (parameter.ThoiGianKhauHaoDen == null  || parameter.ThoiGianKhauHaoDen> x.ThoiDiemKTKhauHao)).ToList();
                var companyConfigEntity = context.CompanyConfiguration.FirstOrDefault();
                var companyConfig = new CompanyConfigEntityModel();
                companyConfig.CompanyId = companyConfigEntity.CompanyId;
                companyConfig.CompanyName = companyConfigEntity.CompanyName;
                companyConfig.Email = companyConfigEntity.Email;
                companyConfig.Phone = companyConfigEntity.Phone;
                companyConfig.TaxCode = companyConfigEntity.TaxCode;
                companyConfig.CompanyAddress = companyConfigEntity.CompanyAddress;
                companyConfig.CompanyAddress = companyConfigEntity.CompanyAddress;

                return new BaoCaoKhauHaoResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    Status = true,
                    ListAsset = listAsset,
                    CompanyConfig = companyConfig,
                    ListPhanLoaiTaiSan = listPhanLoai
                    
                };
            }
            catch (Exception e)
            {
                return new BaoCaoKhauHaoResult()
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    Message = e.Message
                };
            }
        }

        public BaoCaoPhanBoResult GetMasterDataBaoCaoPhanBo(BaoCaoPhanBoParameter parameter)
        {
            var listLoaiTSPB = new List<CategoryEntityModel>();
            var listEmployee = new List<EmployeeEntityModel>();
            var listOrganization = new List<OrganizationEntityModel>();
            #region Common data
            var listAllCategoryType = context.CategoryType.ToList();
            var listAllCategory = context.Category.ToList();
            #endregion

            #region Lấy danh sách Phân loại tài sản
            var phanLoaiTSId = listAllCategoryType.FirstOrDefault(x => x.CategoryTypeCode == "PLTS")?.CategoryTypeId;
            listLoaiTSPB = listAllCategory
                .Where(x => x.Active == true && x.CategoryTypeId == phanLoaiTSId)
                .Select(y => new CategoryEntityModel()
                {
                    CategoryId = y.CategoryId,
                    CategoryCode = y.CategoryCode,
                    CategoryName = y.CategoryName
                }).ToList();

            #endregion

            #region Danh sách phòng ban
            listOrganization = context.Organization.Where(x => x.Active == true).Select(y => new OrganizationEntityModel
            {
                OrganizationName = y.OrganizationName,
                OrganizationId = y.OrganizationId
            }).ToList();
            #endregion

            #region Lấy danh sách nhân viên
            listEmployee = context.Employee.Where(x => x.Active == true).Select(y =>
                       new EmployeeEntityModel
                       {
                           EmployeeId = y.EmployeeId,
                           EmployeeCode = y.EmployeeCode,
                           EmployeeName = y.EmployeeName,
                           EmployeeCodeName = y.EmployeeCode + " - " + y.EmployeeName,
                           OrganizationId = y.OrganizationId,
                           PositionId = y.PositionId
                       }).ToList();

            var listPosition = context.Position.ToList();

            listEmployee?.ForEach(item =>
            {
                var phongBan = listOrganization.FirstOrDefault(x => x.OrganizationId == item.OrganizationId);
                item.OrganizationName = phongBan?.OrganizationName;

                var chucVu = listPosition.FirstOrDefault(x => x.PositionId == item.PositionId);
                item.PositionName = chucVu?.PositionName;
            });
            #endregion

            var companyConfigEntity = context.CompanyConfiguration.FirstOrDefault();
            var companyConfig = new CompanyConfigEntityModel();
            companyConfig.CompanyId = companyConfigEntity.CompanyId;
            companyConfig.CompanyName = companyConfigEntity.CompanyName;
            companyConfig.Email = companyConfigEntity.Email;
            companyConfig.Phone = companyConfigEntity.Phone;
            companyConfig.TaxCode = companyConfigEntity.TaxCode;
            companyConfig.CompanyAddress = companyConfigEntity.CompanyAddress;
            companyConfig.CompanyAddress = companyConfigEntity.CompanyAddress;
            return new BaoCaoPhanBoResult()
            {
                StatusCode = HttpStatusCode.OK,
                Status = true,
                CompanyConfig = companyConfig,
                ListPhanLoaiTaiSan = listLoaiTSPB,
                ListOrganization = listOrganization,
                ListEmployee = listEmployee
            };
        }


        #endregion

        public ImportAssetResult ImportAsset(ImportAssetParameter parameter)
        {
            using (var trans = context.Database.BeginTransaction())
            {
                try
                {
                    var listAllTaiSan = context.TaiSan.ToList();
                    var listFail = new List<TaiSan>();
                    parameter.ListTaiSan.ForEach(item =>
                    {
                        var listAssetCode = listAllTaiSan.Where(a => a.TaiSanId != item.TaiSanId).Select(x => x.MaTaiSan).ToList();
                        var duplicateCode = false;
                        if (listAssetCode.Count > 0)
                        {
                            listAssetCode.ForEach(x =>
                            {
                                if (x.Equals(item.MaTaiSan))
                                {
                                    duplicateCode = true;
                                }
                            });
                        }
                        if (duplicateCode)
                        {
                            listFail.Add(item);
                        }
                        // Thêm mới tài sản
                        if (item.TaiSanId == 0)
                        {
                            item.CreatedById = parameter.UserId;
                            item.CreatedDate = DateTime.Now;
                            item.UpdatedById = parameter.UserId;
                            item.UpdatedDate = DateTime.Now;
                            context.TaiSan.Add(item);
                        }
                    });
                    if (listFail.Count > 0)
                    {
                            trans.Rollback();
                            return new ImportAssetResult()
                            {
                                Status = true,
                                ListFail = listFail,
                                StatusCode = HttpStatusCode.ExpectationFailed,
                                Message = "Danh sách tài sản nhập lỗi do trùng mã tài sản!",
                            };
                    }
                    context.SaveChanges();
                        trans.Commit();

                        return new ImportAssetResult()
                    {
                        Status = true,
                        StatusCode = HttpStatusCode.OK,
                        Message = "Nhập danh sách tài sản thành công!",
                    };
                }
                catch (Exception e)
                {
                    trans.Rollback();
                    return new ImportAssetResult()
                    {
                        Message = e.Message,
                        StatusCode = HttpStatusCode.Forbidden,
                        Status = false,
                    };
                }
            }
        }
        private List<Guid?> getOrganizationChildrenId(Guid? id, List<Guid?> list)
        {
            var Organization = context.Organization.Where(o => o.ParentId == id).ToList();
            Organization.ForEach(item =>
            {
                list.Add(item.OrganizationId);
                getOrganizationChildrenId(item.OrganizationId, list);
            });

            return list;
        }


        public DownloadTemplateImportResult DownloadTemplateImportAsset(DownloadTemplateImportParameter parameter)
        {
            try
            {
                string rootFolder = hostingEnvironment.WebRootPath + "\\ExcelTemplate";
                string fileName = @"Template_Import_Nhap_tai_san.xlsx";

                string newFilePath = Path.Combine(rootFolder, fileName);
                byte[] data = File.ReadAllBytes(newFilePath);

                return new DownloadTemplateImportResult
                {
                    TemplateExcel = data,
                    Message = string.Format("Đã dowload file Template_Import_Nhap_tai_san"),
                    FileName = "Template_Import_Nhap_tai_san",
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new DownloadTemplateImportResult
                {
                    Message = "Đã có lỗi xảy ra trong quá trình download",
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }


        public DotKiemKeSearchResult DotKiemKeSearch(DotKiemKeSearchParameter parameter)
        {
            try
            {
                var ngayHienTai = DateTime.Now.Date;
                var listTrangThaiKiemKe = GeneralList.GetTrangThais("DotKiemKe").ToList();
                var listDotKiemKeCheck = context.DotKiemKe.ToList();
                listDotKiemKeCheck.ForEach(item =>
                {
                    if (item.NgayBatDau.Date > ngayHienTai)// Chưa bắt đầu
                    {
                        item.TrangThaiId = listTrangThaiKiemKe.FirstOrDefault(x => x.Value == 1).Value;
                    }

                    if (item.NgayBatDau.Date <= ngayHienTai && ngayHienTai <= item.NgayBatDau.Date)// Đang thưc hiện
                    {
                        item.TrangThaiId = listTrangThaiKiemKe.FirstOrDefault(x => x.Value == 2).Value;
                    }

                    if (ngayHienTai > item.NgayKetThuc.Date)// Hoàn thành
                    {
                        item.TrangThaiId = listTrangThaiKiemKe.FirstOrDefault(x => x.Value == 3).Value;
                    }

                });
                context.DotKiemKe.UpdateRange(listDotKiemKeCheck);
                context.SaveChanges();

                var listAllChiTietDotKiemKe = context.DotKiemKeChiTiet.ToList();
                var listDotKiemKe = context.DotKiemKe
                    .Where(x =>
                            (parameter.TenDotKiemKe == null || parameter.TenDotKiemKe.Trim() == "" || x.TenDoiKiemKe.Contains(parameter.TenDotKiemKe.Trim()))
                            && (parameter.NgayBatDau == null || x.NgayBatDau.Date >= parameter.NgayBatDau.Value.Date)
                            && (parameter.NgayKetThuc == null || x.NgayKetThuc.Date <= parameter.NgayKetThuc.Value.Date)
                            && (parameter.ListTrangThai == null || parameter.ListTrangThai.Count == 0 || parameter.ListTrangThai.Contains(x.TrangThaiId))
                    )
                    .Select(x => new DotKiemKeEntityModel
                {
                    DotKiemKeId = x.DotKiemKeId,
                    TenDoiKiemKe = x.TenDoiKiemKe,
                    SoLuongTaiSan = listAllChiTietDotKiemKe.Where(y => y.DotKiemKeId == x.DotKiemKeId).Count(),
                    NgayBatDau = x.NgayBatDau,
                    NgayKetThuc = x.NgayKetThuc,
                    TrangThaiId = x.TrangThaiId,
                    CreatedById = x.CreatedById,
                }).OrderByDescending(x => x.NgayBatDau).ToList();

                return new DotKiemKeSearchResult
                {
                    ListDotKiemKe = listDotKiemKe,
                    ListTrangThaiKiemKe = listTrangThaiKiemKe,
                    Message = "",
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new DotKiemKeSearchResult
                {
                    Message = ex.Message,
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }

        public TaoDotKiemKeResult TaoDotKiemKe(TaoDotKiemKeParameter parameter)
        {
            try
            {
                var ngayHienTai = DateTime.Now.Date;
                var listTrangThaiKiemKe = GeneralList.GetTrangThais("DotKiemKe").ToList();
                var message = "";
                if (parameter.DotKiemKeId == null)
                {
                    message = "Tạo mới đợt kiểm kê thành công!";
                    var newDotKiemKe = new DotKiemKe();
                    newDotKiemKe.TenDoiKiemKe = parameter.TenDotKiemKe;
                    newDotKiemKe.NgayBatDau = parameter.NgayBatDau;
                    newDotKiemKe.NgayKetThuc = parameter.NgayKetThuc;
                    newDotKiemKe.CreatedById = parameter.UserId;
                    newDotKiemKe.CreatedDate = DateTime.Now;

                    if (parameter.NgayBatDau.Date > ngayHienTai)// Chưa bắt đầu
                    {
                        newDotKiemKe.TrangThaiId = listTrangThaiKiemKe.FirstOrDefault(x => x.Value == 1).Value;
                    }

                    if (parameter.NgayBatDau.Date <= ngayHienTai && ngayHienTai <= parameter.NgayBatDau.Date)// Đang thưc hiện
                    {
                        newDotKiemKe.TrangThaiId = listTrangThaiKiemKe.FirstOrDefault(x => x.Value == 2).Value;
                    }

                    if (ngayHienTai > parameter.NgayKetThuc.Date)// Hoàn thành
                    {
                        newDotKiemKe.TrangThaiId = listTrangThaiKiemKe.FirstOrDefault(x => x.Value == 3).Value;
                    }
                    context.DotKiemKe.Add(newDotKiemKe);
                    context.SaveChanges();
                }else if(parameter.DotKiemKeId != 0)
                {
                    message = "Cập nhật đợt kiểm kê thành công!";
                    var dotKiemKe = context.DotKiemKe.FirstOrDefault(x => x.DotKiemKeId == parameter.DotKiemKeId);
                    if (dotKiemKe == null)
                    {
                        return new TaoDotKiemKeResult
                        {
                            Message = "Đợt kiểm kê không tồn tại trên hệ thống!",
                            StatusCode = HttpStatusCode.OK
                        };
                    }
                    dotKiemKe.TenDoiKiemKe = parameter.TenDotKiemKe;
                    dotKiemKe.NgayBatDau = parameter.NgayBatDau;
                    dotKiemKe.NgayKetThuc = parameter.NgayKetThuc;

                    if (parameter.NgayBatDau.Date > ngayHienTai)// Chưa bắt đầu
                    {
                        dotKiemKe.TrangThaiId = listTrangThaiKiemKe.FirstOrDefault(x => x.Value == 1).Value;
                    }

                    if (parameter.NgayBatDau.Date <= ngayHienTai && ngayHienTai <= parameter.NgayBatDau.Date)// Đang thưc hiện
                    {
                        dotKiemKe.TrangThaiId = listTrangThaiKiemKe.FirstOrDefault(x => x.Value == 2).Value;
                    }

                    if (ngayHienTai > parameter.NgayKetThuc.Date)// Hoàn thành
                    {
                        dotKiemKe.TrangThaiId = listTrangThaiKiemKe.FirstOrDefault(x => x.Value == 3).Value;
                    }
                    context.DotKiemKe.Update(dotKiemKe);
                    context.SaveChanges();
                }
                return new TaoDotKiemKeResult
                {
                    Message = message,
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new TaoDotKiemKeResult
                {
                    Message = ex.Message,
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }

        public DeleteDotKiemKeResult DeleteDotKiemKe(DeleteDotKiemKeParameter parameter)
        {
            try
            {
                var dotKiemKe = context.DotKiemKe.FirstOrDefault(x => x.DotKiemKeId == parameter.DotKiemKeId);
                if (dotKiemKe == null)
                {
                    return new DeleteDotKiemKeResult
                    {
                        Message = "Đợt kiểm kê không tồn tại trên hệ thống!",
                        StatusCode = HttpStatusCode.OK
                    };
                }
                context.DotKiemKe.Remove(dotKiemKe);
                context.SaveChanges();
                return new DeleteDotKiemKeResult
                {
                    Message = "Xóa đợt kiểm kê thành công!",
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new DeleteDotKiemKeResult
                {
                    Message = ex.Message,
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }


        public DotKiemKeDetailResult DotKiemKeDetail(DotKiemKeDetailParameter parameter)
        {
            try
            {
                var dotKiemKe = context.DotKiemKe.FirstOrDefault(x => x.DotKiemKeId == parameter.DotKiemKeId);
                if(dotKiemKe == null)
                {
                    return new DotKiemKeDetailResult
                    {
                        Message = "Đợt kiểm kê không tồn tại trên hệ thống!",
                        StatusCode = HttpStatusCode.Forbidden
                    };
                }
                var ngayHienTai = DateTime.Now.Date;
                var listTrangThaiKiemKe = GeneralList.GetTrangThais("DotKiemKe").ToList();
                if (dotKiemKe.NgayBatDau.Date > ngayHienTai)// Chưa bắt đầu
                {
                    dotKiemKe.TrangThaiId = listTrangThaiKiemKe.FirstOrDefault(x => x.Value == 1).Value;
                }
                if (dotKiemKe.NgayBatDau.Date <= ngayHienTai && ngayHienTai <= dotKiemKe.NgayBatDau.Date)// Đang thưc hiện
                {
                    dotKiemKe.TrangThaiId = listTrangThaiKiemKe.FirstOrDefault(x => x.Value == 2).Value;
                }
                if (ngayHienTai > dotKiemKe.NgayKetThuc.Date)// Hoàn thành
                {
                    dotKiemKe.TrangThaiId = listTrangThaiKiemKe.FirstOrDefault(x => x.Value == 3).Value;
                }
                context.DotKiemKe.Update(dotKiemKe);
                context.SaveChanges();

                var listAllTaiSan = context.TaiSan.ToList();
                var listProvince = context.Province.Where(x => x.IsShowAsset == true).Select(x => new ProvinceEntityModel()
                {
                    ProvinceId = x.ProvinceId,
                    ProvinceName = x.ProvinceName,
                    ProvinceCode = x.ProvinceCode,
                }).ToList();

                var listDotKiemKeChiTiet = context.DotKiemKeChiTiet.Where(x => x.DotKiemKeId == parameter.DotKiemKeId).Select(x => new DotKiemKeChiTietEntityModel {
                    DotKiemKeChiTietId = x.DotKiemKeChiTietId,
                    DotKiemKeId = x.DotKiemKeId,
                    TaiSanId = x.TaiSanId,
                    NguoiKiemKeId = x.NguoiKiemKeId,
                    CreatedDate = x.CreatedDate
                }).OrderByDescending(x => x.CreatedDate).ToList();

                var listNguoiKiemKeId = listDotKiemKeChiTiet.Select(x => x.NguoiKiemKeId).Distinct().ToList();
                var listAllNguoiKiemKe = context.Employee.Where(x => listNguoiKiemKeId.Contains(x.EmployeeId)).Select(x => new EmployeeEntityModel()
                {
                    EmployeeId = x.EmployeeId,
                    EmployeeName = x.EmployeeName,
                    EmployeeCode = x.EmployeeCode
                }).ToList();

                var hienTai = DateTime.Now;
                listDotKiemKeChiTiet.ForEach(item =>
                {
                    var taiSan = listAllTaiSan.FirstOrDefault(x => x.TaiSanId == item.TaiSanId);
                    if(taiSan != null)
                    {
                        item.NguoiKiemKeName = listAllNguoiKiemKe.FirstOrDefault(x => x.EmployeeId == item.NguoiKiemKeId)?.EmployeeName;
                        item.MaTaiSan = taiSan.MaTaiSan;
                        item.TenTaiSan = taiSan.TenTaiSan;

                        item.KhuVucName = listProvince.FirstOrDefault(x => x.ProvinceId == taiSan.KhuVucTaiSanId)?.ProvinceName;

                        item.TinhTrangName = taiSan.HienTrangTaiSan == 0 ? "Không sử dụng" : "Đang sử dụng";

                        item.GiaTriTinhKhauHao = taiSan.GiaTriTinhKhauHao;


                        var tiLeKhauHaoTheoThang = 100 / taiSan.ThoiGianKhauHao;
                        var giaTriKhauHaoTheoThang = taiSan.GiaTriTinhKhauHao * tiLeKhauHaoTheoThang / 100;

                        if (taiSan.ThoiDiemBdtinhKhauHao != null)
                        {
                            item.KhauHaoLuyKe = giaTriKhauHaoTheoThang * GetMonthDifference(hienTai, taiSan.ThoiDiemBdtinhKhauHao.Value); // sau fixx công thức thì sửa
                            item.GiaTriConLai = taiSan.GiaTriTinhKhauHao - item.KhauHaoLuyKe; // sau fixx công thức thì sửa
                        }

                        item.MoTaTaiSan = taiSan.MoTa;

                        item.ProvincecId = taiSan.KhuVucTaiSanId;
                        item.PhanLoaiTaiSanId = taiSan.PhanLoaiTaiSanId;
                        item.HienTrangTaiSan = taiSan.HienTrangTaiSan;
                    }
                });


                // Bộ lọc
                listDotKiemKeChiTiet = listDotKiemKeChiTiet.Where(x =>
                                   (parameter.ProvincecId == null || parameter.ProvincecId.Count == 0 || parameter.ProvincecId.Contains(x.ProvincecId.Value))
                                && (parameter.PhanLoaiTaiSanId == null || parameter.PhanLoaiTaiSanId.Count == 0  || parameter.PhanLoaiTaiSanId.Contains(x.PhanLoaiTaiSanId.Value))
                                && (parameter.HienTrangTaiSan == null || x.HienTrangTaiSan == parameter.HienTrangTaiSan)
                                && (parameter.NguoiKiemKeId == null || parameter.NguoiKiemKeId.Contains(x.NguoiKiemKeId.Value))
                                && (parameter.NgayKiemKe == null || x.CreatedDate.Value.Date == parameter.NgayKiemKe.Value.Date)
                            ).ToList();

 
                var dotKiemKeReturn = new DotKiemKeEntityModel();
                dotKiemKeReturn.DotKiemKeId = dotKiemKe.DotKiemKeId;
                dotKiemKeReturn.TenDoiKiemKe = dotKiemKe.TenDoiKiemKe;
                dotKiemKeReturn.SoLuongTaiSan = listDotKiemKeChiTiet.Count();
                dotKiemKeReturn.NgayBatDau = dotKiemKe.NgayBatDau;
                dotKiemKeReturn.NgayKetThuc = dotKiemKe.NgayKetThuc;
                dotKiemKeReturn.TrangThaiId = dotKiemKe.TrangThaiId;
                dotKiemKeReturn.CreatedById = dotKiemKe.CreatedById;


                //Phân loại tài sản
                var phanLoaiTaiSanTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "PLTS")?.CategoryTypeId;
                var listPhanLoaiTaiSan = context.Category.Where(x => x.CategoryTypeId == phanLoaiTaiSanTypeId).Select(x => new CategoryEntityModel
                {
                    CategoryId = x.CategoryId,
                    CategoryCode = x.CategoryCode,
                    CategoryName = x.CategoryName
                }).ToList();

                //Hiện trạng tài sản
                var listHienTrangTaiSan = GeneralList.GetTrangThais("TinhTrangTaiSan").ToList();

                return new DotKiemKeDetailResult
                {
                    DotKiemKe = dotKiemKeReturn,
                    ListTrangThaiKiemKe = listTrangThaiKiemKe,
                    ListDotKiemKeChiTiet = listDotKiemKeChiTiet,
                    ListPhanLoaiTaiSan = listPhanLoaiTaiSan,
                    ListHienTrangTaiSan = listHienTrangTaiSan,
                    ListAllNguoiKiemKe = listAllNguoiKiemKe,
                    ListAllProvince = listProvince,
                    Message = "",
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new DotKiemKeDetailResult
                {
                    Message = ex.Message,
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }

        public static int GetMonthDifference(DateTime startDate, DateTime endDate)
        {
            int monthsApart = 12 * (startDate.Year - endDate.Year) + startDate.Month - endDate.Month;
            return Math.Abs(monthsApart);
        }

        public UpdateKhauHaoMobileResult UpdateKhauHaoMobile(UpdateKhauHaoMobileParameter parameter)
        {
            try
            {
                var taiSan = context.TaiSan.FirstOrDefault(x => x.TaiSanId == parameter.TaiSanId);
                if(taiSan == null)
                {
                    return new UpdateKhauHaoMobileResult
                    {
                        Message = "Tài sản không tồn tại trên hệ thống!",
                        StatusCode = HttpStatusCode.Forbidden
                    };
                }
                taiSan.GiaTriNguyenGia = parameter.GiaTriNguyenGia != null ? parameter.GiaTriNguyenGia : taiSan.GiaTriNguyenGia;
                taiSan.GiaTriTinhKhauHao = parameter.GiaTriTinhKhauHao != null ? parameter.GiaTriTinhKhauHao : taiSan.GiaTriTinhKhauHao;
                taiSan.ThoiGianKhauHao = parameter.ThoiGianKhauHao != null ? parameter.ThoiGianKhauHao : taiSan.ThoiGianKhauHao;
                taiSan.ThoiDiemBdtinhKhauHao = parameter.ThoiDiemDatDauTinhKhauHao != null ? parameter.ThoiDiemDatDauTinhKhauHao : taiSan.ThoiDiemBdtinhKhauHao;
                context.TaiSan.Update(taiSan);
                context.SaveChanges();
                return new UpdateKhauHaoMobileResult
                {
                    Message = "Cập nhật khấu hao thành công!",
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new UpdateKhauHaoMobileResult
                {
                    Message = ex.Message,
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }


        public AddTaiSanToDotKiemKeResult AddTaiSanToDotKiemKe(AddTaiSanToDotKiemKeParameter parameter)
        {
            try
            {
                var dotKiemKe = context.DotKiemKe.FirstOrDefault(x => x.DotKiemKeId == parameter.DotKiemKeId);
                if (dotKiemKe == null)
                {
                    return new AddTaiSanToDotKiemKeResult
                    {
                        Message = "Đợt kiểm kê không tồn tại trên hệ thống",
                        StatusCode = HttpStatusCode.Forbidden
                    };
                }

                var nguoiKiemKeEmpId = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                if (nguoiKiemKeEmpId == null)
                {
                    return new AddTaiSanToDotKiemKeResult
                    {
                        Message = "Người kiểm kê không tồn tại trên hệ thống!",
                        StatusCode = HttpStatusCode.Forbidden
                    };
                }

                //Kiểm tra xem tài sản có trong đợt kiểm kê chưa
                var checKTaiSanDotKiemKe = context.DotKiemKeChiTiet.FirstOrDefault(x => x.DotKiemKeId == parameter.DotKiemKeId && x.TaiSanId == parameter.TaiSanId);
                //Chưa có thì thêm mới
                if (checKTaiSanDotKiemKe == null)
                {
                    var newDotKiemKeChiTiet = new DotKiemKeChiTiet();
                    newDotKiemKeChiTiet.DotKiemKeId = parameter.DotKiemKeId;
                    newDotKiemKeChiTiet.TaiSanId = parameter.TaiSanId;
                    newDotKiemKeChiTiet.NguoiKiemKeId = nguoiKiemKeEmpId.EmployeeId.Value;
                    newDotKiemKeChiTiet.CreatedById = parameter.UserId;
                    newDotKiemKeChiTiet.CreatedDate = DateTime.Now;
                    context.DotKiemKeChiTiet.Add(newDotKiemKeChiTiet);
                }
                //có r thì cập nhật
                else
                {
                    checKTaiSanDotKiemKe.NguoiKiemKeId = nguoiKiemKeEmpId.EmployeeId.Value;
                    checKTaiSanDotKiemKe.CreatedById = parameter.UserId;
                    checKTaiSanDotKiemKe.CreatedDate = DateTime.Now;
                    context.DotKiemKeChiTiet.Update(checKTaiSanDotKiemKe);
                }
                context.SaveChanges();
                return new AddTaiSanToDotKiemKeResult
                {
                    Message = "Thêm tài sản kiểm kê thành công!",
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new AddTaiSanToDotKiemKeResult
                {
                    Message = ex.Message,
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }

        public GetMasterDataAddTaiSanVaoDotKiemKeResult GetMasterDataAddTaiSanVaoDotKiemKe(GetMasterDataAddTaiSanVaoDotKiemKeParameter parameter)
        {
            try
            {
                var listDotKiemKe = context.DotKiemKe.Where(x => x.TrangThaiId == 2).Select(x => new DotKiemKeEntityModel {
                    DotKiemKeId = x.DotKiemKeId,
                    TenDoiKiemKe = x.TenDoiKiemKe,
                    NgayBatDau = x.NgayBatDau,
                    NgayKetThuc = x.NgayKetThuc,
                    TrangThaiId = x.TrangThaiId,
                    CreatedById = x.CreatedById,
                    CreatedDate = x.CreatedDate
                }).ToList();//Đang thực hiện
              
                return new GetMasterDataAddTaiSanVaoDotKiemKeResult
                {
                    ListDotKiemKe = listDotKiemKe,
                    Message = "Lấy thông tin thành công!",
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
            catch (Exception ex)
            {
                return new GetMasterDataAddTaiSanVaoDotKiemKeResult
                {
                    Message = ex.Message,
                    StatusCode = HttpStatusCode.Forbidden
                };
            }
        }

    }
}