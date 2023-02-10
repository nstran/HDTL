using System;
using System.Collections.Generic;
using System.Text;
using TN.TNM.DataAccess.Models.DynamicColumnTable;
using TN.TNM.DataAccess.Models.Employee;
using TN.TNM.DataAccess.Models.Project;

namespace TN.TNM.DataAccess.Messages.Results.Project
{
    public class GetBaoCaoSuDungNguonLucResult : BaseResult
    {
        public List<List<DataRowModel>> ListData { get; set; }
        public List<DataHeaderModel> ListHeaderRow1 { get; set; }
        public List<DataHeaderModel> ListHeaderRow2 { get; set; }
        public List<DataHeaderModel> ListDataFooter { get; set; }
        public List<EmployeeEntityModel> ListNhanVienKhongThamGiaDuAn { get; set; }
    }
}
