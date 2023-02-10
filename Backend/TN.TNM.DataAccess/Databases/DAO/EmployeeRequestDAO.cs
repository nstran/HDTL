using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Internal;
using TN.TNM.Common;
using TN.TNM.Common.Helper;
using TN.TNM.DataAccess.ConstType.Note;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Employee;
using TN.TNM.DataAccess.Messages.Results.Employee;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.DeXuatXinNghiModel;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class EmployeeRequestDAO : BaseDAO, IEmployeeRequestDataAccess
    {
        public EmployeeRequestDAO(TNTN8Context _content, IAuditTraceDataAccess _iAuditTrace)
        {
            this.context = _content;
            this.iAuditTrace = _iAuditTrace;
        }

        public CreateEmployeeRequestResult CreateEmployeeRequest(CreateEmplyeeRequestParameter parameter)
        {
            var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
            if (user == null)
            {
                return new CreateEmployeeRequestResult()
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound,
                    MessageCode = "Người dùng không tồn tại trên hệ thống"
                };
            }

            var emp = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
            if (emp == null)
            {
                return new CreateEmployeeRequestResult()
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound,
                    MessageCode = "Người dùng không tồn tại trên hệ thống"
                };
            }

            var org = context.Organization.FirstOrDefault(x => x.OrganizationId == emp.OrganizationId);
            if (org == null)
            {
                return new CreateEmployeeRequestResult()
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound,
                    MessageCode = "Người dùng chưa thuộc phòng ban nào trong hệ thống"
                };
            }

            //Lấy quy trình
            var quyTrinh = context.QuyTrinh.FirstOrDefault(x => x.HoatDong &&
                                                                x.DoiTuongApDung == 9);
            if (quyTrinh == null)
            {
                return new CreateEmployeeRequestResult()
                {
                    StatusCode = System.Net.HttpStatusCode.NotFound,
                    MessageCode = "Chưa có quy trình phê duyệt, bạn phải tạo quy trình trước khi tạo đề xuất"
                };
            }

            parameter.DeXuatXinNghi.EmployeeId = emp.EmployeeId;
            parameter.DeXuatXinNghi.OrganizationId = emp.OrganizationId;
            parameter.DeXuatXinNghi.PositionId = emp.PositionId;

            int deXuatXinNghiId = 0;

            try
            {
                if (parameter.DeXuatXinNghi.ListDate.Count == 0)
                {
                    return new CreateEmployeeRequestResult()
                    {
                        StatusCode = System.Net.HttpStatusCode.NotFound,
                        MessageCode = "Bạn chưa chọn ngày xin nghỉ",
                    };
                }

                using (var trans = context.Database.BeginTransaction())
                {
                    parameter.DeXuatXinNghi.Code = GenCodeDeXuatXinNghi();
                    parameter.DeXuatXinNghi.TrangThaiId = 0;
                    parameter.DeXuatXinNghi.CreatedById = parameter.UserId;
                    var deXuatXinNghi = parameter.DeXuatXinNghi.ToEntity();
                    context.DeXuatXinNghi.Add(deXuatXinNghi);
                    context.SaveChanges();

                    deXuatXinNghiId = deXuatXinNghi.DeXuatXinNghiId;

                    bool isError;
                    string messError;
                    var listDeXuatXinNghiChiTiet = SetListDeXuatXinNghiChiTiet(deXuatXinNghi,
                        parameter.DeXuatXinNghi.ListDate,
                        parameter.DeXuatXinNghi.Ca, parameter.DeXuatXinNghi.TuCa, parameter.DeXuatXinNghi.DenCa,
                        out isError, out messError);

                    //Nếu lỗi
                    if (isError)
                    {
                        return new CreateEmployeeRequestResult()
                        {
                            StatusCode = System.Net.HttpStatusCode.Conflict,
                            MessageCode = messError,
                        };
                    }

                    context.DeXuatXinNghiChiTiet.AddRange(listDeXuatXinNghiChiTiet);
                    context.SaveChanges();

                    trans.Commit();
                }
            }
            catch (Exception ex)
            {
                return new CreateEmployeeRequestResult
                {
                    MessageCode = ex.Message,
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                };
            }

            return new CreateEmployeeRequestResult
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                MessageCode = "Lưu thành công",
                DeXuatXinNghiId = deXuatXinNghiId
            };
        }

        public EditEmployeeRequestByIdResult EditEmployeeRequestById(EditEmployeeRequestByIdParameter parameter)
        {
            try
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    var deXuatXinNghi =
                        context.DeXuatXinNghi.FirstOrDefault(x =>
                            x.DeXuatXinNghiId == parameter.DeXuatXinNghi.DeXuatXinNghiId);
                    if (deXuatXinNghi == null)
                    {
                        return new EditEmployeeRequestByIdResult()
                        {
                            MessageCode = "Đề xuất không tồn tại trên hệ thống",
                            StatusCode = System.Net.HttpStatusCode.NotFound
                        };
                    }

                    if (deXuatXinNghi.TrangThaiId != 0)
                    {
                        return new EditEmployeeRequestByIdResult()
                        {
                            MessageCode = "Chỉ được sửa đề xuất ở trạng thái Mới",
                            StatusCode = System.Net.HttpStatusCode.Conflict
                        };
                    }

                    deXuatXinNghi.LoaiDeXuatId = parameter.DeXuatXinNghi.LoaiDeXuatId;
                    deXuatXinNghi.LyDo = parameter.DeXuatXinNghi.LyDo;
                    context.DeXuatXinNghi.Update(deXuatXinNghi);
                    context.SaveChanges();

                    #region Xóa list date cũ

                    var listOldDeXuatXinNghiChiTiet = context.DeXuatXinNghiChiTiet
                        .Where(x => x.DeXuatXinNghiId == deXuatXinNghi.DeXuatXinNghiId).ToList();
                    context.DeXuatXinNghiChiTiet.RemoveRange(listOldDeXuatXinNghiChiTiet);
                    context.SaveChanges();

                    #endregion

                    #region Thêm lại list date mới

                    bool isError;
                    string messError;
                    var listDeXuatXinNghiChiTiet = SetListDeXuatXinNghiChiTiet(deXuatXinNghi,
                        parameter.DeXuatXinNghi.ListDate,
                        parameter.DeXuatXinNghi.Ca, parameter.DeXuatXinNghi.TuCa, parameter.DeXuatXinNghi.DenCa,
                        out isError, out messError);

                    //Nếu lỗi
                    if (isError)
                    {
                        return new EditEmployeeRequestByIdResult()
                        {
                            StatusCode = System.Net.HttpStatusCode.Conflict,
                            MessageCode = messError,
                        };
                    }

                    context.DeXuatXinNghiChiTiet.AddRange(listDeXuatXinNghiChiTiet);
                    context.SaveChanges();

                    #endregion

                    trans.Commit();

                    return new EditEmployeeRequestByIdResult()
                    {
                        StatusCode = System.Net.HttpStatusCode.OK,
                        MessageCode = "Lưu thành công"
                    };
                }
            }
            catch (Exception ex)
            {
                return new EditEmployeeRequestByIdResult()
                {
                    MessageCode = ex.Message,
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                };
            }
        }

        public GetEmployeeRequestByIdResult GetEmployeeRequestById(GetEmployeeRequestByIdParameter parameter)
        {
            try
            {
                var deXuatXinNghi = context.DeXuatXinNghi
                        .Where(x => x.DeXuatXinNghiId == parameter.DeXuatXinNghiId)
                        .Select(y => new DeXuatXinNghiModel(y)).FirstOrDefault();
                if (deXuatXinNghi == null)
                {
                    return new GetEmployeeRequestByIdResult()
                    {
                        MessageCode = "Đề xuất không tồn tại trên hệ thống",
                        StatusCode = System.Net.HttpStatusCode.NotFound
                    };
                }

                var listStatus = GeneralList.GetTrangThais("TrangThaiDeXuatXinNghi");
                var listKyHieuValue = GetListLoaiDeXuat();
                var listKyHieuChamCong = GeneralList.GetTrangThais("KyHieuChamCong")
                    .Where(x => listKyHieuValue.Contains(x.Value))
                    .ToList();
                var listLoaiCaLamViec = GeneralList.GetTrangThais("LoaiCaLamViec");

                var emp = context.Employee.FirstOrDefault(x => x.EmployeeId == deXuatXinNghi.EmployeeId);
                var org = context.Organization.FirstOrDefault(x => x.OrganizationId == deXuatXinNghi.OrganizationId);
                var position = context.Position.FirstOrDefault(x => x.PositionId == deXuatXinNghi.PositionId);

                deXuatXinNghi.StatusName = listStatus.FirstOrDefault(x => x.Value == deXuatXinNghi.TrangThaiId).Name;
                deXuatXinNghi.EmployeeCodeName = emp?.EmployeeCode + " - " + emp?.EmployeeName;
                deXuatXinNghi.OrganizationName = org?.OrganizationName;
                deXuatXinNghi.PositionName = position?.PositionName;

                //Chỉ hiển thị số ngày phép còn lại khi đề xuất ở trạng thái Mới
                if (deXuatXinNghi.TrangThaiId == 0)
                {
                    deXuatXinNghi.SoNgayPhepConLai = emp?.SoNgayPhepConLai;
                }
                else
                {
                    deXuatXinNghi.SoNgayPhepConLai = 0;
                }

                deXuatXinNghi = CommonHelper.GetInforDeXuatXinNghi(context, deXuatXinNghi);

                #region Điều kiện hiển thị các button

                bool isShowGuiPheDuyet = false;
                bool isShowPheDuyet = false;
                bool isShowTuChoi = false;
                bool isShowDatVeMoi = false;
                bool isShowXoa = false;
                bool isShowSua = false;

                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);

                //Trạng thái Mới và User đăng nhập là người tạo đề xuất
                if (deXuatXinNghi.TrangThaiId == 0 && deXuatXinNghi.EmployeeId == user.EmployeeId)
                {
                    isShowGuiPheDuyet = true;
                    isShowXoa = true;
                    isShowSua = true;
                }

                //Trạng thái Chờ phê duyệt
                if (deXuatXinNghi.TrangThaiId == 2)
                {
                    var buocHienTai = context.CacBuocApDung.Where(x => x.ObjectNumber == deXuatXinNghi.DeXuatXinNghiId &&
                                                                       x.DoiTuongApDung == 9 &&
                                                                       x.TrangThai == 0)
                        .OrderByDescending(z => z.Stt)
                        .FirstOrDefault();

                    //Nếu là phê duyệt trưởng bộ phận
                    if (buocHienTai?.LoaiPheDuyet == 1)
                    {
                        //Lấy list phòng ban của người tạo đề xuất
                        var listPhongBanId_NguoiPhuTrach = context.ThanhVienPhongBan
                            .Where(x => x.EmployeeId == deXuatXinNghi.EmployeeId)
                            .Select(y => y.OrganizationId).ToList();

                        //Lấy số phòng ban mà User đăng nhập là trưởng bộ phận trong số phòng ban của người tạo đề xuất
                        var countPheDuyet = context.ThanhVienPhongBan.Count(x => x.EmployeeId == user.EmployeeId &&
                                                                                 x.IsManager == 1 &&
                                                                                 listPhongBanId_NguoiPhuTrach.Contains(
                                                                                     x.OrganizationId));

                        //Nếu User đăng nhập là trưởng bộ phận của 1 trong số các phòng ban của người tạo đề xuất
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
                        var listPhongBanIdDaPheDuyet = context.PhongBanApDung
                            .Where(x => x.CacBuocApDungId == buocHienTai.Id &&
                                        x.CacBuocQuyTrinhId == buocHienTai.CacBuocQuyTrinhId)
                            .Select(y => y.OrganizationId).ToList();

                        //Lấy list Phòng ban chưa phê duyệt ở bước hiện tại
                        var listPhongBanId = context.PhongBanTrongCacBuocQuyTrinh
                            .Where(x => x.CacBuocQuyTrinhId == buocHienTai.CacBuocQuyTrinhId &&
                                        !listPhongBanIdDaPheDuyet.Contains(x.OrganizationId))
                            .Select(y => y.OrganizationId).ToList();

                        //Lấy số phòng ban mà User đăng nhập là trưởng bộ phận trong số các phòng ban chưa phê duyệt ở bước hiện tại
                        var countPheDuyet = context.ThanhVienPhongBan.Count(x => x.EmployeeId == user.EmployeeId &&
                                                                                 x.IsManager == 1 &&
                                                                                 listPhongBanId.Contains(
                                                                                     x.OrganizationId));

                        //Nếu User đăng nhập là trưởng bộ phận của 1 trong số các phòng ban chưa phê duyệt ở bước hiện tại
                        if (countPheDuyet > 0)
                        {
                            isShowPheDuyet = true;
                            isShowTuChoi = true;
                        }
                    }
                }

                //Trạng thái Từ chối và User đăng nhập là người tạo đề xuất
                if (deXuatXinNghi.TrangThaiId == 1 && deXuatXinNghi.EmployeeId == user.EmployeeId)
                {
                    isShowDatVeMoi = true;
                }

                #endregion

                return new GetEmployeeRequestByIdResult()
                {
                    MessageCode = "OK",
                    StatusCode = System.Net.HttpStatusCode.OK,
                    DeXuatXinNghi = deXuatXinNghi,
                    ListKyHieuChamCong = listKyHieuChamCong,
                    ListLoaiCaLamViec = listLoaiCaLamViec,
                    IsShowGuiPheDuyet = isShowGuiPheDuyet,
                    IsShowPheDuyet = isShowPheDuyet,
                    IsShowTuChoi = isShowTuChoi,
                    IsShowDatVeMoi = isShowDatVeMoi,
                    IsShowXoa = isShowXoa,
                    IsShowSua = isShowSua
                };
            }
            catch (Exception ex)
            {
                return new GetEmployeeRequestByIdResult()
                {
                    MessageCode = ex.Message,
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                };
            }
        }

        public SearchEmployeeRequestResult SearchEmployeeRequest(SearchEmployeeRequestParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                if (user == null)
                {
                    return new SearchEmployeeRequestResult()
                    {
                        StatusCode = System.Net.HttpStatusCode.NotFound,
                        MessageCode = "Người dùng không tồn tại trên hệ thống"
                    };
                }

                var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                if (employee == null)
                {
                    return new SearchEmployeeRequestResult()
                    {
                        StatusCode = System.Net.HttpStatusCode.NotFound,
                        MessageCode = "Người dùng không tồn tại trên hệ thống"
                    };
                }

                var organization = context.Organization.FirstOrDefault(x => x.OrganizationId == employee.OrganizationId);
                if (organization == null)
                {
                    return new SearchEmployeeRequestResult()
                    {
                        StatusCode = System.Net.HttpStatusCode.NotFound,
                        MessageCode = "Người dùng chưa thuộc phòng ban nào trong hệ thống"
                    };
                }

                var thanhVienPhongBan =
                    context.ThanhVienPhongBan.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId);

                var listStatus = GeneralList.GetTrangThais("TrangThaiDeXuatXinNghi");
                var listKyHieuValue = GetListLoaiDeXuat();
                var listKyHieuChamCong = GeneralList.GetTrangThais("KyHieuChamCong")
                    .Where(x => listKyHieuValue.Contains(x.Value))
                    .ToList();
                var listDeXuatXinNghi = new List<DeXuatXinNghiModel>();

                //Nếu người dùng thuộc phòng ban được quyền xem tất dữ liệu của các phòng ban khác
                if (organization.IsAccess == true)
                {
                    listDeXuatXinNghi = (from dxxn in context.DeXuatXinNghi
                                         join emp in context.Employee on dxxn.EmployeeId equals emp.EmployeeId
                                         join org in context.Organization on emp.OrganizationId equals org.OrganizationId into tmpOrg
                                         from org in tmpOrg.DefaultIfEmpty()
                                         where
                                             (string.IsNullOrWhiteSpace(parameter.Code) ||
                                              dxxn.Code.ToLower().Trim().Contains(parameter.Code.ToLower().Trim())) &&
                                             (string.IsNullOrWhiteSpace(parameter.EmployeeCode) || emp.EmployeeCode.ToLower().Trim()
                                                  .Contains(parameter.EmployeeCode.ToLower().Trim())) &&
                                             (string.IsNullOrWhiteSpace(parameter.EmployeeName) || emp.EmployeeName.ToLower().Trim()
                                                  .Contains(parameter.EmployeeName.ToLower().Trim())) &&
                                             (parameter.ListStatusId.Count == 0 || parameter.ListStatusId.Contains(dxxn.TrangThaiId)) &&
                                             (parameter.ListLoaiDeXuatId.Count == 0 ||
                                              parameter.ListLoaiDeXuatId.Contains(dxxn.LoaiDeXuatId))
                                              && (dxxn.EmployeeId == employee.EmployeeId || (dxxn.EmployeeId != employee.EmployeeId && dxxn.TrangThaiId != 0)) // xem đề xuất mình tạo ra và của người khác với trạng thái khác mới
                                         select new DeXuatXinNghiModel()
                                         {
                                             DeXuatXinNghiId = dxxn.DeXuatXinNghiId,
                                             Code = dxxn.Code,
                                             EmployeeId = dxxn.EmployeeId,
                                             EmployeeCodeName = emp.EmployeeCode + " - " + emp.EmployeeName,
                                             OrganizationName = org.OrganizationName,
                                             StatusName = listStatus.FirstOrDefault(x => x.Value == dxxn.TrangThaiId).Name,
                                             CreatedDate = dxxn.CreatedDate,
                                             TrangThaiId = dxxn.TrangThaiId,
                                             TenLoaiDeXuat = listKyHieuChamCong.FirstOrDefault(x => x.Value == dxxn.LoaiDeXuatId).Name,
                                             BackgroupStatusColor = GetBackgroundStatusColor(dxxn.TrangThaiId)
                                         }).OrderByDescending(z => z.CreatedDate).ToList();
                }
                //Nếu người dùng không thuộc phòng ban được quyền xem tất dữ liệu của các phòng ban khác
                else
                {
                    //Nếu là trưởng bộ phận
                    if (thanhVienPhongBan.IsManager == 1)
                    {
                        //Lấy ra list đối tượng id mà người dùng phụ trách phê duyệt
                        var listId = context.PhongBanPheDuyetDoiTuong
                            .Where(x => x.DoiTuongApDung == 9 &&
                                        x.OrganizationId == organization.OrganizationId).Select(y => y.ObjectNumber)
                            .ToList();

                        listDeXuatXinNghi = (from dxxn in context.DeXuatXinNghi
                                             join emp in context.Employee on dxxn.EmployeeId equals emp.EmployeeId
                                             join org in context.Organization on emp.OrganizationId equals org.OrganizationId into tmpOrg
                                             from org in tmpOrg.DefaultIfEmpty()
                                             where
                                                 (string.IsNullOrWhiteSpace(parameter.Code) ||
                                                  dxxn.Code.ToLower().Trim().Contains(parameter.Code.ToLower().Trim())) &&
                                                 (string.IsNullOrWhiteSpace(parameter.EmployeeCode) || emp.EmployeeCode.ToLower().Trim()
                                                      .Contains(parameter.EmployeeCode.ToLower().Trim())) &&
                                                 (string.IsNullOrWhiteSpace(parameter.EmployeeName) || emp.EmployeeName.ToLower().Trim()
                                                      .Contains(parameter.EmployeeName.ToLower().Trim())) &&
                                                 (dxxn.EmployeeId == employee.EmployeeId || listId.Contains(dxxn.DeXuatXinNghiId)) &&
                                                 (parameter.ListStatusId.Count == 0 || parameter.ListStatusId.Contains(dxxn.TrangThaiId)) &&
                                                 (parameter.ListLoaiDeXuatId.Count == 0 ||
                                                  parameter.ListLoaiDeXuatId.Contains(dxxn.LoaiDeXuatId))
                                             select new DeXuatXinNghiModel()
                                             {
                                                 DeXuatXinNghiId = dxxn.DeXuatXinNghiId,
                                                 Code = dxxn.Code,
                                                 EmployeeId = dxxn.EmployeeId,
                                                 EmployeeCodeName = emp.EmployeeCode + " - " + emp.EmployeeName,
                                                 OrganizationName = org.OrganizationName,
                                                 StatusName = listStatus.FirstOrDefault(x => x.Value == dxxn.TrangThaiId).Name,
                                                 CreatedDate = dxxn.CreatedDate,
                                                 TrangThaiId = dxxn.TrangThaiId,
                                                 TenLoaiDeXuat = listKyHieuChamCong.FirstOrDefault(x => x.Value == dxxn.LoaiDeXuatId).Name,
                                                 BackgroupStatusColor = GetBackgroundStatusColor(dxxn.TrangThaiId)
                                             }).OrderByDescending(z => z.CreatedDate).ToList();
                    }
                    else
                    {
                        listDeXuatXinNghi = (from dxxn in context.DeXuatXinNghi
                                             join emp in context.Employee on dxxn.EmployeeId equals emp.EmployeeId
                                             join org in context.Organization on emp.OrganizationId equals org.OrganizationId into tmpOrg
                                             from org in tmpOrg.DefaultIfEmpty()
                                             where
                                                 (string.IsNullOrWhiteSpace(parameter.Code) ||
                                                  dxxn.Code.ToLower().Trim().Contains(parameter.Code.ToLower().Trim())) &&
                                                 (string.IsNullOrWhiteSpace(parameter.EmployeeCode) || emp.EmployeeCode.ToLower().Trim()
                                                      .Contains(parameter.EmployeeCode.ToLower().Trim())) &&
                                                 (string.IsNullOrWhiteSpace(parameter.EmployeeName) || emp.EmployeeName.ToLower().Trim()
                                                      .Contains(parameter.EmployeeName.ToLower().Trim())) &&
                                                 dxxn.EmployeeId == employee.EmployeeId &&
                                                 (parameter.ListStatusId.Count == 0 || parameter.ListStatusId.Contains(dxxn.TrangThaiId)) &&
                                                 (parameter.ListLoaiDeXuatId.Count == 0 ||
                                                  parameter.ListLoaiDeXuatId.Contains(dxxn.LoaiDeXuatId))
                                             select new DeXuatXinNghiModel()
                                             {
                                                 DeXuatXinNghiId = dxxn.DeXuatXinNghiId,
                                                 Code = dxxn.Code,
                                                 EmployeeId = dxxn.EmployeeId,
                                                 EmployeeCodeName = emp.EmployeeCode + " - " + emp.EmployeeName,
                                                 OrganizationName = org.OrganizationName,
                                                 StatusName = listStatus.FirstOrDefault(x => x.Value == dxxn.TrangThaiId).Name,
                                                 CreatedDate = dxxn.CreatedDate,
                                                 TrangThaiId = dxxn.TrangThaiId,
                                                 TenLoaiDeXuat = listKyHieuChamCong.FirstOrDefault(x => x.Value == dxxn.LoaiDeXuatId).Name,
                                                 BackgroupStatusColor = GetBackgroundStatusColor(dxxn.TrangThaiId)
                                             }).OrderByDescending(z => z.CreatedDate).ToList();
                    }
                }

                return new SearchEmployeeRequestResult
                {
                    MessageCode = "Success",
                    StatusCode = System.Net.HttpStatusCode.OK,
                    ListDeXuatXinNghi = listDeXuatXinNghi
                };
            }
            catch (Exception e)
            {
                return new SearchEmployeeRequestResult
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                };
            }
        }

        public GetDataSearchEmployeeRequestResult GetDataSearchEmployeeRequest(GetDataSearchEmployeeRequestParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                if (user == null)
                {
                    return new GetDataSearchEmployeeRequestResult()
                    {
                        StatusCode = System.Net.HttpStatusCode.NotFound,
                        MessageCode = "Người dùng không tồn tại trên hệ thống"
                    };
                }

                var emp = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                if (emp == null)
                {
                    return new GetDataSearchEmployeeRequestResult()
                    {
                        StatusCode = System.Net.HttpStatusCode.NotFound,
                        MessageCode = "Người dùng không tồn tại trên hệ thống"
                    };
                }

                var org = context.Organization.FirstOrDefault(x => x.OrganizationId == emp.OrganizationId);
                if (org == null)
                {
                    return new GetDataSearchEmployeeRequestResult()
                    {
                        StatusCode = System.Net.HttpStatusCode.NotFound,
                        MessageCode = "Người dùng chưa thuộc phòng ban nào trong hệ thống"
                    };
                }

                bool isShowOrganization = org.IsAccess == true;

                var listStatus = GeneralList.GetTrangThais("TrangThaiDeXuatXinNghi");

                var listKyHieuValue = GetListLoaiDeXuat();
                var listKyHieuChamCong = GeneralList.GetTrangThais("KyHieuChamCong")
                    .Where(x => listKyHieuValue.Contains(x.Value))
                    .ToList();

                return new GetDataSearchEmployeeRequestResult
                {
                    IsShowOrganization = isShowOrganization,
                    OrganizationId = org.OrganizationId,
                    OrganizationName = org.OrganizationName,
                    ListStatus = listStatus,
                    ListKyHieuChamCong = listKyHieuChamCong,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "Success",
                };
            }
            catch (Exception ex)
            {
                return new GetDataSearchEmployeeRequestResult
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = ex.Message
                };
            }
        }

        public GetMasterCreateEmpRequestResult GetMasterCreateEmpRequest(GetMasterCreateEmpRequestParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                if (user == null)
                {
                    return new GetMasterCreateEmpRequestResult()
                    {
                        StatusCode = System.Net.HttpStatusCode.NotFound,
                        MessageCode = "Người dùng không tồn tại trên hệ thống"
                    };
                }

                var emp = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                if (emp == null)
                {
                    return new GetMasterCreateEmpRequestResult()
                    {
                        StatusCode = System.Net.HttpStatusCode.NotFound,
                        MessageCode = "Người dùng không tồn tại trên hệ thống"
                    };
                }

                var org = context.Organization.FirstOrDefault(x => x.OrganizationId == emp.OrganizationId);
                var position = context.Position.FirstOrDefault(x => x.PositionId == emp.PositionId);

                var listLoaiCaLamViec = GeneralList.GetTrangThais("LoaiCaLamViec");
                var listKyHieuValue = GetListLoaiDeXuat();
                var listKyHieuChamCong = GeneralList.GetTrangThais("KyHieuChamCong")
                    .Where(x => listKyHieuValue.Contains(x.Value))
                    .ToList();

                return new GetMasterCreateEmpRequestResult()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "OK",
                    ListKyHieuChamCong = listKyHieuChamCong,
                    ListLoaiCaLamViec = listLoaiCaLamViec,
                    EmployeeName = emp.EmployeeCode + " - " + emp.EmployeeName,
                    OrganizationName = org?.OrganizationName,
                    PositionName = position?.PositionName,
                    SoNgayPhepConLai = emp.SoNgayPhepConLai ?? 0
                };
            }
            catch (Exception e)
            {
                return new GetMasterCreateEmpRequestResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public DeleteDeXuatXinNghiByIdResult DeleteDeXuatXinNghiById(DeleteDeXuatXinNghiByIdParameter parameter)
        {
            try
            {
                var dxxn = context.DeXuatXinNghi.FirstOrDefault(x => x.DeXuatXinNghiId == parameter.Id);
                if (dxxn == null)
                {
                    return new DeleteDeXuatXinNghiByIdResult()
                    {
                        MessageCode = "Đề xuất không tồn tại trên hệ thống",
                        StatusCode = System.Net.HttpStatusCode.NotFound
                    };
                }

                if (dxxn.TrangThaiId != 0)
                {
                    return new DeleteDeXuatXinNghiByIdResult()
                    {
                        MessageCode = "Chỉ được xóa đề xuất ở trạng thái Mới",
                        StatusCode = System.Net.HttpStatusCode.Conflict
                    };
                }

                //Ghi chú
                var listNote = context.Note
                    .Where(x => x.ObjectNumber == dxxn.DeXuatXinNghiId && x.ObjectType == NoteObjectType.DXXN).ToList();

                var listDxxnChiTiet =
                    context.DeXuatXinNghiChiTiet.Where(x => x.DeXuatXinNghiId == parameter.Id).ToList();

                context.Note.RemoveRange(listNote);
                context.DeXuatXinNghiChiTiet.RemoveRange(listDxxnChiTiet);
                context.DeXuatXinNghi.Remove(dxxn);
                context.SaveChanges();

                return new DeleteDeXuatXinNghiByIdResult()
                {
                    MessageCode = "Xóa thành công",
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }
            catch (Exception e)
            {
                return new DeleteDeXuatXinNghiByIdResult()
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                };
            }
        }

        public DatVeMoiDeXuatXinNghiResult DatVeMoiDeXuatXinNghi(DatVeMoiDeXuatXinNghiParameter parameter)
        {
            try
            {
                var dxxn = context.DeXuatXinNghi.FirstOrDefault(x => x.DeXuatXinNghiId == parameter.Id);
                if (dxxn == null)
                {
                    return new DatVeMoiDeXuatXinNghiResult()
                    {
                        MessageCode = "Đề xuất không tồn tại trên hệ thống",
                        StatusCode = System.Net.HttpStatusCode.NotFound
                    };
                }

                if (dxxn.TrangThaiId != 1)
                {
                    return new DatVeMoiDeXuatXinNghiResult()
                    {
                        MessageCode = "Chỉ được đặt về mới đề xuất ở trạng thái Từ chối",
                        StatusCode = System.Net.HttpStatusCode.Conflict
                    };
                }

                dxxn.TrangThaiId = 0;
                context.DeXuatXinNghi.Update(dxxn);

                //Thêm ghi chú
                Note note = new Note();
                note.NoteId = Guid.NewGuid();
                note.ObjectId = Guid.Empty;
                note.Description = "Đã đặt đề xuất về mới";
                note.Type = "ADD";
                note.Active = true;
                note.CreatedById = parameter.UserId;
                note.CreatedDate = DateTime.Now;
                note.NoteTitle = "Đã thêm ghi chú";
                note.ObjectNumber = dxxn.DeXuatXinNghiId;
                note.ObjectType = NoteObjectType.DXXN;

                context.Note.Add(note);

                context.SaveChanges();

                return new DatVeMoiDeXuatXinNghiResult()
                {
                    MessageCode = "Đặt về mới thành công",
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }
            catch (Exception e)
            {
                return new DatVeMoiDeXuatXinNghiResult()
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                };
            }
        }

        private string GenCodeDeXuatXinNghi()
        {
            var code = "";
            var prefix = "DXXN-";

            var list = context.DeXuatXinNghi.Where(x => x.Code.Contains(prefix)).Select(y => new
            {
                Code = Int32.Parse(y.Code.Substring(5))
            }).OrderByDescending(z => z.Code).ToList();

            if (list.Count == 0)
            {
                code = prefix + 1.ToString("D3");
            }
            else if (list.First().Code < 999)
            {
                code = prefix + (list.First().Code + 1).ToString("D3");
            }
            else
            {
                code = prefix + (list.First().Code + 1);
            }

            return code;
        }

        private List<Guid> GetOrganizationChildrenId(Guid id, List<Guid> list, List<Organization> listOrganization)
        {
            var listChildren = listOrganization.Where(o => o.ParentId == id).ToList();
            listChildren.ForEach(item =>
            {
                list.Add(item.OrganizationId);
                GetOrganizationChildrenId(item.OrganizationId, list, listOrganization);
            });

            return list;
        }

        private List<int> GetListLoaiDeXuat()
        {
            return new List<int>() {1, 5, 8, 12, 13};
        }

        private string GetBackgroundStatusColor(int trangThaiId)
        {
            string color = "";

            //Mới
            if (trangThaiId == 0)
            {
                color = "#AEA4A0";
            }
            //Từ chối
            else if (trangThaiId == 1)
            {
                color = "#CC3C00";
            }
            //Chờ phê duyệt
            else if (trangThaiId == 2)
            {
                color = "#FFCC00";
            }
            //Đã phê duyệt
            else if (trangThaiId == 3)
            {
                color = "#007AFF";
            }

            return color;
        }

        private string ConvertNgayTrongTuan(DateTime date)
        {
            string result = date.ToString("dd/MM/yyyy") + " - ";
            var ngay = date.DayOfWeek;
            var code = -1;

            switch (ngay)
            {
                case DayOfWeek.Monday:
                    code = 1;
                    break;
                case DayOfWeek.Tuesday:
                    code = 2;
                    break;
                case DayOfWeek.Wednesday:
                    code = 3;
                    break;
                case DayOfWeek.Thursday:
                    code = 4;
                    break;
                case DayOfWeek.Friday:
                    code = 5;
                    break;
                case DayOfWeek.Saturday:
                    code = 6;
                    break;
                case DayOfWeek.Sunday:
                    code = 0;
                    break;
                default:
                    code = -1;
                    break;
            }

            var listNgayLamViecTrongTuan = GeneralList.GetTrangThais("NgayLamViecTrongTuan");
            result += listNgayLamViecTrongTuan.FirstOrDefault(x => x.Value == code)?.Name;

            return result;
        }

        private List<DeXuatXinNghiChiTiet> SetListDeXuatXinNghiChiTiet(DeXuatXinNghi DeXuatXinNghi, List<DateTime> ListDate,
            int? Ca, int? TuCa, int? DenCa, out bool isError, out string messError)
        {
            isError = false;
            messError = "";
            var listDeXuatXinNghiChiTiet = new List<DeXuatXinNghiChiTiet>();

            //Nếu là Đi muộn hoặc Về sớm
            if (DeXuatXinNghi.LoaiDeXuatId == 12 || DeXuatXinNghi.LoaiDeXuatId == 13)
            {
                ListDate.ForEach(date =>
                {
                    var newItem = new DeXuatXinNghiChiTiet();
                    newItem.DeXuatXinNghiId = DeXuatXinNghi.DeXuatXinNghiId;
                    newItem.Ngay = date.Date;
                    newItem.LoaiCaLamViecId = Ca.Value;

                    listDeXuatXinNghiChiTiet.Add(newItem);
                });
            }
            //Nếu ngược lại
            else
            {
                //Sắp xếp lại list theo ngày xa nhất xếp trước
                var listDate = ListDate.Select(y => y.Date).OrderBy(z => z).ToList();

                //Nếu số ngày xin nghỉ = 1 ngày
                if (listDate.Count == 1)
                {
                    //Nếu chọn Từ ca > Đến ca
                    if (TuCa > DenCa)
                    {
                        isError = true;
                        messError = "Chọn Từ ca làm việc, Đến ca làm việc không hợp lệ";
                        return new List<DeXuatXinNghiChiTiet>();
                    }

                    //Nếu ca bắt đầu = ca kết thúc
                    if (TuCa == DenCa)
                    {
                        var newItem = new DeXuatXinNghiChiTiet();
                        newItem.DeXuatXinNghiId = DeXuatXinNghi.DeXuatXinNghiId;
                        newItem.Ngay = listDate.First().Date;
                        newItem.LoaiCaLamViecId = TuCa.Value;

                        listDeXuatXinNghiChiTiet.Add(newItem);
                    }
                    else
                    {
                        //Ca sáng
                        var newItem1 = new DeXuatXinNghiChiTiet();
                        newItem1.DeXuatXinNghiId = DeXuatXinNghi.DeXuatXinNghiId;
                        newItem1.Ngay = listDate.First().Date;
                        newItem1.LoaiCaLamViecId = 1;

                        //Ca chiều
                        var newItem2 = new DeXuatXinNghiChiTiet();
                        newItem2.DeXuatXinNghiId = DeXuatXinNghi.DeXuatXinNghiId;
                        newItem2.Ngay = listDate.First().Date;
                        newItem2.LoaiCaLamViecId = 2;

                        listDeXuatXinNghiChiTiet.Add(newItem1);
                        listDeXuatXinNghiChiTiet.Add(newItem2);
                    }
                }
                //Nếu số ngày xin nghỉ > 1 ngày
                else if (listDate.Count > 1)
                {
                    for (int i = 0; i < listDate.Count; i++)
                    {
                        var date = listDate[i].Date;

                        //Nếu là ngày đầu tiên
                        if (i == 0)
                        {
                            //Nếu ca bắt đầu là ca sáng
                            if (TuCa == 1)
                            {
                                //Ca sáng
                                var newItem1 = new DeXuatXinNghiChiTiet();
                                newItem1.DeXuatXinNghiId = DeXuatXinNghi.DeXuatXinNghiId;
                                newItem1.Ngay = date;
                                newItem1.LoaiCaLamViecId = 1;

                                //Ca chiều
                                var newItem2 = new DeXuatXinNghiChiTiet();
                                newItem2.DeXuatXinNghiId = DeXuatXinNghi.DeXuatXinNghiId;
                                newItem2.Ngay = date;
                                newItem2.LoaiCaLamViecId = 2;

                                listDeXuatXinNghiChiTiet.Add(newItem1);
                                listDeXuatXinNghiChiTiet.Add(newItem2);
                            }
                            //Nếu ca bắt đầu là ca chiều
                            else
                            {
                                var newItem = new DeXuatXinNghiChiTiet();
                                newItem.DeXuatXinNghiId = DeXuatXinNghi.DeXuatXinNghiId;
                                newItem.Ngay = date;
                                newItem.LoaiCaLamViecId = 2;

                                listDeXuatXinNghiChiTiet.Add(newItem);
                            }
                        }
                        //Nếu là ngày cuối cùng
                        else if (i == listDate.Count - 1)
                        {
                            //Nếu ca kết thúc là ca sáng
                            if (DenCa == 1)
                            {
                                var newItem = new DeXuatXinNghiChiTiet();
                                newItem.DeXuatXinNghiId = DeXuatXinNghi.DeXuatXinNghiId;
                                newItem.Ngay = date;
                                newItem.LoaiCaLamViecId = 1;

                                listDeXuatXinNghiChiTiet.Add(newItem);
                            }
                            //Nếu ca kết thúc là ca chiều
                            else
                            {
                                //Ca sáng
                                var newItem1 = new DeXuatXinNghiChiTiet();
                                newItem1.DeXuatXinNghiId = DeXuatXinNghi.DeXuatXinNghiId;
                                newItem1.Ngay = date;
                                newItem1.LoaiCaLamViecId = 1;

                                //Ca chiều
                                var newItem2 = new DeXuatXinNghiChiTiet();
                                newItem2.DeXuatXinNghiId = DeXuatXinNghi.DeXuatXinNghiId;
                                newItem2.Ngay = date;
                                newItem2.LoaiCaLamViecId = 2;

                                listDeXuatXinNghiChiTiet.Add(newItem1);
                                listDeXuatXinNghiChiTiet.Add(newItem2);
                            }
                        }
                        //Nếu không phải ngày đầu tiên hoặc cuối cùng
                        else
                        {
                            //Ca sáng
                            var newItem1 = new DeXuatXinNghiChiTiet();
                            newItem1.DeXuatXinNghiId = DeXuatXinNghi.DeXuatXinNghiId;
                            newItem1.Ngay = date;
                            newItem1.LoaiCaLamViecId = 1;

                            //Ca chiều
                            var newItem2 = new DeXuatXinNghiChiTiet();
                            newItem2.DeXuatXinNghiId = DeXuatXinNghi.DeXuatXinNghiId;
                            newItem2.Ngay = date;
                            newItem2.LoaiCaLamViecId = 2;

                            listDeXuatXinNghiChiTiet.Add(newItem1);
                            listDeXuatXinNghiChiTiet.Add(newItem2);
                        }
                    }
                }
            }

            return listDeXuatXinNghiChiTiet;
        }

        private List<int> GetListCanPheDuyet(Guid employeeId)
        {
            var result = new List<int>();
            int doiTuongApDung = 9;

            //Quy trình
            var quyTrinh = context.QuyTrinh.FirstOrDefault(x => x.DoiTuongApDung == doiTuongApDung &&
                                                                x.HoatDong);

            //Nếu không có quy trình
            if (quyTrinh == null)
            {
                return result;
            }

            //Lấy list id đối tượng 
            var listObjectNumber = context.CacBuocApDung.Where(x => x.QuyTrinhId == quyTrinh.Id &&
                                                                    x.DoiTuongApDung == doiTuongApDung &&
                                                                    x.ObjectNumber != null)
                .Select(y => y.ObjectNumber).ToList();



            return result;
        }
    }
}
