using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.EntityFrameworkCore.Internal;
using TN.TNM.Common.NotificationSetting;
using TN.TNM.DataAccess.ConstType.Contact;
using TN.TNM.DataAccess.ConstType.Note;
using TN.TNM.DataAccess.Databases.Entities;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Interfaces;
using TN.TNM.DataAccess.Messages.Parameters.Salary;
using TN.TNM.DataAccess.Messages.Results.Salary;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.CauHinhNghiLe;
using TN.TNM.DataAccess.Models.CauHinhOtMođel;
using TN.TNM.DataAccess.Models.CauHinhThue;
using TN.TNM.DataAccess.Models.ChamCong;
using TN.TNM.DataAccess.Models.DynamicColumnTable;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.GiamTru;
using TN.TNM.DataAccess.Models.Salary;
using TongHopChamCongModel = TN.TNM.DataAccess.Models.Salary.TongHopChamCongModel;

namespace TN.TNM.DataAccess.Databases.DAO
{
    public class SalaryDAO : BaseDAO, ISalaryDataAccess
    {
        private readonly IHostingEnvironment hostingEnvironment;

        public SalaryDAO(TNTN8Context _content, IHostingEnvironment _hostingEnvironment)
        {
            this.context = _content;
            this.hostingEnvironment = _hostingEnvironment;
        }

        public CreateOrUpdateCaLamViecResult CreateOrUpdateCaLamViec(CreateOrUpdateCaLamViecParameter parameter)
        {
            try
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    var mess = "";

                    //Create
                    if (parameter.CaLamViec.CaLamViecId == null || parameter.CaLamViec.CaLamViecId == 0)
                    {
                        var caLamViec = parameter.CaLamViec.ToEntity();
                        context.CaLamViec.Add(caLamViec);
                        context.SaveChanges();

                        var listCaLamViecChiTiet = new List<CaLamViecChiTiet>();
                        parameter.CaLamViec.ListCaLamViecChiTiet.ForEach(item =>
                        {
                            var caLamViecChiTiet = new CaLamViecChiTiet();
                            caLamViecChiTiet.CaLamViecId = caLamViec.CaLamViecId;
                            caLamViecChiTiet.NgayTrongTuan = item.NgayTrongTuan;

                            listCaLamViecChiTiet.Add(caLamViecChiTiet);
                        });

                        context.CaLamViecChiTiet.AddRange(listCaLamViecChiTiet);
                        context.SaveChanges();

                        mess = "Tạo thành công";
                    }
                    //Update
                    else
                    {
                        var caLamViec =
                            context.CaLamViec.FirstOrDefault(x => x.CaLamViecId == parameter.CaLamViec.CaLamViecId);

                        if (caLamViec == null)
                        {
                            return new CreateOrUpdateCaLamViecResult()
                            {
                                StatusCode = HttpStatusCode.ExpectationFailed,
                                MessageCode = "Ca làm việc không tồn tại trên hệ thống"
                            };
                        }

                        caLamViec.LoaiCaLamViecId = parameter.CaLamViec.LoaiCaLamViecId;
                        caLamViec.GioVao = parameter.CaLamViec.GioVao;
                        caLamViec.GioRa = parameter.CaLamViec.GioRa;
                        caLamViec.ThoiGianKetThucCa = parameter.CaLamViec.ThoiGianKetThucCa;

                        /* Xóa list cũ */
                        var listCaLamViecChiTietOld = context.CaLamViecChiTiet
                            .Where(x => x.CaLamViecId == caLamViec.CaLamViecId).ToList();
                        context.CaLamViecChiTiet.RemoveRange(listCaLamViecChiTietOld);
                        context.SaveChanges();

                        /* Thêm list mới */
                        var listCaLamViecChiTiet = new List<CaLamViecChiTiet>();
                        parameter.CaLamViec.ListCaLamViecChiTiet.ForEach(item =>
                        {
                            var caLamViecChiTiet = new CaLamViecChiTiet();
                            caLamViecChiTiet.CaLamViecId = caLamViec.CaLamViecId;
                            caLamViecChiTiet.NgayTrongTuan = item.NgayTrongTuan;

                            listCaLamViecChiTiet.Add(caLamViecChiTiet);
                        });

                        context.CaLamViecChiTiet.AddRange(listCaLamViecChiTiet);
                        context.SaveChanges();

                        mess = "Lưu thành công";
                    }

                    trans.Commit();

                    return new CreateOrUpdateCaLamViecResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = mess,
                    };
                }
            }
            catch (Exception e)
            {
                return new CreateOrUpdateCaLamViecResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreateOrUpdateCauHinhGiamTruResult CreateOrUpdateCauHinhGiamTru(CreateOrUpdateCauHinhGiamTruParameter parameter)
        {
            try
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    var mess = "";

                    //Create
                    if (parameter.CauHinhGiamTru.CauHinhGiamTruId == null || parameter.CauHinhGiamTru.CauHinhGiamTruId == 0)
                    {
                        var exists = context.CauHinhGiamTru.FirstOrDefault(x =>
                            x.LoaiGiamTruId == parameter.CauHinhGiamTru.LoaiGiamTruId);

                        if (exists != null)
                        {
                            return new CreateOrUpdateCauHinhGiamTruResult()
                            {
                                StatusCode = HttpStatusCode.Conflict,
                                MessageCode = "Loại giảm trừ đã tồn tại trên hệ thống"
                            };
                        }

                        var cauHinhGiamTru = parameter.CauHinhGiamTru.ToEntity();
                        context.CauHinhGiamTru.Add(cauHinhGiamTru);
                        context.SaveChanges();

                        mess = "Tạo thành công";
                    }
                    //Update
                    else
                    {
                        var cauHinhGiamTru = context.CauHinhGiamTru
                            .FirstOrDefault(x => x.CauHinhGiamTruId == parameter.CauHinhGiamTru.CauHinhGiamTruId);

                        if (cauHinhGiamTru == null)
                        {
                            return new CreateOrUpdateCauHinhGiamTruResult()
                            {
                                StatusCode = HttpStatusCode.NotFound,
                                MessageCode = "Cấu hình không tồn tại trên hệ thống"
                            };
                        }

                        var exists = context.CauHinhGiamTru.FirstOrDefault(x =>
                            x.LoaiGiamTruId == parameter.CauHinhGiamTru.LoaiGiamTruId &&
                            x.CauHinhGiamTruId != parameter.CauHinhGiamTru.CauHinhGiamTruId);

                        if (exists != null)
                        {
                            return new CreateOrUpdateCauHinhGiamTruResult()
                            {
                                StatusCode = HttpStatusCode.Conflict,
                                MessageCode = "Loại giảm trừ đã tồn tại trên hệ thống"
                            };
                        }

                        cauHinhGiamTru.LoaiGiamTruId = parameter.CauHinhGiamTru.LoaiGiamTruId;
                        cauHinhGiamTru.MucGiamTru = parameter.CauHinhGiamTru.MucGiamTru;
                        context.CauHinhGiamTru.Update(cauHinhGiamTru);
                        context.SaveChanges();

                        mess = "Lưu thành công";
                    }

                    trans.Commit();

                    return new CreateOrUpdateCauHinhGiamTruResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = mess,
                    };
                }
            }
            catch (Exception e)
            {
                return new CreateOrUpdateCauHinhGiamTruResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreateOrUpdateCauHinhNghiLeResult CreateOrUpdateCauHinhNghiLe(CreateOrUpdateCauHinhNghiLeParameter parameter)
        {
            try
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    var mess = "";

                    //Create
                    if (parameter.CauHinhNghiLe.NghiLeId == null || parameter.CauHinhNghiLe.NghiLeId == 0)
                    {
                        var existsSoNam =
                            context.CauHinhNghiLe.FirstOrDefault(x => x.SoNam == parameter.CauHinhNghiLe.SoNam);

                        if (existsSoNam != null)
                        {
                            return new CreateOrUpdateCauHinhNghiLeResult()
                            {
                                StatusCode = HttpStatusCode.Conflict,
                                MessageCode = "Số năm đã tồn tại, hãy nhập số năm khác"
                            };
                        }

                        bool isTrungNgay = false;
                        parameter.CauHinhNghiLe.ListCauHinhNghiLeChiTiet.ForEach(item =>
                        {
                            var count = parameter.CauHinhNghiLe.ListCauHinhNghiLeChiTiet.Count(x =>
                                x.Ngay.Date == item.Ngay.Date);

                            if (count > 1) isTrungNgay = true;
                        });

                        if (isTrungNgay)
                        {
                            return new CreateOrUpdateCauHinhNghiLeResult()
                            {
                                StatusCode = HttpStatusCode.Conflict,
                                MessageCode = "Ngày nghỉ lễ, Ngày nghỉ bù, Ngày làm bù không được trùng nhau"
                            };
                        }

                        var cauHinhNghiLe = parameter.CauHinhNghiLe.ToEntity();
                        context.CauHinhNghiLe.Add(cauHinhNghiLe);
                        context.SaveChanges();

                        var listCauHinhNghiLeChiTiet = new List<CauHinhNghiLeChiTiet>();
                        parameter.CauHinhNghiLe.ListCauHinhNghiLeChiTiet.ForEach(item =>
                        {
                            var cauHinhNghiLeChiTiet = new CauHinhNghiLeChiTiet();
                            cauHinhNghiLeChiTiet.NghiLeId = cauHinhNghiLe.NghiLeId;
                            cauHinhNghiLeChiTiet.LoaiNghiLe = item.LoaiNghiLe;
                            cauHinhNghiLeChiTiet.Ngay = item.Ngay;

                            listCauHinhNghiLeChiTiet.Add(cauHinhNghiLeChiTiet);
                        });

                        context.CauHinhNghiLeChiTiet.AddRange(listCauHinhNghiLeChiTiet);
                        context.SaveChanges();

                        mess = "Tạo thành công";
                    }
                    //Update
                    else
                    {
                        var cauHinhNghiLe =
                            context.CauHinhNghiLe.FirstOrDefault(x => x.NghiLeId == parameter.CauHinhNghiLe.NghiLeId);

                        if (cauHinhNghiLe == null)
                        {
                            return new CreateOrUpdateCauHinhNghiLeResult()
                            {
                                StatusCode = HttpStatusCode.NotFound,
                                MessageCode = "Cấu hình không tồn tại trên hệ thống"
                            };
                        }

                        var existsSoNam =
                            context.CauHinhNghiLe.FirstOrDefault(x => x.SoNam == parameter.CauHinhNghiLe.SoNam &&
                                                                      x.NghiLeId != cauHinhNghiLe.NghiLeId);

                        if (existsSoNam != null)
                        {
                            return new CreateOrUpdateCauHinhNghiLeResult()
                            {
                                StatusCode = HttpStatusCode.Conflict,
                                MessageCode = "Số năm đã tồn tại, hãy nhập số năm khác"
                            };
                        }

                        bool isTrungNgay = false;
                        parameter.CauHinhNghiLe.ListCauHinhNghiLeChiTiet.ForEach(item =>
                        {
                            var count = parameter.CauHinhNghiLe.ListCauHinhNghiLeChiTiet.Count(x =>
                                x.Ngay.Date == item.Ngay.Date);

                            if (count > 1) isTrungNgay = true;
                        });

                        if (isTrungNgay)
                        {
                            return new CreateOrUpdateCauHinhNghiLeResult()
                            {
                                StatusCode = HttpStatusCode.Conflict,
                                MessageCode = "Ngày nghỉ lễ, Ngày nghỉ bù, Ngày làm bù không được trùng nhau"
                            };
                        }

                        cauHinhNghiLe.SoNam = parameter.CauHinhNghiLe.SoNam;

                        /* Xóa list cũ */
                        var listCauHinhNghiLeChiTietOld = context.CauHinhNghiLeChiTiet
                            .Where(x => x.NghiLeId == cauHinhNghiLe.NghiLeId).ToList();
                        context.CauHinhNghiLeChiTiet.RemoveRange(listCauHinhNghiLeChiTietOld);
                        context.SaveChanges();

                        /* Thêm list mới */
                        var listCauHinhNghiLeChiTiet = new List<CauHinhNghiLeChiTiet>();
                        parameter.CauHinhNghiLe.ListCauHinhNghiLeChiTiet.ForEach(item =>
                        {
                            var cauHinhNghiLeChiTiet = new CauHinhNghiLeChiTiet();
                            cauHinhNghiLeChiTiet.NghiLeId = cauHinhNghiLe.NghiLeId;
                            cauHinhNghiLeChiTiet.LoaiNghiLe = item.LoaiNghiLe;
                            cauHinhNghiLeChiTiet.Ngay = item.Ngay;

                            listCauHinhNghiLeChiTiet.Add(cauHinhNghiLeChiTiet);
                        });

                        context.CauHinhNghiLeChiTiet.AddRange(listCauHinhNghiLeChiTiet);
                        context.SaveChanges();

                        mess = "Lưu thành công";
                    }

                    trans.Commit();

                    return new CreateOrUpdateCauHinhNghiLeResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = mess,
                    };
                }
            }
            catch (Exception e)
            {
                return new CreateOrUpdateCauHinhNghiLeResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreateOrUpdateCauHinhOtResult CreateOrUpdateCauHinhOt(CreateOrUpdateCauHinhOtParameter parameter)
        {
            try
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    var mess = "";

                    //Create
                    if (parameter.CauHinhOt.CauHinhOtId == null || parameter.CauHinhOt.CauHinhOtId == 0)
                    {
                        var exists = context.CauHinhOt.FirstOrDefault(x => x.LoaiOtId == parameter.CauHinhOt.LoaiOtId);

                        if (exists != null)
                        {
                            return new CreateOrUpdateCauHinhOtResult()
                            {
                                StatusCode = HttpStatusCode.NotFound,
                                MessageCode = "Loại OT đã tồn tại trên hệ thống"
                            };
                        }

                        var cauHinhOt = parameter.CauHinhOt.ToEntity();
                        context.CauHinhOt.Add(cauHinhOt);
                        context.SaveChanges();

                        mess = "Tạo thành công";
                    }
                    //Update
                    else
                    {
                        var cauHinhOt = context.CauHinhOt.FirstOrDefault(x => x.CauHinhOtId == parameter.CauHinhOt.CauHinhOtId);

                        if (cauHinhOt == null)
                        {
                            return new CreateOrUpdateCauHinhOtResult
                            {
                                StatusCode = HttpStatusCode.NotFound,
                                MessageCode = "Cấu hình không tồn tại trên hệ thống"
                            };
                        }

                        var exists = context.CauHinhOt
                            .FirstOrDefault(x => x.LoaiOtId == parameter.CauHinhOt.LoaiOtId &&
                                                 x.CauHinhOtId != parameter.CauHinhOt.CauHinhOtId);

                        if (exists != null)
                        {
                            return new CreateOrUpdateCauHinhOtResult()
                            {
                                StatusCode = HttpStatusCode.NotFound,
                                MessageCode = "Loại OT đã tồn tại trên hệ thống"
                            };
                        }

                        cauHinhOt.LoaiOtId = parameter.CauHinhOt.LoaiOtId;
                        cauHinhOt.TyLeHuong = parameter.CauHinhOt.TyLeHuong;
                        context.CauHinhOt.Update(cauHinhOt);
                        context.SaveChanges();

                        mess = "Lưu thành công";
                    }

                    trans.Commit();

                    return new CreateOrUpdateCauHinhOtResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = mess,
                    };
                }
            }
            catch (Exception e)
            {
                return new CreateOrUpdateCauHinhOtResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreateOrUpdateCongThucTinhLuongResult CreateOrUpdateCongThucTinhLuong(CreateOrUpdateCongThucTinhLuongParameter parameter)
        {
            try
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    var mess = "";
                    var isValid = CheckSyntaxCongThuc(parameter.CongThucTinhLuong.CongThuc);

                    //Nếu công thức sai cú pháp
                    if (!isValid)
                    {
                        return new CreateOrUpdateCongThucTinhLuongResult()
                        {
                            StatusCode = HttpStatusCode.Conflict,
                            MessageCode = "Công thức không đúng định dạng"
                        };
                    }

                    //Create
                    if (parameter.CongThucTinhLuong.CongThucTinhLuongId == null || parameter.CongThucTinhLuong.CongThucTinhLuongId == 0)
                    {
                        var congThucTinhLuong = parameter.CongThucTinhLuong.ToEntity();
                        context.CongThucTinhLuong.Add(congThucTinhLuong);
                        context.SaveChanges();

                        mess = "Tạo thành công";
                    }
                    //Update
                    else
                    {
                        var congThucTinhLuong = context.CongThucTinhLuong
                                .FirstOrDefault(x => x.CongThucTinhLuongId == parameter.CongThucTinhLuong.CongThucTinhLuongId);

                        if (congThucTinhLuong == null)
                        {
                            return new CreateOrUpdateCongThucTinhLuongResult
                            {
                                StatusCode = HttpStatusCode.NotFound,
                                MessageCode = "Cấu hình không tồn tại trên hệ thống"
                            };
                        }

                        congThucTinhLuong.CongThuc = parameter.CongThucTinhLuong.CongThuc;
                        context.CongThucTinhLuong.Update(congThucTinhLuong);
                        context.SaveChanges();

                        mess = "Lưu thành công";
                    }

                    trans.Commit();

                    return new CreateOrUpdateCongThucTinhLuongResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = mess,
                    };
                }
            }
            catch (Exception e)
            {
                return new CreateOrUpdateCongThucTinhLuongResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetTkDiMuonVeSomResult GetTkDiMuonVeSom(GetTkDiMuonVeSomParameter parameter)
        {
            try
            {
                if (parameter.TuNgay.Date > parameter.DenNgay.Date)
                {
                    return new GetTkDiMuonVeSomResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Bộ lọc Từ ngày phải trước Đến ngày"
                    };
                }

                var listData = new List<List<DataRowModel>>();
                var listColorChamCong = new List<ColorChamCongModel>();
                
                var listChamCong = context.ChamCong.Where(x => x.NgayChamCong.Date >= parameter.TuNgay.Date &&
                                                               x.NgayChamCong.Date <= parameter.DenNgay.Date &&
                                                               (parameter.ListEmployeeId.Count == 0 ||
                                                                parameter.ListEmployeeId.Contains(x.EmployeeId.Value)))
                    .OrderBy(z => z.NgayChamCong).ToList();
                var listEmpId = listChamCong.Select(y => y.EmployeeId).Distinct().ToList();
                var listEmp = new List<Employee>();

                //Nếu hiển thị tất cả nhân viên 
                if (parameter.IsShowOption == 0)
                {
                    listEmp = context.Employee
                        .Where(x => !String.IsNullOrWhiteSpace(x.CodeMayChamCong) &&
                                    (parameter.ListEmployeeId.Count == 0 ||
                                     parameter.ListEmployeeId.Contains(x.EmployeeId)))
                        .OrderBy(z => z.EmployeeName)
                        .ToList();
                }
                //Nếu chỉ hiển thị nhân viên đang làm việc
                else if (parameter.IsShowOption == 1)
                {
                    listEmp = context.Employee
                        .Join(context.User,
                            emp => emp.EmployeeId,
                            user => user.EmployeeId,
                            (emp, user) => new { Emp = emp, User = user }) // selection
                        .Where(x => !String.IsNullOrWhiteSpace(x.Emp.CodeMayChamCong) &&
                                    (parameter.ListEmployeeId.Count == 0 ||
                                     parameter.ListEmployeeId.Contains(x.Emp.EmployeeId)) &&
                                     x.Emp.Active == true)
                        .Select(y => new Employee
                        {
                            EmployeeId = y.Emp.EmployeeId,
                            CodeMayChamCong = y.Emp.CodeMayChamCong,
                            EmployeeCode = y.Emp.EmployeeCode,
                            EmployeeName = y.Emp.EmployeeName,
                        })
                        .OrderBy(z => z.EmployeeName)
                        .ToList();
                }
                //Nếu chỉ hiển thị nhân viên đã nghỉ viêc
                else if (parameter.IsShowOption == 2)
                {
                    listEmp = context.Employee
                        .Join(context.User,
                            emp => emp.EmployeeId,
                            user => user.EmployeeId,
                            (emp, user) => new { Emp = emp, User = user }) // selection
                        .Where(x => !String.IsNullOrWhiteSpace(x.Emp.CodeMayChamCong) &&
                                    (parameter.ListEmployeeId.Count == 0 ||
                                     parameter.ListEmployeeId.Contains(x.Emp.EmployeeId)) &&
                                    x.Emp.Active == false && x.User.Active == false)
                        .Select(y => new Employee
                        {
                            EmployeeId = y.Emp.EmployeeId,
                            CodeMayChamCong = y.Emp.CodeMayChamCong,
                            EmployeeCode = y.Emp.EmployeeCode,
                            EmployeeName = y.Emp.EmployeeName,
                        })
                        .OrderBy(z => z.EmployeeName)
                        .ToList();
                }

                var listDate = new List<DateTime>();
                var startDate = parameter.TuNgay.Date;
                var endDate = parameter.DenNgay.Date;
                while (startDate <= endDate)
                {
                    listDate.Add(startDate);
                    startDate = startDate.AddDays(1).Date;
                }
                var listDmvs = context.ThongKeDiMuonVeSom
                    .Where(x => listEmpId.Contains(x.EmployeeId) && 
                                x.NgayDmvs.Date >= parameter.TuNgay.Date &&
                                x.NgayDmvs.Date <= parameter.DenNgay.Date).ToList();
                var listNgayLamViecTrongTuan = GeneralList.GetTrangThais("NgayLamViecTrongTuan");

                #region Data

                int stt = 0;
                listEmp.ForEach(item =>
                {
                    stt++;
                    var listDataRow = new List<DataRowModel>();

                    var dataEmpIdRow = new DataRowModel();
                    dataEmpIdRow.ColumnKey = "employee_id";
                    dataEmpIdRow.ColumnValue = item.EmployeeId.ToString();
                    dataEmpIdRow.Width = "60px";
                    dataEmpIdRow.TextAlign = "center";
                    dataEmpIdRow.IsShow = false;
                    listDataRow.Add(dataEmpIdRow);

                    var dataRow = new DataRowModel();
                    dataRow.ColumnKey = "stt";
                    dataRow.ColumnValue = stt.ToString();
                    dataRow.ValueType = ValueTypeEnum.NUMBER;
                    dataRow.Width = "60px";
                    dataRow.TextAlign = "center";
                    listDataRow.Add(dataRow);

                    var dataRow2 = new DataRowModel();
                    dataRow2.ColumnKey = "code";
                    dataRow2.ColumnValue = item.CodeMayChamCong;
                    dataRow2.Width = "60px";
                    dataRow2.TextAlign = "center";
                    listDataRow.Add(dataRow2);

                    var dataRow3 = new DataRowModel();
                    dataRow3.ColumnKey = "name";
                    dataRow3.ColumnValue = item.EmployeeCode + " - " + item.EmployeeName;
                    dataRow3.Width = "250px";
                    dataRow3.TextAlign = "left";
                    listDataRow.Add(dataRow3);

                    #region Phần tổng kết

                    int tongSoPhutDmvs = CommonHelper.GetSoPhutDmvs(item.EmployeeId, listDmvs);
                    decimal tongSoNgayDmvs = Math.Round((decimal)tongSoPhutDmvs / 480, 2);
                    int tongSoLanDmvs = GetSoLanDmvs(item.EmployeeId, listDmvs);
                    double tongSoNgayNghi = GetTongSoNgayNghi(item.EmployeeId, listChamCong);

                    var dataRow4 = new DataRowModel();
                    dataRow4.ColumnKey = "soPhutDmvs";
                    dataRow4.ColumnValue = tongSoPhutDmvs.ToString();
                    dataRow4.Width = "85px";
                    dataRow4.TextAlign = "right";
                    dataRow4.ValueType = ValueTypeEnum.NUMBER;
                    listDataRow.Add(dataRow4);

                    var dataRow5 = new DataRowModel();
                    dataRow5.ColumnKey = "soNgayDmvs";
                    dataRow5.ColumnValue = tongSoNgayDmvs.ToString();
                    dataRow5.Width = "85px";
                    dataRow5.TextAlign = "right";
                    dataRow5.ValueType = ValueTypeEnum.NUMBER;
                    listDataRow.Add(dataRow5);

                    var dataRow6 = new DataRowModel();
                    dataRow6.ColumnKey = "soNgayNghi";
                    dataRow6.ColumnValue = tongSoNgayNghi.ToString();
                    dataRow6.Width = "85px";
                    dataRow6.TextAlign = "right";
                    dataRow6.ValueType = ValueTypeEnum.NUMBER;
                    listDataRow.Add(dataRow6);

                    var dataRow7 = new DataRowModel();
                    dataRow7.ColumnKey = "soLanDmvs";
                    dataRow7.ColumnValue = tongSoLanDmvs.ToString();
                    dataRow7.Width = "85px";
                    dataRow7.TextAlign = "right";
                    dataRow7.ValueType = ValueTypeEnum.NUMBER;
                    listDataRow.Add(dataRow7);

                    #endregion

                    int index = 0;
                    listDate.ForEach(data =>
                    {
                        //Lấy dữ liệu chấm công của nhân viên theo ngày
                        var chamCong = listChamCong.FirstOrDefault(x =>
                            x.NgayChamCong == data && x.EmployeeId == item.EmployeeId);

                        //Nếu nhân viên có dữ liệu chấm công theo ngày này
                        if (chamCong != null)
                        {
                            #region Vào sáng

                            index++;
                            var _vaoSang = new DataRowModel();
                            _vaoSang.ColumnKey = "index_" + index;
                            _vaoSang.ColumnValue = HienThiKyHieu(chamCong, 1);
                            _vaoSang.Width = "60px";
                            _vaoSang.TextAlign = "center";

                            listDataRow.Add(_vaoSang);

                            #region Màu hiển thị của dữ liệu

                            var _colorVaoSang = new ColorChamCongModel();
                            _colorVaoSang.Code = item.CodeMayChamCong;
                            _colorVaoSang.NgayChamCong = data.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                            _colorVaoSang.Index = _vaoSang.ColumnKey;
                            _colorVaoSang.ColorCode = HienThiMau(chamCong, 1, listDmvs);

                            listColorChamCong.Add(_colorVaoSang);

                            #endregion

                            #endregion

                            #region Ra sáng

                            index++;
                            var _raSang = new DataRowModel();
                            _raSang.ColumnKey = "index_" + index;
                            _raSang.ColumnValue = HienThiKyHieu(chamCong, 2);
                            _raSang.Width = "60px";
                            _raSang.TextAlign = "center";

                            listDataRow.Add(_raSang);

                            #region Màu hiển thị của dữ liệu

                            var _colorRaSang = new ColorChamCongModel();
                            _colorRaSang.Code = item.CodeMayChamCong;
                            _colorRaSang.NgayChamCong = data.ToString("dd/MM/yyyy");
                            _colorRaSang.Index = _raSang.ColumnKey;
                            _colorRaSang.ColorCode = HienThiMau(chamCong, 2, listDmvs);

                            listColorChamCong.Add(_colorRaSang);

                            #endregion

                            #endregion

                            #region Vào chiều

                            index++;
                            var _vaoChieu = new DataRowModel();
                            _vaoChieu.ColumnKey = "index_" + index;
                            _vaoChieu.ColumnValue = HienThiKyHieu(chamCong, 3);
                            _vaoChieu.Width = "60px";
                            _vaoChieu.TextAlign = "center";

                            listDataRow.Add(_vaoChieu);

                            #region Màu hiển thị của dữ liệu

                            var _colorVaoChieu = new ColorChamCongModel();
                            _colorVaoChieu.Code = item.CodeMayChamCong;
                            _colorVaoChieu.NgayChamCong = data.ToString("dd/MM/yyyy");
                            _colorVaoChieu.Index = _vaoChieu.ColumnKey;
                            _colorVaoChieu.ColorCode = HienThiMau(chamCong, 3, listDmvs);

                            listColorChamCong.Add(_colorVaoChieu);

                            #endregion

                            #endregion

                            #region Ra chiều

                            index++;
                            var _raChieu = new DataRowModel();
                            _raChieu.ColumnKey = "index_" + index;
                            _raChieu.ColumnValue = HienThiKyHieu(chamCong, 4);
                            _raChieu.Width = "60px";
                            _raChieu.TextAlign = "center";

                            listDataRow.Add(_raChieu);

                            #region Màu hiển thị của dữ liệu

                            var _colorRaChieu = new ColorChamCongModel();
                            _colorRaChieu.Code = item.CodeMayChamCong;
                            _colorRaChieu.NgayChamCong = data.ToString("dd/MM/yyyy");
                            _colorRaChieu.Index = _raChieu.ColumnKey;
                            _colorRaChieu.ColorCode = HienThiMau(chamCong, 4, listDmvs);

                            listColorChamCong.Add(_colorRaChieu);

                            #endregion

                            #endregion
                        }
                        //Nếu nhân viên không có dữ liệu chấm công
                        else
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                index++;
                                var _dataRow = new DataRowModel();
                                _dataRow.ColumnKey = "index_" + index;
                                _dataRow.ColumnValue = "--";
                                _dataRow.Width = "60px";
                                _dataRow.TextAlign = "center";

                                listDataRow.Add(_dataRow);
                            }
                        }
                    });

                    listData.Add(listDataRow);
                });

                #endregion

                #region Header

                var listDataHeader = new List<List<DataHeaderModel>>();

                /* tr1 */
                var listHeader1 = new List<DataHeaderModel>()
                {
                    new DataHeaderModel()
                    {
                        ColumnKey = "stt",
                        ColumnValue = "#",
                        Rowspan = 3,
                        Colspan = 0,
                        Width = "40px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "code",
                        ColumnValue = "Code",
                        Rowspan = 3,
                        Colspan = 0,
                        Width = "120px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "name",
                        ColumnValue = "Họ tên",
                        Rowspan = 3,
                        Colspan = 0,
                        Width = "230px",
                    },
                };

                listHeader1.Add(new DataHeaderModel()
                {
                    ColumnKey = "soPhutDmvs",
                    ColumnValue = "Số phút đi muộn/về sớm",
                    Rowspan = 3,
                    Colspan = 0,
                    Width = "85px",
                });

                listHeader1.Add(new DataHeaderModel()
                {
                    ColumnKey = "soNgayDmvs",
                    ColumnValue = "Số ngày đi muộn/về sớm",
                    Rowspan = 3,
                    Colspan = 0,
                    Width = "85px",
                });

                listHeader1.Add(new DataHeaderModel()
                {
                    ColumnKey = "soNgayNghi",
                    ColumnValue = "Tổng số ngày nghỉ",
                    Rowspan = 3,
                    Colspan = 0,
                    Width = "85px",
                });

                listHeader1.Add(new DataHeaderModel()
                {
                    ColumnKey = "soLanDmvs",
                    ColumnValue = "Số lần đi muộn/về sớm",
                    Rowspan = 3,
                    Colspan = 0,
                    Width = "85px",
                });

                int _index = 0;
                listDate.ForEach(item =>
                {
                    string ngayTrongTuan = listNgayLamViecTrongTuan
                        .FirstOrDefault(x => x.Value == ConvertNgayTrongTuan(item.DayOfWeek))?.Name;

                    _index++;
                    var _dataHeader = new DataHeaderModel();
                    _dataHeader.ColumnKey = "index_ngay_" + _index;
                    _dataHeader.ColumnValue = item.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) + " - " + ngayTrongTuan;
                    _dataHeader.Rowspan = 0;
                    _dataHeader.Colspan = 4;
                    _dataHeader.Width = "240px";

                    listHeader1.Add(_dataHeader);
                });

                listDataHeader.Add(listHeader1);

                /* tr2 */
                var listHeader2 = new List<DataHeaderModel>();
                int _indexCa = 0;
                listDate.ForEach(date =>
                {
                    for (int i = 1; i <= 2; i++)
                    {
                        _indexCa++;
                        var _dataHeader = new DataHeaderModel();
                        _dataHeader.ColumnKey = "index_ca_" + _indexCa;
                        _dataHeader.ColumnValue = i % 2 != 0 ? "Sáng": "Chiều";
                        _dataHeader.Rowspan = 0;
                        _dataHeader.Colspan = 2;
                        _dataHeader.Width = "120px";

                        listHeader2.Add(_dataHeader);
                    }
                });

                listDataHeader.Add(listHeader2);

                /* tr3 */
                var listHeader3 = new List<DataHeaderModel>();
                listDate.ForEach(date =>
                {
                    for (int i = 1; i <= 4; i++)
                    {
                        var _dataHeader = new DataHeaderModel();
                        _dataHeader.ColumnKey = i.ToString();
                        _dataHeader.ColumnValue = i % 2 != 0 ? "Vào" : "Ra";
                        _dataHeader.Rowspan = 0;
                        _dataHeader.Colspan = 0;
                        _dataHeader.Width = "60px";

                        listHeader3.Add(_dataHeader);
                    }
                });

                listDataHeader.Add(listHeader3);

                #endregion

                return new GetTkDiMuonVeSomResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListData = listData,
                    ListDataHeader = listDataHeader,
                    ListColorChamCong = listColorChamCong
                };
            }
            catch (Exception e)
            {
                return new GetTkDiMuonVeSomResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public ImportChamCongResult ImportChamCong(ImportChamCongParameter parameter)
        {
            try
            {
                var listEmp = context.Employee.ToList();

                var listDataConvert = new List<DataChamCongConvertModel>();

                for (int i = 0; i < parameter.ListData.Count; i++)
                {
                    var item = parameter.ListData[i];
                    item.NoteError = "";

                    var emp = listEmp.FirstOrDefault(x =>
                        x.CodeMayChamCong != null && x.CodeMayChamCong.Trim() == item.No.Trim());

                    if (emp == null)
                    {
                        item.IsError = true;
                        item.NoteError = "<p>- Code trên máy chấm công không tồn tại trên hệ thống</p>";
                    }

                    DateTime thoiGianChamCong;
                    if (!DateTime.TryParseExact(item.ThoiGianChamCong, "M/d/yyyy h:mm:ss tt",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out thoiGianChamCong))
                    {
                        item.IsError = true;
                        item.NoteError += "<p>- Định dạng thời gian không hợp lệ</p>";
                    }

                    if (item.IsError != true)
                    {
                        var ngayChamCong = thoiGianChamCong.Date;

                        var dataConvert = new DataChamCongConvertModel();
                        dataConvert.EmployeeId = emp.EmployeeId;
                        dataConvert.NgayChamCong = ngayChamCong;
                        dataConvert.ThoiGianChamCong = thoiGianChamCong.TimeOfDay;

                        listDataConvert.Add(dataConvert);
                    }
                }

                var existsError = parameter.ListData.Count(x => x.IsError == true);
                if (existsError > 0)
                {
                    return new ImportChamCongResult()
                    {
                        StatusCode = HttpStatusCode.Conflict,
                        MessageCode = "Có dữ liệu không hợp lệ",
                        ListDataError = parameter.ListData
                    };
                }

                var listCaLamViecChiTiet = context.CaLamViecChiTiet.ToList();
                var listCaLamViec = context.CaLamViec
                    .Select(y => new CaLamViecModel
                    {
                        CaLamViecId = y.CaLamViecId,
                        LoaiCaLamViecId = y.LoaiCaLamViecId,
                        GioVao = y.GioVao,
                        GioRa = y.GioRa,
                        ThoiGianKetThucCa = y.ThoiGianKetThucCa,
                        ListCaLamViecChiTiet = listCaLamViecChiTiet.Where(x => x.CaLamViecId == y.CaLamViecId)
                            .Select(z => new CaLamViecChiTietModel(z)).ToList()
                    }).ToList();

                var listCauHinhNghiLeChiTiet = context.CauHinhNghiLeChiTiet.ToList();
                var listCauHinhNghiLe = context.CauHinhNghiLe.Select(y => new CauHinhNghiLeModel
                {
                    NghiLeId = y.NghiLeId,
                    SoNam = y.SoNam,
                    ListCauHinhNghiLeChiTiet = listCauHinhNghiLeChiTiet.Where(x => x.NghiLeId == y.NghiLeId)
                        .Select(z => new CauHinhNghiLeChiTietModel(z)).ToList()
                }).ToList();

                var listKeHoachOt = context.KeHoachOt.Where(x => (x.TrangThai == 5 ||
                                                                  x.TrangThai == 6 ||
                                                                  x.TrangThai == 7) &&
                                                                 x.Active == true).ToList();
                var listKeHoachOtId = listKeHoachOt.Select(y => y.KeHoachOtId).ToList();
                var listKeHoachOtThanhVien = context.KeHoachOtThanhVien
                    .Where(x => x.KeHoachOtId != null && listKeHoachOtId.Contains(x.KeHoachOtId.Value)).ToList();
                var cauHinhChamCongOt = context.CauHinhChamCongOt.FirstOrDefault();
                var cauHinhOtCaNgay = context.CauHinhOtCaNgay.FirstOrDefault();

                // Lọc ra các ngày nghỉ
                var listAllNgay = listDataConvert.Select(y => y.NgayChamCong).Distinct().OrderBy(z => z).ToList();

                var listEmpId = listDataConvert.Select(y => y.EmployeeId).Distinct().ToList();
                var listChamCong = new List<ChamCong>();
                var listThongKeDiMuonVeSom = new List<ThongKeDiMuonVeSom>();
                var listChamCongOt = new List<ChamCongOt>();
                var listTongHopChamCongOt = new List<TongHopChamCongOt>();

                listAllNgay.ForEach(ngay =>
                {
                    var cauHinhNghiLe = listCauHinhNghiLe.FirstOrDefault(x => x.SoNam == ngay.Year);

                    bool isNgayNghi = false;
                    if (cauHinhNghiLe != null)
                    {
                        var listNgayNghi = cauHinhNghiLe.ListCauHinhNghiLeChiTiet
                            .Where(x => x.LoaiNghiLe == 1 || x.LoaiNghiLe == 2).Select(y => y.Ngay.Date)
                            .ToList();

                        var count = listNgayNghi.Count(x => x == ngay);
                        if (count > 0) isNgayNghi = true;
                    }

                    if (!isNgayNghi)
                    {
                        #region Chấm công thường

                        listEmpId.ForEach(empId =>
                        {
                            var listLanChamCongTrongNgay = listDataConvert
                                .Where(x => x.NgayChamCong == ngay && x.EmployeeId == empId).ToList();

                            var listThoiGianChamCong = listLanChamCongTrongNgay.Select(y => y.ThoiGianChamCong).ToList();

                            //Nếu có dữ liệu chấm công
                            if (listThoiGianChamCong.Count > 0)
                            {
                                var ngayTrongTuan = ConvertNgayTrongTuan(ngay.DayOfWeek);
                                var _chamCong = new ChamCong();
                                var _listThongKeDiMuonVeSom = new List<ThongKeDiMuonVeSom>();
                                GetGioChamCong(empId, ngay, listThoiGianChamCong, listCaLamViec, ngayTrongTuan,
                                    out bool isNoData,
                                    out _chamCong, out _listThongKeDiMuonVeSom);

                                if (!isNoData)
                                {
                                    var chamCong = new ChamCong();
                                    chamCong.NgayChamCong = ngay;
                                    chamCong.CreatedDate = DateTime.Now;
                                    chamCong.CreatedById = parameter.UserId;
                                    chamCong.EmployeeId = empId;
                                    chamCong.VaoSang = _chamCong.VaoSang;
                                    chamCong.RaSang = _chamCong.RaSang;
                                    chamCong.VaoChieu = _chamCong.VaoChieu;
                                    chamCong.RaChieu = _chamCong.RaChieu;

                                    listChamCong.Add(chamCong);

                                    if (_listThongKeDiMuonVeSom.Count > 0)
                                    {
                                        listThongKeDiMuonVeSom.AddRange(_listThongKeDiMuonVeSom);
                                    }
                                }
                            }
                        });

                        #endregion
                    }

                    #region Chấm công OT

                    //List kế hoạch OT có: Ngày bắt đầu <= ngày <= Ngày kết thúc 
                    var _listKeHoachOt = listKeHoachOt.Where(x => x.NgayBatDau.Value.Date <= ngay &&
                                                                  x.NgayKetThuc.Value.Date >= ngay).ToList();

                    _listKeHoachOt.ForEach(keHoach =>
                    {
                        TimeSpan gioBatDauTinhChamCongOt;
                        TimeSpan gioBatDau;
                        TimeSpan gioKetThuc;

                        var _listKeHoachOtThanhVien = listKeHoachOtThanhVien
                            .Where(x => x.KeHoachOtId == keHoach.KeHoachOtId &&
                                        x.TrangThai == 3).ToList();

                        var listDangKyOtEmpId =
                            _listKeHoachOtThanhVien.Select(y => y.EmployeeId).Distinct().ToList();

                        //Nếu là Ca sáng
                        if (keHoach.LoaiCaId == 1 && cauHinhOtCaNgay != null)
                        {
                            gioBatDau = cauHinhOtCaNgay.GioVaoSang;
                            gioKetThuc = cauHinhOtCaNgay.GioRaSang;

                            listDangKyOtEmpId.ForEach(empId =>
                            {
                                var chamCongOt = new ChamCongOt();

                                var listLanChamCongTrongNgay = listDataConvert
                                    .Where(x => x.NgayChamCong == ngay && x.EmployeeId == empId).ToList();

                                var listThoiGianChamCong = listLanChamCongTrongNgay
                                    .Where(x => x.ThoiGianChamCong <= cauHinhOtCaNgay.GioKetThucSang)
                                    .Select(y => y.ThoiGianChamCong).ToList();

                                //Nếu chỉ có giờ vào
                                if (listThoiGianChamCong.Count == 1)
                                {
                                    chamCongOt.EmployeeId = empId.Value;
                                    chamCongOt.LoaiOtId = keHoach.LoaiOtId.Value;
                                    chamCongOt.NgayChamCong = ngay;
                                    chamCongOt.GioVaoSang = listThoiGianChamCong.First() > gioBatDau
                                        ? listThoiGianChamCong.First()
                                        : gioBatDau;

                                    listChamCongOt.Add(chamCongOt);
                                }
                                //Nếu có cả giờ vào và giờ ra
                                else if (listThoiGianChamCong.Count > 1)
                                {
                                    chamCongOt.EmployeeId = empId.Value;
                                    chamCongOt.LoaiOtId = keHoach.LoaiOtId.Value;
                                    chamCongOt.NgayChamCong = ngay;
                                    chamCongOt.GioVaoSang = listThoiGianChamCong.First() > gioBatDau
                                        ? listThoiGianChamCong.First()
                                        : gioBatDau;
                                    chamCongOt.GioRaSang = listThoiGianChamCong.Last() < gioKetThuc
                                        ? listThoiGianChamCong.Last()
                                        : gioKetThuc;

                                    listChamCongOt.Add(chamCongOt);
                                }
                            });
                        }
                        //Nếu là Ca chiều
                        else if (keHoach.LoaiCaId == 2 && cauHinhOtCaNgay != null)
                        {
                            gioBatDau = cauHinhOtCaNgay.GioVaoChieu;
                            gioKetThuc = cauHinhOtCaNgay.GioRaChieu;

                            listDangKyOtEmpId.ForEach(empId =>
                            {
                                var chamCongOt = new ChamCongOt();

                                var listLanChamCongTrongNgay = listDataConvert
                                    .Where(x => x.NgayChamCong == ngay && x.EmployeeId == empId).ToList();

                                var listThoiGianChamCong = listLanChamCongTrongNgay
                                    .Where(x => x.ThoiGianChamCong >= cauHinhOtCaNgay.GioKetThucSang &&
                                                x.ThoiGianChamCong <= cauHinhOtCaNgay.GioKetThucChieu)
                                    .Select(y => y.ThoiGianChamCong).ToList();

                                //Nếu chỉ có giờ vào
                                if (listThoiGianChamCong.Count == 1)
                                {
                                    chamCongOt.EmployeeId = empId.Value;
                                    chamCongOt.LoaiOtId = keHoach.LoaiOtId.Value;
                                    chamCongOt.NgayChamCong = ngay;
                                    chamCongOt.GioVaoChieu = listThoiGianChamCong.First() > gioBatDau
                                        ? listThoiGianChamCong.First()
                                        : gioBatDau;

                                    listChamCongOt.Add(chamCongOt);
                                }
                                //Nếu có cả giờ vào và giờ ra
                                else if (listThoiGianChamCong.Count > 1)
                                {
                                    chamCongOt.EmployeeId = empId.Value;
                                    chamCongOt.LoaiOtId = keHoach.LoaiOtId.Value;
                                    chamCongOt.NgayChamCong = ngay;
                                    chamCongOt.GioVaoChieu = listThoiGianChamCong.First() > gioBatDau
                                        ? listThoiGianChamCong.First()
                                        : gioBatDau;
                                    chamCongOt.GioRaChieu = listThoiGianChamCong.Last() < gioKetThuc
                                        ? listThoiGianChamCong.Last()
                                        : gioKetThuc;

                                    listChamCongOt.Add(chamCongOt);
                                }
                            });
                        }
                        //Nếu là Ca tối
                        else if (keHoach.LoaiCaId == 3)
                        {
                            gioBatDau = keHoach.GioBatDau.Value;
                            gioKetThuc = keHoach.GioKetThuc.Value;

                            //Nếu có cấu hình chấm công OT
                            if (cauHinhChamCongOt != null)
                            {
                                gioBatDauTinhChamCongOt =
                                    TimeSpan.FromMinutes(gioBatDau.TotalMinutes - (double) cauHinhChamCongOt.SoPhut);
                            }
                            else
                            {
                                gioBatDauTinhChamCongOt = gioBatDau;
                            }

                            listDangKyOtEmpId.ForEach(empId =>
                            {
                                var listLanChamCongTrongNgay = listDataConvert
                                    .Where(x => x.NgayChamCong == ngay && x.EmployeeId == empId).ToList();

                                var listThoiGianChamCong = listLanChamCongTrongNgay
                                    .Where(x => x.ThoiGianChamCong > gioBatDauTinhChamCongOt)
                                    .Select(y => y.ThoiGianChamCong).ToList();

                                var chamCongOt = new ChamCongOt();

                                //Nếu chỉ có giờ vào
                                if (listThoiGianChamCong.Count == 1)
                                {
                                    chamCongOt.EmployeeId = empId.Value;
                                    chamCongOt.LoaiOtId = keHoach.LoaiOtId.Value;
                                    chamCongOt.NgayChamCong = ngay;
                                    chamCongOt.GioVaoToi = listThoiGianChamCong.First() > gioBatDau
                                        ? listThoiGianChamCong.First()
                                        : gioBatDau;
                                    chamCongOt.GioRaToi = null;

                                    listChamCongOt.Add(chamCongOt);
                                }
                                //Nếu có cả giờ vào và giờ ra
                                else if (listThoiGianChamCong.Count > 1)
                                {
                                    chamCongOt.EmployeeId = empId.Value;
                                    chamCongOt.LoaiOtId = keHoach.LoaiOtId.Value;
                                    chamCongOt.NgayChamCong = ngay;
                                    chamCongOt.GioVaoToi = listThoiGianChamCong.First() > gioBatDau
                                        ? listThoiGianChamCong.First()
                                        : gioBatDau;
                                    chamCongOt.GioRaToi = listThoiGianChamCong.Last() < gioKetThuc
                                        ? listThoiGianChamCong.Last()
                                        : gioKetThuc;

                                    listChamCongOt.Add(chamCongOt);
                                }
                            });
                        }
                        //Nếu là Cả ngày 
                        else if (keHoach.LoaiCaId == 4 && cauHinhOtCaNgay != null)
                        {
                            gioBatDau = cauHinhOtCaNgay.GioVaoSang;
                            gioKetThuc = cauHinhOtCaNgay.GioRaChieu;

                            listDangKyOtEmpId.ForEach(empId =>
                            {
                                var chamCongOt = new ChamCongOt();

                                var listLanChamCongTrongNgay = listDataConvert
                                    .Where(x => x.NgayChamCong == ngay && x.EmployeeId == empId).ToList();

                                #region Ca sáng

                                var listThoiGianChamCongSang = listLanChamCongTrongNgay
                                    .Where(x => x.ThoiGianChamCong <= cauHinhOtCaNgay.GioKetThucSang)
                                    .Select(y => y.ThoiGianChamCong).ToList();

                                //Nếu chỉ có giờ vào
                                if (listThoiGianChamCongSang.Count == 1)
                                {
                                    chamCongOt.EmployeeId = empId.Value;
                                    chamCongOt.LoaiOtId = keHoach.LoaiOtId.Value;
                                    chamCongOt.NgayChamCong = ngay;
                                    chamCongOt.GioVaoSang = listThoiGianChamCongSang.First() > gioBatDau
                                        ? listThoiGianChamCongSang.First()
                                        : gioBatDau;
                                }
                                //Nếu có cả giờ vào và giờ ra
                                else if (listThoiGianChamCongSang.Count > 1)
                                {
                                    chamCongOt.EmployeeId = empId.Value;
                                    chamCongOt.LoaiOtId = keHoach.LoaiOtId.Value;
                                    chamCongOt.NgayChamCong = ngay;
                                    chamCongOt.GioVaoSang = listThoiGianChamCongSang.First() > gioBatDau
                                        ? listThoiGianChamCongSang.First()
                                        : gioBatDau;
                                    chamCongOt.GioRaSang = listThoiGianChamCongSang.Last() < gioKetThuc
                                        ? listThoiGianChamCongSang.Last()
                                        : gioKetThuc;
                                }

                                #endregion

                                #region Ca chiều

                                var listThoiGianChamCongChieu = listLanChamCongTrongNgay
                                    .Where(x => x.ThoiGianChamCong >= cauHinhOtCaNgay.GioKetThucSang &&
                                                x.ThoiGianChamCong <= cauHinhOtCaNgay.GioKetThucChieu)
                                    .Select(y => y.ThoiGianChamCong).ToList();

                                //Nếu chỉ có giờ vào
                                if (listThoiGianChamCongChieu.Count == 1)
                                {
                                    chamCongOt.EmployeeId = empId.Value;
                                    chamCongOt.LoaiOtId = keHoach.LoaiOtId.Value;
                                    chamCongOt.NgayChamCong = ngay;
                                    chamCongOt.GioVaoChieu = listThoiGianChamCongChieu.First() > gioBatDau
                                        ? listThoiGianChamCongChieu.First()
                                        : gioBatDau;
                                }
                                //Nếu có cả giờ vào và giờ ra
                                else if (listThoiGianChamCongChieu.Count > 1)
                                {
                                    chamCongOt.EmployeeId = empId.Value;
                                    chamCongOt.LoaiOtId = keHoach.LoaiOtId.Value;
                                    chamCongOt.NgayChamCong = ngay;
                                    chamCongOt.GioVaoChieu = listThoiGianChamCongChieu.First() > gioBatDau
                                        ? listThoiGianChamCongChieu.First()
                                        : gioBatDau;
                                    chamCongOt.GioRaChieu = listThoiGianChamCongChieu.Last() < gioKetThuc
                                        ? listThoiGianChamCongChieu.Last()
                                        : gioKetThuc;
                                }

                                #endregion

                                if (chamCongOt.GioVaoSang != null || chamCongOt.GioVaoChieu != null)
                                {
                                    listChamCongOt.Add(chamCongOt);
                                }
                            });
                        }
                    });

                    #endregion
                });

                listChamCongOt = listChamCongOt.GroupBy(g => new {g.EmployeeId, g.NgayChamCong, g.LoaiOtId})
                    .Select(y => new ChamCongOt
                    {
                        EmployeeId = y.Key.EmployeeId,
                        NgayChamCong = y.Key.NgayChamCong,
                        LoaiOtId = y.Key.LoaiOtId,
                        GioVaoSang = y.FirstOrDefault(x => x.GioVaoSang != null)?.GioVaoSang,
                        GioRaSang = y.FirstOrDefault(x => x.GioRaSang != null)?.GioRaSang,
                        GioVaoChieu = y.FirstOrDefault(x => x.GioVaoChieu != null)?.GioVaoChieu,
                        GioRaChieu = y.FirstOrDefault(x => x.GioRaChieu != null)?.GioRaChieu,
                        GioVaoToi = y.FirstOrDefault(x => x.GioVaoToi != null)?.GioVaoToi,
                        GioRaToi = y.FirstOrDefault(x => x.GioRaToi != null)?.GioRaToi,
                    }).ToList();

                listTongHopChamCongOt = TinhTongHopChamCongOt(listChamCongOt);

                //Nếu chưa có cấu hình ca làm việc
                if (listChamCong.Count == 0 && listChamCongOt.Count == 0)
                {
                    return new ImportChamCongResult()
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        MessageCode = "Chưa có cấu hình ca làm việc"
                    };
                }

                using (var trans = context.Database.BeginTransaction())
                {
                    #region Chấm công thường

                    #region Xóa dữ liệu cũ

                    var listOldChamCong = (from a in context.ChamCong
                        join b in listChamCong on
                            new { a.EmployeeId, a.NgayChamCong } equals
                            new { b.EmployeeId, b.NgayChamCong }
                        select a).ToList();

                    var listOldThongKeDiMuonVeSom = (from a in context.ThongKeDiMuonVeSom
                        join b in listChamCong on
                            new { EmployeeId = a.EmployeeId, NgayChamCong = a.NgayDmvs } equals
                            new { EmployeeId = b.EmployeeId.Value, NgayChamCong = b.NgayChamCong }
                        select a).ToList();

                    context.ChamCong.RemoveRange(listOldChamCong);
                    context.ThongKeDiMuonVeSom.RemoveRange(listOldThongKeDiMuonVeSom);
                    context.SaveChanges();

                    #endregion

                    context.ChamCong.AddRange(listChamCong);
                    context.ThongKeDiMuonVeSom.AddRange(listThongKeDiMuonVeSom);
                    context.SaveChanges();

                    #endregion

                    #region Chấm công OT

                    #region Xóa dữ liệu cũ

                    var listOldChamCongOt = (from a in context.ChamCongOt
                        join b in listChamCongOt on
                            new {a.EmployeeId, a.NgayChamCong, a.LoaiOtId} equals
                            new {b.EmployeeId, b.NgayChamCong, b.LoaiOtId}
                        select a).ToList();

                    var listOldTongHopChamCongOt = (from a in context.TongHopChamCongOt
                        join b in listTongHopChamCongOt on
                            new {a.EmployeeId, a.NgayChamCong, a.LoaiOtId} equals
                            new {b.EmployeeId, b.NgayChamCong, b.LoaiOtId}
                        select a).ToList();

                    context.ChamCongOt.RemoveRange(listOldChamCongOt);
                    context.TongHopChamCongOt.RemoveRange(listOldTongHopChamCongOt);
                    context.SaveChanges();

                    #endregion

                    context.ChamCongOt.AddRange(listChamCongOt);
                    context.TongHopChamCongOt.AddRange(listTongHopChamCongOt);
                    context.SaveChanges();

                    #endregion

                    #region Lưu lịch sử vào dòng thòi gian

                    var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                    var emp = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

                    var contentNote = "";
                    contentNote = "<p>- <strong>" + emp.EmployeeCode + " - " + emp.EmployeeName +
                                  "</strong> đã import file chấm công</p>";

                    Note note = new Note
                    {
                        NoteId = Guid.NewGuid(),
                        Description = contentNote,
                        Type = "ADD",
                        ObjectId = Guid.Empty,
                        ObjectNumber = 1,
                        ObjectType = NoteObjectType.TKCCT,
                        Active = true,
                        CreatedById = parameter.UserId,
                        CreatedDate = DateTime.Now,
                        NoteTitle = "đã thêm ghi chú"
                    };

                    context.Note.Add(note);
                    context.SaveChanges();

                    #endregion

                    trans.Commit();
                }

                return new ImportChamCongResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Import thành công"
                };
            }
            catch (Exception e)
            {
                return new ImportChamCongResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetMasterThongKeChamCongResult GetMasterThongKeChamCong(GetMasterThongKeChamCongParameter parameter)
        {
            try
            {
                var listKyHieuChamCong = GeneralList.GetTrangThais("KyHieuChamCong")
                    .Where(x => x.Value != 12 && x.Value != 13).ToList();

                var listEmployee = context.Employee
                    .Where(x => !String.IsNullOrWhiteSpace(x.CodeMayChamCong))
                    .Select(y => new EmployeeEntityModel
                    {
                        EmployeeId = y.EmployeeId,
                        EmployeeCode = y.EmployeeCode,
                        EmployeeName = y.EmployeeName,
                        EmployeeCodeName = y.EmployeeCode + " - " + y.EmployeeName
                    }).OrderBy(z => z.EmployeeName).ToList();

                return new GetMasterThongKeChamCongResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "OK",
                    ListKyHieuChamCong = listKyHieuChamCong,
                    ListEmployee = listEmployee,
                };
            }
            catch (Exception e)
            {
                return new GetMasterThongKeChamCongResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreateOrUpdateChamCongResult CreateOrUpdateChamCong(CreateOrUpdateChamCongParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                var emp = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);

                using (var trans = context.Database.BeginTransaction())
                {
                    var listCaLamViecChiTiet = context.CaLamViecChiTiet.ToList();
                    var listCaLamViec = context.CaLamViec
                        .Select(y => new CaLamViecModel
                        {
                            CaLamViecId = y.CaLamViecId,
                            LoaiCaLamViecId = y.LoaiCaLamViecId,
                            GioVao = y.GioVao,
                            GioRa = y.GioRa,
                            ThoiGianKetThucCa = y.ThoiGianKetThucCa,
                            ListCaLamViecChiTiet = listCaLamViecChiTiet.Where(x => x.CaLamViecId == y.CaLamViecId)
                                .Select(z => new CaLamViecChiTietModel(z)).ToList()
                        }).ToList();

                    for (int i = 0; i < parameter.ListEmployeeId.Count; i++)
                    {
                        var EmployeeId = parameter.ListEmployeeId[i];
                        var employee = context.Employee.FirstOrDefault(x => x.EmployeeId == EmployeeId);

                        var chamCong = context.ChamCong
                            .FirstOrDefault(x => x.EmployeeId == EmployeeId &&
                                                 x.NgayChamCong.Date == parameter.NgayChamCong.Date);

                        var ngayTrongTuan = ConvertNgayTrongTuan(parameter.NgayChamCong.DayOfWeek);
                        var _chamCong = new ChamCong();
                        var _listThongKeDiMuonVeSom = new List<ThongKeDiMuonVeSom>();
                        var listThoiGianChamCong = new List<TimeSpan>();

                        //Nếu là Giờ
                        if (parameter.TypeUpdate == "GIO")
                        {
                            int loaiCaLamViecId = 0;
                            if (parameter.TypeField == 1 || parameter.TypeField == 2)
                            {
                                loaiCaLamViecId = 1;
                            }
                            else if (parameter.TypeField == 3 || parameter.TypeField == 4)
                            {
                                loaiCaLamViecId = 2;
                            }

                            var caLamViec =
                                CheckExistsCauHinhChamCong(listCaLamViec, ngayTrongTuan, loaiCaLamViecId);

                            if (caLamViec == null)
                            {
                                return new CreateOrUpdateChamCongResult()
                                {
                                    StatusCode = HttpStatusCode.NotFound,
                                    MessageCode = "Chưa có cấu hình ca làm việc"
                                };
                            }

                            var oldThoiGian = "--";
                            var newThoiGian = parameter.ThoiGian == null ? "--" : parameter.ThoiGian.Value.ToString(@"hh\:mm");
                            var ngay = parameter.NgayChamCong.ToString("dd/MM/yyyy");

                            //Create
                            if (chamCong == null)
                            {
                                //Ra sáng hoặc Ra chiều
                                if (parameter.TypeField == 2 || parameter.TypeField == 4)
                                {
                                    string validGioVaoGioRa = CheckGioVaoGioRa(null, parameter.ThoiGian);
                                    if (validGioVaoGioRa != "")
                                    {
                                        return new CreateOrUpdateChamCongResult()
                                        {
                                            StatusCode = HttpStatusCode.NotFound,
                                            MessageCode = validGioVaoGioRa
                                        };
                                    }
                                }

                                if (parameter.ThoiGian != null)
                                {
                                    #region Tính lại thống kê đi muộn về sớm

                                    var thongKeDmvs = new ThongKeDiMuonVeSom();
                                    GetThongKeDmvsTheoThoiGian(caLamViec, parameter.TypeField,
                                        parameter.ThoiGian.Value, out thongKeDmvs);

                                    //Nếu đi muộn về sớm
                                    if (thongKeDmvs.ThoiGian > 0)
                                    {
                                        thongKeDmvs.EmployeeId = EmployeeId;
                                        thongKeDmvs.NgayDmvs = parameter.NgayChamCong.Date;
                                        context.ThongKeDiMuonVeSom.Add(thongKeDmvs);
                                        context.SaveChanges();
                                    }

                                    #endregion

                                    //Vào sáng
                                    if (parameter.TypeField == 1)
                                    {
                                        _chamCong.VaoSang = parameter.ThoiGian;
                                    }
                                    //Ra sáng
                                    else if (parameter.TypeField == 2)
                                    {
                                        _chamCong.RaSang = parameter.ThoiGian;
                                    }
                                    //Vào chiều
                                    else if (parameter.TypeField == 3)
                                    {
                                        _chamCong.VaoChieu = parameter.ThoiGian;
                                    }
                                    //Ra chiều
                                    else if (parameter.TypeField == 4)
                                    {
                                        _chamCong.RaChieu = parameter.ThoiGian;
                                    }

                                    _chamCong.NgayChamCong = parameter.NgayChamCong.Date;
                                    _chamCong.CreatedDate = DateTime.Now;
                                    _chamCong.CreatedById = parameter.UserId;
                                    _chamCong.EmployeeId = EmployeeId;

                                    context.ChamCong.Add(_chamCong);
                                    context.SaveChanges();
                                }
                            }
                            //Update
                            else
                            {
                                #region Xóa thống kê đi muộn về sớm

                                var oldThongKeDmvs = context.ThongKeDiMuonVeSom.FirstOrDefault(x =>
                                    x.EmployeeId == EmployeeId &&
                                    x.NgayDmvs.Date == parameter.NgayChamCong.Date &&
                                    x.LoaiDmvs == parameter.TypeField);

                                if (oldThongKeDmvs != null)
                                {
                                    context.ThongKeDiMuonVeSom.Remove(oldThongKeDmvs);
                                    context.SaveChanges();
                                }

                                #endregion

                                #region Tính lại thống kê đi muộn về sớm

                                if (parameter.ThoiGian != null)
                                {
                                    var thongKeDmvs = new ThongKeDiMuonVeSom();
                                    GetThongKeDmvsTheoThoiGian(caLamViec, parameter.TypeField,
                                        parameter.ThoiGian.Value, out thongKeDmvs);

                                    //Nếu đi muộn về sớm
                                    if (thongKeDmvs.ThoiGian > 0)
                                    {
                                        thongKeDmvs.EmployeeId = EmployeeId;
                                        thongKeDmvs.NgayDmvs = parameter.NgayChamCong.Date;
                                        context.ThongKeDiMuonVeSom.Add(thongKeDmvs);
                                        context.SaveChanges();
                                    }
                                }

                                #endregion

                                //Vào sáng
                                if (parameter.TypeField == 1)
                                {
                                    string validGioVaoGioRa = CheckGioVaoGioRa(parameter.ThoiGian, chamCong.RaSang);
                                    if (validGioVaoGioRa != "")
                                    {
                                        return new CreateOrUpdateChamCongResult()
                                        {
                                            StatusCode = HttpStatusCode.NotFound,
                                            MessageCode = validGioVaoGioRa
                                        };
                                    }

                                    oldThoiGian = chamCong.VaoSang == null
                                        ? "--"
                                        : chamCong.VaoSang.Value.ToString(@"hh\:mm");

                                    chamCong.VaoSang = parameter.ThoiGian;

                                    context.ChamCong.Update(chamCong);
                                    context.SaveChanges();
                                }
                                //Ra sáng
                                else if (parameter.TypeField == 2)
                                {
                                    string validGioVaoGioRa = CheckGioVaoGioRa(chamCong.VaoSang, parameter.ThoiGian);
                                    if (validGioVaoGioRa != "")
                                    {
                                        return new CreateOrUpdateChamCongResult()
                                        {
                                            StatusCode = HttpStatusCode.NotFound,
                                            MessageCode = validGioVaoGioRa
                                        };
                                    }

                                    oldThoiGian = chamCong.RaSang == null
                                        ? "--"
                                        : chamCong.RaSang.Value.ToString(@"hh\:mm");

                                    chamCong.RaSang = parameter.ThoiGian;

                                    context.ChamCong.Update(chamCong);
                                    context.SaveChanges();
                                }
                                //Vào chiều
                                else if (parameter.TypeField == 3)
                                {
                                    string validGioVaoGioRa = CheckGioVaoGioRa(parameter.ThoiGian, chamCong.RaChieu);
                                    if (validGioVaoGioRa != "")
                                    {
                                        return new CreateOrUpdateChamCongResult()
                                        {
                                            StatusCode = HttpStatusCode.NotFound,
                                            MessageCode = validGioVaoGioRa
                                        };
                                    }

                                    oldThoiGian = chamCong.VaoChieu == null
                                        ? "--"
                                        : chamCong.VaoChieu.Value.ToString(@"hh\:mm");

                                    chamCong.VaoChieu = parameter.ThoiGian;

                                    context.ChamCong.Update(chamCong);
                                    context.SaveChanges();
                                }
                                //Ra chiều
                                else if (parameter.TypeField == 4)
                                {
                                    string validGioVaoGioRa = CheckGioVaoGioRa(chamCong.VaoChieu, parameter.ThoiGian);
                                    if (validGioVaoGioRa != "")
                                    {
                                        return new CreateOrUpdateChamCongResult()
                                        {
                                            StatusCode = HttpStatusCode.NotFound,
                                            MessageCode = validGioVaoGioRa
                                        };
                                    }

                                    oldThoiGian = chamCong.RaChieu == null
                                        ? "--"
                                        : chamCong.RaChieu.Value.ToString(@"hh\:mm");

                                    chamCong.RaChieu = parameter.ThoiGian;

                                    context.ChamCong.Update(chamCong);
                                    context.SaveChanges();
                                }
                            }

                            #region Lưu lịch sử vào dòng thòi gian
                            
                            var _emp = context.Employee.FirstOrDefault(x => x.EmployeeId == EmployeeId);
                            var ca = "";

                            //Vào sáng
                            if (parameter.TypeField == 1)
                            {
                                ca = "vào sáng";
                            }
                            //Ra sáng
                            else if (parameter.TypeField == 2)
                            {
                                ca = "ra sáng";
                            }
                            //Vào chiều
                            else if (parameter.TypeField == 3)
                            {
                                ca = "vào chiều";
                            }
                            //Ra chiều
                            else if (parameter.TypeField == 4)
                            {
                                ca = "ra chiều";
                            }

                            var contentNote = "";
                            contentNote = "<p>- <strong>" + emp.EmployeeCode + " - " + emp.EmployeeName +
                                          "</strong> đã thay đổi giờ " + ca +
                                          " của <strong>" + _emp.EmployeeCode + " - " + _emp.EmployeeName +
                                          "</strong> từ <strong>\"" + oldThoiGian + "\"</strong> sang <strong>\"" + newThoiGian +
                                          "\"</strong> trong ngày <strong>" + ngay + "</strong></p>";

                            Note note = new Note
                            {
                                NoteId = Guid.NewGuid(),
                                Description = contentNote,
                                Type = "ADD",
                                ObjectId = Guid.Empty,
                                ObjectNumber = 1,
                                ObjectType = NoteObjectType.TKCCT,
                                Active = true,
                                CreatedById = parameter.UserId,
                                CreatedDate = DateTime.Now,
                                NoteTitle = "đã thêm ghi chú"
                            };

                            context.Note.Add(note);
                            context.SaveChanges();

                            #endregion
                        }
                        //Nếu là Ca
                        else if (parameter.TypeUpdate == "CA")
                        {
                            //Create
                            if (chamCong == null)
                            {
                                //Nếu là Nghỉ phép thì sẽ kiểm tra xem nhân viên còn phép hay không
                                if (parameter.MaKyHieu == 1)
                                {
                                    //Nếu không còn phép thì bỏ qua
                                    if ((decimal) 0.5 > employee.SoNgayPhepConLai)
                                    {
                                        continue;
                                    }
                                    //Nếu còn phép thì trừ đi số ngày phép còn lại
                                    else
                                    {
                                        employee.SoNgayDaNghiPhep = employee.SoNgayDaNghiPhep + (decimal) 0.5;
                                        employee.SoNgayPhepConLai = employee.SoNgayPhepConLai - (decimal) 0.5;
                                        context.Employee.Update(employee);
                                    }
                                }

                                _chamCong.EmployeeId = EmployeeId;
                                _chamCong.NgayChamCong = parameter.NgayChamCong.Date;
                                _chamCong.CreatedById = parameter.UserId;
                                _chamCong.CreatedDate = DateTime.Now;

                                //Ca sáng
                                if (parameter.TypeField == 1)
                                {
                                    _chamCong.KyHieuVaoSang = parameter.MaKyHieu;
                                    _chamCong.KyHieuRaSang = parameter.MaKyHieu;
                                }
                                //Ca chiều
                                else if (parameter.TypeField == 2)
                                {
                                    _chamCong.KyHieuVaoChieu = parameter.MaKyHieu;
                                    _chamCong.KyHieuRaChieu = parameter.MaKyHieu;
                                }

                                context.ChamCong.Add(_chamCong);
                                context.SaveChanges();
                            }
                            //Update
                            else
                            {
                                //Nếu xóa ký hiệu
                                if (parameter.MaKyHieu == null)
                                {
                                    #region Tính lại thống kê đi muộn về sớm

                                    if (chamCong.VaoSang != null)
                                    {
                                        listThoiGianChamCong.Add(chamCong.VaoSang.Value);
                                    }

                                    if (chamCong.RaSang != null)
                                    {
                                        listThoiGianChamCong.Add(chamCong.RaSang.Value);
                                    }

                                    if (chamCong.VaoChieu != null)
                                    {
                                        listThoiGianChamCong.Add(chamCong.VaoChieu.Value);
                                    }

                                    if (chamCong.RaChieu != null)
                                    {
                                        listThoiGianChamCong.Add(chamCong.RaChieu.Value);
                                    }

                                    GetGioChamCong(EmployeeId, parameter.NgayChamCong.Date,
                                        listThoiGianChamCong,
                                        listCaLamViec, ngayTrongTuan,
                                        out bool isNoData,
                                        out _chamCong, out _listThongKeDiMuonVeSom);

                                    //Ca sáng
                                    if (parameter.TypeField == 1)
                                    {
                                        //Nếu ký hiệu cũ là nghỉ phép và ký hiệu mới khác nghỉ phép thì sẽ cộng thêm lại vào tổng số ngày phép còn lại của nhân viên
                                        if (chamCong.KyHieuVaoSang == 1)
                                        {
                                            employee.SoNgayDaNghiPhep = employee.SoNgayDaNghiPhep - (decimal) 0.5;
                                            employee.SoNgayPhepConLai = employee.SoNgayPhepConLai + (decimal) 0.5;
                                            context.Employee.Update(employee);
                                        }

                                        chamCong.KyHieuVaoSang = parameter.MaKyHieu;
                                        chamCong.KyHieuRaSang = parameter.MaKyHieu;
                                        context.ChamCong.Update(chamCong);
                                        context.SaveChanges();

                                        #region Xóa thống kê đi muộn về sớm

                                        var oldThongKeDmvs = context.ThongKeDiMuonVeSom.Where(x =>
                                            x.EmployeeId == EmployeeId &&
                                            x.NgayDmvs.Date == parameter.NgayChamCong.Date &&
                                            (x.LoaiDmvs == 1 || x.LoaiDmvs == 2)).ToList();

                                        if (oldThongKeDmvs.Count > 0)
                                        {
                                            context.ThongKeDiMuonVeSom.RemoveRange(oldThongKeDmvs);
                                            context.SaveChanges();
                                        }

                                        #endregion

                                        //Nếu là đi muộn về sớm thì
                                        if (_listThongKeDiMuonVeSom.Count > 0)
                                        {
                                            var listTk = _listThongKeDiMuonVeSom
                                                .Where(x => x.LoaiDmvs == 1 || x.LoaiDmvs == 2).ToList();

                                            if (listTk.Count > 0)
                                            {
                                                context.ThongKeDiMuonVeSom.AddRange(listTk);
                                                context.SaveChanges();
                                            }
                                        }
                                    }
                                    //Ca chiều
                                    else
                                    {
                                        //Nếu ký hiệu cũ là nghỉ phép và ký hiệu mới khác nghỉ phép thì sẽ cộng thêm lại vào tổng số ngày phép còn lại của nhân viên
                                        if (chamCong.KyHieuVaoChieu == 1)
                                        {
                                            employee.SoNgayDaNghiPhep = employee.SoNgayDaNghiPhep - (decimal)0.5;
                                            employee.SoNgayPhepConLai = employee.SoNgayPhepConLai + (decimal)0.5;
                                            context.Employee.Update(employee);
                                        }

                                        chamCong.KyHieuVaoChieu = parameter.MaKyHieu;
                                        chamCong.KyHieuRaChieu = parameter.MaKyHieu;
                                        context.ChamCong.Update(chamCong);
                                        context.SaveChanges();

                                        #region Xóa thống kê đi muộn về sớm

                                        var oldThongKeDmvs = context.ThongKeDiMuonVeSom.Where(x =>
                                            x.EmployeeId == EmployeeId &&
                                            x.NgayDmvs.Date == parameter.NgayChamCong.Date &&
                                            (x.LoaiDmvs == 3 || x.LoaiDmvs == 4)).ToList();

                                        if (oldThongKeDmvs.Count > 0)
                                        {
                                            context.ThongKeDiMuonVeSom.RemoveRange(oldThongKeDmvs);
                                            context.SaveChanges();
                                        }

                                        #endregion

                                        //Nếu là đi muộn về sớm thì
                                        if (_listThongKeDiMuonVeSom.Count > 0)
                                        {
                                            var listTk = _listThongKeDiMuonVeSom
                                                .Where(x => x.LoaiDmvs == 3 || x.LoaiDmvs == 4).ToList();

                                            if (listTk.Count > 0)
                                            {
                                                context.ThongKeDiMuonVeSom.AddRange(listTk);
                                                context.SaveChanges();
                                            }
                                        }
                                    }

                                    #endregion
                                }
                                //Nếu sửa ký hiệu
                                else
                                {
                                    //Ca sáng
                                    if (parameter.TypeField == 1)
                                    {
                                        //Nếu ký hiệu cũ là nghỉ phép và ký hiệu mới khác nghỉ phép thì sẽ cộng thêm lại vào tổng số ngày phép còn lại của nhân viên
                                        if (chamCong.KyHieuVaoSang == 1 && parameter.MaKyHieu != 1)
                                        {
                                            employee.SoNgayDaNghiPhep = employee.SoNgayDaNghiPhep - (decimal)0.5;
                                            employee.SoNgayPhepConLai = employee.SoNgayPhepConLai + (decimal)0.5;
                                            context.Employee.Update(employee);
                                        }
                                        //Nếu ký hiệu cũ khác Nghỉ phép và ký hiệu mới là Nghỉ phép thì sẽ trừ đi số ngày phép còn lại của nhân viên
                                        else if (chamCong.KyHieuVaoSang != 1 && parameter.MaKyHieu == 1)
                                        {
                                            //Nếu không còn phép thì bỏ qua
                                            if ((decimal)0.5 > employee.SoNgayPhepConLai)
                                            {
                                                continue;
                                            }
                                            //Nếu còn phép thì trừ đi số ngày phép còn lại
                                            else
                                            {
                                                employee.SoNgayDaNghiPhep = employee.SoNgayDaNghiPhep + (decimal)0.5;
                                                employee.SoNgayPhepConLai = employee.SoNgayPhepConLai - (decimal)0.5;
                                                context.Employee.Update(employee);
                                            }
                                        }

                                        #region Xóa thống kê đi muộn về sớm

                                        var oldThongKeDmvs = context.ThongKeDiMuonVeSom.Where(x =>
                                            x.EmployeeId == EmployeeId &&
                                            x.NgayDmvs.Date == parameter.NgayChamCong.Date &&
                                            (x.LoaiDmvs == 1 || x.LoaiDmvs == 2)).ToList();

                                        if (oldThongKeDmvs.Count > 0)
                                        {
                                            context.ThongKeDiMuonVeSom.RemoveRange(oldThongKeDmvs);
                                            context.SaveChanges();
                                        }

                                        #endregion

                                        chamCong.KyHieuVaoSang = parameter.MaKyHieu;
                                        chamCong.KyHieuRaSang = parameter.MaKyHieu;
                                        context.ChamCong.Update(chamCong);
                                        context.SaveChanges();
                                    }
                                    //Ca chiều
                                    else
                                    {
                                        //Nếu ký hiệu cũ là nghỉ phép và ký hiệu mới khác nghỉ phép thì sẽ cộng thêm lại vào tổng số ngày phép còn lại của nhân viên
                                        if (chamCong.KyHieuVaoChieu == 1 && parameter.MaKyHieu != 1)
                                        {
                                            employee.SoNgayDaNghiPhep = employee.SoNgayDaNghiPhep - (decimal)0.5;
                                            employee.SoNgayPhepConLai = employee.SoNgayPhepConLai + (decimal)0.5;
                                            context.Employee.Update(employee);
                                        }
                                        //Nếu ký hiệu cũ khác Nghỉ phép và ký hiệu mới là Nghỉ phép thì sẽ trừ đi số ngày phép còn lại của nhân viên
                                        else if (chamCong.KyHieuVaoChieu != 1 && parameter.MaKyHieu == 1)
                                        {
                                            //Nếu không còn phép thì bỏ qua
                                            if ((decimal)0.5 > employee.SoNgayPhepConLai)
                                            {
                                                continue;
                                            }
                                            //Nếu còn phép thì trừ đi số ngày phép còn lại
                                            else
                                            {
                                                employee.SoNgayDaNghiPhep = employee.SoNgayDaNghiPhep + (decimal)0.5;
                                                employee.SoNgayPhepConLai = employee.SoNgayPhepConLai - (decimal)0.5;
                                                context.Employee.Update(employee);
                                            }
                                        }

                                        #region Xóa thống kê đi muộn về sớm

                                        var oldThongKeDmvs = context.ThongKeDiMuonVeSom.Where(x =>
                                            x.EmployeeId == EmployeeId &&
                                            x.NgayDmvs.Date == parameter.NgayChamCong.Date &&
                                            (x.LoaiDmvs == 3 || x.LoaiDmvs == 4)).ToList();

                                        if (oldThongKeDmvs.Count > 0)
                                        {
                                            context.ThongKeDiMuonVeSom.RemoveRange(oldThongKeDmvs);
                                            context.SaveChanges();
                                        }

                                        #endregion

                                        chamCong.KyHieuVaoChieu = parameter.MaKyHieu;
                                        chamCong.KyHieuRaChieu = parameter.MaKyHieu;
                                        context.ChamCong.Update(chamCong);
                                        context.SaveChanges();
                                    }
                                }
                            }
                        }
                        //Nếu là Ngày
                        else if (parameter.TypeUpdate == "NGAY")
                        {
                            //Create
                            if (chamCong == null)
                            {
                                //Nếu là Nghỉ phép thì sẽ kiểm tra xem nhân viên còn phép hay không
                                if (parameter.MaKyHieu == 1)
                                {
                                    //Nếu không còn phép thì bỏ qua
                                    if (1 > employee.SoNgayPhepConLai)
                                    {
                                        continue;
                                    }
                                    //Nếu còn phép thì trừ đi số ngày phép còn lại
                                    else
                                    {
                                        employee.SoNgayDaNghiPhep = employee.SoNgayDaNghiPhep + 1;
                                        employee.SoNgayPhepConLai = employee.SoNgayPhepConLai - 1;
                                        context.Employee.Update(employee);
                                    }
                                }

                                _chamCong.EmployeeId = EmployeeId;
                                _chamCong.NgayChamCong = parameter.NgayChamCong.Date;
                                _chamCong.KyHieuVaoSang = parameter.MaKyHieu;
                                _chamCong.KyHieuRaSang = parameter.MaKyHieu;
                                _chamCong.KyHieuVaoChieu = parameter.MaKyHieu;
                                _chamCong.KyHieuRaChieu = parameter.MaKyHieu;
                                _chamCong.CreatedById = parameter.UserId;
                                _chamCong.CreatedDate = DateTime.Now;

                                context.ChamCong.Add(_chamCong);
                                context.SaveChanges();
                            }
                            //Update 
                            else
                            {
                                #region Kiểm tra số ngày phép

                                //Nếu ký hiệu mới là Nghỉ phép 
                                if (parameter.MaKyHieu == 1)
                                {
                                    //Nếu ký hiệu cũ khác Nghỉ phép và ký hiệu mới là Nghỉ phép (cả ngày)
                                    if (chamCong.KyHieuVaoSang != 1 && chamCong.KyHieuVaoChieu != 1)
                                    {
                                        //Nếu không còn phép thì bỏ qua
                                        if (1 > employee.SoNgayPhepConLai)
                                        {
                                            continue;
                                        }
                                        //Nếu còn phép thì trừ đi số ngày phép còn lại
                                        else
                                        {
                                            employee.SoNgayDaNghiPhep = employee.SoNgayDaNghiPhep + 1;
                                            employee.SoNgayPhepConLai = employee.SoNgayPhepConLai - 1;
                                            context.Employee.Update(employee);
                                        }
                                    }
                                    //Nếu ký hiệu cũ khác Nghỉ phép và ký hiệu mới là Nghỉ phép (nửa ngày)
                                    else if ((chamCong.KyHieuVaoSang == 1 && chamCong.KyHieuVaoChieu != 1) ||
                                              (chamCong.KyHieuVaoSang != 1 && chamCong.KyHieuVaoChieu == 1))
                                    {
                                        //Nếu không còn phép thì bỏ qua
                                        if ((decimal) 0.5 > employee.SoNgayPhepConLai)
                                        {
                                            continue;
                                        }
                                        //Nếu còn phép thì trừ đi số ngày phép còn lại
                                        else
                                        {
                                            employee.SoNgayDaNghiPhep = employee.SoNgayDaNghiPhep + (decimal) 0.5;
                                            employee.SoNgayPhepConLai = employee.SoNgayPhepConLai - (decimal) 0.5;
                                            context.Employee.Update(employee);
                                        }
                                    }
                                }
                                //Nếu ký hiệu mới không phải nghỉ phép
                                else
                                {
                                    //Nếu ký hiệu cũ là Nghỉ phép (cả ngày)
                                    if (chamCong.KyHieuVaoSang == 1 && chamCong.KyHieuVaoChieu == 1)
                                    {
                                        employee.SoNgayDaNghiPhep = employee.SoNgayDaNghiPhep - 1;
                                        employee.SoNgayPhepConLai = employee.SoNgayPhepConLai + 1;
                                        context.Employee.Update(employee);
                                    }
                                    //Nếu ký hiệu cũ có ca ngày hoặc ca sáng là Nghỉ phép (nửa ngày)
                                    else if ((chamCong.KyHieuVaoSang == 1 && chamCong.KyHieuVaoChieu != 1) ||
                                             chamCong.KyHieuVaoSang != 1 && chamCong.KyHieuVaoChieu == 1)
                                    {
                                        employee.SoNgayDaNghiPhep = employee.SoNgayDaNghiPhep - (decimal)0.5;
                                        employee.SoNgayPhepConLai = employee.SoNgayPhepConLai + (decimal) 0.5;
                                        context.Employee.Update(employee);
                                    }
                                }

                                #endregion

                                #region Xóa thống kê đi muộn về sớm

                                var oldThongKeDmvs = context.ThongKeDiMuonVeSom.Where(x =>
                                    x.EmployeeId == EmployeeId &&
                                    x.NgayDmvs.Date == parameter.NgayChamCong.Date).ToList();

                                if (oldThongKeDmvs.Count > 0)
                                {
                                    context.ThongKeDiMuonVeSom.RemoveRange(oldThongKeDmvs);
                                    context.SaveChanges();
                                }

                                #endregion

                                //Nếu xóa ký hiệu
                                if (parameter.MaKyHieu == null)
                                {
                                    #region Tính lại thống kê đi muộn về sớm

                                    if (chamCong.VaoSang != null)
                                    {
                                        listThoiGianChamCong.Add(chamCong.VaoSang.Value);
                                    }

                                    if (chamCong.RaSang != null)
                                    {
                                        listThoiGianChamCong.Add(chamCong.RaSang.Value);
                                    }

                                    if (chamCong.VaoChieu != null)
                                    {
                                        listThoiGianChamCong.Add(chamCong.VaoChieu.Value);
                                    }

                                    if (chamCong.RaChieu != null)
                                    {
                                        listThoiGianChamCong.Add(chamCong.RaChieu.Value);
                                    }

                                    GetGioChamCong(EmployeeId, parameter.NgayChamCong.Date,
                                        listThoiGianChamCong,
                                        listCaLamViec, ngayTrongTuan,
                                        out bool isNoData,
                                        out _chamCong, out _listThongKeDiMuonVeSom);

                                    chamCong.KyHieuVaoSang = parameter.MaKyHieu;
                                    chamCong.KyHieuRaSang = parameter.MaKyHieu;
                                    chamCong.KyHieuVaoChieu = parameter.MaKyHieu;
                                    chamCong.KyHieuRaChieu = parameter.MaKyHieu;
                                    context.ChamCong.Update(chamCong);
                                    context.SaveChanges();

                                    //Nếu là đi muộn về sớm thì
                                    if (_listThongKeDiMuonVeSom.Count > 0)
                                    {
                                        context.ThongKeDiMuonVeSom.AddRange(_listThongKeDiMuonVeSom);
                                        context.SaveChanges();
                                    }

                                    #endregion
                                }
                                //Nếu sửa ký hiệu
                                else
                                {
                                    chamCong.KyHieuVaoSang = parameter.MaKyHieu;
                                    chamCong.KyHieuRaSang = parameter.MaKyHieu;
                                    chamCong.KyHieuVaoChieu = parameter.MaKyHieu;
                                    chamCong.KyHieuRaChieu = parameter.MaKyHieu;
                                    context.ChamCong.Update(chamCong);
                                    context.SaveChanges();
                                }
                            }
                        }
                    }

                    trans.Commit();

                    return new CreateOrUpdateChamCongResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = "Lưu thành công"
                    };
                }
            }
            catch (Exception e)
            {
                return new CreateOrUpdateChamCongResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreateOrUpdateCauHinhChamCongOtResult CreateOrUpdateCauHinhChamCongOt(
            CreateOrUpdateCauHinhChamCongOtParameter parameter)
        {
            try
            {
                string mess = "";

                //Create
                if (parameter.CauHinhChamCongOt.CauHinhChamCongOtId == null)
                {
                    var cauHinh = parameter.CauHinhChamCongOt;
                    cauHinh.CreatedById = parameter.UserId;

                    context.CauHinhChamCongOt.Add(cauHinh.ToEntity());
                    context.SaveChanges();

                    mess = "Thêm mới thành công";
                }
                //Update
                else
                {
                    var cauHinh = context.CauHinhChamCongOt.FirstOrDefault(x =>
                        x.CauHinhChamCongOtId == parameter.CauHinhChamCongOt.CauHinhChamCongOtId);
                    if (cauHinh == null)
                    {
                        return new CreateOrUpdateCauHinhChamCongOtResult()
                        {
                            StatusCode = HttpStatusCode.NotFound,
                            MessageCode = "Cấu hình không tồn tại trên hệ thống"
                        };
                    }

                    cauHinh.SoPhut = parameter.CauHinhChamCongOt.SoPhut;

                    context.CauHinhChamCongOt.Update(cauHinh);
                    context.SaveChanges();

                    mess = "Lưu thành công";
                }

                return new CreateOrUpdateCauHinhChamCongOtResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = mess
                };
            }
            catch (Exception e)
            {
                return new CreateOrUpdateCauHinhChamCongOtResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetTkThoiGianOtResult GetTkThoiGianOt(GetTkThoiGianOtParameter parameter)
        {
            try
            {
                var listData = new List<List<DataRowModel>>();

                var listEmp = new List<Employee>();

                //Nếu hiển thị tất cả nhân viên 
                if (parameter.IsShowOption == 0)
                {
                    listEmp = context.Employee
                        .Where(x => !String.IsNullOrWhiteSpace(x.CodeMayChamCong) &&
                                    (parameter.ListEmployeeId.Count == 0 ||
                                     parameter.ListEmployeeId.Contains(x.EmployeeId)))
                        .OrderBy(z => z.EmployeeName)
                        .ToList();
                }
                //Nếu chỉ hiển thị nhân viên đang làm việc
                else if (parameter.IsShowOption == 1)
                {
                    listEmp = context.Employee
                        .Join(context.User,
                            emp => emp.EmployeeId,
                            user => user.EmployeeId,
                            (emp, user) => new { Emp = emp, User = user }) // selection
                        .Where(x => !String.IsNullOrWhiteSpace(x.Emp.CodeMayChamCong) &&
                                    (parameter.ListEmployeeId.Count == 0 ||
                                     parameter.ListEmployeeId.Contains(x.Emp.EmployeeId)) &&
                                     x.Emp.Active == true)
                        .Select(y => new Employee
                        {
                            EmployeeId = y.Emp.EmployeeId,
                            CodeMayChamCong = y.Emp.CodeMayChamCong,
                            EmployeeCode = y.Emp.EmployeeCode,
                            EmployeeName = y.Emp.EmployeeName,
                        })
                        .OrderBy(z => z.EmployeeName)
                        .ToList();
                }
                //Nếu chỉ hiển thị nhân viên đã nghỉ viêc
                else if (parameter.IsShowOption == 2)
                {
                    listEmp = context.Employee
                        .Join(context.User,
                            emp => emp.EmployeeId,
                            user => user.EmployeeId,
                            (emp, user) => new { Emp = emp, User = user }) // selection
                        .Where(x => !String.IsNullOrWhiteSpace(x.Emp.CodeMayChamCong) &&
                                    (parameter.ListEmployeeId.Count == 0 ||
                                     parameter.ListEmployeeId.Contains(x.Emp.EmployeeId)) &&
                                    x.Emp.Active == false && x.User.Active == false)
                        .Select(y => new Employee
                        {
                            EmployeeId = y.Emp.EmployeeId,
                            CodeMayChamCong = y.Emp.CodeMayChamCong,
                            EmployeeCode = y.Emp.EmployeeCode,
                            EmployeeName = y.Emp.EmployeeName,
                        })
                        .OrderBy(z => z.EmployeeName)
                        .ToList();
                }

                //Lấy list các loại OT
                var otCategoryTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LOAIOT")?.CategoryTypeId;
                var listLoaiOt = context.Category
                    .Where(x => x.CategoryTypeId == otCategoryTypeId)
                    .Select(y => new CategoryEntityModel(y)).ToList();

                var listEmpId = listEmp.Select(y => y.EmployeeId).ToList();
                var listTongHopChamCongOt = context.TongHopChamCongOt
                    .Where(x => (parameter.TuNgay == null || x.NgayChamCong.Date >= parameter.TuNgay.Date) && 
                                (parameter.DenNgay == null || x.NgayChamCong.Date <= parameter.DenNgay.Date) && 
                                listEmpId.Contains(x.EmployeeId))
                    .GroupBy(g => new {g.EmployeeId, g.LoaiOtId})
                    .Select(y => new TongHopChamCongOtModel()
                    {
                        EmployeeId = y.Key.EmployeeId,
                        LoaiOtId = y.Key.LoaiOtId,
                        SoPhut = y.Sum(s => s.SoPhut),
                        SoGio = y.Sum(s => s.SoGio),
                    }).OrderBy(z => z.LoaiOtId).ToList();

                var listLoaiOtId = listLoaiOt.Select(y => y.CategoryId).Distinct().OrderBy(z => z).ToList();

                #region Data

                int stt = 0;
                listEmp.ForEach(item =>
                {
                    stt++;
                    var listDataRow = new List<DataRowModel>();

                    var dataEmpIdRow = new DataRowModel();
                    dataEmpIdRow.ColumnKey = "employee_id";
                    dataEmpIdRow.ColumnValue = item.EmployeeId.ToString();
                    dataEmpIdRow.Width = "60px";
                    dataEmpIdRow.TextAlign = "center";
                    dataEmpIdRow.IsShow = false;
                    listDataRow.Add(dataEmpIdRow);

                    var dataRow = new DataRowModel();
                    dataRow.ColumnKey = "stt";
                    dataRow.ColumnValue = stt.ToString();
                    dataRow.ValueType = ValueTypeEnum.NUMBER;
                    dataRow.Width = "60px";
                    dataRow.TextAlign = "center";
                    listDataRow.Add(dataRow);

                    var dataRow2 = new DataRowModel();
                    dataRow2.ColumnKey = "code";
                    dataRow2.ColumnValue = item.CodeMayChamCong;
                    dataRow2.Width = "60px";
                    dataRow2.TextAlign = "center";
                    listDataRow.Add(dataRow2);

                    var dataRow3 = new DataRowModel();
                    dataRow3.ColumnKey = "name";
                    dataRow3.ColumnValue = item.EmployeeCode + " - " + item.EmployeeName;
                    dataRow3.Width = "250px";
                    dataRow3.TextAlign = "left";
                    listDataRow.Add(dataRow3);

                    int index = 0;
                    listLoaiOtId.ForEach(loaiOtId =>
                    {
                        index++;
                        var data = listTongHopChamCongOt.FirstOrDefault(x => x.EmployeeId == item.EmployeeId &&
                                                                             x.LoaiOtId == loaiOtId);
                        decimal soGio = 0;
                        if (data != null) soGio = data.SoGio;

                        var _dataRow = new DataRowModel();
                        _dataRow.ColumnKey = "index_" + index;
                        _dataRow.ColumnValue = soGio.ToString();
                        _dataRow.ValueType = ValueTypeEnum.NUMBER;
                        _dataRow.Width = "150px";
                        _dataRow.TextAlign = "right";
                        listDataRow.Add(_dataRow);
                    });

                    listData.Add(listDataRow);
                });

                #endregion

                #region Header

                var listDataHeader = new List<List<DataHeaderModel>>();

                /* tr1 */
                var listHeader1 = new List<DataHeaderModel>()
                {
                    new DataHeaderModel()
                    {
                        ColumnKey = "stt",
                        ColumnValue = "#",
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "60px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "code",
                        ColumnValue = "Code",
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "60px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "name",
                        ColumnValue = "Họ tên",
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "250px",
                    },
                };

                int _index = 0;
                listLoaiOtId.ForEach(loaiOtId =>
                {
                    string headerName = listLoaiOt
                        .FirstOrDefault(x => x.CategoryId == loaiOtId)?.CategoryName;

                    _index++;
                    var _dataHeader = new DataHeaderModel();
                    _dataHeader.ColumnKey = "index_" + _index;
                    _dataHeader.ColumnValue = "Tổng số giờ OT " + headerName;
                    _dataHeader.Rowspan = 0;
                    _dataHeader.Colspan = 0;
                    _dataHeader.Width = "150px";

                    listHeader1.Add(_dataHeader);
                });

                listDataHeader.Add(listHeader1);

                #endregion

                return new GetTkThoiGianOtResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "OK",
                    ListData = listData,
                    ListDataHeader = listDataHeader
                };
            }
            catch (Exception e)
            {
                return new GetTkThoiGianOtResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreateOrUpdateCauHinhOtCaNgayResult CreateOrUpdateCauHinhOtCaNgay(CreateOrUpdateCauHinhOtCaNgayParameter parameter)
        {
            try
            {
                string mess = "";
                var cauHinhOtCaNgay = parameter.CauHinhOtCaNgay;

                //Create
                if (cauHinhOtCaNgay.CauHinhOtCaNgayId == null)
                {
                    context.CauHinhOtCaNgay.Add(cauHinhOtCaNgay.ToEntity());
                    context.SaveChanges();

                    mess = "Tạo thành công";
                }
                //Update
                else
                {
                    var cauHinh = context.CauHinhOtCaNgay.FirstOrDefault(x =>
                        x.CauHinhOtCaNgayId == cauHinhOtCaNgay.CauHinhOtCaNgayId);
                    if (cauHinh == null)
                    {
                        return new CreateOrUpdateCauHinhOtCaNgayResult()
                        {
                            StatusCode = HttpStatusCode.NotFound,
                            MessageCode = "Cấu hình không tồn tại trên hệ thống"
                        };
                    }

                    cauHinh.GioVaoSang = cauHinhOtCaNgay.GioVaoSang;
                    cauHinh.GioRaSang = cauHinhOtCaNgay.GioRaSang;
                    cauHinh.GioKetThucSang = cauHinhOtCaNgay.GioKetThucSang;
                    cauHinh.GioVaoChieu = cauHinhOtCaNgay.GioVaoChieu;
                    cauHinh.GioRaChieu = cauHinhOtCaNgay.GioRaChieu;
                    cauHinh.GioKetThucChieu = cauHinhOtCaNgay.GioKetThucChieu;

                    context.CauHinhOtCaNgay.Update(cauHinh);
                    context.SaveChanges();

                    mess = "Lưu thành công";
                }

                return new CreateOrUpdateCauHinhOtCaNgayResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = mess
                };
            }
            catch (Exception e)
            {
                return new CreateOrUpdateCauHinhOtCaNgayResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreateOrUpdateCauHinhThueTncnResult CreateOrUpdateCauHinhThueTncn(CreateOrUpdateCauHinhThueTncnParameter parameter)
        {
            try
            {
                string mess = "";
                var cauHinhThueTncn = parameter.CauHinhThueTncn;

                //Create
                if (cauHinhThueTncn.CauHinhThueTncnId == null)
                {
                    context.CauHinhThueTncn.Add(cauHinhThueTncn.ToEntity());
                    context.SaveChanges();

                    mess = "Tạo thành công";
                }
                //Update
                else
                {
                    var cauHinh = context.CauHinhThueTncn.FirstOrDefault(x =>
                        x.CauHinhThueTncnId == cauHinhThueTncn.CauHinhThueTncnId);
                    if (cauHinh == null)
                    {
                        return new CreateOrUpdateCauHinhThueTncnResult()
                        {
                            StatusCode = HttpStatusCode.OK,
                            MessageCode = "Cấu hình không tồn tại trên hệ thống"
                        };
                    }

                    cauHinh.SoTienTu = cauHinhThueTncn.SoTienTu;
                    cauHinh.SoTienDen = cauHinhThueTncn.SoTienDen;
                    cauHinh.PhanTramThue = cauHinhThueTncn.PhanTramThue;
                    cauHinh.SoBiTruTheoCongThuc = cauHinhThueTncn.SoBiTruTheoCongThuc;

                    context.CauHinhThueTncn.Update(cauHinh);
                    context.SaveChanges();

                    mess = "Lưu thành công";
                }

                return new CreateOrUpdateCauHinhThueTncnResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = mess
                };
            }
            catch (Exception e)
            {
                return new CreateOrUpdateCauHinhThueTncnResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public DeleteCauHinhThueTncnResult DeleteCauHinhThueTncn(DeleteCauHinhThueTncnParameter parameter)
        {
            try
            {
                var cauHinh =
                    context.CauHinhThueTncn.FirstOrDefault(x => x.CauHinhThueTncnId == parameter.CauHinhThueTncnId);
                if (cauHinh == null)
                {
                    return new DeleteCauHinhThueTncnResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Cấu hình không tồn tại trên hệ thống"
                    };
                }

                context.CauHinhThueTncn.Remove(cauHinh);
                context.SaveChanges();

                return new DeleteCauHinhThueTncnResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Xóa thành công"
                };
            }
            catch (Exception e)
            {
                return new DeleteCauHinhThueTncnResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetChamCongOtByEmpIdResult GetChamCongOtByEmpId(GetChamCongOtByEmpIdParameter parameter)
        {
            try
            {
                if (parameter.TuNgay.Date > parameter.DenNgay.Date)
                {
                    return new GetChamCongOtByEmpIdResult()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                        MessageCode = "Bộ lọc Từ ngày phải trước Đến ngày"
                    };
                }

                var listData = new List<List<DataRowModel>>();
                var listDataHeader = new List<List<DataHeaderModel>>();

                //Lấy list các loại OT
                var otCategoryTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LOAIOT")?.CategoryTypeId;
                var listLoaiOt = context.Category
                    .Where(x => x.CategoryTypeId == otCategoryTypeId)
                    .Select(y => new CategoryEntityModel(y)).ToList();

                var listChamCongOt = context.ChamCongOt.Where(x =>
                        (parameter.TuNgay == null || x.NgayChamCong.Date >= parameter.TuNgay.Date) &&
                        (parameter.DenNgay == null || x.NgayChamCong.Date <= parameter.DenNgay.Date) &&
                        x.EmployeeId == parameter.EmployeeId)
                    .OrderByDescending(z => z.NgayChamCong).ToList();

                var listLoaiOtId = listLoaiOt.Select(y => y.CategoryId).Distinct().OrderBy(z => z).ToList();

                var listDate = new List<DateTime>();
                var startDate = parameter.TuNgay;
                var endDate = parameter.DenNgay;
                while (startDate <= endDate)
                {
                    listDate.Add(startDate);
                    startDate = startDate.AddDays(1).Date;
                }

                listDate = listDate.OrderByDescending(z => z).ToList();

                #region Data

                listDate.ForEach(item =>
                {
                    var listDataRow = new List<DataRowModel>();

                    var dataRow = new DataRowModel();
                    dataRow.ColumnKey = "ngay";
                    dataRow.ColumnValue = item.ToString("dd/MM/yyyy");
                    dataRow.Width = "90px";
                    dataRow.TextAlign = "center";
                    listDataRow.Add(dataRow);

                    int index = 0;
                    listLoaiOtId.ForEach(loaiOtId =>
                    {
                        var dataChamCongOt = listChamCongOt.FirstOrDefault(x => x.NgayChamCong.Date == item &&
                                                                                x.LoaiOtId == loaiOtId);

                        index++;
                        var dataRowVaoSang = new DataRowModel();
                        dataRowVaoSang.ColumnKey = "index_vs_" + loaiOtId + "_" + index;
                        dataRowVaoSang.ColumnValue = ConvertGioChamCongOt(dataChamCongOt?.GioVaoSang);
                        dataRowVaoSang.Width = "50px";
                        dataRowVaoSang.TextAlign = "center";
                        listDataRow.Add(dataRowVaoSang);

                        index++;
                        var dataRowRaSang = new DataRowModel();
                        dataRowRaSang.ColumnKey = "index_rs_" + loaiOtId + "_" + index;
                        dataRowRaSang.ColumnValue = ConvertGioChamCongOt(dataChamCongOt?.GioRaSang);
                        dataRowRaSang.Width = "50px";
                        dataRowRaSang.TextAlign = "center";
                        listDataRow.Add(dataRowRaSang);

                        index++;
                        var dataRowVaoChieu = new DataRowModel();
                        dataRowVaoChieu.ColumnKey = "index_vc_" + loaiOtId + "_" + index;
                        dataRowVaoChieu.ColumnValue = ConvertGioChamCongOt(dataChamCongOt?.GioVaoChieu);
                        dataRowVaoChieu.Width = "50px";
                        dataRowVaoChieu.TextAlign = "center";
                        listDataRow.Add(dataRowVaoChieu);

                        index++;
                        var dataRowRaChieu = new DataRowModel();
                        dataRowRaChieu.ColumnKey = "index_rc_" + loaiOtId + "_" + index;
                        dataRowRaChieu.ColumnValue = ConvertGioChamCongOt(dataChamCongOt?.GioRaChieu);
                        dataRowRaChieu.Width = "50px";
                        dataRowRaChieu.TextAlign = "center";
                        listDataRow.Add(dataRowRaChieu);

                        index++;
                        var dataRowVaoToi = new DataRowModel();
                        dataRowVaoToi.ColumnKey = "index_vt_" + loaiOtId + "_" + index;
                        dataRowVaoToi.ColumnValue = ConvertGioChamCongOt(dataChamCongOt?.GioVaoToi);
                        dataRowVaoToi.Width = "50px";
                        dataRowVaoToi.TextAlign = "center";
                        listDataRow.Add(dataRowVaoToi);

                        index++;
                        var dataRowRaToi = new DataRowModel();
                        dataRowRaToi.ColumnKey = "index_rt_" + loaiOtId + "_" + index;
                        dataRowRaToi.ColumnValue = ConvertGioChamCongOt(dataChamCongOt?.GioRaToi);
                        dataRowRaToi.Width = "50px";
                        dataRowRaToi.TextAlign = "center";
                        listDataRow.Add(dataRowRaToi);
                    });

                    listData.Add(listDataRow);
                });

                #endregion

                #region Header

                /* tr1 */
                var listHeader1 = new List<DataHeaderModel>()
                {
                    new DataHeaderModel()
                    {
                        ColumnKey = "ngay",
                        ColumnValue = "Ngày",
                        Rowspan = 2,
                        Colspan = 0,
                        Width = "90px",
                    },
                };

                int _index = 0;
                listLoaiOtId.ForEach(item =>
                {
                    _index++;
                    var _dataHeader = new DataHeaderModel();
                    _dataHeader.ColumnKey = "index_loaiOt_" + _index;
                    _dataHeader.ColumnValue = "Loại OT " + listLoaiOt.FirstOrDefault(x => x.CategoryId == item)?.CategoryName;
                    _dataHeader.Rowspan = 0;
                    _dataHeader.Colspan = 6;
                    _dataHeader.Width = "300px";

                    listHeader1.Add(_dataHeader);
                });

                listDataHeader.Add(listHeader1);

                /* tr2 */
                var listHeader2 = new List<DataHeaderModel>();
                int _indexCa = 0;
                listLoaiOtId.ForEach(item =>
                {
                    //Vào sáng
                    _indexCa++;
                    var _dataHeader1 = new DataHeaderModel();
                    _dataHeader1.ColumnKey = "index_ca_" + _indexCa;
                    _dataHeader1.ColumnValue = "Vào sáng";
                    _dataHeader1.Rowspan = 0;
                    _dataHeader1.Colspan = 0;
                    _dataHeader1.Width = "50px";

                    listHeader2.Add(_dataHeader1);

                    //Ra sáng
                    _indexCa++;
                    var _dataHeader2 = new DataHeaderModel();
                    _dataHeader2.ColumnKey = "index_ca_" + _indexCa;
                    _dataHeader2.ColumnValue = "Ra sáng";
                    _dataHeader2.Rowspan = 0;
                    _dataHeader2.Colspan = 0;
                    _dataHeader2.Width = "50px";

                    listHeader2.Add(_dataHeader2);

                    //Vào chiều
                    _indexCa++;
                    var _dataHeader3 = new DataHeaderModel();
                    _dataHeader3.ColumnKey = "index_ca_" + _indexCa;
                    _dataHeader3.ColumnValue = "Vào chiều";
                    _dataHeader3.Rowspan = 0;
                    _dataHeader3.Colspan = 0;
                    _dataHeader3.Width = "50px";

                    listHeader2.Add(_dataHeader3);

                    //Ra chiều
                    _indexCa++;
                    var _dataHeader4 = new DataHeaderModel();
                    _dataHeader4.ColumnKey = "index_ca_" + _indexCa;
                    _dataHeader4.ColumnValue = "Ra chiều";
                    _dataHeader4.Rowspan = 0;
                    _dataHeader4.Colspan = 0;
                    _dataHeader4.Width = "50px";

                    listHeader2.Add(_dataHeader4);

                    //Vào tối
                    _indexCa++;
                    var _dataHeader5 = new DataHeaderModel();
                    _dataHeader5.ColumnKey = "index_ca_" + _indexCa;
                    _dataHeader5.ColumnValue = "Vào tối";
                    _dataHeader5.Rowspan = 0;
                    _dataHeader5.Colspan = 0;
                    _dataHeader5.Width = "50px";

                    listHeader2.Add(_dataHeader5);

                    //Ra tối
                    _indexCa++;
                    var _dataHeader6 = new DataHeaderModel();
                    _dataHeader6.ColumnKey = "index_ca_" + _indexCa;
                    _dataHeader6.ColumnValue = "Ra tối";
                    _dataHeader6.Rowspan = 0;
                    _dataHeader6.Colspan = 0;
                    _dataHeader6.Width = "50px";

                    listHeader2.Add(_dataHeader6);
                });

                listDataHeader.Add(listHeader2);

                #endregion

                return new GetChamCongOtByEmpIdResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "OK",
                    ListData = listData,
                    ListDataHeader = listDataHeader,
                    ListLoaiOt = listLoaiOt
                };
            }
            catch (Exception e)
            {
                return new GetChamCongOtByEmpIdResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreateOrUpdateChamCongOtResult CreateOrUpdateChamCongOt(CreateOrUpdateChamCongOtParameter parameter)
        {
            try
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    var chamCongOt = context.ChamCongOt.FirstOrDefault(x => x.EmployeeId == parameter.EmployeeId &&
                                                                        x.NgayChamCong.Date ==
                                                                        parameter.NgayChamCong.Date &&
                                                                        x.LoaiOtId == parameter.LoaiOtId);

                    var otCategoryTypeId = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LOAIOT")?.CategoryTypeId;
                    var listLoaiOt = context.Category
                        .Where(x => x.CategoryTypeId == otCategoryTypeId)
                        .Select(y => new CategoryEntityModel(y)).ToList();

                    var oldThoiGian = "--";
                    var newThoiGian = parameter.ThoiGian == null ? "--" : parameter.ThoiGian.Value.ToString(@"hh\:mm");
                    var loaiOt = listLoaiOt.FirstOrDefault(x => x.CategoryId == parameter.LoaiOtId)?.CategoryName;
                    var ngay = parameter.NgayChamCong.ToString("dd/MM/yyyy");

                    //Create
                    if (chamCongOt == null)
                    {
                        var _chamCongOt = new ChamCongOt();
                        _chamCongOt.EmployeeId = parameter.EmployeeId;
                        _chamCongOt.NgayChamCong = parameter.NgayChamCong.Date;
                        _chamCongOt.LoaiOtId = parameter.LoaiOtId;

                        if (parameter.Type == "vs") _chamCongOt.GioVaoSang = parameter.ThoiGian;
                        else if (parameter.Type == "rs") _chamCongOt.GioRaSang = parameter.ThoiGian;
                        else if (parameter.Type == "vc") _chamCongOt.GioVaoChieu = parameter.ThoiGian;
                        else if (parameter.Type == "rc") _chamCongOt.GioRaChieu = parameter.ThoiGian;
                        else if (parameter.Type == "vt") _chamCongOt.GioVaoToi = parameter.ThoiGian;
                        else if (parameter.Type == "rt") _chamCongOt.GioRaToi = parameter.ThoiGian;

                        context.ChamCongOt.Add(_chamCongOt);
                        context.SaveChanges();
                    }
                    //Update
                    else
                    {
                        if (parameter.Type == "vs")
                        {
                            oldThoiGian = chamCongOt.GioVaoSang == null
                                ? "--"
                                : chamCongOt.GioVaoSang.Value.ToString(@"hh\:mm");

                            chamCongOt.GioVaoSang = parameter.ThoiGian;
                        }
                        else if (parameter.Type == "rs")
                        {
                            oldThoiGian = chamCongOt.GioRaSang == null
                                ? "--"
                                : chamCongOt.GioRaSang.Value.ToString(@"hh\:mm");

                            chamCongOt.GioRaSang = parameter.ThoiGian;
                        }
                        else if (parameter.Type == "vc")
                        {
                            oldThoiGian = chamCongOt.GioVaoChieu == null
                                ? "--"
                                : chamCongOt.GioVaoChieu.Value.ToString(@"hh\:mm");

                            chamCongOt.GioVaoChieu = parameter.ThoiGian;
                        }
                        else if (parameter.Type == "rc")
                        {
                            oldThoiGian = chamCongOt.GioRaChieu == null
                                ? "--"
                                : chamCongOt.GioRaChieu.Value.ToString(@"hh\:mm");

                            chamCongOt.GioRaChieu = parameter.ThoiGian;
                        }
                        else if (parameter.Type == "vt")
                        {
                            oldThoiGian = chamCongOt.GioVaoToi == null
                                ? "--"
                                : chamCongOt.GioVaoToi.Value.ToString(@"hh\:mm");

                            chamCongOt.GioVaoToi = parameter.ThoiGian;
                        }
                        else if (parameter.Type == "rt")
                        {
                            oldThoiGian = chamCongOt.GioRaToi == null
                                ? "--"
                                : chamCongOt.GioRaToi.Value.ToString(@"hh\:mm");

                            chamCongOt.GioRaToi = parameter.ThoiGian;
                        }

                        context.ChamCongOt.Update(chamCongOt);
                        context.SaveChanges();
                    }

                    #region Tính lại giờ OT

                    var tongHopChamCongOt = context.TongHopChamCongOt.FirstOrDefault(x =>
                        x.EmployeeId == parameter.EmployeeId &&
                        x.LoaiOtId == parameter.LoaiOtId &&
                        x.NgayChamCong.Date == parameter.NgayChamCong.Date);

                    if (tongHopChamCongOt != null)
                    {
                        context.TongHopChamCongOt.Remove(tongHopChamCongOt);
                    }

                    var newChamCongOt = context.ChamCongOt
                        .Where(x => x.EmployeeId == parameter.EmployeeId &&
                                    x.NgayChamCong.Date == parameter.NgayChamCong.Date &&
                                    x.LoaiOtId == parameter.LoaiOtId).ToList();

                    var listTongHopChamCongOt = TinhTongHopChamCongOt(newChamCongOt);

                    context.TongHopChamCongOt.AddRange(listTongHopChamCongOt);
                    context.SaveChanges();

                    #endregion

                    #region Lưu lịch sử vào dòng thời gian

                    var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                    var emp = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                    var _emp = context.Employee.FirstOrDefault(x => x.EmployeeId == parameter.EmployeeId);

                    var ca = "";
                    if (parameter.Type == "vs") ca = "vào sáng";
                    else if (parameter.Type == "rs") ca = "ra sáng";
                    else if (parameter.Type == "vc") ca = "vào chiều";
                    else if (parameter.Type == "rc") ca = "ra chiều";
                    else if (parameter.Type == "vt") ca = "vào tối";
                    else if (parameter.Type == "rt") ca = "ra tối";

                    var contentNote = "";
                    contentNote = "<p>- <strong>" + emp.EmployeeCode + " - " + emp.EmployeeName +
                                  "</strong> đã thay đổi giờ " + ca +
                                  " của <strong>" + _emp.EmployeeCode + " - " + _emp.EmployeeName +
                                  "</strong> từ <strong>\"" + oldThoiGian + "\"</strong> sang <strong>\"" + newThoiGian +
                                  "\"</strong> với loại OT: <strong> " + loaiOt + 
                                  "</strong> trong ngày <strong>" + ngay + "</strong></p>";

                    Note note = new Note
                    {
                        NoteId = Guid.NewGuid(),
                        Description = contentNote,
                        Type = "ADD",
                        ObjectId = Guid.Empty,
                        ObjectNumber = 1,
                        ObjectType = NoteObjectType.TKCCOT,
                        Active = true,
                        CreatedById = parameter.UserId,
                        CreatedDate = DateTime.Now,
                        NoteTitle = "đã thêm ghi chú"
                    };

                    context.Note.Add(note);
                    context.SaveChanges();

                    #endregion

                    trans.Commit();

                    return new CreateOrUpdateChamCongOtResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = "Lưu thành công"
                    };
                }
            }
            catch (Exception e)
            {
                return new CreateOrUpdateChamCongOtResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetMasterDataTroCapResult GetMasterDataTroCap(GetMasterDataTroCapParameter parameter)
        {
            try
            {
                var listLoaiNgayNghi = GeneralList.GetTrangThais("TroCap_LoaiNgayNghi");
                var listHinhThucTru = GeneralList.GetTrangThais("TroCap_HinhThucTru");

                #region Master data

                var troCapCoDinhType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LTCCD");
                var listLoaiTroCapCoDinh = context.Category
                    .Where(x => x.CategoryTypeId == troCapCoDinhType.CategoryTypeId &&
                                x.CategoryCode != "TCCCNC" && x.CategoryCode != "TCCCDMVS")
                    .Select(y => new CategoryEntityModel
                    {
                        CategoryId = y.CategoryId,
                        CategoryName = y.CategoryName
                    }).ToList();

                var listLoaiTroCapChuyenCanNgayCong = context.Category
                    .Where(x => x.CategoryTypeId == troCapCoDinhType.CategoryTypeId &&
                                x.CategoryCode == "TCCCNC")
                    .Select(y => new CategoryEntityModel
                    {
                        CategoryId = y.CategoryId,
                        CategoryName = y.CategoryName
                    }).ToList();

                var listLoaiTroCapChuyenCanDmvs = context.Category
                    .Where(x => x.CategoryTypeId == troCapCoDinhType.CategoryTypeId &&
                                x.CategoryCode == "TCCCDMVS")
                    .Select(y => new CategoryEntityModel
                    {
                        CategoryId = y.CategoryId,
                        CategoryName = y.CategoryName
                    }).ToList();

                var troCapKhacType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LTCK");
                var listLoaiTroCapKhac = context.Category
                    .Where(x => x.CategoryTypeId == troCapKhacType.CategoryTypeId)
                    .Select(y => new CategoryEntityModel
                    {
                        CategoryId = y.CategoryId,
                        CategoryName = y.CategoryName
                    }).ToList();

                var listPosition = context.Position.Select(y => new PositionModel
                {
                    PositionId = y.PositionId,
                    PositionName = y.PositionName
                }).OrderBy(z => z.PositionName).ToList();

                var loaiHopDongType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LHDNS");
                var listLoaiHopDong = context.Category
                    .Where(x => x.CategoryTypeId == loaiHopDongType.CategoryTypeId)
                    .Select(y => new CategoryEntityModel
                    {
                        CategoryId = y.CategoryId,
                        CategoryName = y.CategoryName
                    }).ToList();

                var dieuKienHuongType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "DKHTC");
                var listDieuKienHuong = context.Category
                    .Where(x => x.CategoryTypeId == dieuKienHuongType.CategoryTypeId)
                    .Select(y => new CategoryEntityModel
                    {
                        CategoryId = y.CategoryId,
                        CategoryName = y.CategoryName
                    }).ToList();

                #endregion

                #region Trợ cấp cố định

                var listTroCapCoDinh = context.TroCap.Where(x => x.TypeId == 1).Select(y => new TroCapModel(y)).ToList();
                var listTroCapCoDinhId = listTroCapCoDinh.Select(y => y.TroCapId).ToList();

                var listChucVuTccd = context.TroCapChucVuMapping
                    .Where(x => listTroCapCoDinhId.Contains(x.TroCapId))
                    .Select(y => new TroCapChucVuMappingModel(y)).ToList();
                var listLoaiHopDongTccd = context.TroCapLoaiHopDongMapping
                    .Where(x => listTroCapCoDinhId.Contains(x.TroCapId))
                    .Select(y => new TroCapLoaiHopDongMappingModel(y)).ToList();
                var listDieuKienHuongTccd = context.TroCapDieuKienHuongMapping
                    .Where(x => listTroCapCoDinhId.Contains(x.TroCapId))
                    .Select(y => new TroCapDieuKienHuongMappingModel(y)).ToList();

                listTroCapCoDinh.ForEach(item =>
                {
                    var loaiTroCap = listLoaiTroCapCoDinh.FirstOrDefault(x => x.CategoryId == item.LoaiTroCapId);
                    item.LoaiTroCap = loaiTroCap?.CategoryName;

                    item.ListChucVu = listChucVuTccd.Where(x => x.TroCapId == item.TroCapId).ToList();
                    var listChucVuId = item.ListChucVu.Select(y => y.PositionId).ToList();
                    var listChucVu = listPosition.Where(x => listChucVuId.Contains(x.PositionId))
                        .OrderBy(z => z.PositionName).Select(y => y.PositionName).ToList();
                    listChucVu.ForEach(cv => { item.ChucVuText += "<p>+ " + cv + "</p>"; });

                    item.ListLoaiHopDong = listLoaiHopDongTccd.Where(x => x.TroCapId == item.TroCapId).ToList();
                    var listLoaiHopDongId = item.ListLoaiHopDong.Select(y => y.LoaiHopDongId).ToList();
                    var _listLoaiHopDong = listLoaiHopDong
                        .Where(x => listLoaiHopDongId.Contains(x.CategoryId.Value))
                        .OrderBy(z => z.CategoryName).Select(y => y.CategoryName).ToList();
                    _listLoaiHopDong.ForEach(lhd => { item.LoaiHopDongText += "<p>+ " + lhd + "</p>"; });

                    item.ListDieuKienHuong = listDieuKienHuongTccd.Where(x => x.TroCapId == item.TroCapId).ToList();
                    var listDieuKienHuongId = item.ListDieuKienHuong.Select(y => y.DieuKienHuongId).ToList();
                    var _listDieuKienHuong = listDieuKienHuong.Where(x => listDieuKienHuongId.Contains(x.CategoryId.Value))
                        .OrderBy(z => z.CategoryName).Select(y => y.CategoryName).ToList();
                    _listDieuKienHuong.ForEach(dkh => { item.DieuKienHuongText += "<p>+ " + dkh + "</p>"; });
                });

                #endregion

                #region Trợ cấp theo chuyên cần ngày công

                var troCapTheoChuyenCanNgayCong = context.TroCap.Where(x => x.TypeId == 2)
                    .Select(y => new TroCapModel(y)).FirstOrDefault();

                if (troCapTheoChuyenCanNgayCong != null)
                {
                    var loaiTroCapChuyenCanNgayCong = listLoaiTroCapCoDinh
                        .FirstOrDefault(x => x.CategoryId == troCapTheoChuyenCanNgayCong.LoaiTroCapId);
                    troCapTheoChuyenCanNgayCong.LoaiTroCap = loaiTroCapChuyenCanNgayCong?.CategoryName;

                    troCapTheoChuyenCanNgayCong.ListChucVu = context.TroCapChucVuMapping
                        .Where(x => x.TroCapId == troCapTheoChuyenCanNgayCong.TroCapId)
                        .Select(y => new TroCapChucVuMappingModel(y)).ToList();

                    troCapTheoChuyenCanNgayCong.ListLoaiHopDong = context.TroCapLoaiHopDongMapping
                        .Where(x => x.TroCapId == troCapTheoChuyenCanNgayCong.TroCapId)
                        .Select(y => new TroCapLoaiHopDongMappingModel(y)).ToList();

                    troCapTheoChuyenCanNgayCong.ListDieuKienHuong = context.TroCapDieuKienHuongMapping
                        .Where(x => x.TroCapId == troCapTheoChuyenCanNgayCong.TroCapId)
                        .Select(y => new TroCapDieuKienHuongMappingModel(y)).ToList();

                    troCapTheoChuyenCanNgayCong.ListMucHuongTheoNgayNghi = context.MucHuongTheoNgayNghi
                        .Where(x => x.TroCapId == troCapTheoChuyenCanNgayCong.TroCapId)
                        .Select(y => new MucHuongTheoNgayNghiModel(y)).OrderByDescending(z => z.MucHuongPhanTram).ToList();
                }

                #endregion

                #region Trợ cấp theo chuyên cần đi muộn về sớm

                var troCapTheoChuyenCanDmvs = context.TroCap.Where(x => x.TypeId == 3)
                    .Select(y => new TroCapModel(y)).FirstOrDefault();

                if (troCapTheoChuyenCanDmvs != null)
                {
                    var loaiTroCapChuyenCanDmvs = listLoaiTroCapCoDinh
                        .FirstOrDefault(x => x.CategoryId == troCapTheoChuyenCanDmvs.LoaiTroCapId);
                    troCapTheoChuyenCanDmvs.LoaiTroCap = loaiTroCapChuyenCanDmvs?.CategoryName;

                    troCapTheoChuyenCanDmvs.ListChucVu = context.TroCapChucVuMapping
                        .Where(x => x.TroCapId == troCapTheoChuyenCanDmvs.TroCapId)
                        .Select(y => new TroCapChucVuMappingModel(y)).ToList();

                    troCapTheoChuyenCanDmvs.ListLoaiHopDong = context.TroCapLoaiHopDongMapping
                        .Where(x => x.TroCapId == troCapTheoChuyenCanDmvs.TroCapId)
                        .Select(y => new TroCapLoaiHopDongMappingModel(y)).ToList();

                    troCapTheoChuyenCanDmvs.ListDieuKienHuong = context.TroCapDieuKienHuongMapping
                        .Where(x => x.TroCapId == troCapTheoChuyenCanDmvs.TroCapId)
                        .Select(y => new TroCapDieuKienHuongMappingModel(y)).ToList();

                    troCapTheoChuyenCanDmvs.ListMucHuongDmvs = context.MucHuongDmvs
                        .Where(x => x.TroCapId == troCapTheoChuyenCanDmvs.TroCapId)
                        .Select(y => new MucHuongDmvsModel(y)).ToList();
                }

                #endregion

                #region Trợ cấp khác

                var listTroCapKhac = context.TroCap.Where(x => x.TypeId == 4).Select(y => new TroCapModel(y)).ToList();
                var listTroCapKhacId = listTroCapKhac.Select(y => y.TroCapId).ToList();

                var listChucVuTck = context.TroCapChucVuMapping
                    .Where(x => listTroCapKhacId.Contains(x.TroCapId))
                    .Select(y => new TroCapChucVuMappingModel(y)).ToList();
                var listLoaiHopDongTck = context.TroCapLoaiHopDongMapping
                    .Where(x => listTroCapKhacId.Contains(x.TroCapId))
                    .Select(y => new TroCapLoaiHopDongMappingModel(y)).ToList();
                var listDieuKienHuongTck = context.TroCapDieuKienHuongMapping
                    .Where(x => listTroCapKhacId.Contains(x.TroCapId))
                    .Select(y => new TroCapDieuKienHuongMappingModel(y)).ToList();

                listTroCapKhac.ForEach(item =>
                {
                    var loaiTroCap = listLoaiTroCapKhac.FirstOrDefault(x => x.CategoryId == item.LoaiTroCapId);
                    item.LoaiTroCap = loaiTroCap?.CategoryName;

                    item.ListChucVu = listChucVuTck.Where(x => x.TroCapId == item.TroCapId).ToList();
                    var listChucVuId = item.ListChucVu.Select(y => y.PositionId).ToList();
                    var listChucVu = listPosition.Where(x => listChucVuId.Contains(x.PositionId))
                        .OrderBy(z => z.PositionName).Select(y => y.PositionName).ToList();
                    listChucVu.ForEach(cv => { item.ChucVuText += "<p>+ " + cv + "</p>"; });

                    item.ListLoaiHopDong = listLoaiHopDongTck.Where(x => x.TroCapId == item.TroCapId).ToList();
                    var listLoaiHopDongId = item.ListLoaiHopDong.Select(y => y.LoaiHopDongId).ToList();
                    var _listLoaiHopDong = listLoaiHopDong
                        .Where(x => listLoaiHopDongId.Contains(x.CategoryId.Value))
                        .OrderBy(z => z.CategoryName).Select(y => y.CategoryName).ToList();
                    _listLoaiHopDong.ForEach(lhd => { item.LoaiHopDongText += "<p>+ " + lhd + "</p>"; });

                    item.ListDieuKienHuong = listDieuKienHuongTck.Where(x => x.TroCapId == item.TroCapId).ToList();
                    var listDieuKienHuongId = item.ListDieuKienHuong.Select(y => y.DieuKienHuongId).ToList();
                    var _listDieuKienHuong = listDieuKienHuong.Where(x => listDieuKienHuongId.Contains(x.CategoryId.Value))
                        .OrderBy(z => z.CategoryName).Select(y => y.CategoryName).ToList();
                    _listDieuKienHuong.ForEach(dkh => { item.DieuKienHuongText += "<p>+ " + dkh + "</p>"; });
                });

                #endregion

                return new GetMasterDataTroCapResult()
                {
                    ListTroCapCoDinh = listTroCapCoDinh,
                    TroCapTheoChuyenCanNgayCong = troCapTheoChuyenCanNgayCong ?? new TroCapModel(),
                    TroCapTheoChuyenCanDmvs = troCapTheoChuyenCanDmvs ?? new TroCapModel(),
                    ListTroCapKhac = listTroCapKhac,
                    ListLoaiTroCapCoDinh = listLoaiTroCapCoDinh,
                    ListLoaiTroCapChuyenCanNgayCong = listLoaiTroCapChuyenCanNgayCong,
                    ListLoaiTroCapChuyenCanDmvs = listLoaiTroCapChuyenCanDmvs,
                    ListLoaiTroCapKhac = listLoaiTroCapKhac,
                    ListPosition = listPosition,
                    ListLoaiHopDong = listLoaiHopDong,
                    ListDieuKienHuong = listDieuKienHuong,
                    ListLoaiNgayNghi = listLoaiNgayNghi,
                    ListHinhThucTru = listHinhThucTru,
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "OK",
                };
            }
            catch (Exception e)
            {
                return new GetMasterDataTroCapResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreateOrUpdateTroCapResult CreateOrUpdateTroCap(CreateOrUpdateTroCapParameter parameter)
        {
            try
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    int troCapId;

                    var troCapParameter = parameter.TroCap;

                    //Create
                    if (troCapParameter.TroCapId == null)
                    {
                        if (troCapParameter.TypeId == 1 || troCapParameter.TypeId == 4)
                        {
                            var exists =
                                context.TroCap.FirstOrDefault(x => x.LoaiTroCapId == troCapParameter.LoaiTroCapId &&
                                                                   x.TypeId == troCapParameter.TypeId);

                            if (exists != null)
                            {
                                return new CreateOrUpdateTroCapResult()
                                {
                                    StatusCode = HttpStatusCode.Conflict,
                                    MessageCode = "Loại Trợ cấp này đã tồn tại trên hệ thống, bạn không thể thêm"
                                };
                            }
                        }

                        var troCap = troCapParameter.ToEntity();
                        context.TroCap.Add(troCap);

                        context.SaveChanges();

                        troCapId = troCap.TroCapId;
                    }
                    //Update
                    else
                    {
                        var troCap = context.TroCap.FirstOrDefault(x => x.TroCapId == troCapParameter.TroCapId);
                        if (troCap == null)
                        {
                            return new CreateOrUpdateTroCapResult()
                            {
                                StatusCode = HttpStatusCode.NotFound,
                                MessageCode = "Trợ cấp không tồn tại trên hệ thống"
                            };
                        }

                        if (troCapParameter.TypeId == 1 || troCapParameter.TypeId == 4)
                        {
                            var exists =
                                context.TroCap.FirstOrDefault(x => x.LoaiTroCapId == troCapParameter.LoaiTroCapId &&
                                                                   x.TypeId == troCapParameter.TypeId &&
                                                                   x.TroCapId != troCap.TroCapId);

                            if (exists != null)
                            {
                                return new CreateOrUpdateTroCapResult()
                                {
                                    StatusCode = HttpStatusCode.Conflict,
                                    MessageCode = "Loại Trợ cấp này đã tồn tại trên hệ thống, bạn không thể thêm"
                                };
                            }
                        }

                        troCap.LoaiTroCapId = troCapParameter.LoaiTroCapId;
                        troCap.MucTroCap = troCapParameter.MucTroCap;

                        context.TroCap.Update(troCap);
                        context.SaveChanges();

                        troCapId = troCap.TroCapId;

                        #region Xóa các list mapping

                        var listChucVu = context.TroCapChucVuMapping.Where(x => x.TroCapId == troCapId).ToList();
                        var listLoaiHopDong = context.TroCapLoaiHopDongMapping.Where(x => x.TroCapId == troCapId).ToList();
                        var listDieuKienHuong = context.TroCapDieuKienHuongMapping.Where(x => x.TroCapId == troCapId).ToList();
                        var listMucHuongTheoNgayNghi = context.MucHuongTheoNgayNghi.Where(x => x.TroCapId == troCapId).ToList();
                        var listMucHuongDmvs = context.MucHuongDmvs.Where(x => x.TroCapId == troCapId).ToList();

                        context.TroCapChucVuMapping.RemoveRange(listChucVu);
                        context.TroCapLoaiHopDongMapping.RemoveRange(listLoaiHopDong);
                        context.TroCapDieuKienHuongMapping.RemoveRange(listDieuKienHuong);
                        context.MucHuongTheoNgayNghi.RemoveRange(listMucHuongTheoNgayNghi);
                        context.MucHuongDmvs.RemoveRange(listMucHuongDmvs);
                        context.SaveChanges();

                        #endregion
                    }

                    #region Thêm các list mapping

                    //List chức vụ
                    troCapParameter.ListChucVu.ForEach(item =>
                    {
                        item.TroCapId = troCapId;
                        context.TroCapChucVuMapping.Add(item.ToEntity());
                        context.SaveChanges();
                    });

                    //List loại hợp đồng
                    troCapParameter.ListLoaiHopDong.ForEach(item =>
                    {
                        item.TroCapId = troCapId;
                        context.TroCapLoaiHopDongMapping.Add(item.ToEntity());
                        context.SaveChanges();
                    });

                    //List điều kiện hưởng
                    troCapParameter.ListDieuKienHuong.ForEach(item =>
                    {
                        item.TroCapId = troCapId;
                        context.TroCapDieuKienHuongMapping.Add(item.ToEntity());
                        context.SaveChanges();
                    });

                    //Trợ cấp theo chuyên cần ngày công 
                    if (troCapParameter.TypeId == 2)
                    {
                        troCapParameter.ListMucHuongTheoNgayNghi.ForEach(item =>
                        {
                            item.TroCapId = troCapId;
                            context.MucHuongTheoNgayNghi.Add(item.ToEntity());
                            context.SaveChanges();
                        });
                    }
                    //Trợ cấp theo chuyên cần đi muộn về sớm
                    else if (troCapParameter.TypeId == 3)
                    {
                        troCapParameter.ListMucHuongDmvs.ForEach(item =>
                        {
                            item.TroCapId = troCapId;
                            context.MucHuongDmvs.Add(item.ToEntity());
                            context.SaveChanges();
                        });
                    }

                    #endregion

                    trans.Commit();

                    return new CreateOrUpdateTroCapResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = "Lưu thành công"
                    };
                }
            }
            catch (Exception e)
            {
                return new CreateOrUpdateTroCapResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public DeleteTroCapResult DeleteTroCap(DeleteTroCapParameter parameter)
        {
            try
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    //Trợ cấp cố định, Trợ cấp khác
                    if (parameter.TypeId == 1 || parameter.TypeId == 4)
                    {
                        var troCap = context.TroCap.FirstOrDefault(x => x.TroCapId == parameter.ObjectId);
                        if (troCap == null)
                        {
                            return new DeleteTroCapResult()
                            {
                                StatusCode = HttpStatusCode.NotFound,
                                MessageCode = "Trợ cấp không tồn tại trên hệ thống"
                            };
                        }

                        var listChucVu = context.TroCapChucVuMapping.Where(x => x.TroCapId == troCap.TroCapId).ToList();
                        var listLoaiHopDong = context.TroCapLoaiHopDongMapping.Where(x => x.TroCapId == troCap.TroCapId).ToList();
                        var listDieuKienHuong = context.TroCapDieuKienHuongMapping.Where(x => x.TroCapId == troCap.TroCapId).ToList();

                        context.TroCapChucVuMapping.RemoveRange(listChucVu);
                        context.TroCapLoaiHopDongMapping.RemoveRange(listLoaiHopDong);
                        context.TroCapDieuKienHuongMapping.RemoveRange(listDieuKienHuong);
                        context.TroCap.Remove(troCap);

                        context.SaveChanges();
                    }
                    //Trợ cấp chuyên cần ngày công
                    else if (parameter.TypeId == 2)
                    {
                        var mucHuongTheoNgayNghi = context.MucHuongTheoNgayNghi
                            .FirstOrDefault(x => x.MucHuongTheoNgayNghiId == parameter.ObjectId);
                        if (mucHuongTheoNgayNghi == null)
                        {
                            return new DeleteTroCapResult()
                            {
                                StatusCode = HttpStatusCode.NotFound,
                                MessageCode = "Trợ cấp không tồn tại trên hệ thống"
                            };
                        }

                        context.MucHuongTheoNgayNghi.Remove(mucHuongTheoNgayNghi);

                        context.SaveChanges();
                    }
                    //Trợ cấp chuyên cần dmvs
                    else if (parameter.TypeId == 3)
                    {
                        var mucHuongDmvs = context.MucHuongDmvs
                            .FirstOrDefault(x => x.MucHuongDmvsId == parameter.ObjectId);
                        if (mucHuongDmvs == null)
                        {
                            return new DeleteTroCapResult()
                            {
                                StatusCode = HttpStatusCode.NotFound,
                                MessageCode = "Trợ cấp không tồn tại trên hệ thống"
                            };
                        }

                        context.MucHuongDmvs.Remove(mucHuongDmvs);

                        context.SaveChanges();
                    }

                    trans.Commit();

                    return new DeleteTroCapResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = "Xóa thành công"
                    };
                }
            }
            catch (Exception e)
            {
                return new DeleteTroCapResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetListKyLuongResult GetListKyLuong(GetListKyLuongParameter parameter)
        {
            try
            {
                var listAllUser = context.User.ToList();
                var user = listAllUser.FirstOrDefault(c => c.UserId == parameter.UserId);
                if (user == null)
                {
                    return new GetListKyLuongResult
                    {
                        Message = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }
                var employee = context.Employee.FirstOrDefault(c => c.EmployeeId == user.EmployeeId);
                if (employee == null)
                {
                    return new GetListKyLuongResult
                    {
                        Message = "Nhân viên không tồn tại trong hệ thống",
                        StatusCode = HttpStatusCode.ExpectationFailed
                    };
                }

                var listStatus = GeneralList.GetTrangThais("TrangThaiKyLuong");

                var listData = context.KyLuong.Where(x =>
                        (String.IsNullOrWhiteSpace(parameter.TenKyLuong) || x.TenKyLuong.ToLower().Trim()
                             .Contains(parameter.TenKyLuong.ToLower().Trim())) &&
                        parameter.ListTrangThai.Count == 0 || parameter.ListTrangThai.Contains(x.TrangThai))
                    .Select(y => new KyLuongModel(y)).ToList();

                #region Phân quyền dữ liệu theo quy trình phê duyệt

                var thanhVienPhongBan =
                    context.ThanhVienPhongBan.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId);

                var thongTinDeXuatTL = new List<DeXuatChucVuEntityModel>();
                var isAccess = context.Organization.FirstOrDefault(x => x.OrganizationId == employee.OrganizationId)?.IsAccess;
                if(isAccess == false)
                {
                    //Nếu là trưởng bộ phận (IsManager = 1)
                    if (thanhVienPhongBan.IsManager == 1)
                    {
                        //Lấy ra list đối tượng id mà người dùng phụ trách phê duyệt
                        var listId = context.PhongBanPheDuyetDoiTuong
                            .Where(x => x.DoiTuongApDung == 14 &&
                                        x.OrganizationId == thanhVienPhongBan.OrganizationId).Select(y => y.ObjectNumber)
                            .ToList();
                        var listEmpIdCungPhongBan = context.Employee.Where(x => x.OrganizationId == employee.OrganizationId).Select(x => x.EmployeeId).ToList();
                        var listUserIdCungPhongBan = listAllUser.Where(x => listEmpIdCungPhongBan.Contains(x.EmployeeId.Value)).Select(x => x.UserId).ToList();

                        listData = listData.Where(x =>
                                                x.CreatedById == user.UserId || //Người tạo
                                                 listId.Contains(x.KyLuongId) || // cần phê duyệt
                                                 (listUserIdCungPhongBan.Contains(x.CreatedById.Value) && x.TrangThai != 1)) // cùng phòng ban tt khác 1
                                             .ToList();
                    }
                    //Nếu là nhân viên thường (IsManager = 0)
                    else
                    {
                        listData = listData.Where(x => x.CreatedById == user.UserId).ToList(); // Theo người tạo
                    }
                }
                else
                {
                    listData = listData.Where(x =>
                                        x.CreatedById == user.UserId || // Theo người tạo
                                        (x.CreatedById != user.UserId && x.TrangThai != 1) // Người khác tạo và trajgn thái khác mới: 1
                    ).ToList();
                }
                #endregion

                return new GetListKyLuongResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "OK",
                    ListData = listData,
                    ListStatus = listStatus
                };
            }
            catch (Exception e)
            {
                return new GetListKyLuongResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreateOrUpdateKyLuongResult CreateOrUpdateKyLuong(CreateOrUpdateKyLuongParameter parameter)
        {
            try
            {
                #region Kiểm tra điều kiện trước khi tạo kỳ lương

                if (parameter.KyLuong.TuNgay.Date > parameter.KyLuong.DenNgay.Date)
                {
                    return new CreateOrUpdateKyLuongResult()
                    {
                        StatusCode = HttpStatusCode.Conflict,
                        MessageCode = "Ngày bắt đầu phải nhỏ hơn Ngày kết thúc"
                    };
                }

                //Lấy dữ liệu chấm công trong khoảng Từ ngày => Đến ngày
                var countError = context.ChamCong.Count(x => x.NgayChamCong.Date >= parameter.KyLuong.TuNgay.Date &&
                                                             x.NgayChamCong.Date <= parameter.KyLuong.DenNgay.Date &&
                                                             x.KyHieuVaoSang == null && x.KyHieuRaSang == null &&
                                                             x.KyHieuVaoChieu == null && x.KyHieuRaChieu == null &&
                                                             ((x.VaoSang == null && x.RaSang != null) ||
                                                              (x.VaoSang != null && x.RaSang == null) ||
                                                              (x.VaoChieu == null && x.RaChieu != null) ||
                                                              (x.VaoChieu != null && x.RaChieu == null)));

                //var listTest = context.ChamCong.Join(context.User,
                //        cc => cc.EmployeeId,
                //        user => user.EmployeeId,
                //        (cc, user) => new {Cc = cc, User = user})
                //    .Where(x => x.User.Disabled != true &&
                //                x.Cc.NgayChamCong.Date >= parameter.KyLuong.TuNgay.Date &&
                //                x.Cc.NgayChamCong.Date <= parameter.KyLuong.DenNgay.Date &&
                //                x.Cc.KyHieuVaoSang == null && x.Cc.KyHieuRaSang == null &&
                //                x.Cc.KyHieuVaoChieu == null && x.Cc.KyHieuRaChieu == null &&
                //                ((x.Cc.VaoSang == null && x.Cc.RaSang != null) ||
                //                 (x.Cc.VaoSang != null && x.Cc.RaSang == null) ||
                //                 (x.Cc.VaoChieu == null && x.Cc.RaChieu != null) ||
                //                 (x.Cc.VaoChieu != null && x.Cc.RaChieu == null))).ToList();

                if (countError > 0)
                {
                    return new CreateOrUpdateKyLuongResult()
                    {
                        StatusCode = HttpStatusCode.Conflict,
                        MessageCode = "Dữ liệu chấm công chưa đúng, bạn vui lòng kiểm tra lại"
                    };
                }

                #endregion

                int kyLuongId = 0;

                using (var trans = context.Database.BeginTransaction())
                {
                    //Create
                    if (parameter.KyLuong.KyLuongId == null)
                    {
                        var exists = context.KyLuong.OrderByDescending(z => z.TuNgay).FirstOrDefault();

                        if (exists != null && parameter.KyLuong.TuNgay.Date != exists.DenNgay.AddDays(1).Date)
                        {
                            return new CreateOrUpdateKyLuongResult()
                            {
                                StatusCode = HttpStatusCode.Conflict,
                                MessageCode = "Ngày bắt đầu kỳ lương phải liền kề với ngày kết thúc kỳ lương trước"
                            };
                        }

                        parameter.KyLuong.TuNgay = parameter.KyLuong.TuNgay.Date;
                        parameter.KyLuong.DenNgay = parameter.KyLuong.DenNgay.Date;
                        parameter.KyLuong.CreatedById = parameter.UserId;
                        parameter.KyLuong.TrangThai = 1;

                        var kyLuong = parameter.KyLuong.ToEntity();

                        context.KyLuong.Add(kyLuong);
                        context.SaveChanges();

                        kyLuongId = kyLuong.KyLuongId;
                    }
                    //Update
                    else
                    {
                        var kyLuong = context.KyLuong.FirstOrDefault(x => x.KyLuongId == parameter.KyLuong.KyLuongId);
                        if (kyLuong == null)
                        {
                            return new CreateOrUpdateKyLuongResult()
                            {
                                StatusCode = HttpStatusCode.NotFound,
                                MessageCode = "Kỳ lương không tồn tại trên hệ thống"
                            };
                        }

                        var exists = context.KyLuong.Where(x => x.KyLuongId != kyLuong.KyLuongId)
                            .OrderByDescending(z => z.TuNgay).FirstOrDefault();

                        if (exists != null && parameter.KyLuong.TuNgay.Date != exists.DenNgay.AddDays(1).Date)
                        {
                            return new CreateOrUpdateKyLuongResult()
                            {
                                StatusCode = HttpStatusCode.Conflict,
                                MessageCode = "Ngày bắt đầu kỳ lương phải liền kề với ngày kết thúc kỳ lương trước"
                            };
                        }

                        if (kyLuong.TrangThai != 1)
                        {
                            return new CreateOrUpdateKyLuongResult()
                            {
                                StatusCode = HttpStatusCode.Conflict,
                                MessageCode = "Không thể cập nhật do Kỳ lương không phải ở trạng thái Mới"
                            };
                        }

                        kyLuong.TenKyLuong = parameter.KyLuong.TenKyLuong.Trim();
                        kyLuong.SoNgayLamViec = parameter.KyLuong.SoNgayLamViec;
                        kyLuong.TuNgay = parameter.KyLuong.TuNgay.Date;
                        kyLuong.DenNgay = parameter.KyLuong.DenNgay.Date;

                        context.KyLuong.Update(kyLuong);
                        context.SaveChanges();

                        kyLuongId = kyLuong.KyLuongId;
                    }

                    SaveTongHopChamCong(kyLuongId, parameter.KyLuong.TuNgay.Date, parameter.KyLuong.DenNgay.Date);

                    SaveLuongCt_TroCap(kyLuongId, parameter.KyLuong.SoNgayLamViec, parameter.KyLuong.TuNgay.Date,
                        parameter.KyLuong.DenNgay.Date);

                    SaveLuongCt_TroCapOt(kyLuongId, parameter.KyLuong.SoNgayLamViec, parameter.KyLuong.TuNgay.Date,
                        parameter.KyLuong.DenNgay.Date);

                    SaveLuongCt_ThuNhapTinhThue(kyLuongId, parameter.KyLuong.TuNgay.Date, parameter.KyLuong.DenNgay.Date);

                    SaveLuongCt_BaoHiem(kyLuongId, parameter.KyLuong.TuNgay.Date, parameter.KyLuong.DenNgay.Date);

                    SaveLuongCt_GiamTruTruocThue(kyLuongId, parameter.KyLuong.TuNgay.Date,
                        parameter.KyLuong.DenNgay.Date);

                    SaveLuongCt_GiamTruSauThue(kyLuongId, parameter.KyLuong.TuNgay.Date,
                        parameter.KyLuong.DenNgay.Date);

                    SaveLuongCt_HoanLaiSauThue(kyLuongId, parameter.KyLuong.TuNgay.Date,
                        parameter.KyLuong.DenNgay.Date);

                    SaveLuongCt_CtyDong(kyLuongId, parameter.KyLuong.TuNgay.Date,
                        parameter.KyLuong.DenNgay.Date);

                    SaveLuongCt_Other(kyLuongId, parameter.KyLuong.TuNgay.Date,
                        parameter.KyLuong.DenNgay.Date);

                    SaveLuongTongHop(kyLuongId, parameter.KyLuong.SoNgayLamViec, parameter.KyLuong.TuNgay.Date,
                        parameter.KyLuong.DenNgay.Date);

                    trans.Commit();
                }

                return new CreateOrUpdateKyLuongResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Lưu thành công",
                    KyLuongId = kyLuongId
                };
            }
            catch (Exception e)
            {
                return new CreateOrUpdateKyLuongResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public DeleteKyLuongResult DeleteKyLuong(DeleteKyLuongParameter parameter)
        {
            try
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    var kyLuong = context.KyLuong.FirstOrDefault(x => x.KyLuongId == parameter.KyLuongId);
                    if (kyLuong == null)
                    {
                        return new DeleteKyLuongResult()
                        {
                            StatusCode = HttpStatusCode.NotFound,
                            MessageCode = "Kỳ lương không tồn tại trên hệ thống"
                        };
                    }

                    if (kyLuong.TrangThai != 1)
                    {
                        return new DeleteKyLuongResult()
                        {
                            StatusCode = HttpStatusCode.Conflict,
                            MessageCode = "Không thể xóa do Kỳ lương không phải ở trạng thái Mới"
                        };
                    }

                    var listTongHopChamCong =
                        context.TongHopChamCong.Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                    var listLuongCtTroCapCoDinh =
                        context.LuongCtTroCapCoDinh.Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                    var listLuongCtLoaiTroCapCoDinh = context.LuongCtLoaiTroCapCoDinh
                        .Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                    var listLuongCtDieuKienTroCapCoDinh = context.LuongCtDieuKienTroCapCoDinh
                        .Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                    var listLuongCtTroCapKhac =
                        context.LuongCtTroCapKhac.Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                    var listLuongCtLoaiTroCapKhac = context.LuongCtLoaiTroCapKhac
                        .Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                    var listLuongCtDieuKienTroCapKhac = context.LuongCtDieuKienTroCapKhac
                        .Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                    var listLuongCtEmpDktcKhac =
                        context.LuongCtEmpDktcKhac.Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                    var listLuongCtTroCapOt =
                        context.LuongCtTroCapOt.Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                    var listLuongCtLoaiTroCapOt =
                        context.LuongCtLoaiTroCapOt.Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                    var listLuongCtThuNhapTinhThue = context.LuongCtThuNhapTinhThue
                        .Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                    var listLuongCtBaoHiem =
                        context.LuongCtBaoHiem.Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                    var listLuongCtGiamTruTruocThue = context.LuongCtGiamTruTruocThue
                        .Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                    var listLuongCtGiamTruSauThue = context.LuongCtGiamTruSauThue
                        .Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                    var listLuongCtHoanLaiSauThue = context.LuongCtHoanLaiSauThue
                        .Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                    var listLuongCtCtyDong =
                        context.LuongCtCtyDong.Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                    var listLuongCtOther = context.LuongCtOther.Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                    var listLuongTongHop = context.LuongTongHop.Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();

                    context.KyLuong.Remove(kyLuong);
                    context.TongHopChamCong.RemoveRange(listTongHopChamCong);
                    context.LuongCtTroCapCoDinh.RemoveRange(listLuongCtTroCapCoDinh);
                    context.LuongCtLoaiTroCapCoDinh.RemoveRange(listLuongCtLoaiTroCapCoDinh);
                    context.LuongCtDieuKienTroCapCoDinh.RemoveRange(listLuongCtDieuKienTroCapCoDinh);
                    context.LuongCtTroCapKhac.RemoveRange(listLuongCtTroCapKhac);
                    context.LuongCtLoaiTroCapKhac.RemoveRange(listLuongCtLoaiTroCapKhac);
                    context.LuongCtDieuKienTroCapKhac.RemoveRange(listLuongCtDieuKienTroCapKhac);
                    context.LuongCtEmpDktcKhac.RemoveRange(listLuongCtEmpDktcKhac);
                    context.LuongCtTroCapOt.RemoveRange(listLuongCtTroCapOt);
                    context.LuongCtLoaiTroCapOt.RemoveRange(listLuongCtLoaiTroCapOt);
                    context.LuongCtThuNhapTinhThue.RemoveRange(listLuongCtThuNhapTinhThue);
                    context.LuongCtBaoHiem.RemoveRange(listLuongCtBaoHiem);
                    context.LuongCtGiamTruTruocThue.RemoveRange(listLuongCtGiamTruTruocThue);
                    context.LuongCtGiamTruSauThue.RemoveRange(listLuongCtGiamTruSauThue);
                    context.LuongCtHoanLaiSauThue.RemoveRange(listLuongCtHoanLaiSauThue);
                    context.LuongCtCtyDong.RemoveRange(listLuongCtCtyDong);
                    context.LuongCtOther.RemoveRange(listLuongCtOther);
                    context.LuongTongHop.RemoveRange(listLuongTongHop);
                    context.SaveChanges();

                    trans.Commit();

                    return new DeleteKyLuongResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = "Xóa thành công"
                    };
                }
            }
            catch (Exception e)
            {
                return new DeleteKyLuongResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public DatVeMoiKyLuongResult DatVeMoiKyLuong(DatVeMoiKyLuongParameter parameter)
        {
            try
            {
                var kyLuong = context.KyLuong.FirstOrDefault(x => x.KyLuongId == parameter.KyLuongId);
                if (kyLuong == null)
                {
                    return new DatVeMoiKyLuongResult()
                    {
                        MessageCode = "Kỳ lương không tồn tại trên hệ thống",
                        StatusCode = HttpStatusCode.NotFound
                    };
                }

                if (kyLuong.TrangThai != 4)
                {
                    return new DatVeMoiKyLuongResult()
                    {
                        MessageCode = "Chỉ được đặt về mới đề xuất ở trạng thái Từ chối",
                        StatusCode = HttpStatusCode.Conflict
                    };
                }

                kyLuong.TrangThai = 1;
                context.KyLuong.Update(kyLuong);

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
                note.ObjectNumber = kyLuong.KyLuongId;
                note.ObjectType = NoteObjectType.KYLUONG;

                context.Note.Add(note);

                context.SaveChanges();

                return new DatVeMoiKyLuongResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Đặt về mới thành công"
                };
            }
            catch (Exception e)
            {
                return new DatVeMoiKyLuongResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetKyLuongByIdResult GetKyLuongById(GetKyLuongByIdParameter parameter)
        {
            try
            {
                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                if (user == null)
                {
                    return new GetKyLuongByIdResult()
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        MessageCode = "Nguời dùng không tồn tại trên hệ thống"
                    };
                }

                var emp = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                if (emp == null)
                {
                    return new GetKyLuongByIdResult()
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        MessageCode = "Nguời dùng không tồn tại trên hệ thống"
                    };
                }

                var kyLuong = context.KyLuong.Where(x => x.KyLuongId == parameter.KyLuongId)
                    .Select(y => new KyLuongModel(y)).FirstOrDefault();
                if (kyLuong == null)
                {
                    return new GetKyLuongByIdResult()
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        MessageCode = "Kỳ lương không tồn tại trên hệ thống"
                    };
                }

                var userCreated = context.User.FirstOrDefault(x => x.UserId == kyLuong.CreatedById);
                var empCreated = new Employee();
                if (userCreated != null)
                {
                    empCreated = context.Employee.FirstOrDefault(x => x.EmployeeId == userCreated.EmployeeId);
                    kyLuong.CreatedName = empCreated?.EmployeeCode + " - " + empCreated?.EmployeeName;
                }

                var listEmp =
                    context.Employee.Select(y => new Employee
                    {
                        EmployeeId = y.EmployeeId,
                        EmployeeCode = y.EmployeeCode,
                        EmployeeName = y.EmployeeName,
                        SubCode1Value = y.SubCode1Value
                    }).ToList();

                var listPosition = context.Position.Select(y => new Position
                {
                    PositionId = y.PositionId,
                    PositionCode = y.PositionCode,
                    PositionName = y.PositionName
                }).ToList();

                var listGetSubCode1 = GeneralList.GetSubCode1();

                #region Tổng hợp chấm công

                var listTongHopChamCong = context.TongHopChamCong.Where(x => x.KyLuongId == kyLuong.KyLuongId)
                    .Select(y => new TongHopChamCongModel(y)).ToList();

                listTongHopChamCong.ForEach(item =>
                {
                    var _emp = listEmp.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                    var subCode1 = listGetSubCode1.FirstOrDefault(x => x.Value == _emp.SubCode1Value);

                    item.EmployeeCode = _emp?.EmployeeCode;
                    item.EmployeeName = _emp?.EmployeeName;
                    item.SubCode1 = subCode1?.Name;
                });

                listTongHopChamCong = listTongHopChamCong.OrderBy(z => z.EmployeeCode).ToList();

                #endregion

                #region Bảng lương tổng hợp

                var listLuongTongHop = context.LuongTongHop.Where(x => x.KyLuongId == kyLuong.KyLuongId)
                    .Select(y => new LuongTongHopModel(y)).ToList();

                listLuongTongHop.ForEach(item =>
                {
                    var _emp = listEmp.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                    var subCode1 = listGetSubCode1.FirstOrDefault(x => x.Value == _emp.SubCode1Value);
                    var _pos = listPosition.FirstOrDefault(x => x.PositionId == item.PositionId);

                    item.EmployeeCode = _emp?.EmployeeCode;
                    item.EmployeeName = _emp?.EmployeeName;
                    item.SubCode1 = subCode1?.Name;
                    item.PositionName = _pos?.PositionName;
                });

                #endregion

                #region Thu nhập chỉ đưa vào tính thuế

                var listLuongCtThuNhapTinhThue = context.LuongCtThuNhapTinhThue
                    .Where(x => x.KyLuongId == kyLuong.KyLuongId)
                    .Select(y => new LuongCtThuNhapTinhThueModel(y)).ToList();

                listLuongCtThuNhapTinhThue.ForEach(item =>
                {
                    var _emp = listEmp.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                    var subCode1 = listGetSubCode1.FirstOrDefault(x => x.Value == _emp.SubCode1Value);
                    var _pos = listPosition.FirstOrDefault(x => x.PositionId == item.PositionId);

                    item.EmployeeCode = _emp?.EmployeeCode;
                    item.EmployeeName = _emp?.EmployeeName;
                    item.SubCode1 = subCode1?.Name;
                    item.PositionName = _pos?.PositionName;
                });

                #endregion

                #region Giảm trừ trước thuế

                var listLuongCtGiamTruTruocThue = context.LuongCtGiamTruTruocThue
                    .Where(x => x.KyLuongId == kyLuong.KyLuongId)
                    .Select(y => new LuongCtGiamTruTruocThueModel(y)).ToList();

                listLuongCtGiamTruTruocThue.ForEach(item =>
                {
                    var _emp = listEmp.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                    var subCode1 = listGetSubCode1.FirstOrDefault(x => x.Value == _emp.SubCode1Value);
                    var _pos = listPosition.FirstOrDefault(x => x.PositionId == item.PositionId);

                    item.EmployeeCode = _emp?.EmployeeCode;
                    item.EmployeeName = _emp?.EmployeeName;
                    item.SubCode1 = subCode1?.Name;
                    item.PositionName = _pos?.PositionName;
                });

                #endregion

                #region Giảm trừ sau thuế

                var listLuongCtGiamTruSauThue = context.LuongCtGiamTruSauThue
                    .Where(x => x.KyLuongId == kyLuong.KyLuongId)
                    .Select(y => new LuongCtGiamTruSauThueModel(y)).ToList();

                listLuongCtGiamTruSauThue.ForEach(item =>
                {
                    var _emp = listEmp.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                    var subCode1 = listGetSubCode1.FirstOrDefault(x => x.Value == _emp.SubCode1Value);
                    var _pos = listPosition.FirstOrDefault(x => x.PositionId == item.PositionId);

                    item.EmployeeCode = _emp?.EmployeeCode;
                    item.EmployeeName = _emp?.EmployeeName;
                    item.SubCode1 = subCode1?.Name;
                    item.PositionName = _pos?.PositionName;
                });

                #endregion

                #region Hoàn lại sau thuế

                var listLuongCtHoanLaiSauThue = context.LuongCtHoanLaiSauThue
                    .Where(x => x.KyLuongId == kyLuong.KyLuongId)
                    .Select(y => new LuongCtHoanLaiSauThueModel(y)).ToList();

                listLuongCtHoanLaiSauThue.ForEach(item =>
                {
                    var _emp = listEmp.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                    var subCode1 = listGetSubCode1.FirstOrDefault(x => x.Value == _emp.SubCode1Value);
                    var _pos = listPosition.FirstOrDefault(x => x.PositionId == item.PositionId);

                    item.EmployeeCode = _emp?.EmployeeCode;
                    item.EmployeeName = _emp?.EmployeeName;
                    item.SubCode1 = subCode1?.Name;
                    item.PositionName = _pos?.PositionName;
                });

                #endregion

                #region Công ty phải đóng

                var listLuongCtCtyDong = context.LuongCtCtyDong
                    .Where(x => x.KyLuongId == kyLuong.KyLuongId)
                    .Select(y => new LuongCtCtyDongModel(y)).ToList();

                listLuongCtCtyDong.ForEach(item =>
                {
                    var _emp = listEmp.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                    var subCode1 = listGetSubCode1.FirstOrDefault(x => x.Value == _emp.SubCode1Value);
                    var _pos = listPosition.FirstOrDefault(x => x.PositionId == item.PositionId);

                    item.EmployeeCode = _emp?.EmployeeCode;
                    item.EmployeeName = _emp?.EmployeeName;
                    item.SubCode1 = subCode1?.Name;
                    item.PositionName = _pos?.PositionName;
                });

                #endregion

                #region Other

                var listLuongCtOther = context.LuongCtOther
                    .Where(x => x.KyLuongId == kyLuong.KyLuongId)
                    .Select(y => new LuongCtOtherModel(y)).ToList();

                listLuongCtOther.ForEach(item =>
                {
                    var _emp = listEmp.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                    var subCode1 = listGetSubCode1.FirstOrDefault(x => x.Value == _emp.SubCode1Value);
                    var _pos = listPosition.FirstOrDefault(x => x.PositionId == item.PositionId);

                    item.EmployeeCode = _emp?.EmployeeCode;
                    item.EmployeeName = _emp?.EmployeeName;
                    item.SubCode1 = subCode1?.Name;
                    item.PositionName = _pos?.PositionName;
                });

                #endregion

                #region Bảo hiểm

                var listLuongCtBaoHiem = context.LuongCtBaoHiem
                    .Where(x => x.KyLuongId == kyLuong.KyLuongId)
                    .Select(y => new LuongCtBaoHiemModel(y)).ToList();

                listLuongCtBaoHiem.ForEach(item =>
                {
                    var _emp = listEmp.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                    var subCode1 = listGetSubCode1.FirstOrDefault(x => x.Value == _emp.SubCode1Value);
                    var _pos = listPosition.FirstOrDefault(x => x.PositionId == item.PositionId);

                    item.EmployeeCode = _emp?.EmployeeCode;
                    item.EmployeeName = _emp?.EmployeeName;
                    item.SubCode1 = subCode1?.Name;
                    item.PositionName = _pos?.PositionName;
                });

                #endregion

                #region Trợ cấp OT

                var listLuongCtTroCapOt = context.LuongCtTroCapOt
                    .Where(x => x.KyLuongId == kyLuong.KyLuongId)
                    .Select(y => new LuongCtTroCapOtModel(y)).ToList();

                var listLuongCtLoaiTroCapOt =
                    context.LuongCtLoaiTroCapOt.Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();

                var typeLoaiOt = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LOAIOT");
                var listLoaiOt = context.Category.Where(x => x.CategoryTypeId == typeLoaiOt.CategoryTypeId).ToList();

                #region Data

                var listDataTroCapOt = new List<List<DataRowModel>>();

                int stt = 0;
                listLuongCtTroCapOt.ForEach(item =>
                {
                    var _emp = listEmp.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                    var subCode1 = listGetSubCode1.FirstOrDefault(x => x.Value == _emp.SubCode1Value);
                    var _pos = listPosition.FirstOrDefault(x => x.PositionId == item.PositionId);

                    item.EmployeeCode = _emp?.EmployeeCode;
                    item.EmployeeName = _emp?.EmployeeName;
                    item.SubCode1 = subCode1?.Name;
                    item.PositionName = _pos?.PositionName;

                    var listLuongCtLoaiTroCapOt_ByEmp = listLuongCtLoaiTroCapOt
                        .Where(x => x.LuongCtTroCapOtId == item.LuongCtTroCapOtId).ToList();

                    stt++;
                    var listDataRow = new List<DataRowModel>();

                    var dataEmpIdRow = new DataRowModel();
                    dataEmpIdRow.ColumnKey = "employee_id";
                    dataEmpIdRow.ColumnValue = item.EmployeeId.ToString();
                    dataEmpIdRow.Width = "60px";
                    dataEmpIdRow.TextAlign = "center";
                    dataEmpIdRow.IsShow = false;
                    listDataRow.Add(dataEmpIdRow);

                    var dataRow = new DataRowModel();
                    dataRow.ColumnKey = "stt";
                    dataRow.ColumnValue = stt.ToString();
                    dataRow.ValueType = ValueTypeEnum.NUMBER;
                    dataRow.Width = "40!important";
                    dataRow.TextAlign = "center";
                    listDataRow.Add(dataRow);

                    var dataRow2 = new DataRowModel();
                    dataRow2.ColumnKey = "employeeCode";
                    dataRow2.ColumnValue = item.EmployeeCode;
                    dataRow2.Width = "150px";
                    dataRow2.TextAlign = "center";
                    listDataRow.Add(dataRow2);

                    var dataRow3 = new DataRowModel();
                    dataRow3.ColumnKey = "subCode1";
                    dataRow3.ColumnValue = item.SubCode1;
                    dataRow3.Width = "90px";
                    dataRow3.TextAlign = "left";
                    listDataRow.Add(dataRow3);

                    var dataRow4 = new DataRowModel();
                    dataRow4.ColumnKey = "employeeName";
                    dataRow4.ColumnValue = item.EmployeeName;
                    dataRow4.Width = "200px";
                    dataRow4.TextAlign = "left";
                    listDataRow.Add(dataRow4);

                    var dataRow5 = new DataRowModel();
                    dataRow5.ColumnKey = "positionName";
                    dataRow5.ColumnValue = item.PositionName;
                    dataRow5.Width = "130px";
                    dataRow5.TextAlign = "left";
                    listDataRow.Add(dataRow5);

                    var dataRow6 = new DataRowModel();
                    dataRow6.ColumnKey = "mucLuongHienTai";
                    dataRow6.ColumnValue = item.MucLuongHienTai.ToString();
                    dataRow6.Width = "200px";
                    dataRow6.TextAlign = "right";
                    dataRow6.ValueType = ValueTypeEnum.NUMBER;
                    listDataRow.Add(dataRow6);

                    var tongLuongOt = TongLuongOt(listLuongCtLoaiTroCapOt_ByEmp);

                    var dataRow7 = new DataRowModel();
                    dataRow7.ColumnKey = "tongLuongOt";
                    dataRow7.ColumnValue = tongLuongOt.ToString();
                    dataRow7.Width = "200px";
                    dataRow7.TextAlign = "right";
                    dataRow7.ValueType = ValueTypeEnum.NUMBER;
                    listDataRow.Add(dataRow7);

                    var luongOtTinhThue = LuongOtTinhThue(item.ToEntity(), listLuongCtLoaiTroCapOt_ByEmp);

                    var dataRow8 = new DataRowModel();
                    dataRow8.ColumnKey = "luongOtTinhThue";
                    dataRow8.ColumnValue = luongOtTinhThue.ToString();
                    dataRow8.Width = "200px";
                    dataRow8.TextAlign = "right";
                    dataRow8.ValueType = ValueTypeEnum.NUMBER;
                    listDataRow.Add(dataRow8);

                    int index = 0;
                    listLoaiOt.ForEach(loaiOt =>
                    {
                        var existsOt =
                            listLuongCtLoaiTroCapOt_ByEmp.FirstOrDefault(x => x.LoaiOtId == loaiOt.CategoryId);

                        //Nếu nhân viên có dữ liệu của loại OT này
                        if (existsOt != null)
                        {
                            index++;
                            var dataRowLoaiOt = new DataRowModel();
                            dataRowLoaiOt.ColumnKey = "index_" + index;
                            dataRowLoaiOt.ColumnValue = existsOt.MucTroCap.ToString();
                            dataRowLoaiOt.Width = "200px";
                            dataRowLoaiOt.TextAlign = "right";
                            dataRowLoaiOt.ValueType = ValueTypeEnum.NUMBER;
                            listDataRow.Add(dataRowLoaiOt);

                            index++;
                            var dataRowMucTroCap = new DataRowModel();
                            dataRowMucTroCap.ColumnKey = "index_" + index;
                            dataRowMucTroCap.ColumnValue = existsOt.SoGioOt.ToString();
                            dataRowMucTroCap.Width = "200px";
                            dataRowMucTroCap.TextAlign = "right";
                            dataRowMucTroCap.ValueType = ValueTypeEnum.NUMBER;
                            listDataRow.Add(dataRowMucTroCap);
                        }
                        //Nếu nhân viên không có dữ liệu của loại OT này
                        else
                        {
                            index++;
                            var dataRowLoaiOt = new DataRowModel();
                            dataRowLoaiOt.ColumnKey = "index_" + index;
                            dataRowLoaiOt.ColumnValue = "0";
                            dataRowLoaiOt.Width = "200px";
                            dataRowLoaiOt.TextAlign = "right";
                            dataRowLoaiOt.ValueType = ValueTypeEnum.NUMBER;
                            listDataRow.Add(dataRowLoaiOt);

                            index++;
                            var dataRowMucTroCap = new DataRowModel();
                            dataRowMucTroCap.ColumnKey = "index_" + index;
                            dataRowMucTroCap.ColumnValue = "0";
                            dataRowMucTroCap.Width = "200px";
                            dataRowMucTroCap.TextAlign = "right";
                            dataRowMucTroCap.ValueType = ValueTypeEnum.NUMBER;
                            listDataRow.Add(dataRowMucTroCap);
                        }
                    });

                    listDataTroCapOt.Add(listDataRow);
                });

                #endregion

                #region Header

                var listDataHeaderTroCapOt = new List<List<DataHeaderModel>>();

                /* tr1 */
                var listHeader1 = new List<DataHeaderModel>()
                {
                    new DataHeaderModel()
                    {
                        ColumnKey = "stt",
                        ColumnValue = "#",
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "90px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "employeeCode",
                        ColumnValue = "Mã nhân viên",
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "200px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "subCode1",
                        ColumnValue = "Mã phòng ban",
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "100px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "employeeName",
                        ColumnValue = "Họ tên",
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "200px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "positionName",
                        ColumnValue = "Chức vụ",
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "200px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "mucLuongHienTai",
                        ColumnValue = "Mức lương hiện tại (1 giờ)",
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "200px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "tongLuongOt",
                        ColumnValue = "Tổng lương OT",
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "200px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "luongOtTinhThue",
                        ColumnValue = "Tổng lương OT tính thuế",
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "200px",
                    },
                };

                int _index = 0;
                listLoaiOt.ForEach(loaiOt =>
                {
                    _index++;
                    var _dataHeader1 = new DataHeaderModel()
                    {
                        ColumnKey = "index_loai_" + _index,
                        ColumnValue = "Mức OT " + loaiOt.CategoryName,
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "200px",
                    };
                    listHeader1.Add(_dataHeader1);

                    _index++;
                    var _dataHeader2 = new DataHeaderModel()
                    {
                        ColumnKey = "index_gio_" + _index,
                        ColumnValue = "Số giờ " + loaiOt.CategoryName,
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "200px",
                    };
                    listHeader1.Add(_dataHeader2);
                });

                listDataHeaderTroCapOt.Add(listHeader1);

                #endregion

                #endregion

                #region Trợ cấp cố định

                var listLuongCtTroCapCoDinh = context.LuongCtTroCapCoDinh
                    .Where(x => x.KyLuongId == kyLuong.KyLuongId)
                    .Select(y => new LuongCtTroCapCoDinhModel(y)).ToList();

                var listLuongCtLoaiTroCapCoDinh = context.LuongCtLoaiTroCapCoDinh
                    .Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();

                var loaiTroCapCoDinhType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LTCCD");
                var listLoaiTroCapCoDinh = context.Category
                    .Where(x => x.CategoryTypeId == loaiTroCapCoDinhType.CategoryTypeId).ToList();

                var loaiHopDongType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LHDNS");
                var listLoaiHopDong = context.Category.Where(x => x.CategoryTypeId == loaiHopDongType.CategoryTypeId).ToList();

                #region Data

                var listDataTroCapCoDinh = new List<List<DataRowModel>>();

                int stt_tccd = 0;
                listLuongCtTroCapCoDinh.ForEach(item =>
                {
                    var _emp = listEmp.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                    var subCode1 = listGetSubCode1.FirstOrDefault(x => x.Value == _emp.SubCode1Value);
                    var _pos = listPosition.FirstOrDefault(x => x.PositionId == item.PositionId);

                    item.EmployeeCode = _emp?.EmployeeCode;
                    item.EmployeeName = _emp?.EmployeeName;
                    item.SubCode1 = subCode1?.Name;
                    item.PositionName = _pos?.PositionName;

                    var listLuongCtLoaiTroCapCoDinh_ByEmp = listLuongCtLoaiTroCapCoDinh
                        .Where(x => x.LuongCtTroCapCoDinhId == item.LuongCtTroCapCoDinhId).ToList();

                    stt_tccd++;
                    var listDataRow = new List<DataRowModel>();

                    var dataEmpIdRow = new DataRowModel();
                    dataEmpIdRow.ColumnKey = "employee_id";
                    dataEmpIdRow.ColumnValue = item.EmployeeId.ToString();
                    dataEmpIdRow.Width = "60px";
                    dataEmpIdRow.TextAlign = "center";
                    dataEmpIdRow.IsShow = false;
                    listDataRow.Add(dataEmpIdRow);

                    var dataRow = new DataRowModel();
                    dataRow.ColumnKey = "stt";
                    dataRow.ColumnValue = stt_tccd.ToString();
                    dataRow.ValueType = ValueTypeEnum.NUMBER;
                    dataRow.Width = "40px!important";
                    dataRow.TextAlign = "center";
                    listDataRow.Add(dataRow);

                    var dataRow2 = new DataRowModel();
                    dataRow2.ColumnKey = "employeeCode";
                    dataRow2.ColumnValue = item.EmployeeCode;
                    dataRow2.Width = "150px";
                    dataRow2.TextAlign = "center";
                    listDataRow.Add(dataRow2);

                    var dataRow3 = new DataRowModel();
                    dataRow3.ColumnKey = "subCode1";
                    dataRow3.ColumnValue = item.SubCode1;
                    dataRow3.Width = "90px";
                    dataRow3.TextAlign = "left";
                    listDataRow.Add(dataRow3);

                    var dataRow4 = new DataRowModel();
                    dataRow4.ColumnKey = "employeeName";
                    dataRow4.ColumnValue = item.EmployeeName;
                    dataRow4.Width = "200px";
                    dataRow4.TextAlign = "left";
                    listDataRow.Add(dataRow4);

                    var dataRow5 = new DataRowModel();
                    dataRow5.ColumnKey = "positionName";
                    dataRow5.ColumnValue = item.PositionName;
                    dataRow5.Width = "130px";
                    dataRow5.TextAlign = "left";
                    listDataRow.Add(dataRow5);

                    var loaiHopDong = listLoaiHopDong.FirstOrDefault(x => x.CategoryId == item.LoaiHopDongId);

                    var dataRow6 = new DataRowModel();
                    dataRow6.ColumnKey = "loaiHopDong";
                    dataRow6.ColumnValue = loaiHopDong?.CategoryName;
                    dataRow6.Width = "300px";
                    dataRow6.TextAlign = "left";
                    listDataRow.Add(dataRow6);

                    int index = 0;
                    listLoaiTroCapCoDinh.ForEach(loaiTroCap =>
                    {
                        var exists =
                            listLuongCtLoaiTroCapCoDinh_ByEmp.FirstOrDefault(x => x.LoaiTroCapId == loaiTroCap.CategoryId);

                        //Nếu nhân viên có dữ liệu của loại trợ cấp này
                        if (exists != null)
                        {
                            index++;
                            var dataRowLoaiTroCap = new DataRowModel();
                            dataRowLoaiTroCap.ColumnKey = "index_" + index;
                            dataRowLoaiTroCap.ColumnValue = exists.MucTroCap.ToString();
                            dataRowLoaiTroCap.Width = "200px";
                            dataRowLoaiTroCap.TextAlign = "right";
                            dataRowLoaiTroCap.ValueType = ValueTypeEnum.NUMBER;
                            listDataRow.Add(dataRowLoaiTroCap);

                            var dataRowLoaiTroCapId = new DataRowModel();
                            dataRowLoaiTroCapId.ColumnKey = "loaiTroCapCoDinhId_index_" + index;
                            dataRowLoaiTroCapId.ColumnValue = exists.LuongCtLoaiTroCapCoDinhId.ToString();
                            dataRowLoaiTroCapId.Width = "200px";
                            dataRowLoaiTroCapId.TextAlign = "right";
                            dataRowLoaiTroCapId.ValueType = ValueTypeEnum.NUMBER;
                            dataRowLoaiTroCapId.IsShow = false;
                            listDataRow.Add(dataRowLoaiTroCapId);
                        }
                        //Nếu nhân viên không có dữ liệu của loại trợ cấp này
                        else
                        {
                            index++;
                            var dataRowLoaiTroCap = new DataRowModel();
                            dataRowLoaiTroCap.ColumnKey = "index_" + index;
                            dataRowLoaiTroCap.ColumnValue = "0";
                            dataRowLoaiTroCap.Width = "200px";
                            dataRowLoaiTroCap.TextAlign = "right";
                            dataRowLoaiTroCap.ValueType = ValueTypeEnum.NUMBER;
                            listDataRow.Add(dataRowLoaiTroCap);

                            var dataRowLoaiTroCapId = new DataRowModel();
                            dataRowLoaiTroCapId.ColumnKey = "loaiTroCapCoDinhId_index_" + index;
                            dataRowLoaiTroCapId.ColumnValue = null;
                            dataRowLoaiTroCapId.Width = "200px";
                            dataRowLoaiTroCapId.TextAlign = "right";
                            dataRowLoaiTroCapId.ValueType = ValueTypeEnum.NUMBER;
                            dataRowLoaiTroCapId.IsShow = false;
                            listDataRow.Add(dataRowLoaiTroCapId);
                        }
                    });

                    listDataTroCapCoDinh.Add(listDataRow);
                });

                #endregion

                #region Header

                var listDataHeaderTroCapCoDinh = new List<List<DataHeaderModel>>();

                /* tr1 */
                var listHeader1_tccd = new List<DataHeaderModel>()
                {
                    new DataHeaderModel()
                    {
                        ColumnKey = "stt",
                        ColumnValue = "#",
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "90px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "employeeCode",
                        ColumnValue = "Mã nhân viên",
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "200px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "subCode1",
                        ColumnValue = "Mã phòng ban",
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "100px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "employeeName",
                        ColumnValue = "Họ tên",
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "200px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "positionName",
                        ColumnValue = "Chức vụ",
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "200px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "loaiHopDong",
                        ColumnValue = "Loại HĐ",
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "200px",
                    },
                };

                int _index_tccd = 0;
                listLoaiTroCapCoDinh.ForEach(loaiTroCap =>
                {
                    _index_tccd++;
                    var _dataHeader1 = new DataHeaderModel()
                    {
                        ColumnKey = "index_loai_" + _index_tccd,
                        ColumnValue = loaiTroCap.CategoryName,
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "200px",
                    };
                    listHeader1_tccd.Add(_dataHeader1);
                });

                listDataHeaderTroCapCoDinh.Add(listHeader1_tccd);

                #endregion

                #endregion

                #region Trợ cấp khác

                var loaiTroCapKhacType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LTCK");
                var listLoaiTroCapKhac = context.Category
                    .Where(x => x.CategoryTypeId == loaiTroCapKhacType.CategoryTypeId)
                    .Select(y => new CategoryEntityModel
                    {
                        CategoryId = y.CategoryId,
                        CategoryName = y.CategoryName
                    }).ToList();

                var listLuongCtTroCapKhac = context.LuongCtTroCapKhac
                    .Where(x => x.KyLuongId == kyLuong.KyLuongId)
                    .Select(y => new LuongCtTroCapKhacModel(y)).ToList();
                var listLuongCtLoaiTroCapKhac = context.LuongCtLoaiTroCapKhac
                    .Where(x => x.KyLuongId == kyLuong.KyLuongId)
                    .ToList();
                var listSelectedLoaiTroCapKhacId =
                    listLuongCtLoaiTroCapKhac.Select(y => y.LoaiTroCapId).Distinct().ToList();
                var listSelectedLoaiTroCapKhac = listLoaiTroCapKhac
                    .Where(x => listSelectedLoaiTroCapKhacId.Contains(x.CategoryId.Value)).ToList();

                #region Data

                var listDataTroCapKhac = new List<List<DataRowModel>>();

                int stt_tck = 0;
                listLuongCtTroCapKhac.ForEach(item =>
                {
                    var _emp = listEmp.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                    var subCode1 = listGetSubCode1.FirstOrDefault(x => x.Value == _emp.SubCode1Value);
                    var _pos = listPosition.FirstOrDefault(x => x.PositionId == item.PositionId);

                    item.EmployeeCode = _emp?.EmployeeCode;
                    item.EmployeeName = _emp?.EmployeeName;
                    item.SubCode1 = subCode1?.Name;
                    item.PositionName = _pos?.PositionName;

                    var listLuongCtLoaiTroCapKhac_ByEmp = listLuongCtLoaiTroCapKhac
                        .Where(x => x.LuongCtTroCapKhacId == item.LuongCtTroCapKhacId).ToList();

                    stt_tck++;
                    var listDataRow = new List<DataRowModel>();

                    var dataEmpIdRow = new DataRowModel();
                    dataEmpIdRow.ColumnKey = "employee_id";
                    dataEmpIdRow.ColumnValue = item.EmployeeId.ToString();
                    dataEmpIdRow.Width = "60px";
                    dataEmpIdRow.TextAlign = "center";
                    dataEmpIdRow.IsShow = false;
                    listDataRow.Add(dataEmpIdRow);

                    var dataRow = new DataRowModel();
                    dataRow.ColumnKey = "stt";
                    dataRow.ColumnValue = stt_tck.ToString();
                    dataRow.ValueType = ValueTypeEnum.NUMBER;
                    dataRow.Width = "40px!important";
                    dataRow.TextAlign = "center";
                    listDataRow.Add(dataRow);

                    var dataRow2 = new DataRowModel();
                    dataRow2.ColumnKey = "employeeCode";
                    dataRow2.ColumnValue = item.EmployeeCode;
                    dataRow2.Width = "150px";
                    dataRow2.TextAlign = "center";
                    listDataRow.Add(dataRow2);

                    var dataRow3 = new DataRowModel();
                    dataRow3.ColumnKey = "subCode1";
                    dataRow3.ColumnValue = item.SubCode1;
                    dataRow3.Width = "130px";
                    dataRow3.TextAlign = "left";
                    listDataRow.Add(dataRow3);

                    var dataRow4 = new DataRowModel();
                    dataRow4.ColumnKey = "employeeName";
                    dataRow4.ColumnValue = item.EmployeeName;
                    dataRow4.Width = "200px";
                    dataRow4.TextAlign = "left";
                    listDataRow.Add(dataRow4);

                    var dataRow5 = new DataRowModel();
                    dataRow5.ColumnKey = "positionName";
                    dataRow5.ColumnValue = item.PositionName;
                    dataRow5.Width = "130px";
                    dataRow5.TextAlign = "left";
                    listDataRow.Add(dataRow5);

                    var loaiHopDong = listLoaiHopDong.FirstOrDefault(x => x.CategoryId == item.LoaiHopDongId);

                    var dataRow6 = new DataRowModel();
                    dataRow6.ColumnKey = "loaiHopDong";
                    dataRow6.ColumnValue = loaiHopDong?.CategoryName;
                    dataRow6.Width = "300px";
                    dataRow6.TextAlign = "left";
                    listDataRow.Add(dataRow6);

                    int index = 0;
                    listSelectedLoaiTroCapKhac.ForEach(loaiTroCap =>
                    {
                        var exists =
                            listLuongCtLoaiTroCapKhac_ByEmp.FirstOrDefault(x => x.LoaiTroCapId == loaiTroCap.CategoryId);

                        //Nếu nhân viên có dữ liệu của loại trợ cấp này
                        if (exists != null)
                        {
                            index++;
                            var dataRowLoaiTroCap = new DataRowModel();
                            dataRowLoaiTroCap.ColumnKey = "index_" + index;
                            dataRowLoaiTroCap.ColumnValue = exists.MucTroCap.ToString();
                            dataRowLoaiTroCap.Width = "200px";
                            dataRowLoaiTroCap.TextAlign = "right";
                            dataRowLoaiTroCap.ValueType = ValueTypeEnum.NUMBER;
                            listDataRow.Add(dataRowLoaiTroCap);

                            var dataRowLoaiTroCapId = new DataRowModel();
                            dataRowLoaiTroCapId.ColumnKey = "loaiTroCapKhacId_index_" + index;
                            dataRowLoaiTroCapId.ColumnValue = exists.LuongCtLoaiTroCapKhacId.ToString();
                            dataRowLoaiTroCapId.Width = "200px";
                            dataRowLoaiTroCapId.TextAlign = "right";
                            dataRowLoaiTroCapId.ValueType = ValueTypeEnum.NUMBER;
                            dataRowLoaiTroCapId.IsShow = false;
                            listDataRow.Add(dataRowLoaiTroCapId);

                            var dataRowIsEdit = new DataRowModel();
                            dataRowIsEdit.ColumnKey = "isEdit_index_" + index;
                            dataRowIsEdit.ColumnValue = exists.IsEdit.ToString();
                            dataRowIsEdit.Width = "200px";
                            dataRowIsEdit.TextAlign = "right";
                            dataRowIsEdit.ValueType = ValueTypeEnum.BOOL;
                            dataRowIsEdit.IsShow = false;
                            listDataRow.Add(dataRowIsEdit);
                        }
                    });

                    listDataTroCapKhac.Add(listDataRow);
                });

                #endregion

                #region Header

                var listDataHeaderTroCapKhac = new List<List<DataHeaderModel>>();

                /* tr1 */
                var listHeader1_tck = new List<DataHeaderModel>()
                {
                    new DataHeaderModel()
                    {
                        ColumnKey = "stt",
                        ColumnValue = "#",
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "90px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "employeeCode",
                        ColumnValue = "Mã nhân viên",
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "200px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "subCode1",
                        ColumnValue = "Mã phòng ban",
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "100px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "employeeName",
                        ColumnValue = "Họ tên",
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "200px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "positionName",
                        ColumnValue = "Chức vụ",
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "200px",
                    },
                    new DataHeaderModel()
                    {
                        ColumnKey = "loaiHopDong",
                        ColumnValue = "Loại HĐ",
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "200px",
                    },
                };

                int _index_tck = 0;
                listSelectedLoaiTroCapKhac.ForEach(loaiTroCap =>
                {
                    _index_tck++;
                    var _dataHeader1 = new DataHeaderModel()
                    {
                        ColumnKey = "index_loai_" + _index_tck,
                        ColumnValue = loaiTroCap.CategoryName,
                        Rowspan = 0,
                        Colspan = 0,
                        Width = "200px",
                    };
                    listHeader1_tck.Add(_dataHeader1);
                });

                listDataHeaderTroCapKhac.Add(listHeader1_tck);

                #endregion

                #endregion

                #region Điều kiện hiển thị các button

                bool isShowGuiPheDuyet = false;
                    bool isShowPheDuyet = false;
                    bool isShowTuChoi = false;
                    bool isShowDatVeMoi = false;
                    bool isShowXoa = false;
                    bool isShowSua = false;

                    //Trạng thái Mới và User đăng nhập là người tạo đề xuất
                    if (kyLuong.TrangThai == 1 && kyLuong.CreatedById == user.UserId)
                    {
                        isShowGuiPheDuyet = true;
                        isShowXoa = true;
                        isShowSua = true;
                    }

                    //Trạng thái Chờ phê duyệt
                    if (kyLuong.TrangThai == 2)
                    {
                        var buocHienTai = context.CacBuocApDung.Where(x => x.ObjectNumber == kyLuong.KyLuongId &&
                                                                           x.DoiTuongApDung == 14 &&
                                                                           x.TrangThai == 0)
                            .OrderByDescending(z => z.Stt)
                            .FirstOrDefault();

                        //Nếu là phê duyệt trưởng bộ phận
                        if (buocHienTai?.LoaiPheDuyet == 1)
                        {
                            //Lấy list phòng ban của người tạo đề xuất
                            var listPhongBanId_NguoiPhuTrach = context.ThanhVienPhongBan
                                .Where(x => x.EmployeeId == empCreated.EmployeeId)
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
                    if (kyLuong.TrangThai == 4 && kyLuong.CreatedById == user.UserId)
                    {
                        isShowDatVeMoi = true;
                    }

                    #endregion

                return new GetKyLuongByIdResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "OK",
                    KyLuong = kyLuong,
                    ListTongHopChamCong = listTongHopChamCong,
                    ListLuongTongHop = listLuongTongHop,
                    ListLuongCtThuNhapTinhThue = listLuongCtThuNhapTinhThue,
                    ListLuongCtBaoHiem = listLuongCtBaoHiem,
                    ListLuongCtGiamTruTruocThue = listLuongCtGiamTruTruocThue,
                    ListLuongCtGiamTruSauThue = listLuongCtGiamTruSauThue,
                    ListLuongCtHoanLaiSauThue = listLuongCtHoanLaiSauThue,
                    ListLuongCtCtyDong = listLuongCtCtyDong,
                    ListLuongCtOther = listLuongCtOther,
                    ListDataTroCapOt = listDataTroCapOt,
                    ListDataHeaderTroCapOt = listDataHeaderTroCapOt,
                    ListDataTroCapCoDinh = listDataTroCapCoDinh,
                    ListDataHeaderTroCapCoDinh = listDataHeaderTroCapCoDinh,
                    ListLoaiTroCapKhac = listLoaiTroCapKhac,
                    ListSelectedLoaiTroCapKhac = listSelectedLoaiTroCapKhac,
                    ListDataTroCapKhac = listDataTroCapKhac,
                    ListDataHeaderTroCapKhac = listDataHeaderTroCapKhac,
                    IsShowGuiPheDuyet = isShowGuiPheDuyet,
                    IsShowPheDuyet = isShowPheDuyet,
                    IsShowDatVeMoi = isShowDatVeMoi,
                    IsShowTuChoi = isShowTuChoi,
                    IsShowXoa = isShowXoa,
                    IsShowSua = isShowSua
                };
            }
            catch (Exception e)
            {
                return new GetKyLuongByIdResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public SaveThuNhapTinhThueResult SaveThuNhapTinhThue(SaveThuNhapTinhThueParameter parameter)
        {
            try
            {
                var listUpdate = new List<LuongCtThuNhapTinhThue>();

                parameter.ListLuongCtThuNhapTinhThue.ForEach(item =>
                {
                    listUpdate.Add(item.ToEntity());
                });

                context.LuongCtThuNhapTinhThue.UpdateRange(listUpdate);
                context.SaveChanges();

                return new SaveThuNhapTinhThueResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Lưu thành công"
                };
            }
            catch (Exception e)
            {
                return new SaveThuNhapTinhThueResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public SaveBaoHiemResult SaveBaoHiem(SaveBaoHiemParameter parameter)
        {
            try
            {
                var listUpdate = new List<LuongCtBaoHiem>();

                parameter.ListLuongCtBaoHiem.ForEach(item =>
                {
                    listUpdate.Add(item.ToEntity());
                });

                context.LuongCtBaoHiem.UpdateRange(listUpdate);
                context.SaveChanges();

                return new SaveBaoHiemResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Lưu thành công"
                };
            }
            catch (Exception e)
            {
                return new SaveBaoHiemResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public SaveGiamTruTruocThueResult SaveGiamTruTruocThue(SaveGiamTruTruocThueParameter parameter)
        {
            try
            {
                var listUpdate = new List<LuongCtGiamTruTruocThue>();

                parameter.ListLuongCtGiamTruTruocThue.ForEach(item =>
                {
                    listUpdate.Add(item.ToEntity());
                });

                context.LuongCtGiamTruTruocThue.UpdateRange(listUpdate);
                context.SaveChanges();

                return new SaveGiamTruTruocThueResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Lưu thành công"
                };
            }
            catch (Exception e)
            {
                return new SaveGiamTruTruocThueResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public SaveGiamTruSauThueResult SaveGiamTruSauThue(SaveGiamTruSauThueParameter parameter)
        {
            try
            {
                var listUpdate = new List<LuongCtGiamTruSauThue>();

                parameter.ListLuongCtGiamTruSauThue.ForEach(item =>
                {
                    listUpdate.Add(item.ToEntity());
                });

                context.LuongCtGiamTruSauThue.UpdateRange(listUpdate);
                context.SaveChanges();

                return new SaveGiamTruSauThueResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Lưu thành công"
                };
            }
            catch (Exception e)
            {
                return new SaveGiamTruSauThueResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public SaveHoanLaiSauThueResult SaveHoanLaiSauThue(SaveHoanLaiSauThueParameter parameter)
        {
            try
            {
                var listUpdate = new List<LuongCtHoanLaiSauThue>();

                parameter.ListLuongCtHoanLaiSauThue.ForEach(item =>
                {
                    listUpdate.Add(item.ToEntity());
                });

                context.LuongCtHoanLaiSauThue.UpdateRange(listUpdate);
                context.SaveChanges();

                return new SaveHoanLaiSauThueResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Lưu thành công"
                };
            }
            catch (Exception e)
            {
                return new SaveHoanLaiSauThueResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public SaveCtyDongResult SaveCtyDong(SaveCtyDongParameter parameter)
        {
            try
            {
                var listUpdate = new List<LuongCtCtyDong>();

                parameter.ListLuongCtCtyDong.ForEach(item =>
                {
                    listUpdate.Add(item.ToEntity());
                });

                context.LuongCtCtyDong.UpdateRange(listUpdate);
                context.SaveChanges();

                return new SaveCtyDongResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Lưu thành công"
                };
            }
            catch (Exception e)
            {
                return new SaveCtyDongResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public SaveOtherResult SaveOther(SaveOtherParameter parameter)
        {
            try
            {
                var listUpdate = new List<LuongCtOther>();

                parameter.ListLuongCtOther.ForEach(item =>
                {
                    listUpdate.Add(item.ToEntity());
                });

                context.LuongCtOther.UpdateRange(listUpdate);
                context.SaveChanges();

                return new SaveOtherResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Lưu thành công"
                };
            }
            catch (Exception e)
            {
                return new SaveOtherResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetListDieuKienTroCapCoDinhResult GetListDieuKienTroCapCoDinh(GetListDieuKienTroCapCoDinhParameter parameter)
        {
            try
            {
                var luongCtLoaiTroCapCoDinh = context.LuongCtLoaiTroCapCoDinh.FirstOrDefault(x =>
                    x.LuongCtLoaiTroCapCoDinhId == parameter.LuongCtLoaiTroCapCoDinhId);
                if (luongCtLoaiTroCapCoDinh == null)
                {
                    return new GetListDieuKienTroCapCoDinhResult()
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        MessageCode = "Data loại trợ cấp không tồn tại trên hệ thống"
                    };
                }

                var loaiTroCapCoDinhType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LTCCD");
                var listLoaiTroCapCoDinh = context.Category
                    .Where(x => x.CategoryTypeId == loaiTroCapCoDinhType.CategoryTypeId).ToList();

                var loaiTroCapCoDinh =
                    listLoaiTroCapCoDinh.FirstOrDefault(x => x.CategoryId == luongCtLoaiTroCapCoDinh.LoaiTroCapId);
                if (loaiTroCapCoDinh == null)
                {
                    return new GetListDieuKienTroCapCoDinhResult()
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        MessageCode = "Loại trợ cấp không tồn tại trên hệ thống"
                    };
                }

                var listLuongCtDieuKienTroCapCoDinh = context.LuongCtDieuKienTroCapCoDinh.Where(x =>
                    x.LuongCtLoaiTroCapCoDinhId == luongCtLoaiTroCapCoDinh.LuongCtLoaiTroCapCoDinhId)
                    .Select(y => new LuongCtDieuKienTroCapCoDinhModel(y)).ToList();

                var dieuKienHuongType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "DKHTC");
                var listDieuKienHuong = context.Category
                    .Where(x => x.CategoryTypeId == dieuKienHuongType.CategoryTypeId).ToList();

                listLuongCtDieuKienTroCapCoDinh.ForEach(item =>
                {
                    var dieuKienHuong = listDieuKienHuong.FirstOrDefault(x => x.CategoryId == item.DieuKienHuongId);
                    item.DieuKienHuong = dieuKienHuong?.CategoryName;
                });

                return new GetListDieuKienTroCapCoDinhResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "OK",
                    LoaiTroCap = loaiTroCapCoDinh?.CategoryName,
                    ListLuongCtDieuKienTroCapCoDinh = listLuongCtDieuKienTroCapCoDinh
                };
            }
            catch (Exception e)
            {
                return new GetListDieuKienTroCapCoDinhResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public UpdateDieuKienTroCapCoDinhResult UpdateDieuKienTroCapCoDinh(UpdateDieuKienTroCapCoDinhParameter parameter)
        {
            try
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    var kyLuong = context.KyLuong.FirstOrDefault(x =>
                        x.KyLuongId == parameter.LuongCtDieuKienTroCapCoDinh.KyLuongId);

                    if (kyLuong.TrangThai != 1)
                    {
                        return new UpdateDieuKienTroCapCoDinhResult()
                        {
                            StatusCode = HttpStatusCode.Conflict,
                            MessageCode = "Lưu thất bại, kỳ lương phải ở trạng thái mới"
                        };
                    }

                    //Lấy các điều kiện hưởng trong các loại trợ cấp khác nếu cùng điều kiện hưởng
                    var listUpdate = context.LuongCtDieuKienTroCapCoDinh.Where(x =>
                            x.LuongCtTroCapCoDinhId == parameter.LuongCtDieuKienTroCapCoDinh.LuongCtTroCapCoDinhId &&
                            x.DieuKienHuongId == parameter.LuongCtDieuKienTroCapCoDinh.DieuKienHuongId)
                        .ToList();

                    listUpdate.ForEach(item => { item.DuDieuKien = parameter.LuongCtDieuKienTroCapCoDinh.DuDieuKien; });
                    
                    context.LuongCtDieuKienTroCapCoDinh.UpdateRange(listUpdate);
                    context.SaveChanges();

                    #region Tính lại trợ cấp

                    var listTroCapCoDinh = context.TroCap.Where(x => x.TypeId != 4).ToList();
                    var luongCtTroCapCoDinh = context.LuongCtTroCapCoDinh
                        .FirstOrDefault(x =>
                            x.LuongCtTroCapCoDinhId == parameter.LuongCtDieuKienTroCapCoDinh.LuongCtTroCapCoDinhId);

                    //Lấy list loại trợ cấp có thay đổi
                    var listLuongCtLoaiTroCapCoDinhId = listUpdate.Select(y => y.LuongCtLoaiTroCapCoDinhId).ToList();
                    var listLuongCtLoaiTroCapCoDinh = context.LuongCtLoaiTroCapCoDinh
                        .Where(x => listLuongCtLoaiTroCapCoDinhId.Contains(x.LuongCtLoaiTroCapCoDinhId))
                        .ToList();

                    var listChamCongByEmp = context.ChamCong
                        .Where(x => x.NgayChamCong.Date >= kyLuong.TuNgay.Date &&
                                    x.NgayChamCong.Date <= kyLuong.DenNgay.Date &&
                                    x.EmployeeId == luongCtTroCapCoDinh.EmployeeId).ToList();
                    var listMucHuongTheoNgayNghi = context.MucHuongTheoNgayNghi.ToList();

                    var listDmvs = context.ThongKeDiMuonVeSom
                        .Where(x => x.EmployeeId == luongCtTroCapCoDinh.EmployeeId &&
                                    x.NgayDmvs.Date >= kyLuong.TuNgay.Date &&
                                    x.NgayDmvs.Date <= kyLuong.DenNgay.Date).ToList();

                    listLuongCtLoaiTroCapCoDinh.ForEach(item =>
                    {
                        var troCap =
                            listTroCapCoDinh.FirstOrDefault(x => x.LoaiTroCapId == item.LoaiTroCapId);

                        //
                        if (troCap != null && troCap.TypeId == 1)
                        {
                            var ngayLamViecThucTe = context.TongHopChamCong
                                                        .FirstOrDefault(x => x.KyLuongId == item.KyLuongId &&
                                                                             x.EmployeeId == luongCtTroCapCoDinh.EmployeeId)
                                                        ?.NgayLamViecThucTe ?? 0;

                            var countFalse = context.LuongCtDieuKienTroCapCoDinh.Count(x =>
                                x.KyLuongId == item.KyLuongId &&
                                x.LuongCtLoaiTroCapCoDinhId == item.LuongCtLoaiTroCapCoDinhId &&
                                x.DuDieuKien == false);

                            //Nếu tất cả điều kiện hưởng đều = true
                            if (countFalse == 0)
                            {
                                item.MucTroCap =
                                    Math.Round(troCap.MucTroCap * ngayLamViecThucTe / kyLuong.SoNgayLamViec, 0);
                            }
                            //Nếu có điều kiện hưởng = false
                            else
                            {
                                item.MucTroCap = 0;
                            }
                        }
                        //Trợ cấp theo ngày nghỉ
                        else if (troCap != null && troCap.TypeId == 2)
                        {
                            var listMucHuong = listMucHuongTheoNgayNghi
                                .Where(x => x.TroCapId == troCap.TroCapId)
                                .OrderByDescending(z => z.MucHuongPhanTram).ToList();

                            var countFalse = context.LuongCtDieuKienTroCapCoDinh.Count(x =>
                                    x.KyLuongId == item.KyLuongId &&
                                    x.LuongCtLoaiTroCapCoDinhId == item.LuongCtLoaiTroCapCoDinhId &&
                                    x.DuDieuKien == false);

                            //Nếu tất cả điều kiện hưởng đều = true
                            if (countFalse == 0)
                            {
                                item.MucTroCap = TinhMucHuongTroCapTheoNgayNghi(troCap, listChamCongByEmp, listMucHuong);
                            }
                            //Nếu có điều kiện hưởng = false
                            else
                            {
                                item.MucTroCap = 0;
                            }
                        }
                        //Trợ cấp theo số lần đi muộn về sớm
                        else if (troCap != null && troCap.TypeId == 3)
                        {
                            var listMucHuong = context.MucHuongDmvs.Where(x => x.TroCapId == troCap.TroCapId)
                                .OrderByDescending(z => z.MucTru).ToList();

                            var countFalse = context.LuongCtDieuKienTroCapCoDinh.Count(x =>
                                    x.KyLuongId == item.KyLuongId &&
                                    x.LuongCtLoaiTroCapCoDinhId == item.LuongCtLoaiTroCapCoDinhId &&
                                    x.DuDieuKien == false);

                            //Nếu tất cả điều kiện hưởng đều = true
                            if (countFalse == 0)
                            {
                                int soLanDmvs = GetSoLanDmvs(luongCtTroCapCoDinh.EmployeeId, listDmvs);

                                item.MucTroCap = TinhMucHuongTroCapTheoDmvs(troCap, soLanDmvs, listMucHuong);
                            }
                            //Nếu có điều kiện hưởng = false
                            else
                            {
                                item.MucTroCap = 0;
                            }
                        }
                    });

                    context.LuongCtLoaiTroCapCoDinh.UpdateRange(listLuongCtLoaiTroCapCoDinh);
                    context.SaveChanges();

                    #endregion

                    trans.Commit();

                    return new UpdateDieuKienTroCapCoDinhResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = "Lưu thành công"
                    };
                }
            }
            catch (Exception e)
            {
                return new UpdateDieuKienTroCapCoDinhResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public SaveLuongCtTroCapKhacResult SaveLuongCtTroCapKhac(SaveLuongCtTroCapKhacParameter parameter)
        {
            try
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    var kyLuong = context.KyLuong.FirstOrDefault(x => x.KyLuongId == parameter.KyLuongId);
                    if (kyLuong == null)
                    {
                        return new SaveLuongCtTroCapKhacResult()
                        {
                            StatusCode = HttpStatusCode.NotFound,
                            MessageCode = "Kỳ lương không tồn tại"
                        };
                    }

                    var listLoaiTroCapKhac = context.Category
                        .Where(x => parameter.ListLoaiTroCapKhacId.Contains(x.CategoryId)).ToList();

                    var listChamCong = context.ChamCong.Where(x => x.NgayChamCong.Date >= kyLuong.TuNgay.Date &&
                                                                   x.NgayChamCong.Date <= kyLuong.DenNgay.Date).ToList();
                    var listEmpId = listChamCong.Select(y => y.EmployeeId).Distinct().ToList();
                    var listEmp = context.Employee.Join(context.User,
                            emp => emp.EmployeeId,
                            user => user.EmployeeId,
                            (emp, user) => new { Emp = emp, User = user })
                        .Where(x => listEmpId.Contains(x.Emp.EmployeeId) &&
                                    x.User.IsAdmin != true).OrderBy(z => z.Emp.EmployeeCode).ToList();

                    listEmp.ForEach(item => { SaveLuongCt_TroCapKhac(kyLuong.KyLuongId, item.Emp, listLoaiTroCapKhac); });

                    trans.Commit();

                    return new SaveLuongCtTroCapKhacResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = "Lưu thành công"
                    };
                }
            }
            catch (Exception e)
            {
                return new SaveLuongCtTroCapKhacResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetListDieuKienTroCapKhacResult GetListDieuKienTroCapKhac(GetListDieuKienTroCapKhacParameter parameter)
        {
            try
            {
                var luongCtLoaiTroCapKhac = context.LuongCtLoaiTroCapKhac.FirstOrDefault(x =>
                    x.LuongCtLoaiTroCapKhacId == parameter.LuongCtLoaiTroCapKhacId);
                if (luongCtLoaiTroCapKhac == null)
                {
                    return new GetListDieuKienTroCapKhacResult()
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        MessageCode = "Data loại trợ cấp không tồn tại trên hệ thống"
                    };
                }

                var loaiTroCapKhacType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LTCK");
                var listLoaiTroCapKhac = context.Category
                    .Where(x => x.CategoryTypeId == loaiTroCapKhacType.CategoryTypeId).ToList();

                var loaiTroCapKhac =
                    listLoaiTroCapKhac.FirstOrDefault(x => x.CategoryId == luongCtLoaiTroCapKhac.LoaiTroCapId);
                if (listLoaiTroCapKhac == null)
                {
                    return new GetListDieuKienTroCapKhacResult()
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        MessageCode = "Loại trợ cấp không tồn tại trên hệ thống"
                    };
                }

                bool isEdit = loaiTroCapKhac.IsEdit;
                var existCauHinhLoaiTroCap =
                    context.TroCap.FirstOrDefault(x => x.LoaiTroCapId == luongCtLoaiTroCapKhac.LoaiTroCapId);
                bool isCoCauHinhLoaiTroCap = existCauHinhLoaiTroCap != null;

                var luongCtTroCapKhac = context.LuongCtTroCapKhac.FirstOrDefault(x =>
                    x.LuongCtTroCapKhacId == luongCtLoaiTroCapKhac.LuongCtTroCapKhacId);
                if (luongCtTroCapKhac == null)
                {
                    return new GetListDieuKienTroCapKhacResult()
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        MessageCode = "Data trợ cấp khác không tồn tại trên hệ thống"
                    };
                }

                var listLuongCtEmpDktcKhac = context.LuongCtEmpDktcKhac.Where(x =>
                    x.EmployeeId == luongCtTroCapKhac.EmployeeId &&
                    x.KyLuongId == luongCtTroCapKhac.KyLuongId).ToList();

                var listLuongCtDieuKienTroCapKhac = context.LuongCtDieuKienTroCapKhac.Where(x =>
                    x.LuongCtLoaiTroCapKhacId == luongCtLoaiTroCapKhac.LuongCtLoaiTroCapKhacId)
                    .Select(y => new LuongCtDieuKienTroCapKhacModel(y)).ToList();

                if (isCoCauHinhLoaiTroCap && listLuongCtDieuKienTroCapKhac.Count == 0)
                {
                    return new GetListDieuKienTroCapKhacResult()
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        MessageCode = "Loại trợ cấp này không có điều kiện hưởng hoặc nhân viên không đủ điều kiện hưởng trợ cấp"
                    };
                }

                var dieuKienHuongType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "DKHTC");
                var listDieuKienHuong = context.Category
                    .Where(x => x.CategoryTypeId == dieuKienHuongType.CategoryTypeId).ToList();

                listLuongCtDieuKienTroCapKhac.ForEach(item =>
                {
                    var dieuKienHuong = listDieuKienHuong.FirstOrDefault(x => x.CategoryId == item.DieuKienHuongId);
                    item.DieuKienHuong = dieuKienHuong?.CategoryName;

                    var duDieuKienMapping =
                        listLuongCtEmpDktcKhac.FirstOrDefault(x => x.DieuKienHuongId == item.DieuKienHuongId);
                    item.DuDieuKien = duDieuKienMapping.DuDieuKien;
                });

                return new GetListDieuKienTroCapKhacResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "OK",
                    LoaiTroCap = loaiTroCapKhac?.CategoryName,
                    MucTroCap = luongCtLoaiTroCapKhac.MucTroCap,
                    ListLuongCtDieuKienTroCapKhac = listLuongCtDieuKienTroCapKhac,
                    IsCoCauHinhLoaiTroCap = isCoCauHinhLoaiTroCap
                };
            }
            catch (Exception e)
            {
                return new GetListDieuKienTroCapKhacResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public UpdateDieuKienTroCapKhacResult UpdateDieuKienTroCapKhac(UpdateDieuKienTroCapKhacParameter parameter)
        {
            try
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    var kyLuong = context.KyLuong.FirstOrDefault(x =>
                        x.KyLuongId == parameter.LuongCtDieuKienTroCapKhac.KyLuongId);

                    if (kyLuong.TrangThai != 1)
                    {
                        return new UpdateDieuKienTroCapKhacResult()
                        {
                            StatusCode = HttpStatusCode.Conflict,
                            MessageCode = "Lưu thất bại, kỳ lương phải ở trạng thái mới"
                        };
                    }

                    SaveLuongCtEmpDktcKhac(parameter.EmployeeId, kyLuong.KyLuongId,
                        parameter.LuongCtDieuKienTroCapKhac.DieuKienHuongId,
                        parameter.LuongCtDieuKienTroCapKhac.DuDieuKien);

                    #region Tính lại trợ cấp

                    var listTroCapKhac = context.TroCap.Where(x => x.TypeId == 4).ToList();
                    var luongCtTroCapKhac = context.LuongCtTroCapKhac
                        .FirstOrDefault(x =>
                            x.LuongCtTroCapKhacId == parameter.LuongCtDieuKienTroCapKhac.LuongCtTroCapKhacId);

                    //Lấy list loại trợ cấp có thay đổi
                    var listLuongCtLoaiTroCapKhac = context.LuongCtLoaiTroCapKhac
                        .Where(x => x.LuongCtTroCapKhacId == luongCtTroCapKhac.LuongCtTroCapKhacId)
                        .ToList();
                    var listDieuKienHuong = context.LuongCtDieuKienTroCapKhac
                        .Where(x => x.LuongCtTroCapKhacId == luongCtTroCapKhac.LuongCtTroCapKhacId).ToList();

                    listLuongCtLoaiTroCapKhac.ForEach(item =>
                    {
                        var troCap =
                            listTroCapKhac.FirstOrDefault(x => x.LoaiTroCapId == item.LoaiTroCapId);

                        if (troCap != null)
                        {
                            var listDieuKienHuongId = listDieuKienHuong.Where(x =>
                                    x.LuongCtLoaiTroCapKhacId == item.LuongCtLoaiTroCapKhacId)
                                .Select(y => y.DieuKienHuongId).ToList();

                            var countFalse = context.LuongCtEmpDktcKhac.Count(x =>
                                x.EmployeeId == parameter.EmployeeId &&
                                x.KyLuongId == kyLuong.KyLuongId &&
                                listDieuKienHuongId.Contains(x.DieuKienHuongId) &&
                                x.DuDieuKien == false);

                            //Nếu tất cả điều kiện hưởng đều = true
                            if (countFalse == 0)
                            {
                                item.MucTroCap = troCap.MucTroCap;
                            }
                            //Nếu có điều kiện hưởng = false
                            else
                            {
                                item.MucTroCap = 0;
                            }
                        }
                    });

                    context.LuongCtLoaiTroCapKhac.UpdateRange(listLuongCtLoaiTroCapKhac);
                    context.SaveChanges();

                    #endregion

                    trans.Commit();

                    return new UpdateDieuKienTroCapKhacResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = "Lưu thành công"
                    };
                }
            }
            catch (Exception e)
            {
                return new UpdateDieuKienTroCapKhacResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public UpdateMucTroCapKhacResult UpdateMucTroCapKhac(UpdateMucTroCapKhacParameter parameter)
        {
            try
            {
                var loaiTroCapKhac = context.LuongCtLoaiTroCapKhac.FirstOrDefault(x =>
                    x.LuongCtLoaiTroCapKhacId == parameter.LuongCtLoaiTroCapKhacId);
                if (loaiTroCapKhac == null)
                {
                    return new UpdateMucTroCapKhacResult()
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        MessageCode = "Loại trợ cấp không tồn tại trên hệ thống"
                    };
                }

                loaiTroCapKhac.MucTroCap = parameter.MucTroCap;
                context.LuongCtLoaiTroCapKhac.Update(loaiTroCapKhac);
                context.SaveChanges();

                return new UpdateMucTroCapKhacResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Lưu thành công"
                };
            }
            catch (Exception e)
            {
                return new UpdateMucTroCapKhacResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public ImportTroCapKhacResult ImportTroCapKhac(ImportTroCapKhacParameter parameter)
        {
            try
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    if (parameter.ListData.Count == 0)
                    {
                        return new ImportTroCapKhacResult()
                        {
                            StatusCode = HttpStatusCode.NotFound,
                            MessageCode = "File import không có dữ liệu"
                        };
                    }

                    var kyLuong = context.KyLuong.FirstOrDefault(x => x.KyLuongId == parameter.KyLuongId);
                    if (kyLuong == null)
                    {
                        return new ImportTroCapKhacResult()
                        {
                            StatusCode = HttpStatusCode.NotFound,
                            MessageCode = "Kỳ lương không tồn tại trên hệ thống"
                        };
                    }

                    var listEmp = context.Employee.ToList();
                    var listCauHinhTroCapKhac = context.TroCap.Where(x => x.TypeId == 4).ToList();
                    var listLuongCtTroCapKhac =
                        context.LuongCtTroCapKhac.Where(x => x.KyLuongId == parameter.KyLuongId).ToList();
                    var listLoaiTroCapKhacId = context.LuongCtLoaiTroCapKhac.Where(x => x.KyLuongId == parameter.KyLuongId)
                        .Select(y => y.LoaiTroCapId).Distinct().ToList();
                    var listLoaiTroCapKhac = context.Category
                        .Where(x => listLoaiTroCapKhacId.Contains(x.CategoryId)).ToList();

                    var messEmpHeThong = new List<string>();
                    var messEmpBangLuong = new List<string>();
                    var messLoaiTroCap = new List<string>();

                    var listUpdate = new List<LuongCtLoaiTroCapKhac>();

                    parameter.ListData.ForEach(item =>
                    {
                        var emp = listEmp.FirstOrDefault(x =>
                            x.EmployeeCode.ToLower().Trim() == item.EmployeeCode?.ToLower()?.Trim());

                        if (emp == null)
                        {
                            messEmpHeThong.Add(item.EmployeeCode);
                        }
                        else
                        {
                            var empBangLuong = listLuongCtTroCapKhac.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId);

                            //Nếu không có trong bảng lương
                            if (empBangLuong == null)
                            {
                                messEmpBangLuong.Add(item.EmployeeCode);
                            }
                            //Nếu có trong bảng lương
                            else
                            {
                                item.ListDataLoaiTroCap.ForEach(_item =>
                                {
                                    var loaiTroCap = listLoaiTroCapKhac.FirstOrDefault(x =>
                                        x.CategoryName.Trim() == _item.TenLoaiTroCap?.Trim());

                                    //Nếu tên loại trợ cấp không đúng
                                    if (loaiTroCap == null)
                                    {
                                        messLoaiTroCap.Add(_item.TenLoaiTroCap);
                                    }
                                    //Nếu tên loại trợ cấp đúng trong bảng mapping (bảng 2)
                                    else
                                    {
                                        var existsCauHinh =
                                            listCauHinhTroCapKhac.FirstOrDefault(x => x.LoaiTroCapId == loaiTroCap.CategoryId);

                                        //Nếu loại trợ cấp này không có trong cấu hình
                                        if (existsCauHinh == null)
                                        {
                                            //Update bảng 2
                                            var luongCtLoaiTroCapKhac = context.LuongCtLoaiTroCapKhac.FirstOrDefault(x =>
                                                x.LuongCtTroCapKhacId == empBangLuong.LuongCtTroCapKhacId &&
                                                x.KyLuongId == parameter.KyLuongId &&
                                                x.LoaiTroCapId == loaiTroCap.CategoryId);

                                            luongCtLoaiTroCapKhac.MucTroCap = _item.MucTroCap;

                                            listUpdate.Add(luongCtLoaiTroCapKhac);
                                        }
                                    }
                                });
                            }
                        }
                    });

                    bool isError = true;
                    //Nếu không có lỗi
                    if (messEmpHeThong.Count == 0 && messEmpBangLuong.Count == 0 && messLoaiTroCap.Count == 0)
                    {
                        context.LuongCtLoaiTroCapKhac.UpdateRange(listUpdate);
                        context.SaveChanges();

                        trans.Commit();

                        isError = false;
                    }

                    return new ImportTroCapKhacResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = "Import thành công",
                        IsError = isError,
                        MessEmpHeThong = messEmpHeThong.Join(),
                        MessEmpBangLuong = messEmpBangLuong.Join(),
                        MessLoaiTroCap = messLoaiTroCap.Join()
                };
                }
            }
            catch (Exception e)
            {
                return new ImportTroCapKhacResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public ImportLuongChiTietResult ImportLuongChiTiet(ImportLuongChiTietParameter parameter)
        {
            try
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    var messEmpHeThong = new List<string>();
                    var messEmpBangLuong = new List<string>();
                    bool isError = true;

                    if (parameter.ListData.Count == 0)
                    {
                        return new ImportLuongChiTietResult()
                        {
                            StatusCode = HttpStatusCode.NotFound,
                            MessageCode = "File import không có dữ liệu"
                        };
                    }

                    var kyLuong = context.KyLuong.FirstOrDefault(x => x.KyLuongId == parameter.ListData.First().KyLuongId);
                    if (kyLuong == null)
                    {
                        return new ImportLuongChiTietResult()
                        {
                            StatusCode = HttpStatusCode.NotFound,
                            MessageCode = "Kỳ lương không tồn tại trên hệ thống"
                        };
                    }

                    var listEmp = context.Employee.ToList();
                    var listLuongCtThuNhapTinhThue = context.LuongCtThuNhapTinhThue
                        .Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                    var listLuongCtBaoHiem =
                        context.LuongCtBaoHiem.Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                    var listLuongCtGiamTruTruocThue = context.LuongCtGiamTruTruocThue
                        .Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                    var listLuongCtGiamTruSauThue = context.LuongCtGiamTruSauThue
                        .Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                    var listLuongCtHoanLaiSauThue = context.LuongCtHoanLaiSauThue
                        .Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                    var listLuongCtCtyDong =
                        context.LuongCtCtyDong.Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                    var listLuongCtOther = context.LuongCtOther.Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();

                    parameter.ListData.ForEach(item =>
                    {
                        var emp = listEmp.FirstOrDefault(x =>
                            x.EmployeeCode.ToLower().Trim() == item.EmployeeCode?.ToLower()?.Trim());

                        if (emp == null)
                        {
                            messEmpHeThong.Add(item.EmployeeCode);
                        }
                        else
                        {
                            var emp1 = listLuongCtThuNhapTinhThue.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId);
                            if (emp1 == null)
                            {
                                messEmpBangLuong.Add(item.EmployeeCode);
                            }
                            else
                            {
                                emp1.NetToGross = item.ThuNhapTinhThueNetToGross;
                                emp1.Month13 = item.ThuNhapTinhThueMonth13;
                                emp1.Gift = item.ThuNhapTinhThueGift;
                                emp1.Other = item.ThuNhapTinhThueOther;

                                context.LuongCtThuNhapTinhThue.Update(emp1);
                            }

                            var emp2 = listLuongCtBaoHiem.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId);
                            if (emp2 == null)
                            {
                                messEmpBangLuong.Add(item.EmployeeCode);
                            }
                            else
                            {
                                emp2.Other = item.BaoHiemOther;

                                context.LuongCtBaoHiem.Update(emp2);
                            }

                            var emp3 = listLuongCtGiamTruTruocThue.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId);
                            if (emp3 == null)
                            {
                                messEmpBangLuong.Add(item.EmployeeCode);
                            }
                            else
                            {
                                emp3.GiamTruKhac = item.GiamTruTruocThueGiamTruKhac;

                                context.LuongCtGiamTruTruocThue.Update(emp3);
                            }

                            var emp4 = listLuongCtGiamTruSauThue.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId);
                            if (emp4 == null)
                            {
                                messEmpBangLuong.Add(item.EmployeeCode);
                            }
                            else
                            {
                                emp4.QuyetToanThueTncn = item.GiamTruSauThueQuyetToanThueTncn;
                                emp4.Other = item.GiamTruSauThueOther;

                                context.LuongCtGiamTruSauThue.Update(emp4);
                            }

                            var emp5 = listLuongCtHoanLaiSauThue.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId);
                            if (emp5 == null)
                            {
                                messEmpBangLuong.Add(item.EmployeeCode);
                            }
                            else
                            {
                                emp5.ThueTncn = item.HoanLaiSauThueThueTncn;
                                emp5.Other = item.HoanLaiSauThueOther;

                                context.LuongCtHoanLaiSauThue.Update(emp5);
                            }

                            var emp6 = listLuongCtCtyDong.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId);
                            if (emp6 == null)
                            {
                                messEmpBangLuong.Add(item.EmployeeCode);
                            }
                            else
                            {
                                emp6.Other = item.CtyDongOther;
                                emp6.FundOct = item.CtyDongFundOct;

                                context.LuongCtCtyDong.Update(emp6);
                            }

                            var emp7 = listLuongCtOther.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId);
                            if (emp7 == null)
                            {
                                messEmpBangLuong.Add(item.EmployeeCode);
                            }
                            else
                            {
                                emp7.KhoanBuTruThangTruoc = item.OtherKhoanBuTruThangTruoc;
                                emp7.TroCapKhacKhongTinhThue = item.OtherTroCapKhacKhongTinhThue;
                                emp7.KhauTruHoanLaiTruocThue = item.OtherKhauTruHoanLaiTruocThue;
                                emp7.LuongTamUng = item.OtherLuongTamUng;

                                context.LuongCtOther.Update(emp7);
                            }
                        }
                    });

                    messEmpBangLuong = messEmpBangLuong.Select(y => y).Distinct().ToList();
                    //Nếu không có lỗi
                    if (messEmpHeThong.Count == 0 && messEmpBangLuong.Count == 0)
                    {
                        context.SaveChanges();
                        trans.Commit();

                        isError = false;
                    }

                    return new ImportLuongChiTietResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = "Import thành công",
                        IsError = isError,
                        MessEmpHeThong = messEmpHeThong.Join(),
                        MessEmpBangLuong = messEmpBangLuong.Join(),
                    };
                }
            }
            catch (Exception e)
            {
                return new ImportLuongChiTietResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public HoanThanhKyLuongResult HoanThanhKyLuong(HoanThanhKyLuongParameter parameter)
        {
            var listPhieuLuong = new List<PhieuLuongModel>();

            try
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId);
                    if (user == null)
                    {
                        return new HoanThanhKyLuongResult()
                        {
                            StatusCode = HttpStatusCode.NotFound,
                            MessageCode = "Người dùng không tồn tại trên hệ thống"
                        };
                    }

                    var empUser = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId);
                    if (empUser == null)
                    {
                        return new HoanThanhKyLuongResult()
                        {
                            StatusCode = HttpStatusCode.NotFound,
                            MessageCode = "Người dùng không tồn tại trên hệ thống"
                        };
                    }

                    var kyLuong = context.KyLuong.FirstOrDefault(x => x.KyLuongId == parameter.KyLuongId);
                    if (kyLuong == null)
                    {
                        return new HoanThanhKyLuongResult()
                        {
                            StatusCode = HttpStatusCode.NotFound,
                            MessageCode = "Kỳ lương không tồn tại trên hệ thống"
                        };
                    }

                    //Nếu kỳ lương có trạng thái khác Đã phê duyệt
                    if (kyLuong.TrangThai != 3)
                    {
                        return new HoanThanhKyLuongResult()
                        {
                            StatusCode = HttpStatusCode.NotFound,
                            MessageCode = "Kỳ lương có trạng thái khác Đã phê duyệt"
                        };
                    }

                    //Chuyển trạng thái => Hoàn thành
                    kyLuong.TrangThai = 5;
                    context.KyLuong.Update(kyLuong);

                    var listLuongTongHop = context.LuongTongHop.Where(x => x.KyLuongId == parameter.KyLuongId).ToList();
                    var listEmpId = listLuongTongHop.Select(y => y.EmployeeId).ToList();
                    var listEmp = context.Employee.Where(x => listEmpId.Contains(x.EmployeeId)).ToList();
                    var listContact = context.Contact.Where(x => x.ObjectType == ContactObjectType.EMP &&
                                                                 listEmpId.Contains(x.ObjectId)).ToList();
                    var listGetSubCode1 = GeneralList.GetSubCode1();
                    var listTongHopChamCong = context.TongHopChamCong.Where(x => x.KyLuongId == parameter.KyLuongId)
                        .Select(y => new TongHopChamCongModel(y)).ToList();
                    var listLuongCtGiamTruTruocThue = context.LuongCtGiamTruTruocThue
                        .Where(x => x.KyLuongId == parameter.KyLuongId).ToList();

                    var troCapCoDinhType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LTCCD");
                    var listTroCapCoDinh = context.Category.Where(x => x.CategoryTypeId == troCapCoDinhType.CategoryTypeId)
                        .ToList();
                    var troCapDiLai = listTroCapCoDinh.FirstOrDefault(x => x.CategoryCode == "TCDL");
                    var troCapDienThoai = listTroCapCoDinh.FirstOrDefault(x => x.CategoryCode == "TCDT");
                    var troCapAnTrua = listTroCapCoDinh.FirstOrDefault(x => x.CategoryCode == "TCAT");
                    var troCapCcnc = listTroCapCoDinh.FirstOrDefault(x => x.CategoryCode == "TCCCNC");
                    var troCapDmvs = listTroCapCoDinh.FirstOrDefault(x => x.CategoryCode == "TCCCDMVS");

                    var listTroCap = context.TroCap.ToList();
                    var troCapCcncCauHinh = listTroCap.FirstOrDefault(x => x.LoaiTroCapId == troCapCcnc.CategoryId);
                    var troCapDmvsCauHinh = listTroCap.FirstOrDefault(x => x.LoaiTroCapId == troCapDmvs.CategoryId);

                    var listLuongCtTroCapCoDinh = context.LuongCtTroCapCoDinh.Where(x => x.KyLuongId == parameter.KyLuongId).ToList();
                    var listLuongCtLoaiTroCapCoDinh = context.LuongCtLoaiTroCapCoDinh.Where(x => x.KyLuongId == parameter.KyLuongId).ToList();
                    var listLuongCtDieuKienTroCapCoDinh = context.LuongCtDieuKienTroCapCoDinh
                        .Where(x => x.KyLuongId == parameter.KyLuongId).ToList();

                    var dieuKienHuongTroCapType =
                        context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "DKHTC");
                    var listDieuKienHuongTroCap =
                        context.Category.Where(x => x.CategoryTypeId == dieuKienHuongTroCapType.CategoryTypeId).ToList();
                    var dieuKienHuongDatKpi = listDieuKienHuongTroCap.FirstOrDefault(x => x.CategoryCode == "DKH002");
                    var dieuKienHuongDuocHuongTccc =
                        listDieuKienHuongTroCap.FirstOrDefault(x => x.CategoryCode == "DKH004");

                    var troCapKhacType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LTCK");
                    var listTroCapKhac = context.Category.Where(x => x.CategoryTypeId == troCapKhacType.CategoryTypeId)
                        .ToList();
                    var troCapThuongKpi = listTroCapKhac.FirstOrDefault(x => x.CategoryCode == "TKPITT");
                    var troCapTrachNhiem = listTroCapKhac.FirstOrDefault(x => x.CategoryCode == "TCTN1");

                    var listLuongCtTroCapKhac = context.LuongCtTroCapKhac.Where(x => x.KyLuongId == parameter.KyLuongId).ToList();
                    var listLuongCtLoaiTroCapKhac = context.LuongCtLoaiTroCapKhac.Where(x => x.KyLuongId == parameter.KyLuongId).ToList();

                    var listLuongCtThuNhapTinhThue = context.LuongCtThuNhapTinhThue.Where(x => x.KyLuongId == parameter.KyLuongId).ToList();
                    var listLuongCtCtyDong = context.LuongCtCtyDong.Where(x => x.KyLuongId == parameter.KyLuongId).ToList();

                    var cauHinhGiamTruBanThan = context.CauHinhGiamTru.FirstOrDefault(x => x.LoaiGiamTruId == 1);
                    var cauHinhGiamTruNguoiPhuThuoc = context.CauHinhGiamTru.FirstOrDefault(x => x.LoaiGiamTruId == 2);

                    var cauHinhKinhPhiCongDoan = context.KinhPhiCongDoan.FirstOrDefault();

                    var cauHinhBaoHiem = context.CauHinhBaoHiem.FirstOrDefault();

                    if (listLuongTongHop.Count == 0)
                    {
                        return new HoanThanhKyLuongResult()
                        {
                            StatusCode = HttpStatusCode.NotFound,
                            MessageCode = "Kỳ lương này không có nhân viên"
                        };
                    }
                    
                    listLuongTongHop.ForEach(item =>
                    {
                        var emp = listEmp.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                        var contact = listContact.FirstOrDefault(x => x.ObjectId == item.EmployeeId);
                        var tongHopChamCong = listTongHopChamCong.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                        var luongCtGiamTruTruocThue =
                            listLuongCtGiamTruTruocThue.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                        var luongCtTroCapCoDinh =
                            listLuongCtTroCapCoDinh.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);

                        var luongCtTroCapDiLai = listLuongCtLoaiTroCapCoDinh.FirstOrDefault(x =>
                            x.LuongCtTroCapCoDinhId == luongCtTroCapCoDinh.LuongCtTroCapCoDinhId &&
                            x.LoaiTroCapId == troCapDiLai.CategoryId);
                        var listLuongCtDieuKienTroCapDiLai = listLuongCtDieuKienTroCapCoDinh.Where(x =>
                            x.LuongCtLoaiTroCapCoDinhId == luongCtTroCapDiLai.LuongCtLoaiTroCapCoDinhId).ToList();

                        var luongCtTroCapDienThoai = listLuongCtLoaiTroCapCoDinh.FirstOrDefault(x =>
                            x.LuongCtTroCapCoDinhId == luongCtTroCapCoDinh.LuongCtTroCapCoDinhId &&
                            x.LoaiTroCapId == troCapDienThoai.CategoryId);
                        var listLuongCtDieuKienTroCapDienThoai = listLuongCtDieuKienTroCapCoDinh.Where(x =>
                            x.LuongCtLoaiTroCapCoDinhId == luongCtTroCapDienThoai.LuongCtLoaiTroCapCoDinhId).ToList();

                        var luongCtTroCapAnTrua = listLuongCtLoaiTroCapCoDinh.FirstOrDefault(x =>
                            x.LuongCtTroCapCoDinhId == luongCtTroCapCoDinh.LuongCtTroCapCoDinhId &&
                            x.LoaiTroCapId == troCapAnTrua.CategoryId);
                        var listLuongCtDieuKienTroCapAnTrua = listLuongCtDieuKienTroCapCoDinh.Where(x =>
                            x.LuongCtLoaiTroCapCoDinhId == luongCtTroCapAnTrua.LuongCtLoaiTroCapCoDinhId).ToList();

                        var luongCtTroCapCcnc = listLuongCtLoaiTroCapCoDinh.FirstOrDefault(x =>
                            x.LuongCtTroCapCoDinhId == luongCtTroCapCoDinh.LuongCtTroCapCoDinhId &&
                            x.LoaiTroCapId == troCapCcnc.CategoryId);
                        var listLuongCtDieuKienTroCapCcnc = listLuongCtDieuKienTroCapCoDinh.Where(x =>
                            x.LuongCtLoaiTroCapCoDinhId == luongCtTroCapCcnc.LuongCtLoaiTroCapCoDinhId).ToList();

                        var luongCtTroCapDmvs = listLuongCtLoaiTroCapCoDinh.FirstOrDefault(x =>
                            x.LuongCtTroCapCoDinhId == luongCtTroCapCoDinh.LuongCtTroCapCoDinhId &&
                            x.LoaiTroCapId == troCapDmvs.CategoryId);
                        var listLuongCtDieuKienTroCapDmvs = listLuongCtDieuKienTroCapCoDinh.Where(x =>
                            x.LuongCtLoaiTroCapCoDinhId == luongCtTroCapDmvs.LuongCtLoaiTroCapCoDinhId).ToList();

                        var luongCtTroCapKhac =
                            listLuongCtTroCapKhac.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);

                        var luongCtTroCapKpi = listLuongCtLoaiTroCapKhac.FirstOrDefault(x =>
                            x.LuongCtTroCapKhacId == luongCtTroCapKhac.LuongCtTroCapKhacId &&
                            x.LoaiTroCapId == troCapThuongKpi.CategoryId);

                        var luongCtTroCapTrachNhiem = listLuongCtLoaiTroCapKhac.FirstOrDefault(x =>
                            x.LuongCtTroCapKhacId == luongCtTroCapKhac.LuongCtTroCapKhacId &&
                            x.LoaiTroCapId == troCapTrachNhiem.CategoryId);

                        var luongCtThuNhapTinhThue =
                            listLuongCtThuNhapTinhThue.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);

                        var luongCtCtyDong = listLuongCtCtyDong.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);

                        var phieuLuong = new PhieuLuongModel();
                        phieuLuong.PhieuLuongCode = GenCodePhieuLuong();
                        phieuLuong.KyLuongId = parameter.KyLuongId;
                        phieuLuong.TenKyLuong = kyLuong.TenKyLuong;
                        phieuLuong.EmployeeId = item.EmployeeId;
                        phieuLuong.EmployeeCode = emp.EmployeeCode;
                        phieuLuong.EmployeeName = emp.EmployeeName;
                        phieuLuong.SubCode1 = listGetSubCode1.FirstOrDefault(x => x.Value == emp.SubCode1Value)?.Name;
                        phieuLuong.OrganizationId = item.OrganizationId;
                        phieuLuong.PositionId = item.PositionId;
                        phieuLuong.CreatedByEmpId = user.EmployeeId;

                        phieuLuong.WorkEmail = contact?.WorkEmail;
                        phieuLuong.SoNgayLamViec = kyLuong.SoNgayLamViec;
                        phieuLuong.NgayBatDauKyLuong = kyLuong.TuNgay.Day.ToString("D2");
                        phieuLuong.ThangBatDauKyLuong = kyLuong.TuNgay.Month.ToString("D2");
                        phieuLuong.NamBatDauKyLuong = kyLuong.TuNgay.Year.ToString();
                        phieuLuong.NgayKetThucKyLuong = kyLuong.DenNgay.Day.ToString("D2");
                        phieuLuong.ThangKetThucKyLuong = kyLuong.DenNgay.Month.ToString("D2");
                        phieuLuong.NamKetThucKyLuong = kyLuong.DenNgay.Year.ToString();

                        var thangTruoc = kyLuong.TuNgay.AddDays(-30);
                        phieuLuong.ThangTruoc = thangTruoc.Month.ToString("D2");
                        phieuLuong.ThangTruocTiengAnh = ConvertThangSangTiengAnh(thangTruoc.Month, false);
                        phieuLuong.NamTheoThangTruoc = thangTruoc.Year.ToString();

                        phieuLuong.CauHinhTroCapCc = troCapCcncCauHinh?.MucTroCap ?? 0;
                        phieuLuong.CauHinhTroCapDmvs = troCapDmvsCauHinh?.MucTroCap ?? 0;

                        phieuLuong.CauHinhGiamTruCaNhan = cauHinhGiamTruBanThan?.MucGiamTru ?? 0;
                        phieuLuong.CauHinhGiamTruNguoiPhuThuoc = cauHinhGiamTruNguoiPhuThuoc?.MucGiamTru ?? 0;
                        phieuLuong.PhanTramKinhPhiCongDoanCty = cauHinhKinhPhiCongDoan?.MucDongCongTy ?? 0;
                        phieuLuong.PhanTramBaoHiemNld = (cauHinhBaoHiem?.TiLePhanBoMucDongBhxhcuaNld +
                                                         cauHinhBaoHiem?.TiLePhanBoMucDongBhytcuaNld +
                                                         cauHinhBaoHiem?.TiLePhanBoMucDongBhtncuaNld +
                                                         cauHinhBaoHiem?.TiLePhanBoMucDongBhtnnncuaNld) ?? 0;

                        phieuLuong.PhanTramBaoHiemCty = (cauHinhBaoHiem?.TiLePhanBoMucDongBhxhcuaNsdld +
                                                         cauHinhBaoHiem?.TiLePhanBoMucDongBhytcuaNsdld +
                                                         cauHinhBaoHiem?.TiLePhanBoMucDongBhtncuaNsdld +
                                                         cauHinhBaoHiem?.TiLePhanBoMucDongBhtnnncuaNsdld) ?? 0;

                        phieuLuong.LuongCoBan = item.MucLuongCu;
                        phieuLuong.LuongCoBanSau = item.MucLuongHienTai;
                        phieuLuong.MucDieuChinh = phieuLuong.LuongCoBanSau - phieuLuong.LuongCoBan;
                        phieuLuong.NgayLamViecThucTe = tongHopChamCong.NgayLamViecThucTe;
                        phieuLuong.NgayNghiPhep = tongHopChamCong.NghiPhep;
                        phieuLuong.NgayNghiLe = tongHopChamCong.TongSoNgayTinhLuong - tongHopChamCong.NghiPhep;
                        phieuLuong.NgayNghiKhongLuong = tongHopChamCong.NghiKhongLuong;
                        phieuLuong.NgayDmvs = tongHopChamCong.TongNgayDmvs;
                        phieuLuong.NgayKhongHuongChuyenCan = tongHopChamCong.TongNgayNghiTinhTroCapChuyenCan;

                        var listDieuKienTroCapCoDinh = new List<LuongCtDieuKienTroCapCoDinh>();
                        listDieuKienTroCapCoDinh.AddRange(listLuongCtDieuKienTroCapDiLai);
                        listDieuKienTroCapCoDinh.AddRange(listLuongCtDieuKienTroCapDienThoai);
                        listDieuKienTroCapCoDinh.AddRange(listLuongCtDieuKienTroCapAnTrua);
                        listDieuKienTroCapCoDinh.AddRange(listLuongCtDieuKienTroCapCcnc);
                        listDieuKienTroCapCoDinh.AddRange(listLuongCtDieuKienTroCapDmvs);

                        phieuLuong.DuocHuongTroCapKpi = CheckDuocHuongTroCap(dieuKienHuongDatKpi.CategoryId, listDieuKienTroCapCoDinh);
                        phieuLuong.DuocHuongTroCapChuyenCan = CheckDuocHuongTroCap(dieuKienHuongDuocHuongTccc.CategoryId, listDieuKienTroCapCoDinh);

                        phieuLuong.SoLuongDkGiamTruGiaCanh = luongCtGiamTruTruocThue.SoNguoiPhuThuoc;
                        phieuLuong.GiamTruGiaCanh = luongCtGiamTruTruocThue.GiamTruCaNhan + luongCtGiamTruTruocThue.GiamTruNguoiPhuThuoc;
                        phieuLuong.LuongTheoNgayHocViec = tongHopChamCong.TongSoNgayTinhLuong;
                        phieuLuong.TroCapDiLai = luongCtTroCapDiLai?.MucTroCap ?? 0;
                        phieuLuong.TroCapDienThoai = luongCtTroCapDienThoai?.MucTroCap ?? 0;
                        phieuLuong.TroCapAnTrua = luongCtTroCapAnTrua?.MucTroCap ?? 0;
                        phieuLuong.TroCapNhaO = luongCtTroCapCcnc?.MucTroCap ?? 0;
                        phieuLuong.TroCapChuyenCan = luongCtTroCapDmvs?.MucTroCap ?? 0;
                        phieuLuong.ThuongKpi = luongCtTroCapKpi?.MucTroCap ?? 0;
                        phieuLuong.ThuongCuoiNam = 0;
                        phieuLuong.TroCapTrachNhiem = luongCtTroCapTrachNhiem?.MucTroCap ?? 0;
                        phieuLuong.TroCapHocViec = 0;
                        phieuLuong.OtTinhThue = item.LuongOtTinhThue;
                        phieuLuong.OtKhongTinhThue = item.LuongOtKhongTinhThue;
                        phieuLuong.LuongThang13 = luongCtThuNhapTinhThue?.Month13 ?? 0;
                        phieuLuong.QuaBocTham = luongCtThuNhapTinhThue?.Gift ?? 0;
                        phieuLuong.TongThueTncn = item.ThueTncnNld;
                        phieuLuong.BaoHiem = item.TongTienBhNldPhaiDong;
                        phieuLuong.ThucNhan = item.ThuNhapThucNhan;
                        phieuLuong.CtyTraBh = item.TongTienBhCtyPhaiDong;
                        phieuLuong.KinhPhiCongDoan = luongCtCtyDong.KinhPhiCongDoan;
                        phieuLuong.TongChiPhiNhanVien = item.TongChiPhiCtyPhaiTra;

                        //Tự ý nghỉ không hưởng lương: tongHopChamCong.NghiKhongPhep;
                        //Tổng số ngày không tính lương: tongHopChamCong.TongSoNgayKhongTinhLuong
                        //Ngày làm việc thực tế: phieuLuong.NgayLamViecThucTe
                        //Trợ cấp chuyên cần ngày công theo ngày làm việc thực tế: troCapCcncCauHinh.MucTroCap / kyLuong.SoNgayLamViec * tongHopChamCong.NgayLamViecThucTe
                        //Trợ cấp chuyên cần ngày công được hưởng: phieuLuong.TroCapNhaO
                        //Tổng số lần đi muộn về sớm: tongHopChamCong.SoLanTruChuyenCan
                        //Tổng ngày làm việc trong tháng: phieuLuong.SoNgayLamViec
                        //Trợ cấp chuyên cần đi muộn về sớm: troCapDmvsCauHinh.MucTroCap
                        //Trợ cấp chuyên cần ngày công được hưởng: phieuLuong.TroCapChuyenCan

                        phieuLuong.NghiKhongPhep = tongHopChamCong.NghiKhongPhep;
                        phieuLuong.TongSoNgayKhongTinhLuong = tongHopChamCong.TongSoNgayKhongTinhLuong;
                        phieuLuong.TroCapCcncTheoNgayLvtt =
                            troCapCcncCauHinh.MucTroCap / kyLuong.SoNgayLamViec * tongHopChamCong.NgayLamViecThucTe;
                        phieuLuong.SoLanTruChuyenCan = tongHopChamCong.SoLanTruChuyenCan;
                        phieuLuong.TroCapChuyenCanDmvs = troCapDmvsCauHinh.MucTroCap;

                        listPhieuLuong.Add(phieuLuong);

                        context.PhieuLuong.Add(phieuLuong.ToEntity());
                        context.SaveChanges();
                    });

                    #region Thêm ghi chú vào dòng thời gian

                    Note note = new Note();
                    note.NoteId = Guid.NewGuid();
                    note.ObjectId = Guid.Empty;
                    note.Description = "Đã chuyển đề xuất về hoàn thành";
                    note.Type = "ADD";
                    note.Active = true;
                    note.CreatedById = parameter.UserId;
                    note.CreatedDate = DateTime.Now;
                    note.NoteTitle = "Đã thêm ghi chú";
                    note.ObjectNumber = kyLuong.KyLuongId;
                    note.ObjectType = NoteObjectType.KYLUONG;

                    context.Note.Add(note);
                    context.SaveChanges();

                    #endregion

                    trans.Commit();
                }
            }
            catch (Exception e)
            {
                return new HoanThanhKyLuongResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }

            /* Gửi email phiếu lương đến nhân viên */
            listPhieuLuong.ForEach(item =>
            {
                NotificationHelper.AccessNotification(context, TypeModel.PhieuLuong, "SEND_EMP", item, item,
                    true, null, null, new List<string>());
            });

            return new HoanThanhKyLuongResult()
            {
                StatusCode = HttpStatusCode.OK,
                MessageCode = "Lưu thành công"
            };
        }

        public GetListPhieuLuongResult GetListPhieuLuong(GetListPhieuLuongParameter parameter)
        {
            try
            {
                #region Check permision: manager

                var user = context.User.FirstOrDefault(x => x.UserId == parameter.UserId && x.Active == true);
                if (user == null)
                {
                    return new GetListPhieuLuongResult
                    {
                        Status = false,
                        Message = "User không có quyền truy xuất dữ liệu trong hệ thống"
                    };
                }

                var employeeLogin = context.Employee.FirstOrDefault(x => x.EmployeeId == user.EmployeeId && x.Active == true);
                if (employeeLogin == null)
                {
                    return new GetListPhieuLuongResult
                    {
                        Status = false,
                        Message = "User không có quyền truy xuất dữ liệu trong hệ thống"
                    };
                }
                #endregion

                var isAccess = context.Organization.FirstOrDefault(x => x.OrganizationId == employeeLogin.OrganizationId)?.IsAccess;

                var listKyLuong = context.KyLuong.Select(y => new KyLuongModel(y)).ToList();
                var listEmployee = context.Employee.Select(y => new EmployeeEntityModel
                {
                    EmployeeId = y.EmployeeId,
                    EmployeeCode = y.EmployeeCode,
                    EmployeeName = y.EmployeeName,
                    EmployeeCodeName = y.EmployeeCode + " - " + y.EmployeeName,
                    SubCode1Value = y.SubCode1Value
                }).ToList();
                var listPosition = context.Position.ToList();
                var listGetSubCode1 = GeneralList.GetSubCode1();

                var listPhieuLuong = context.PhieuLuong.Where(x =>
                        (parameter.ListKyLuongId.Count == 0 || parameter.ListKyLuongId.Contains(x.KyLuongId)) &&
                        (parameter.ListEmployeeId.Count == 0 || parameter.ListEmployeeId.Contains(x.EmployeeId)))
                    .Select(y => new PhieuLuongModel(y)).ToList();

                listPhieuLuong.ForEach(item =>
                {
                    var kyLuong = listKyLuong.FirstOrDefault(x => x.KyLuongId == item.KyLuongId);
                    item.TenKyLuong = kyLuong?.TenKyLuong;

                    var emp = listEmployee.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                    item.EmployeeCode = emp?.EmployeeCode;
                    item.EmployeeName = emp?.EmployeeName;

                    var position = listPosition.FirstOrDefault(x => x.PositionId == item.PositionId);
                    item.PositionName = position?.PositionName;

                    item.SubCode1 = listGetSubCode1.FirstOrDefault(x => x.Value == emp.SubCode1Value)?.Name;
                });

                //Không có quyền xem dự liệu phòng ban khác => xem mỗi phiếu lương của người đăng nhập
                if(isAccess == true)
                {
                    listPhieuLuong = listPhieuLuong.OrderByDescending(z => z.KyLuongId).ThenBy(z => z.EmployeeCode)
                                    .ToList();
                }
                //Nếu có quyền xem data phòng ban khác => xem all
                else
                {
                    listPhieuLuong = listPhieuLuong.Where(x => x.EmployeeId == employeeLogin.EmployeeId).OrderByDescending(z => z.KyLuongId).ThenBy(z => z.EmployeeCode)
                      .ToList();
                }

                return new GetListPhieuLuongResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "OK",
                    ListKyLuong = listKyLuong,
                    ListEmployee = listEmployee,
                    ListPhieuLuong = listPhieuLuong
                };
            }
            catch (Exception e)
            {
                return new GetListPhieuLuongResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetPhieuLuongByIdResult GetPhieuLuongById(GetPhieuLuongByIdParameter parameter)
        {
            try
            {
                var phieuLuong = context.PhieuLuong.Where(x => x.PhieuLuongId == parameter.PhieuLuongId)
                    .Select(y => new PhieuLuongModel(y)).FirstOrDefault();

                var emp = context.Employee.FirstOrDefault(x => x.EmployeeId == phieuLuong.EmployeeId);
                var empContact = context.Contact.FirstOrDefault(x =>
                    x.ObjectId == emp.EmployeeId && x.ObjectType == ContactObjectType.EMP);

                phieuLuong.EmployeeName = emp.EmployeeName;
                phieuLuong.EmployeeCode = emp.EmployeeCode;
                phieuLuong.WorkEmail = empContact.WorkEmail;

                return new GetPhieuLuongByIdResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "OK",
                    PhieuLuong = phieuLuong
                };
            }
            catch (Exception e)
            {
                return new GetPhieuLuongByIdResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public ExportBaoCaoKyLuongResult ExportBaoCaoKyLuong(ExportBaoCaoKyLuongParameter parameter)
        {
            try
            {
                var kyLuong = context.KyLuong.FirstOrDefault(x => x.KyLuongId == parameter.KyLuongId);
                if (kyLuong == null)
                {
                    return new ExportBaoCaoKyLuongResult()
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        MessageCode = "Kỳ lương không tồn tại trên hệ thống"
                    };
                }

                var kyLuongTruoc = context.KyLuong.Where(x => x.TuNgay.Date < kyLuong.TuNgay.Date)
                    .OrderByDescending(z => z.TuNgay).FirstOrDefault();

                var listLuongTongHop = context.LuongTongHop.Where(x => x.KyLuongId == kyLuong.KyLuongId)
                    .Select(y => new LuongTongHopModel(y)).ToList();
                var listLuongCtCyDong = context.LuongCtCtyDong.Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                var listTongHopChamCong = context.TongHopChamCong.Where(x => x.KyLuongId == kyLuong.KyLuongId)
                    .Select(y => new TongHopChamCongModel(y)).ToList();
                var listLuongCtOther = context.LuongCtOther.Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                var listLuongCtCtyDong = context.LuongCtCtyDong.Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();
                var listLuongCtBaoHiem = context.LuongCtBaoHiem.Where(x => x.KyLuongId == kyLuong.KyLuongId).ToList();

                #region Báo cáo 1

                #region Data Header

                var thangKetThucKyLuongTruoc =
                    kyLuongTruoc == null ? "" : ConvertThangSangTiengAnh(kyLuongTruoc.DenNgay.Month, false);
                decimal tongSoNhanVienKyLuongTruoc = kyLuongTruoc == null ? 0 :
                    context.LuongTongHop.Count(x => x.KyLuongId == kyLuongTruoc.KyLuongId);
                var thangKetThucKyLuong = ConvertThangSangTiengAnh(kyLuong.DenNgay.Month, false);
                decimal tongSoNhanVienKyLuong = context.KyLuong.Count(x => x.KyLuongId == kyLuong.KyLuongId);

                var listHeaderBaoBao = new List<string>()
                {
                    "No.",
                    "Payment items",
                    thangKetThucKyLuongTruoc + ". CD actual (" + tongSoNhanVienKyLuongTruoc.ToString() + ")",
                    thangKetThucKyLuong + " Forecast",
                    thangKetThucKyLuong + ". CD actual (" + tongSoNhanVienKyLuong.ToString() + ")",
                    "Vs actual in " + thangKetThucKyLuongTruoc,
                    "Vs forecast",
                    "Note"
                };

                #endregion

                #region Data Báo cáo

                var listDataBaoCao = new List<ExportBaoCaoKyLuongModel>();

                var row1Bc1 = new ExportBaoCaoKyLuongModel();
                row1Bc1.Payment = "Salary";
                row1Bc1.CdActual1 = kyLuongTruoc == null
                    ? 0
                    : context.LuongTongHop.Where(x => x.KyLuongId == kyLuongTruoc.KyLuongId)
                        .Sum(s => s.ThuNhapThucNhan);
                row1Bc1.Forecast = 0;
                row1Bc1.CdActual2 = context.LuongTongHop.Where(x => x.KyLuongId == kyLuong.KyLuongId)
                    .Sum(s => s.ThuNhapThucNhan);
                row1Bc1.VsActual = row1Bc1.CdActual2 - row1Bc1.CdActual1;
                row1Bc1.VsForecast = row1Bc1.Forecast - row1Bc1.CdActual2;
                listDataBaoCao.Add(row1Bc1);

                var row2Bc1 = new ExportBaoCaoKyLuongModel();
                row2Bc1.Payment = "Trade Union";
                row2Bc1.CdActual1 = kyLuongTruoc == null
                    ? 0
                    : context.LuongCtCtyDong.Where(x => x.KyLuongId == kyLuongTruoc.KyLuongId)
                        .Sum(s => s.KinhPhiCongDoan + s.FundOct);
                row2Bc1.Forecast = 0;
                row2Bc1.CdActual2 = context.LuongCtCtyDong.Where(x => x.KyLuongId == kyLuong.KyLuongId)
                    .Sum(s => s.KinhPhiCongDoan + s.FundOct);
                row2Bc1.VsActual = row2Bc1.CdActual2 - row2Bc1.CdActual1;
                row2Bc1.VsForecast = row2Bc1.Forecast - row2Bc1.CdActual2;
                listDataBaoCao.Add(row2Bc1);

                var row3Bc1 = new ExportBaoCaoKyLuongModel();
                row3Bc1.Payment = "Obligation insurance (SI, HI, UI,AI)";
                row3Bc1.CdActual1 = kyLuongTruoc == null
                    ? 0
                    : context.LuongTongHop.Where(x => x.KyLuongId == kyLuongTruoc.KyLuongId)
                        .Sum(s => s.TongTienBhCtyPhaiDong + s.TongTienBhNldPhaiDong);
                row3Bc1.Forecast = 0;
                row3Bc1.CdActual2 = context.LuongTongHop.Where(x => x.KyLuongId == kyLuong.KyLuongId)
                    .Sum(s => s.TongTienBhCtyPhaiDong + s.TongTienBhNldPhaiDong);
                row3Bc1.VsActual = row3Bc1.CdActual2 - row3Bc1.CdActual1;
                row3Bc1.VsForecast = row3Bc1.Forecast - row3Bc1.CdActual2;
                listDataBaoCao.Add(row3Bc1);

                var row4Bc1 = new ExportBaoCaoKyLuongModel();
                row4Bc1.Payment = "PIT";
                row4Bc1.CdActual1 = kyLuongTruoc == null
                    ? 0
                    : context.LuongTongHop.Where(x => x.KyLuongId == kyLuongTruoc.KyLuongId)
                        .Sum(s => s.ThueTncnNld);
                row4Bc1.Forecast = 0;
                row4Bc1.CdActual2 = context.LuongTongHop.Where(x => x.KyLuongId == kyLuong.KyLuongId)
                    .Sum(s => s.ThueTncnNld);
                row4Bc1.VsActual = row4Bc1.CdActual2 - row4Bc1.CdActual1;
                row4Bc1.VsForecast = row4Bc1.Forecast - row4Bc1.CdActual2;
                listDataBaoCao.Add(row4Bc1);

                var row5Bc1 = new ExportBaoCaoKyLuongModel();
                row5Bc1.Payment = "";
                row5Bc1.CdActual1 = listDataBaoCao.Sum(s => s.CdActual1);
                row5Bc1.Forecast = 0;
                row5Bc1.CdActual2 = listDataBaoCao.Sum(s => s.CdActual2);
                row5Bc1.VsActual = listDataBaoCao.Sum(s => s.VsActual);
                row5Bc1.VsForecast = listDataBaoCao.Sum(s => s.VsForecast);
                listDataBaoCao.Add(row5Bc1);

                #endregion

                #endregion

                #region Báo cáo 2

                var thangNamBatDauKyLuongTiengAnh = ConvertThangSangTiengAnh(kyLuong.TuNgay.Month, true) + " " + kyLuong.TuNgay.Year;

                #region Bảng 1

                var listDataBaoCao2Bang1 = new List<ExportBaoCaoKyLuong2Bang1Model>();

                var listSubCode1 = GeneralList.GetSubCode1().OrderByDescending(z => z.Value).ToList();

                listSubCode1.ForEach(item =>
                {
                    var listEmpId = listLuongTongHop.Where(x => x.SubCode1Value == item.Value).Select(y => y.EmployeeId)
                        .ToList();

                    var newRow = new ExportBaoCaoKyLuong2Bang1Model();
                    newRow.Dept = item.Name.ToUpper();
                    newRow.NoOfStaff = listLuongTongHop.Count(x => x.SubCode1Value == item.Value);
                    newRow.BasicGrossSalary = listLuongTongHop.Where(x => x.SubCode1Value == item.Value)
                        .Sum(s => s.LuongThucTe);
                    newRow.Bonus = listLuongTongHop.Where(x => x.SubCode1Value == item.Value)
                        .Sum(s => s.LuongOtTinhThue + s.LuongOtKhongTinhThue + s.TongTroCapCoDinh + s.TongTroCapKhac);
                    newRow.TradeUnion = listLuongCtCyDong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.KinhPhiCongDoan);
                    newRow.PitTax = listLuongTongHop.Where(x => x.SubCode1Value == item.Value)
                        .Sum(s => s.TongThueTncnCtyVaNld);
                    newRow.Obligation = listLuongTongHop.Where(x => x.SubCode1Value == item.Value)
                        .Sum(s => s.TongTienBhCtyPhaiDong + s.TongTienBhNldPhaiDong);
                    newRow.NetPayable = listLuongTongHop.Where(x => x.SubCode1Value == item.Value)
                        .Sum(s => s.ThuNhapThucNhan);
                    newRow.TotalCompBefore = listLuongTongHop.Where(x => x.SubCode1Value == item.Value)
                        .Sum(s => s.TongChiPhiCtyPhaiTra);
                    newRow.TotalCompAfter = null;

                    listDataBaoCao2Bang1.Add(newRow);
                });

                #endregion

                #region Bảng 2

                var listDataBaoCao2Bang2 = new List<ExportBaoCaoKyLuong2Bang2Model>();

                var capBacType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "CAPBAC");
                var listCapBac = context.Category.Where(x => x.CategoryTypeId == capBacType.CategoryTypeId &&
                                                             (x.CategoryCode == "Junior" ||
                                                              x.CategoryCode == "Middle" ||
                                                              x.CategoryCode == "Senior")).OrderBy(z => z.CategoryCode).ToList();

                listCapBac.ForEach(item =>
                {
                    var listEmpId = listLuongTongHop.Where(x => x.CapBacId == item.CategoryId).Select(y => y.EmployeeId)
                        .ToList();

                    var newRow = new ExportBaoCaoKyLuong2Bang2Model();
                    newRow.Dept = item.CategoryName;
                    newRow.NoOfStaff = listLuongTongHop.Count(x => x.CapBacId == item.CategoryId);
                    newRow.GrossSalary = listLuongTongHop.Where(x => x.CapBacId == item.CategoryId)
                        .Sum(s => s.LuongThucTe);
                    newRow.Bonus = listLuongTongHop.Where(x => x.CapBacId == item.CategoryId)
                        .Sum(s => s.LuongOtTinhThue + s.LuongOtKhongTinhThue + s.TongTroCapCoDinh + s.TongTroCapKhac);
                    newRow.TradeUnion = listLuongCtCyDong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.KinhPhiCongDoan);
                    newRow.PitTax = listLuongTongHop.Where(x => x.CapBacId == item.CategoryId)
                        .Sum(s => s.TongThueTncnCtyVaNld);
                    newRow.Obligation = listLuongTongHop.Where(x => x.CapBacId == item.CategoryId)
                        .Sum(s => s.TongTienBhCtyPhaiDong + s.TongTienBhNldPhaiDong);
                    newRow.NetPayroll = listLuongTongHop.Where(x => x.CapBacId == item.CategoryId)
                        .Sum(s => s.ThuNhapThucNhan);
                    newRow.TotalComp = listLuongTongHop.Where(x => x.CapBacId == item.CategoryId)
                        .Sum(s => s.TongChiPhiCtyPhaiTra);

                    listDataBaoCao2Bang2.Add(newRow);
                });

                #endregion

                #region Bảng 3

                var listDataBaoCao2Bang3 = new List<ExportBaoCaoKyLuong2Bang3Model>();

                listCapBac.ForEach(item =>
                {
                    var listEmpId = listLuongTongHop.Where(x => x.CapBacId == item.CategoryId).Select(y => y.EmployeeId)
                        .ToList();

                    var newRow = new ExportBaoCaoKyLuong2Bang3Model();
                    newRow.Grade = item.CategoryName;
                    newRow.NumberOfEmployees = listLuongTongHop.Count(x => x.CapBacId == item.CategoryId);
                    newRow.GrossBase = listLuongTongHop.Where(x => x.CapBacId == item.CategoryId)
                        .Sum(s => s.LuongThucTe);
                    newRow.AverageGrossBase = 0; //Tính trên excel
                    newRow.TargetAverage = item.CategoryCode == "Junior" ? 8000000 : (item.CategoryCode == "Middle" ? 12500000 : 20000000);
                    newRow.Actual = 0; //Tính trên excel

                    listDataBaoCao2Bang3.Add(newRow);
                });

                #endregion

                #region Bảng 4

                var listDataBaoCao2Bang4 = new List<ExportBaoCaoKyLuong2Bang3Model>();

                listCapBac.ForEach(item =>
                {
                    var listEmpId = listLuongTongHop.Where(x => x.CapBacId == item.CategoryId).Select(y => y.EmployeeId)
                        .ToList();

                    var newRow = new ExportBaoCaoKyLuong2Bang3Model();
                    newRow.Grade = item.CategoryName;
                    newRow.NumberOfEmployees = listLuongTongHop.Count(x => x.CapBacId == item.CategoryId);
                    newRow.TargetAverage = item.CategoryCode == "Junior" ? 10500000 : (item.CategoryCode == "Middle" ? 15000000 : 22500000);

                    listDataBaoCao2Bang4.Add(newRow);
                });

                #endregion

                #region Bảng 5

                var listDataBaoCao2Bang5 = new List<ExportBaoCaoKyLuong2Bang3Model>();

                listCapBac.ForEach(item =>
                {
                    var listEmpId = listLuongTongHop.Where(x => x.CapBacId == item.CategoryId).Select(y => y.EmployeeId)
                        .ToList();

                    var newRow = new ExportBaoCaoKyLuong2Bang3Model();
                    newRow.Grade = item.CategoryName;
                    newRow.NumberOfEmployees = listLuongTongHop.Count(x => x.CapBacId == item.CategoryId);
                    newRow.TargetAverage = item.CategoryCode == "Junior" ? 10500000 : (item.CategoryCode == "Middle" ? 15000000 : 22500000);

                    listDataBaoCao2Bang5.Add(newRow);
                });

                #endregion

                #endregion

                #region Báo cáo 3

                #region Bảng 1

                var listDataBaoCao3Bang1 = new List<ExportBaoCaoKyLuong3Bang1Model>();

                int index = 0;
                listSubCode1.ForEach(item =>
                {
                    index++;
                    var listEmpId = listLuongTongHop.Where(x => x.SubCode1Value == item.Value).Select(y => y.EmployeeId)
                        .ToList();

                    var newRow = new ExportBaoCaoKyLuong3Bang1Model();
                    newRow.Col1 = index;
                    newRow.Col2 = item.Name.ToUpper();
                    newRow.Col3 = item.Name.ToUpper();
                    newRow.Col4 = listLuongTongHop.Count(x => x.SubCode1Value == item.Value);
                    newRow.Col5 = null;
                    newRow.Col6 = null;
                    newRow.Col7 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.NgayLamViecThucTe);
                    newRow.Col8 = listLuongTongHop.Where(x => x.SubCode1Value == item.Value)
                        .Sum(s => s.MucLuongCu);
                    newRow.Col9 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.NghiPhep);
                    newRow.Col10 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.NghiLe);
                    newRow.Col11 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.CongTac + s.DaoTaoHoiThao + s.NghiCheDo + s.NghiHuongLuongKhac + s.NghiBu);
                    newRow.Col12 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.NghiKhongLuong + s.NghiHuongBhxh);
                    newRow.Col13 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.NghiKhongPhep);
                    newRow.Col14 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.TongNgayDmvs);
                    newRow.Col15 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.SoLanTruChuyenCan);
                    newRow.Col16 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.TongSoNgayTinhLuong);
                    newRow.Col17 = listLuongCtOther.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.KhoanBuTruThangTruoc);
                    newRow.Col18 = listLuongTongHop.Where(x => x.SubCode1Value == item.Value)
                        .Sum(s => s.LuongThucTe);
                    newRow.Col19 = listLuongTongHop.Where(x => x.SubCode1Value == item.Value)
                        .Sum(s => s.LuongOtTinhThue + s.LuongOtKhongTinhThue);
                    newRow.Col20 = listLuongTongHop.Where(x => x.SubCode1Value == item.Value)
                        .Sum(s => s.TongTroCapCoDinh);
                    newRow.Col21 = listLuongTongHop.Where(x => x.SubCode1Value == item.Value)
                        .Sum(s => s.TongTroCapKhac);
                    newRow.Col22 = listLuongCtOther.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.TroCapKhacKhongTinhThue);
                    newRow.Col23 = listLuongCtOther.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.KhauTruHoanLaiTruocThue);
                    newRow.Col24 = listLuongTongHop.Where(x => x.SubCode1Value == item.Value)
                        .Sum(s => s.ThueTncnNld);
                    newRow.Col25 = listLuongTongHop.Where(x => x.SubCode1Value == item.Value)
                        .Sum(s => s.TongThueTncnCtyVaNld);
                    newRow.Col26 = listLuongCtCtyDong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.Bhtn);
                    newRow.Col27 = listLuongCtBaoHiem.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.Bhtn);
                    newRow.Col28 = listLuongCtCtyDong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.Bhxh + s.Bhtnnn);
                    newRow.Col29 = listLuongCtBaoHiem.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.Bhxh + s.Bhtnnn);
                    newRow.Col30 = listLuongCtCtyDong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.Bhyt);
                    newRow.Col31 = listLuongCtBaoHiem.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.Bhyt);
                    newRow.Col32 = listLuongCtBaoHiem.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.Other);
                    newRow.Col33 = null;
                    newRow.Col34 = null;
                    newRow.Col35 = null;
                    newRow.Col36 = listLuongCtCtyDong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.KinhPhiCongDoan);
                    newRow.Col37 = listLuongTongHop.Where(x => x.SubCode1Value == item.Value)
                        .Sum(s => s.CacKhoanGiamTruSauThue);
                    newRow.Col38 = listLuongTongHop.Where(x => x.SubCode1Value == item.Value)
                        .Sum(s => s.CacKhoanHoanLaiSauThue);
                    newRow.Col39 = listLuongTongHop.Where(x => x.SubCode1Value == item.Value)
                        .Sum(s => s.ThuNhapThucNhan);
                    newRow.Col40 = listLuongTongHop.Where(x => x.SubCode1Value == item.Value)
                        .Sum(s => s.LuongTamUng);
                    newRow.Col41 = listLuongTongHop.Where(x => x.SubCode1Value == item.Value)
                        .Sum(s => s.LuongConLai);
                    newRow.Col42 = listLuongTongHop.Where(x => x.SubCode1Value == item.Value)
                        .Sum(s => s.TongChiPhiCtyPhaiTra);
                    newRow.Col43 = listLuongTongHop.Where(x => x.SubCode1Value == item.Value)
                        .Sum(s => s.TongChiPhiCtyPhaiTra);
                    newRow.Col44 = null;
                    newRow.Col45 = null;

                    listDataBaoCao3Bang1.Add(newRow);
                });

                #endregion

                #region Bảng 2

                var listDataBaoCao3Bang2 = new List<ExportBaoCaoKyLuong3Bang1Model>();

                var listSubCode2 = GeneralList.GetSubCode2().OrderByDescending(z => z.Value).ToList();

                for (int i = 0; i < 9; i++)
                {
                    var newRow = new ExportBaoCaoKyLuong3Bang1Model();
                    newRow.Col1 = i + 1;

                    if (i >= 0 && i <= 4)
                    {
                        newRow.Col2 = "G&A";

                        if (i == 0)
                        {
                            newRow.Col3 = "CM";
                        }
                        else if (i == 1)
                        {
                            newRow.Col3 = "HRD";
                        }
                        else if (i == 2)
                        {
                            newRow.Col3 = "G&A-HR";
                        }
                        else if (i == 3)
                        {
                            newRow.Col3 = "G&A-ACC";
                        }
                        else if (i == 4)
                        {
                            newRow.Col3 = "G&A-AD";
                        }
                    }
                    else if (i >= 5 && i <= 6)
                    {
                        newRow.Col2 = "OPS";

                        if (i == 5)
                        {
                            newRow.Col3 = "OPS-PM";
                        }
                        else
                        {
                            newRow.Col3 = "OPS-IT";
                        }
                    }
                    else
                    {
                        newRow.Col2 = "COS";

                        if (i == 7)
                        {
                            newRow.Col3 = "COS-3D";
                        }
                        else
                        {
                            newRow.Col3 = "COS-QA";
                        }
                    }

                    var subCode2 = listSubCode2.FirstOrDefault(x => x.Name == newRow.Col3);
                    var listEmpId = listLuongTongHop.Where(x => x.SubCode2Value == subCode2.Value).Select(y => y.EmployeeId)
                        .ToList();

                    newRow.Col4 = listLuongTongHop.Count(x => x.SubCode2Value == subCode2.Value);
                    newRow.Col5 = null;
                    newRow.Col6 = null;
                    newRow.Col7 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.NgayLamViecThucTe);
                    newRow.Col8 = listLuongTongHop.Where(x => x.SubCode1Value == subCode2.Value)
                        .Sum(s => s.MucLuongCu);
                    newRow.Col9 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.NghiPhep);
                    newRow.Col10 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.NghiLe);
                    newRow.Col11 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.CongTac + s.DaoTaoHoiThao + s.NghiCheDo + s.NghiHuongLuongKhac + s.NghiBu);
                    newRow.Col12 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.NghiKhongLuong + s.NghiHuongBhxh);
                    newRow.Col13 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.NghiKhongPhep);
                    newRow.Col14 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.TongNgayDmvs);
                    newRow.Col15 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.SoLanTruChuyenCan);
                    newRow.Col16 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.TongSoNgayTinhLuong);
                    newRow.Col17 = listLuongCtOther.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.KhoanBuTruThangTruoc);
                    newRow.Col18 = listLuongTongHop.Where(x => x.SubCode1Value == subCode2.Value)
                        .Sum(s => s.LuongThucTe);
                    newRow.Col19 = listLuongTongHop.Where(x => x.SubCode1Value == subCode2.Value)
                        .Sum(s => s.LuongOtTinhThue + s.LuongOtKhongTinhThue);
                    newRow.Col20 = listLuongTongHop.Where(x => x.SubCode1Value == subCode2.Value)
                        .Sum(s => s.TongTroCapCoDinh);
                    newRow.Col21 = listLuongTongHop.Where(x => x.SubCode1Value == subCode2.Value)
                        .Sum(s => s.TongTroCapKhac);
                    newRow.Col22 = listLuongCtOther.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.TroCapKhacKhongTinhThue);
                    newRow.Col23 = listLuongCtOther.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.KhauTruHoanLaiTruocThue);
                    newRow.Col24 = listLuongTongHop.Where(x => x.SubCode1Value == subCode2.Value)
                        .Sum(s => s.ThueTncnNld);
                    newRow.Col25 = listLuongTongHop.Where(x => x.SubCode1Value == subCode2.Value)
                        .Sum(s => s.TongThueTncnCtyVaNld);
                    newRow.Col26 = listLuongCtCtyDong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.Bhtn);
                    newRow.Col27 = listLuongCtBaoHiem.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.Bhtn);
                    newRow.Col28 = listLuongCtCtyDong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.Bhxh + s.Bhtnnn);
                    newRow.Col29 = listLuongCtBaoHiem.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.Bhxh + s.Bhtnnn);
                    newRow.Col30 = listLuongCtCtyDong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.Bhyt);
                    newRow.Col31 = listLuongCtBaoHiem.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.Bhyt);
                    newRow.Col32 = listLuongCtBaoHiem.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.Other);
                    newRow.Col33 = null;
                    newRow.Col34 = null;
                    newRow.Col35 = null;
                    newRow.Col36 = listLuongCtCtyDong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.KinhPhiCongDoan);
                    newRow.Col37 = listLuongTongHop.Where(x => x.SubCode1Value == subCode2.Value)
                        .Sum(s => s.CacKhoanGiamTruSauThue);
                    newRow.Col38 = listLuongTongHop.Where(x => x.SubCode1Value == subCode2.Value)
                        .Sum(s => s.CacKhoanHoanLaiSauThue);
                    newRow.Col39 = listLuongTongHop.Where(x => x.SubCode1Value == subCode2.Value)
                        .Sum(s => s.ThuNhapThucNhan);
                    newRow.Col40 = listLuongTongHop.Where(x => x.SubCode1Value == subCode2.Value)
                        .Sum(s => s.LuongTamUng);
                    newRow.Col41 = listLuongTongHop.Where(x => x.SubCode1Value == subCode2.Value)
                        .Sum(s => s.LuongConLai);
                    newRow.Col42 = listLuongTongHop.Where(x => x.SubCode1Value == subCode2.Value)
                        .Sum(s => s.TongChiPhiCtyPhaiTra);
                    newRow.Col43 = listLuongTongHop.Where(x => x.SubCode1Value == subCode2.Value)
                        .Sum(s => s.TongChiPhiCtyPhaiTra);
                    newRow.Col44 = null;
                    newRow.Col45 = null;

                    listDataBaoCao3Bang2.Add(newRow);
                }

                #endregion

                #region Bảng 3

                var listDataBaoCao3Bang3 = new List<ExportBaoCaoKyLuong3Bang1Model>();

                for (int i = 0; i < 3; i++)
                {
                    var newRow = new ExportBaoCaoKyLuong3Bang1Model();
                    newRow.Col1 = i + 1;
                    newRow.Col2 = "COS-3D";

                    if (i == 0)
                    {
                        newRow.Col3 = "Middle";
                    }
                    else if (i == 1)
                    {
                        newRow.Col3 = "Junior";
                    }
                    else if (i == 2)
                    {
                        newRow.Col3 = "Senior";
                    }

                    var item = listCapBac.FirstOrDefault(x => x.CategoryCode == newRow.Col3);
                    var listEmpId = listLuongTongHop.Where(x => x.CapBacId == item.CategoryId).Select(y => y.EmployeeId)
                        .ToList();

                    newRow.Col4 = listLuongTongHop.Count(x => listEmpId.Contains(x.EmployeeId));
                    newRow.Col5 = null;
                    newRow.Col6 = null;
                    newRow.Col7 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.NgayLamViecThucTe);
                    newRow.Col8 = listLuongTongHop.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.MucLuongCu);
                    newRow.Col9 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.NghiPhep);
                    newRow.Col10 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.NghiLe);
                    newRow.Col11 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.CongTac + s.DaoTaoHoiThao + s.NghiCheDo + s.NghiHuongLuongKhac + s.NghiBu);
                    newRow.Col12 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.NghiKhongLuong + s.NghiHuongBhxh);
                    newRow.Col13 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.NghiKhongPhep);
                    newRow.Col14 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.TongNgayDmvs);
                    newRow.Col15 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.SoLanTruChuyenCan);
                    newRow.Col16 = listTongHopChamCong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.TongSoNgayTinhLuong);
                    newRow.Col17 = listLuongCtOther.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.KhoanBuTruThangTruoc);
                    newRow.Col18 = listLuongTongHop.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.LuongThucTe);
                    newRow.Col19 = listLuongTongHop.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.LuongOtTinhThue + s.LuongOtKhongTinhThue);
                    newRow.Col20 = listLuongTongHop.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.TongTroCapCoDinh);
                    newRow.Col21 = listLuongTongHop.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.TongTroCapKhac);
                    newRow.Col22 = listLuongCtOther.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.TroCapKhacKhongTinhThue);
                    newRow.Col23 = listLuongCtOther.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.KhauTruHoanLaiTruocThue);
                    newRow.Col24 = listLuongTongHop.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.ThueTncnNld);
                    newRow.Col25 = listLuongTongHop.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.TongThueTncnCtyVaNld);
                    newRow.Col26 = listLuongCtCtyDong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.Bhtn);
                    newRow.Col27 = listLuongCtBaoHiem.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.Bhtn);
                    newRow.Col28 = listLuongCtCtyDong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.Bhxh + s.Bhtnnn);
                    newRow.Col29 = listLuongCtBaoHiem.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.Bhxh + s.Bhtnnn);
                    newRow.Col30 = listLuongCtCtyDong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.Bhyt);
                    newRow.Col31 = listLuongCtBaoHiem.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.Bhyt);
                    newRow.Col32 = listLuongCtBaoHiem.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.Other);
                    newRow.Col33 = null;
                    newRow.Col34 = null;
                    newRow.Col35 = null;
                    newRow.Col36 = listLuongCtCtyDong.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.KinhPhiCongDoan);
                    newRow.Col37 = listLuongTongHop.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.CacKhoanGiamTruSauThue);
                    newRow.Col38 = listLuongTongHop.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.CacKhoanHoanLaiSauThue);
                    newRow.Col39 = listLuongTongHop.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.ThuNhapThucNhan);
                    newRow.Col40 = listLuongTongHop.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.LuongTamUng);
                    newRow.Col41 = listLuongTongHop.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.LuongConLai);
                    newRow.Col42 = listLuongTongHop.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.TongChiPhiCtyPhaiTra);
                    newRow.Col43 = listLuongTongHop.Where(x => listEmpId.Contains(x.EmployeeId))
                        .Sum(s => s.TongChiPhiCtyPhaiTra);
                    newRow.Col44 = null;
                    newRow.Col45 = null;

                    listDataBaoCao3Bang3.Add(newRow);
                }

                #endregion

                #endregion

                return new ExportBaoCaoKyLuongResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "OK",
                    ListHeaderBaoBao = listHeaderBaoBao,
                    ListDataBaoCao = listDataBaoCao,
                    ThangNamBatDauKyLuongTiengAnh = thangNamBatDauKyLuongTiengAnh,
                    ListDataBaoCao2Bang1 = listDataBaoCao2Bang1,
                    ListDataBaoCao2Bang2 = listDataBaoCao2Bang2,
                    ListDataBaoCao2Bang3 = listDataBaoCao2Bang3,
                    ListDataBaoCao2Bang4 = listDataBaoCao2Bang4,
                    ListDataBaoCao2Bang5 = listDataBaoCao2Bang5,
                    ListDataBaoCao3Bang1 = listDataBaoCao3Bang1,
                    ListDataBaoCao3Bang2 = listDataBaoCao3Bang2,
                    ListDataBaoCao3Bang3 = listDataBaoCao3Bang3
                };
            }
            catch (Exception e)
            {
                return new ExportBaoCaoKyLuongResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CapNhatBangLuongResult CapNhatBangLuong(CapNhatBangLuongParameter parameter)
        {
            try
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    var kyLuong = context.KyLuong.FirstOrDefault(x => x.KyLuongId == parameter.KyLuongId);
                    if (kyLuong == null)
                    {
                        return new CapNhatBangLuongResult()
                        {
                            StatusCode = HttpStatusCode.NotFound,
                            MessageCode = "Kỳ lương không tồn tại trên hệ thống"
                        };
                    }

                    int kyLuongId = kyLuong.KyLuongId;

                    SaveTongHopChamCong(kyLuongId, kyLuong.TuNgay.Date, kyLuong.DenNgay.Date);

                    SaveLuongCt_TroCap(kyLuongId, kyLuong.SoNgayLamViec, kyLuong.TuNgay.Date,
                        kyLuong.DenNgay.Date);

                    SaveLuongCt_TroCapOt(kyLuongId, kyLuong.SoNgayLamViec, kyLuong.TuNgay.Date,
                        kyLuong.DenNgay.Date);

                    SaveLuongCt_ThuNhapTinhThue(kyLuongId, kyLuong.TuNgay.Date, kyLuong.DenNgay.Date);

                    SaveLuongCt_BaoHiem(kyLuongId, kyLuong.TuNgay.Date, kyLuong.DenNgay.Date);

                    SaveLuongCt_GiamTruTruocThue(kyLuongId, kyLuong.TuNgay.Date,
                        kyLuong.DenNgay.Date);

                    SaveLuongCt_GiamTruSauThue(kyLuongId, kyLuong.TuNgay.Date,
                        kyLuong.DenNgay.Date);

                    SaveLuongCt_HoanLaiSauThue(kyLuongId, kyLuong.TuNgay.Date,
                        kyLuong.DenNgay.Date);

                    SaveLuongCt_CtyDong(kyLuongId, kyLuong.TuNgay.Date,
                        kyLuong.DenNgay.Date);

                    SaveLuongCt_Other(kyLuongId, kyLuong.TuNgay.Date,
                        kyLuong.DenNgay.Date);

                    SaveLuongTongHop(kyLuongId, kyLuong.SoNgayLamViec, kyLuong.TuNgay.Date,
                        kyLuong.DenNgay.Date);

                    context.SaveChanges();

                    trans.Commit();

                    return new CapNhatBangLuongResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = "Cập nhật thành công"
                    };
                }
            }
            catch (Exception e)
            {
                return new CapNhatBangLuongResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public CreateOrUpdateKinhPhiResult CreateOrUpdateKinhPhi(CreateOrUpdateKinhPhiParameter parameter)
        {
            try
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    var mess = "";

                    if (parameter.KinhPhiCongDoan.MucDongNhanVien > 100 ||
                        parameter.KinhPhiCongDoan.MucDongCongTy > 100)
                    {
                        return new CreateOrUpdateKinhPhiResult()
                        {
                            StatusCode = HttpStatusCode.Conflict,
                            MessageCode = "Số phần trăm không được vượt quá 100%"
                        };
                    }

                    //Create
                    if (parameter.KinhPhiCongDoan.KinhPhiCongDoanId == null || parameter.KinhPhiCongDoan.KinhPhiCongDoanId == 0)
                    {
                        var kinhPhiCongDoan = parameter.KinhPhiCongDoan.ToEntity();
                        context.KinhPhiCongDoan.Add(kinhPhiCongDoan);
                        context.SaveChanges();

                        mess = "Tạo thành công";
                    }
                    //Update
                    else
                    {
                        var kinhPhiCongDoan = context.KinhPhiCongDoan.
                                FirstOrDefault(x => x.KinhPhiCongDoanId == parameter.KinhPhiCongDoan.KinhPhiCongDoanId);

                        if (kinhPhiCongDoan == null)
                        {
                            return new CreateOrUpdateKinhPhiResult
                            {
                                StatusCode = HttpStatusCode.NotFound,
                                MessageCode = "Cấu hình không tồn tại trên hệ thống"
                            };
                        }

                        kinhPhiCongDoan.MucDongNhanVien = parameter.KinhPhiCongDoan.MucDongNhanVien;
                        kinhPhiCongDoan.MucDongCongTy = parameter.KinhPhiCongDoan.MucDongCongTy;
                        context.KinhPhiCongDoan.Update(kinhPhiCongDoan);
                        context.SaveChanges();

                        mess = "Lưu thành công";
                    }

                    trans.Commit();

                    return new CreateOrUpdateKinhPhiResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = mess,
                    };
                }
            }
            catch (Exception e)
            {
                return new CreateOrUpdateKinhPhiResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public DeleteCauHinhCaLamViecResult DeleteCauHinhCaLamViec(DeleteCauHinhCaLamViecParameter parameter)
        {
            try
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    var objDelete = context.CaLamViec.FirstOrDefault(x =>
                        x.CaLamViecId == parameter.CaLamViecId);
                    if (objDelete == null)
                    {
                        return new DeleteCauHinhCaLamViecResult()
                        {
                            StatusCode = HttpStatusCode.FailedDependency,
                            MessageCode = "Dữ liệu không có trong hệ thống, xóa thất bại.",
                        };
                    }

                    context.CaLamViec.Remove(objDelete);
                    context.SaveChanges();

                    var listCaLamViecChiTiet = context.CaLamViecChiTiet
                        .Where(x => x.CaLamViecId == objDelete.CaLamViecId).ToList();
                    context.CaLamViecChiTiet.RemoveRange(listCaLamViecChiTiet);
                    context.SaveChanges();

                    trans.Commit();

                    return new DeleteCauHinhCaLamViecResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = "Xóa thành công",
                    };
                }
            }
            catch (Exception e)
            {
                return new DeleteCauHinhCaLamViecResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public DeleteCauHinhGiamTruResult DeleteCauHinhGiamTru(DeleteCauHinhGiamTruParameter parameter)
        {
            try
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    var objDelete = context.CauHinhGiamTru.FirstOrDefault(x =>
                        x.CauHinhGiamTruId == parameter.CauHinhGiamTruId);
                    if (objDelete == null)
                    {
                        return new DeleteCauHinhGiamTruResult()
                        {
                            StatusCode = HttpStatusCode.FailedDependency,
                            MessageCode = "Dữ liệu không có trong hệ thống, xóa thất bại.",
                        };
                    }
                    context.CauHinhGiamTru.Remove(objDelete);
                    context.SaveChanges();
                    trans.Commit();

                    return new DeleteCauHinhGiamTruResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = "Xóa thành công",
                    };
                }
            }
            catch (Exception e)
            {
                return new DeleteCauHinhGiamTruResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public DeleteCauHinhNghiLeResult DeleteCauHinhNghiLe(DeleteCauHinhNghiLeParameter parameter)
        {
            try
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    var objDelete = context.CauHinhNghiLe.FirstOrDefault(x =>
                        x.NghiLeId == parameter.NghiLeId);

                    if (objDelete == null)
                    {
                        return new DeleteCauHinhNghiLeResult()
                        {
                            StatusCode = HttpStatusCode.FailedDependency,
                            MessageCode = "Dữ liệu không có trong hệ thống, xóa thất bại.",
                        };
                    }

                    var listOld = context.CauHinhNghiLeChiTiet.Where(x => x.NghiLeId == parameter.NghiLeId).ToList();
                    context.CauHinhNghiLeChiTiet.RemoveRange(listOld);
                    context.CauHinhNghiLe.Remove(objDelete);
                    context.SaveChanges();
                    trans.Commit();

                    return new DeleteCauHinhNghiLeResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = "Xóa thành công",
                    };
                }
            }
            catch (Exception e)
            {
                return new DeleteCauHinhNghiLeResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public DeleteCauHinhOtResult DeleteCauHinhOt(DeleteCauHinhOtParameter parameter)
        {
            try
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    var objDelete = context.CauHinhOt.FirstOrDefault(x => x.CauHinhOtId == parameter.CauHinhOtId);
                    if (objDelete == null)
                    {
                        return new DeleteCauHinhOtResult()
                        {
                            StatusCode = HttpStatusCode.FailedDependency,
                            MessageCode = "Dữ liệu không có trong hệ thống, xóa thất bại.",
                        };
                    }

                    context.CauHinhOt.Remove(objDelete);
                    context.SaveChanges();
                    trans.Commit();

                    return new DeleteCauHinhOtResult()
                    {
                        StatusCode = HttpStatusCode.OK,
                        MessageCode = "Xóa thành công",
                    };
                }
            }
            catch (Exception e)
            {
                return new DeleteCauHinhOtResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        public GetMasterCauHinhLuongResult GetMasterCauHinhLuong(GetMasterCauHinhLuongParameter parameter)
        {
            try
            {
                #region Ca làm việc

                var listLoaiCaLamViec = GeneralList.GetTrangThais("LoaiCaLamViec");
                var listNgayLamViecTrongTuan = GeneralList.GetTrangThais("NgayLamViecTrongTuan");
                var listCaLamViec = context.CaLamViec.Select(y => new CaLamViecModel(y)).ToList();
                var listCaLamViecChiTiet = context.CaLamViecChiTiet.ToList();

                listCaLamViec.ForEach(item =>
                {
                    var loaiCa = listLoaiCaLamViec.FirstOrDefault(x => x.Value == item.LoaiCaLamViecId);
                    item.TenLoaiCaLamViec = loaiCa?.Name;

                    item.ListCaLamViecChiTiet = listCaLamViecChiTiet
                        .Where(x => x.CaLamViecId == item.CaLamViecId)
                        .Select(y => new CaLamViecChiTietModel(y))
                        .OrderBy(z => z.NgayTrongTuan)
                        .ToList();

                    for (int i = 0; i < item.ListCaLamViecChiTiet.Count; i++)
                    {
                        var _item = item.ListCaLamViecChiTiet[i];

                        if (i == 0)
                        {
                            item.NgayLamViecTrongTuanText = _item.NgayTrongTuanText;
                        }
                        else
                        {
                            item.NgayLamViecTrongTuanText += ", " + _item.NgayTrongTuanText;
                        }
                    }
                });

                #endregion

                #region Nghỉ lễ

                var listLoaiNghiLe = GeneralList.GetTrangThais("LoaiNghiLe");
                var listCauHinhNghiLe = context.CauHinhNghiLe.Select(y => new CauHinhNghiLeModel(y))
                    .OrderByDescending(z => z.SoNam).ToList();
                var listCauHinhNghiLeChiTiet = context.CauHinhNghiLeChiTiet.ToList();

                listCauHinhNghiLe.ForEach(item =>
                {
                    var listNgay = listCauHinhNghiLeChiTiet.Where(x => x.NghiLeId == item.NghiLeId)
                        .Select(y => new CauHinhNghiLeChiTietModel(y)).ToList();
                    item.ListCauHinhNghiLeChiTiet = listNgay;

                    #region Ngày nghỉ lễ

                    var listNgayNghiLe = listNgay.Where(x => x.LoaiNghiLe == 1).OrderBy(z => z.Ngay).ToList();

                    for (int i = 0; i < listNgayNghiLe.Count; i++)
                    {
                        var _item = listNgayNghiLe[i];

                        if (i == 0)
                        {
                            item.NgayNghiLe = _item.Ngay.ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            item.NgayNghiLe += ", " + _item.Ngay.ToString("dd/MM/yyyy");
                        }
                    }

                    #endregion

                    #region Ngày nghỉ bù

                    var listNgayNghiBu = listNgay.Where(x => x.LoaiNghiLe == 2).OrderBy(z => z.Ngay).ToList();

                    for (int i = 0; i < listNgayNghiBu.Count; i++)
                    {
                        var _item = listNgayNghiBu[i];

                        if (i == 0)
                        {
                            item.NgayNghiBu = _item.Ngay.ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            item.NgayNghiBu += ", " + _item.Ngay.ToString("dd/MM/yyyy");
                        }
                    }

                    #endregion

                    #region Ngày làm bù

                    var listNgayLamBu = listNgay.Where(x => x.LoaiNghiLe == 3).OrderBy(z => z.Ngay).ToList();

                    for (int i = 0; i < listNgayLamBu.Count; i++)
                    {
                        var _item = listNgayLamBu[i];

                        if (i == 0)
                        {
                            item.NgayLamBu = _item.Ngay.ToString("dd/MM/yyyy");
                        }
                        else
                        {
                            item.NgayLamBu += ", " + _item.Ngay.ToString("dd/MM/yyyy");
                        }
                    }

                    #endregion
                });

                #endregion

                #region Cấu hình OT

                var loaiOtType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LOAIOT");
                var listLoaiOt = new List<CategoryEntityModel>();
                if (loaiOtType != null)
                {
                    listLoaiOt = context.Category
                        .Where(x => x.CategoryTypeId == loaiOtType.CategoryTypeId)
                        .Select(y => new CategoryEntityModel(y))
                        .ToList();
                }

                var listCauHinhOt = context.CauHinhOt.Select(y => new CauHinhOtModel(y))
                    .OrderByDescending(z => z.CreatedDate).ToList();

                var stt_ot = 0;
                listCauHinhOt.ForEach(item =>
                {
                    stt_ot++;
                    item.Stt = stt_ot;

                    var loaiOt = listLoaiOt.FirstOrDefault(x => x.CategoryId == item.LoaiOtId);
                    item.TenLoaiOt = loaiOt?.CategoryName;
                });

                #endregion

                #region Cấu hình giảm trừ

                var listLoaiGiamTru = GeneralList.GetTrangThais("LoaiGiamTru");
                var listCauHinhGiamTru = context.CauHinhGiamTru.Select(y => new CauHinhGiamTruModel(y))
                    .OrderByDescending(z => z.CreatedDate).ToList();
                var stt_giamtru = 0;
                listCauHinhGiamTru.ForEach(item =>
                {
                    stt_giamtru++;
                    item.Stt = stt_giamtru;
                });

                #endregion

                #region Cấu hình kinh phí công đoàn

                var kinhPhiCongDoan = context.KinhPhiCongDoan
                        .Select(y => new KinhPhiCongDoanModel(y)).FirstOrDefault();

                #endregion

                #region Công thức tính lương

                var listTokenTinhLuong = GeneralList.GetTrangThais("TokenTinhLuong");
                var congThucTinhLuong = context.CongThucTinhLuong
                    .Select(y => new CongThucTinhLuongModel(y)).FirstOrDefault();

                #endregion

                #region Cấu hình chấm công OT

                var cauHinhChamCongOt = context.CauHinhChamCongOt
                    .Select(y => new CauHinhChamCongOtModel(y)).FirstOrDefault();

                #endregion

                #region Cấu hình OT cả ngày

                var cauHinhOtCaNgay = context.CauHinhOtCaNgay
                    .Select(y => new CauHinhOtCaNgayModel(y)).FirstOrDefault();

                #endregion

                #region Cấu hình thuế TNCN

                var listCauHinhThueTncn = context.CauHinhThueTncn
                    .Select(y => new CauHinhThueTncnModel(y))
                    .OrderBy(z => z.SoTienTu).ToList();

                #endregion

                return new GetMasterCauHinhLuongResult()
                {
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Success",
                    ListCaLamViec = listCaLamViec,
                    ListLoaiCaLamViec = listLoaiCaLamViec,
                    ListNgayLamViecTrongTuan = listNgayLamViecTrongTuan,
                    ListLoaiNghiLe = listLoaiNghiLe,
                    ListCauHinhNghiLe = listCauHinhNghiLe,
                    ListLoaiOt = listLoaiOt,
                    ListCauHinhOt = listCauHinhOt,
                    ListLoaiGiamTru = listLoaiGiamTru,
                    ListCauHinhGiamTru = listCauHinhGiamTru,
                    KinhPhiCongDoan = kinhPhiCongDoan,
                    ListTokenTinhLuong = listTokenTinhLuong,
                    CongThucTinhLuong = congThucTinhLuong,
                    CauHinhChamCongOt = cauHinhChamCongOt,
                    CauHinhOtCaNgay = cauHinhOtCaNgay,
                    ListCauHinhThueTncn = listCauHinhThueTncn
                };
            }
            catch (Exception e)
            {
                return new GetMasterCauHinhLuongResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }

        private int ConvertNgayTrongTuan(DayOfWeek ngay)
        {
            switch (ngay)
            {
                case DayOfWeek.Monday:
                    return 1;
                case DayOfWeek.Tuesday:
                    return 2;
                case DayOfWeek.Wednesday:
                    return 3;
                case DayOfWeek.Thursday:
                    return 4;
                case DayOfWeek.Friday:
                    return 5;
                case DayOfWeek.Saturday:
                    return 6;
                case DayOfWeek.Sunday:
                    return 0;
                default: return -1;
            }
        }

        private void GetGioChamCong(Guid employeeId, DateTime ngayChamCong, List<TimeSpan> listThoiGianChamCong, List<CaLamViecModel> listCaLamViec, 
            int ngayTrongTuan, out bool isNoData, out ChamCong data, out List<ThongKeDiMuonVeSom> listThongKeDiMuonVeSom)
        {
            data = new ChamCong
            {
                VaoSang = null,
                RaSang = null,
                VaoChieu = null,
                RaChieu = null
            };

            listThongKeDiMuonVeSom = new List<ThongKeDiMuonVeSom>();

            isNoData = false;
            listThoiGianChamCong = listThoiGianChamCong.OrderBy(z => z).ToList();

            var listCaLamViecChiTiet = new List<CaLamViecChiTietModel>();
            listCaLamViec.ForEach(item =>
            {
                listCaLamViecChiTiet.AddRange(item.ListCaLamViecChiTiet);
            });

            //Lấy ra cấu hình ca làm việc chi tiết theo ngày chấm công
            var _listCaLamViecChiTiet = listCaLamViecChiTiet.Where(x => x.NgayTrongTuan == ngayTrongTuan).ToList();

            //Nếu ngày chấm công không có trong cấu hình ca làm việc thì bỏ qua không thêm data
            if (_listCaLamViecChiTiet.Count == 0)
            {
                isNoData = true;
                return;
            }

            var _listCaLamViecId = _listCaLamViecChiTiet.Select(y => y.CaLamViecId).ToList();

            /* Nếu ngày chấm công có cấu hình ca làm việc thì */
            var caSang =
                listCaLamViec.FirstOrDefault(x => _listCaLamViecId.Contains(x.CaLamViecId) && x.LoaiCaLamViecId == 1);
            var caChieu =
                listCaLamViec.FirstOrDefault(x => _listCaLamViecId.Contains(x.CaLamViecId) && x.LoaiCaLamViecId == 2);

            //Nếu có cấu hình ca sáng
            if (caSang != null)
            {
                var _listGio = listThoiGianChamCong.Where(x => x <= caSang.ThoiGianKetThucCa)
                    .OrderBy(z => z).ToList();

                if (_listGio.Count == 1)
                {
                    data.VaoSang = _listGio.First();

                    //Nếu đi muộn
                    if (data.VaoSang > caSang.GioVao)
                    {
                        int thoiGian = (int)Math.Floor(TinhSoPhut(caSang.GioVao, data.VaoSang.Value));
                        if (thoiGian > 0)
                        {
                            listThongKeDiMuonVeSom.Add(new ThongKeDiMuonVeSom
                            {
                                EmployeeId = employeeId,
                                LoaiDmvs = 1,
                                NgayDmvs = ngayChamCong,
                                ThoiGian = thoiGian
                            });
                        }
                    }
                }

                if (_listGio.Count > 1)
                {
                    data.VaoSang = _listGio.First();
                    data.RaSang = _listGio.Last();

                    #region Tính đi muộn về sớm

                    //Nếu đi muộn
                    if (data.VaoSang > caSang.GioVao)
                    {
                        int thoiGian = (int)Math.Floor(TinhSoPhut(caSang.GioVao, data.VaoSang.Value));
                        if (thoiGian > 0)
                        {
                            listThongKeDiMuonVeSom.Add(new ThongKeDiMuonVeSom
                            {
                                EmployeeId = employeeId,
                                LoaiDmvs = 1,
                                NgayDmvs = ngayChamCong,
                                ThoiGian = thoiGian
                            });
                        }
                    }

                    //Nếu về sớm
                    if (data.RaSang < caSang.GioRa)
                    {
                        int thoiGian = (int)Math.Floor(TinhSoPhut(data.RaSang.Value, caSang.GioRa));
                        if (thoiGian > 0)
                        {
                            listThongKeDiMuonVeSom.Add(new ThongKeDiMuonVeSom
                            {
                                EmployeeId = employeeId,
                                LoaiDmvs = 2,
                                NgayDmvs = ngayChamCong,
                                ThoiGian = thoiGian
                            });
                        }
                    }

                    #endregion
                }
            }

            //Nếu có cấu hình ca chiều
            if (caChieu != null)
            {
                var _listGio = listThoiGianChamCong.Where(x => (caSang == null || x > caSang.ThoiGianKetThucCa) && 
                                                               x <= caChieu.ThoiGianKetThucCa)
                    .OrderBy(z => z).ToList();

                if (_listGio.Count == 1)
                {
                    data.VaoChieu = _listGio.First();

                    //Nếu đi muộn
                    if (data.VaoChieu > caChieu.GioVao)
                    {
                        int thoiGian = (int)Math.Floor(TinhSoPhut(caChieu.GioVao, data.VaoChieu.Value));
                        if (thoiGian > 0)
                        {
                            listThongKeDiMuonVeSom.Add(new ThongKeDiMuonVeSom
                            {
                                EmployeeId = employeeId,
                                LoaiDmvs = 3,
                                NgayDmvs = ngayChamCong,
                                ThoiGian = thoiGian
                            });
                        }
                    }
                }

                if (_listGio.Count > 1)
                {
                    data.VaoChieu = _listGio.First();
                    data.RaChieu = _listGio.Last();

                    #region Tính đi muộn về sớm

                    //Nếu đi muộn
                    if (data.VaoChieu > caChieu.GioVao)
                    {
                        int thoiGian = (int)Math.Floor(TinhSoPhut(caChieu.GioVao, data.VaoChieu.Value));
                        if (thoiGian > 0)
                        {
                            listThongKeDiMuonVeSom.Add(new ThongKeDiMuonVeSom
                            {
                                EmployeeId = employeeId,
                                LoaiDmvs = 3,
                                NgayDmvs = ngayChamCong,
                                ThoiGian = thoiGian
                            });
                        }
                    }

                    //Nếu về sớm
                    if (data.RaChieu < caChieu.GioRa)
                    {
                        int thoiGian = (int)Math.Floor(TinhSoPhut(data.RaChieu.Value, caChieu.GioRa));
                        if (thoiGian > 0)
                        {
                            listThongKeDiMuonVeSom.Add(new ThongKeDiMuonVeSom
                            {
                                EmployeeId = employeeId,
                                LoaiDmvs = 4,
                                NgayDmvs = ngayChamCong,
                                ThoiGian = thoiGian
                            });
                        }
                    }

                    #endregion
                }
            }
        }

        private double TinhSoPhut(TimeSpan start, TimeSpan end)
        {
            double result = (end - start).TotalMinutes;

            return result;
        }

        private string HienThiKyHieu(ChamCong chamCong, int type)
        {
            string result = "--";
            var listKyHieuChamCong = GeneralList.GetTrangThais("KyHieuChamCong");

            //Vào sáng
            if (type == 1)
            {
                //Nếu có dữ liệu chấm công
                if (chamCong.VaoSang != null)
                {
                    //Nếu ko có ký hiệu
                    if (chamCong.KyHieuVaoSang == null)
                    {
                        result = chamCong.VaoSang.Value.ToString(@"hh\:mm");
                    }
                    //Nếu có ký hiệu
                    else
                    {
                        result = listKyHieuChamCong.FirstOrDefault(x => x.Value == chamCong.KyHieuVaoSang)?.ValueText;
                    }
                }
                //Nếu không có dữ liệu chấm công
                else
                {
                    //Nếu có ký hiệu
                    if (chamCong.KyHieuVaoSang != null)
                    {
                        result = listKyHieuChamCong.FirstOrDefault(x => x.Value == chamCong.KyHieuVaoSang)?.ValueText;
                    }
                }
            }
            //Ra sáng
            else if (type == 2)
            {
                //Nếu có dữ liệu chấm công
                if (chamCong.RaSang != null)
                {
                    //Nếu ko có ký hiệu
                    if (chamCong.KyHieuRaSang == null)
                    {
                        result = chamCong.RaSang.Value.ToString(@"hh\:mm");
                    }
                    //Nếu có ký hiệu
                    else
                    {
                        result = listKyHieuChamCong.FirstOrDefault(x => x.Value == chamCong.KyHieuRaSang)?.ValueText;
                    }
                }
                //Nếu không có dữ liệu chấm công
                else
                {
                    //Nếu có ký hiệu
                    if (chamCong.KyHieuRaSang != null)
                    {
                        result = listKyHieuChamCong.FirstOrDefault(x => x.Value == chamCong.KyHieuRaSang)?.ValueText;
                    }
                }
            }
            //Vào chiều
            else if (type == 3)
            {
                //Nếu có dữ liệu chấm công
                if (chamCong.VaoChieu != null)
                {
                    //Nếu ko có ký hiệu
                    if (chamCong.KyHieuVaoChieu == null)
                    {
                        result = chamCong.VaoChieu.Value.ToString(@"hh\:mm");
                    }
                    //Nếu có ký hiệu
                    else
                    {
                        result = listKyHieuChamCong.FirstOrDefault(x => x.Value == chamCong.KyHieuVaoChieu)?.ValueText;
                    }
                }
                //Nếu không có dữ liệu chấm công
                else
                {
                    //Nếu có ký hiệu
                    if (chamCong.KyHieuVaoChieu != null)
                    {
                        result = listKyHieuChamCong.FirstOrDefault(x => x.Value == chamCong.KyHieuVaoChieu)?.ValueText;
                    }
                }
            }
            //Ra chiều
            else if (type == 4)
            {
                //Nếu có dữ liệu chấm công
                if (chamCong.RaChieu != null)
                {
                    //Nếu ko có ký hiệu
                    if (chamCong.KyHieuRaChieu == null)
                    {
                        result = chamCong.RaChieu.Value.ToString(@"hh\:mm");
                    }
                    //Nếu có ký hiệu
                    else
                    {
                        result = listKyHieuChamCong.FirstOrDefault(x => x.Value == chamCong.KyHieuRaChieu)?.ValueText;
                    }
                }
                //Nếu không có dữ liệu chấm công
                else
                {
                    //Nếu có ký hiệu
                    if (chamCong.KyHieuRaChieu != null)
                    {
                        result = listKyHieuChamCong.FirstOrDefault(x => x.Value == chamCong.KyHieuRaChieu)?.ValueText;
                    }
                }
            }

            return result;
        }

        private string HienThiMau(ChamCong chamCong, int type, List<ThongKeDiMuonVeSom> listDmvs)
        {
            string result = "#000";
            string red = "#f44336";
            var _listDmvs = listDmvs.Where(x => x.EmployeeId == chamCong.EmployeeId && x.NgayDmvs == chamCong.NgayChamCong)
                .ToList();

            //Vào sáng
            if (type == 1)
            {
                if (chamCong.VaoSang != null)
                {
                    var dmvs = _listDmvs.FirstOrDefault(x => x.LoaiDmvs == 1);

                    //Nếu ko có ký hiệu và là đi muộn về sớm
                    if (chamCong.KyHieuVaoSang == null && dmvs != null)
                    {
                        result = red;
                    }
                }
            }
            //Ra sáng
            else if (type == 2)
            {
                if (chamCong.RaSang != null)
                {
                    var dmvs = _listDmvs.FirstOrDefault(x => x.LoaiDmvs == 2);

                    //Nếu ko có ký hiệu và là đi muộn về sớm
                    if (chamCong.KyHieuRaSang == null && dmvs != null)
                    {
                        result = red;
                    }
                }
            }
            //Vào chiều
            else if (type == 3)
            {
                if (chamCong.VaoChieu != null)
                {
                    var dmvs = _listDmvs.FirstOrDefault(x => x.LoaiDmvs == 3);

                    //Nếu ko có ký hiệu và là đi muộn về sớm
                    if (chamCong.KyHieuVaoChieu == null && dmvs != null)
                    {
                        result = red;
                    }
                }
            }
            //Ra chiều
            else if (type == 4)
            {
                if (chamCong.RaChieu != null)
                {
                    var dmvs = _listDmvs.FirstOrDefault(x => x.LoaiDmvs == 4);

                    //Nếu ko có ký hiệu và là đi muộn về sớm
                    if (chamCong.KyHieuRaChieu == null && dmvs != null)
                    {
                        result = red;
                    }
                }
            }

            return result;
        }

        private int GetSoLanDmvs(Guid employeeId, List<ThongKeDiMuonVeSom> listDmvs)
        {
            int result = 0;

            result = listDmvs.Count(x => x.EmployeeId == employeeId);

            return result;
        }

        private double GetTongSoNgayNghi(Guid employeeId, List<ChamCong> listChamCong)
        {
            double result = 0;

            var _listChamCong = listChamCong.Where(x => x.EmployeeId == employeeId &&
                                                        (x.KyHieuVaoSang != null || x.KyHieuRaSang != null ||
                                                         x.KyHieuVaoChieu != null || x.KyHieuRaChieu != null)).ToList();

            //Bỏ qua nếu đc đánh dấu là Làm tại nhà (mã 14)
            _listChamCong.ForEach(item =>
            {
                if (item.KyHieuVaoSang != null && item.KyHieuVaoSang != 14) result += 0.25;
                if (item.KyHieuRaSang != null && item.KyHieuRaSang != 14) result += 0.25;
                if (item.KyHieuVaoChieu != null && item.KyHieuVaoChieu != 14) result += 0.25;
                if (item.KyHieuRaChieu != null && item.KyHieuRaChieu != 14) result += 0.25;
            });

            return result;
        }

        private CaLamViecModel CheckExistsCauHinhChamCong(List<CaLamViecModel> listCaLamViec, int ngayTrongTuan, int loaiCaLamViecId)
        {
            //Mặc định là có cấu hình
            var caLamViec = new CaLamViecModel();

            var listCaLamViecChiTiet = new List<CaLamViecChiTietModel>();
            listCaLamViec.ForEach(item =>
            {
                listCaLamViecChiTiet.AddRange(item.ListCaLamViecChiTiet);
            });

            //Lấy ra cấu hình ca làm việc chi tiết theo ngày chấm công
            var _listCaLamViecChiTiet = listCaLamViecChiTiet.Where(x => x.NgayTrongTuan == ngayTrongTuan).ToList();
            var _listCaLamViecId = _listCaLamViecChiTiet.Select(y => y.CaLamViecId).ToList();

            /* Nếu ngày chấm công có cấu hình ca làm việc thì */
            caLamViec = listCaLamViec.FirstOrDefault(x =>
                _listCaLamViecId.Contains(x.CaLamViecId) && x.LoaiCaLamViecId == loaiCaLamViecId);

            return caLamViec;
        }

        private string CheckGioVaoGioRa(TimeSpan? gioVao, TimeSpan? gioRa)
        {
            string result = "";

            bool valid_1 = !(gioVao != null && gioRa != null && gioVao > gioRa);

            bool valid_2 = !(gioVao == null && gioRa != null);


            if (!valid_1)
            {
                result = "Giờ vào phải trước Giờ ra";
            }
            else if (!valid_2)
            {
                result = "Có giờ ra mà không có giờ vào";
            }

            return result;
        }

        private void GetThongKeDmvsTheoThoiGian(CaLamViecModel caLamViec, int typeField, TimeSpan thoiGian, out ThongKeDiMuonVeSom thongKeDmvs)
        {
            thongKeDmvs = new ThongKeDiMuonVeSom();
            thongKeDmvs.LoaiDmvs = typeField;

            //Vào sáng
            if (typeField == 1 || typeField == 3)
            {
                if (thoiGian > caLamViec.GioVao)
                {
                    int _thoiGian = (int)Math.Floor(TinhSoPhut(caLamViec.GioVao, thoiGian));
                    if (_thoiGian > 0)
                    {
                        thongKeDmvs.ThoiGian = _thoiGian;
                    }
                }
            }
            //Ra sáng
            else if (typeField == 2 || typeField == 4)
            {
                if (thoiGian < caLamViec.GioRa)
                {
                    int _thoiGian = (int)Math.Floor(TinhSoPhut(thoiGian, caLamViec.GioRa));
                    if (_thoiGian > 0)
                    {
                        thongKeDmvs.ThoiGian = _thoiGian;
                    }
                }
            }
        }

        private bool CheckSyntaxCongThuc(string congThuc)
        {
            bool isValid;
            var listTokenTinhLuong = GeneralList.GetTrangThais("TokenTinhLuong");

            listTokenTinhLuong.ForEach(token => { congThuc = congThuc.Replace(token.ValueText, "1"); });

            try
            {
                var result = new DataTable().Compute(congThuc, null).ToString();

                decimal _result;
                isValid = decimal.TryParse(result, out _result);
            }
            //Nếu công thức sai syntax
            catch (Exception e)
            {
                isValid = false;
            }

            return isValid;
        }

        private string ConvertGioChamCongOt(TimeSpan? gio)
        {
            string result = "--";

            if (gio != null)
            {
                result = gio.Value.ToString(@"hh\:mm");
            }

            return result;
        }

        private List<TongHopChamCongOt> TinhTongHopChamCongOt(List<ChamCongOt> listChamCongOt)
        {
            var result = new List<TongHopChamCongOt>();

            #region Ca sáng

            var listCaSang = listChamCongOt
                .Where(x => x.GioVaoSang != null && x.GioRaSang != null && x.GioVaoSang < x.GioRaSang)
                .GroupBy(g => new {g.EmployeeId, g.LoaiOtId, g.NgayChamCong})
                .Select(y => new TongHopChamCongOt
                {
                    EmployeeId = y.Key.EmployeeId,
                    NgayChamCong = y.Key.NgayChamCong,
                    LoaiOtId = y.Key.LoaiOtId,
                    SoPhut = Math.Round(
                        (decimal) y.Sum(s => s.GioRaSang.Value.TotalMinutes - s.GioVaoSang.Value.TotalMinutes), 0),
                    SoGio = Math.Round(
                        (decimal) y.Sum(s => s.GioRaSang.Value.TotalMinutes - s.GioVaoSang.Value.TotalMinutes) / 60, 2)
                }).ToList();

            result.AddRange(listCaSang);

            #endregion

            #region Ca chiều

            var listCaChieu = listChamCongOt
                .Where(x => x.GioVaoChieu != null && x.GioRaChieu != null && x.GioVaoChieu < x.GioRaChieu)
                .GroupBy(g => new {g.EmployeeId, g.LoaiOtId, g.NgayChamCong})
                .Select(y => new TongHopChamCongOt
                {
                    EmployeeId = y.Key.EmployeeId,
                    NgayChamCong = y.Key.NgayChamCong,
                    LoaiOtId = y.Key.LoaiOtId,
                    SoPhut = Math.Round(
                        (decimal) y.Sum(s => s.GioRaChieu.Value.TotalMinutes - s.GioVaoChieu.Value.TotalMinutes), 0),
                    SoGio = Math.Round(
                        (decimal) y.Sum(s => s.GioRaChieu.Value.TotalMinutes - s.GioVaoChieu.Value.TotalMinutes) / 60, 2)
                }).ToList();

            result.AddRange(listCaChieu);

            #endregion

            #region Ca tối

            var listCaToi = listChamCongOt
                .Where(x => x.GioVaoToi != null && x.GioRaToi != null && x.GioVaoToi < x.GioRaToi)
                .GroupBy(g => new {g.EmployeeId, g.LoaiOtId, g.NgayChamCong})
                .Select(y => new TongHopChamCongOt
                {
                    EmployeeId = y.Key.EmployeeId,
                    NgayChamCong = y.Key.NgayChamCong,
                    LoaiOtId = y.Key.LoaiOtId,
                    SoPhut = Math.Round(
                        (decimal) y.Sum(s => s.GioRaToi.Value.TotalMinutes - s.GioVaoToi.Value.TotalMinutes), 0),
                    SoGio = Math.Round(
                        (decimal) y.Sum(s => s.GioRaToi.Value.TotalMinutes - s.GioVaoToi.Value.TotalMinutes) / 60, 2)
                }).ToList();

            result.AddRange(listCaToi);

            #endregion

            result = result.GroupBy(g => new { g.EmployeeId, g.LoaiOtId, g.NgayChamCong })
                .Select(y => new TongHopChamCongOt
                {
                    EmployeeId = y.Key.EmployeeId,
                    NgayChamCong = y.Key.NgayChamCong,
                    LoaiOtId = y.Key.LoaiOtId,
                    SoPhut = y.Sum(s => s.SoPhut),
                    SoGio = y.Sum(s => s.SoGio)
                }).ToList();

            return result;
        }

        private void SaveTongHopChamCong(int kyLuongId, DateTime ngayBatDau, DateTime ngayKetThuc)
        {
            #region Dữ liệu cũ

            var listOld = context.TongHopChamCong.Where(x => x.KyLuongId == kyLuongId).ToList();

            #endregion

            var listChamCong = context.ChamCong.Where(x => x.NgayChamCong.Date >= ngayBatDau.Date &&
                                                           x.NgayChamCong.Date <= ngayKetThuc.Date).ToList();

            var listEmpId = listChamCong.Select(y => y.EmployeeId).Distinct().ToList();
            var listEmp = context.Employee.Where(x => listEmpId.Contains(x.EmployeeId)).OrderBy(z => z.EmployeeCode).ToList();

            var listDmvs = context.ThongKeDiMuonVeSom
                .Where(x => listEmpId.Contains(x.EmployeeId) &&
                            x.NgayDmvs.Date >= ngayBatDau.Date &&
                            x.NgayDmvs.Date <= ngayKetThuc.Date).ToList();

            var listTongHopChamCong = new List<TongHopChamCong>();

            listEmp.ForEach(item =>
            {
                var newItem = new TongHopChamCong();
                newItem.KyLuongId = kyLuongId;
                newItem.EmployeeId = item.EmployeeId;
                newItem.OrganizationId = item.OrganizationId ?? Guid.Empty;
                newItem.PositionId = item.PositionId ?? Guid.Empty;

                var listChamCongByEmp = listChamCong.Where(x => x.EmployeeId == item.EmployeeId).ToList();

                #region Tính số ngày

                double ngayLvThucTe = 0;
                double ngayCongTac = 0;
                double ngayHoiThao = 0;
                double ngayNghiPhep = 0;
                double ngayNghiLe = 0;
                double ngayNghiCheDo = 0;
                double ngayNghiHuongLuongKhac = 0;
                double ngayNghiBu = 0;
                double ngayNghiBhxh = 0;
                double ngayNghiTuYKoLuong = 0;
                double ngayNghiKoLuong_KoDuLieu = 0;

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

                    #region Ngày nghỉ BHXH

                    if (chamCong.KyHieuVaoSang == 11) ngayNghiBhxh += 0.25;
                    if (chamCong.KyHieuRaSang == 11) ngayNghiBhxh += 0.25;
                    if (chamCong.KyHieuVaoChieu == 11) ngayNghiBhxh += 0.25;
                    if (chamCong.KyHieuRaChieu == 11) ngayNghiBhxh += 0.25;

                    #endregion

                    #region Ngày nghỉ tự ý nghỉ không lương

                    if (chamCong.KyHieuVaoSang == 9) ngayNghiTuYKoLuong += 0.25;
                    if (chamCong.KyHieuRaSang == 9) ngayNghiTuYKoLuong += 0.25;
                    if (chamCong.KyHieuVaoChieu == 9) ngayNghiTuYKoLuong += 0.25;
                    if (chamCong.KyHieuRaChieu == 9) ngayNghiTuYKoLuong += 0.25;

                    #endregion

                    #region Ngày nghỉ không lương + Không có dữ liệu bảng công

                    if (chamCong.KyHieuVaoSang == 8 || chamCong.KyHieuVaoSang == 10) ngayNghiKoLuong_KoDuLieu += 0.25;
                    if (chamCong.KyHieuRaSang == 8 || chamCong.KyHieuRaSang == 10) ngayNghiKoLuong_KoDuLieu += 0.25;
                    if (chamCong.KyHieuVaoChieu == 8 || chamCong.KyHieuVaoChieu == 10) ngayNghiKoLuong_KoDuLieu += 0.25;
                    if (chamCong.KyHieuRaChieu == 8 || chamCong.KyHieuRaChieu == 10) ngayNghiKoLuong_KoDuLieu += 0.25;

                    #endregion
                    
                });

                #region Tổng ngày đi muộn/về sớm không được trả lương

                int tongSoPhutDmvs = CommonHelper.GetSoPhutDmvs(item.EmployeeId, listDmvs);
                double tongSoNgayDmvs = Math.Round((double)tongSoPhutDmvs / 480, 2);

                #endregion

                #region Số lần trừ chuyên cần

                int tongSoLanDmvs = GetSoLanDmvs(item.EmployeeId, listDmvs);

                #endregion

                #region Tổng số ngày không tính lương

                double tongNgayKhongTinhLuong = ngayNghiKoLuong_KoDuLieu + ngayNghiTuYKoLuong +
                                                ngayNghiBhxh + tongSoNgayDmvs;

                #endregion

                #region Tổng số ngày tính lương

                double tongNgayTinhLuong = ngayLvThucTe + ngayCongTac + ngayHoiThao + ngayNghiPhep +
                                           ngayNghiLe + ngayNghiBu + ngayNghiCheDo + ngayNghiHuongLuongKhac -
                                           tongSoNgayDmvs;

                #endregion

                #region Tổng ngày nghỉ tính trợ cấp chuyên cần

                double tongNgayNghiTinhTroCapChuyenCan = ngayNghiPhep + ngayNghiKoLuong_KoDuLieu + ngayNghiTuYKoLuong;

                #endregion

                #endregion

                newItem.NgayLamViecThucTe = (decimal) ngayLvThucTe;
                newItem.CongTac = (decimal) ngayCongTac;
                newItem.DaoTaoHoiThao = (decimal) ngayHoiThao;
                newItem.NghiPhep = (decimal) ngayNghiPhep;
                newItem.NghiLe = (decimal) ngayNghiLe;
                newItem.NghiCheDo = (decimal) ngayNghiCheDo;
                newItem.NghiHuongLuongKhac = (decimal) ngayNghiHuongLuongKhac;
                newItem.NghiBu = (decimal) ngayNghiBu;
                newItem.NghiHuongBhxh = (decimal) ngayNghiBhxh;
                newItem.NghiKhongPhep = (decimal) ngayNghiTuYKoLuong;
                newItem.NghiKhongLuong = (decimal) ngayNghiKoLuong_KoDuLieu;
                newItem.TongNgayDmvs = (decimal) tongSoNgayDmvs;
                newItem.SoLanTruChuyenCan = (decimal) tongSoLanDmvs;

                var exists = listOld.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);

                //Create
                if (exists == null)
                {
                    listTongHopChamCong.Add(newItem);
                }
                //Update
                else
                {
                    exists.OrganizationId = item.OrganizationId ?? Guid.Empty;
                    exists.PositionId = item.PositionId ?? Guid.Empty;
                    exists.NgayLamViecThucTe = (decimal)ngayLvThucTe;
                    exists.CongTac = (decimal)ngayCongTac;
                    exists.DaoTaoHoiThao = (decimal)ngayHoiThao;
                    exists.NghiPhep = (decimal)ngayNghiPhep;
                    exists.NghiLe = (decimal)ngayNghiLe;
                    exists.NghiCheDo = (decimal)ngayNghiCheDo;
                    exists.NghiHuongLuongKhac = (decimal)ngayNghiHuongLuongKhac;
                    exists.NghiBu = (decimal)ngayNghiBu;
                    exists.NghiHuongBhxh = (decimal)ngayNghiBhxh;
                    exists.NghiKhongPhep = (decimal)ngayNghiTuYKoLuong;
                    exists.NghiKhongLuong = (decimal)ngayNghiKoLuong_KoDuLieu;
                    exists.TongNgayDmvs = (decimal)tongSoNgayDmvs;
                    exists.SoLanTruChuyenCan = (decimal)tongSoLanDmvs;

                    context.TongHopChamCong.Update(exists);
                }
            });

            context.TongHopChamCong.AddRange(listTongHopChamCong);
            context.SaveChanges();
        }

        private void SaveLuongCt_TroCap(int kyLuongId, decimal soNgayLamViec, DateTime ngayBatDau, DateTime ngayKetThuc)
        {
            #region Dữ liệu cũ

            var listOldLuongCtTroCapCoDinh = context.LuongCtTroCapCoDinh.Where(x => x.KyLuongId == kyLuongId).ToList();
            var listOldLuongCtLoaiTroCapCoDinh = context.LuongCtLoaiTroCapCoDinh.Where(x => x.KyLuongId == kyLuongId).ToList();
            var listOldLuongCtDieuKienTroCapCoDinh = context.LuongCtDieuKienTroCapCoDinh.Where(x => x.KyLuongId == kyLuongId).ToList();

            context.SaveChanges();

            #endregion

            var listTongHopChamCong = context.TongHopChamCong.Where(x => x.KyLuongId == kyLuongId).ToList();
            var listEmpId = listTongHopChamCong.Select(y => y.EmployeeId).Distinct().ToList();
            var listEmp = context.Employee.Where(x => listEmpId.Contains(x.EmployeeId)).OrderBy(z => z.EmployeeCode).ToList();

            var loaiHopDongType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LHDNS");
            var listLoaiHopDong = context.Category.Where(x => x.CategoryTypeId == loaiHopDongType.CategoryTypeId).ToList();
            var listHopDongNhanSu = context.HopDongNhanSu.Where(x => listEmpId.Contains(x.EmployeeId)).ToList();
            var listTroCap = context.TroCap.Where(x => x.TypeId != 4).ToList();
            var listTroCapChucVu = context.TroCapChucVuMapping.ToList();
            var listTroCapLoaiHopDong = context.TroCapLoaiHopDongMapping.ToList();
            var listTroCapDieuKienHuong = context.TroCapDieuKienHuongMapping.ToList();
            var listMucHuongTheoNgayNghi = context.MucHuongTheoNgayNghi.ToList();
            var listMucHuongDmvs = context.MucHuongDmvs.ToList();
            var listChamCong = context.ChamCong.Where(x => x.NgayChamCong.Date >= ngayBatDau.Date &&
                                                           x.NgayChamCong.Date <= ngayKetThuc.Date).ToList();
            var listDmvs = context.ThongKeDiMuonVeSom.Where(x => listEmpId.Contains(x.EmployeeId) &&
                                                                 x.NgayDmvs.Date >= ngayBatDau.Date &&
                                                                 x.NgayDmvs.Date <= ngayKetThuc.Date).ToList(); 

            for (int e = 0; e < listEmp.Count; e++)
            {
                var item = listEmp[e];

                var ngayLamViecThucTe = listTongHopChamCong.FirstOrDefault(x => x.EmployeeId == item.EmployeeId)
                                       ?.NgayLamViecThucTe ?? 0;

                if (ngayLamViecThucTe == 0)
                {
                    //continue;
                }

                var hopDongMoiNhat = listHopDongNhanSu.Where(x => x.EmployeeId == item.EmployeeId)
                    .OrderByDescending(z => z.NgayBatDauLamViec).FirstOrDefault();
                var loaiHopDong = listLoaiHopDong.FirstOrDefault(x => x.CategoryId == hopDongMoiNhat?.LoaiHopDongId);
                var listChamCongByEmp = listChamCong.Where(x => x.EmployeeId == item.EmployeeId).ToList();

                var existsTroCapCoDinh =
                    listOldLuongCtTroCapCoDinh.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);

                //Nếu chưa có data bảng 1
                if (existsTroCapCoDinh == null)
                {
                    var luongCtTroCapCoDinh = new LuongCtTroCapCoDinh();
                    luongCtTroCapCoDinh.KyLuongId = kyLuongId;
                    luongCtTroCapCoDinh.EmployeeId = item.EmployeeId;
                    luongCtTroCapCoDinh.OrganizationId = item.OrganizationId ?? Guid.Empty;
                    luongCtTroCapCoDinh.PositionId = item.PositionId ?? Guid.Empty;
                    luongCtTroCapCoDinh.LoaiHopDongId = loaiHopDong?.CategoryId;

                    context.LuongCtTroCapCoDinh.Add(luongCtTroCapCoDinh);
                    context.SaveChanges();

                    #region Trợ cấp cố định

                    var listTroCapCoDinh = listTroCap.Where(x => x.TypeId == 1).ToList();

                    for (int i = 0; i < listTroCapCoDinh.Count; i++)
                    {
                        var troCap = listTroCapCoDinh[i];
                        var listTcChucVu = listTroCapChucVu.Where(x => x.TroCapId == troCap.TroCapId).ToList();
                        var listChucVuId = listTcChucVu.Select(y => y.PositionId).ToList();
                        var listTcLoaiHopDong = listTroCapLoaiHopDong.Where(x => x.TroCapId == troCap.TroCapId).ToList();
                        var listLoaiHopDongId = listTcLoaiHopDong.Select(y => y.LoaiHopDongId).ToList();
                        var listTcDieuKienHuong =
                            listTroCapDieuKienHuong.Where(x => x.TroCapId == troCap.TroCapId).ToList();

                        var luongCtLoaiTroCapCoDinh = new LuongCtLoaiTroCapCoDinh();
                        luongCtLoaiTroCapCoDinh.KyLuongId = kyLuongId;
                        luongCtLoaiTroCapCoDinh.LuongCtTroCapCoDinhId = luongCtTroCapCoDinh.LuongCtTroCapCoDinhId;
                        luongCtLoaiTroCapCoDinh.LoaiTroCapId = troCap.LoaiTroCapId;
                        luongCtLoaiTroCapCoDinh.MucTroCap = Math.Round(troCap.MucTroCap * ngayLamViecThucTe / soNgayLamViec, 0);

                        //Nếu đủ điều kiện Chức vụ và Loại hợp đồng trong cấu hình
                        if (listChucVuId.Contains(item.PositionId ?? Guid.Empty) &&
                            listLoaiHopDongId.Contains(loaiHopDong?.CategoryId ?? Guid.Empty))
                        {
                            luongCtLoaiTroCapCoDinh.MucTroCap = Math.Round(troCap.MucTroCap * ngayLamViecThucTe / soNgayLamViec, 0);

                            context.LuongCtLoaiTroCapCoDinh.Add(luongCtLoaiTroCapCoDinh);
                            context.SaveChanges();

                            listTcDieuKienHuong.ForEach(dieuKienHuong =>
                            {
                                var luongCtDieuKienTroCapCoDinh = new LuongCtDieuKienTroCapCoDinh();
                                luongCtDieuKienTroCapCoDinh.KyLuongId = kyLuongId;
                                luongCtDieuKienTroCapCoDinh.LuongCtTroCapCoDinhId = luongCtTroCapCoDinh.LuongCtTroCapCoDinhId;
                                luongCtDieuKienTroCapCoDinh.LuongCtLoaiTroCapCoDinhId = luongCtLoaiTroCapCoDinh.LuongCtLoaiTroCapCoDinhId;
                                luongCtDieuKienTroCapCoDinh.DieuKienHuongId = dieuKienHuong.DieuKienHuongId;
                                luongCtDieuKienTroCapCoDinh.DuDieuKien = true;

                                context.LuongCtDieuKienTroCapCoDinh.Add(luongCtDieuKienTroCapCoDinh);
                                context.SaveChanges();
                            });
                        }
                        //Nếu không đủ điều kiện
                        else
                        {
                            luongCtLoaiTroCapCoDinh.MucTroCap = 0;

                            context.LuongCtLoaiTroCapCoDinh.Add(luongCtLoaiTroCapCoDinh);
                            context.SaveChanges();
                        }
                    }

                    #endregion

                    #region Trợ cấp theo ngày nghỉ

                    var troCapTheoNgayNghi = listTroCap.FirstOrDefault(x => x.TypeId == 2);

                    if (troCapTheoNgayNghi != null)
                    {
                        var listTcChucVu = listTroCapChucVu.Where(x => x.TroCapId == troCapTheoNgayNghi.TroCapId).ToList();
                        var listChucVuId = listTcChucVu.Select(y => y.PositionId).ToList();
                        var listTcLoaiHopDong = listTroCapLoaiHopDong
                            .Where(x => x.TroCapId == troCapTheoNgayNghi.TroCapId).ToList();
                        var listLoaiHopDongId = listTcLoaiHopDong.Select(y => y.LoaiHopDongId).ToList();
                        var listTcDieuKienHuong = listTroCapDieuKienHuong
                            .Where(x => x.TroCapId == troCapTheoNgayNghi.TroCapId).ToList();
                        var listMucHuong = listMucHuongTheoNgayNghi.Where(x => x.TroCapId == troCapTheoNgayNghi.TroCapId)
                            .OrderByDescending(z => z.MucHuongPhanTram).ToList();

                        var luongCtLoaiTroCapCoDinh = new LuongCtLoaiTroCapCoDinh();
                        luongCtLoaiTroCapCoDinh.KyLuongId = kyLuongId;
                        luongCtLoaiTroCapCoDinh.LuongCtTroCapCoDinhId = luongCtTroCapCoDinh.LuongCtTroCapCoDinhId;
                        luongCtLoaiTroCapCoDinh.LoaiTroCapId = troCapTheoNgayNghi.LoaiTroCapId;

                        if (listChucVuId.Contains(item.PositionId ?? Guid.Empty) &&
                            listLoaiHopDongId.Contains(loaiHopDong?.CategoryId ?? Guid.Empty))
                        {
                            luongCtLoaiTroCapCoDinh.MucTroCap = TinhMucHuongTroCapTheoNgayNghi(troCapTheoNgayNghi,
                                listChamCongByEmp, listMucHuong);

                            context.LuongCtLoaiTroCapCoDinh.Add(luongCtLoaiTroCapCoDinh);
                            context.SaveChanges();

                            listTcDieuKienHuong.ForEach(dieuKienHuong =>
                            {
                                var luongCtDieuKienTroCapCoDinh = new LuongCtDieuKienTroCapCoDinh();
                                luongCtDieuKienTroCapCoDinh.KyLuongId = kyLuongId;
                                luongCtDieuKienTroCapCoDinh.LuongCtTroCapCoDinhId = luongCtTroCapCoDinh.LuongCtTroCapCoDinhId;
                                luongCtDieuKienTroCapCoDinh.LuongCtLoaiTroCapCoDinhId = luongCtLoaiTroCapCoDinh.LuongCtLoaiTroCapCoDinhId;
                                luongCtDieuKienTroCapCoDinh.DieuKienHuongId = dieuKienHuong.DieuKienHuongId;
                                luongCtDieuKienTroCapCoDinh.DuDieuKien = true;

                                context.LuongCtDieuKienTroCapCoDinh.Add(luongCtDieuKienTroCapCoDinh);
                                context.SaveChanges();
                            });
                        }
                        else
                        {
                            luongCtLoaiTroCapCoDinh.MucTroCap = 0;

                            context.LuongCtLoaiTroCapCoDinh.Add(luongCtLoaiTroCapCoDinh);
                            context.SaveChanges();
                        }
                    }

                    #endregion

                    #region Trợ cấp theo số lần đi muộn về sớm

                    var troCapTheoDmvs = listTroCap.FirstOrDefault(x => x.TypeId == 3);

                    if (troCapTheoDmvs != null)
                    {
                        var listTcChucVu = listTroCapChucVu.Where(x => x.TroCapId == troCapTheoDmvs.TroCapId).ToList();
                        var listChucVuId = listTcChucVu.Select(y => y.PositionId).ToList();
                        var listTcLoaiHopDong = listTroCapLoaiHopDong
                            .Where(x => x.TroCapId == troCapTheoDmvs.TroCapId).ToList();
                        var listLoaiHopDongId = listTcLoaiHopDong.Select(y => y.LoaiHopDongId).ToList();
                        var listTcDieuKienHuong = listTroCapDieuKienHuong
                            .Where(x => x.TroCapId == troCapTheoDmvs.TroCapId).ToList();
                        var listMucHuong = listMucHuongDmvs.Where(x => x.TroCapId == troCapTheoDmvs.TroCapId)
                            .OrderByDescending(z => z.MucTru).ToList();

                        var luongCtLoaiTroCapCoDinh = new LuongCtLoaiTroCapCoDinh();
                        luongCtLoaiTroCapCoDinh.KyLuongId = kyLuongId;
                        luongCtLoaiTroCapCoDinh.LuongCtTroCapCoDinhId = luongCtTroCapCoDinh.LuongCtTroCapCoDinhId;
                        luongCtLoaiTroCapCoDinh.LoaiTroCapId = troCapTheoDmvs.LoaiTroCapId;

                        if (listChucVuId.Contains(item.PositionId ?? Guid.Empty) &&
                            listLoaiHopDongId.Contains(loaiHopDong?.CategoryId ?? Guid.Empty))
                        {
                            int soLanDmvs = GetSoLanDmvs(item.EmployeeId, listDmvs);

                            luongCtLoaiTroCapCoDinh.MucTroCap = TinhMucHuongTroCapTheoDmvs(troCapTheoDmvs,
                                soLanDmvs, listMucHuong);

                            context.LuongCtLoaiTroCapCoDinh.Add(luongCtLoaiTroCapCoDinh);
                            context.SaveChanges();

                            listTcDieuKienHuong.ForEach(dieuKienHuong =>
                            {
                                var luongCtDieuKienTroCapCoDinh = new LuongCtDieuKienTroCapCoDinh();
                                luongCtDieuKienTroCapCoDinh.KyLuongId = kyLuongId;
                                luongCtDieuKienTroCapCoDinh.LuongCtTroCapCoDinhId = luongCtTroCapCoDinh.LuongCtTroCapCoDinhId;
                                luongCtDieuKienTroCapCoDinh.LuongCtLoaiTroCapCoDinhId = luongCtLoaiTroCapCoDinh.LuongCtLoaiTroCapCoDinhId;
                                luongCtDieuKienTroCapCoDinh.DieuKienHuongId = dieuKienHuong.DieuKienHuongId;
                                luongCtDieuKienTroCapCoDinh.DuDieuKien = true;

                                context.LuongCtDieuKienTroCapCoDinh.Add(luongCtDieuKienTroCapCoDinh);
                                context.SaveChanges();
                            });
                        }
                        else
                        {
                            luongCtLoaiTroCapCoDinh.MucTroCap = 0;

                            context.LuongCtLoaiTroCapCoDinh.Add(luongCtLoaiTroCapCoDinh);
                            context.SaveChanges();
                        }
                    }

                    #endregion
                }
                //Nếu đã có data bảng 1
                else
                {
                    existsTroCapCoDinh.OrganizationId = item.OrganizationId ?? Guid.Empty;
                    existsTroCapCoDinh.PositionId = item.PositionId ?? Guid.Empty;
                    existsTroCapCoDinh.LoaiHopDongId = loaiHopDong?.CategoryId;

                    context.LuongCtTroCapCoDinh.Update(existsTroCapCoDinh);
                    context.SaveChanges();

                    var _listOldLuongCtLoaiTroCapCoDinh = listOldLuongCtLoaiTroCapCoDinh.Where(x =>
                        x.LuongCtTroCapCoDinhId == existsTroCapCoDinh.LuongCtTroCapCoDinhId).ToList();

                    #region Trợ cấp cố định

                    var listTroCapCoDinh = listTroCap.Where(x => x.TypeId == 1).ToList();

                    for (int i = 0; i < listTroCapCoDinh.Count; i++)
                    {
                        var troCap = listTroCapCoDinh[i];
                        var listTcChucVu = listTroCapChucVu.Where(x => x.TroCapId == troCap.TroCapId).ToList();
                        var listChucVuId = listTcChucVu.Select(y => y.PositionId).ToList();
                        var listTcLoaiHopDong = listTroCapLoaiHopDong.Where(x => x.TroCapId == troCap.TroCapId).ToList();
                        var listLoaiHopDongId = listTcLoaiHopDong.Select(y => y.LoaiHopDongId).ToList();
                        var listTcDieuKienHuong =
                            listTroCapDieuKienHuong.Where(x => x.TroCapId == troCap.TroCapId).ToList();

                        var existsLoaiTroCap = listOldLuongCtLoaiTroCapCoDinh.FirstOrDefault(x =>
                            x.LuongCtTroCapCoDinhId == existsTroCapCoDinh.LuongCtTroCapCoDinhId &&
                            x.LoaiTroCapId == troCap.LoaiTroCapId);

                        //Nếu chưa có data bảng 2
                        if (existsLoaiTroCap == null)
                        {
                            var luongCtLoaiTroCapCoDinh = new LuongCtLoaiTroCapCoDinh();
                            luongCtLoaiTroCapCoDinh.KyLuongId = kyLuongId;
                            luongCtLoaiTroCapCoDinh.LuongCtTroCapCoDinhId = existsTroCapCoDinh.LuongCtTroCapCoDinhId;
                            luongCtLoaiTroCapCoDinh.LoaiTroCapId = troCap.LoaiTroCapId;

                            context.LuongCtLoaiTroCapCoDinh.Add(luongCtLoaiTroCapCoDinh);
                            context.SaveChanges();

                            //Nếu đủ điều kiện Chức vụ và Loại hợp đồng trong cấu hình
                            if (listChucVuId.Contains(item.PositionId ?? Guid.Empty) &&
                                listLoaiHopDongId.Contains(loaiHopDong?.CategoryId ?? Guid.Empty))
                            {
                                luongCtLoaiTroCapCoDinh.MucTroCap = Math.Round(troCap.MucTroCap * ngayLamViecThucTe / soNgayLamViec, 0);

                                context.LuongCtLoaiTroCapCoDinh.Add(luongCtLoaiTroCapCoDinh);
                                context.SaveChanges();

                                listTcDieuKienHuong.ForEach(dieuKienHuong =>
                                {
                                    var luongCtDieuKienTroCapCoDinh = new LuongCtDieuKienTroCapCoDinh();
                                    luongCtDieuKienTroCapCoDinh.KyLuongId = kyLuongId;
                                    luongCtDieuKienTroCapCoDinh.LuongCtTroCapCoDinhId = existsTroCapCoDinh.LuongCtTroCapCoDinhId;
                                    luongCtDieuKienTroCapCoDinh.LuongCtLoaiTroCapCoDinhId =
                                        luongCtLoaiTroCapCoDinh.LuongCtLoaiTroCapCoDinhId;
                                    luongCtDieuKienTroCapCoDinh.DieuKienHuongId = dieuKienHuong.DieuKienHuongId;
                                    luongCtDieuKienTroCapCoDinh.DuDieuKien = true;

                                    context.LuongCtDieuKienTroCapCoDinh.Add(luongCtDieuKienTroCapCoDinh);
                                    context.SaveChanges();
                                });
                            }
                            //Nếu không đủ điều kiện
                            else
                            {
                                luongCtLoaiTroCapCoDinh.MucTroCap = 0;

                                context.LuongCtLoaiTroCapCoDinh.Add(luongCtLoaiTroCapCoDinh);
                                context.SaveChanges();
                            }
                        }
                        //Nếu đã có data bảng 2
                        else
                        {
                            #region Kiểm tra nếu cấu hình điều kiện hưởng có, mà trong data không có => thêm data

                            listTcDieuKienHuong.ForEach(dieuKienHuong =>
                            {
                                var existsDieuKienHuong = listOldLuongCtDieuKienTroCapCoDinh.FirstOrDefault(x =>
                                    x.LuongCtLoaiTroCapCoDinhId == existsLoaiTroCap.LuongCtLoaiTroCapCoDinhId &&
                                    x.DieuKienHuongId == dieuKienHuong.DieuKienHuongId);

                                //Nếu chưa có data bảng 3
                                if (existsDieuKienHuong == null)
                                {
                                    var luongCtDieuKienTroCapCoDinh = new LuongCtDieuKienTroCapCoDinh();
                                    luongCtDieuKienTroCapCoDinh.KyLuongId = kyLuongId;
                                    luongCtDieuKienTroCapCoDinh.LuongCtTroCapCoDinhId = existsTroCapCoDinh.LuongCtTroCapCoDinhId;
                                    luongCtDieuKienTroCapCoDinh.LuongCtLoaiTroCapCoDinhId =
                                        existsLoaiTroCap.LuongCtLoaiTroCapCoDinhId;
                                    luongCtDieuKienTroCapCoDinh.DieuKienHuongId = dieuKienHuong.DieuKienHuongId;
                                    luongCtDieuKienTroCapCoDinh.DuDieuKien = true;

                                    context.LuongCtDieuKienTroCapCoDinh.Add(luongCtDieuKienTroCapCoDinh);
                                    context.SaveChanges();
                                }
                            });

                            #endregion

                            #region Kiểm tra nếu cấu hình điều kiện hưởng không có, mà trong data có => xóa data

                            var _listOldLuongCtDieuKienTroCapCoDinh = listOldLuongCtDieuKienTroCapCoDinh.Where(x =>
                                    x.LuongCtLoaiTroCapCoDinhId == existsLoaiTroCap.LuongCtLoaiTroCapCoDinhId)
                                .ToList();

                            var listRemoveDieuKienTroCapCoDinh = new List<LuongCtDieuKienTroCapCoDinh>();
                            _listOldLuongCtDieuKienTroCapCoDinh.ForEach(dataDieuKienHuong =>
                            {
                                var existsDieuKienHuong = listTcDieuKienHuong.FirstOrDefault(x =>
                                    x.DieuKienHuongId == dataDieuKienHuong.DieuKienHuongId);

                                if (existsDieuKienHuong == null)
                                {
                                    listRemoveDieuKienTroCapCoDinh.Add(dataDieuKienHuong);
                                }
                            });

                            context.LuongCtDieuKienTroCapCoDinh.RemoveRange(listRemoveDieuKienTroCapCoDinh);
                            context.SaveChanges();

                            #endregion

                            #region Tính lại mức trợ cấp

                            //Nếu đủ điều kiện Chức vụ và Loại hợp đồng trong cấu hình
                            if (listChucVuId.Contains(item.PositionId ?? Guid.Empty) &&
                                listLoaiHopDongId.Contains(loaiHopDong?.CategoryId ?? Guid.Empty))
                            {
                                var countFalse = context.LuongCtDieuKienTroCapCoDinh.Count(x =>
                                    x.KyLuongId == kyLuongId &&
                                    x.LuongCtLoaiTroCapCoDinhId == existsLoaiTroCap.LuongCtLoaiTroCapCoDinhId &&
                                    x.DuDieuKien == false);

                                //Nếu tất cả điều kiện hưởng đều = true
                                if (countFalse == 0)
                                {
                                    existsLoaiTroCap.MucTroCap = Math.Round(troCap.MucTroCap * ngayLamViecThucTe / soNgayLamViec, 0);
                                }
                                //Nếu có điều kiện hưởng = false
                                else
                                {
                                    existsLoaiTroCap.MucTroCap = 0;
                                }
                            }
                            //Nếu không đủ điều kiện
                            else
                            {
                                existsLoaiTroCap.MucTroCap = 0;

                                //Xóa điều kiện hưởng
                                var newListDieuKienHuong = context.LuongCtDieuKienTroCapCoDinh.Where(x =>
                                    x.KyLuongId == kyLuongId &&
                                    x.LuongCtLoaiTroCapCoDinhId == existsLoaiTroCap.LuongCtLoaiTroCapCoDinhId).ToList();

                                context.LuongCtDieuKienTroCapCoDinh.RemoveRange(newListDieuKienHuong);
                            }

                            context.LuongCtLoaiTroCapCoDinh.Update(existsLoaiTroCap);
                            context.SaveChanges();

                            #endregion
                        }
                    }

                    #endregion

                    #region Trợ cấp theo ngày nghỉ

                    var troCapTheoNgayNghi = listTroCap.FirstOrDefault(x => x.TypeId == 2);

                    if (troCapTheoNgayNghi != null)
                    {
                        var listTcChucVu = listTroCapChucVu.Where(x => x.TroCapId == troCapTheoNgayNghi.TroCapId).ToList();
                        var listChucVuId = listTcChucVu.Select(y => y.PositionId).ToList();
                        var listTcLoaiHopDong = listTroCapLoaiHopDong
                            .Where(x => x.TroCapId == troCapTheoNgayNghi.TroCapId).ToList();
                        var listLoaiHopDongId = listTcLoaiHopDong.Select(y => y.LoaiHopDongId).ToList();
                        var listTcDieuKienHuong = listTroCapDieuKienHuong
                            .Where(x => x.TroCapId == troCapTheoNgayNghi.TroCapId).ToList();
                        var listMucHuong = listMucHuongTheoNgayNghi.Where(x => x.TroCapId == troCapTheoNgayNghi.TroCapId)
                            .OrderByDescending(z => z.MucHuongPhanTram).ToList();

                        var existsDataTroCapTheoNgayNghi = listOldLuongCtLoaiTroCapCoDinh.FirstOrDefault(x =>
                            x.LuongCtTroCapCoDinhId == existsTroCapCoDinh.LuongCtTroCapCoDinhId &&
                            x.LoaiTroCapId == troCapTheoNgayNghi.LoaiTroCapId);

                        //Nếu không có data trong bảng 2
                        if (existsDataTroCapTheoNgayNghi == null)
                        {
                            var luongCtLoaiTroCapCoDinh = new LuongCtLoaiTroCapCoDinh();
                            luongCtLoaiTroCapCoDinh.KyLuongId = kyLuongId;
                            luongCtLoaiTroCapCoDinh.LuongCtTroCapCoDinhId = existsTroCapCoDinh.LuongCtTroCapCoDinhId;
                            luongCtLoaiTroCapCoDinh.LoaiTroCapId = troCapTheoNgayNghi.LoaiTroCapId;

                            if (listChucVuId.Contains(item.PositionId ?? Guid.Empty) &&
                                listLoaiHopDongId.Contains(loaiHopDong?.CategoryId ?? Guid.Empty))
                            {
                                luongCtLoaiTroCapCoDinh.MucTroCap = TinhMucHuongTroCapTheoNgayNghi(troCapTheoNgayNghi,
                                    listChamCongByEmp, listMucHuong);

                                context.LuongCtLoaiTroCapCoDinh.Add(luongCtLoaiTroCapCoDinh);
                                context.SaveChanges();

                                listTcDieuKienHuong.ForEach(dieuKienHuong =>
                                {
                                    var luongCtDieuKienTroCapCoDinh = new LuongCtDieuKienTroCapCoDinh();
                                    luongCtDieuKienTroCapCoDinh.KyLuongId = kyLuongId;
                                    luongCtDieuKienTroCapCoDinh.LuongCtTroCapCoDinhId = existsTroCapCoDinh.LuongCtTroCapCoDinhId;
                                    luongCtDieuKienTroCapCoDinh.LuongCtLoaiTroCapCoDinhId = luongCtLoaiTroCapCoDinh.LuongCtLoaiTroCapCoDinhId;
                                    luongCtDieuKienTroCapCoDinh.DieuKienHuongId = dieuKienHuong.DieuKienHuongId;
                                    luongCtDieuKienTroCapCoDinh.DuDieuKien = true;

                                    context.LuongCtDieuKienTroCapCoDinh.Add(luongCtDieuKienTroCapCoDinh);
                                    context.SaveChanges();
                                });
                            }
                            else
                            {
                                luongCtLoaiTroCapCoDinh.MucTroCap = 0;

                                context.LuongCtLoaiTroCapCoDinh.Add(luongCtLoaiTroCapCoDinh);
                                context.SaveChanges();
                            }
                        }
                        //Nếu có data trong bảng 2
                        else
                        {
                            //Nếu đủ điều kiện Chức vụ và Loại hợp đồng trong cấu hình
                            if (listChucVuId.Contains(item.PositionId ?? Guid.Empty) &&
                                listLoaiHopDongId.Contains(loaiHopDong?.CategoryId ?? Guid.Empty))
                            {
                                #region Kiểm tra nếu cấu hình điều kiện hưởng có, mà trong data không có => thêm data

                                listTcDieuKienHuong.ForEach(dieuKienHuong =>
                                {
                                    var existsDieuKienHuong = listOldLuongCtDieuKienTroCapCoDinh.FirstOrDefault(x =>
                                        x.LuongCtLoaiTroCapCoDinhId == existsDataTroCapTheoNgayNghi.LuongCtLoaiTroCapCoDinhId &&
                                        x.DieuKienHuongId == dieuKienHuong.DieuKienHuongId);

                                    //Nếu chưa có data bảng 3
                                    if (existsDieuKienHuong == null)
                                    {
                                        var luongCtDieuKienTroCapCoDinh = new LuongCtDieuKienTroCapCoDinh();
                                        luongCtDieuKienTroCapCoDinh.KyLuongId = kyLuongId;
                                        luongCtDieuKienTroCapCoDinh.LuongCtTroCapCoDinhId = existsTroCapCoDinh.LuongCtTroCapCoDinhId;
                                        luongCtDieuKienTroCapCoDinh.LuongCtLoaiTroCapCoDinhId =
                                            existsDataTroCapTheoNgayNghi.LuongCtLoaiTroCapCoDinhId;
                                        luongCtDieuKienTroCapCoDinh.DieuKienHuongId = dieuKienHuong.DieuKienHuongId;
                                        luongCtDieuKienTroCapCoDinh.DuDieuKien = true;

                                        context.LuongCtDieuKienTroCapCoDinh.Add(luongCtDieuKienTroCapCoDinh);
                                        context.SaveChanges();
                                    }
                                });

                                #endregion

                                #region Kiểm tra nếu cấu hình điều kiện hưởng không có, mà trong data có => xóa data

                                var _listOldLuongCtDieuKienTroCapCoDinh = listOldLuongCtDieuKienTroCapCoDinh.Where(x =>
                                        x.LuongCtLoaiTroCapCoDinhId == existsDataTroCapTheoNgayNghi.LuongCtLoaiTroCapCoDinhId)
                                    .ToList();

                                var listRemoveDieuKienTroCapCoDinh = new List<LuongCtDieuKienTroCapCoDinh>();
                                _listOldLuongCtDieuKienTroCapCoDinh.ForEach(dataDieuKienHuong =>
                                {
                                    var existsDieuKienHuong = listTcDieuKienHuong.FirstOrDefault(x =>
                                        x.DieuKienHuongId == dataDieuKienHuong.DieuKienHuongId);

                                    if (existsDieuKienHuong == null)
                                    {
                                        listRemoveDieuKienTroCapCoDinh.Add(dataDieuKienHuong);
                                    }
                                });

                                context.LuongCtDieuKienTroCapCoDinh.RemoveRange(listRemoveDieuKienTroCapCoDinh);
                                context.SaveChanges();

                                #endregion

                                #region Tính lại mức trợ cấp

                                var countFalse = context.LuongCtDieuKienTroCapCoDinh.Count(x =>
                                    x.KyLuongId == kyLuongId &&
                                    x.LuongCtLoaiTroCapCoDinhId == existsDataTroCapTheoNgayNghi.LuongCtLoaiTroCapCoDinhId &&
                                    x.DuDieuKien == false);

                                //Nếu tất cả điều kiện hưởng đều = true
                                if (countFalse == 0)
                                {
                                    existsDataTroCapTheoNgayNghi.MucTroCap = TinhMucHuongTroCapTheoNgayNghi(
                                        troCapTheoNgayNghi, listChamCongByEmp, listMucHuong);
                                }
                                //Nếu có điều kiện hưởng = false
                                else
                                {
                                    existsDataTroCapTheoNgayNghi.MucTroCap = 0;
                                }

                                context.LuongCtLoaiTroCapCoDinh.Update(existsDataTroCapTheoNgayNghi);
                                context.SaveChanges();

                                #endregion
                            }
                            //Nếu không đủ điều kiện
                            else
                            {
                                existsDataTroCapTheoNgayNghi.MucTroCap = 0;

                                context.LuongCtLoaiTroCapCoDinh.Update(existsDataTroCapTheoNgayNghi);
                                context.SaveChanges();

                                //Xóa data điều kiện hưởng
                                var newListDieuKienHuong = listOldLuongCtDieuKienTroCapCoDinh.Where(x =>
                                    x.LuongCtLoaiTroCapCoDinhId ==
                                    existsDataTroCapTheoNgayNghi.LuongCtLoaiTroCapCoDinhId).ToList();

                                context.LuongCtDieuKienTroCapCoDinh.RemoveRange(newListDieuKienHuong);
                                context.SaveChanges();
                            }
                        }
                    }

                    #endregion

                    #region Trợ cấp theo số lần đi muộn về sớm

                    var troCapTheoDmvs = listTroCap.FirstOrDefault(x => x.TypeId == 3);

                    if (troCapTheoDmvs != null)
                    {
                        var listTcChucVu = listTroCapChucVu.Where(x => x.TroCapId == troCapTheoDmvs.TroCapId).ToList();
                        var listChucVuId = listTcChucVu.Select(y => y.PositionId).ToList();
                        var listTcLoaiHopDong = listTroCapLoaiHopDong
                            .Where(x => x.TroCapId == troCapTheoDmvs.TroCapId).ToList();
                        var listLoaiHopDongId = listTcLoaiHopDong.Select(y => y.LoaiHopDongId).ToList();
                        var listTcDieuKienHuong = listTroCapDieuKienHuong
                            .Where(x => x.TroCapId == troCapTheoDmvs.TroCapId).ToList();
                        var listMucHuong = listMucHuongDmvs.Where(x => x.TroCapId == troCapTheoDmvs.TroCapId)
                            .OrderByDescending(z => z.MucTru).ToList();

                        var existsDataTroCapTheoDmvs = listOldLuongCtLoaiTroCapCoDinh.FirstOrDefault(x =>
                            x.LuongCtTroCapCoDinhId == existsTroCapCoDinh.LuongCtTroCapCoDinhId &&
                            x.LoaiTroCapId == troCapTheoDmvs.LoaiTroCapId);

                        //Nếu không có data trong bảng 2
                        if (existsDataTroCapTheoDmvs == null)
                        {
                            var luongCtLoaiTroCapCoDinh = new LuongCtLoaiTroCapCoDinh();
                            luongCtLoaiTroCapCoDinh.KyLuongId = kyLuongId;
                            luongCtLoaiTroCapCoDinh.LuongCtTroCapCoDinhId = existsDataTroCapTheoDmvs.LuongCtTroCapCoDinhId;
                            luongCtLoaiTroCapCoDinh.LoaiTroCapId = troCapTheoDmvs.LoaiTroCapId;

                            if (listChucVuId.Contains(item.PositionId ?? Guid.Empty) &&
                                listLoaiHopDongId.Contains(loaiHopDong?.CategoryId ?? Guid.Empty))
                            {
                                int soLanDmvs = GetSoLanDmvs(item.EmployeeId, listDmvs);

                                luongCtLoaiTroCapCoDinh.MucTroCap = TinhMucHuongTroCapTheoDmvs(troCapTheoDmvs,
                                    soLanDmvs, listMucHuong);

                                context.LuongCtLoaiTroCapCoDinh.Add(luongCtLoaiTroCapCoDinh);
                                context.SaveChanges();

                                listTcDieuKienHuong.ForEach(dieuKienHuong =>
                                {
                                    var luongCtDieuKienTroCapCoDinh = new LuongCtDieuKienTroCapCoDinh();
                                    luongCtDieuKienTroCapCoDinh.KyLuongId = kyLuongId;
                                    luongCtDieuKienTroCapCoDinh.LuongCtTroCapCoDinhId = existsDataTroCapTheoDmvs.LuongCtTroCapCoDinhId;
                                    luongCtDieuKienTroCapCoDinh.LuongCtLoaiTroCapCoDinhId = luongCtLoaiTroCapCoDinh.LuongCtLoaiTroCapCoDinhId;
                                    luongCtDieuKienTroCapCoDinh.DieuKienHuongId = dieuKienHuong.DieuKienHuongId;
                                    luongCtDieuKienTroCapCoDinh.DuDieuKien = true;

                                    context.LuongCtDieuKienTroCapCoDinh.Add(luongCtDieuKienTroCapCoDinh);
                                    context.SaveChanges();
                                });
                            }
                            else
                            {
                                luongCtLoaiTroCapCoDinh.MucTroCap = 0;

                                context.LuongCtLoaiTroCapCoDinh.Add(luongCtLoaiTroCapCoDinh);
                                context.SaveChanges();
                            }
                        }
                        //Nếu có data trong bảng 2
                        else
                        {
                            //Nếu đủ điều kiện Chức vụ và Loại hợp đồng trong cấu hình
                            if (listChucVuId.Contains(item.PositionId ?? Guid.Empty) &&
                                listLoaiHopDongId.Contains(loaiHopDong?.CategoryId ?? Guid.Empty))
                            {
                                #region Kiểm tra nếu cấu hình điều kiện hưởng có, mà trong data không có => thêm data

                                listTcDieuKienHuong.ForEach(dieuKienHuong =>
                                {
                                    var existsDieuKienHuong = listOldLuongCtDieuKienTroCapCoDinh.FirstOrDefault(x =>
                                        x.LuongCtLoaiTroCapCoDinhId == existsDataTroCapTheoDmvs.LuongCtLoaiTroCapCoDinhId &&
                                        x.DieuKienHuongId == dieuKienHuong.DieuKienHuongId);

                                    //Nếu chưa có data bảng 3
                                    if (existsDieuKienHuong == null)
                                    {
                                        var luongCtDieuKienTroCapCoDinh = new LuongCtDieuKienTroCapCoDinh();
                                        luongCtDieuKienTroCapCoDinh.KyLuongId = kyLuongId;
                                        luongCtDieuKienTroCapCoDinh.LuongCtTroCapCoDinhId = existsTroCapCoDinh.LuongCtTroCapCoDinhId;
                                        luongCtDieuKienTroCapCoDinh.LuongCtLoaiTroCapCoDinhId =
                                            existsDataTroCapTheoDmvs.LuongCtLoaiTroCapCoDinhId;
                                        luongCtDieuKienTroCapCoDinh.DieuKienHuongId = dieuKienHuong.DieuKienHuongId;
                                        luongCtDieuKienTroCapCoDinh.DuDieuKien = true;

                                        context.LuongCtDieuKienTroCapCoDinh.Add(luongCtDieuKienTroCapCoDinh);
                                        context.SaveChanges();
                                    }
                                });

                                #endregion

                                #region Kiểm tra nếu cấu hình điều kiện hưởng không có, mà trong data có => xóa data

                                var _listOldLuongCtDieuKienTroCapCoDinh = listOldLuongCtDieuKienTroCapCoDinh.Where(x =>
                                        x.LuongCtLoaiTroCapCoDinhId == existsDataTroCapTheoDmvs.LuongCtLoaiTroCapCoDinhId)
                                    .ToList();

                                var listRemoveDieuKienTroCapCoDinh = new List<LuongCtDieuKienTroCapCoDinh>();
                                _listOldLuongCtDieuKienTroCapCoDinh.ForEach(dataDieuKienHuong =>
                                {
                                    var existsDieuKienHuong = listTcDieuKienHuong.FirstOrDefault(x =>
                                        x.DieuKienHuongId == dataDieuKienHuong.DieuKienHuongId);

                                    if (existsDieuKienHuong == null)
                                    {
                                        listRemoveDieuKienTroCapCoDinh.Add(dataDieuKienHuong);
                                    }
                                });

                                context.LuongCtDieuKienTroCapCoDinh.RemoveRange(listRemoveDieuKienTroCapCoDinh);
                                context.SaveChanges();

                                #endregion

                                #region Tính lại mức trợ cấp

                                var countFalse = context.LuongCtDieuKienTroCapCoDinh.Count(x =>
                                    x.KyLuongId == kyLuongId &&
                                    x.LuongCtLoaiTroCapCoDinhId == existsDataTroCapTheoDmvs.LuongCtLoaiTroCapCoDinhId &&
                                    x.DuDieuKien == false);

                                //Nếu tất cả điều kiện hưởng đều = true
                                if (countFalse == 0)
                                {
                                    int soLanDmvs = GetSoLanDmvs(item.EmployeeId, listDmvs);

                                    existsDataTroCapTheoDmvs.MucTroCap = TinhMucHuongTroCapTheoDmvs(troCapTheoDmvs,
                                        soLanDmvs, listMucHuong);
                                }
                                //Nếu có điều kiện hưởng = false
                                else
                                {
                                    existsDataTroCapTheoDmvs.MucTroCap = 0;
                                }

                                context.LuongCtLoaiTroCapCoDinh.Update(existsDataTroCapTheoDmvs);
                                context.SaveChanges();

                                #endregion
                            }
                            //Nếu không đủ điều kiện
                            else
                            {
                                existsDataTroCapTheoDmvs.MucTroCap = 0;

                                context.LuongCtLoaiTroCapCoDinh.Update(existsDataTroCapTheoDmvs);
                                context.SaveChanges();

                                //Xóa data điều kiện hưởng
                                var newListDieuKienHuong = listOldLuongCtDieuKienTroCapCoDinh.Where(x =>
                                    x.LuongCtLoaiTroCapCoDinhId ==
                                    existsDataTroCapTheoDmvs.LuongCtLoaiTroCapCoDinhId).ToList();

                                context.LuongCtDieuKienTroCapCoDinh.RemoveRange(newListDieuKienHuong);
                                context.SaveChanges();
                            }
                        }
                    }

                    #endregion

                    #region Kiểm tra nếu cấu hình loại trợ cấp không có, mà trong data có => xóa data

                    var listRemoveLuongCtLoaiTroCapCoDinh = new List<LuongCtLoaiTroCapCoDinh>();
                    var listRemoveLuongCtDieuKienTroCapCoDinh = new List<LuongCtDieuKienTroCapCoDinh>();
                    _listOldLuongCtLoaiTroCapCoDinh.ForEach(dataLoaiTroCap =>
                    {
                        var existsLoaiTroCap =
                            listTroCap.FirstOrDefault(x => x.LoaiTroCapId == dataLoaiTroCap.LoaiTroCapId);

                        if (existsLoaiTroCap == null)
                        {
                            listRemoveLuongCtLoaiTroCapCoDinh.Add(dataLoaiTroCap);

                            var _listRemoveLuongCtDieuKienTroCapCoDinh = listOldLuongCtDieuKienTroCapCoDinh
                                .Where(x => x.LuongCtLoaiTroCapCoDinhId == dataLoaiTroCap.LuongCtLoaiTroCapCoDinhId)
                                .ToList();

                            listRemoveLuongCtDieuKienTroCapCoDinh.AddRange(_listRemoveLuongCtDieuKienTroCapCoDinh);
                        }
                    });

                    context.LuongCtLoaiTroCapCoDinh.RemoveRange(listRemoveLuongCtLoaiTroCapCoDinh);
                    context.LuongCtDieuKienTroCapCoDinh.RemoveRange(listRemoveLuongCtDieuKienTroCapCoDinh);
                    context.SaveChanges();

                    #endregion
                }
            }
        }

        private void SaveLuongCt_TroCapKhac(int kyLuongId, Employee item, List<Category> listLoaiTroCapKhac)
        {
            var listOldLuongCtTroCapKhac = context.LuongCtTroCapKhac.Where(x => x.KyLuongId == kyLuongId).ToList();
            var listOldLuongCtLoaiTroCapKhac = context.LuongCtLoaiTroCapKhac.Where(x => x.KyLuongId == kyLuongId).ToList();
            var listOldLuongCtDieuKienTroCapKhac = context.LuongCtDieuKienTroCapKhac.Where(x => x.KyLuongId == kyLuongId).ToList();
            var listTroCapChucVu = context.TroCapChucVuMapping.ToList();
            var listTroCapLoaiHopDong = context.TroCapLoaiHopDongMapping.ToList();
            var listTroCapDieuKienHuong = context.TroCapDieuKienHuongMapping.ToList();
            var listHopDongNhanSu = context.HopDongNhanSu.Where(x => x.EmployeeId == item.EmployeeId).ToList();
            var hopDongMoiNhat = listHopDongNhanSu.Where(x => x.EmployeeId == item.EmployeeId)
                .OrderByDescending(z => z.NgayBatDauLamViec).FirstOrDefault();
            var loaiHopDongType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LHDNS");
            var listLoaiHopDong = context.Category.Where(x => x.CategoryTypeId == loaiHopDongType.CategoryTypeId).ToList();
            var loaiHopDong = listLoaiHopDong.FirstOrDefault(x => x.CategoryId == hopDongMoiNhat?.LoaiHopDongId);

            #region Trợ cấp khác

            var existsTroCapKhac = listOldLuongCtTroCapKhac.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
            var listTroCapKhac = context.TroCap.Where(x => x.TypeId == 4).ToList();

            //Create
            if (existsTroCapKhac == null)
            {
                var luongCtTroCapKhac = new LuongCtTroCapKhac();
                luongCtTroCapKhac.KyLuongId = kyLuongId;
                luongCtTroCapKhac.EmployeeId = item.EmployeeId;
                luongCtTroCapKhac.OrganizationId = item.OrganizationId ?? Guid.Empty;
                luongCtTroCapKhac.PositionId = item.PositionId ?? Guid.Empty;
                luongCtTroCapKhac.LoaiHopDongId = loaiHopDong?.CategoryId;

                context.LuongCtTroCapKhac.Add(luongCtTroCapKhac);
                context.SaveChanges();

                for (int i = 0; i < listLoaiTroCapKhac.Count; i++)
                {
                    var loaiTroCapKhac = listLoaiTroCapKhac[i];

                    var troCap = listTroCapKhac.FirstOrDefault(x => x.LoaiTroCapId == loaiTroCapKhac.CategoryId);

                    SaveLuongCt_LoaiTroCapKhac_DieuKienTroCapKhac(item.EmployeeId, troCap, listTroCapChucVu, listTroCapLoaiHopDong,
                        listTroCapDieuKienHuong, kyLuongId, luongCtTroCapKhac, item.PositionId,
                        loaiHopDong?.CategoryId, loaiTroCapKhac.CategoryId);
                }
            }
            //Update
            else
            {
                existsTroCapKhac.OrganizationId = item.OrganizationId ?? Guid.Empty;
                existsTroCapKhac.PositionId = item.PositionId ?? Guid.Empty;
                existsTroCapKhac.LoaiHopDongId = loaiHopDong?.CategoryId;

                context.LuongCtTroCapKhac.Update(existsTroCapKhac);
                context.SaveChanges();

                listOldLuongCtLoaiTroCapKhac = listOldLuongCtLoaiTroCapKhac
                    .Where(x => x.LuongCtTroCapKhacId == existsTroCapKhac.LuongCtTroCapKhacId).ToList();

                for (int i = 0; i < listLoaiTroCapKhac.Count; i++)
                {
                    var loaiTroCapKhac = listLoaiTroCapKhac[i];

                    var existsData = listOldLuongCtLoaiTroCapKhac.FirstOrDefault(x =>
                        x.LoaiTroCapId == loaiTroCapKhac.CategoryId);

                    //Nếu chưa có data loại trợ cấp
                    if (existsData == null)
                    {
                        var existsCauHinh = listTroCapKhac.FirstOrDefault(x => x.LoaiTroCapId == loaiTroCapKhac.CategoryId);

                        SaveLuongCt_LoaiTroCapKhac_DieuKienTroCapKhac(item.EmployeeId, existsCauHinh, listTroCapChucVu, listTroCapLoaiHopDong,
                            listTroCapDieuKienHuong, kyLuongId, existsTroCapKhac, item.PositionId,
                            loaiHopDong?.CategoryId, loaiTroCapKhac.CategoryId);
                    }
                    //Nếu có data loại trợ cấp
                    else
                    {
                        var existsLoaiTroCapCauHinh = listTroCapKhac.FirstOrDefault(x => x.LoaiTroCapId == loaiTroCapKhac.CategoryId);

                        //Nếu có loại trợ cấp trong cấu hình
                        if (existsLoaiTroCapCauHinh != null)
                        {
                            #region Kiểm tra Điều kiện hưởng trong cấu hình và trong bảng có thay đổi không?

                            var listTcDieuKienHuong =
                                listTroCapDieuKienHuong.Where(x => x.TroCapId == existsLoaiTroCapCauHinh.TroCapId).ToList();

                            var listDataDieuKienHuong = listOldLuongCtDieuKienTroCapKhac.Where(x =>
                                x.LuongCtLoaiTroCapKhacId == existsData.LuongCtLoaiTroCapKhacId).ToList();

                            listTcDieuKienHuong.ForEach(dieuKienHuong =>
                            {
                                var existsDataDieuKienHuong =
                                    listDataDieuKienHuong.FirstOrDefault(x => x.DieuKienHuongId == dieuKienHuong.DieuKienHuongId);

                                //Nếu điều kiện hưởng có trong cấu hình nhưng chưa có data trong bảng
                                if (existsDataDieuKienHuong == null)
                                {
                                    var luongCtDieuKienTroCapKhac = new LuongCtDieuKienTroCapKhac();
                                    luongCtDieuKienTroCapKhac.KyLuongId = kyLuongId;
                                    luongCtDieuKienTroCapKhac.LuongCtTroCapKhacId = existsData.LuongCtTroCapKhacId;
                                    luongCtDieuKienTroCapKhac.LuongCtLoaiTroCapKhacId = existsData.LuongCtLoaiTroCapKhacId;
                                    luongCtDieuKienTroCapKhac.DieuKienHuongId = dieuKienHuong.DieuKienHuongId;

                                    context.LuongCtDieuKienTroCapKhac.Add(luongCtDieuKienTroCapKhac);
                                    context.SaveChanges();

                                    SaveLuongCtEmpDktcKhac(item.EmployeeId, kyLuongId, dieuKienHuong.DieuKienHuongId);
                                }
                            });

                            var listRemove = new List<LuongCtDieuKienTroCapKhac>();
                            listDataDieuKienHuong.ForEach(dataDieuKienHuong =>
                            {
                                var existsDieuKienHuong = listTcDieuKienHuong.FirstOrDefault(x =>
                                    x.DieuKienHuongId == dataDieuKienHuong.DieuKienHuongId);

                                //Nếu có data nhưng không có trong cấu hình
                                if (existsDieuKienHuong == null)
                                {
                                    //Thêm vào list để xóa đi
                                    listRemove.Add(dataDieuKienHuong);
                                }
                            });

                            context.LuongCtDieuKienTroCapKhac.RemoveRange(listRemove);
                            context.SaveChanges();

                            #endregion

                            var listTcChucVu = listTroCapChucVu.Where(x => x.TroCapId == existsLoaiTroCapCauHinh.TroCapId).ToList();
                            var listChucVuId = listTcChucVu.Select(y => y.PositionId).ToList();
                            var listTcLoaiHopDong = listTroCapLoaiHopDong.Where(x => x.TroCapId == existsLoaiTroCapCauHinh.TroCapId).ToList();
                            var listLoaiHopDongId = listTcLoaiHopDong.Select(y => y.LoaiHopDongId).ToList();

                            //Nếu đủ điều kiện Chức vụ và Loại hợp đồng trong cấu hình
                            if (listChucVuId.Contains(item.PositionId ?? Guid.Empty) &&
                                listLoaiHopDongId.Contains(loaiHopDong?.CategoryId ?? Guid.Empty))
                            {
                                var listDieuKienHuongId = context.LuongCtDieuKienTroCapKhac.Where(x =>
                                        x.LuongCtLoaiTroCapKhacId == existsData.LuongCtLoaiTroCapKhacId)
                                    .Select(y => y.DieuKienHuongId).ToList();

                                var countFalse = context.LuongCtEmpDktcKhac.Count(x =>
                                    x.EmployeeId == item.EmployeeId &&
                                    x.KyLuongId == kyLuongId &&
                                    listDieuKienHuongId.Contains(x.DieuKienHuongId) &&
                                    x.DuDieuKien == false);

                                //Nếu tất cả điều kiện hưởng đều = true
                                if (countFalse == 0)
                                {
                                    existsData.MucTroCap = existsLoaiTroCapCauHinh.MucTroCap;
                                }
                                //Nếu có điều kiện hưởng = false
                                else
                                {
                                    existsData.MucTroCap = 0;
                                }
                            }
                            //Nếu không đủ điều kiện Chức vụ hoặc Loại hợp đồng trong cấu hình
                            else
                            {
                                existsData.MucTroCap = 0;

                                //Xóa điều kiện hưởng
                                var newListDieuKienHuong = context.LuongCtDieuKienTroCapKhac.Where(x =>
                                    x.KyLuongId == kyLuongId &&
                                    x.LuongCtLoaiTroCapKhacId == existsData.LuongCtLoaiTroCapKhacId).ToList();

                                context.LuongCtDieuKienTroCapKhac.RemoveRange(newListDieuKienHuong);
                            }

                            context.LuongCtLoaiTroCapKhac.Update(existsData);
                            context.SaveChanges();
                        }
                        //Nếu không có loại trợ cấp trong cấu hình
                        else
                        {
                            //Xóa điều kiện hưởng đã lưu nếu có
                            var listDataDieuKienHuong = listOldLuongCtDieuKienTroCapKhac.Where(x =>
                                x.LuongCtLoaiTroCapKhacId == existsData.LuongCtLoaiTroCapKhacId).ToList();
                            context.LuongCtDieuKienTroCapKhac.RemoveRange(listDataDieuKienHuong);
                            context.SaveChanges();

                            existsData.MucTroCap = 0;
                            context.LuongCtLoaiTroCapKhac.Update(existsData);
                            context.SaveChanges();
                        }
                    }
                }

                var listRemoveLuongCtLoaiTroCapKhac = new List<LuongCtLoaiTroCapKhac>();
                var listRemoveLuongCtDieuKienTroCapKhac = new List<LuongCtDieuKienTroCapKhac>();
                listOldLuongCtLoaiTroCapKhac.ForEach(loaiTroCapData =>
                {
                    var existsLoaiTroCap =
                        listLoaiTroCapKhac.FirstOrDefault(x => x.CategoryId == loaiTroCapData.LoaiTroCapId);

                    //Nếu có trong data nhưng không có trong cấu hình được chọn
                    if (existsLoaiTroCap == null)
                    {
                        listRemoveLuongCtLoaiTroCapKhac.Add(loaiTroCapData);

                        var LuongCtDieuKienTroCapKhac = listOldLuongCtDieuKienTroCapKhac.Where(x =>
                            x.LuongCtLoaiTroCapKhacId == loaiTroCapData.LuongCtLoaiTroCapKhacId).ToList();
                        listRemoveLuongCtDieuKienTroCapKhac.AddRange(LuongCtDieuKienTroCapKhac);
                    }
                });

                context.LuongCtLoaiTroCapKhac.RemoveRange(listRemoveLuongCtLoaiTroCapKhac);
                context.LuongCtDieuKienTroCapKhac.RemoveRange(listRemoveLuongCtDieuKienTroCapKhac);
                context.SaveChanges();
            }

            #endregion
        }

        private void SaveLuongCt_LoaiTroCapKhac_DieuKienTroCapKhac(Guid employeeId, TroCap troCap, List<TroCapChucVuMapping> listTroCapChucVu, 
            List<TroCapLoaiHopDongMapping> listTroCapLoaiHopDong, List<TroCapDieuKienHuongMapping> listTroCapDieuKienHuong,
            int kyLuongId, LuongCtTroCapKhac luongCtTroCapKhac, Guid? positionId, Guid? loaiHopDongId, Guid loaiTroCapId)
        {
            //Nếu loại trợ cấp có cấu hình
            if (troCap != null)
            {
                var listTcChucVu = listTroCapChucVu.Where(x => x.TroCapId == troCap.TroCapId).ToList();
                var listChucVuId = listTcChucVu.Select(y => y.PositionId).ToList();
                var listTcLoaiHopDong = listTroCapLoaiHopDong.Where(x => x.TroCapId == troCap.TroCapId).ToList();
                var listLoaiHopDongId = listTcLoaiHopDong.Select(y => y.LoaiHopDongId).ToList();
                var listTcDieuKienHuong =
                    listTroCapDieuKienHuong.Where(x => x.TroCapId == troCap.TroCapId).ToList();

                var luongCtLoaiTroCapKhac = new LuongCtLoaiTroCapKhac();
                luongCtLoaiTroCapKhac.KyLuongId = kyLuongId;
                luongCtLoaiTroCapKhac.LuongCtTroCapKhacId = luongCtTroCapKhac.LuongCtTroCapKhacId;
                luongCtLoaiTroCapKhac.LoaiTroCapId = troCap.LoaiTroCapId;
                luongCtLoaiTroCapKhac.IsEdit = false;

                if (listChucVuId.Contains(positionId ?? Guid.Empty) &&
                    listLoaiHopDongId.Contains(loaiHopDongId ?? Guid.Empty))
                {
                    luongCtLoaiTroCapKhac.MucTroCap = troCap.MucTroCap;

                    context.LuongCtLoaiTroCapKhac.Add(luongCtLoaiTroCapKhac);
                    context.SaveChanges();

                    listTcDieuKienHuong.ForEach(dieuKienHuong =>
                    {
                        var luongCtDieuKienTroCapKhac = new LuongCtDieuKienTroCapKhac();
                        luongCtDieuKienTroCapKhac.KyLuongId = kyLuongId;
                        luongCtDieuKienTroCapKhac.LuongCtTroCapKhacId = luongCtTroCapKhac.LuongCtTroCapKhacId;
                        luongCtDieuKienTroCapKhac.LuongCtLoaiTroCapKhacId = luongCtLoaiTroCapKhac.LuongCtLoaiTroCapKhacId;
                        luongCtDieuKienTroCapKhac.DieuKienHuongId = dieuKienHuong.DieuKienHuongId;

                        context.LuongCtDieuKienTroCapKhac.Add(luongCtDieuKienTroCapKhac);
                        context.SaveChanges();

                        SaveLuongCtEmpDktcKhac(employeeId, kyLuongId, dieuKienHuong.DieuKienHuongId);
                    });
                }
                //Nếu không thỏa mãn điều kiện Chức vụ hoặc Hợp đồng
                else
                {
                    luongCtLoaiTroCapKhac.MucTroCap = 0;

                    context.LuongCtLoaiTroCapKhac.Add(luongCtLoaiTroCapKhac);
                    context.SaveChanges();
                }
            }
            //Nếu loại trợ cấp không có cấu hình
            else
            {
                var luongCtLoaiTroCapKhac = new LuongCtLoaiTroCapKhac();
                luongCtLoaiTroCapKhac.KyLuongId = kyLuongId;
                luongCtLoaiTroCapKhac.LuongCtTroCapKhacId = luongCtTroCapKhac.LuongCtTroCapKhacId;
                luongCtLoaiTroCapKhac.LoaiTroCapId = loaiTroCapId;
                luongCtLoaiTroCapKhac.MucTroCap = 0;
                luongCtLoaiTroCapKhac.IsEdit = true;

                context.LuongCtLoaiTroCapKhac.Add(luongCtLoaiTroCapKhac);
                context.SaveChanges();
            }
        }

        private void SaveLuongCtEmpDktcKhac(Guid employeeId, int kyLuongId, Guid dieuKienHuongId, bool? duDieuKien = null)
        {
            var exists = context.LuongCtEmpDktcKhac.FirstOrDefault(x => x.EmployeeId == employeeId &&
                                                                        x.KyLuongId == kyLuongId &&
                                                                        x.DieuKienHuongId == dieuKienHuongId);

            //Update
            if (exists != null && duDieuKien != null)
            {
                exists.DuDieuKien = duDieuKien.Value;

                context.LuongCtEmpDktcKhac.Update(exists);
                context.SaveChanges();
            }
            //Create
            else if (exists == null)
            {
                var luongCtEmpDktcKhac = new LuongCtEmpDktcKhac();
                luongCtEmpDktcKhac.EmployeeId = employeeId;
                luongCtEmpDktcKhac.KyLuongId = kyLuongId;
                luongCtEmpDktcKhac.DieuKienHuongId = dieuKienHuongId;
                luongCtEmpDktcKhac.DuDieuKien = true;

                context.LuongCtEmpDktcKhac.Add(luongCtEmpDktcKhac);
                context.SaveChanges();
            }
        }

        private decimal TinhSoTienTroCapDmvsDuocHuong(decimal mucTroCap, MucHuongDmvs mucHuong, int soLanDmvs)
        {
            decimal result = 0;

            //Nếu trừ theo số lần
            if (mucHuong.HinhThucTru == 1)
            {
                result = mucHuong.MucTru * soLanDmvs;
            }
            //Nếu trừ theo phần trăm mức trợ cấp
            else if (mucHuong.HinhThucTru == 2)
            {
                result = mucHuong.MucTru * mucTroCap / 100;
            }

            return mucTroCap - Math.Round(result, 0);
        }

        private void SaveLuongCt_TroCapOt(int kyLuongId, decimal soNgayLamViec, DateTime ngayBatDau, DateTime ngayKetThuc)
        {
            #region Xóa dữ liệu cũ

            var listOldLuongCtTroCapOt = context.LuongCtTroCapOt.Where(x => x.KyLuongId == kyLuongId).ToList();
            context.LuongCtTroCapOt.RemoveRange(listOldLuongCtTroCapOt);
            var listOldLuongCtLoaiTroCapOt = context.LuongCtLoaiTroCapOt.Where(x => x.KyLuongId == kyLuongId).ToList();
            context.LuongCtLoaiTroCapOt.RemoveRange(listOldLuongCtLoaiTroCapOt);

            context.SaveChanges();

            #endregion

            var listTongHopChamCongOt = context.TongHopChamCongOt.Where(x => x.NgayChamCong.Date >= ngayBatDau.Date &&
                                                                             x.NgayChamCong.Date <= ngayKetThuc.Date).ToList();
            var listEmpId = listTongHopChamCongOt.Select(y => y.EmployeeId).Distinct().ToList();
            var listEmp = context.Employee.Where(x => listEmpId.Contains(x.EmployeeId)).OrderBy(z => z.EmployeeCode).ToList();
            var listCauHinhOt = context.CauHinhOt.ToList();

            for (int e = 0; e < listEmp.Count; e++)
            {
                var item = listEmp[e];

                decimal mucLuongHienTai = CommonHelper.GetMucLuongHienTaiByEmployeeId(context, item.EmployeeId);

                var luongCtTroCapOt = new LuongCtTroCapOt();
                luongCtTroCapOt.KyLuongId = kyLuongId;
                luongCtTroCapOt.EmployeeId = item.EmployeeId;
                luongCtTroCapOt.OrganizationId = item.OrganizationId ?? Guid.Empty;
                luongCtTroCapOt.PositionId = item.PositionId ?? Guid.Empty;
                luongCtTroCapOt.MucLuongHienTai = Math.Round(mucLuongHienTai / soNgayLamViec / 8, 0);

                context.LuongCtTroCapOt.Add(luongCtTroCapOt);
                context.SaveChanges();

                var listTongHopChamCongOtByEmp =
                    listTongHopChamCongOt.Where(x => x.EmployeeId == item.EmployeeId).ToList();

                var listLoaiOtId = listTongHopChamCongOtByEmp.Select(y => y.LoaiOtId).Distinct().ToList();

                listLoaiOtId.ForEach(loaiOtId =>
                {
                    var cauHinhTheoLoaiOt = listCauHinhOt.FirstOrDefault(x => x.LoaiOtId == loaiOtId);
                    decimal phanTram = cauHinhTheoLoaiOt?.TyLeHuong ?? 0;
                    decimal soGioOt = listTongHopChamCongOtByEmp.Where(x => x.LoaiOtId == loaiOtId).Sum(s => s.SoGio);

                    var luongCtLoaiTroCapOt = new LuongCtLoaiTroCapOt();
                    luongCtLoaiTroCapOt.LuongCtTroCapOtId = luongCtTroCapOt.LuongCtTroCapOtId;
                    luongCtLoaiTroCapOt.KyLuongId = kyLuongId;
                    luongCtLoaiTroCapOt.LoaiOtId = loaiOtId;
                    luongCtLoaiTroCapOt.MucTroCap = Math.Round(luongCtTroCapOt.MucLuongHienTai * phanTram / 100, 0);
                    luongCtLoaiTroCapOt.SoGioOt = soGioOt;

                    context.LuongCtLoaiTroCapOt.Add(luongCtLoaiTroCapOt);
                    context.SaveChanges();
                });
            }
        }

        private void SaveLuongCt_ThuNhapTinhThue(int kyLuongId, DateTime ngayBatDau, DateTime ngayKetThuc)
        {
            #region Dữ liệu cũ

            var listOld = context.LuongCtThuNhapTinhThue.Where(x => x.KyLuongId == kyLuongId).ToList();

            #endregion

            var listChamCong = context.ChamCong.Where(x => x.NgayChamCong.Date >= ngayBatDau.Date &&
                                                           x.NgayChamCong.Date <= ngayKetThuc.Date).ToList();
            var listEmpId = listChamCong.Select(y => y.EmployeeId).Distinct().ToList();
            var listEmp = context.Employee.Join(context.User,
                    emp => emp.EmployeeId,
                    user => user.EmployeeId,
                    (emp, user) => new {Emp = emp, User = user})
                .Where(x => listEmpId.Contains(x.Emp.EmployeeId) &&
                            x.User.IsAdmin != true).OrderBy(z => z.Emp.EmployeeCode).ToList();

            listEmp.ForEach(item =>
            {
                var exists = listOld.FirstOrDefault(x => x.EmployeeId == item.Emp.EmployeeId);

                //Create
                if (exists == null)
                {
                    var luongCtThuNhapTinhThue = new LuongCtThuNhapTinhThue();
                    luongCtThuNhapTinhThue.KyLuongId = kyLuongId;
                    luongCtThuNhapTinhThue.EmployeeId = item.Emp.EmployeeId;
                    luongCtThuNhapTinhThue.OrganizationId = item.Emp.OrganizationId ?? Guid.Empty;
                    luongCtThuNhapTinhThue.PositionId = item.Emp.PositionId ?? Guid.Empty;
                    luongCtThuNhapTinhThue.NetToGross = 0;
                    luongCtThuNhapTinhThue.Month13 = 0;
                    luongCtThuNhapTinhThue.Gift = 0;
                    luongCtThuNhapTinhThue.Other = 0;

                    context.LuongCtThuNhapTinhThue.Add(luongCtThuNhapTinhThue);
                }
                //Update
                else
                {
                    exists.OrganizationId = item.Emp.OrganizationId ?? Guid.Empty;
                    exists.PositionId = item.Emp.PositionId ?? Guid.Empty;

                    context.LuongCtThuNhapTinhThue.Update(exists);
                }

                context.SaveChanges();
            });
        }

        private void SaveLuongCt_BaoHiem(int kyLuongId, DateTime ngayBatDau, DateTime ngayKetThuc)
        {
            #region Dữ liệu cũ

            var listOld = context.LuongCtBaoHiem.Where(x => x.KyLuongId == kyLuongId).ToList();

            #endregion

            var listChamCong = context.ChamCong.Where(x => x.NgayChamCong.Date >= ngayBatDau.Date &&
                                                           x.NgayChamCong.Date <= ngayKetThuc.Date).ToList();
            var listEmpId = listChamCong.Select(y => y.EmployeeId).Distinct().ToList();
            var listEmp = context.Employee.Join(context.User,
                    emp => emp.EmployeeId,
                    user => user.EmployeeId,
                    (emp, user) => new { Emp = emp, User = user })
                .Where(x => listEmpId.Contains(x.Emp.EmployeeId) &&
                            x.User.IsAdmin != true).OrderBy(z => z.Emp.EmployeeCode).ToList();
            var listTongHopChamCong = context.TongHopChamCong.Where(x => x.KyLuongId == kyLuongId).ToList();
            var loaiHopDongType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LHDNS");
            var listLoaiHopDong = context.Category.Where(x => x.CategoryTypeId == loaiHopDongType.CategoryTypeId).ToList();
            var listHopDongNhanSu = context.HopDongNhanSu.Where(x => listEmpId.Contains(x.EmployeeId)).ToList();
            var cauHinhBaoHiem = context.CauHinhBaoHiem.FirstOrDefault();

            listEmp.ForEach(item =>
            {
                var ngayLamViecThucTe = listTongHopChamCong.FirstOrDefault(x => x.EmployeeId == item.Emp.EmployeeId)
                                            ?.NgayLamViecThucTe ?? 0;
                decimal mucLuongHienTai = CommonHelper.GetMucLuongHienTaiByEmployeeId(context, item.Emp.EmployeeId);
                var hopDongMoiNhat = listHopDongNhanSu.Where(x => x.EmployeeId == item.Emp.EmployeeId)
                    .OrderByDescending(z => z.NgayBatDauLamViec).FirstOrDefault();
                var loaiHopDong = listLoaiHopDong.FirstOrDefault(x => x.CategoryId == hopDongMoiNhat?.LoaiHopDongId);

                var luongCtBaoHiem = new LuongCtBaoHiem();
                luongCtBaoHiem.KyLuongId = kyLuongId;
                luongCtBaoHiem.EmployeeId = item.Emp.EmployeeId;
                luongCtBaoHiem.OrganizationId = item.Emp.OrganizationId ?? Guid.Empty;
                luongCtBaoHiem.PositionId = item.Emp.PositionId ?? Guid.Empty;

                #region Lương cơ bản đóng BHXH

                decimal baseBhxh = 0;
                if (loaiHopDong != null)
                {
                    if (ngayLamViecThucTe <= 14 || !loaiHopDong.CategoryCode.Contains("HĐLĐ"))
                    {
                        baseBhxh = 0;
                    }
                    else if (ngayLamViecThucTe > 14 && loaiHopDong.CategoryCode.Contains("HĐLĐ"))
                    {
                        if (cauHinhBaoHiem != null && mucLuongHienTai >= cauHinhBaoHiem.MucDongToiDa)
                        {
                            baseBhxh = cauHinhBaoHiem.MucDongToiDa;
                        }
                        else
                        {
                            baseBhxh = mucLuongHienTai;
                        }
                    }
                }

                luongCtBaoHiem.BaseBhxh = baseBhxh;

                #endregion

                #region Lương đóng BHXH

                decimal bhxh = 0;
                if (cauHinhBaoHiem != null)
                {
                    bhxh = luongCtBaoHiem.BaseBhxh * cauHinhBaoHiem.TiLePhanBoMucDongBhxhcuaNld / 100;
                }

                luongCtBaoHiem.Bhxh = bhxh;

                #endregion

                #region Lương đóng BHYT

                decimal bhyt = 0;
                if (cauHinhBaoHiem != null)
                {
                    bhyt = luongCtBaoHiem.BaseBhxh * cauHinhBaoHiem.TiLePhanBoMucDongBhytcuaNld / 100;
                }

                luongCtBaoHiem.Bhyt = bhyt;

                #endregion

                #region Lương đóng BHTN

                decimal bhtn = 0;
                if (cauHinhBaoHiem != null)
                {
                    bhtn = luongCtBaoHiem.BaseBhxh * cauHinhBaoHiem.TiLePhanBoMucDongBhtncuaNld / 100;
                }

                luongCtBaoHiem.Bhtn = bhtn;

                #endregion

                #region Lương đóng BHTNNN

                decimal bhtnnn = 0;
                if (cauHinhBaoHiem != null)
                {
                    bhtnnn = luongCtBaoHiem.BaseBhxh * cauHinhBaoHiem.TiLePhanBoMucDongBhtnnncuaNld / 100;
                }

                luongCtBaoHiem.Bhtnnn = bhtnnn;

                #endregion

                luongCtBaoHiem.Other = 0;

                var exists = listOld.FirstOrDefault(x => x.EmployeeId == item.Emp.EmployeeId);
                //Create
                if (exists == null)
                {
                    context.LuongCtBaoHiem.Add(luongCtBaoHiem);
                }
                //Update
                else
                {
                    exists.OrganizationId = item.Emp.OrganizationId ?? Guid.Empty;
                    exists.PositionId = item.Emp.PositionId ?? Guid.Empty;
                    exists.BaseBhxh = baseBhxh;
                    exists.Bhxh = bhxh;
                    exists.Bhyt = bhyt;
                    exists.Bhtn = bhtn;
                    exists.Bhtnnn = bhtnnn;

                    context.LuongCtBaoHiem.Update(exists);
                }

                context.SaveChanges();
            });
        }

        private void SaveLuongCt_GiamTruTruocThue(int kyLuongId, DateTime ngayBatDau, DateTime ngayKetThuc)
        {
            #region Dữ liệu cũ

            var listOld = context.LuongCtGiamTruTruocThue.Where(x => x.KyLuongId == kyLuongId).ToList();

            #endregion

            var listChamCong = context.ChamCong.Where(x => x.NgayChamCong.Date >= ngayBatDau.Date &&
                                                           x.NgayChamCong.Date <= ngayKetThuc.Date).ToList();
            var listEmpId = listChamCong.Select(y => y.EmployeeId).Distinct().ToList();
            var listEmp = context.Employee.Join(context.User,
                    emp => emp.EmployeeId,
                    user => user.EmployeeId,
                    (emp, user) => new { Emp = emp, User = user })
                .Where(x => listEmpId.Contains(x.Emp.EmployeeId) &&
                            x.User.IsAdmin != true).OrderBy(z => z.Emp.EmployeeCode).ToList();
            var cauHinhGiamTru = context.CauHinhGiamTru.ToList();
            var cauHinhGiamTruBanThan = cauHinhGiamTru.FirstOrDefault(x => x.LoaiGiamTruId == 1);
            decimal mucGiamTruBanThan = cauHinhGiamTruBanThan?.MucGiamTru ?? 0;
            var cauHinhGiamTruNguoiPhuThuoc = cauHinhGiamTru.FirstOrDefault(x => x.LoaiGiamTruId == 2);
            decimal mucGiamTruNguoiPhuThuoc = cauHinhGiamTruNguoiPhuThuoc?.MucGiamTru ?? 0;
            var listContact = context.Contact.Where(x => x.ObjectType == ContactObjectType.EMP_CON).ToList();
            var listLuongCtBaoHiem = context.LuongCtBaoHiem.Where(x => x.KyLuongId == kyLuongId).ToList();

            listEmp.ForEach(item =>
            {
                var luongCtGiamTruTruocThue = new LuongCtGiamTruTruocThue();
                luongCtGiamTruTruocThue.KyLuongId = kyLuongId;
                luongCtGiamTruTruocThue.EmployeeId = item.Emp.EmployeeId;
                luongCtGiamTruTruocThue.OrganizationId = item.Emp.OrganizationId ?? Guid.Empty;
                luongCtGiamTruTruocThue.PositionId = item.Emp.PositionId ?? Guid.Empty;
                luongCtGiamTruTruocThue.GiamTruCaNhan = mucGiamTruBanThan;
                luongCtGiamTruTruocThue.GiamTruKhac = 0;

                var listNguoiPhuThuoc = listContact.Where(x =>
                    x.ObjectId == item.Emp.EmployeeId && x.QuanHeId != null &&
                    x.PhuThuoc == true &&
                    x.PhuThuocTuNgay != null).ToList();

                decimal soNguoiPhuThuoc = 0;
                listNguoiPhuThuoc.ForEach(nguoiPhuThuoc =>
                {
                    if (nguoiPhuThuoc.PhuThuocDenNgay == null)
                    {
                        if (nguoiPhuThuoc.PhuThuocTuNgay.Value.Year < DateTime.Now.Year ||
                            (nguoiPhuThuoc.PhuThuocTuNgay.Value.Year == DateTime.Now.Year &&
                             nguoiPhuThuoc.PhuThuocTuNgay.Value.Month <= DateTime.Now.Month)) soNguoiPhuThuoc++;
                    }
                    else
                    {
                        var tuNgay = new DateTime(nguoiPhuThuoc.PhuThuocTuNgay.Value.Year,
                            nguoiPhuThuoc.PhuThuocTuNgay.Value.Month, 1);
                        var denNgay = new DateTime(nguoiPhuThuoc.PhuThuocDenNgay.Value.Year,
                            nguoiPhuThuoc.PhuThuocDenNgay.Value.Month, 1);
                        var ngayHienTai = new DateTime(DateTime.Now.Year,
                            DateTime.Now.Month, 1);

                        if (tuNgay.Ticks <= ngayHienTai.Ticks && ngayHienTai.Ticks <= denNgay.Ticks) soNguoiPhuThuoc++;
                    }
                });

                luongCtGiamTruTruocThue.SoNguoiPhuThuoc = (int) soNguoiPhuThuoc;
                decimal giamTruNguoiPhuThuoc = soNguoiPhuThuoc * mucGiamTruNguoiPhuThuoc;
                luongCtGiamTruTruocThue.GiamTruNguoiPhuThuoc = giamTruNguoiPhuThuoc;

                decimal tienDongBaoHiem = listLuongCtBaoHiem.Where(x => x.EmployeeId == item.Emp.EmployeeId)
                    .Sum(s => s.BaseBhxh + s.Bhxh + s.Bhyt + s.Bhtn + s.Bhtnnn + s.Other);
                luongCtGiamTruTruocThue.TienDongBaoHiem = tienDongBaoHiem;

                var exists = listOld.FirstOrDefault(x => x.EmployeeId == item.Emp.EmployeeId);

                //Create
                if (exists == null)
                {
                    context.LuongCtGiamTruTruocThue.Add(luongCtGiamTruTruocThue);
                }
                //Update
                else
                {
                    exists.OrganizationId = item.Emp.OrganizationId ?? Guid.Empty;
                    exists.PositionId = item.Emp.PositionId ?? Guid.Empty;
                    exists.GiamTruCaNhan = mucGiamTruBanThan;
                    exists.GiamTruNguoiPhuThuoc = giamTruNguoiPhuThuoc;
                    exists.SoNguoiPhuThuoc = (int) soNguoiPhuThuoc;
                    exists.TienDongBaoHiem = tienDongBaoHiem;

                    context.LuongCtGiamTruTruocThue.Update(exists);
                }

                context.SaveChanges();
            });
        }

        private void SaveLuongCt_GiamTruSauThue(int kyLuongId, DateTime ngayBatDau, DateTime ngayKetThuc)
        {
            #region Dữ liệu cũ

            var listOld = context.LuongCtGiamTruSauThue.Where(x => x.KyLuongId == kyLuongId).ToList();

            #endregion

            var listChamCong = context.ChamCong.Where(x => x.NgayChamCong.Date >= ngayBatDau.Date &&
                                                           x.NgayChamCong.Date <= ngayKetThuc.Date).ToList();
            var listEmpId = listChamCong.Select(y => y.EmployeeId).Distinct().ToList();
            var listEmp = context.Employee.Join(context.User,
                    emp => emp.EmployeeId,
                    user => user.EmployeeId,
                    (emp, user) => new { Emp = emp, User = user })
                .Where(x => listEmpId.Contains(x.Emp.EmployeeId) &&
                            x.User.IsAdmin != true).OrderBy(z => z.Emp.EmployeeCode).ToList();
            var listLuongCtBaoHiem = context.LuongCtBaoHiem.Where(x => x.KyLuongId == kyLuongId).ToList();
            var cauHinhPhiCongDoan = context.KinhPhiCongDoan.FirstOrDefault();
            var mucDongCuaNhanVien = cauHinhPhiCongDoan?.MucDongNhanVien ?? 0;

            listEmp.ForEach(item =>
            {
                decimal baseBhxh = listLuongCtBaoHiem.
                                   FirstOrDefault(x => x.EmployeeId == item.Emp.EmployeeId)?.BaseBhxh ?? 0;

                var luongCtGiamTruSauThue = new LuongCtGiamTruSauThue();
                luongCtGiamTruSauThue.KyLuongId = kyLuongId;
                luongCtGiamTruSauThue.EmployeeId = item.Emp.EmployeeId;
                luongCtGiamTruSauThue.OrganizationId = item.Emp.OrganizationId ?? Guid.Empty;
                luongCtGiamTruSauThue.PositionId = item.Emp.PositionId ?? Guid.Empty;

                decimal kinhPhiCongDoan = Math.Round(baseBhxh * mucDongCuaNhanVien / 100, 0);
                luongCtGiamTruSauThue.KinhPhiCongDoan = kinhPhiCongDoan;

                luongCtGiamTruSauThue.QuyetToanThueTncn = 0;
                luongCtGiamTruSauThue.Other = 0;

                var exists = listOld.FirstOrDefault(x => x.EmployeeId == item.Emp.EmployeeId);

                //Create
                if (exists == null)
                {
                    context.LuongCtGiamTruSauThue.Add(luongCtGiamTruSauThue);
                }
                //Update
                else
                {
                    exists.OrganizationId = item.Emp.OrganizationId ?? Guid.Empty;
                    exists.PositionId = item.Emp.PositionId ?? Guid.Empty;
                    exists.KinhPhiCongDoan = kinhPhiCongDoan;

                    context.LuongCtGiamTruSauThue.Update(exists);
                }

                context.SaveChanges();
            });
        }

        private void SaveLuongCt_HoanLaiSauThue(int kyLuongId, DateTime ngayBatDau, DateTime ngayKetThuc)
        {
            #region Dữ liệu cũ

            var listOld = context.LuongCtHoanLaiSauThue.Where(x => x.KyLuongId == kyLuongId).ToList();

            #endregion

            var listChamCong = context.ChamCong.Where(x => x.NgayChamCong.Date >= ngayBatDau.Date &&
                                                           x.NgayChamCong.Date <= ngayKetThuc.Date).ToList();
            var listEmpId = listChamCong.Select(y => y.EmployeeId).Distinct().ToList();
            var listEmp = context.Employee.Join(context.User,
                    emp => emp.EmployeeId,
                    user => user.EmployeeId,
                    (emp, user) => new { Emp = emp, User = user })
                .Where(x => listEmpId.Contains(x.Emp.EmployeeId) &&
                            x.User.IsAdmin != true).OrderBy(z => z.Emp.EmployeeCode).ToList();

            listEmp.ForEach(item =>
            {
                var exists = listOld.FirstOrDefault(x => x.EmployeeId == item.Emp.EmployeeId);

                //Create
                if (exists == null)
                {
                    var luongCtHoanLaiSauThue = new LuongCtHoanLaiSauThue();
                    luongCtHoanLaiSauThue.KyLuongId = kyLuongId;
                    luongCtHoanLaiSauThue.EmployeeId = item.Emp.EmployeeId;
                    luongCtHoanLaiSauThue.OrganizationId = item.Emp.OrganizationId ?? Guid.Empty;
                    luongCtHoanLaiSauThue.PositionId = item.Emp.PositionId ?? Guid.Empty;
                    luongCtHoanLaiSauThue.ThueTncn = 0;
                    luongCtHoanLaiSauThue.Other = 0;

                    context.LuongCtHoanLaiSauThue.Add(luongCtHoanLaiSauThue);
                }
                //Update
                else
                {
                    exists.OrganizationId = item.Emp.OrganizationId ?? Guid.Empty;
                    exists.PositionId = item.Emp.PositionId ?? Guid.Empty;

                    context.LuongCtHoanLaiSauThue.Update(exists);
                }

                context.SaveChanges();
            });
        }

        private void SaveLuongCt_CtyDong(int kyLuongId, DateTime ngayBatDau, DateTime ngayKetThuc)
        {
            #region Dữ liệu cũ

            var listOld = context.LuongCtCtyDong.Where(x => x.KyLuongId == kyLuongId).ToList();

            #endregion

            var listChamCong = context.ChamCong.Where(x => x.NgayChamCong.Date >= ngayBatDau.Date &&
                                                           x.NgayChamCong.Date <= ngayKetThuc.Date).ToList();
            var listEmpId = listChamCong.Select(y => y.EmployeeId).Distinct().ToList();
            var listEmp = context.Employee.Join(context.User,
                    emp => emp.EmployeeId,
                    user => user.EmployeeId,
                    (emp, user) => new { Emp = emp, User = user })
                .Where(x => listEmpId.Contains(x.Emp.EmployeeId) &&
                            x.User.IsAdmin != true).OrderBy(z => z.Emp.EmployeeCode).ToList();
            var listTongHopChamCong = context.TongHopChamCong.Where(x => x.KyLuongId == kyLuongId).ToList();
            var loaiHopDongType = context.CategoryType.FirstOrDefault(x => x.CategoryTypeCode == "LHDNS");
            var listLoaiHopDong = context.Category.Where(x => x.CategoryTypeId == loaiHopDongType.CategoryTypeId).ToList();
            var listHopDongNhanSu = context.HopDongNhanSu.Where(x => listEmpId.Contains(x.EmployeeId)).ToList();
            var cauHinhBaoHiem = context.CauHinhBaoHiem.FirstOrDefault();
            var cauHinhPhiCongDoan = context.KinhPhiCongDoan.FirstOrDefault();
            var mucDongCuaCty = cauHinhPhiCongDoan?.MucDongCongTy ?? 0;

            listEmp.ForEach(item =>
            {
                var exists = listOld.FirstOrDefault(x => x.EmployeeId == item.Emp.EmployeeId);

                var ngayLamViecThucTe = listTongHopChamCong.FirstOrDefault(x => x.EmployeeId == item.Emp.EmployeeId)
                                            ?.NgayLamViecThucTe ?? 0;
                decimal mucLuongHienTai = CommonHelper.GetMucLuongHienTaiByEmployeeId(context, item.Emp.EmployeeId);
                var hopDongMoiNhat = listHopDongNhanSu.Where(x => x.EmployeeId == item.Emp.EmployeeId)
                    .OrderByDescending(z => z.NgayBatDauLamViec).FirstOrDefault();
                var loaiHopDong = listLoaiHopDong.FirstOrDefault(x => x.CategoryId == hopDongMoiNhat?.LoaiHopDongId);

                var luongCtCtyDong = new LuongCtCtyDong();
                luongCtCtyDong.KyLuongId = kyLuongId;
                luongCtCtyDong.EmployeeId = item.Emp.EmployeeId;
                luongCtCtyDong.OrganizationId = item.Emp.OrganizationId ?? Guid.Empty;
                luongCtCtyDong.PositionId = item.Emp.OrganizationId ?? Guid.Empty;

                #region Lương cơ bản đóng BHXH

                decimal baseBhxh = 0;
                if (loaiHopDong != null)
                {
                    if (ngayLamViecThucTe <= 14 || !loaiHopDong.CategoryCode.Contains("HĐLĐ"))
                    {
                        baseBhxh = 0;
                    }
                    else if (ngayLamViecThucTe > 14 && loaiHopDong.CategoryCode.Contains("HĐLĐ"))
                    {
                        if (cauHinhBaoHiem != null && mucLuongHienTai >= cauHinhBaoHiem.MucDongToiDa)
                        {
                            baseBhxh = cauHinhBaoHiem.MucDongToiDa;
                        }
                        else
                        {
                            baseBhxh = mucLuongHienTai;
                        }
                    }
                }

                luongCtCtyDong.BaseBhxh = baseBhxh;

                #endregion

                #region Lương đóng BHXH

                decimal bhxh = 0;
                if (cauHinhBaoHiem != null)
                {
                    bhxh = luongCtCtyDong.BaseBhxh * cauHinhBaoHiem.TiLePhanBoMucDongBhxhcuaNsdld / 100;
                }

                luongCtCtyDong.Bhxh = bhxh;

                #endregion

                #region Lương đóng BHYT

                decimal bhyt = 0;
                if (cauHinhBaoHiem != null)
                {
                    bhyt = luongCtCtyDong.BaseBhxh * cauHinhBaoHiem.TiLePhanBoMucDongBhytcuaNsdld / 100;
                }

                luongCtCtyDong.Bhyt = bhyt;

                #endregion

                #region Lương đóng BHTN

                decimal bhtn = 0;
                if (cauHinhBaoHiem != null)
                {
                    bhtn = luongCtCtyDong.BaseBhxh * cauHinhBaoHiem.TiLePhanBoMucDongBhtncuaNsdld / 100;
                }

                luongCtCtyDong.Bhtn = bhtn;

                #endregion

                #region Lương đóng BHTNNN

                decimal bhtnnn = 0;
                if (cauHinhBaoHiem != null)
                {
                    bhtnnn = luongCtCtyDong.BaseBhxh * cauHinhBaoHiem.TiLePhanBoMucDongBhtnnncuaNsdld / 100;
                }

                luongCtCtyDong.Bhtnnn = bhtnnn;

                #endregion

                #region Kinh phí công đoàn

                decimal kinhPhiCongDoan = luongCtCtyDong.BaseBhxh * mucDongCuaCty / 100;
                luongCtCtyDong.KinhPhiCongDoan = kinhPhiCongDoan;

                #endregion

                luongCtCtyDong.Other = 0;
                luongCtCtyDong.FundOct = 0;

                //Create
                if (exists == null)
                {
                    context.LuongCtCtyDong.Add(luongCtCtyDong);
                }
                //Update
                else
                {
                    exists.OrganizationId = item.Emp.OrganizationId ?? Guid.Empty;
                    exists.PositionId = item.Emp.PositionId ?? Guid.Empty;
                    exists.BaseBhxh = baseBhxh;
                    exists.Bhxh = bhxh;
                    exists.Bhyt = bhyt;
                    exists.Bhtn = bhtn;
                    exists.Bhtnnn = bhtnnn;
                    exists.KinhPhiCongDoan = kinhPhiCongDoan;

                    context.LuongCtCtyDong.Update(exists);
                }

                context.SaveChanges();
            });
        }

        private void SaveLuongCt_Other(int kyLuongId, DateTime ngayBatDau, DateTime ngayKetThuc)
        {
            #region Dữ liệu cũ

            var listOld = context.LuongCtOther.Where(x => x.KyLuongId == kyLuongId).ToList();

            #endregion

            var listChamCong = context.ChamCong.Where(x => x.NgayChamCong.Date >= ngayBatDau.Date &&
                                                           x.NgayChamCong.Date <= ngayKetThuc.Date).ToList();
            var listEmpId = listChamCong.Select(y => y.EmployeeId).Distinct().ToList();
            var listEmp = context.Employee.Join(context.User,
                    emp => emp.EmployeeId,
                    user => user.EmployeeId,
                    (emp, user) => new { Emp = emp, User = user })
                .Where(x => listEmpId.Contains(x.Emp.EmployeeId) &&
                            x.User.IsAdmin != true).OrderBy(z => z.Emp.EmployeeCode).ToList();

            listEmp.ForEach(item =>
            {
                var exists = listOld.FirstOrDefault(x => x.EmployeeId == item.Emp.EmployeeId);

                if (exists == null)
                {
                    var luongCtOther = new LuongCtOther();
                    luongCtOther.KyLuongId = kyLuongId;
                    luongCtOther.EmployeeId = item.Emp.EmployeeId;
                    luongCtOther.OrganizationId = item.Emp.OrganizationId ?? Guid.Empty;
                    luongCtOther.PositionId = item.Emp.PositionId ?? Guid.Empty;
                    luongCtOther.KhoanBuTruThangTruoc = 0;
                    luongCtOther.TroCapKhacKhongTinhThue = 0;
                    luongCtOther.KhauTruHoanLaiTruocThue = 0;
                    luongCtOther.LuongTamUng = 0;

                    context.LuongCtOther.Add(luongCtOther);
                }
                else
                {
                    exists.OrganizationId = item.Emp.OrganizationId ?? Guid.Empty;
                    exists.PositionId = item.Emp.PositionId ?? Guid.Empty;

                    context.LuongCtOther.Update(exists);
                }

                context.SaveChanges();
            });
        }

        private void SaveLuongTongHop(int kyLuongId, decimal soNgayLamViec, DateTime ngayBatDau, DateTime ngayKetThuc)
        {
            #region Xóa dữ liệu cũ

            var listOld = context.LuongTongHop.Where(x => x.KyLuongId == kyLuongId).ToList();

            context.LuongTongHop.RemoveRange(listOld);
            context.SaveChanges();

            #endregion

            var listChamCong = context.ChamCong.Where(x => x.NgayChamCong.Date >= ngayBatDau.Date &&
                                                           x.NgayChamCong.Date <= ngayKetThuc.Date).ToList();
            var listEmpId = listChamCong.Select(y => y.EmployeeId).Distinct().ToList();
            var listEmp = context.Employee.Join(context.User,
                    emp => emp.EmployeeId,
                    user => user.EmployeeId,
                    (emp, user) => new { Emp = emp, User = user })
                .Where(x => listEmpId.Contains(x.Emp.EmployeeId) &&
                            x.User.IsAdmin != true).OrderBy(z => z.Emp.EmployeeCode).ToList();
            var listTongHopChamCong = context.TongHopChamCong.Where(x => x.KyLuongId == kyLuongId).ToList();
            var listLuongCtOther = context.LuongCtOther.Where(x => x.KyLuongId == kyLuongId).ToList();
            var listLuongCtTroCapOt = context.LuongCtTroCapOt.Where(x => x.KyLuongId == kyLuongId).ToList();
            var listLuongCtLoaiTroCapOt = context.LuongCtLoaiTroCapOt.Where(x => x.KyLuongId == kyLuongId).ToList();
            var listLuongCtTroCapCoDinh = context.LuongCtTroCapCoDinh.Where(x => x.KyLuongId == kyLuongId).ToList();
            var listLuongCtLoaiTroCapCoDinh = context.LuongCtLoaiTroCapCoDinh.Where(x => x.KyLuongId == kyLuongId).ToList();
            var listLuongCtTroCapKhac = context.LuongCtTroCapKhac.Where(x => x.KyLuongId == kyLuongId).ToList();
            var listLuongCtLoaiTroCapKhac = context.LuongCtLoaiTroCapKhac.Where(x => x.KyLuongId == kyLuongId).ToList();
            var listLuongCtGiamTruTruocThue = context.LuongCtGiamTruTruocThue.Where(x => x.KyLuongId == kyLuongId).ToList();
            var listLuongCtThuNhapTinhThue = context.LuongCtThuNhapTinhThue.Where(x => x.KyLuongId == kyLuongId).ToList();
            var listCauHinhThueTncn = context.CauHinhThueTncn.ToList();
            var cauHinhBaoHiem = context.CauHinhBaoHiem.FirstOrDefault();
            var listLuongCtBaoHiem = context.LuongCtBaoHiem.Where(x => x.KyLuongId == kyLuongId).ToList();
            var listLuongCtCtyDong = context.LuongCtCtyDong.Where(x => x.KyLuongId == kyLuongId).ToList();
            var listLuongCtGiamTruSauThue = context.LuongCtGiamTruSauThue.Where(x => x.KyLuongId == kyLuongId).ToList();
            var listLuongCtHoanLaiSauThue = context.LuongCtHoanLaiSauThue.Where(x => x.KyLuongId == kyLuongId).ToList();

            listEmp.ForEach(item =>
            {
                var tongHopChamCong = listTongHopChamCong.FirstOrDefault(x => x.EmployeeId == item.Emp.EmployeeId);

                var luongTongHop = new LuongTongHop();
                luongTongHop.KyLuongId = kyLuongId;
                luongTongHop.EmployeeId = item.Emp.EmployeeId;
                luongTongHop.OrganizationId = item.Emp.OrganizationId ?? Guid.Empty;
                luongTongHop.PositionId = item.Emp.PositionId ?? Guid.Empty;
                luongTongHop.SubCode1Value = item.Emp.SubCode1Value;
                luongTongHop.SubCode2Value = item.Emp.SubCode2Value;
                luongTongHop.CapBacId = item.Emp.CapBacId;

                #region Tổng ngày tính lương

                if (tongHopChamCong == null) luongTongHop.TongNgayTinhLuong = 0;
                else
                {
                    luongTongHop.TongNgayTinhLuong =
                        tongHopChamCong.NgayLamViecThucTe + tongHopChamCong.CongTac + tongHopChamCong.DaoTaoHoiThao +
                        tongHopChamCong.NghiPhep + tongHopChamCong.NghiLe + tongHopChamCong.NghiBu +
                        tongHopChamCong.NghiCheDo + tongHopChamCong.NghiHuongLuongKhac - 
                        tongHopChamCong.TongNgayDmvs;
                }

                #endregion

                #region Mức lương cũ

                luongTongHop.MucLuongCu =
                    CommonHelper.GetMucLuongCuByEmployeeId(context, item.Emp.EmployeeId, ngayBatDau);

                #endregion

                #region Số ngày tính theo mức lương cũ

                luongTongHop.SoNgayMucLuongCu =
                    CommonHelper.GetSoNgayTinhTheoMucLuongCu(context, item.Emp.EmployeeId, ngayBatDau);

                #endregion

                #region Mức lương hiện tại

                luongTongHop.MucLuongHienTai =
                    CommonHelper.GetMucLuongHienTaiByEmployeeId(context, item.Emp.EmployeeId);

                #endregion

                #region Lương thực tế

                luongTongHop.LuongThucTe =
                    luongTongHop.MucLuongHienTai / soNgayLamViec * (luongTongHop.TongNgayTinhLuong - luongTongHop.SoNgayMucLuongCu) +
                    luongTongHop.MucLuongCu * luongTongHop.SoNgayMucLuongCu;

                #endregion

                #region Khoản bù trừ tháng trước

                var luongCtOther = listLuongCtOther.FirstOrDefault(x => x.EmployeeId == item.Emp.EmployeeId);
                luongTongHop.KhoanBuTruThangTruoc = luongCtOther?.KhoanBuTruThangTruoc ?? 0;

                #endregion

                #region Lương OT tính thuế

                var luongCtTroCapOt = listLuongCtTroCapOt.FirstOrDefault(x => x.EmployeeId == item.Emp.EmployeeId);
                if (luongCtTroCapOt == null) luongTongHop.LuongOtTinhThue = 0;
                else
                {
                    var _listLuongCtLoaiTroCapOt = listLuongCtLoaiTroCapOt
                        .Where(x => x.LuongCtTroCapOtId == luongCtTroCapOt.LuongCtTroCapOtId).ToList();

                    luongTongHop.LuongOtTinhThue = LuongOtTinhThue(luongCtTroCapOt, _listLuongCtLoaiTroCapOt);
                }

                #endregion

                #region Lương OT không tính thuế

                if (luongCtTroCapOt == null) luongTongHop.LuongOtKhongTinhThue = 0;
                else 
                {
                    var _listLuongCtLoaiTroCapOt = listLuongCtLoaiTroCapOt
                        .Where(x => x.LuongCtTroCapOtId == luongCtTroCapOt.LuongCtTroCapOtId).ToList();

                    var tongLuongOt = TongLuongOt(_listLuongCtLoaiTroCapOt);
                    luongTongHop.LuongOtKhongTinhThue = tongLuongOt - luongTongHop.LuongOtTinhThue;
                }

                #endregion

                #region Tổng trợ cấp cố định

                var luongCtTroCapCoDinh =
                    listLuongCtTroCapCoDinh.FirstOrDefault(x => x.EmployeeId == item.Emp.EmployeeId);
                if (luongCtTroCapCoDinh == null) luongTongHop.TongTroCapCoDinh = 0;
                else
                {
                    var _listLuongCtLoaiTroCapCoDinh = listLuongCtLoaiTroCapCoDinh
                        .Where(x => x.LuongCtTroCapCoDinhId == luongCtTroCapCoDinh.LuongCtTroCapCoDinhId).ToList();
                    luongTongHop.TongTroCapCoDinh = TongTroCapCoDinh(_listLuongCtLoaiTroCapCoDinh);
                }

                #endregion

                #region Tổng trợ cấp khác

                var luongCtTroCapKhac =
                    listLuongCtTroCapKhac.FirstOrDefault(x => x.EmployeeId == item.Emp.EmployeeId);
                if (luongCtTroCapKhac == null) luongTongHop.TongTroCapKhac = 0;
                else
                {
                    var _listLuongCtLoaiTroCapKhac = listLuongCtLoaiTroCapKhac
                        .Where(x => x.LuongCtTroCapKhacId == luongCtTroCapKhac.LuongCtTroCapKhacId).ToList();
                    luongTongHop.TongTroCapKhac = TongTroCapKhac(_listLuongCtLoaiTroCapKhac);
                }

                #endregion

                #region Trợ cấp khác không tính thuế

                luongTongHop.TroCapKhacKhongTinhThue = luongCtOther?.TroCapKhacKhongTinhThue ?? 0;

                #endregion

                #region Khấu trừ/Hoàn lại trước thuế

                luongTongHop.KhauTruHoanLaiTruocThue = luongCtOther?.KhauTruHoanLaiTruocThue ?? 0;

                #endregion

                #region Tổng thu nhập bao gồm cả các khoản tính thuế và không tính thuế

                luongTongHop.TongThuNhapBaoGomThueVaKhongThue = luongTongHop.LuongThucTe + luongTongHop.KhoanBuTruThangTruoc + luongTongHop.LuongOtTinhThue +
                                                                luongTongHop.LuongOtKhongTinhThue + luongTongHop.TongTroCapCoDinh + luongTongHop.TongTroCapKhac +
                                                                luongTongHop.TroCapKhacKhongTinhThue - luongTongHop.KhauTruHoanLaiTruocThue;

                #endregion

                #region Tổng thu nhập sau khi đã loại bỏ các khoản không chịu thuế

                luongTongHop.TongThuNhapSauKhiBoCacKhoanKhongTinhThue =
                    (luongTongHop.TongThuNhapBaoGomThueVaKhongThue - luongTongHop.TroCapKhacKhongTinhThue -
                     luongTongHop.LuongOtKhongTinhThue) < 0
                        ? 0
                        : (luongTongHop.TongThuNhapBaoGomThueVaKhongThue - luongTongHop.TroCapKhacKhongTinhThue -
                           luongTongHop.LuongOtKhongTinhThue);

                #endregion

                #region Giảm trừ trước thuế

                var luongCtGiamTruTruocThue =
                    listLuongCtGiamTruTruocThue.FirstOrDefault(x => x.EmployeeId == item.Emp.EmployeeId);
                luongTongHop.GiamTruTruocThue = TongGiamTruTruocThue(luongCtGiamTruTruocThue);

                #endregion

                #region Thu nhập chỉ đưa vào tính thuế

                var luongCtThuNhapTinhThue =
                    listLuongCtThuNhapTinhThue.FirstOrDefault(x => x.EmployeeId == item.Emp.EmployeeId);
                luongTongHop.ThuNhapChiDuaVaoTinhThue = TongThuNhapChiDuaVaoTinhThue(luongCtThuNhapTinhThue);

                #endregion

                #region Tổng thu nhập chịu thuế sau giảm trừ

                luongTongHop.TongThuNhapChiuThueSauGiamTru = luongTongHop.TongThuNhapSauKhiBoCacKhoanKhongTinhThue - luongTongHop.GiamTruTruocThue +
                                                             luongTongHop.ThuNhapChiDuaVaoTinhThue;

                #endregion

                #region Tổng thuế TNCN phải trả bởi công ty và NLĐ

                if (listCauHinhThueTncn.Count == 0) luongTongHop.TongThueTncnCtyVaNld = 0;
                else
                {
                    var cauHinh = listCauHinhThueTncn.FirstOrDefault(x =>
                        x.SoTienTu < luongTongHop.TongThuNhapChiuThueSauGiamTru &&
                        x.SoTienDen >= luongTongHop.TongThuNhapChiuThueSauGiamTru);

                    if (cauHinh == null) luongTongHop.TongThueTncnCtyVaNld = 0;
                    else
                    {
                        luongTongHop.TongThueTncnCtyVaNld =
                            Math.Round(luongTongHop.TongThuNhapChiuThueSauGiamTru * cauHinh.PhanTramThue / 100 -
                                       cauHinh.SoBiTruTheoCongThuc, 0);
                    }
                }

                #endregion

                #region Thuế TNCN NLĐ phải trả

                if (listCauHinhThueTncn.Count == 0) luongTongHop.ThueTncnNld = 0;
                else
                {
                    var cauHinh = listCauHinhThueTncn.FirstOrDefault(x =>
                        x.SoTienTu < luongTongHop.TongThuNhapChiuThueSauGiamTru &&
                        x.SoTienDen >= luongTongHop.TongThuNhapChiuThueSauGiamTru);

                    if (cauHinh == null) luongTongHop.ThueTncnNld = 0;
                    else
                    {
                        decimal gross = luongCtThuNhapTinhThue?.NetToGross ?? 0;
                        luongTongHop.ThueTncnNld =
                            Math.Round((luongTongHop.TongThuNhapChiuThueSauGiamTru - gross) * cauHinh.PhanTramThue / 100 -
                                       cauHinh.SoBiTruTheoCongThuc, 0);
                    }
                }

                #endregion

                #region Lương cơ bản đóng BHXH, BHYT, BHNN, KPCĐ

                if (tongHopChamCong == null) luongTongHop.LuongCoBanDongBh = 0;
                else
                {
                    if (tongHopChamCong.NghiKhongPhep + tongHopChamCong.NghiHuongBhxh +
                        tongHopChamCong.TongNgayDmvs > 14)
                    {
                        luongTongHop.LuongCoBanDongBh = 0;
                    }
                    else
                    {
                        if (cauHinhBaoHiem == null)
                        {
                            luongTongHop.LuongCoBanDongBh = 0;
                        }
                        else if (luongTongHop.MucLuongHienTai >= cauHinhBaoHiem.MucDongToiDa)
                        {
                            luongTongHop.LuongCoBanDongBh = cauHinhBaoHiem.MucDongToiDa;
                        }
                        else if (luongTongHop.MucLuongHienTai < cauHinhBaoHiem.MucDongToiDa)
                        {
                            luongTongHop.LuongCoBanDongBh = luongTongHop.MucLuongHienTai;
                        }
                    }
                }

                #endregion

                #region Tổng tiền BH NLĐ phải đóng

                var luongCtBaoHiem = listLuongCtBaoHiem.FirstOrDefault(x => x.EmployeeId == item.Emp.EmployeeId);
                luongTongHop.TongTienBhNldPhaiDong = TongTienBaoHiem(luongCtBaoHiem);

                #endregion

                #region Tổng tiền BH Công ty phải đóng

                var luongCtCtyDong = listLuongCtCtyDong.FirstOrDefault(x => x.EmployeeId == item.Emp.EmployeeId);
                if (luongCtCtyDong == null) luongTongHop.TongTienBhCtyPhaiDong = 0;
                else
                {
                    luongTongHop.TongTienBhCtyPhaiDong = luongCtCtyDong.Bhxh + luongCtCtyDong.Bhyt + luongCtCtyDong.Bhtn + luongCtCtyDong.Bhtnnn +
                                                         luongCtCtyDong.Other;
                }

                #endregion

                #region Thu nhập thực nhận trước khi bù trừ thuế nếu có

                luongTongHop.ThuNhapThucNhanTruocKhiBuTruThue =
                    luongTongHop.TongThuNhapBaoGomThueVaKhongThue - luongTongHop.ThueTncnNld -
                    luongTongHop.TongTienBhNldPhaiDong;

                #endregion

                #region Các khoản giảm trừ sau thuế

                var luongCtGiamTruSauThue =
                    listLuongCtGiamTruSauThue.FirstOrDefault(x => x.EmployeeId == item.Emp.EmployeeId);
                luongTongHop.CacKhoanGiamTruSauThue = TongGiamTruSauThue(luongCtGiamTruSauThue);

                #endregion

                #region Các khoản hoàn lại sau thuế

                var luongCtHoanLaiSauThue =
                    listLuongCtHoanLaiSauThue.FirstOrDefault(x => x.EmployeeId == item.Emp.EmployeeId);
                luongTongHop.CacKhoanHoanLaiSauThue = TongCacKhoanHoanLaiSauThue(luongCtHoanLaiSauThue);

                #endregion

                #region Thu nhập thực nhận

                luongTongHop.ThuNhapThucNhan = luongTongHop.ThuNhapThucNhanTruocKhiBuTruThue -
                                               luongTongHop.CacKhoanGiamTruSauThue + luongTongHop.CacKhoanHoanLaiSauThue;

                #endregion

                #region Đã trả lần 1 hoặc Tạm ứng

                luongTongHop.LuongTamUng = luongCtOther?.LuongTamUng ?? 0;

                #endregion

                #region Thực nhận trong tháng hoặc lương còn lại

                luongTongHop.LuongConLai = luongTongHop.ThuNhapThucNhan - luongCtOther?.LuongTamUng ?? 0;

                #endregion

                #region Các khoản công ty phải trả khác

                luongTongHop.CacKhoanCtyPhaiTraKhac = TongCacKhoanCtyPhaiDong(luongCtCtyDong);

                #endregion

                #region Tổng chi phí công ty phải trả

                luongTongHop.TongChiPhiCtyPhaiTra =
                    luongTongHop.TongThueTncnCtyVaNld + luongTongHop.TongTienBhCtyPhaiDong +
                    luongTongHop.ThuNhapThucNhanTruocKhiBuTruThue +
                    luongTongHop.CacKhoanGiamTruSauThue - luongTongHop.CacKhoanHoanLaiSauThue +
                    luongTongHop.CacKhoanCtyPhaiTraKhac;

                #endregion

                context.LuongTongHop.Add(luongTongHop);
                context.SaveChanges();
            });
        }

        private decimal LuongOtTinhThue(LuongCtTroCapOt luongCtTroCapOt,
            List<LuongCtLoaiTroCapOt> listLuongCtLoaiTroCapOt)
        {
            decimal result = 0;

            if (luongCtTroCapOt != null)
            {
                result = luongCtTroCapOt.MucLuongHienTai * listLuongCtLoaiTroCapOt.Sum(s => s.SoGioOt);
            }

            return result;
        }

        private decimal TongLuongOt(List<LuongCtLoaiTroCapOt> listLuongCtLoaiTroCapOt)
        {
            decimal result = listLuongCtLoaiTroCapOt.Sum(s => s.MucTroCap * s.SoGioOt);

            return result;
        }

        private decimal TongTroCapCoDinh(List<LuongCtLoaiTroCapCoDinh> listLuongCtLoaiTroCapCoDinh)
        {
            decimal result = listLuongCtLoaiTroCapCoDinh.Sum(s => s.MucTroCap);

            return result;
        }

        private decimal TongTroCapKhac(List<LuongCtLoaiTroCapKhac> listLuongCtLoaiTroCapKhac)
        {
            decimal result = listLuongCtLoaiTroCapKhac.Sum(s => s.MucTroCap);

            return result;
        }

        private decimal TongGiamTruTruocThue(LuongCtGiamTruTruocThue luongCtGiamTruTruocThue)
        {
            decimal result = 0;

            if (luongCtGiamTruTruocThue != null)
            {
                result = luongCtGiamTruTruocThue.GiamTruCaNhan + luongCtGiamTruTruocThue.GiamTruNguoiPhuThuoc +
                         luongCtGiamTruTruocThue.TienDongBaoHiem + luongCtGiamTruTruocThue.GiamTruKhac;
            }

            return result;
        }

        private decimal TongThuNhapChiDuaVaoTinhThue(LuongCtThuNhapTinhThue luongCtThuNhapTinhThue)
        {
            decimal result = 0;

            if (luongCtThuNhapTinhThue != null)
            {
                result = luongCtThuNhapTinhThue.NetToGross + luongCtThuNhapTinhThue.Month13 +
                         luongCtThuNhapTinhThue.Gift + luongCtThuNhapTinhThue.Other;
            }

            return result;
        }

        private decimal TongTienBaoHiem(LuongCtBaoHiem luongCtBaoHiem)
        {
            decimal result = 0;

            if (luongCtBaoHiem != null)
            {
                result = luongCtBaoHiem.Bhxh +
                         luongCtBaoHiem.Bhyt + luongCtBaoHiem.Bhtn + luongCtBaoHiem.Bhtnnn + luongCtBaoHiem.Other;
            }

            return result;
        }

        private decimal TongCacKhoanCtyPhaiDong(LuongCtCtyDong luongCtCtyDong)
        {
            decimal result = 0;

            if (luongCtCtyDong != null)
            {
                result = luongCtCtyDong.Bhxh + luongCtCtyDong.Bhyt + luongCtCtyDong.Bhtn + luongCtCtyDong.Bhtnnn + 
                         luongCtCtyDong.KinhPhiCongDoan + luongCtCtyDong.Other + luongCtCtyDong.FundOct;
            }

            return result;
        }

        private decimal TongGiamTruSauThue(LuongCtGiamTruSauThue luongCtGiamTruSauThue)
        {
            decimal result = 0;

            if (luongCtGiamTruSauThue != null)
            {
                result = luongCtGiamTruSauThue.KinhPhiCongDoan + luongCtGiamTruSauThue.QuyetToanThueTncn +
                         luongCtGiamTruSauThue.Other;
            }

            return result;
        }

        private decimal TongCacKhoanHoanLaiSauThue(LuongCtHoanLaiSauThue luongCtHoanLaiSauThue)
        {
            decimal result = 0;

            if (luongCtHoanLaiSauThue != null)
            {
                result = luongCtHoanLaiSauThue.ThueTncn + luongCtHoanLaiSauThue.Other;
            }

            return result;
        }

        private decimal TinhMucHuongTroCapTheoNgayNghi(TroCap troCapTheoNgayNghi,
            List<ChamCong> listChamCongByEmp, List<MucHuongTheoNgayNghi> listMucHuong)
        {
            decimal result = 0;

            double ngayNghiBaoTruoc = 0;
            double ngayNghiDotXuat = 0;
            double tongNgayNghi = 0;

            listChamCongByEmp.ForEach(chamCong =>
            {
                #region Ngày nghỉ có báo trước

                if (chamCong.KyHieuVaoSang == 1 ||
                    chamCong.KyHieuVaoSang == 5 ||
                    chamCong.KyHieuVaoSang == 8) ngayNghiBaoTruoc += 0.25;

                if (chamCong.KyHieuRaSang == 1 ||
                    chamCong.KyHieuRaSang == 5 ||
                    chamCong.KyHieuRaSang == 8) ngayNghiBaoTruoc += 0.25;

                if (chamCong.KyHieuVaoChieu == 1 ||
                    chamCong.KyHieuVaoChieu == 5 ||
                    chamCong.KyHieuVaoChieu == 8) ngayNghiBaoTruoc += 0.25;

                if (chamCong.KyHieuRaChieu == 1 ||
                    chamCong.KyHieuRaChieu == 5 ||
                    chamCong.KyHieuRaChieu == 8) ngayNghiBaoTruoc += 0.25;

                #endregion

                #region Ngày nghỉ đột xuất

                if (chamCong.KyHieuVaoSang == 9) ngayNghiDotXuat += 0.25;
                if (chamCong.KyHieuRaSang == 9) ngayNghiDotXuat += 0.25;
                if (chamCong.KyHieuVaoChieu == 9) ngayNghiDotXuat += 0.25;
                if (chamCong.KyHieuRaChieu == 9) ngayNghiDotXuat += 0.25;

                #endregion
            });

            tongNgayNghi = ngayNghiBaoTruoc + ngayNghiDotXuat;
            var listMucHuongPhanTram = new List<decimal>();

            for (int i = 0; i < listMucHuong.Count; i++)
            {
                var mucHuong = listMucHuong[i];

                //Nếu số ngày bằng một số cố định
                if (mucHuong.SoNgayTu == mucHuong.SoNgayDen)
                {
                    //Nếu là Nghỉ báo trước + Nghỉ đột xuất
                    if (mucHuong.LoaiNgayNghi == null && mucHuong.SoNgayTu == (decimal)tongNgayNghi)
                    {
                        listMucHuongPhanTram.Add(mucHuong.MucHuongPhanTram);
                    }
                    //Nếu là Nghỉ báo trước
                    else if (mucHuong.LoaiNgayNghi == 1 && mucHuong.SoNgayTu == (decimal)ngayNghiBaoTruoc)
                    {
                        listMucHuongPhanTram.Add(mucHuong.MucHuongPhanTram);
                    }
                    //Nếu là Nghỉ đột xuất
                    else if (mucHuong.LoaiNgayNghi == 2 && mucHuong.SoNgayTu == (decimal)ngayNghiDotXuat)
                    {
                        listMucHuongPhanTram.Add(mucHuong.MucHuongPhanTram);
                    }
                }
                //Nếu số ngày là trong khoảng Từ -> Đến
                else if (mucHuong.SoNgayDen != null)
                {
                    //Nếu là Nghỉ báo trước + Nghỉ đột xuất
                    if (mucHuong.LoaiNgayNghi == null &&
                        mucHuong.SoNgayTu <= (decimal)tongNgayNghi &&
                        (decimal)tongNgayNghi <= mucHuong.SoNgayDen)
                    {
                        listMucHuongPhanTram.Add(mucHuong.MucHuongPhanTram);
                    }
                    //Nếu là Nghỉ báo trước
                    else if (mucHuong.LoaiNgayNghi == 1 &&
                             mucHuong.SoNgayTu <= (decimal)ngayNghiBaoTruoc &&
                             (decimal)ngayNghiBaoTruoc <= mucHuong.SoNgayDen)
                    {
                        listMucHuongPhanTram.Add(mucHuong.MucHuongPhanTram);
                    }
                    //Nếu là Nghỉ đột xuất
                    else if (mucHuong.LoaiNgayNghi == 2 &&
                             mucHuong.SoNgayTu <= (decimal)ngayNghiDotXuat &&
                             (decimal)ngayNghiDotXuat <= mucHuong.SoNgayDen)
                    {
                        listMucHuongPhanTram.Add(mucHuong.MucHuongPhanTram);
                    }
                }
                //Nếu số ngày là lớn hơn 1 số cố định
                else if (mucHuong.SoNgayDen == null)
                {
                    //Nếu là Nghỉ báo trước + Nghỉ đột xuất
                    if (mucHuong.LoaiNgayNghi == null &&
                        mucHuong.SoNgayTu < (decimal)tongNgayNghi)
                    {
                        listMucHuongPhanTram.Add(mucHuong.MucHuongPhanTram);
                    }
                    //Nếu là Nghỉ báo trước
                    else if (mucHuong.LoaiNgayNghi == 1 &&
                             mucHuong.SoNgayTu < (decimal)ngayNghiBaoTruoc)
                    {
                        listMucHuongPhanTram.Add(mucHuong.MucHuongPhanTram);
                    }
                    //Nếu là Nghỉ đột xuất
                    else if (mucHuong.LoaiNgayNghi == 2 &&
                             mucHuong.SoNgayTu < (decimal)ngayNghiDotXuat)
                    {
                        listMucHuongPhanTram.Add(mucHuong.MucHuongPhanTram);
                    }
                }
            }

            decimal mucHuongPhanTram = 0;

            //Nếu không thỏa mãn cấu hình nào thì lấy giá trị = mức trợ cấp
            if (listMucHuongPhanTram.Count == 0)
            {
                mucHuongPhanTram = 100;
            }
            //Nếu có giá trị thỏa mãn cấu hình thì
            else
            {
                mucHuongPhanTram = listMucHuongPhanTram.Min();
            }

            result = Math.Round(troCapTheoNgayNghi.MucTroCap * mucHuongPhanTram / 100, 0);

            return result;
        }

        private decimal TinhMucHuongTroCapTheoDmvs(TroCap troCapTheoDmvs, int soLanDmvs, List<MucHuongDmvs> listMucHuong)
        {
            decimal mucTroCap = 0;
            var listTienTc = new List<decimal>();

            for (int i = 0; i < listMucHuong.Count; i++)
            {
                var mucHuong = listMucHuong[i];

                //Nếu số ngày bằng một số cố định
                if (mucHuong.SoLanTu == mucHuong.SoLanDen && mucHuong.SoLanTu == soLanDmvs)
                {
                    var tienTc = TinhSoTienTroCapDmvsDuocHuong(troCapTheoDmvs.MucTroCap, mucHuong, soLanDmvs);
                    listTienTc.Add(tienTc);
                }
                //Nếu số ngày là trong khoảng Từ -> Đến
                else if (mucHuong.SoLanDen != null && mucHuong.SoLanTu <= soLanDmvs && soLanDmvs <= mucHuong.SoLanDen)
                {
                    var tienTc = TinhSoTienTroCapDmvsDuocHuong(troCapTheoDmvs.MucTroCap, mucHuong, soLanDmvs);
                    listTienTc.Add(tienTc);
                }
                //Nếu số ngày là lớn hơn 1 số cố định
                else if (mucHuong.SoLanDen == null && mucHuong.SoLanTu < soLanDmvs)
                {
                    var tienTc = TinhSoTienTroCapDmvsDuocHuong(troCapTheoDmvs.MucTroCap, mucHuong, soLanDmvs);
                    listTienTc.Add(tienTc);
                }
            }

            //Nếu không thỏa mãn cấu hình nào thì lấy giá trị = mức trợ cấp
            if (listTienTc.Count == 0)
            {
                mucTroCap = troCapTheoDmvs.MucTroCap;
            }
            //Nếu có giá trị thỏa mãn cấu hình thì
            else
            {
                mucTroCap = listTienTc.Min();
            }

            return mucTroCap;
        }

        private string GenCodePhieuLuong()
        {
            var code = "";
            var prefix = "PL";

            var listCode = context.PhieuLuong.Where(x => x.PhieuLuongCode.Contains(prefix)).Select(y => new
            {
                PhieuLuongCode = Int32.Parse(y.PhieuLuongCode.Substring(2))
            }).OrderByDescending(z => z.PhieuLuongCode).ToList();

            if (listCode.Count == 0)
            {
                code = prefix + 1.ToString("D3");
            }

            if (listCode.Count < 999)
            {
                code = prefix + (listCode.Count + 1).ToString("D3");
            }
            else
            {
                code = prefix + (listCode.Count + 1).ToString();
            }

            return code;
        }

        private string ConvertThangSangTiengAnh(int thang, bool isFull)
        {
            var result = "";

            switch (thang)
            {
                case 1:
                    result = isFull ? "January" : "Jan";
                    break;
                case 2:
                    result = isFull ? "February" : "Feb";
                    break;
                case 3:
                    result = isFull ? "March" : "Mar";
                    break;
                case 4:
                    result = isFull ? "April" : "Apr";
                    break;
                case 5:
                    result = isFull ? "May" : "May";
                    break;
                case 6:
                    result = isFull ? "June" : "Jun";
                    break;
                case 7:
                    result = isFull ? "July" : "Jul";
                    break;
                case 8:
                    result = isFull ? "August" : "Aug";
                    break;
                case 9:
                    result = isFull ? "September" : "Sep";
                    break;
                case 10:
                    result = isFull ? "October" : "Oct";
                    break;
                case 11:
                    result = isFull ? "November" : "Nov";
                    break;
                case 12:
                    result = isFull ? "December" : "Dec";
                    break;
            }

            return result;
        }

        private string CheckDuocHuongTroCap(Guid dieuKienHuongId, List<LuongCtDieuKienTroCapCoDinh> listDkTroCap)
        {
            string result = "";

            var cauHinh = listDkTroCap.FirstOrDefault(x => x.DieuKienHuongId == dieuKienHuongId);
            if (cauHinh != null)
            {
                result = cauHinh.DuDieuKien ? "Có" : "Không";
                return result;
            }

            return result;
        }

        public ExportExcelKyLuongMealAllowanceResult ExportExcelKyLuongMealAllowance(ExportExcelKyLuongMealAllowanceParameter parameter)
        {
            try
            {

                //var ExportExcelKyLuongMealAllowance = new List<EmployeeEntityModel>();
                string[] listHeader1 = {
                    "Số TT/ No.",
                    "Mã nhân viên/ Emp. Code",
                    "Họ và tên tiếng Việt/ Full name in Vietnam",
                    "LƯƠNG CƠ BẢN HĐ THỬ VIỆC/ Basic Salary in probation period",
                    "LƯƠNG CƠ BẢN KÝ HỢP ĐỒNG/  Basic Salary in Labor contract",
                    "Balance"
                };

                string[] listHeader2 = {
                    "No.",
                    "Mã nhân viên",
                    "Mã phòng",
                    "Họ tên",
                    "Được trả tiền ăn (Yes/ No)",
                    "Số tiền ăn TB/ ngày",
                    "DỰA THEO SỐ NGÀY LÀM VIỆC THỰC TẾ",
                    "Số tiền ăn được trả tháng này",
                    "Số tiền ăn trả thiếu tháng trước",
                    "Số tiền ăn trả thừa tháng trước",
                    "Số tiền ăn được trả tháng này (+), phải trả thêm tháng này (-)",
                    "Ghi chú"
                };
                string[] listHeader21 = {
                    "",
                    "",
                    "",
                    "",
                    "",
                    "600,000",
                    "",
                    "20",
                    "23",
                    "24",
                    "",
                    "",
                };

                string[] listHeader3 = {
                    "No.",
                    "Mã nhân viên",
                    "Mã phòng",
                    "Họ tên",
                    "Type of Contract",
                    "Được trả Chuyên Cần (Yes/ No)",
                    "Trợ cấp chuyên cần ngày công",
                    "",
                    "",
                    "",
                    "",
                    "Trợ cấp chuyên cần đi muộn về sớm",
                    "",
                    "",
                    "",
                    "Ghi chú",
                };
                string[] listHeader31 = {
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "Tổng ngày nghỉ đột xuất trong tháng",
                    "Tổng ngày nghỉ trong tháng",
                    "Tổng ngày làm việc thực tế trong tháng",
                    "Trợ cấp chuyên cần ngày công theo ngày làm việc thực tế",
                    "Trợ cấp chuyên cần ngày công được hưởng",
                    "Tổng số lần đi muộn về sớm",
                    "Tổng ngày làm việc trong tháng",
                    "Trợ cấp chuyên cần đi muộn về sớm",
                    "Trợ cấp chuyên cần ngày công được hưởng",
                    "",
                };
                string[] listHeader32 = {
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "1000,000",
                    "",
                    "",
                    "",
                    "500,000",
                    "",
                };

                var listHeader = new List<dynamic>();
                listHeader.Add(listHeader1);
                listHeader.Add(listHeader2);
                listHeader.Add(listHeader21);
                listHeader.Add(listHeader3);
                listHeader.Add(listHeader31);
                listHeader.Add(listHeader32);

                var ListData = new List<templateDueDateContract>();
                var ListData1 = new List<templateMealAllowance>();
                var ListData2 = new List<templateTroCapChuyenCan>();

                if (parameter.number == 1)
                {
                    for (var i = 0; i < 10; i++)
                    {
                        var obj = new templateDueDateContract();
                        obj.hoTen = "Đạt vip " + i;
                        obj.luongHopDong = 10000 + i;
                        obj.luongThuViec = 10001 + i;
                        obj.maNhanVien = "RM" + i;
                        ListData.Add(obj);
                    }
                }
                else if (parameter.number == 2)
                {
                    for (var i = 0; i < 10; i++)
                    {
                        var obj = new templateMealAllowance();
                        obj.code = "RM" + i;
                        obj.maPhong = "Phong " + i;
                        obj.name = "Dat Pro " + i;
                        obj.duocTraTienAn = "Yes" + i;
                        obj.soTienAnTB = 1 + i;
                        obj.soNgayLamViec = 1.1 + i;
                        obj.soTienAnDuocTra = 1000 + i;
                        obj.soTienAnThieu = 2000 + i;
                        obj.soTienAnThua = 3000 + i;
                        obj.sumTienAnDuocTra = 4000 + i;
                        obj.ghiChu = "Ghi chu " + i;

                        ListData1.Add(obj);
                    }
                }
                else
                {
                    for (var i = 0; i < 10; i++)
                    {
                        var obj = new templateTroCapChuyenCan();
                        obj.maNV = "RMNV" + i;
                        obj.maPhong = "Phong " + i;
                        obj.name = "Dat Pro " + i;

                        obj.typeOfContact = "CONTACT" + i;
                        obj.chuyenCan = "nO";
                        obj.ngayNghiDotXuat = 1.1 + i;
                        obj.ngayNghi = 1 + i;
                        obj.ngayLamViec = 2 + i;
                        obj.troCapTheoNgayLam = 3000 + i;
                        obj.troCapChuyenCan = 4000 + i;
                        obj.soLanDMVS = 3000 + i;
                        obj.soNgayLamViec = 3000 + i;
                        obj.troCapDMVS = 3000 + i;
                        obj.troCapChuyenCanNgayCong = 3000 + i;
                        obj.ghiChu = "Ghi chu " + i;

                        ListData2.Add(obj);
                    }
                }


                return new ExportExcelKyLuongMealAllowanceResult()
                {
                    //ExportExcelKyLuongMealAllowance = ExportExcelKyLuongMealAllowance,
                    listHeader = listHeader,
                    data = ListData,
                    data1 = ListData1,
                    data2 = ListData2,
                    StatusCode = HttpStatusCode.OK,
                    MessageCode = "Lưu thành công",
                };
            }
            catch (Exception e)
            {
                return new ExportExcelKyLuongMealAllowanceResult()
                {
                    StatusCode = HttpStatusCode.ExpectationFailed,
                    MessageCode = e.Message
                };
            }
        }
    }
}
