using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.QuyTrinh;
using TN.TNM.DataAccess.Messages.Results.QuyTrinh;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.QuyTrinh;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.ConstType.Note;
using TN.TNM.Common.NotificationSetting;
using TN.TNM.DataAccess.Models.DeXuatXinNghiModel;
using TN.TNM.DataAccess.Consts.Product;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class QuyTrinhDAO : BaseDAO, IQuyTrinhDataAccess
    {
        public QuyTrinhDAO(TNTN8Context _context)
        {
            this.context = _context;
        }

        public CreateQuyTrinhResult CreateQuyTrinh(CreateQuyTrinhParameter parameter)
        {
            try
            {
                var exist = context.QuyTrinh.FirstOrDefault(x => x.DoiTuongApDung == parameter.QuyTrinh.DoiTuongApDung);
                if (exist != null)
                {
                    return new CreateQuyTrinhResult()
                    {
                        MessageCode = "Đã tồn tại Quy trình cho đối tượng này",
                        StatusCode = System.Net.HttpStatusCode.Conflict
                    };
                }

                //Nếu quy trình có trạng thái Hoạt động
                if (parameter.QuyTrinh.HoatDong)
                {
                    //Update trạng thái Hoạt động các đối tượng áp dụng -> false
                    var listQuyTrinh = context.QuyTrinh
                        .Where(x => x.DoiTuongApDung == parameter.QuyTrinh.DoiTuongApDung).ToList();
                    listQuyTrinh.ForEach(item => { item.HoatDong = false; });
                    context.QuyTrinh.UpdateRange(listQuyTrinh);
                }

                var quyTrinh = new QuyTrinh();
                quyTrinh.Id = Guid.NewGuid();
                quyTrinh.TenQuyTrinh = parameter.QuyTrinh.TenQuyTrinh;
                quyTrinh.MaQuyTrinh = GenCode();
                quyTrinh.DoiTuongApDung = parameter.QuyTrinh.DoiTuongApDung;
                quyTrinh.HoatDong = parameter.QuyTrinh.HoatDong;
                quyTrinh.MoTa = parameter.QuyTrinh.MoTa;
                quyTrinh.CreatedById = parameter.UserId;
                quyTrinh.CreatedDate = DateTime.Now;

                var listCauHinhQuyTrinh = new List<CauHinhQuyTrinh>();
                var listCacBuocQuyTrinh = new List<CacBuocQuyTrinh>();
                var listPhongBanTrongCacBuocQuyTrinh = new List<PhongBanTrongCacBuocQuyTrinh>();

                parameter.ListCauHinhQuyTrinh.ForEach(cauHinh =>
                {
                    var newCauHinh = new CauHinhQuyTrinh();
                    newCauHinh.Id = Guid.NewGuid();
                    newCauHinh.SoTienTu = cauHinh.SoTienTu;
                    newCauHinh.TenCauHinh = cauHinh.TenCauHinh;
                    newCauHinh.QuyTrinh = cauHinh.QuyTrinh;
                    newCauHinh.QuyTrinhId = quyTrinh.Id;

                    listCauHinhQuyTrinh.Add(newCauHinh);

                    cauHinh.ListCacBuocQuyTrinh.ForEach(buoc =>
                    {
                        var newBuoc = new CacBuocQuyTrinh();
                        newBuoc.Id = Guid.NewGuid();
                        newBuoc.Stt = buoc.Stt;
                        newBuoc.LoaiPheDuyet = buoc.LoaiPheDuyet;
                        newBuoc.CauHinhQuyTrinhId = newCauHinh.Id;

                        listCacBuocQuyTrinh.Add(newBuoc);

                        buoc.ListPhongBanTrongCacBuocQuyTrinh.ForEach(phongBan =>
                        {
                            var newPhongBan = new PhongBanTrongCacBuocQuyTrinh();
                            newPhongBan.Id = Guid.NewGuid();
                            newPhongBan.OrganizationId = phongBan.OrganizationId;
                            newPhongBan.CacBuocQuyTrinhId = newBuoc.Id;

                            listPhongBanTrongCacBuocQuyTrinh.Add(newPhongBan);
                        });
                    });
                });

                context.QuyTrinh.Add(quyTrinh);
                context.CauHinhQuyTrinh.AddRange(listCauHinhQuyTrinh);
                context.CacBuocQuyTrinh.AddRange(listCacBuocQuyTrinh);
                context.PhongBanTrongCacBuocQuyTrinh.AddRange(listPhongBanTrongCacBuocQuyTrinh);
                context.SaveChanges();

                return new CreateQuyTrinhResult()
                {
                    Status = true,
                    Message = "Tạo thành công",
                    MessageCode = "Tạo thành công",
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Id = quyTrinh.Id
                };
            }
            catch (Exception e)
            {
                return new CreateQuyTrinhResult()
                {
                    Status = false,
                    Message = e.Message,
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                };
            }
        }

        public SearchQuyTrinhResult SearchQuyTrinh(SearchQuyTrinhParameter parameter)
        {
            try
            {
                var ListQuyTrinh = new List<QuyTrinhModel>();
                var listUserId = context.User.Where(x => parameter.ListEmployeeId.Contains(x.EmployeeId.Value)).Select(
                    y => y.UserId).ToList();

                ListQuyTrinh = context.QuyTrinh.Where(x =>
                        (listUserId.Count == 0 || listUserId.Contains(x.CreatedById)) &&
                        (parameter.TenQuyTrinh == null || parameter.TenQuyTrinh.Trim() == "" ||
                         x.TenQuyTrinh.Contains(parameter.TenQuyTrinh)) &&
                        (parameter.MaQuyTrinh == null || parameter.MaQuyTrinh.Trim() == "" ||
                         x.MaQuyTrinh.Contains(parameter.MaQuyTrinh)) &&
                        (parameter.CreatedDateFrom == null ||
                         x.CreatedDate.Date >= parameter.CreatedDateFrom.Value.Date) &&
                        (parameter.CreatedDateTo == null || 
                         x.CreatedDate.Date <= parameter.CreatedDateTo.Value.Date) &&
                        (parameter.ListTrangThai.Count == 0 || parameter.ListTrangThai.Contains(x.HoatDong)))
                    .Select(y => new QuyTrinhModel
                    {
                        Id = y.Id,
                        TenQuyTrinh = y.TenQuyTrinh,
                        MaQuyTrinh = y.MaQuyTrinh,
                        HoatDong = y.HoatDong,
                        DoiTuongApDung = y.DoiTuongApDung,
                        CreatedDate = y.CreatedDate,
                        CreatedById = y.CreatedById
                    }).OrderByDescending(z => z.CreatedDate).ToList();

                var listCreatedId = ListQuyTrinh.Select(y => y.CreatedById).Distinct().ToList();
                var listCreated = context.User.Where(x => listCreatedId.Contains(x.UserId)).Select(y => new
                {
                    y.UserId,
                    y.EmployeeId
                }).ToList();
                var listEmployeeId = context.User.Where(x => listCreatedId.Contains(x.UserId)).Select(y => y.EmployeeId)
                    .ToList();
                var listEmployee = context.Employee.Where(x => listEmployeeId.Contains(x.EmployeeId)).Select(y => new
                {
                    y.EmployeeId,
                    y.EmployeeCode,
                    y.EmployeeName
                }).ToList();

                ListQuyTrinh.ForEach(item =>
                {
                    var user = listCreated.FirstOrDefault(x => x.UserId == item.CreatedById);
                    var emp = listEmployee.FirstOrDefault(x => x.EmployeeId == user?.EmployeeId);
                    item.NguoiTao = emp?.EmployeeCode + " - " + emp?.EmployeeName;
                    item.NgayTao = item.CreatedDate.ToString("dd/MM/yyyy");
                    item.TenDoiTuongApDung = GetDoiTuongApDung(item.DoiTuongApDung);
                });

                return new SearchQuyTrinhResult()
                {
                    Status = true,
                    Message = "Success",
                    ListQuyTrinh = ListQuyTrinh,
                    MessageCode = "success",
                    StatusCode = System.Net.HttpStatusCode.OK
                };
            }
            catch (Exception e)
            {
                return new SearchQuyTrinhResult()
                {
                    Status = false,
                    Message = e.Message,
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                };
            }
        }

        public GetMasterDataSearchQuyTrinhResult GetMasterDataSearchQuyTrinh(GetMasterDataSearchQuyTrinhParameter parameter)
        {
            try
            {
                var ListEmployee = new List<EmployeeEntityModel>();
                ListEmployee = context.Employee.Where(x => x.Active == true).Select(y => new EmployeeEntityModel
                {
                    EmployeeId = y.EmployeeId,
                    EmployeeCode = y.EmployeeCode,
                    EmployeeName = y.EmployeeName,
                    EmployeeCodeName = y.EmployeeCode + " - " + y.EmployeeName
                }).OrderBy(z => z.EmployeeName).ToList();

                return new GetMasterDataSearchQuyTrinhResult()
                {
                    MessageCode = "Success",
                    StatusCode = System.Net.HttpStatusCode.OK,
                    ListEmployee = ListEmployee
                };
            }
            catch (Exception e)
            {
                return new GetMasterDataSearchQuyTrinhResult()
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                };
            }
        }

        public GetDetailQuyTrinhResult GetDetailQuyTrinh(GetDetailQuyTrinhParameter parameter)
        {
            try
            {
                var QuyTrinh = new QuyTrinhModel();
                var ListCauHinhQuyTrinh = new List<CauHinhQuyTrinhModel>();
                var listDoiTuongApDung = GeneralList.GetTrangThais("DoiTuongApDungQuyTrinhPheDuyet");

                QuyTrinh = context.QuyTrinh.Where(x => x.Id == parameter.Id).Select(y => new QuyTrinhModel
                {
                    Id = y.Id,
                    TenQuyTrinh = y.TenQuyTrinh,
                    MaQuyTrinh = y.MaQuyTrinh,
                    CreatedDate = y.CreatedDate,
                    CreatedById = y.CreatedById,
                    MoTa = y.MoTa,
                    HoatDong = y.HoatDong,
                    DoiTuongApDung = y.DoiTuongApDung
                }).FirstOrDefault();

                if (QuyTrinh == null)
                {
                    return new GetDetailQuyTrinhResult()
                    {
                        Status = false,
                        Message = "Quy trình không tồn tại trên hệ thống",
                        MessageCode = "Quy trình không tồn tại trên hệ thống",
                        StatusCode = System.Net.HttpStatusCode.NotFound,
                    };
                }

                var userCreated = context.User.FirstOrDefault(x => x.UserId == QuyTrinh.CreatedById);
                var employeeCreated = context.Employee.FirstOrDefault(x => x.EmployeeId == userCreated.EmployeeId);
                QuyTrinh.NguoiTao = employeeCreated?.EmployeeCode + " - " + employeeCreated?.EmployeeName;
                QuyTrinh.NgayTao = QuyTrinh.CreatedDate.ToString("dd/MM/yyyy");

                ListCauHinhQuyTrinh = context.CauHinhQuyTrinh.Where(x => x.QuyTrinhId == QuyTrinh.Id).Select(y =>
                    new CauHinhQuyTrinhModel
                    {
                        Id = y.Id,
                        SoTienTu = y.SoTienTu,
                        TenCauHinh = y.TenCauHinh,
                        QuyTrinh = y.QuyTrinh,
                        ListCacBuocQuyTrinh = new List<CacBuocQuyTrinhModel>()
                    }).OrderBy(z => z.SoTienTu).ToList();

                var listCauHinhQuyTrinhId = ListCauHinhQuyTrinh.Select(y => y.Id).ToList();
                var listCacBuocQuyTrinh = context.CacBuocQuyTrinh
                    .Where(x => listCauHinhQuyTrinhId.Contains(x.CauHinhQuyTrinhId)).ToList();
                var listCacBuocQuyTrinhId = listCacBuocQuyTrinh.Select(y => y.Id).ToList();
                var listPhongBanTrongCacBuoc = context.PhongBanTrongCacBuocQuyTrinh
                    .Where(x => listCacBuocQuyTrinhId.Contains(x.CacBuocQuyTrinhId)).ToList();

                ListCauHinhQuyTrinh.ForEach(cauHinh =>
                {
                    cauHinh.ListCacBuocQuyTrinh = listCacBuocQuyTrinh.Where(x => x.CauHinhQuyTrinhId == cauHinh.Id)
                        .Select(y => new CacBuocQuyTrinhModel
                        {
                            Id = y.Id,
                            CauHinhQuyTrinhId = y.CauHinhQuyTrinhId,
                            LoaiPheDuyet = y.LoaiPheDuyet,
                            Stt = y.Stt,
                            ListPhongBanTrongCacBuocQuyTrinh = new List<PhongBanTrongCacBuocQuyTrinhModel>()
                        }).OrderBy(z => z.Stt).ToList();

                    cauHinh.ListCacBuocQuyTrinh.ForEach(buoc =>
                    {
                        buoc.ListPhongBanTrongCacBuocQuyTrinh = listPhongBanTrongCacBuoc
                            .Where(x => x.CacBuocQuyTrinhId == buoc.Id).Select(y =>
                                new PhongBanTrongCacBuocQuyTrinhModel
                                {
                                    Id = y.Id,
                                    OrganizationId = y.OrganizationId,
                                    CacBuocQuyTrinhId = y.CacBuocQuyTrinhId
                                }).ToList();
                    });
                });

                return new GetDetailQuyTrinhResult()
                {
                    Status = true,
                    Message = "Success",
                    MessageCode = "Success",
                    StatusCode = System.Net.HttpStatusCode.OK,
                    QuyTrinh = QuyTrinh,
                    ListCauHinhQuyTrinh = ListCauHinhQuyTrinh,
                    ListDoiTuongApDung = listDoiTuongApDung
                };
            }
            catch (Exception e)
            {
                return new GetDetailQuyTrinhResult()
                {
                    Status = false,
                    Message = e.Message,
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                };
            }
        }

        public UpdateQuyTrinhResult UpdateQuyTrinh(UpdateQuyTrinhParameter parameter)
        {
            try
            {
                var quyTrinh = context.QuyTrinh.FirstOrDefault(x => x.Id == parameter.QuyTrinh.Id);
                if (quyTrinh == null)
                {
                    return new UpdateQuyTrinhResult()
                    {
                        Status = false,
                        Message = "Quy trình không tồn tại trên hệ thống",
                        MessageCode = "Quy trình không tồn tại trên hệ thống",
                        StatusCode = System.Net.HttpStatusCode.NotFound,
                    };
                }

                var existQuyTrinh = context.QuyTrinh
    .FirstOrDefault(x => x.Id != quyTrinh.Id &&
                         x.DoiTuongApDung == parameter.QuyTrinh.DoiTuongApDung);

                if (existQuyTrinh != null)
                {
                    return new UpdateQuyTrinhResult()
                    {
                        MessageCode = "Đã tồn tại Quy trình cho đối tượng này",
                        StatusCode = System.Net.HttpStatusCode.Conflict
                    };
                }

                //Nếu quy trình có trạng thái Hoạt động
                if (parameter.QuyTrinh.HoatDong)
                {
                    //Update trạng thái Hoạt động các đối tượng áp dụng -> false
                    var listQuyTrinh = context.QuyTrinh
                        .Where(x => x.DoiTuongApDung == parameter.QuyTrinh.DoiTuongApDung).ToList();
                    listQuyTrinh.ForEach(item => { item.HoatDong = false; });
                    context.QuyTrinh.UpdateRange(listQuyTrinh);
                }

                if (quyTrinh.HoatDong && quyTrinh.HoatDong != parameter.QuyTrinh.HoatDong)
                {
                    return new UpdateQuyTrinhResult()
                    {
                        Status = false,
                        Message = "Cần có ít nhất một quy trình gắn với " + GetDoiTuongApDung(quyTrinh.DoiTuongApDung) + " hoạt động",
                        MessageCode = "Cần có ít nhất một quy trình gắn với " +
                                      GetDoiTuongApDung(quyTrinh.DoiTuongApDung) + " hoạt động",
                        StatusCode = System.Net.HttpStatusCode.NotFound,
                    };
                }

                /*
 * Kiểm tra xem quy trình có thay đổi hay không?
 * Nếu thay đổi thì tiền hành xóa CacBuocApDung của các đối tượng đang áp dụng quy trình
 * để áp dụng các bước thực hiện theo quy trình mới cập nhật
 */
                var listCode = new List<string>();
                CheckResetQuyTrinh(parameter.QuyTrinh, parameter.UserId,
                    parameter.ListCauHinhQuyTrinh, true, out listCode);

                quyTrinh.TenQuyTrinh = parameter.QuyTrinh.TenQuyTrinh;
                quyTrinh.DoiTuongApDung = parameter.QuyTrinh.DoiTuongApDung;
                quyTrinh.HoatDong = parameter.QuyTrinh.HoatDong;
                quyTrinh.MoTa = parameter.QuyTrinh.MoTa;

                context.QuyTrinh.Update(quyTrinh);

                #region Xóa cấu hình cũ

                var _listCauHinhQuyTrinh = context.CauHinhQuyTrinh.Where(x => x.QuyTrinhId == quyTrinh.Id).ToList();
                var _listCauHinhQuyTrinhId = _listCauHinhQuyTrinh.Select(y => y.Id).ToList();
                var _listCacBuoc = context.CacBuocQuyTrinh
                    .Where(x => _listCauHinhQuyTrinhId.Contains(x.CauHinhQuyTrinhId)).ToList();
                var _listCacBuocId = _listCacBuoc.Select(y => y.Id).ToList();
                var _listPhongBan = context.PhongBanTrongCacBuocQuyTrinh
                    .Where(x => _listCacBuocId.Contains(x.CacBuocQuyTrinhId)).ToList();

                context.CauHinhQuyTrinh.RemoveRange(_listCauHinhQuyTrinh);
                context.CacBuocQuyTrinh.RemoveRange(_listCacBuoc);
                context.PhongBanTrongCacBuocQuyTrinh.RemoveRange(_listPhongBan);

                #endregion

                #region Thêm cấu hình mới

                var listCauHinhQuyTrinh = new List<CauHinhQuyTrinh>();
                var listCacBuocQuyTrinh = new List<CacBuocQuyTrinh>();
                var listPhongBanTrongCacBuocQuyTrinh = new List<PhongBanTrongCacBuocQuyTrinh>();

                parameter.ListCauHinhQuyTrinh.ForEach(cauHinh =>
                {
                    var newCauHinh = new CauHinhQuyTrinh();
                    newCauHinh.Id = Guid.NewGuid();
                    newCauHinh.SoTienTu = cauHinh.SoTienTu;
                    newCauHinh.TenCauHinh = cauHinh.TenCauHinh;
                    newCauHinh.QuyTrinh = cauHinh.QuyTrinh;
                    newCauHinh.QuyTrinhId = quyTrinh.Id;

                    listCauHinhQuyTrinh.Add(newCauHinh);

                    cauHinh.ListCacBuocQuyTrinh.ForEach(buoc =>
                    {
                        var newBuoc = new CacBuocQuyTrinh();
                        newBuoc.Id = Guid.NewGuid();
                        newBuoc.Stt = buoc.Stt;
                        newBuoc.LoaiPheDuyet = buoc.LoaiPheDuyet;
                        newBuoc.CauHinhQuyTrinhId = newCauHinh.Id;

                        listCacBuocQuyTrinh.Add(newBuoc);

                        buoc.ListPhongBanTrongCacBuocQuyTrinh.ForEach(phongBan =>
                        {
                            var newPhongBan = new PhongBanTrongCacBuocQuyTrinh();
                            newPhongBan.Id = Guid.NewGuid();
                            newPhongBan.OrganizationId = phongBan.OrganizationId;
                            newPhongBan.CacBuocQuyTrinhId = newBuoc.Id;

                            listPhongBanTrongCacBuocQuyTrinh.Add(newPhongBan);
                        });
                    });
                });

                context.CauHinhQuyTrinh.AddRange(listCauHinhQuyTrinh);
                context.CacBuocQuyTrinh.AddRange(listCacBuocQuyTrinh);
                context.PhongBanTrongCacBuocQuyTrinh.AddRange(listPhongBanTrongCacBuocQuyTrinh);

                #endregion

                #region Xóa các bước áp dụng trong bảng PhongBanPheDuyetDoiTuong
                //Nếu đối mở thông tin thì xóa bảng phòng ban phê duyệt của đối tượng áp dụng để tiến hành thực hiện quy trình phê duyệt mới
                if (parameter.IsResetDoiTuong == true)
                {
                    var listPheDuyet = context.PhongBanPheDuyetDoiTuong.Where(x => x.DoiTuongApDung == quyTrinh.DoiTuongApDung).ToList();
                    context.PhongBanPheDuyetDoiTuong.RemoveRange(listPheDuyet);
                }
                #endregion

                context.SaveChanges();

                return new UpdateQuyTrinhResult()
                {
                    Status = true,
                    Message = "Lưu thành công",
                    MessageCode = "Lưu thành công",
                    StatusCode = System.Net.HttpStatusCode.OK,
                };
            }
            catch (Exception e)
            {
                return new UpdateQuyTrinhResult()
                {
                    Status = false,
                    Message = e.Message,
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                };
            }
        }

        public DeleteQuyTrinhResult DeleteQuyTrinh(DeleteQuyTrinhParameter parameter)
        {
            try
            {
                var quyTrinh = context.QuyTrinh.FirstOrDefault(x => x.Id == parameter.Id);
                if (quyTrinh == null)
                {
                    return new DeleteQuyTrinhResult()
                    {
                        Status = false,
                        Message = "Quy trình không tồn tại trên hệ thống",
                        MessageCode = "Quy trình không tồn tại trên hệ thống",
                        StatusCode = System.Net.HttpStatusCode.NotFound,
                    };
                }

                if (quyTrinh.HoatDong)
                {
                    return new DeleteQuyTrinhResult()
                    {
                        Status = false,
                        Message = "Không thể xóa Quy trình có trạng thái Hoạt động",
                        MessageCode = "Không thể xóa Quy trình có trạng thái Hoạt động",
                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed,

                    };
                }

                var listCauHinhQuyTrinh = context.CauHinhQuyTrinh.Where(x => x.QuyTrinhId == quyTrinh.Id).ToList();
                var listCauHinhQuyTrinhId = listCauHinhQuyTrinh.Select(y => y.Id).ToList();
                var listCacBuoc = context.CacBuocQuyTrinh
                    .Where(x => listCauHinhQuyTrinhId.Contains(x.CauHinhQuyTrinhId)).ToList();
                var listCacBuocId = listCacBuoc.Select(y => y.Id).ToList();
                var listPhongBan = context.PhongBanTrongCacBuocQuyTrinh
                    .Where(x => listCacBuocId.Contains(x.CacBuocQuyTrinhId)).ToList();

                context.QuyTrinh.Remove(quyTrinh);
                context.CauHinhQuyTrinh.RemoveRange(listCauHinhQuyTrinh);
                context.CacBuocQuyTrinh.RemoveRange(listCacBuoc);
                context.PhongBanTrongCacBuocQuyTrinh.RemoveRange(listPhongBan);
                context.SaveChanges();

                return new DeleteQuyTrinhResult()
                {
                    Status = true,
                    Message = "Xóa thành công",
                    MessageCode = "Xóa thành công",
                    StatusCode = System.Net.HttpStatusCode.OK,
                };
            }
            catch (Exception e)
            {
                return new DeleteQuyTrinhResult()
                {
                    Status = false,
                    Message = e.Message,
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                };
            }
        }

        public CheckTrangThaiQuyTrinhResult CheckTrangThaiQuyTrinh(CheckTrangThaiQuyTrinhParameter parameter)
        {
            try
            {
                bool exists = false;

                /* Tại một thời điểm: Đối với 1 loại đối tượng chỉ có một Quy trình có trạng thái Hoạt động */

                //Tạo mới
                if (parameter.Id == null)
                {
                    var count = context.QuyTrinh.Count(x => x.HoatDong && x.DoiTuongApDung == parameter.DoiTuongApDung);
                    
                    if (count >= 1)
                    {
                        exists = true;
                    }
                }
                //Cập nhật
                else
                {
                    var count = context.QuyTrinh.Count(x =>
                        x.HoatDong && x.DoiTuongApDung == parameter.DoiTuongApDung && x.Id != parameter.Id);

                    if (count >= 1)
                    {
                        exists = true;
                    }
                }

                return new CheckTrangThaiQuyTrinhResult()
                {
                    Status = true,
                    Message = "Success",
                    MessageCode = "Success",
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Exists = exists
                };
            }
            catch (Exception e)
            {
                return new CheckTrangThaiQuyTrinhResult()
                {
                    Status = false,
                    Message = e.Message,
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                };
            }
        }

        public GuiPheDuyetResult GuiPheDuyet(GuiPheDuyetParameter parameter)
        {
            var buoc1 = new CacBuocQuyTrinh();
            var note = new Note();
            try
            {
                //Kiểm tra Báo giá đã đc áp dụng quy trình chưa?
                int count = context.CacBuocApDung.Count(x => x.ObjectId == parameter.ObjectId &&
                                                     x.DoiTuongApDung == parameter.DoiTuongApDung);
                if (count > 0)
                {
                    return new GuiPheDuyetResult()
                    {
                        Status = false,
                        Message = "Đã được gửi phê duyệt",
                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                        MessageCode = "Đã được gửi phê duyệt"
                    };
                }

                #region Đăng ký quy trình

                //Lấy quy trình
                var quyTrinh = context.QuyTrinh.FirstOrDefault(x => x.HoatDong && 
                                                                    x.DoiTuongApDung == parameter.DoiTuongApDung);
                if (quyTrinh == null)
                {
                    return new GuiPheDuyetResult()
                    {
                        Status = false,
                        Message = "Chưa có quy trình phê duyệt",
                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                        MessageCode = "Chưa có quy trình phê duyệt"
                    };
                }
                //Chọn cấu hình quy trình
                var cauHinhQuyTrinh = context.CauHinhQuyTrinh.FirstOrDefault(x => x.QuyTrinhId == quyTrinh.Id && x.ServicePacketId == parameter.ServicePacketId);
                //Nếu không có cấu hình
                if (cauHinhQuyTrinh == null)
                {
                    return new GuiPheDuyetResult()
                    {
                        MessageCode = "Quy trình chưa có cấu hình quy trình phê duyệt",
                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    };
                }

                buoc1 = context.CacBuocQuyTrinh.FirstOrDefault(x =>
                    x.CauHinhQuyTrinhId == cauHinhQuyTrinh.Id && x.Stt == 1);
                if (buoc1 == null)
                {
                    return new GuiPheDuyetResult()
                    {
                        MessageCode = "Quy trình không tồn tại bước 1",
                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    };
                }

                //Kiểm tra trạng thái của đối tượng trước khi Gửi phê duyệt
                var checkTrangThai = KiemTraTrangThaiDoiTuongGuiPheDuyet(parameter.ObjectId, parameter.ObjectNumber,
                    parameter.DoiTuongApDung);
                if (!checkTrangThai)
                {
                    return new GuiPheDuyetResult()
                    {
                        MessageCode = "Trạng thái hiện tại không thể gửi phê duyệt",
                        StatusCode = System.Net.HttpStatusCode.Conflict,
                    };
                }

                //Thêm vào bảng mapping list phòng ban của người phê duyệt
                XuLyPhongBanPheDuyetDoiTuong(parameter.DoiTuongApDung, cauHinhQuyTrinh, parameter.ObjectNumber,
                    parameter.ObjectId);

                //Chuyển trạng thái đối tượng => Chờ phê duyệt
                ChuyenTrangThaiDoiTuong(parameter.ObjectId.Value, parameter.DoiTuongApDung, parameter.UserId, 1);

                //Thêm ghi chú
                ThemGhiChu(parameter.ObjectId.Value, parameter.DoiTuongApDung, parameter.UserId, 1, null);

                var cacBuocApDung = new CacBuocApDung();
                cacBuocApDung.Id = Guid.NewGuid();
                cacBuocApDung.ObjectId = parameter.ObjectId.Value;
                cacBuocApDung.DoiTuongApDung = parameter.DoiTuongApDung;
                cacBuocApDung.QuyTrinhId = quyTrinh.Id;
                cacBuocApDung.CauHinhQuyTrinhId = cauHinhQuyTrinh.Id;
                cacBuocApDung.CacBuocQuyTrinhId = buoc1.Id;
                cacBuocApDung.Stt = buoc1.Stt;
                cacBuocApDung.LoaiPheDuyet = buoc1.LoaiPheDuyet;
                cacBuocApDung.TrangThai = 0;
                cacBuocApDung.ObjectNumber = parameter.ObjectNumber;

                context.CacBuocApDung.Add(cacBuocApDung);
                context.SaveChanges();

                #endregion
            }
            catch (Exception e)
            {
                return new GuiPheDuyetResult()
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    Status = false,
                    Message = e.Message
                };
            }

            #region Gửi email

            //Gửi mail: áp dụng cho objectnumber != null
            //if (parameter.ObjectNumber != null && parameter.ObjectNumber != 0)
            //{
            //    //Lấy thông tin Người phê duyệt
            //    var listEmail = GetListEmail(parameter.UserId, buoc1.LoaiPheDuyet, buoc1);

            //    // Đề xuất cấp phát
            //    if (parameter.DoiTuongApDung == 20)
            //    {
            //        var deXuatCongTac = context.YeuCauCapPhatTaiSan.FirstOrDefault(x => x.YeuCauCapPhatTaiSanId == parameter.ObjectNumber);

            //        NotificationHelper.AccessNotification(context, TypeModel.DeXuatCapPhatTs, "DXCPTS_REQUESTORACCEPT",
            //            deXuatCongTac, deXuatCongTac, true, note, null, listEmail);
            //    }
            //}

            #endregion


            //Lấy listUserId cho người cần phê duyệt
            

            return new GuiPheDuyetResult()
            {
                MessageCode = "Gửi phê duyệt thành công",
                StatusCode = System.Net.HttpStatusCode.OK,
            };
        }

        public PheDuyetResult PheDuyet(PheDuyetParameter parameter)
        {
            object doiTuongModel = new object();
            string typeModel = "";
            string actionCode = "";
            bool isError = false;
            string message = "";
            int tienTrinhPheDuyet = -1; //1: Là bước cuối cùng, 0; Không phải là bước cuối cùng
            var listEmail = new List<string>();
            var listEmpId = new List<Guid>();

            try
            {
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                if (user == null)
                {
                    return new PheDuyetResult()
                    {
                        Status = false,
                        Message = "Người phê duyệt không tồn tại trên hệ thống",
                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                        MessageCode = "Người phê duyệt không tồn tại trên hệ thống"
                    };
                }

                //Nếu là phê duyệt phát sinh dịch vụ
                if (parameter.DoiTuongApDung == 31)
                {
                    var order = context.CustomerOrder.FirstOrDefault(x => x.OrderId == parameter.ObjectId);
                    if (order == null)
                    {
                        return new PheDuyetResult()
                        {
                            StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                            MessageCode = "Phiếu yêu cầu không tồn tại trên hệ thống!"
                        };
                    }

                    doiTuongModel = order;
                    typeModel = TypeModel.DeXuatTangLuongDetail;
                    actionCode = "SEND_APPROVAL"; // Gửi thông báo cần phê duyệt đến bước tiếp theo

                    XuLyPheDuyet(out isError, out message, out tienTrinhPheDuyet, out listEmail, user,
                        parameter.ObjectId, parameter.ObjectNumber, parameter.Mota, parameter.DoiTuongApDung,
                        doiTuongModel, out listEmpId);

                 
                        //Lấy list thông báo
                        var notificationCateTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == ProductConsts.CategoryTypeCodeNotificationConfig).CategoryTypeId;
                        var listNotifi = context.Category.Where(x => x.CategoryTypeId == notificationCateTypeId).ToList();
                        var listNotificationConfiguration = context.NotificationConfiguration.Where(x => x.ServicePacketId == order.ServicePacketId).ToList();
                        var listNotificationConfigurationId = listNotificationConfiguration.Select(x => x.Id).ToList();
                        var listEmpNoti = context.EmployeeMappingNotificationConfiguration.Where(x => listNotificationConfigurationId.Contains(x.NotificationConfigurationId.Value)).ToList();

                        //Lấy ra người cần nhận thông báo ở sự kiện phê duyệt bổ sung
                        var datDichVuId = listNotifi.FirstOrDefault(x => x.CategoryCode == "A").CategoryId;
                        var notiConfigure = listNotificationConfiguration.FirstOrDefault(x => x.CategoryId == datDichVuId);
                        listEmpId = listEmpNoti.Where(x => x.NotificationConfigurationId == notiConfigure.Id).Select(x => x.EmployeeId.Value).ToList();

                        //Thông báo
                        var notificationReceiver = CommonHelper.GetListEmployeeIdNotifi(context, order, notiConfigure, null, null);
                        var getListEmpId = notificationReceiver.ListEmployeeId;
                        var deviceId = context.User.Where(x => x.EmployeeId == notificationReceiver.CustomerId)?.FirstOrDefault()?.DeviceId;
                        listEmpId.AddRange(getListEmpId);
                        if (deviceId != null)
                        {
                            var body = "Phiếu " + order.OrderCode + ": " + message;
                            var title = "Phiếu " + order.OrderCode;
                            var type = 1; //1 : order, 2: orderAction

                            CommonHelper.PushNotificationToDevice(deviceId, title, body, type.ToString(), order.OrderId.ToString());
                        }

                    //Nếu là bước cuối cùng
                    if (tienTrinhPheDuyet == 1)
                    {
                        //Cập nhật phiếu yêu cầu sang trạng thái Khách hàng phê duyệt chi phí phát sinh (3)
                        order.StatusOrder = GeneralList.GetTrangThais("CustomerOrder").FirstOrDefault(x => x.Value == 3).Value;
                        context.CustomerOrder.Update(order);
                    }

                    if (isError)
                    {
                        return new PheDuyetResult()
                        {
                            StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                            MessageCode = message
                        };
                    }
                }

                //Nếu là phê duyệt phiếu yêu cầu bổ sung
                if (parameter.DoiTuongApDung == 32)
                {
                    var order = context.CustomerOrder.FirstOrDefault(x => x.OrderId == parameter.ObjectId);
                    if (order == null)
                    {
                        return new PheDuyetResult()
                        {
                            StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                            MessageCode = "Đề xuất tăng lương không tồn tại trên hệ thống"
                        };
                    }

                    doiTuongModel = order;
                    typeModel = TypeModel.DeXuatTangLuongDetail;
                    actionCode = "SEND_APPROVAL"; // Gửi thông báo cần phê duyệt đến bước tiếp theo

                    XuLyPheDuyet(out isError, out message, out tienTrinhPheDuyet, out listEmail, user,
                        parameter.ObjectId, parameter.ObjectNumber, parameter.Mota, parameter.DoiTuongApDung,
                        doiTuongModel, out listEmpId);

                    //Nếu là bước cuối cùng
                    if (tienTrinhPheDuyet == 1)
                    {
                        //Cập nhật phiếu yêu cầu sang trạng thái chờ thanh toán (4)
                        order.StatusOrder = GeneralList.GetTrangThais("CustomerOrder").FirstOrDefault(x => x.Value == 4).Value;
                        context.CustomerOrder.Update(order);
                    }

                    if (isError)
                    {
                        return new PheDuyetResult()
                        {
                            StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                            MessageCode = message
                        };
                    }
                }


                context.SaveChanges();
            }
            catch (Exception e)
            {
                return new PheDuyetResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    Status = false,
                    Message = e.Message,
                    MessageCode = e.Message
                };
            }

            //#region Gửi email khi phê duyệt

            ////Nếu là bước cuối cùng
            //if (tienTrinhPheDuyet == 1)
            //{
            //    //Gửi mail phê duyệt cho người phụ trách (người tạo) đối tượng
            //    NotificationHelper.AccessNotification(context, typeModel, actionCode,
            //        doiTuongModel, doiTuongModel, true, null, null, new List<string>());
            //}
            ////Nếu không phải là bước cuối cùng
            //else if (tienTrinhPheDuyet == 0)
            //{
            //    //Gửi mail phê duyệt cho người phê duyệt của đối tượng áp dụng
            //    NotificationHelper.AccessNotification(context, typeModel, actionCode,
            //        doiTuongModel, doiTuongModel, true, null, null, listEmail);
            //}

            //#endregion

            return new PheDuyetResult()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                ListEmpId = listEmpId.Distinct().ToList(),
                Status = true,
                Message = "Phê duyệt thành công",
                MessageCode = "Phê duyệt thành công"
            };
        }

        public HuyYeuCauPheDuyetResult HuyYeuCauPheDuyet(HuyYeuCauPheDuyetParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                if (user == null)
                {
                    return new HuyYeuCauPheDuyetResult()
                    {
                        Status = false,
                        Message = "Người dùng không tồn tại trên hệ thống",
                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                        MessageCode = "Người dùng không tồn tại trên hệ thống"
                    };
                }

                //Cơ hội
                if (parameter.DoiTuongApDung == 1)
                {

                }
                //Hồ sơ thầu
                else if (parameter.DoiTuongApDung == 2)
                {

                }
                //Báo giá
                else if (parameter.DoiTuongApDung == 3)
                {
                    var quote = context.Quote.FirstOrDefault(x => x.QuoteId == parameter.ObjectId);
                    if (quote == null)
                    {
                        return new HuyYeuCauPheDuyetResult()
                        {
                            Status = false,
                            Message = "Báo giá không tồn tại trên hệ thống",
                            StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                            MessageCode = "Báo giá không tồn tại trên hệ thống"
                        };
                    }

                    //Xóa hết các bước áp dụng
                    var listCacBuocApDung = context.CacBuocApDung.Where(x => x.ObjectId == parameter.ObjectId &&
                                                                             x.DoiTuongApDung ==
                                                                             parameter.DoiTuongApDung).ToList();
                    context.CacBuocApDung.RemoveRange(listCacBuocApDung);

                    //Xóa hết các phòng ban áp dụng
                    var listCacBuocApDungId = listCacBuocApDung.Select(y => y.Id).ToList();
                    var listPhongBanApDung = context.PhongBanApDung
                        .Where(x => listCacBuocApDungId.Contains(x.CacBuocApDungId)).ToList();
                    context.PhongBanApDung.RemoveRange(listPhongBanApDung);

                    ChuyenTrangThaiDoiTuong(parameter.ObjectId, parameter.DoiTuongApDung,
                        parameter.UserId, 4);

                    ThemGhiChu(parameter.ObjectId, parameter.DoiTuongApDung, parameter.UserId, 4, null);

                    context.SaveChanges();
                }
                //Hợp đồng
                else if (parameter.DoiTuongApDung == 4)
                {

                }
                //Đơn hàng bán
                else if (parameter.DoiTuongApDung == 5)
                {

                }
                //Hóa đơn
                else if (parameter.DoiTuongApDung == 6)
                {

                }
                //Đề xuất mua hàng
                else if (parameter.DoiTuongApDung == 7)
                {

                }
                //Đơn hàng mua
                else if (parameter.DoiTuongApDung == 8)
                {

                }
                //Đề xuất xin nghỉ
                else if (parameter.DoiTuongApDung == 9)
                {

                }
                //Đề xuất tăng lương
                else if (parameter.DoiTuongApDung == 10)
                {
                    var deXuatTangLuong = context.DeXuatTangLuong.FirstOrDefault(x => x.DeXuatTangLuongId == parameter.ObjectNumber.Value);
                    if (deXuatTangLuong == null)
                    {
                        return new HuyYeuCauPheDuyetResult()
                        {
                            StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                            MessageCode = "Đề xuất tăng lương không tồn tại trên hệ thống"
                        };
                    }

                    #region Gửi email cho người phê duyệt ở bước hiện tại trước khi reset quy trình

                    //Lấy bước hiện tại
                    var buocHienTai = context.CacBuocApDung.FirstOrDefault(x =>
                        x.TrangThai == 0 && x.ObjectNumber == parameter.ObjectNumber &&
                        x.DoiTuongApDung == parameter.DoiTuongApDung);

                    var buoc = context.CacBuocQuyTrinh
                        .FirstOrDefault(x => x.CauHinhQuyTrinhId == buocHienTai.CauHinhQuyTrinhId &&
                                             x.Stt == buocHienTai.Stt);

                    //Lấy thông tin Người phê duyệt
                    var listEmail = GetListEmail(parameter.UserId, buoc.LoaiPheDuyet, buoc);

                    //Gửi mail thông báo đã hủy phê duyệt cho đề xuất tăng lương.
                    NotificationHelper.AccessNotification(context, TypeModel.DeXuatTangLuongDetail, "CANCEL_APPROVAL",
                        deXuatTangLuong, deXuatTangLuong, true, null, null, listEmail);

                    #endregion

                    //Xóa hết các bước áp dụng
                    var listCacBuocApDung = context.CacBuocApDung.Where(x => x.ObjectNumber == parameter.ObjectNumber.Value &&
                                                                             x.DoiTuongApDung ==
                                                                             parameter.DoiTuongApDung).ToList();
                    context.CacBuocApDung.RemoveRange(listCacBuocApDung);

                    //Xóa hết các phòng ban áp dụng
                    var listCacBuocApDungId = listCacBuocApDung.Select(y => y.Id).ToList();
                    var listPhongBanApDung = context.PhongBanApDung
                        .Where(x => listCacBuocApDungId.Contains(x.CacBuocApDungId)).ToList();
                    context.PhongBanApDung.RemoveRange(listPhongBanApDung);

                    ChuyenTrangThaiDoiTuong(parameter.ObjectId, parameter.DoiTuongApDung,
                        parameter.UserId, 4);

                    ThemGhiChu(parameter.ObjectId, parameter.DoiTuongApDung, parameter.UserId, 4, null);

                    context.SaveChanges();
                }
                //Đề xuất chức vụ
                else if (parameter.DoiTuongApDung == 11)
                {
                    var deXuatChucVu = context.DeXuatThayDoiChucVu.FirstOrDefault(x => x.DeXuatThayDoiChucVuId == parameter.ObjectNumber.Value);
                    if (deXuatChucVu == null)
                    {
                        return new HuyYeuCauPheDuyetResult()
                        {
                            StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                            MessageCode = "Đề xuất thay đổi chức vụ không tồn tại trên hệ thống"
                        };
                    }

                    #region Gửi email cho người phê duyệt ở bước hiện tại trước khi reset quy trình

                    //Lấy bước hiện tại
                    var buocHienTai = context.CacBuocApDung.FirstOrDefault(x =>
                        x.TrangThai == 0 && x.ObjectNumber == parameter.ObjectNumber &&
                        x.DoiTuongApDung == parameter.DoiTuongApDung);

                    var buoc = context.CacBuocQuyTrinh
                        .FirstOrDefault(x => x.CauHinhQuyTrinhId == buocHienTai.CauHinhQuyTrinhId &&
                                             x.Stt == buocHienTai.Stt);

                    //Lấy thông tin Người phê duyệt
                    var listEmail = GetListEmail(parameter.UserId, buoc.LoaiPheDuyet, buoc);

                    //Gửi mail thông báo đã hủy phê duyệt cho đề xuất chức vụ.
                    NotificationHelper.AccessNotification(context, TypeModel.DeXuatChucVuDetail, "CANCEL_APPROVAL",
                        deXuatChucVu, deXuatChucVu, true, null, null, listEmail);

                    #endregion

                    //Xóa hết các bước áp dụng
                    var listCacBuocApDung = context.CacBuocApDung.Where(x => x.ObjectNumber == parameter.ObjectNumber.Value &&
                                                                             x.DoiTuongApDung ==
                                                                             parameter.DoiTuongApDung).ToList();
                    context.CacBuocApDung.RemoveRange(listCacBuocApDung);

                    //Xóa hết các phòng ban áp dụng
                    var listCacBuocApDungId = listCacBuocApDung.Select(y => y.Id).ToList();
                    var listPhongBanApDung = context.PhongBanApDung
                        .Where(x => listCacBuocApDungId.Contains(x.CacBuocApDungId)).ToList();
                    context.PhongBanApDung.RemoveRange(listPhongBanApDung);

                    ChuyenTrangThaiDoiTuong(parameter.ObjectId, parameter.DoiTuongApDung,
                        parameter.UserId, 4);

                    ThemGhiChu(parameter.ObjectId, parameter.DoiTuongApDung, parameter.UserId, 4, null);

                    context.SaveChanges();
                }
                //Đề xuất kế hoạch OT
                else if (parameter.DoiTuongApDung == 12)
                {
                    var deXuatKeHoachOT = context.KeHoachOt.FirstOrDefault(x => x.KeHoachOtId == parameter.ObjectNumber.Value);
                    if (deXuatKeHoachOT == null)
                    {
                        return new HuyYeuCauPheDuyetResult()
                        {
                            StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                            MessageCode = "Đề xuất kế hoạch OT không tồn tại trên hệ thống"
                        };
                    }

                    #region Gửi email cho người phê duyệt ở bước hiện tại trước khi reset quy trình

                    //Lấy bước hiện tại
                    var buocHienTai = context.CacBuocApDung.FirstOrDefault(x =>
                        x.TrangThai == 0 && x.ObjectNumber == parameter.ObjectNumber &&
                        x.DoiTuongApDung == parameter.DoiTuongApDung);

                    var buoc = context.CacBuocQuyTrinh
                        .FirstOrDefault(x => x.CauHinhQuyTrinhId == buocHienTai.CauHinhQuyTrinhId &&
                                             x.Stt == buocHienTai.Stt);

                    //Lấy thông tin Người phê duyệt
                    var listEmail = GetListEmail(parameter.UserId, buoc.LoaiPheDuyet, buoc);

                    //Gửi mail thông báo đã hủy phê duyệt cho đề xuất kế hoạch OT
                    NotificationHelper.AccessNotification(context, TypeModel.DeXuatKeHoachOTDetail, "CANCEL_APPROVAL", deXuatKeHoachOT,
                        deXuatKeHoachOT, true, null, null, listEmail);

                    #endregion

                    //Xóa hết các bước áp dụng
                    var listCacBuocApDung = context.CacBuocApDung.Where(x => x.ObjectNumber == parameter.ObjectNumber.Value &&
                                                                             x.DoiTuongApDung ==
                                                                             parameter.DoiTuongApDung).ToList();
                    context.CacBuocApDung.RemoveRange(listCacBuocApDung);

                    //Xóa hết các phòng ban áp dụng
                    var listCacBuocApDungId = listCacBuocApDung.Select(y => y.Id).ToList();
                    var listPhongBanApDung = context.PhongBanApDung
                        .Where(x => listCacBuocApDungId.Contains(x.CacBuocApDungId)).ToList();
                    context.PhongBanApDung.RemoveRange(listPhongBanApDung);

                    ChuyenTrangThaiDoiTuong(parameter.ObjectId, parameter.DoiTuongApDung,
                        parameter.UserId, 4);

                    ThemGhiChu(parameter.ObjectId, parameter.DoiTuongApDung, parameter.UserId, 4, null);

                    context.SaveChanges();
                }
                //Đề xuất đăng ký OT
                else if (parameter.DoiTuongApDung == 13)
                {
                    var deXuatKeHoachOT = context.KeHoachOt.FirstOrDefault(x => x.KeHoachOtId == parameter.ObjectNumber.Value);
                    if (deXuatKeHoachOT == null)
                    {
                        return new HuyYeuCauPheDuyetResult()
                        {
                            StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                            MessageCode = "Đề xuất kế hoạch OT không tồn tại trên hệ thống"
                        };
                    }

                    #region Gửi email cho người phê duyệt ở bước hiện tại trước khi reset quy trình

                    //Lấy bước hiện tại
                    var buocHienTai = context.CacBuocApDung.FirstOrDefault(x =>
                        x.TrangThai == 0 && x.ObjectNumber == parameter.ObjectNumber &&
                        x.DoiTuongApDung == parameter.DoiTuongApDung);

                    var buoc = context.CacBuocQuyTrinh
                        .FirstOrDefault(x => x.CauHinhQuyTrinhId == buocHienTai.CauHinhQuyTrinhId &&
                                             x.Stt == buocHienTai.Stt);

                    //Lấy thông tin Người phê duyệt
                    var listEmail = GetListEmail(parameter.UserId, buoc.LoaiPheDuyet, buoc);

                    //Gửi mail thông báo đã hủy phê duyệt cho đề xuất đăng ký OT.
                    NotificationHelper.AccessNotification(context, TypeModel.DeXuatDangKyOTDetail, "CANCEL_APPROVAL",
                        deXuatKeHoachOT, deXuatKeHoachOT, true, null, null, listEmail);

                    #endregion

                    //Xóa hết các bước áp dụng
                    var listCacBuocApDung = context.CacBuocApDung.Where(x => x.ObjectNumber == parameter.ObjectNumber.Value &&
                                                                             x.DoiTuongApDung ==
                                                                             parameter.DoiTuongApDung).ToList();
                    context.CacBuocApDung.RemoveRange(listCacBuocApDung);

                    //Xóa hết các phòng ban áp dụng
                    var listCacBuocApDungId = listCacBuocApDung.Select(y => y.Id).ToList();
                    var listPhongBanApDung = context.PhongBanApDung
                        .Where(x => listCacBuocApDungId.Contains(x.CacBuocApDungId)).ToList();
                    context.PhongBanApDung.RemoveRange(listPhongBanApDung);

                    ChuyenTrangThaiDoiTuong(parameter.ObjectId, parameter.DoiTuongApDung,
                        parameter.UserId, 4);

                    ThemGhiChu(parameter.ObjectId, parameter.DoiTuongApDung, parameter.UserId, 4, null);

                    context.SaveChanges();
                }
                //Đề xuất công tác
                else if (parameter.DoiTuongApDung == 30)
                {
                    var deXuatCongTac = context.DeXuatCongTac.FirstOrDefault(x => x.DeXuatCongTacId == parameter.ObjectNumber.Value);
                    if (deXuatCongTac == null)
                    {
                        return new HuyYeuCauPheDuyetResult()
                        {
                            StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                            MessageCode = "Đề xuất công tác không tồn tại trên hệ thống"
                        };
                    }

                    //Xóa hết các bước áp dụng
                    var listCacBuocApDung = context.CacBuocApDung.Where(x => x.ObjectNumber == parameter.ObjectNumber.Value &&
                                                                             x.DoiTuongApDung ==
                                                                             parameter.DoiTuongApDung).ToList();
                    context.CacBuocApDung.RemoveRange(listCacBuocApDung);

                    //Xóa hết các phòng ban áp dụng
                    var listCacBuocApDungId = listCacBuocApDung.Select(y => y.Id).ToList();
                    var listPhongBanApDung = context.PhongBanApDung
                        .Where(x => listCacBuocApDungId.Contains(x.CacBuocApDungId)).ToList();
                    context.PhongBanApDung.RemoveRange(listPhongBanApDung);

                    ChuyenTrangThaiDoiTuong(parameter.ObjectId, parameter.DoiTuongApDung,
                        parameter.UserId, 4);

                    ThemGhiChu(parameter.ObjectId, parameter.DoiTuongApDung, parameter.UserId, 4, null);

                    context.SaveChanges();
                }
                //Yêu cầu cấp phát
                else if (parameter.DoiTuongApDung == 20)
                {
                    var yeuCau = context.YeuCauCapPhatTaiSan.FirstOrDefault(x => x.YeuCauCapPhatTaiSanId == parameter.ObjectNumber.Value);
                    if (yeuCau == null)
                    {
                        return new HuyYeuCauPheDuyetResult()
                        {
                            StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                            MessageCode = "Yêu cầu cấp phát không tồn tại trên hệ thống"
                        };
                    }

                    //Xóa hết các bước áp dụng
                    var listCacBuocApDung = context.CacBuocApDung.Where(x => x.ObjectNumber == parameter.ObjectNumber.Value &&
                                                                             x.DoiTuongApDung ==
                                                                             parameter.DoiTuongApDung).ToList();
                    context.CacBuocApDung.RemoveRange(listCacBuocApDung);

                    //Xóa hết các phòng ban áp dụng
                    var listCacBuocApDungId = listCacBuocApDung.Select(y => y.Id).ToList();
                    var listPhongBanApDung = context.PhongBanApDung
                        .Where(x => listCacBuocApDungId.Contains(x.CacBuocApDungId)).ToList();
                    context.PhongBanApDung.RemoveRange(listPhongBanApDung);

                    ChuyenTrangThaiDoiTuong(parameter.ObjectId, parameter.DoiTuongApDung,
                        parameter.UserId, 4);

                    ThemGhiChu(parameter.ObjectId, parameter.DoiTuongApDung, parameter.UserId, 4, null);

                    context.SaveChanges();
                }
                // Đề nghị tạm ứng (21) hoặc Đề nghị hoàn ứng (22)
                else if (parameter.DoiTuongApDung == 21 || parameter.DoiTuongApDung == 22)
                {
                    var deNghi = context.DeNghiTamHoanUng.FirstOrDefault(x => x.DeNghiTamHoanUngId == parameter.ObjectNumber.Value);
                    if (deNghi == null)
                    {
                        return new HuyYeuCauPheDuyetResult()
                        {
                            StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                            MessageCode = "Yêu cầu cấp phát không tồn tại trên hệ thống"
                        };
                    }

                    //Xóa hết các bước áp dụng
                    var listCacBuocApDung = context.CacBuocApDung.Where(x => x.ObjectNumber == parameter.ObjectNumber.Value &&
                                                                             x.DoiTuongApDung ==
                                                                             parameter.DoiTuongApDung).ToList();
                    context.CacBuocApDung.RemoveRange(listCacBuocApDung);

                    //Xóa hết các phòng ban áp dụng
                    var listCacBuocApDungId = listCacBuocApDung.Select(y => y.Id).ToList();
                    var listPhongBanApDung = context.PhongBanApDung
                        .Where(x => listCacBuocApDungId.Contains(x.CacBuocApDungId)).ToList();
                    context.PhongBanApDung.RemoveRange(listPhongBanApDung);

                    ChuyenTrangThaiDoiTuong(parameter.ObjectId, parameter.DoiTuongApDung,
                        parameter.UserId, 4);

                    ThemGhiChu(parameter.ObjectId, parameter.DoiTuongApDung, parameter.UserId, 4, null);

                    context.SaveChanges();
                }

                return new HuyYeuCauPheDuyetResult()
                {
                    Status = true,
                    Message = "Hủy yêu cầu phê duyệt thành công",
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "Hủy yêu cầu phê duyệt thành công"
                };
            }
            catch (Exception e)
            {
                return new HuyYeuCauPheDuyetResult()
                {
                    Status = false,
                    Message = e.Message,
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public TuChoiResult TuChoi(TuChoiParameter parameter)
        {
            var doiTuongModel = new object();
            var typeModel = "";
            var actionCode = "";
            var listEmpId = new List<Guid>();
            try
            {
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                if (user == null)
                {
                    return new TuChoiResult()
                    {
                        Status = false,
                        Message = "Người dùng không tồn tại trên hệ thống",
                        StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                        MessageCode = "Người dùng không tồn tại trên hệ thống"
                    };
                }

                //Phê duyệt phát sinh dịch vụ
                if (parameter.DoiTuongApDung == 31)
                {
                    var order =
                        context.CustomerOrder.FirstOrDefault(x => x.OrderId == parameter.ObjectId);
                    if (order == null)
                    {
                        return new TuChoiResult()
                        {
                            StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                            MessageCode = "Phiếu yêu cầu không tồn tại trên hệ thống!"
                        };
                    }

                    //Chuyển orderprocess gán với order thành đã từ chối dịch vụ phát sinh
                    var deniedStatus = GeneralList.GetTrangThais("OrderProcess").FirstOrDefault(x => x.Value == 4).Value;
                    var orderProcess = context.OrderProcess.FirstOrDefault(x => x.Id == order.OrderProcessId);
                    if(orderProcess != null)
                    {
                        orderProcess.Status = deniedStatus;
                        context.OrderProcess.Update(orderProcess);
                        context.SaveChanges();
                    }

                    doiTuongModel = order;
                    typeModel = TypeModel.DeXuatTangLuongDetail;
                    actionCode = "REJECT";
                }
                

                var listCacBuocApDung = new List<CacBuocApDung>();

                //Xóa hết các bước áp dụng
                if (parameter.ObjectId != Guid.Empty)
                {
                    listCacBuocApDung = context.CacBuocApDung.Where(x =>
                        x.ObjectId == parameter.ObjectId &&
                        x.DoiTuongApDung ==
                        parameter.DoiTuongApDung).ToList();
                    context.CacBuocApDung.RemoveRange(listCacBuocApDung);
                }
                else if (parameter.ObjectNumber != null && parameter.ObjectNumber != 0)
                {
                    listCacBuocApDung = context.CacBuocApDung.Where(x =>
                        x.ObjectNumber == parameter.ObjectNumber.Value &&
                        x.DoiTuongApDung ==
                        parameter.DoiTuongApDung).ToList();
                    context.CacBuocApDung.RemoveRange(listCacBuocApDung);
                }

                //Xóa hết các phòng ban áp dụng
                var listCacBuocApDungId = listCacBuocApDung.Select(y => y.Id).ToList();
                var listPhongBanApDung = context.PhongBanApDung
                    .Where(x => listCacBuocApDungId.Contains(x.CacBuocApDungId)).ToList();
                context.PhongBanApDung.RemoveRange(listPhongBanApDung);

                listEmpId = ChuyenTrangThaiDoiTuong(parameter.ObjectId, parameter.DoiTuongApDung,
                    parameter.UserId, 3);

                ThemGhiChu(parameter.ObjectId, parameter.DoiTuongApDung, parameter.UserId, 3, parameter.Mota);

                //Thêm vào lịch sử
                var lichSuPheDuyet = new LichSuPheDuyet();
                lichSuPheDuyet.Id = Guid.NewGuid();
                lichSuPheDuyet.ObjectId = parameter.ObjectId;
                lichSuPheDuyet.DoiTuongApDung = parameter.DoiTuongApDung;
                lichSuPheDuyet.NgayTao = DateTime.Now;
                lichSuPheDuyet.EmployeeId = user.EmployeeId.Value;
                lichSuPheDuyet.OrganizationId = null;
                lichSuPheDuyet.LyDo = parameter.Mota;
                lichSuPheDuyet.TrangThai = 0;
                lichSuPheDuyet.ObjectNumber = parameter.ObjectNumber;

                context.LichSuPheDuyet.Add(lichSuPheDuyet);
                context.SaveChanges();
            }
            catch (Exception e)
            {
                return new TuChoiResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    Status = false,
                    Message = e.Message,
                    MessageCode = e.Message
                };
            }

            return new TuChoiResult()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Status = true,
                ListEmpId = listEmpId.Distinct().ToList(),
                Message = "Từ chối thành công",
                MessageCode = "Từ chối thành công"
            };
        }

        public GetLichSuPheDuyetResult GetLichSuPheDuyet(GetLichSuPheDuyetParameter parameter)
        {
            try
            {
                var ListLichSuPheDuyet = new List<LichSuPheDuyetModel>();

                ListLichSuPheDuyet = context.LichSuPheDuyet
                    .Where(x => x.ObjectId == parameter.ObjectId && x.DoiTuongApDung == parameter.DoiTuongApDung)
                    .Select(y => new LichSuPheDuyetModel
                    {
                        Id = y.Id,
                        NgayTao = y.NgayTao,
                        EmployeeId = y.EmployeeId,
                        OrganizationId = y.OrganizationId,
                        LyDo = y.LyDo,
                        TrangThai = y.TrangThai
                    }).OrderByDescending(z => z.NgayTao).ToList();

                var listEmployeeId = ListLichSuPheDuyet.Select(y => y.EmployeeId).ToList();
                var listEmployee = context.Employee.Where(x => listEmployeeId.Contains(x.EmployeeId))
                    .Select(y => new {y.EmployeeId, y.EmployeeCode, y.EmployeeName}).ToList();
                var listDonViId = ListLichSuPheDuyet.Select(y => y.OrganizationId).ToList();
                var listDonVi = context.Organization.Where(x => listDonViId.Contains(x.OrganizationId))
                    .Select(y => new {y.OrganizationId, y.OrganizationName}).ToList();

                ListLichSuPheDuyet.ForEach(item =>
                {
                    item.NgayTaoString = item.NgayTao.ToString("dd/MM/yyyy HH:mm");
                    item.TenTrangThai = item.TrangThai == 0 ? "Từ chối" : "Phê duyệt";

                    var nguoiPheDuyet = listEmployee.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                    item.NguoiPheDuyet = nguoiPheDuyet?.EmployeeCode + " - " + nguoiPheDuyet?.EmployeeName;

                    var donVi = listDonVi.FirstOrDefault(x => x.OrganizationId == item.OrganizationId);
                    item.TenDonVi = donVi?.OrganizationName;
                });

                return new GetLichSuPheDuyetResult()
                {
                    Status = true,
                    Message = "Success",
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListLichSuPheDuyet = ListLichSuPheDuyet
                };
            }
            catch (Exception e)
            {
                return new GetLichSuPheDuyetResult()
                {
                    Status = false,
                    Message = e.Message,
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetDuLieuQuyTrinhResult GetDuLieuQuyTrinh(GetDuLieuQuyTrinhParameter parameter)
        {
            try
            {
                var listDonVi = context.Organization.Select(y => new { y.OrganizationId, y.OrganizationName })
                    .ToList();
                var listDuLieuQuyTrinh = new List<DuLieuQuyTrinhModel>();

                //Phê duyệt phát sinh
                if (parameter.DoiTuongApDung == 31)
                {
                    //Lấy quy trình
                    var quyTrinh = context.QuyTrinh.FirstOrDefault(x => x.HoatDong &&
                                                                        x.DoiTuongApDung == parameter.DoiTuongApDung);

                    if (quyTrinh != null)
                    {
                        //Chọn cấu hình quy trình
                        var cauHinhQuyTrinh = context.CauHinhQuyTrinh.FirstOrDefault(x => x.QuyTrinhId == quyTrinh.Id && x.ServicePacketId == parameter.ServicePacketId);

                        //Sau khi lấy được cấu hình quy trình
                        if (cauHinhQuyTrinh != null)
                        {
                            listDuLieuQuyTrinh = GetListDuLieuQuyTrinhByObject(parameter.ObjectId, parameter.ObjectNumber,
                                parameter.DoiTuongApDung, cauHinhQuyTrinh.Id);
                        }
                    }
                }

                return new GetDuLieuQuyTrinhResult()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Message = "Success",
                    MessageCode = "Success",
                    ListDuLieuQuyTrinh = listDuLieuQuyTrinh
                };
            }
            catch (Exception e)
            {
                return new GetDuLieuQuyTrinhResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    Message = e.Message,
                    MessageCode = e.Message
                };
            }
        }

        public GetMasterDataCreateQuyTrinhResult GetMasterDataCreateQuyTrinh(GetMasterDataCreateQuyTrinhParameter parameter)
        {
            try
            {
                var listDoiTuongApDung = GeneralList.GetTrangThais("DoiTuongApDungQuyTrinhPheDuyet");

                return new GetMasterDataCreateQuyTrinhResult()
                {
                    MessageCode = "OK",
                    StatusCode = System.Net.HttpStatusCode.OK,
                    ListDoiTuongApDung = listDoiTuongApDung
                };
            }
            catch (Exception e)
            {
                return new GetMasterDataCreateQuyTrinhResult()
                {
                    MessageCode = e.Message,
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed
                };
            }
        }

        public CheckUpdateQuyTrinhResult CheckUpdateQuyTrinh(CheckUpdateQuyTrinhParameter parameter)
        {
            try
            {
                var listCode = new List<string>();
                bool checkChange = CheckResetQuyTrinh(parameter.QuyTrinh, parameter.UserId,
                    parameter.ListCauHinhQuyTrinh, false, out listCode);

                return new CheckUpdateQuyTrinhResult()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    MessageCode = "OK",
                    IsResetDoiTuong = checkChange,
                    ListDoiTuong = listCode
                };
            }
            catch (Exception e)
            {
                return new CheckUpdateQuyTrinhResult()
                {
                    StatusCode = System.Net.HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        private string GenCode()
        {
            var code = "";
            var max = 0;
            var listMaQuyTrinh = context.QuyTrinh.Select(y => y.MaQuyTrinh.Substring(3)).ToList();
            var listNumber = listMaQuyTrinh.Select(y => Int32.Parse(y)).ToList();

            if (listNumber.Count == 0)
            {
                max = 1;
            }
            else
            {
                var maxCurrent = listNumber.OrderByDescending(z => z).FirstOrDefault();
                max = maxCurrent + 1;
            }

            if (max <= 9999)
            {
                code = "QT-" + max.ToString("D4");
            }
            else
            {
                code = "QT-" + max;
            }

            return code;
        }

        private string GetDoiTuongApDung(int code)
        {
            var listDoiTuongApDung = GeneralList.GetTrangThais("DoiTuongApDungQuyTrinhPheDuyet");
            return listDoiTuongApDung.FirstOrDefault(x => x.Value == code).Name;
        }

        private decimal TongThanhToanBaoGia(
            bool loaiChietKhau, decimal giaTriChietKhau, List<SanPhamBaoGia> listSp, decimal tongChiPhi)
        {
            decimal tongGiaTriHangHoaBanRa = 0;
            decimal tongThue = 0;

            listSp.ForEach(item =>
            {
                decimal thanhTienChietKhau = 0;

                //Chiết khấu theo %
                if (item.LoaiChietKhau)
                {
                    thanhTienChietKhau = (item.SoLuong * item.DonGia * item.TyGia + item.ThanhTienNhanCong) *
                                         item.GiaTriChietKhau / 100;
                }
                //Chiết khấu theo số tiền
                else
                {
                    thanhTienChietKhau = item.GiaTriChietKhau;
                }

                tongGiaTriHangHoaBanRa += (item.SoLuong * item.DonGia * item.TyGia) + item.ThanhTienNhanCong -
                                          thanhTienChietKhau;

                tongThue += ((item.SoLuong * item.DonGia * item.TyGia) + item.ThanhTienNhanCong -
                             thanhTienChietKhau) * item.PhanTramThue / 100;
            });

            decimal tongTienSauThue = tongChiPhi + tongGiaTriHangHoaBanRa + tongThue;
            decimal thanhTienChietKhauBaoGia = 0;

            //Chiết khấu theo %
            if (loaiChietKhau)
            {
                thanhTienChietKhauBaoGia = tongTienSauThue * giaTriChietKhau / 100;
            }
            //Chiết khấu theo số tiền
            else
            {
                thanhTienChietKhauBaoGia = giaTriChietKhau;
            }

            decimal tongThanhToan = tongTienSauThue - thanhTienChietKhauBaoGia;

            return tongThanhToan;
        }

        private List<Guid> ChuyenTrangThaiDoiTuong(Guid ObjectId, int DoiTuongApDung, Guid UserId, int Action)
        {
            /*
             * Action = 1: Gửi phê duyệt
             * Action = 2: Phê duyệt
             * Action = 3: Từ chối
             * Action = 4: Hủy yêu cầu phê duyệt
             */

            var listEmp = new List<Guid>();
            //Phê duyệt phát sinh dịch vụ
            if (DoiTuongApDung == 31)
            {
                var order = context.CustomerOrder.FirstOrDefault(x => x.OrderId == ObjectId);

                //Nếu là gửi phê dyệt
                if (Action == 1)
                {
                    //Nếu là phiếu yêu cầu
                    if (order.OrderType == 1)
                    {
                        order.StatusOrder = GeneralList.GetTrangThais("CustomerOrder").FirstOrDefault(x => x.Value == 11).Value;
                    }
                    //Nếu là phiếu bổ sung
                    else
                    {
                        order.StatusOrder = GeneralList.GetTrangThais("CustomerOrder").FirstOrDefault(x => x.Value == 12).Value;
                    }
                    context.CustomerOrder.Update(order);
                }
                //Nếu là phê dyệt
                if (Action == 2)
                {
                    //Chuyển các phát sinh chờ phê duyệt thành đã duyệt
                    var listOrderDetailExten = context.CustomerOrderDetailExten.Where(x => x.OrderId == ObjectId && x.Status == 3).ToList();
                    listOrderDetailExten.ForEach(item => item.Status = 2);
                    context.CustomerOrderDetailExten.UpdateRange(listOrderDetailExten);
                }


                //Nếu là trạng thái từ chối => chuyển phiếu sang trạng thái từ chối dịch vụ phát sinh (7) 
                if (Action == 3)
                {
                    order.StatusOrder = GeneralList.GetTrangThais("CustomerOrder").FirstOrDefault(x => x.Value == 7).Value;
                    context.CustomerOrder.Update(order);

                    //Lấy list thông báo
                    var notificationCateTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == ProductConsts.CategoryTypeCodeNotificationConfig).CategoryTypeId;
                    var listNotifi = context.Category.Where(x => x.CategoryTypeId == notificationCateTypeId).ToList();

                    var listNotificationConfiguration = context.NotificationConfiguration.Where(x => x.ServicePacketId == order.ServicePacketId).ToList();
                    var listNotificationConfigurationId = listNotificationConfiguration.Select(x => x.Id).ToList();
                    var listEmpNoti = context.EmployeeMappingNotificationConfiguration.Where(x => listNotificationConfigurationId.Contains(x.NotificationConfigurationId.Value)).ToList();

                    //Lấy ra người cần nhận thông báo ở sự kiện từ chối dịch vụ bổ sung hoặc thường
                    var datDichVuId = listNotifi.FirstOrDefault(x => x.CategoryCode == "B").CategoryId;
                    var listEmpId = new List<Guid>();
                    var notiConfigure = listNotificationConfiguration.FirstOrDefault(x => x.CategoryId == datDichVuId);
                    listEmp = listEmpNoti.Where(x => x.NotificationConfigurationId == notiConfigure.Id).Select(x => x.EmployeeId.Value).ToList();
                    //Thông báo
                    var notificationReceiver = CommonHelper.GetListEmployeeIdNotifi(context, order, notiConfigure, null, null);
                    var getListEmpId = notificationReceiver.ListEmployeeId;
                    var deviceId = context.User.Where(x => x.EmployeeId == notificationReceiver.CustomerId)?.FirstOrDefault()?.DeviceId;
                    listEmpId.AddRange(getListEmpId);
                    if (deviceId != null)
                    {
                        var body = "Phiếu " + order.OrderCode + ": " + "từ chối từ chối phê duyệt";
                        var title = "Phiếu " + order.OrderCode;
                        var type = 1; //1 : order, 2: orderAction

                        CommonHelper.PushNotificationToDevice(deviceId, title, body, type.ToString(), order.OrderId.ToString());
                    }
                }
            }
            return listEmp;
        }

        public void ThemGhiChu(Guid ObjectId, int DoiTuongApDung, Guid UserId, int Action, string Mota )
        {
            /*
             * Action = 1: Gửi phê duyệt
             * Action = 2: Phê duyệt
             * Action = 3: Từ chối
             * Action = 4: Hủy yêu cầu phê duyệt (hoặc Đặt về mới)
             */

            Note note = new Note();
            note.NoteId = Guid.NewGuid();
            note.ObjectId = ObjectId;
            note.Type = "ADD";
            note.Active = true;
            note.CreatedById = UserId;
            note.CreatedDate = DateTime.Now;
            note.NoteTitle = "Đã thêm ghi chú";

            //Phê duyệt dịch vụ phát sinh || Phê duyệt phiếu yêu cầu phát sinh
            if (DoiTuongApDung == 31 || DoiTuongApDung == 32)
            {
                note.ObjectType = NoteObjectType.ORDER;

                if (Action == 1)
                {
                    note.Description = "Đã gửi phê duyệt thành công";
                }
                else if (Action == 2)
                {
                    note.Description = "Đã phê duyệt thành công";

                    if (!string.IsNullOrEmpty(Mota))
                    {
                        note.Description = "Đã phê duyệt thành công với lý do: " +
                                           Mota.Trim();
                    }
                }
                else if (Action == 3)
                {
                    note.Description = "Đã bị từ chối";

                    if (!string.IsNullOrEmpty(Mota))
                    {
                        note.Description = "Đã bị từ chối với lý do: " +
                                           Mota.Trim();
                    }
                }
                else if (Action == 4)
                {
                    note.Description = "Đã đặt về mới";
                }

                context.Note.Add(note);
            }
        }

        private List<string> GetListEmail(Guid userId, int loaiPheDuyet, CacBuocQuyTrinh buoc)
        {
            var result = new List<string>();

            // phê duyệt trưởng bộ phận
            if (loaiPheDuyet == 1)
            {
                //lấy email trưởng bộ phận của nv gửi phê duyệt
                var userInfor = context.User.FirstOrDefault(x => x.UserId == userId);
                var empOrg = context.Employee.FirstOrDefault(x => x.EmployeeId == userInfor.EmployeeId)?.OrganizationId;
                var listEmpIdTruongBoPhan = context.ThanhVienPhongBan
                    .Where(x => x.IsManager == 1 &&
                                x.OrganizationId == empOrg).Select(y => y.EmployeeId).ToList();

                //Lấy mail từ bảng contact 
                var listEmailTruongBoPhan =
                    context.Contact.Where(x => listEmpIdTruongBoPhan.Contains(x.ObjectId)).ToList();

                listEmailTruongBoPhan.ForEach(item =>
                {
                    if (!String.IsNullOrEmpty(item.WorkEmail))
                    {
                        result.Add(item.WorkEmail.Trim());
                    }
                    else if (!String.IsNullOrEmpty(item.Email))
                    {
                        result.Add(item.Email.Trim());
                    }
                });
            }
            // phê duyệt phòng ban
            else if (loaiPheDuyet == 2)
            {
                //Lấy thông tin về các phòng ban phê duyệt
                var listPhongBanPheDuyetTrongBuoc = context.PhongBanTrongCacBuocQuyTrinh
                    .Where(x => x.CacBuocQuyTrinhId == buoc.Id).Select(y => y.OrganizationId).ToList();

                var listEmpIdTruongBoPhan = context.ThanhVienPhongBan.Where(x =>
                    listPhongBanPheDuyetTrongBuoc.Contains(x.OrganizationId) &&
                    x.IsManager == 1).Select(y => y.EmployeeId).ToList();

                //Lấy mail từ bảng contact 
                var listEmailTruongBoPhan =
                    context.Contact.Where(x => listEmpIdTruongBoPhan.Contains(x.ObjectId)).ToList();

                listEmailTruongBoPhan.ForEach(item =>
                {
                    if (!String.IsNullOrEmpty(item.WorkEmail))
                    {
                        result.Add(item.WorkEmail.Trim());
                    }
                    else if (!String.IsNullOrEmpty(item.Email))
                    {
                        result.Add(item.Email.Trim());
                    }
                });
            }

            result = result.Where(x => x != null).Select(y => y).Distinct().ToList();

            return result;
        }
        private List<Guid> GetListEmpId(Guid userId, int loaiPheDuyet, CacBuocQuyTrinh buoc, Guid objectId)
        {
            var result = new List<Guid>();

            // phê duyệt trưởng bộ phận
            if (loaiPheDuyet == 1)
            {
                //lấy email trưởng bộ phận của nv gửi phê duyệt
                var userInfor = context.User.FirstOrDefault(x => x.UserId == userId);
                var empOrg = context.Employee.FirstOrDefault(x => x.EmployeeId == userInfor.EmployeeId)?.OrganizationId;
                var listEmpIdTruongBoPhan = context.ThanhVienPhongBan
                    .Where(x => x.IsManager == 1 &&
                                x.OrganizationId == empOrg).Select(y => y.EmployeeId).ToList();

                result.AddRange(listEmpIdTruongBoPhan);
            }
            // phê duyệt phòng ban
            else if (loaiPheDuyet == 2)
            {
                //Lấy thông tin về các phòng ban phê duyệt
                var listPhongBanPheDuyetTrongBuoc = context.PhongBanTrongCacBuocQuyTrinh
                    .Where(x => x.CacBuocQuyTrinhId == buoc.Id).Select(y => y.OrganizationId).ToList();

                var listEmpIdTruongBoPhan = context.ThanhVienPhongBan.Where(x =>
                    listPhongBanPheDuyetTrongBuoc.Contains(x.OrganizationId) &&
                    x.IsManager == 1).Select(y => y.EmployeeId).ToList();

                result.AddRange(listEmpIdTruongBoPhan);
            }
            result = result.Where(x => x != null).Select(y => y).Distinct().ToList();

            return result;
        }


        private List<DuLieuQuyTrinhModel> GetListDuLieuQuyTrinhByObject(Guid objectId, int? objectNumber, int doiTuongApDung, Guid cauHinhQuyTrinhId)
        {
            var list = new List<DuLieuQuyTrinhModel>();
            var listCacBuocHienTai = new List<CacBuocApDung>();

            var listDonVi = context.Organization.Select(y => new { y.OrganizationId, y.OrganizationName })
                .ToList();

            if (objectId != Guid.Empty)
            {
                listCacBuocHienTai = context.CacBuocApDung.Where(x => x.ObjectId == objectId &&
                                                                          x.DoiTuongApDung == doiTuongApDung)
                    .OrderBy(z => z.Stt).ToList();
            }
            else if (objectNumber != null && objectNumber != 0)
            {
                listCacBuocHienTai = context.CacBuocApDung.Where(x => x.ObjectNumber == objectNumber &&
                                                                      x.DoiTuongApDung == doiTuongApDung)
                    .OrderBy(z => z.Stt).ToList();
            }

            var listCacBuoc = context.CacBuocQuyTrinh
                    .Where(x => x.CauHinhQuyTrinhId == cauHinhQuyTrinhId)
                    .OrderBy(z => z.Stt).ToList();

            var sttBuocApDungCuoi = listCacBuocHienTai.Count;

            var listCacBuocId = listCacBuoc.Select(y => y.Id).ToList();
            var listPhongBanTrongCacBuoc = context.PhongBanTrongCacBuocQuyTrinh
                .Where(x => listCacBuocId.Contains(x.CacBuocQuyTrinhId)).ToList();
            var listlistCacBuocHienTaiId = listCacBuocHienTai.Select(y => y.Id).ToList();
            var listPhongBanDaPheDuyet = context.PhongBanApDung
                .Where(x => listlistCacBuocHienTaiId.Contains(x.CacBuocApDungId)).ToList();

            listCacBuoc.ForEach(buoc =>
            {
                var buocApDung = listCacBuocHienTai.FirstOrDefault(x => x.Stt == buoc.Stt);

                var duLieu = new DuLieuQuyTrinhModel();

                //Nếu có dữ liệu
                if (buocApDung != null)
                {
                    //Nếu bước đã đc hoàn thành
                    if (buocApDung.TrangThai == 1)
                    {
                        duLieu.IsComplete = true;
                    }
                    //Nếu bước chưa hoàn thành
                    else
                    {
                        //Bước hiện tại
                        if (buoc.Stt == sttBuocApDungCuoi)
                        {
                            duLieu.IsCurrent = true;
                            duLieu.IsActive = true;
                        }
                    }
                }

                //Phê duyệt trưởng bộ phận
                if (buoc.LoaiPheDuyet == 1)
                {
                    duLieu.NodeName = "Phê duyệt trưởng bộ phận";
                    duLieu.Tooltip = duLieu.NodeName;
                }
                //Phòng ban phê duyệt
                else if (buoc.LoaiPheDuyet == 2)
                {
                    //Lấy list Phòng ban
                    var listPhongBan = listPhongBanTrongCacBuoc.Where(x => x.CacBuocQuyTrinhId == buoc.Id)
                        .ToList();

                    //Nếu bước chỉ có 1 phòng ban phê duyệt
                    if (listPhongBan.Count == 1)
                    {
                        var donViId = listPhongBan.Select(y => y.OrganizationId).FirstOrDefault();
                        var donVi = listDonVi.FirstOrDefault(x => x.OrganizationId == donViId);
                        duLieu.NodeName = donVi?.OrganizationName;
                        duLieu.Tooltip = duLieu.NodeName;
                    }
                    //Nếu bước có nhiều hơn 1 phòng ban phê duyệt
                    else if (listPhongBan.Count > 1)
                    {
                        duLieu.NodeName = "Phòng ban phê duyệt";

                        listPhongBan.ForEach(donViPheDuyet =>
                        {
                            var newDonViPheDuyet = new DuLieuPhongBanPheDuyetModel();
                            newDonViPheDuyet.OrganizationId = donViPheDuyet.OrganizationId;
                            newDonViPheDuyet.TrangThai = 0;

                            var donVi = listDonVi.FirstOrDefault(x =>
                                x.OrganizationId == donViPheDuyet.OrganizationId);
                            newDonViPheDuyet.TenDonVi = donVi?.OrganizationName;

                            duLieu.ListDonVi.Add(newDonViPheDuyet);
                        });

                        //Nếu có dữ liệu
                        if (buocApDung != null)
                        {
                            var listDonViDaDuyet = listPhongBanDaPheDuyet
                                .Where(x => x.CacBuocApDungId == buocApDung.Id)
                                .ToList();

                            duLieu.ListDonVi.ForEach(donVi =>
                            {
                                var donViDaDuyet = listDonViDaDuyet.FirstOrDefault(x =>
                                    x.OrganizationId == donVi.OrganizationId);

                                if (donViDaDuyet != null)
                                {
                                    donVi.TrangThai = 1;
                                }
                            });
                        }

                        duLieu.Tooltip = duLieu.ListDonVi
                            .Select(y =>
                                y.TrangThai == 0
                                    ? "<p>- " + y.TenDonVi + ": Chưa phê duyệt</p>"
                                    : "<p>- " + y.TenDonVi + ": Đã phê duyệt</p>")
                            .ToArray().Join(" ");
                    }
                }
                //Quản lý gói phê duyệt
                else if (buoc.LoaiPheDuyet == 3)
                {
                    duLieu.NodeName = "Phê duyệt quản lý gói dịch vụ";
                    duLieu.Tooltip = duLieu.NodeName;
                }
                list.Add(duLieu);
            });

            return list;
        }

        private void XuLyPheDuyet(out bool isError, out string message, out int tienTrinhPheDuyet, out List<string> listEmail,
            User user, Guid objectId, int? objectNumber, string moTa, int doiTuongApDung, object doiTuongModel, out List<Guid> listEmpId)
        {
            isError = false;
            message = "";
            tienTrinhPheDuyet = -1; //1: Là bước cuối cùng, 0; Không phải là bước cuối cùng
            listEmail = new List<string>();
            listEmpId = new List<Guid>();

            //Lấy bước hiện tại của Đối tượng
            var buocHienTai = new CacBuocApDung();

            if (objectId != Guid.Empty)
            {
                buocHienTai = context.CacBuocApDung.Where(x => x.ObjectId == objectId &&
                                                               x.DoiTuongApDung == doiTuongApDung &&
                                                               x.TrangThai == 0)
                    .OrderByDescending(z => z.Stt)
                    .FirstOrDefault();
            }
            else if (objectNumber != null && objectNumber != 0)
            {
                buocHienTai = context.CacBuocApDung.Where(x => x.ObjectNumber == objectNumber &&
                                                               x.DoiTuongApDung == doiTuongApDung &&
                                                               x.TrangThai == 0)
                    .OrderByDescending(z => z.Stt)
                    .FirstOrDefault();
            }

            if (buocHienTai == null)
            {
                isError = true;
                message = "Phê duyệt thất bại, không tồn tại bước phê duyệt";
                return;
            }

            //Lấy Quy trình
            var quyTrinh = context.QuyTrinh.FirstOrDefault(x => x.Id == buocHienTai.QuyTrinhId);
            if (quyTrinh == null)
            {
                isError = true;
                message = "Quy trình không tồn tại";
                return;
            }

            //Lấy list Các bước trong Quy trình theo Cấu hình
            var listCacBuoc = context.CacBuocQuyTrinh
                .Where(x => x.CauHinhQuyTrinhId == buocHienTai.CauHinhQuyTrinhId).OrderByDescending(z => z.Stt)
                .ToList();

            int tongSoBuoc = listCacBuoc.Count;

            //Nếu là phê duyệt trưởng bộ phận
            if (buocHienTai.LoaiPheDuyet == 1)
            {
                if (buocHienTai.TrangThai == 1)
                {
                    isError = true;
                    message = "Bước hiện tại được phê duyệt";
                    return;
                }

                //Đổi trạng thái của bước hiện tại => Đã xong
                buocHienTai.TrangThai = 1;
                context.CacBuocApDung.Update(buocHienTai);

                //Nếu đây là bước cuối cùng của Quy trình => Phê duyệt
                if (buocHienTai.Stt == tongSoBuoc)
                {
                    tienTrinhPheDuyet = 1;
                    ChuyenTrangThaiDoiTuong(objectId, doiTuongApDung, user.UserId, 2);
                }
                //Nếu không phải bước cuối cùng
                else
                {
                    tienTrinhPheDuyet = 0;

                    #region Các case xử lý đặc biệt theo đối tượng áp dụng

                    XuLyDacBietKhiPheDuyet(doiTuongApDung, doiTuongModel, user.UserId);

                    #endregion

                    var buocTiepTheo = listCacBuoc.FirstOrDefault(x => x.Stt == buocHienTai.Stt + 1);

                    //Lấy thông tin Người phê duyệt
                    listEmail = GetListEmail(user.UserId, buocTiepTheo.LoaiPheDuyet, buocTiepTheo);
                    listEmpId = GetListEmpId(user.UserId, buocTiepTheo.LoaiPheDuyet, buocTiepTheo, objectId);

                    //Thêm bước tiếp vào lịch sử
                    var cacBuocApDung = new CacBuocApDung();
                    cacBuocApDung.Id = Guid.NewGuid();
                    cacBuocApDung.ObjectId = objectId;
                    cacBuocApDung.DoiTuongApDung = doiTuongApDung;
                    cacBuocApDung.QuyTrinhId = quyTrinh.Id;
                    cacBuocApDung.CauHinhQuyTrinhId = buocTiepTheo.CauHinhQuyTrinhId;
                    cacBuocApDung.CacBuocQuyTrinhId = buocTiepTheo.Id;
                    cacBuocApDung.Stt = buocTiepTheo.Stt;
                    cacBuocApDung.LoaiPheDuyet = buocTiepTheo.LoaiPheDuyet;
                    cacBuocApDung.TrangThai = 0;
                    cacBuocApDung.ObjectNumber = objectNumber;
                    context.CacBuocApDung.Add(cacBuocApDung);
                }

                //Thêm vào lịch sử
                var lichSuPheDuyet = new LichSuPheDuyet();
                lichSuPheDuyet.Id = Guid.NewGuid();
                lichSuPheDuyet.ObjectId = objectId;
                lichSuPheDuyet.DoiTuongApDung = doiTuongApDung;
                lichSuPheDuyet.NgayTao = DateTime.Now;
                lichSuPheDuyet.EmployeeId = user.EmployeeId.Value;
                lichSuPheDuyet.OrganizationId = null;
                lichSuPheDuyet.LyDo = moTa;
                lichSuPheDuyet.TrangThai = 1;
                lichSuPheDuyet.ObjectNumber = objectNumber;

                context.LichSuPheDuyet.Add(lichSuPheDuyet);
            }
            //Nếu là phòng ban phê duyệt
            else if (buocHienTai.LoaiPheDuyet == 2)
            {
                //Lấy các phòng ban đã phê duyệt bước hiện tại
                var listDonViIdDaPheDuyet = context.PhongBanApDung
                    .Where(x => x.CacBuocApDungId == buocHienTai.Id &&
                                x.CacBuocQuyTrinhId == buocHienTai.CacBuocQuyTrinhId)
                    .Select(y => y.OrganizationId).ToList();

                //Lấy các phòng chưa phê duyệt ở bước hiện tại
                var listDonViId = context.PhongBanTrongCacBuocQuyTrinh
                    .Where(x => x.CacBuocQuyTrinhId == buocHienTai.CacBuocQuyTrinhId &&
                                !listDonViIdDaPheDuyet.Contains(x.OrganizationId))
                    .Select(y => y.OrganizationId).ToList();

                //Lấy phòng ban mà người phê duyệt là Trưởng bộ phận
                var listDonViId_NguoiPheDuyet =
                    context.ThanhVienPhongBan.Where(x => x.EmployeeId == user.EmployeeId &&
                                                         x.IsManager == 1)
                        .Select(y => y.OrganizationId).ToList();

                //Lấy phòng ban sẽ phê duyệt bước hiện tại
                var listDonViIdPheDuyet = listDonViId_NguoiPheDuyet.Where(x => listDonViId.Contains(x)).ToList();
                if (listDonViIdPheDuyet.Count == 0)
                {
                    isError = true;
                    message = "Phê duyệt thất bại, phòng ban người dùng không hợp lệ";
                    return;
                }

                var listPhongBanApDung = new List<PhongBanApDung>();
                var listLichSuPheDuyet = new List<LichSuPheDuyet>();

                listDonViIdPheDuyet.ForEach(donViId =>
                {
                    var phongBanApDung = new PhongBanApDung();
                    phongBanApDung.Id = Guid.NewGuid();
                    phongBanApDung.CacBuocApDungId = buocHienTai.Id;
                    phongBanApDung.OrganizationId = donViId;
                    phongBanApDung.CacBuocQuyTrinhId = buocHienTai.CacBuocQuyTrinhId;

                    listPhongBanApDung.Add(phongBanApDung);

                    var lichSuPheDuyet = new LichSuPheDuyet();
                    lichSuPheDuyet.Id = Guid.NewGuid();
                    lichSuPheDuyet.ObjectId = objectId;
                    lichSuPheDuyet.DoiTuongApDung = doiTuongApDung;
                    lichSuPheDuyet.NgayTao = DateTime.Now;
                    lichSuPheDuyet.EmployeeId = user.EmployeeId.Value;
                    lichSuPheDuyet.OrganizationId = donViId;
                    lichSuPheDuyet.LyDo = moTa;
                    lichSuPheDuyet.TrangThai = 1;
                    lichSuPheDuyet.ObjectNumber = objectNumber;

                    listLichSuPheDuyet.Add(lichSuPheDuyet);
                });

                context.PhongBanApDung.AddRange(listPhongBanApDung);
                context.LichSuPheDuyet.AddRange(listLichSuPheDuyet);

                // Nếu tất cả phòng ban đều đã phê duyệt:
                // (Số phòng ban chưa phê duyệt == Số phòng ban phê duyệt ở bước hiện tại)
                if (listDonViId.Count == listPhongBanApDung.Count)
                {
                    //Đổi trạng thái của bước hiện tại => Đã xong
                    buocHienTai.TrangThai = 1;
                    context.CacBuocApDung.Update(buocHienTai);

                    //Nếu đây là bước cuối cùng của Quy trình => Phê duyệt
                    if (buocHienTai.Stt == tongSoBuoc)
                    {
                        tienTrinhPheDuyet = 1;
                        ChuyenTrangThaiDoiTuong(objectId, doiTuongApDung, user.UserId, 2);
                    }
                    //Nếu không phải bước cuối cùng
                    else
                    {
                        tienTrinhPheDuyet = 0;

                        #region Các case xử lý đặc biệt theo đối tượng áp dụng

                        XuLyDacBietKhiPheDuyet(doiTuongApDung, doiTuongModel, user.UserId);

                        #endregion

                        var buocTiepTheo = listCacBuoc.FirstOrDefault(x => x.Stt == buocHienTai.Stt + 1);

                        //Lấy thông tin Người phê duyệt
                        listEmail = GetListEmail(user.UserId, buocTiepTheo.LoaiPheDuyet, buocTiepTheo);

                        //Thêm bước tiếp vào lịch sử
                        var cacBuocApDung = new CacBuocApDung();
                        cacBuocApDung.Id = Guid.NewGuid();
                        cacBuocApDung.ObjectId = objectId;
                        cacBuocApDung.DoiTuongApDung = doiTuongApDung;
                        cacBuocApDung.QuyTrinhId = quyTrinh.Id;
                        cacBuocApDung.CauHinhQuyTrinhId = buocTiepTheo.CauHinhQuyTrinhId;
                        cacBuocApDung.CacBuocQuyTrinhId = buocTiepTheo.Id;
                        cacBuocApDung.Stt = buocTiepTheo.Stt;
                        cacBuocApDung.LoaiPheDuyet = buocTiepTheo.LoaiPheDuyet;
                        cacBuocApDung.TrangThai = 0;
                        cacBuocApDung.ObjectNumber = objectNumber;

                        context.CacBuocApDung.Add(cacBuocApDung);
                    }
                }
            }
            //Nếu là phê duyệt quản lý gói
            else if (buocHienTai.LoaiPheDuyet == 3)
            {
                if (buocHienTai.TrangThai == 1)
                {
                    isError = true;
                    message = "Bước hiện tại được phê duyệt";
                    return;
                }

                //Đổi trạng thái của bước hiện tại => Đã xong
                buocHienTai.TrangThai = 1;
                context.CacBuocApDung.Update(buocHienTai);

                //Nếu đây là bước cuối cùng của Quy trình => Phê duyệt
                if (buocHienTai.Stt == tongSoBuoc)
                {
                    tienTrinhPheDuyet = 1;
                    ChuyenTrangThaiDoiTuong(objectId, doiTuongApDung, user.UserId, 2);
                }
                //Nếu không phải bước cuối cùng
                else
                {
                    tienTrinhPheDuyet = 0;

                    #region Các case xử lý đặc biệt theo đối tượng áp dụng

                    XuLyDacBietKhiPheDuyet(doiTuongApDung, doiTuongModel, user.UserId);

                    #endregion

                    var buocTiepTheo = listCacBuoc.FirstOrDefault(x => x.Stt == buocHienTai.Stt + 1);

                    //Lấy thông tin Người phê duyệt
                    listEmail = GetListEmail(user.UserId, buocTiepTheo.LoaiPheDuyet, buocTiepTheo);

                    //Thêm bước tiếp vào lịch sử
                    var cacBuocApDung = new CacBuocApDung();
                    cacBuocApDung.Id = Guid.NewGuid();
                    cacBuocApDung.ObjectId = objectId;
                    cacBuocApDung.DoiTuongApDung = doiTuongApDung;
                    cacBuocApDung.QuyTrinhId = quyTrinh.Id;
                    cacBuocApDung.CauHinhQuyTrinhId = buocTiepTheo.CauHinhQuyTrinhId;
                    cacBuocApDung.CacBuocQuyTrinhId = buocTiepTheo.Id;
                    cacBuocApDung.Stt = buocTiepTheo.Stt;
                    cacBuocApDung.LoaiPheDuyet = buocTiepTheo.LoaiPheDuyet;
                    cacBuocApDung.TrangThai = 0;
                    cacBuocApDung.ObjectNumber = objectNumber;
                    context.CacBuocApDung.Add(cacBuocApDung);
                }

                //Thêm vào lịch sử
                var lichSuPheDuyet = new LichSuPheDuyet();
                lichSuPheDuyet.Id = Guid.NewGuid();
                lichSuPheDuyet.ObjectId = objectId;
                lichSuPheDuyet.DoiTuongApDung = doiTuongApDung;
                lichSuPheDuyet.NgayTao = DateTime.Now;
                lichSuPheDuyet.EmployeeId = user.EmployeeId.Value;
                lichSuPheDuyet.OrganizationId = null;
                lichSuPheDuyet.LyDo = moTa;
                lichSuPheDuyet.TrangThai = 1;
                lichSuPheDuyet.ObjectNumber = objectNumber;

                context.LichSuPheDuyet.Add(lichSuPheDuyet);
            }

            ThemGhiChu(objectId, doiTuongApDung, user.UserId, 2, moTa);

            context.SaveChanges();
        }

        private void XuLyDacBietKhiPheDuyet(int doiTuongApDung, object doiTuongModel, Guid userId)
        {
            //Đề xuất tăng lương
            if (doiTuongApDung == 10)
            {
                #region Chuyển trạng thái nhân viên trong đề xuất

                var deXuatTangLuong = doiTuongModel as DeXuatTangLuong;

                var listTrangThaiNV = GeneralList.GetTrangThais("DXTangLuongNhanVien");
                var deXuatTangLuongNV = context.DeXuatTangLuongNhanVien
                    .Where(x => x.DeXuatTangLuongId == deXuatTangLuong.DeXuatTangLuongId).ToList();

                //Danh sách nhân viên được đề xuất
                deXuatTangLuongNV.ForEach(item =>
                {
                    //Chuyển trạng thái nhân viên Phê duyệt => Chờ phê duyệt sau khi quy trình chuyển sang bước mới
                    if (item.TrangThai == 3)
                    {
                        item.TrangThai = (byte)listTrangThaiNV.FirstOrDefault(x => x.Value == 2).Value;
                    }
                    item.UpdatedById = userId;
                    item.UpdatedDate = DateTime.Now;
                });

                context.DeXuatTangLuongNhanVien.UpdateRange(deXuatTangLuongNV);

                #endregion
            }
            //Đề xuất thay đổi chức vụ
            else if (doiTuongApDung == 11)
            {
                #region Chuyển trạng thái nhân viên trong đề xuất

                var deXuatThayDoiChucVu = doiTuongModel as DeXuatThayDoiChucVu;

                var listTrangThaiNV = GeneralList.GetTrangThais("DXThayDoiChucVuNhanVien");
                var listNhanVienDeXuat = context.NhanVienDeXuatThayDoiChucVu
                    .Where(x => x.DeXuatThayDoiChucVuId == deXuatThayDoiChucVu.DeXuatThayDoiChucVuId).ToList();

                //Danh sách nhân viên được đề xuất
                listNhanVienDeXuat.ForEach(item =>
                {
                    //Chuyển trạng thái nhân viên Phê duyệt => Chờ phê duyệt sau khi quy trình chuyển sang bước mới
                    if (item.TrangThai == 3)
                    {
                        item.TrangThai = (byte)listTrangThaiNV.FirstOrDefault(x => x.Value == 2).Value;
                    }
                    item.UpdatedById = userId;
                    item.UpdatedDate = DateTime.Now;
                });

                context.NhanVienDeXuatThayDoiChucVu.UpdateRange(listNhanVienDeXuat);

                #endregion
            }
            //Đề xuất dịch vụ phát sinh
            else if (doiTuongApDung == 31)
            {
                #region Chuyển trạng thái dịch vụ phát sinh trong đề xuất

                var order = doiTuongModel as CustomerOrder;
                var listOrderDetailExtend = context.CustomerOrderDetailExten.Where(x => x.OrderId == order.OrderId && x.Status == 2).ToList();
                listOrderDetailExtend.ForEach(item => item.Status = 3);
                context.CustomerOrderDetailExten.UpdateRange(listOrderDetailExtend);
                #endregion
            }

        }

        private bool KiemTraTrangThaiDoiTuongGuiPheDuyet(Guid? ObjectId, int? ObjectNumber, int DoiTuongApDung)
        {
            bool result = true;

            switch (DoiTuongApDung)
            {
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    break;
                case 6:
                    break;
                case 7:
                    break;
                case 8:
                    break;
                case 9:

                    #region Đề xuất xin nghỉ

                    var dxxn = context.DeXuatXinNghi.FirstOrDefault(x => x.DeXuatXinNghiId == ObjectNumber);

                    //Nếu trạng thái khác Mới
                    if (dxxn.TrangThaiId != 0) result = false;

                    #endregion

                    break;
                case 10:
                    break;
                case 11:
                    break;
                case 12:
                    break;
                case 13:
                    break;
                case 14:

                    #region Kỳ lương

                    var kyLuong = context.KyLuong.FirstOrDefault(x => x.KyLuongId == ObjectNumber);

                    //Nếu trạng thái khác Mới
                    if (kyLuong.TrangThai != 1) result = false;

                    #endregion

                    break;
                case 20:
                    break;
                case 21:
                    break;
                case 22:
                    break;
                case 30:
                    break;
            }

            return result;
        }

        private bool CheckResetQuyTrinh(QuyTrinhModel QuyTrinh, Guid UserId, List<CauHinhQuyTrinhModel> ListCauHinhQuyTrinh,
            bool isReset, out List<string> ListCode)
        {
            bool checkChange = false;
            ListCode = new List<string>();

            var paramGetDetail = new GetDetailQuyTrinhParameter();
            paramGetDetail.Id = QuyTrinh.Id.Value;
            paramGetDetail.UserId = UserId;
            var detailQuyTrinhOld = GetDetailQuyTrinh(paramGetDetail);

            var lisOldCauHinhQuyTrinh = detailQuyTrinhOld.ListCauHinhQuyTrinh.OrderBy(z => z.SoTienTu).ToList();
            var listNewCauHinhQuyTrinh = ListCauHinhQuyTrinh.OrderBy(z => z.SoTienTu).ToList();

            //Nếu số lượng cấu hình giống nhau
            if (listNewCauHinhQuyTrinh.Count == lisOldCauHinhQuyTrinh.Count)
            {
                for (int i = 0; i < listNewCauHinhQuyTrinh.Count; i++)
                {
                    var oldCauHinh = lisOldCauHinhQuyTrinh[i];
                    var newCauHinh = listNewCauHinhQuyTrinh[i];

                    //Nếu số tiền thay đổi
                    if (oldCauHinh.SoTienTu != newCauHinh.SoTienTu)
                    {
                        checkChange = true;
                        break;
                    }
                    //Nếu số lượng các bước trong cấu hình thay đổi
                    else if (oldCauHinh.ListCacBuocQuyTrinh.Count != newCauHinh.ListCacBuocQuyTrinh.Count)
                    {
                        checkChange = true;
                        break;
                    }
                    //Nếu số tiền không thay đổi và số lượng các bước trong cấu hình không thay đổi
                    else
                    {
                        var listOldCacBuocQuyTrinh = oldCauHinh.ListCacBuocQuyTrinh.OrderBy(z => z.Stt).ToList();
                        var listNewCacBuocQuyTrinh = newCauHinh.ListCacBuocQuyTrinh.OrderBy(z => z.Stt).ToList();

                        for (int j = 0; j < listOldCacBuocQuyTrinh.Count; j++)
                        {
                            var oldCacBuocQuyTrinh = listOldCacBuocQuyTrinh[j];
                            var newCacBuocQuyTrinh = listNewCacBuocQuyTrinh[j];

                            //Nếu loại phê duyệt thay đổi
                            if (oldCacBuocQuyTrinh.LoaiPheDuyet != newCacBuocQuyTrinh.LoaiPheDuyet)
                            {
                                checkChange = true;
                                break;
                            }
                            //Nếu số lượng phòng ban trong bước thay đổi
                            else if (oldCacBuocQuyTrinh.ListPhongBanTrongCacBuocQuyTrinh.Count !=
                                     newCacBuocQuyTrinh.ListPhongBanTrongCacBuocQuyTrinh.Count)
                            {
                                checkChange = true;
                                break;
                            }
                            /*
                             * Nếu loại phê duyệt không thay đổi và Số lượng phòng ban trong bước không thay đổi và
                             * Số lượng phòng ban trong bước > 0
                             */
                            else if (oldCacBuocQuyTrinh.ListPhongBanTrongCacBuocQuyTrinh.Count > 0)
                            {
                                var listOldIdPhongBanTrongCacBuoc =
                                    oldCacBuocQuyTrinh.ListPhongBanTrongCacBuocQuyTrinh
                                        .Select(y => y.OrganizationId).ToList();
                                var listNewIdPhongBanTrongCacBuoc =
                                    newCacBuocQuyTrinh.ListPhongBanTrongCacBuocQuyTrinh
                                        .Select(y => y.OrganizationId).ToList();

                                for (int k = 0; k < listOldIdPhongBanTrongCacBuoc.Count; k++)
                                {
                                    var phongBanId = listOldIdPhongBanTrongCacBuoc[k];

                                    var exists = listNewIdPhongBanTrongCacBuoc.Contains(phongBanId);

                                    //Nếu có phòng ban không tồn tại
                                    if (!exists)
                                    {
                                        checkChange = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //Nếu số lượng cấu hình khác nhau
            else
            {
                checkChange = true;
            }

            if (checkChange)
            {
                //Đề xuất xin nghỉ
                if (QuyTrinh.DoiTuongApDung == 9)
                {
                    //Lấy list đối tượng có trạng thái Chờ phê duyệt
                    var listDoiTuong = context.DeXuatXinNghi.Where(x => x.TrangThaiId == 2).ToList();
                    var listObjectId = listDoiTuong.Select(x => x.DeXuatXinNghiId).ToList();

                    var listCacBuocApDung = context.CacBuocApDung
                        .Where(x => x.ObjectNumber != null && listObjectId.Contains(x.ObjectNumber.Value) && x.DoiTuongApDung == 9)
                        .ToList();
                    var listCacBuocApDungId = listCacBuocApDung.Select(y => y.Id).ToList();

                    var listPhongBanApDung = context.PhongBanApDung
                        .Where(x => listCacBuocApDungId.Contains(x.CacBuocApDungId)).ToList();

                    //Nếu không có đối tượng nào thì cho cập nhật quy trình
                    if (listDoiTuong.Count == 0)
                    {
                        checkChange = false;
                    }
                    //Nếu có đối tượng đang thực hiện chưa xong quy trình
                    else
                    {
                        ListCode = listDoiTuong.Select(y => y.Code).ToList();

                        //Nếu muốn đổi trạng thái và reset các bước của quy trình
                        if (isReset)
                        {
                            //Đổi trạng thái => Mới
                            listDoiTuong.ForEach(item =>
                            {
                                item.TrangThaiId = 0;

                                //Thêm ghi chú
                                Note note = new Note();
                                note.NoteId = Guid.NewGuid();
                                note.ObjectId = Guid.Empty;
                                note.Description = "Đề xuất đã được chuyển trạng thái về Mới vì quy trình phê duyệt đã được thay đổi";
                                note.Type = "ADD";
                                note.Active = true;
                                note.CreatedById = UserId;
                                note.CreatedDate = DateTime.Now;
                                note.NoteTitle = "Đã thêm ghi chú";
                                note.ObjectNumber = item.DeXuatXinNghiId;
                                note.ObjectType = NoteObjectType.DXXN;

                                context.Note.Add(note);
                            });

                            context.CacBuocApDung.RemoveRange(listCacBuocApDung);
                            context.PhongBanApDung.RemoveRange(listPhongBanApDung);
                            context.DeXuatXinNghi.UpdateRange(listDoiTuong);
                        }
                    }
                }
                //Phê duyệt phát sinh || phê duyệt yêu cầu bổ sung
                else if (QuyTrinh.DoiTuongApDung == 31 || QuyTrinh.DoiTuongApDung == 32)
                {
                    //Lấy list đối tượng có trạng thái Chờ phê duyệt
                    var listDoiTuong = context.CustomerOrder.Where(x => x.StatusOrder == 2 || x.StatusOrder == 10).ToList();
                    var listObjectId = listDoiTuong.Select(x => x.OrderId).ToList();

                    var listCacBuocApDung = context.CacBuocApDung
                        .Where(x => x.ObjectNumber != null && listObjectId.Contains(x.ObjectId) && x.DoiTuongApDung == 31)
                        .ToList();
                    var listCacBuocApDungId = listCacBuocApDung.Select(y => y.Id).ToList();

                    var listPhongBanApDung = context.PhongBanApDung
                        .Where(x => listCacBuocApDungId.Contains(x.CacBuocApDungId)).ToList();

                    //Nếu không có đối tượng nào thì cho cập nhật quy trình
                    if (listDoiTuong.Count == 0)
                    {
                        checkChange = false;
                    }
                    //Nếu có đối tượng đang thực hiện chưa xong quy trình
                    else
                    {
                        ListCode = listDoiTuong.Select(y => y.OrderCode).ToList();

                        //Nếu muốn đổi trạng thái và reset các bước của quy trình
                        if (isReset)
                        {
                            //Thêm ghi chú báo thay đổi quy trình
                            listDoiTuong.ForEach(item =>
                            {
                                Note note = new Note();
                                note.NoteId = Guid.NewGuid();
                                note.ObjectId = Guid.Empty;
                                note.Description = "Quy trình phê duyệt đã được thay đổi";
                                note.Type = "ADD";
                                note.Active = true;
                                note.CreatedById = UserId;
                                note.CreatedDate = DateTime.Now;
                                note.NoteTitle = "Đã thêm ghi chú";
                                note.ObjectId = item.OrderId;
                                note.ObjectType = NoteObjectType.ORDER;
                                context.Note.Add(note);
                            });
                            context.CacBuocApDung.RemoveRange(listCacBuocApDung);
                            context.PhongBanApDung.RemoveRange(listPhongBanApDung);
                        }
                    }
                }
                //Đề xuất chức vụ
                else if (QuyTrinh.DoiTuongApDung == 11)
                {
                    //Lấy list đối tượng có trạng thái Chờ phê duyệt
                    var listDoiTuong = context.DeXuatThayDoiChucVu.Where(x => x.TrangThai == 2).ToList();
                    var listObjectId = listDoiTuong.Select(x => x.DeXuatThayDoiChucVuId).ToList();

                    var listCacBuocApDung = context.CacBuocApDung
                        .Where(x => x.ObjectNumber != null && listObjectId.Contains(x.ObjectNumber.Value) && x.DoiTuongApDung == 11)
                        .ToList();
                    var listCacBuocApDungId = listCacBuocApDung.Select(y => y.Id).ToList();

                    var listPhongBanApDung = context.PhongBanApDung
                        .Where(x => listCacBuocApDungId.Contains(x.CacBuocApDungId)).ToList();

                    //Nếu không có đối tượng nào thì cho cập nhật quy trình
                    if (listDoiTuong.Count == 0)
                    {
                        checkChange = false;
                    }
                    //Nếu có đối tượng đang thực hiện chưa xong quy trình
                    else
                    {
                        ListCode = listDoiTuong.Select(y => y.TenDeXuat).ToList();

                        //Nếu muốn đổi trạng thái và reset các bước của quy trình
                        if (isReset)
                        {
                            //Đổi trạng thái => Mới
                            listDoiTuong.ForEach(item =>
                            {
                                item.TrangThai = 1;

                                //Thêm ghi chú
                                Note note = new Note();
                                note.NoteId = Guid.NewGuid();
                                note.ObjectId = Guid.Empty;
                                note.Description = "Đề xuất đã được chuyển trạng thái về Mới vì quy trình phê duyệt đã được thay đổi";
                                note.Type = "ADD";
                                note.Active = true;
                                note.CreatedById = UserId;
                                note.CreatedDate = DateTime.Now;
                                note.NoteTitle = "Đã thêm ghi chú";
                                note.ObjectNumber = item.DeXuatThayDoiChucVuId;
                                note.ObjectType = NoteObjectType.DXCV;

                                context.Note.Add(note);
                            });

                            context.CacBuocApDung.RemoveRange(listCacBuocApDung);
                            context.PhongBanApDung.RemoveRange(listPhongBanApDung);
                            context.DeXuatThayDoiChucVu.UpdateRange(listDoiTuong);
                        }
                    }
                }
             
               
            }

            return checkChange;
        }

        private void UpdateDuLieuChamCong(DeXuatXinNghi dxxn, Guid userId)
        {
            //Nếu không phải Đi muộn, Về sớm
            if (dxxn.LoaiDeXuatId != 12 && dxxn.LoaiDeXuatId != 13)
            {
                var listDxnnChiTiet = context.DeXuatXinNghiChiTiet
                    .Where(x => x.DeXuatXinNghiId == dxxn.DeXuatXinNghiId)
                    .ToList();

                var listDate = listDxnnChiTiet.Select(y => y.Ngay.Date).OrderBy(z => z).Distinct().ToList();

                var listChamCong = context.ChamCong.Where(x => x.EmployeeId == dxxn.EmployeeId).ToList();
                var listCreateChamCong = new List<ChamCong>();
                var listUpdateChamCong = new List<ChamCong>();

                listDate.ForEach(date =>
                {
                    var existsChamCong = listChamCong.FirstOrDefault(x => x.NgayChamCong.Date == date);
                    var listXinNghi = listDxnnChiTiet.Where(x => x.Ngay.Date == date).ToList();
                    var caSang = listXinNghi.FirstOrDefault(x => x.LoaiCaLamViecId == 1);
                    var caChieu = listXinNghi.FirstOrDefault(x => x.LoaiCaLamViecId == 2);

                    //Nếu đã có dữ liệu chấm công
                    if (existsChamCong != null)
                    {
                        //Nếu nghỉ ca sáng
                        if (caSang != null)
                        {
                            existsChamCong.KyHieuVaoSang = dxxn.LoaiDeXuatId;
                            existsChamCong.KyHieuRaSang = dxxn.LoaiDeXuatId;
                        }

                        //Nếu nghỉ ca chiều
                        if (caChieu != null)
                        {
                            existsChamCong.KyHieuVaoChieu = dxxn.LoaiDeXuatId;
                            existsChamCong.KyHieuRaChieu = dxxn.LoaiDeXuatId;
                        }

                        listUpdateChamCong.Add(existsChamCong);
                    }
                    //Nếu chưa có dữ liệu chấm công
                    else
                    {
                        var chamCong = new ChamCong();
                        chamCong.EmployeeId = dxxn.EmployeeId;
                        chamCong.NgayChamCong = date;
                        chamCong.CreatedDate = DateTime.Now;
                        chamCong.CreatedById = userId;

                        //Nếu nghỉ ca sáng
                        if (caSang != null)
                        {
                            chamCong.KyHieuVaoSang = dxxn.LoaiDeXuatId;
                            chamCong.KyHieuRaSang = dxxn.LoaiDeXuatId;
                        }

                        //Nếu nghỉ ca chiều
                        if (caChieu != null)
                        {
                            chamCong.KyHieuVaoChieu = dxxn.LoaiDeXuatId;
                            chamCong.KyHieuRaChieu = dxxn.LoaiDeXuatId;
                        }

                        listCreateChamCong.Add(chamCong);
                    }
                });

                context.ChamCong.UpdateRange(listUpdateChamCong);
                context.ChamCong.AddRange(listCreateChamCong);
            }
        }

        private void XuLyPhongBanPheDuyetDoiTuong(int doiTuongApDung, CauHinhQuyTrinh cauHinhQuyTrinh, int? objectNumber, Guid? objectId)
        {
            #region Xóa list data cũ

            var listOld = new List<PhongBanPheDuyetDoiTuong>();
            //Xóa data trong bảng mapping
            if (objectId != null && objectId != Guid.Empty)
            {
                listOld = context.PhongBanPheDuyetDoiTuong.Where(x =>
                        x.DoiTuongApDung == doiTuongApDung && x.ObjectId == objectId)
                    .ToList();
            }
            //Xóa data
            else
            {
                listOld = context.PhongBanPheDuyetDoiTuong.Where(x =>
                        x.DoiTuongApDung == doiTuongApDung && x.ObjectNumber == objectNumber)
                    .ToList();
            }

            context.PhongBanPheDuyetDoiTuong.RemoveRange(listOld);

            #endregion

            //Lấy list các bước trong quy trình
            var listCacBuocQuyTrinh = context.CacBuocQuyTrinh.Where(x => x.CauHinhQuyTrinhId == cauHinhQuyTrinh.Id)
                .ToList();
            var listCacBuocQuyTrinhId = listCacBuocQuyTrinh.Select(y => y.Id).ToList();

            //Lấy list phòng ban phê duyệt
            var listPhongBanId = context.PhongBanTrongCacBuocQuyTrinh
                .Where(x => listCacBuocQuyTrinhId.Contains(x.CacBuocQuyTrinhId)).Select(y => y.OrganizationId)
                .ToList();

            //Kiểm tra xem có loại phê duyệt trưởng bộ phận (1) hay không?
            var count = listCacBuocQuyTrinh.Count(x => x.LoaiPheDuyet == 1);

            Guid? userId = null;

            //Phê duyệt dịch vụ phát sinh
            if (doiTuongApDung == 31 && count > 0)
            {
                //Lấy đối tượng
                userId = context.CustomerOrder.FirstOrDefault(x => x.OrderId == objectId)?.CreatedById;
            }
            //Phê duyệt yêu cầu bổ sung
            else if (doiTuongApDung == 32 && count > 0)
            {
                //Lấy đối tượng
                userId = context.CustomerOrder.FirstOrDefault(x => x.OrderId == objectId)?.CreatedById;
            }

            if (userId != Guid.Empty && userId != null)
            {
                var user = context.User.FirstOrDefault(x => x.UserId == userId);

                var emp = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

                //Thêm phòng ban vào list
                if (emp?.OrganizationId != null) listPhongBanId.Add(emp.OrganizationId.Value);

                listPhongBanId = listPhongBanId.Distinct().ToList();
            }

            var list = new List<PhongBanPheDuyetDoiTuong>();
            //Thêm vào bảng mapping
            listPhongBanId.ForEach(item =>
            {
                var newItem = new PhongBanPheDuyetDoiTuong();
                newItem.DoiTuongApDung = doiTuongApDung;
                newItem.OrganizationId = item;
                newItem.ObjectNumber = objectNumber;
                newItem.ObjectId = objectId;

                list.Add(newItem);
            });

            context.PhongBanPheDuyetDoiTuong.AddRange(list);
        }
    }

    public class SanPhamBaoGia
    {
        public decimal SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal TyGia { get; set; }
        public decimal ThanhTienNhanCong { get; set; }
        public bool LoaiChietKhau { get; set; }
        public decimal GiaTriChietKhau { get; set; }
        public decimal PhanTramThue { get; set; }
    }
}
