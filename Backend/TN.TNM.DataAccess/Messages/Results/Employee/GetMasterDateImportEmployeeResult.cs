using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Models;
using TN.TNM.DataAccess.Models.Address;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Employee
{
    public class GetMasterDateImportEmployeeResult : BaseResult
    {
        public List<CategoryEntityModel> ListCapBac { get; set; }
        public List<BaseType> ListDeptCode { get; set; }
        public List<BaseType> ListSubCode1 { get; set; }
        public List<BaseType> ListSubCode2 { get; set; }
        public List<OrganizationEntityModel> ListPhongBan { get; set; }
        public List<BaseType> ListKyNangTayNghe { get; set; }
        public List<CategoryEntityModel> ListBangCap { get; set; }
        public List<CategoryEntityModel> ListPTTD { get; set; }
        public List<CategoryEntityModel> ListKenhTd { get; set; }
        public List<ProvinceEntityModel> ListProvince { get; set; }

        public List<string> ListEmpCode { get; set; }
        public List<string> ListPhone { get; set; }
        public List<string> ListEmail { get; set; }
        public List<string> ListWorkEmail { get; set; }
        public List<string> ListCodeMayChamCong { get; set; }

        public List<EmployeeEntityModel> ListEmployeeExport { get; set; }
    }
}
