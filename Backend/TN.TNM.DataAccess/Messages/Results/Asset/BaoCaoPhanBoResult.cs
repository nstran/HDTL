using System;
using System.Collections.Generic;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Asset;
using TN.TNM.DataAccess.Models.Employee;
namespace TN.TNM.DataAccess.Messages.Results.Asset
{
    public class BaoCaoPhanBoResult : BaseResult
    {
        public List<BaoCaoPhanBoEntityModel> ListTaiSanPhanBo { get; set; }
        public CompanyConfigEntityModel CompanyConfig { get; set; }
        public List<CategoryEntityModel> ListPhanLoaiTaiSan { get; set; }
        public List<OrganizationEntityModel> ListOrganization { get; set; }
        public List<EmployeeEntityModel> ListEmployee { get; set; }
    }
}
