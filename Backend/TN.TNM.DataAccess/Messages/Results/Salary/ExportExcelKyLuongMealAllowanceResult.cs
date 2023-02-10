using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Helper;
using TN.TNM.DataAccess.Models.CauHinhNghiLe;
using TN.TNM.DataAccess.Models.Employee;

namespace TN.TNM.DataAccess.Messages.Results.Salary
{
    public class ExportExcelKyLuongMealAllowanceResult : BaseResult
    {
        public List<EmployeeEntityModel> ExportExcelKyLuongMealAllowance { get; set; }
        public List<dynamic> listHeader { get; set; }
        public List<templateDueDateContract> data { get; set; }
        public List<templateMealAllowance> data1 { get; set; }
        public List<templateTroCapChuyenCan> data2 { get; set; }
    }
}
