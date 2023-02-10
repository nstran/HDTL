using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Databases;
using System.Linq;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Models.DeXuatXinNghiModel;
using TN.TNM.DataAccess.Models.Employee;
using Microsoft.EntityFrameworkCore.Internal;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using FirebaseAdmin.Messaging;
using System.Net;

namespace TN.TNM.DataAccess.Helper
{
    public static class CommonHelper
    {
        public static FirebaseApp credential = null;

        public static decimal GetMucLuongHienTaiByEmployeeId(TNTN8Context context, Guid employeeId)
        {
            decimal luongHienTai = 0;

            var list = GetLichSuLuongTheoNgaySoSanh(context, employeeId, DateTime.Now);

            if (list.Count > 0)
            {
                var lichSuLuongHienTai = list.First();
                luongHienTai = lichSuLuongHienTai.MucLuong;
            }

            return luongHienTai;
        }

        public static decimal GetMucLuongCuByEmployeeId(TNTN8Context context, Guid employeeId, DateTime ngaySoSanh)
        {
            decimal luongCu = 0;

            var nowDate = DateTime.Now;

            var list = GetLichSuLuongTheoNgaySoSanh(context, employeeId, nowDate);

            decimal luongHienTai = list.Count == 0 ? 0 : (list.First()?.MucLuong ?? 0);

            //Nếu chỉ có lương hiện tại
            if (list.Count == 1)
            {
                luongCu = luongHienTai;
            }
            //Nếu có mức lương cũ
            else if (list.Count > 1)
            {
                var lichSuLuongCu = list[1];

                //Nếu lương cũ có ngày áp dụng <= ngày so sánh
                if (lichSuLuongCu.NgayApDung.Date <= ngaySoSanh.Date)
                {
                    luongCu = luongHienTai;
                }
                //Nếu lương cũ có ngày áp dụng > ngày so sánh
                else
                {
                    luongCu = lichSuLuongCu.MucLuong;
                }
            }

            return luongCu;
        }

        public static decimal GetSoNgayTinhTheoMucLuongCu(TNTN8Context context, Guid employeeId, DateTime ngaySoSanh)
        {
            decimal soNgay = 0;

            var list = GetLichSuLuongTheoNgaySoSanh(context, employeeId, DateTime.Now);

            if (list.Count > 0)
            {
                var lichSuLuongHienTai = list.First();

                //Nếu lương cũ có ngày áp dụng > ngày so sánh
                if (lichSuLuongHienTai.NgayApDung.Date > ngaySoSanh)
                {
                    var tuNgay = ngaySoSanh.Date;
                    var denNgay = lichSuLuongHienTai.NgayApDung.AddDays(-1).Date;

                    var listChamCongByEmp = context.ChamCong.Where(x => x.NgayChamCong.Date >= tuNgay.Date &&
                                                                   x.NgayChamCong.Date <= denNgay.Date &&
                                                                   x.EmployeeId == employeeId).ToList();

                    var listDmvs = context.ThongKeDiMuonVeSom
                        .Where(x => x.EmployeeId == employeeId &&
                                    x.NgayDmvs.Date >= tuNgay.Date &&
                                    x.NgayDmvs.Date <= denNgay.Date).ToList();

                    double ngayLvThucTe = 0;
                    double ngayCongTac = 0;
                    double ngayHoiThao = 0;
                    double ngayNghiPhep = 0;
                    double ngayNghiLe = 0;
                    double ngayNghiCheDo = 0;
                    double ngayNghiHuongLuongKhac = 0;
                    double ngayNghiBu = 0;

                    listChamCongByEmp.ForEach(chamCong =>
                    {
                        #region Ngày làm việc thực tế

                        if (chamCong.KyHieuVaoSang == null && chamCong.KyHieuRaSang == null &&
                            chamCong.VaoSang != null && chamCong.RaSang != null)
                        {
                            ngayLvThucTe += 0.5;
                        }

                        if (chamCong.KyHieuVaoChieu == null && chamCong.KyHieuRaChieu == null &&
                            chamCong.VaoChieu != null && chamCong.RaChieu != null)
                        {
                            ngayLvThucTe += 0.5;
                        }

                        if (chamCong.KyHieuVaoSang == 14) ngayLvThucTe += 0.25;

                        if (chamCong.KyHieuRaSang == 14) ngayLvThucTe += 0.25;

                        if (chamCong.KyHieuVaoChieu == 14) ngayLvThucTe += 0.25;

                        if (chamCong.KyHieuRaChieu == 14) ngayLvThucTe += 0.25;

                        #endregion

                        #region Ngày công tác

                        if (chamCong.KyHieuVaoSang == 6) ngayCongTac += 0.25;
                        if (chamCong.KyHieuRaSang == 6) ngayCongTac += 0.25;
                        if (chamCong.KyHieuVaoChieu == 6) ngayCongTac += 0.25;
                        if (chamCong.KyHieuRaChieu == 6) ngayCongTac += 0.25;

                        #endregion

                        #region Ngày hội thảo

                        if (chamCong.KyHieuVaoSang == 7) ngayHoiThao += 0.25;
                        if (chamCong.KyHieuRaSang == 7) ngayHoiThao += 0.25;
                        if (chamCong.KyHieuVaoChieu == 7) ngayHoiThao += 0.25;
                        if (chamCong.KyHieuRaChieu == 7) ngayHoiThao += 0.25;

                        #endregion

                        #region Ngày nghỉ phép

                        if (chamCong.KyHieuVaoSang == 1) ngayNghiPhep += 0.25;
                        if (chamCong.KyHieuRaSang == 1) ngayNghiPhep += 0.25;
                        if (chamCong.KyHieuVaoChieu == 1) ngayNghiPhep += 0.25;
                        if (chamCong.KyHieuRaChieu == 1) ngayNghiPhep += 0.25;

                        #endregion

                        #region Ngày nghỉ lễ

                        if (chamCong.KyHieuVaoSang == 2) ngayNghiLe += 0.25;
                        if (chamCong.KyHieuRaSang == 2) ngayNghiLe += 0.25;
                        if (chamCong.KyHieuVaoChieu == 2) ngayNghiLe += 0.25;
                        if (chamCong.KyHieuRaChieu == 2) ngayNghiLe += 0.25;

                        #endregion

                        #region Ngày nghỉ chế độ

                        if (chamCong.KyHieuVaoSang == 5) ngayNghiCheDo += 0.25;
                        if (chamCong.KyHieuRaSang == 5) ngayNghiCheDo += 0.25;
                        if (chamCong.KyHieuVaoChieu == 5) ngayNghiCheDo += 0.25;
                        if (chamCong.KyHieuRaChieu == 5) ngayNghiCheDo += 0.25;

                        #endregion

                        #region Ngày nghỉ hưởng lương khác

                        if (chamCong.KyHieuVaoSang == 4) ngayNghiHuongLuongKhac += 0.25;
                        if (chamCong.KyHieuRaSang == 4) ngayNghiHuongLuongKhac += 0.25;
                        if (chamCong.KyHieuVaoChieu == 4) ngayNghiHuongLuongKhac += 0.25;
                        if (chamCong.KyHieuRaChieu == 4) ngayNghiHuongLuongKhac += 0.25;

                        #endregion

                        #region Ngày nghỉ bù

                        if (chamCong.KyHieuVaoSang == 3) ngayNghiBu += 0.25;
                        if (chamCong.KyHieuRaSang == 3) ngayNghiBu += 0.25;
                        if (chamCong.KyHieuVaoChieu == 3) ngayNghiBu += 0.25;
                        if (chamCong.KyHieuRaChieu == 3) ngayNghiBu += 0.25;

                        #endregion
                    });

                    int tongSoPhutDmvs = GetSoPhutDmvs(employeeId, listDmvs);
                    double tongSoNgayDmvs = Math.Round((double)tongSoPhutDmvs / 480, 2);

                    soNgay = (decimal) (ngayLvThucTe + ngayCongTac + ngayHoiThao + ngayNghiPhep + ngayNghiLe +
                                        ngayNghiCheDo + ngayNghiHuongLuongKhac + ngayNghiBu - tongSoNgayDmvs);
                }
            }

            return soNgay;
        }

        public static int GetSoPhutDmvs(Guid employeeId, List<ThongKeDiMuonVeSom> listDmvs)
        {
            int result = 0;

            result = listDmvs.Where(x => x.EmployeeId == employeeId).Sum(y => y.ThoiGian);

            return result;
        }

        public static int GetSoNamKinhNghiemByEmployeeId(TNTN8Context context, Guid employeeId)
        {
            int soNamKinhNghiem = 0;
            var nowDate = DateTime.Now;

            //Chỉ lấy những hợp đồng có Ngày ký hợp đồng nhỏ hơn hoặc bằng thời điểm hiện tại
            var listHopDong = context.HopDongNhanSu
                .Where(x => x.EmployeeId == employeeId &&
                            x.NgayBatDauLamViec.Date <= nowDate.Date).ToList();

            double tongSoNgay = 0;

            listHopDong.ForEach(item =>
            {
                if (item.NgayKetThucHopDong != null)
                {
                    tongSoNgay += (item.NgayKetThucHopDong.Value.Date - item.NgayBatDauLamViec.Date).TotalDays;
                }
                else
                {
                    tongSoNgay += (nowDate.Date - item.NgayBatDauLamViec.Date).TotalDays;
                }
            });

            soNamKinhNghiem = (int)(Math.Floor(tongSoNgay / 365));

            return soNamKinhNghiem;
        }

        public static List<Guid> GetListEmployeeOfCurrentOrgAndChildOrg(TNTN8Context context, Guid employeeId)
        {
            var listEmp = context.Employee.ToList();
            var employee = listEmp.FirstOrDefault(x => x.EmployeeId == employeeId);

            //Lấy list phòng ban con của user
            List<Guid> listOrganizationId = new List<Guid>();

            if (employee?.OrganizationId != null)
            {
                listOrganizationId.Add(employee.OrganizationId.Value);
                listOrganizationId =
                    GetOrganizationChildrenId(context, employee.OrganizationId.Value, listOrganizationId);
            }

            var listEmployee = listEmp
                .Where(x => x.OrganizationId != null && listOrganizationId.Contains(x.OrganizationId.Value))
                .Select(x => x.EmployeeId).ToList();

            return listEmployee;
        }

        public static DeXuatXinNghiModel GetInforDeXuatXinNghi(TNTN8Context context, DeXuatXinNghiModel deXuatXinNghi)
        {
            var listDeXuatXinNghiChiTiet = context.DeXuatXinNghiChiTiet
                .Where(x => x.DeXuatXinNghiId == deXuatXinNghi.DeXuatXinNghiId).ToList();
            deXuatXinNghi.ListDate = listDeXuatXinNghiChiTiet.Select(y => y.Ngay.Date)
                .Distinct().OrderBy(z => z).ToList();
            deXuatXinNghi.ListDateText = deXuatXinNghi.ListDate.Select(ConvertNgayTrongTuan).ToArray().Join(", ");

            //Nếu loại đề xuất là Đi muộn hoặc Về sớm
            if (deXuatXinNghi.LoaiDeXuatId == 12 || deXuatXinNghi.LoaiDeXuatId == 13)
            {
                deXuatXinNghi.Ca = listDeXuatXinNghiChiTiet.First().LoaiCaLamViecId;

                //Tính số ngày đi muộn, về sớm
                deXuatXinNghi.TongNgayNghi = listDeXuatXinNghiChiTiet.Count;
            }
            //Nếu loại đề xuất không phải Đi muộn hoặc Về sớm
            else
            {
                //Nếu số ngày là 1
                if (deXuatXinNghi.ListDate.Count == 1)
                {
                    var ngay = deXuatXinNghi.ListDate.First();
                    var listCa = listDeXuatXinNghiChiTiet.Where(x => x.Ngay.Date == ngay)
                        .OrderBy(z => z.LoaiCaLamViecId).ToList();

                    //Nếu xin nghỉ nửa ngày
                    if (listCa.Count == 1)
                    {
                        deXuatXinNghi.TuCa = listCa.First().LoaiCaLamViecId;
                        deXuatXinNghi.DenCa = listCa.First().LoaiCaLamViecId;
                    }
                    //Nếu xin nghỉ 1 ngày
                    else if (listCa.Count > 1)
                    {
                        deXuatXinNghi.TuCa = listCa[0].LoaiCaLamViecId;
                        deXuatXinNghi.DenCa = listCa[1].LoaiCaLamViecId;
                    }

                    //Tính số ngày nghỉ
                    deXuatXinNghi.TongNgayNghi = (decimal)listDeXuatXinNghiChiTiet.Count / 2;
                }
                //Nếu số ngày > 1
                else if (deXuatXinNghi.ListDate.Count > 1)
                {
                    var ngayBatDau = deXuatXinNghi.ListDate.First();
                    var ngayKetThuc = deXuatXinNghi.ListDate.Last();

                    //List ca xin nghỉ trong ngày bắt đầu (sắp xếp ca đầu lên trước)
                    var listCaTrongNgayBatDau = listDeXuatXinNghiChiTiet.Where(x => x.Ngay.Date == ngayBatDau)
                        .OrderBy(z => z.LoaiCaLamViecId).ToList();

                    //List ca xin nghỉ trong ngày kết thúc (sắp xếp ca cuối lên trước)
                    var listCaTrongNgayKetThuc = listDeXuatXinNghiChiTiet.Where(x => x.Ngay.Date == ngayKetThuc)
                        .OrderByDescending(z => z.LoaiCaLamViecId).ToList();

                    //Đề xuất xin nghỉ sẽ bắt đầu từ ca
                    deXuatXinNghi.TuCa = listCaTrongNgayBatDau[0].LoaiCaLamViecId;

                    //Đề xuất xin nghỉ sẽ kết thúc đến ca
                    deXuatXinNghi.DenCa = listCaTrongNgayKetThuc[0].LoaiCaLamViecId;

                    //Tính số ngày nghỉ
                    deXuatXinNghi.TongNgayNghi = (decimal)listDeXuatXinNghiChiTiet.Count / 2;
                }
            }

            return deXuatXinNghi;
        }

        private static List<Guid> GetOrganizationChildrenId(TNTN8Context context, Guid id, List<Guid> list)
        {
            var organization = context.Organization.Where(o => o.ParentId == id).ToList();

            organization.ForEach(item =>
            {
                list.Add(item.OrganizationId);
                GetOrganizationChildrenId(context, item.OrganizationId, list);
            });

            return list;
        }

        private static List<LichSuLuong> GetLichSuLuongTheoNgaySoSanh(TNTN8Context context, Guid employeeId, DateTime ngaySoSanh)
        {
            var listHopDong = context.HopDongNhanSu
                .Where(x => x.EmployeeId == employeeId &&
                            x.NgayBatDauLamViec.Date <= ngaySoSanh.Date)
                .OrderByDescending(z => z.NgayBatDauLamViec)
                .Select(y => new LichSuLuong
                {
                    Type = 1,
                    MucLuong = y.MucLuong,
                    NgayApDung = y.NgayBatDauLamViec
                }).ToList();

            var listDeXuatTangLuong = context.DeXuatTangLuongNhanVien
                .Join(context.DeXuatTangLuong,
                    dxnv => dxnv.DeXuatTangLuongId,
                    dx => dx.DeXuatTangLuongId,
                    (dxnv, dx) => new { Dxnv = dxnv, Dx = dx })
                .Where(x => x.Dxnv.EmployeeId == employeeId &&
                            x.Dxnv.TrangThai == 3 &&
                            x.Dx.TrangThai == 3 &&
                            x.Dx.NgayApDung != null &&
                            x.Dx.NgayApDung.Value.Date <= ngaySoSanh.Date)
                .Select(y => new LichSuLuong
                {
                    Type = 2,
                    MucLuong = y.Dxnv.LuongDeXuat,
                    NgayApDung = y.Dx.NgayApDung.Value,
                })
                .OrderByDescending(z => z.NgayApDung)
                .ToList();

            var list = new List<LichSuLuong>();
            list.AddRange(listHopDong);
            list.AddRange(listDeXuatTangLuong);

            list = list.OrderByDescending(z => z.NgayApDung).ToList();

            return list;
        }

        private static string ConvertNgayTrongTuan(DateTime date)
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

        public static NotificationReceiver GetListEmployeeIdNotifi(TNTN8Context context, CustomerOrder order, NotificationConfiguration notiConfigure, ReportPoint reportPoint, OrderProcess orderProcess)
        {
            var notificationReceiver = new NotificationReceiver();
            var list = new List<Guid>();
            //Nếu tích người tạo phiếu yêu cầu, hoặc KH 
            if (notiConfigure.ServiceRequestMaker == true)
            {
                //Nếu là phiếu yêu cầu
                if (order.OrderType == 1 && orderProcess == null)
                {
                    var userCreate = context.User.FirstOrDefault(x => x.UserId == order.CreatedById);
                    list.Add(userCreate.EmployeeId.Value);

                    //trả về list customerID lấy từ bảng order
                    notificationReceiver.CustomerId = order.CustomerId;
                }
                //Nếu là hỗ trợ
                if (order.OrderType == 2 && orderProcess == null)
                {
                    //Lấy phiếu yêu cầu
                    var customerOrder = context.CustomerOrder.FirstOrDefault(x => x.OrderId == order.ObjectId);
                    var userCreate = context.User.FirstOrDefault(x => x.UserId == customerOrder.CreatedById);
                    list.Add(userCreate.EmployeeId.Value);
                }
            }
            //Nếu tích quản lý dịch vụ
            if (notiConfigure.ServiceManagement == true && (orderProcess != null || order != null))
            {
                //Lấy list quản lý dịch vụ của gói
                var listEmpId = new List<Guid>();
                if (orderProcess != null) listEmpId = context.ManagerPacketService.Where(x => x.PackId == orderProcess.ServicePacketId).Select(x => x.EmployeeId).ToList();
                if (order != null) listEmpId = context.ManagerPacketService.Where(x => x.PackId == order.ServicePacketId).Select(x => x.EmployeeId).ToList();
                list.AddRange(listEmpId);
            }
            //Nếu tích người tạo phiếu hộ trợ dịch vụ
            if (notiConfigure.ServiceSupporter == true && order.IsOrderAction == true && orderProcess == null)
            {
                var userCreate = context.User.FirstOrDefault(x => x.UserId == order.CreatedById);
                if (userCreate != null) list.Add(userCreate.EmployeeId.Value);
            }
            //Nếu tích nhân viên hỗ trợ
            if (notiConfigure.Supporter == true && order.IsOrderAction == true && orderProcess == null)
            {
                var listCustomerOrderTaskId = context.CustomerOrderTask.Where(x => x.OrderActionId == order.OrderId).Select(x => x.Id).ToList();
                var listEmpId = context.OrderTaskMappingEmp.Where(x => listCustomerOrderTaskId.Contains(x.CustomerOrderTaskId)).Select(x => x.EmployeeId).ToList();
                list.AddRange(listEmpId);
            }
            //Nếu tích nhân viên báo cáo
            if (notiConfigure.Reporter == true && order.IsOrderAction == true && orderProcess == null)
            {
                if (reportPoint != null)
                {
                    list.Add(reportPoint.EmpId);
                }
            }
            notificationReceiver.ListEmployeeId = list;
            return notificationReceiver;
        }

        public static bool PushNotificationToDevice(string deviceId, string title, string body, string type, string orderId) {
            try
            {
                if(credential == null)
                {
                    credential = FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromFile("private_key.json")
                    });
                }

                var message = new Message()
                {
                    Token = deviceId,
                    Data = new Dictionary<string, string>()
                    {
                        {type, orderId}
                    },
                    Notification = new Notification()
                    {
                        Title = title,
                        Body = body
                    }
                };

                string response = FirebaseMessaging.DefaultInstance.SendAsync(message).Result;
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }

    public class LichSuLuong
    {
        public int Type { get; set; } /* 1: Theo hợp đồng, 2: Theo đề xuất tăng lương */
        public decimal MucLuong { get; set; }
        public DateTime NgayApDung { get; set; }
    }

    public class NotificationReceiver
    {
        public Guid? CustomerId { get; set; } 

        public List<Guid> ListEmployeeId { get; set; }
    }
}
