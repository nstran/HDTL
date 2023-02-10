using System;
using System.Collections.Generic;
using System.Text;

namespace TN.TNM.DataAccess.Helper
{
    public static class GeneralList
    {
        public static List<TrangThaiGeneral> GetTrangThais(string LoaiTrangThai)
        {
            switch (LoaiTrangThai)
            {
                case "DataTypeAttr":
                    return new List<TrangThaiGeneral>()
                    {
                        new TrangThaiGeneral() { Value = 1, Name = "Ký tự số" },
                        new TrangThaiGeneral() { Value = 2, Name = "Chuỗi ký tự" },
                        new TrangThaiGeneral() { Value = 3, Name = "Thời gian" },
                    };
                case "ServicePacketRole":
                    return new List<TrangThaiGeneral>()
                    {
                        new TrangThaiGeneral() { Value = 1, Name = "Nhân viên hỗ trợ" },
                        new TrangThaiGeneral() { Value = 2, Name = "Quản lý dịch vụ" },
                    };
                case "StepServicePacket":
                    return new List<TrangThaiGeneral>()
                    {
                        new TrangThaiGeneral() { Value = 1, Name = "Tạo phiếu yêu cầu dịch vụ" },
                        new TrangThaiGeneral() { Value = 2, Name = "Xác nhận thanh toán phiếu yêu cầu dịch vụ" },
                        new TrangThaiGeneral() { Value = 3, Name = "Tạo phiếu hỗ trợ dịch vụ" },
                        new TrangThaiGeneral() { Value = 4, Name = "Cập nhật trạng thái, kết quả các điểm báo cáo" },
                        new TrangThaiGeneral() { Value = 5, Name = "Xác nhận hoàn thành hỗ trợ dịch vụ" },
                        new TrangThaiGeneral() { Value = 6, Name = "Thông báo kết quả cho khách hàng" },
                        new TrangThaiGeneral() { Value = 7, Name = "Phê duyệt phiếu yêu cầu dịch vụ phát sinh" },
                    };
                case "EmployeeType":
                    return new List<TrangThaiGeneral>()
                    {
                        new TrangThaiGeneral() { Value = 1, Name = "Nhân viên" },
                        new TrangThaiGeneral() { Value = 2, Name = "Cộng tác viên" },
                    };
                case "EmployeeBenefitLevel":
                    return new List<TrangThaiGeneral>()
                    {
                        new TrangThaiGeneral() { Value = 1, Name = "Như nhân viên" },
                        new TrangThaiGeneral() { Value = 2, Name = "Hưởng theo %" },
                    };
                case "CustomerOrder": // Phiếu yêu cầu dịch vụ
                    return new List<TrangThaiGeneral>()
                    {
                        new TrangThaiGeneral() { Value = 1, Name = "Mới" },
                        new TrangThaiGeneral() { Value = 2, Name = "Quản lý phê duyệt dịch vụ phát sinh" }, // Quản lý xác nhận phát sinh
                        new TrangThaiGeneral() { Value = 3, Name = "Khách hàng phê duyệt chi phí phát sinh" }, // Khách hàng xác nhận phát sinh
                        new TrangThaiGeneral() { Value = 4, Name = "Chờ thanh toán" },
                        new TrangThaiGeneral() { Value = 5, Name = "Đã thanh toán" },
                        new TrangThaiGeneral() { Value = 6, Name = "Hủy" },
                        new TrangThaiGeneral() { Value = 7, Name = "Từ chối dịch vụ phát sinh" },
                        new TrangThaiGeneral() { Value = 8, Name = "Từ chối chi phí phát sinh" },
                        new TrangThaiGeneral() { Value = 9, Name = "Từ chối phiếu yêu cầu bổ sung" },
                        new TrangThaiGeneral() { Value = 10, Name = "Quản lý phê duyệt phiếu yêu cầu bổ sung" },
                        new TrangThaiGeneral() { Value = 11, Name = "Chờ phê duyệt" },
                        new TrangThaiGeneral() { Value = 12, Name = "Chờ phê duyệt" },
                    };
                case "OrderType": // Loại phiếu yêu cầu dịch vụ
                    return new List<TrangThaiGeneral>()
                    {
                        new TrangThaiGeneral() { Value = 1, Name = "Phiếu yêu cầu" },
                        new TrangThaiGeneral() { Value = 2, Name = "Phiếu bổ sung yêu cầu" }
                    };
                case "CustomerOrderAction": // Phiếu hỗ trợ dịch vụ
                    return new List<TrangThaiGeneral>()
                    {
                        new TrangThaiGeneral() { Value = 1, Name = "Mới" },
                        new TrangThaiGeneral() { Value = 2, Name = "Đang thực hiện" },
                        new TrangThaiGeneral() { Value = 3, Name = "Hoàn thành" },
                    };
                case "ReportPoint": // Điểm báo cáo
                    return new List<TrangThaiGeneral>()
                    {
                        new TrangThaiGeneral() { Value = 1, Name = "Mới" },
                        new TrangThaiGeneral() { Value = 2, Name = "Đang thực hiện" },
                        new TrangThaiGeneral() { Value = 3, Name = "Hoàn thành" },
                    };
                case "OrderProcess": // Trạng thái Quy trình đặt dịch vụ
                    return new List<TrangThaiGeneral>()
                    {
                        new TrangThaiGeneral() { Value = 1, Name = "Mới" },
                        new TrangThaiGeneral() { Value = 2, Name = "Đang thực hiện" },
                        new TrangThaiGeneral() { Value = 3, Name = "Hoàn thành" },
                        new TrangThaiGeneral() { Value = 4, Name = "Từ chối dịch vụ phát sinh" },
                        new TrangThaiGeneral() { Value = 5, Name = "Hủy đặt dịch vụ" },
                        new TrangThaiGeneral() { Value = 6, Name = "Khách hàng từ chối chi phí dịch vụ" },
                    };

                case "OrderProcessDetail": //Trạng thái từng bước trong Quy trình đặt dịch vụ
                    return new List<TrangThaiGeneral>()
                    {
                        new TrangThaiGeneral() { Value = 1, Name = "Mới" },
                        new TrangThaiGeneral() { Value = 2, Name = "Đang thực hiện" },
                        new TrangThaiGeneral() { Value = 3, Name = "Hoàn thành" },
                    };
                case "OrderDetailExten": // Trạng thái các dịch vụ phát sinh
                    return new List<TrangThaiGeneral>()
                    {
                        new TrangThaiGeneral() { Value = 1, Name = "Từ chối" },
                        new TrangThaiGeneral() { Value = 2, Name = "Đã phê duyệt" },
                        new TrangThaiGeneral() { Value = 3, Name = "Chờ phê duyệt" },
                    };
                case "DoiTuongApDungQuyTrinhPheDuyet":
                    return new List<TrangThaiGeneral>()
                    {
                        new TrangThaiGeneral() { Value = 1,  Name = "Cơ hội" },
                        new TrangThaiGeneral() { Value = 2,  Name = "Hồ sơ thầu" },
                        new TrangThaiGeneral() { Value = 3,  Name = "Báo giá" },
                        new TrangThaiGeneral() { Value = 4,  Name = "Hợp đồng" },
                        new TrangThaiGeneral() { Value = 5,  Name = "Đơn hàng bán" },
                        new TrangThaiGeneral() { Value = 6,  Name = "Hóa đơn" },
                        new TrangThaiGeneral() { Value = 7,  Name = "Đề xuất mua hàng" },
                        new TrangThaiGeneral() { Value = 8,  Name = "Đơn hàng mua" },
                        new TrangThaiGeneral() { Value = 9,  Name = "Đề xuất xin nghỉ" },
                        new TrangThaiGeneral() { Value = 10,  Name = "Đề xuất tăng lương" },
                        new TrangThaiGeneral() { Value = 11,  Name = "Đề xuất chức vụ" },
                        new TrangThaiGeneral() { Value = 12,  Name = "Đề xuất kế hoạch OT" },
                        new TrangThaiGeneral() { Value = 13,  Name = "Đăng ký OT" },
                        new TrangThaiGeneral() { Value = 14,  Name = "Kỳ lương" },
                        new TrangThaiGeneral() { Value = 20,  Name = "Yêu cầu cấp phát" },
                        new TrangThaiGeneral() { Value = 21,  Name = "Đề nghị tạm ứng" },
                        new TrangThaiGeneral() { Value = 22,  Name = "Đề nghị hoàn ứng" },
                        new TrangThaiGeneral() { Value = 30,  Name = "Đề xuất công tác" },
                        new TrangThaiGeneral() { Value = 31,  Name = "Phê duyệt phát sinh dịch vụ" },
                        new TrangThaiGeneral() { Value = 32,  Name = "Phê duyệt yêu cầu bổ sung" },
                    };
                default:
                    return new List<TrangThaiGeneral>();
            }
        }
        public static List<BaseType> GetDeptCode()
        {
            return new List<BaseType>()
            {
                new BaseType() { Value = 1, Name = "Cost of sales" },
                new BaseType() { Value = 2, Name = "Operations" },
                new BaseType() { Value = 3, Name = "General & Administrative" }
            };
        }

        public static List<BaseType> GetSubCode1()
        {
            return new List<BaseType>()
            {
                new BaseType() { Value = 1, Name = "COS" },
                new BaseType() { Value = 2, Name = "OPS" },
                new BaseType() { Value = 3, Name = "G&A" }
            };
        }

        public static List<BaseType> GetSubCode2()
        {
            return new List<BaseType>()
            {
                new BaseType() { Value = 1, Name = "G&A-HR" },
                new BaseType() { Value = 2, Name = "G&A-ACC" },
                new BaseType() { Value = 3, Name = "COS-3D" },
                new BaseType() { Value = 4, Name = "OPS-PM" },
                new BaseType() { Value = 5, Name = "OPS-IT" },
                new BaseType() { Value = 6, Name = "CM" },
                new BaseType() { Value = 7, Name = "HRD" },
                new BaseType() { Value = 8, Name = "G&A-AD" },
                new BaseType() { Value = 9, Name = "COS-QA" },
            };
        }

        public static List<BaseType> GetLoaiBaoHiem()
        {
            return new List<BaseType>()
            {
                new BaseType() { Value = 1, Name = "BHXH" },
                new BaseType() { Value = 2, Name = "BH Loftcare" }
            };
        }

        public static List<BaseType> GetKyNangTayNghe()
        {
            return new List<BaseType>()
            {
                new BaseType() { Value = 1, Name = "Other" },
                new BaseType() { Value = 2, Name = "Interior design" }
            };
        }
        public static List<BaseType> GetGiaTriThuocTinh()
        {
            return new List<BaseType>()
            {
                new BaseType() { Value = 1, Name = "Ký tự số" },
                new BaseType() { Value = 2, Name = "Chuỗi ký tự" },
                new BaseType() { Value = 3, Name = "Thời gian" }
            };
        }
        public static List<BaseType> GetBenefitType()
        {
            return new List<BaseType>()
            {
                new BaseType() { Value = 1, Name = "Như nhân viên" },
                new BaseType() { Value = 2, Name = "Theo %" },
            };
        }
    }
    public partial class TrangThaiGeneral
    {
        public int Value { get; set; }
        public string ValueText { get; set; }
        public string Name { get; set; }
        public bool? IsEdit { get; set; }
        public Guid? CategoryId { get; set; }
    }


    public partial class BaseType
    {
        public string Name { get; set; }
        public int Value { get; set; }
        public string Key { get; set; }
    }

    public partial class ThongKeTaiSanChartModel
    {
        public string Name { get; set; }
        public List<int> Data { get; set; }
        public string Stack { get; set; }
    }


    public partial class DataPieChartModel
    {
        public string Name { get; set; }
        public int Y { get; set; }
        public string Drilldown { get; set; }

    }

    public partial class ThongKeNhanSuChartModel
    {
        public string Name { get; set; }
        public List<int> Data { get; set; }
        public string Stack { get; set; }
    }

    public partial class DoTuoi
    {
        public string Name { get; set; }
        public int Value1 { get; set; }
        public int Value2 { get; set; }
    }
    //Model cho xuất template Due date contract
    public partial class templateDueDateContract
    {
        public string maNhanVien { get; set; }
        public string hoTen { get; set; }
        public int luongThuViec { get; set; }
        public int luongHopDong { get; set; }
    }

    //Model cho xuất template Meal Allowance
    public partial class templateMealAllowance
    {
        public string code { get; set; }
        public string maPhong { get; set; }
        public string name { get; set; }
        public string duocTraTienAn { get; set; }
        public int soTienAnTB { get; set; }
        public double soNgayLamViec { get; set; }
        public int soTienAnDuocTra { get; set; }
        public int soTienAnThieu { get; set; }
        public int soTienAnThua { get; set; }
        public int sumTienAnDuocTra { get; set; }
        public string ghiChu { get; set; }
    }

    //Model cho xuất template Trợ cấp chuyên cần
    public partial class templateTroCapChuyenCan
    {
        public string maNV { get; set; }
        public string maPhong { get; set; }
        public string name { get; set; }
        public string typeOfContact { get; set; }
        public string chuyenCan { get; set; }
        public double ngayNghiDotXuat { get; set; }
        public int ngayNghi { get; set; }
        public double ngayLamViec { get; set; }
        public int troCapTheoNgayLam { get; set; }
        public int troCapChuyenCan { get; set; }
        public int soLanDMVS { get; set; }
        public int soNgayLamViec { get; set; }
        public int troCapDMVS { get; set; }
        public int troCapChuyenCanNgayCong { get; set; }
        public string ghiChu { get; set; }
    }




}
