using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Address;
using TN.TNM.DataAccess.Models.Asset;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Asset
{
    public class DotKiemKeDetailResult: BaseResult
    {
        public DotKiemKeEntityModel DotKiemKe { get; set; }
        public List<TrangThaiGeneral> ListTrangThaiKiemKe { get; set; }
        public List<DotKiemKeChiTietEntityModel> ListDotKiemKeChiTiet { get; set; }
        public List<CategoryEntityModel> ListPhanLoaiTaiSan { get; set; }
        public List<TrangThaiGeneral> ListHienTrangTaiSan { get; set; }
        public List<EmployeeEntityModel> ListAllNguoiKiemKe { get; set; }
        public List<ProvinceEntityModel> ListAllProvince { get; set; }
        
    }
}
